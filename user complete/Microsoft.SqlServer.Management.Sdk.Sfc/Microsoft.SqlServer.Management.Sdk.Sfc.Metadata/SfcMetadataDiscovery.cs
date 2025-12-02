using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

public class SfcMetadataDiscovery
{
	internal struct TypeHandlePropertyNameKey
	{
		private readonly RuntimeTypeHandle typeHandle;

		private readonly string propertyName;

		internal TypeHandlePropertyNameKey(string propertyName, RuntimeTypeHandle typeHandle)
		{
			TraceHelper.Assert(propertyName != null, "PropertyName can't be null in TypeHandlePropertyNameKey");
			TraceHelper.Assert(typeHandle != null, "TypeHandle can't be null in TypeHandlePropertyNameKey");
			this.propertyName = propertyName;
			this.typeHandle = typeHandle;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is TypeHandlePropertyNameKey))
			{
				return false;
			}
			return Equals((TypeHandlePropertyNameKey)obj);
		}

		public bool Equals(TypeHandlePropertyNameKey obj)
		{
			if (obj.typeHandle.Equals(typeHandle))
			{
				return obj.propertyName.Equals(propertyName, StringComparison.Ordinal);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return typeHandle.GetHashCode() ^ propertyName.GetHashCode();
		}

		public override string ToString()
		{
			return propertyName + "," + typeHandle;
		}
	}

	private const int maximumDictionaryCount = 20000;

	private static Dictionary<Type, List<SfcMetadataRelation>> typesRelationsCache = new Dictionary<Type, List<SfcMetadataRelation>>();

	private static Dictionary<Type, List<SfcMetadataRelation>> typesKeysCache = new Dictionary<Type, List<SfcMetadataRelation>>();

	private static Dictionary<RuntimeTypeHandle, List<SfcMetadataRelation>> typesPropertiesCache = new Dictionary<RuntimeTypeHandle, List<SfcMetadataRelation>>();

	private static Dictionary<Type, List<Type>> typesReferencesCache = new Dictionary<Type, List<Type>>();

	private static Dictionary<TypeHandlePropertyNameKey, PropertyInfo> typePropertyInfosCache = new Dictionary<TypeHandlePropertyNameKey, PropertyInfo>();

	private static Dictionary<SfcMetadataRelation, List<string>> relationViewNamesCache = new Dictionary<SfcMetadataRelation, List<string>>();

	private static bool internalGraphBuilt = false;

	private Type m_type;

	private AttributeCollection m_typeAttributes;

	public Type Type => m_type;

	public string ElementTypeName
	{
		get
		{
			SfcElementTypeAttribute[] array = (SfcElementTypeAttribute[])m_type.GetCustomAttributes(typeof(SfcElementTypeAttribute), inherit: false);
			if (array.Length == 0)
			{
				return m_type.Name;
			}
			return array[0].ElementTypeName;
		}
	}

	public bool IsBrowsable
	{
		get
		{
			SfcBrowsableAttribute[] array = (SfcBrowsableAttribute[])m_type.GetCustomAttributes(typeof(SfcBrowsableAttribute), inherit: false);
			if (array.Length > 0)
			{
				return array[0].IsBrowsable;
			}
			bool result = true;
			foreach (Type item in ReferredBy)
			{
				SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(item);
				foreach (SfcMetadataRelation relation in sfcMetadataDiscovery.Relations)
				{
					if (relation.Relationship != SfcRelationship.ParentObject && relation.Relationship != SfcRelationship.Ignore && relation.Relationship != SfcRelationship.None && relation.Type == Type && !relation.IsBrowsable)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}
	}

	public AttributeCollection TypeAttributes
	{
		get
		{
			if (m_typeAttributes == null)
			{
				object[] customAttributes = m_type.GetCustomAttributes(inherit: true);
				List<Attribute> list = new List<Attribute>();
				object[] array = customAttributes;
				foreach (object obj in array)
				{
					if (obj is SfcSkuAttribute)
					{
						list.Add(obj as SfcSkuAttribute);
					}
					else if (obj is SfcVersionAttribute)
					{
						list.Add(obj as SfcVersionAttribute);
					}
					else if (obj is SfcElementTypeAttribute)
					{
						list.Add(obj as SfcElementTypeAttribute);
					}
				}
				if (list.Count == 0)
				{
					m_typeAttributes = new AttributeCollection();
				}
				else
				{
					Attribute[] array2 = new Attribute[list.Count];
					list.CopyTo(array2);
					m_typeAttributes = new AttributeCollection(array2);
				}
			}
			return m_typeAttributes;
		}
	}

	public virtual List<SfcMetadataRelation> Relations
	{
		get
		{
			List<SfcMetadataRelation> typeRelations = GetTypeRelations(Type);
			SfcMetadataRelation[] array = new SfcMetadataRelation[typeRelations.Count];
			typeRelations.CopyTo(array);
			return new List<SfcMetadataRelation>(array);
		}
	}

	internal ReadOnlyCollection<SfcMetadataRelation> ReadOnlyCollectionRelations => GetTypeRelations(Type).AsReadOnly();

	internal List<SfcMetadataRelation> InternalStorageSupported
	{
		get
		{
			List<SfcMetadataRelation> list = new List<SfcMetadataRelation>();
			foreach (SfcMetadataRelation relation in Relations)
			{
				if (relation.IsSfcProperty)
				{
					list.Add(relation);
				}
			}
			return list;
		}
	}

	internal int InternalStorageSupportedCount
	{
		get
		{
			int num = 0;
			foreach (SfcMetadataRelation relation in Relations)
			{
				if (relation.IsSfcProperty)
				{
					num++;
				}
			}
			return num;
		}
	}

	public virtual List<Type> ReferredBy
	{
		get
		{
			lock (typesReferencesCache)
			{
				new List<Type>();
				if (typesReferencesCache.ContainsKey(Type))
				{
					List<Type> list = typesReferencesCache[Type];
					Type[] array = new Type[list.Count];
					list.CopyTo(array);
					return new List<Type>(array);
				}
				InternalBuildRelationsGraph(Type);
				internalGraphBuilt = true;
				if (typesReferencesCache.ContainsKey(Type))
				{
					List<Type> list2 = typesReferencesCache[Type];
					Type[] array2 = new Type[list2.Count];
					list2.CopyTo(array2);
					return new List<Type>(array2);
				}
				return new List<Type>();
			}
		}
	}

	public virtual List<SfcMetadataRelation> Keys
	{
		get
		{
			List<SfcMetadataRelation> typeKeys = GetTypeKeys(Type);
			SfcMetadataRelation[] array = new SfcMetadataRelation[typeKeys.Count];
			typeKeys.CopyTo(array);
			return new List<SfcMetadataRelation>(array);
		}
	}

	internal ReadOnlyCollection<SfcMetadataRelation> ReadOnlyKeys => GetTypeKeys(Type).AsReadOnly();

	public virtual List<SfcMetadataRelation> Objects
	{
		get
		{
			List<SfcMetadataRelation> list = new List<SfcMetadataRelation>();
			foreach (SfcMetadataRelation relation in Relations)
			{
				foreach (Attribute relationshipAttribute in relation.RelationshipAttributes)
				{
					if (relationshipAttribute is SfcRelationshipAttribute)
					{
						list.Add(relation.MemberwiseClone() as SfcMetadataRelation);
						break;
					}
				}
			}
			return list;
		}
	}

	public virtual List<SfcMetadataRelation> Properties
	{
		get
		{
			List<SfcMetadataRelation> list = new List<SfcMetadataRelation>();
			foreach (SfcMetadataRelation relation in Relations)
			{
				foreach (Attribute relationshipAttribute in relation.RelationshipAttributes)
				{
					if (relationshipAttribute is SfcPropertyAttribute)
					{
						list.Add(relation.MemberwiseClone() as SfcMetadataRelation);
						break;
					}
				}
			}
			return list;
		}
	}

	internal ReadOnlyCollection<SfcMetadataRelation> ReadOnlyCollectionProperties
	{
		get
		{
			List<SfcMetadataRelation> value = null;
			if (!typesPropertiesCache.TryGetValue(Type.TypeHandle, out value))
			{
				lock (typesPropertiesCache)
				{
					if (!typesPropertiesCache.TryGetValue(Type.TypeHandle, out value))
					{
						value = new List<SfcMetadataRelation>();
						foreach (SfcMetadataRelation readOnlyCollectionRelation in ReadOnlyCollectionRelations)
						{
							foreach (Attribute relationshipAttribute in readOnlyCollectionRelation.RelationshipAttributes)
							{
								if (relationshipAttribute is SfcPropertyAttribute)
								{
									value.Add(readOnlyCollectionRelation);
									break;
								}
							}
						}
						typesPropertiesCache[Type.TypeHandle] = value;
					}
				}
			}
			TraceHelper.Assert(value != null, "ReadOnlyProperties return list can't be null");
			return value.AsReadOnly();
		}
	}

	public SfcMetadataDiscovery(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_type = type;
	}

	internal static List<SfcMetadataRelation> GetTypeRelations(Type sfcType)
	{
		List<SfcMetadataRelation> value = null;
		if (!typesRelationsCache.TryGetValue(sfcType, out value))
		{
			lock (typesRelationsCache)
			{
				if (!typesRelationsCache.TryGetValue(sfcType, out value))
				{
					return InternalLoadTypeRelationsInformationThroughReflection(sfcType);
				}
			}
		}
		TraceHelper.Assert(value != null, "TypeRelations list can't be null");
		return value;
	}

	internal static List<SfcMetadataRelation> GetTypeKeys(Type sfcType)
	{
		List<SfcMetadataRelation> value = null;
		if (!typesKeysCache.TryGetValue(sfcType, out value))
		{
			lock (typesKeysCache)
			{
				if (!typesKeysCache.TryGetValue(sfcType, out value))
				{
					return InternalLoadTypeKeysInformationThroughReflection(sfcType);
				}
			}
		}
		TraceHelper.Assert(value != null, "TypeKeys list can't be null");
		return value;
	}

	private static void InternalBuildRelationsGraph(Type type)
	{
		if (!internalGraphBuilt)
		{
			Type parentType = GetParentType(type);
			SfcMetadataDiscovery metadataType = new SfcMetadataDiscovery(parentType);
			typesReferencesCache[parentType] = new List<Type>();
			InternalBuildRelationsGraphRecursive(metadataType);
		}
	}

	private static void InternalBuildRelationsGraphRecursive(SfcMetadataDiscovery metadataType)
	{
		List<SfcMetadataRelation> relations = metadataType.Relations;
		foreach (SfcMetadataRelation item in relations)
		{
			if (typesReferencesCache.ContainsKey(item.Type))
			{
				List<Type> list = typesReferencesCache[item.Type];
				if (list.Contains(metadataType.Type))
				{
					continue;
				}
			}
			if (item.Relationship != SfcRelationship.None && item.Relationship != SfcRelationship.ParentObject)
			{
				List<Type> list2 = null;
				list2 = ((!typesReferencesCache.ContainsKey(item.Type)) ? new List<Type>() : typesReferencesCache[item.Type]);
				list2.Add(metadataType.Type);
				typesReferencesCache[item.Type] = list2;
				InternalBuildRelationsGraphRecursive(item);
			}
		}
	}

	private static Type GetParentType(Type childType)
	{
		return SfcRegistration.GetRegisteredDomainForType(childType).RootType;
	}

	public static List<Type> GetParentsFromType(Type childType)
	{
		List<Type> list = new List<Type>();
		PropertyInfo propertyInfo = childType.GetProperty("Parent", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? childType.GetProperty("Parent");
		if (propertyInfo == null)
		{
			return null;
		}
		object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(SfcParentAttribute), inherit: true);
		if (customAttributes != null && customAttributes.Length > 0)
		{
			object[] array = customAttributes;
			foreach (object obj in array)
			{
				if (obj is SfcParentAttribute)
				{
					SfcParentAttribute sfcParentAttribute = obj as SfcParentAttribute;
					string text = sfcParentAttribute.Parent;
					if (text.IndexOf(".") == -1)
					{
						text = childType.Namespace + "." + text;
					}
					Type objectTypeFromFullName = SfcRegistration.GetObjectTypeFromFullName(text, ignoreCase: true);
					list.Add(objectTypeFromFullName);
				}
			}
		}
		else
		{
			Type propertyType = propertyInfo.PropertyType;
			list.Add(propertyType);
		}
		return list;
	}

	public static List<string> GetUrnSkeletonsFromType(Type inputType)
	{
		List<string> list = new List<string>();
		GetUrnSkeletonsFromTypeRec(inputType, "", list);
		return list;
	}

	private static string GetUrnSuffixForType(Type type)
	{
		PropertyInfo property = type.GetProperty("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
		if (null == property)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(SfcElementTypeAttribute), inherit: false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				TraceHelper.Assert(customAttributes.Length == 1, "SfcElementType attribute should exist only once");
				SfcElementTypeAttribute sfcElementTypeAttribute = (SfcElementTypeAttribute)customAttributes[0];
				return sfcElementTypeAttribute.ElementTypeName;
			}
			return type.Name;
		}
		return property.GetValue(null, new object[0]) as string;
	}

	private static void GetUrnSkeletonsFromTypeRec(Type t, string urnFragment, List<string> urns)
	{
		List<Type> parentsFromType = GetParentsFromType(t);
		if (parentsFromType == null || t.GetInterface(typeof(ISfcDomain).FullName) != null)
		{
			string item = GetUrnSuffixForType(t) + urnFragment;
			urns.Add(item);
			return;
		}
		foreach (Type item2 in parentsFromType)
		{
			GetUrnSkeletonsFromTypeRec(item2, "/" + GetUrnSuffixForType(t) + urnFragment, urns);
		}
	}

	public static Type GetRootFromType(Type inputType)
	{
		Type type = inputType;
		List<Type> list = new List<Type>();
		int num;
		do
		{
			list.Add(type);
			List<Type> parentsFromType = GetParentsFromType(type);
			if (parentsFromType == null)
			{
				return type;
			}
			type = parentsFromType[0];
			num = list.IndexOf(type);
		}
		while (num == -1);
		return list[(num != 0) ? (num - 1) : 0];
	}

	private static List<SfcMetadataRelation> InternalLoadTypeRelationsInformationThroughReflection(Type sfcType)
	{
		List<SfcMetadataRelation> list = new List<SfcMetadataRelation>();
		Dictionary<int, SfcMetadataRelation> dictionary = new Dictionary<int, SfcMetadataRelation>();
		PropertyInfo[] properties = sfcType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			AddToPropertyInfoCache(sfcType.TypeHandle, propertyInfo.Name, propertyInfo);
			object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: true);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				continue;
			}
			Type containerType = null;
			Type type = propertyInfo.PropertyType;
			SfcCardinality cardinality = SfcCardinality.None;
			SfcRelationship sfcRelationship = SfcRelationship.None;
			SfcPropertyFlags sfcPropertyFlags = SfcPropertyFlags.None;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			int key = -1;
			bool flag5 = false;
			bool flag6 = false;
			object defaultValue = null;
			List<Attribute> list2 = new List<Attribute>();
			object[] array = customAttributes;
			foreach (object obj in array)
			{
				if (!(obj is Attribute))
				{
					continue;
				}
				Type type2 = obj.GetType();
				if (type2 == typeof(SfcObjectAttribute))
				{
					if (sfcRelationship != SfcRelationship.None || sfcPropertyFlags != SfcPropertyFlags.None || flag5 || flag6)
					{
						ConflictReporting(customAttributes, sfcType.Name, propertyInfo.Name);
					}
					SfcObjectAttribute sfcObjectAttribute = obj as SfcObjectAttribute;
					sfcRelationship = sfcObjectAttribute.Relationship;
					if (sfcRelationship == SfcRelationship.ChildContainer || sfcRelationship == SfcRelationship.ObjectContainer)
					{
						type = sfcObjectAttribute.ContainsType;
						containerType = propertyInfo.PropertyType;
					}
					cardinality = sfcObjectAttribute.Cardinality;
					list2.Add(sfcObjectAttribute);
					flag = true;
					continue;
				}
				if (type2 == typeof(SfcIgnoreAttribute))
				{
					flag = false;
					break;
				}
				if (type2 == typeof(SfcKeyAttribute))
				{
					if (flag3 || flag2)
					{
						ConflictReporting(customAttributes, sfcType.Name, propertyInfo.Name);
					}
					flag4 = true;
					key = ((SfcKeyAttribute)obj).Position;
					list2.Add((SfcKeyAttribute)obj);
					flag = true;
				}
				else if (type2 == typeof(SfcPropertyAttribute))
				{
					if (sfcRelationship != SfcRelationship.None)
					{
						ConflictReporting(customAttributes, sfcType.Name, propertyInfo.Name);
					}
					SfcPropertyAttribute sfcPropertyAttribute = obj as SfcPropertyAttribute;
					sfcPropertyFlags = sfcPropertyAttribute.Flags;
					defaultValue = ConvertDefaultValue(sfcPropertyAttribute.DefaultValue, type);
					list2.Add(sfcPropertyAttribute);
					flag = true;
				}
				else if (type2 == typeof(SfcReferenceAttribute))
				{
					if (sfcRelationship != SfcRelationship.None)
					{
						ConflictReporting(customAttributes, sfcType.Name, propertyInfo.Name);
					}
					SfcReferenceAttribute item = obj as SfcReferenceAttribute;
					list2.Add(item);
					flag5 = true;
					flag = true;
				}
				else if (type2 == typeof(SfcReferenceSelectorAttribute))
				{
					if (sfcRelationship != SfcRelationship.None)
					{
						ConflictReporting(customAttributes, sfcType.Name, propertyInfo.Name);
					}
					SfcReferenceSelectorAttribute item2 = obj as SfcReferenceSelectorAttribute;
					list2.Add(item2);
					flag5 = true;
					flag = true;
				}
				else if (type2 == typeof(SfcReferenceCollectionAttribute))
				{
					if (sfcRelationship != SfcRelationship.None)
					{
						ConflictReporting(customAttributes, sfcType.Name, propertyInfo.Name);
					}
					SfcReferenceCollectionAttribute item3 = obj as SfcReferenceCollectionAttribute;
					list2.Add(item3);
					flag6 = true;
					flag = true;
				}
				else if (type2 == typeof(SfcSkuAttribute))
				{
					if (flag4)
					{
						ConflictReporting(customAttributes, sfcType.Name, propertyInfo.Name);
					}
					list2.Add(obj as SfcSkuAttribute);
					flag3 = true;
					flag = true;
				}
				else if (type2 == typeof(SfcVersionAttribute))
				{
					if (flag4)
					{
						ConflictReporting(customAttributes, sfcType.Name, propertyInfo.Name);
					}
					list2.Add(obj as SfcVersionAttribute);
					flag2 = true;
					flag = true;
				}
				else if (type2 == typeof(SfcSerializationAdapterAttribute))
				{
					list2.Add(obj as SfcSerializationAdapterAttribute);
					flag = true;
				}
				else
				{
					list2.Add(obj as Attribute);
				}
			}
			if (flag && list2.Count != 0)
			{
				SfcMetadataRelation item4 = new SfcMetadataRelation(propertyInfo.Name, type, cardinality, sfcRelationship, containerType, sfcPropertyFlags, defaultValue, new AttributeCollection(list2.ToArray()));
				list.Add(item4);
			}
			if (flag4)
			{
				SfcMetadataRelation value = new SfcMetadataRelation(propertyInfo.Name, type, SfcCardinality.None, SfcRelationship.None, null, SfcPropertyFlags.None, new AttributeCollection(list2.ToArray()));
				dictionary.Add(key, value);
			}
		}
		typesRelationsCache[sfcType] = list;
		List<SfcMetadataRelation> list3 = new List<SfcMetadataRelation>(dictionary.Count);
		for (int k = 0; k < dictionary.Count; k++)
		{
			list3.Add(dictionary[k]);
		}
		typesKeysCache[sfcType] = list3;
		return list;
	}

	private static void ConflictReporting(object[] attributes, string typeName, string propertyName)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		foreach (object obj in attributes)
		{
			if (obj is Attribute)
			{
				if (obj.GetType() == typeof(SfcPropertyAttribute))
				{
					flag = true;
				}
				else if (obj.GetType() == typeof(SfcObjectAttribute))
				{
					flag2 = true;
				}
				else if (obj.GetType() == typeof(SfcReferenceAttribute))
				{
					flag6 = true;
				}
				else if (obj.GetType() == typeof(SfcReferenceSelectorAttribute))
				{
					flag7 = true;
				}
				else if (obj.GetType() == typeof(SfcReferenceCollectionAttribute))
				{
					flag8 = true;
				}
				else if (obj.GetType() == typeof(SfcKeyAttribute))
				{
					flag3 = true;
				}
				else if (obj.GetType() == typeof(SfcVersionAttribute))
				{
					flag5 = true;
				}
				else if (obj.GetType() == typeof(SfcSkuAttribute))
				{
					flag4 = true;
				}
			}
		}
		string message = "";
		if (flag && flag2)
		{
			message = SfcStrings.AttributeConflict("SfcPropertyAttribute", "SfcObjectAttribute", typeName, propertyName);
		}
		else if (flag2 && flag6)
		{
			message = SfcStrings.AttributeConflict("SfcObjectAttribute", "SfcReferenceAttribute", typeName, propertyName);
		}
		else if (flag2 && flag7)
		{
			message = SfcStrings.AttributeConflict("SfcObjectAttribute", "SfcReferenceSelectorAttribute", typeName, propertyName);
		}
		else if (flag2 && flag8)
		{
			message = SfcStrings.AttributeConflict("SfcObjectAttribute", "SfcReferenceCollectionAttribute", typeName, propertyName);
		}
		else if (flag3 && flag4)
		{
			message = SfcStrings.AttributeConflict("SfcKeyAttribute", "SfcSkuAttribute", typeName, propertyName);
		}
		else if (flag3 && flag5)
		{
			message = SfcStrings.AttributeConflict("SfcKeyAttribute", "SfcVersionAttribute", typeName, propertyName);
		}
		throw new SfcMetadataException(message);
	}

	private static void AddToPropertyInfoCache(RuntimeTypeHandle handle, string propertyName, PropertyInfo property)
	{
		TypeHandlePropertyNameKey key = new TypeHandlePropertyNameKey(propertyName, handle);
		if (typePropertyInfosCache.Count < 20000)
		{
			PropertyInfo value = null;
			if (!typePropertyInfosCache.TryGetValue(key, out value) || !value.DeclaringType.TypeHandle.Equals(handle))
			{
				typePropertyInfosCache[key] = property;
			}
		}
	}

	internal static bool TryGetCachedPropertyInfo(RuntimeTypeHandle typeHandle, string propertyName, out PropertyInfo pInfo)
	{
		TypeHandlePropertyNameKey key = new TypeHandlePropertyNameKey(propertyName, typeHandle);
		return typePropertyInfosCache.TryGetValue(key, out pInfo);
	}

	private static List<SfcMetadataRelation> InternalLoadTypeKeysInformationThroughReflection(Type sfcType)
	{
		Dictionary<int, SfcMetadataRelation> dictionary = new Dictionary<int, SfcMetadataRelation>();
		PropertyInfo[] properties = sfcType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: true);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				continue;
			}
			bool flag = false;
			int key = -1;
			List<Attribute> list = new List<Attribute>();
			object[] array = customAttributes;
			foreach (object obj in array)
			{
				if (obj is Attribute)
				{
					if (obj is SfcKeyAttribute)
					{
						flag = true;
						key = ((SfcKeyAttribute)obj).Position;
					}
					list.Add(obj as Attribute);
				}
			}
			if (flag)
			{
				AttributeCollection attributes = new AttributeCollection(list.ToArray());
				SfcMetadataRelation value = new SfcMetadataRelation(propertyInfo.Name, propertyInfo.PropertyType, SfcCardinality.None, SfcRelationship.None, null, SfcPropertyFlags.None, attributes);
				dictionary.Add(key, value);
			}
		}
		List<SfcMetadataRelation> list2 = new List<SfcMetadataRelation>(dictionary.Count);
		for (int k = 0; k < dictionary.Count; k++)
		{
			list2.Add(dictionary[k]);
		}
		typesKeysCache[sfcType] = list2;
		return list2;
	}

	private static object ConvertDefaultValue(string defaultValueAsString, Type propertyType)
	{
		object result = null;
		if (!SqlContext.IsAvailable && !string.IsNullOrEmpty(defaultValueAsString))
		{
			TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
			if (converter != null && converter.CanConvertFrom(typeof(string)))
			{
				result = converter.ConvertFrom(defaultValueAsString);
			}
		}
		return result;
	}

	internal static void CleanupCaches()
	{
		lock (typesRelationsCache)
		{
			typesRelationsCache.Clear();
		}
		lock (relationViewNamesCache)
		{
			relationViewNamesCache.Clear();
		}
		lock (typesKeysCache)
		{
			typesKeysCache.Clear();
		}
		lock (typesPropertiesCache)
		{
			typesPropertiesCache.Clear();
		}
		lock (typesReferencesCache)
		{
			typesReferencesCache.Clear();
		}
		lock (typePropertyInfosCache)
		{
			typePropertyInfosCache.Clear();
		}
		lock (SfcUtility.typeCache)
		{
			SfcUtility.typeCache.Clear();
		}
	}

	public virtual SfcMetadataRelation FindProperty(string propertyName)
	{
		foreach (SfcMetadataRelation relation in Relations)
		{
			if (relation.IsSfcProperty && relation.PropertyName == propertyName)
			{
				return relation.MemberwiseClone() as SfcMetadataRelation;
			}
		}
		return null;
	}
}
