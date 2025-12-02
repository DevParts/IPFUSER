using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcSerializer
{
	private SfcCache cache;

	private List<object> instanceList;

	private SfcDependencyEngine.DependencyListEnumerator listEnumerator;

	private object rootInstance;

	private XmlWriter writer;

	private SfcDomainInfo domainInfo;

	private Dictionary<RuntimeTypeHandle, XmlSerializer> xmlSerializersCache = new Dictionary<RuntimeTypeHandle, XmlSerializer>();

	private List<object> unParentedReferences;

	public List<object> UnParentedReferences => unParentedReferences;

	public event FilterPropertyHandler FilterPropertyHandler;

	public SfcSerializer()
	{
		cache = new SfcCache();
		instanceList = new List<object>();
		unParentedReferences = new List<object>();
		listEnumerator = null;
		rootInstance = null;
	}

	public void Serialize(object instance)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance", SfcStrings.SfcNullArgumentToSerialize);
		}
		rootInstance = instance;
		domainInfo = SfcRegistration.GetRegisteredDomainForType(instance.GetType());
		if (instance is SfcInstance)
		{
			SfcInstance obj = instance as SfcInstance;
			SfcDependencyEngine sfcDependencyEngine = new SfcDependencyEngine(SfcDependencyDiscoveryMode.Full, SfcDependencyAction.Serialize);
			sfcDependencyEngine.Add(obj);
			sfcDependencyEngine.Discover();
			listEnumerator = sfcDependencyEngine.GetListEnumerator();
		}
		else
		{
			if (!(instance is IAlienObject))
			{
				throw new SfcNonSerializableTypeException(SfcStrings.SfcNonSerializableType(instance.GetType().Name));
			}
			IAlienObject alienObject = instance as IAlienObject;
			instanceList = alienObject.Discover();
		}
	}

	public void Write(XmlWriter xmlWriter)
	{
		if (xmlWriter == null)
		{
			throw new ArgumentNullException("writer", SfcStrings.SfcNullWriterToSerialize);
		}
		if (rootInstance == null)
		{
			throw new SfcSerializationException(SfcStrings.SfcInvalidWriteWithoutDiscovery);
		}
		try
		{
			writer = xmlWriter;
			WriteAllInstances();
		}
		finally
		{
			SfcMetadataDiscovery.CleanupCaches();
			writer.Close();
		}
	}

	internal void WriteInstancesDocInfo(XmlWriter docWriter, string smlUri, int version)
	{
		docWriter.WriteStartElement("docinfo");
		docWriter.WriteStartElement("aliases");
		docWriter.WriteStartElement("alias");
		docWriter.WriteRaw(smlUri);
		docWriter.WriteEndElement();
		docWriter.WriteEndElement();
		docWriter.WriteStartElement("sfc", "version", null);
		docWriter.WriteAttributeString("DomainVersion", version.ToString());
		docWriter.WriteEndElement();
		docWriter.WriteEndElement();
	}

	internal void Write(XmlWriter instanceWriter, object instance, Dictionary<string, string> namespaces)
	{
		try
		{
			string smlUri = SfcUtility.GetSmlUri(SfcUtility.GetUrn(instance), instance.GetType(), useCache: true);
			if (smlUri != null)
			{
				instanceWriter.WriteStartElement("document");
				WriteInstancesDocInfo(instanceWriter, smlUri, domainInfo.GetLogicalVersion(rootInstance));
				instanceWriter.WriteStartElement("data");
				WriteInternal(instanceWriter, instance, namespaces);
				instanceWriter.WriteEndElement();
				instanceWriter.WriteEndElement();
			}
		}
		catch (Exception innerException)
		{
			throw new SfcSerializationException(SfcStrings.SfcInvalidSerializationInstance(instance.ToString()), innerException);
		}
	}

	private void WriteAllInstances()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(domainInfo.NamespaceQualifier, $"http://schemas.microsoft.com/sqlserver/{domainInfo.NamespaceQualifier}/2007/08");
		dictionary.Add("sfc", "http://schemas.microsoft.com/sqlserver/sfc/serialization/2007/08");
		dictionary.Add("sml", "http://schemas.serviceml.org/sml/2007/02");
		dictionary.Add("xs", "http://www.w3.org/2001/XMLSchema");
		writer.WriteStartDocument();
		writer.WriteStartElement("model", "http://schemas.serviceml.org/smlif/2007/02");
		writer.WriteStartElement("identity");
		writer.WriteElementString("name", "urn:uuid:96fe1236-abf6-4a57-b54d-e9baab394fd1");
		writer.WriteElementString("baseURI", "http://documentcollection/");
		writer.WriteEndElement();
		writer.WriteStartElement("xs", "bufferSchema", dictionary["xs"]);
		writer.WriteStartElement("definitions", "http://schemas.serviceml.org/smlif/2007/02");
		writer.WriteAttributeString("xmlns", "sfc", null, dictionary["sfc"]);
		writer.WriteStartElement("document");
		WriteInstancesDocInfo(writer, "/system/schema/" + domainInfo.NamespaceQualifier, domainInfo.GetLogicalVersion(rootInstance));
		writer.WriteStartElement("data");
		writer.WriteStartElement("xs", "schema", null);
		writer.WriteAttributeString("targetNamespace", dictionary[domainInfo.NamespaceQualifier]);
		foreach (string key in dictionary.Keys)
		{
			if (key != domainInfo.NamespaceQualifier)
			{
				writer.WriteAttributeString("xmlns", key, null, dictionary[key]);
			}
		}
		writer.WriteAttributeString("elementFormDefault", "qualified");
		WriteSchemaToWriter(writer, dictionary);
		writer.WriteStartElement(domainInfo.NamespaceQualifier, "bufferData", dictionary[domainInfo.NamespaceQualifier]);
		writer.WriteStartElement("instances", "http://schemas.serviceml.org/smlif/2007/02");
		writer.WriteAttributeString("xmlns", "sfc", null, dictionary["sfc"]);
		if (rootInstance is SfcInstance)
		{
			Write(writer, rootInstance, dictionary);
			while (listEnumerator.MoveNext())
			{
				if (listEnumerator.Current.Instance != rootInstance)
				{
					Write(writer, listEnumerator.Current.Instance, dictionary);
				}
			}
		}
		else
		{
			foreach (object instance in instanceList)
			{
				Write(writer, instance, dictionary);
			}
		}
		writer.WriteEndElement();
		writer.WriteEndElement();
		writer.WriteEndElement();
		writer.WriteEndElement();
		writer.WriteEndElement();
		writer.WriteEndElement();
		writer.WriteEndElement();
		writer.WriteEndDocument();
		writer.Close();
	}

	private void WriteSchemaToWriter(XmlWriter writer, Dictionary<string, string> namespaces)
	{
		Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
		if (rootInstance is SfcInstance)
		{
			dictionary.Add(rootInstance.GetType(), 1);
			WriteSchema(writer, rootInstance.GetType(), namespaces);
			while (listEnumerator.MoveNext())
			{
				if (listEnumerator.Current.Instance != rootInstance && !dictionary.ContainsKey(listEnumerator.Current.Instance.GetType()))
				{
					dictionary.Add(listEnumerator.Current.Instance.GetType(), 1);
					WriteSchema(writer, listEnumerator.Current.Instance.GetType(), namespaces);
				}
			}
			listEnumerator.Reset();
			return;
		}
		foreach (object instance in instanceList)
		{
			if (!dictionary.ContainsKey(instance.GetType()))
			{
				dictionary.Add(instance.GetType(), 1);
				WriteSchema(writer, instance.GetType(), namespaces);
			}
		}
	}

	private void WriteInternal(XmlWriter instanceWriter, object instance, Dictionary<string, string> namespaces)
	{
		instanceWriter.WriteStartElement(domainInfo.NamespaceQualifier, instance.GetType().Name, null);
		foreach (string key in namespaces.Keys)
		{
			instanceWriter.WriteAttributeString("xmlns", key, null, namespaces[key]);
		}
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(instance.GetType());
		string text = null;
		foreach (SfcMetadataRelation readOnlyCollectionRelation in sfcMetadataDiscovery.ReadOnlyCollectionRelations)
		{
			switch (readOnlyCollectionRelation.Relationship)
			{
			case SfcRelationship.ParentObject:
				instanceWriter.WriteStartElement(domainInfo.NamespaceQualifier, "Parent", null);
				instanceWriter.WriteStartElement("sfc", "Reference", null);
				instanceWriter.WriteAttributeString("sml", "ref", null, "true");
				instanceWriter.WriteStartElement("sml", "Uri", null);
				instanceWriter.WriteRaw(text ?? (text = SfcUtility.GetSmlUri(SfcUtility.GetUrn(SfcUtility.GetParent(instance)), SfcUtility.GetParent(instance).GetType(), useCache: true)));
				instanceWriter.WriteEndElement();
				instanceWriter.WriteEndElement();
				instanceWriter.WriteEndElement();
				break;
			case SfcRelationship.ObjectContainer:
			case SfcRelationship.ChildContainer:
				{
					bool flag = true;
					foreach (Attribute relationshipAttribute in readOnlyCollectionRelation.RelationshipAttributes)
					{
						if (relationshipAttribute is SfcNonSerializableAttribute)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
					try
					{
						PropertyInfo pInfo = null;
						if (!SfcMetadataDiscovery.TryGetCachedPropertyInfo(instance.GetType().TypeHandle, readOnlyCollectionRelation.PropertyName, out pInfo))
						{
							pInfo = instance.GetType().GetProperty(readOnlyCollectionRelation.PropertyName, BindingFlags.Instance | BindingFlags.Public);
						}
						object value = pInfo.GetValue(instance, null);
						if (value == null)
						{
							break;
						}
						IEnumerator enumerator4 = ((IEnumerable)value).GetEnumerator();
						if (!enumerator4.MoveNext())
						{
							break;
						}
						instanceWriter.WriteStartElement(domainInfo.NamespaceQualifier, readOnlyCollectionRelation.PropertyName, null);
						instanceWriter.WriteStartElement("sfc", "Collection", null);
						do
						{
							object current3 = enumerator4.Current;
							try
							{
								PropertyInfo pInfo2 = null;
								if (!SfcMetadataDiscovery.TryGetCachedPropertyInfo(current3.GetType().TypeHandle, "IsSystemObject", out pInfo2))
								{
									pInfo2 = current3.GetType().GetProperty("IsSystemObject");
								}
								if (pInfo2 != null && (bool)pInfo2.GetValue(current3, null))
								{
									continue;
								}
							}
							catch (TargetInvocationException)
							{
								continue;
							}
							instanceWriter.WriteStartElement("sfc", "Reference", null);
							instanceWriter.WriteAttributeString("sml", "ref", null, "true");
							instanceWriter.WriteStartElement("sml", "Uri", null);
							instanceWriter.WriteRaw(SfcUtility.GetSmlUri(SfcUtility.GetUrn(current3), current3.GetType(), useCache: true));
							instanceWriter.WriteEndElement();
							instanceWriter.WriteEndElement();
						}
						while (enumerator4.MoveNext());
						goto IL_02f6;
					}
					catch (TargetInvocationException)
					{
					}
					break;
				}
				IL_02f6:
				instanceWriter.WriteEndElement();
				instanceWriter.WriteEndElement();
				break;
			}
		}
		foreach (SfcMetadataRelation readOnlyCollectionProperty in sfcMetadataDiscovery.ReadOnlyCollectionProperties)
		{
			foreach (Attribute relationshipAttribute2 in readOnlyCollectionProperty.RelationshipAttributes)
			{
				if (!(relationshipAttribute2 is SfcReferenceAttribute))
				{
					continue;
				}
				SfcReferenceAttribute sfcReferenceAttribute = relationshipAttribute2 as SfcReferenceAttribute;
				string text2 = null;
				try
				{
					text2 = SfcUtility.GetSmlUri(sfcReferenceAttribute.GetUrn(instance), sfcReferenceAttribute.Type, useCache: true);
				}
				catch (TargetInvocationException ex3)
				{
					if (ex3.InnerException.GetType() == typeof(PropertyNotAvailableException))
					{
						continue;
					}
					throw;
				}
				catch (SfcUnsupportedVersionException)
				{
					continue;
				}
				if (text2 != null)
				{
					instanceWriter.WriteStartElement(domainInfo.NamespaceQualifier, instance.GetType().Name + readOnlyCollectionProperty.PropertyName, null);
					instanceWriter.WriteStartElement("sfc", "Reference", null);
					instanceWriter.WriteAttributeString("sml", "ref", null, "true");
					instanceWriter.WriteStartElement("sml", "Uri", null);
					instanceWriter.WriteRaw(text2);
					instanceWriter.WriteEndElement();
					instanceWriter.WriteEndElement();
					instanceWriter.WriteEndElement();
				}
			}
		}
		ReadOnlyCollection<SfcMetadataRelation> readOnlyCollectionProperties = sfcMetadataDiscovery.ReadOnlyCollectionProperties;
		foreach (SfcMetadataRelation item in readOnlyCollectionProperties)
		{
			foreach (Attribute relationshipAttribute3 in item.RelationshipAttributes)
			{
				if (!(relationshipAttribute3 is SfcPropertyAttribute))
				{
					continue;
				}
				SfcPropertyAttribute sfcPropertyAttribute = relationshipAttribute3 as SfcPropertyAttribute;
				if ((sfcPropertyAttribute.Flags & SfcPropertyFlags.Data) == SfcPropertyFlags.Data || (sfcPropertyAttribute.Flags & SfcPropertyFlags.Computed) == SfcPropertyFlags.Computed)
				{
					continue;
				}
				Type type = null;
				object obj = null;
				if (instance is SfcInstance)
				{
					SfcInstance sfcInstance = instance as SfcInstance;
					type = sfcInstance.Properties[item.PropertyName].Type;
					if (this.FilterPropertyHandler != null)
					{
						FilterPropertyEventArgs propertyArgs = new FilterPropertyEventArgs(sfcInstance, item.PropertyName);
						obj = this.FilterPropertyHandler(this, propertyArgs);
					}
					else
					{
						obj = ((SfcInstance)instance).Properties[item.PropertyName].Value;
					}
				}
				else if (instance is IAlienObject)
				{
					IAlienObject alienObject = instance as IAlienObject;
					try
					{
						type = item.Type;
						obj = alienObject.GetPropertyValue(item.PropertyName, type);
					}
					catch (TargetInvocationException)
					{
						continue;
					}
				}
				if (obj == null)
				{
					continue;
				}
				StringBuilder stringBuilder = new StringBuilder();
				XmlWriter xmlWriter = XmlWriter.Create(stringBuilder);
				IXmlSerializationAdapter serializationAdapter = GetSerializationAdapter(item);
				if (serializationAdapter != null)
				{
					serializationAdapter.WriteXml(xmlWriter, obj);
				}
				else
				{
					XmlSerializer value2 = null;
					if (!xmlSerializersCache.TryGetValue(type.TypeHandle, out value2))
					{
						value2 = new XmlSerializer(type);
						xmlSerializersCache.Add(type.TypeHandle, value2);
					}
					if (obj.GetType().Equals(typeof(string)))
					{
						obj = SfcSecureString.XmlEscape(obj.ToString());
					}
					value2.Serialize(xmlWriter, obj);
				}
				xmlWriter.Close();
				StringReader input = new StringReader(stringBuilder.ToString());
				XmlReader xmlReader = XmlReader.Create(input);
				xmlReader.MoveToContent();
				string localName = xmlReader.LocalName;
				xmlReader.Read();
				instanceWriter.WriteStartElement(domainInfo.NamespaceQualifier, item.PropertyName, null);
				instanceWriter.WriteAttributeString("type", localName);
				do
				{
					instanceWriter.WriteNode(xmlReader, defattr: false);
				}
				while (xmlReader.IsStartElement());
				instanceWriter.WriteEndElement();
				xmlReader.Close();
			}
		}
		instanceWriter.WriteEndElement();
	}

	private void WriteSchema(XmlWriter schemaWriter, Type type, Dictionary<string, string> namespaces)
	{
		schemaWriter.WriteStartElement("xs", "element", null);
		schemaWriter.WriteAttributeString("name", type.Name);
		schemaWriter.WriteStartElement("xs", "complexType", null);
		schemaWriter.WriteStartElement("xs", "sequence", null);
		schemaWriter.WriteStartElement("xs", "any", null);
		schemaWriter.WriteAttributeString("namespace", namespaces[domainInfo.NamespaceQualifier]);
		schemaWriter.WriteAttributeString("processContents", "skip");
		schemaWriter.WriteAttributeString("minOccurs", "0");
		schemaWriter.WriteAttributeString("maxOccurs", "unbounded");
		schemaWriter.WriteEndElement();
		schemaWriter.WriteEndElement();
		schemaWriter.WriteEndElement();
		schemaWriter.WriteEndElement();
	}

	public object Deserialize(XmlReader xmlReader)
	{
		if (xmlReader == null)
		{
			throw new ArgumentNullException("reader", SfcStrings.SfcNullReaderToSerialize);
		}
		return Deserialize(xmlReader, SfcObjectState.Pending);
	}

	public object Deserialize(XmlReader xmlReader, SfcObjectState state)
	{
		object obj = null;
		string rootUri = null;
		bool flag = false;
		try
		{
			xmlReader.ReadToFollowing("definitions");
			xmlReader.ReadToFollowing("alias");
			xmlReader.ReadStartElement();
			string value = xmlReader.Value;
			value = value.Substring("/system/schema/".Length);
			string rootTypeFullName = SfcRegistration.Domains.GetDomainForNamespaceQualifier(value).RootTypeFullName;
			ISfcDomainLite sfcDomainLite = SfcRegistration.CreateObject(rootTypeFullName) as ISfcDomainLite;
			int logicalVersion = sfcDomainLite.GetLogicalVersion();
			xmlReader.ReadToFollowing("sfc:version");
			int num = int.Parse(xmlReader.GetAttribute("DomainVersion"));
			UpgradeSession upgradeSession = null;
			if (num > logicalVersion)
			{
				throw new SfcUnsupportedVersionSerializationException(SfcStrings.SfcUnsupportedVersion);
			}
			if (num < logicalVersion)
			{
				if (!(sfcDomainLite is ISfcSerializableUpgrade))
				{
					throw new SfcSerializationException(SfcStrings.SfcUnsupportedDomainUpgrade);
				}
				ISfcSerializableUpgrade sfcSerializableUpgrade = sfcDomainLite as ISfcSerializableUpgrade;
				upgradeSession = sfcSerializableUpgrade.StartSerializationUpgrade();
			}
			xmlReader.ReadToFollowing("instances");
			while (xmlReader.ReadToFollowing("document"))
			{
				xmlReader.ReadToFollowing("alias");
				xmlReader.ReadStartElement();
				string value2 = xmlReader.Value;
				xmlReader.ReadToFollowing("data");
				StringReader input = new StringReader(xmlReader.ReadInnerXml());
				XmlReader xmlReader2 = XmlReader.Create(input);
				xmlReader2.MoveToContent();
				bool flag2 = false;
				if (num < logicalVersion)
				{
					flag2 = upgradeSession.IsUpgradeRequiredOnType(xmlReader2.LocalName, num);
				}
				object instance = null;
				List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
				if (!flag2)
				{
					try
					{
						Deserialize(xmlReader2, value2, out instance, state);
					}
					catch (SfcSerializationException)
					{
						throw;
					}
					catch (Exception innerException)
					{
						throw new SfcSerializationException(SfcStrings.SfcInvalidDeserializationInstance(value2), innerException);
					}
				}
				else
				{
					List<SfcInstanceSerializedData> serializedData = new List<SfcInstanceSerializedData>();
					ParseXmlData(xmlReader2, ref serializedData, isUpgrade: true);
					list = upgradeSession.UpgradeInstance(serializedData, num, value2, cache.Instances);
					if (list != null)
					{
						foreach (KeyValuePair<string, object> item in list)
						{
							cache.Add(item.Key, item.Value, OnCollision.Fail);
						}
					}
				}
				if (!flag)
				{
					flag = true;
					if (flag2)
					{
						obj = list[0].Value;
						rootUri = list[0].Key;
					}
					else
					{
						obj = instance;
						rootUri = value2;
					}
				}
				xmlReader2.Close();
			}
			xmlReader.Close();
			if (num < logicalVersion)
			{
				upgradeSession?.PostProcessUpgrade(cache.Instances, num);
			}
		}
		catch (SfcSerializationException)
		{
			throw;
		}
		catch (Exception innerException2)
		{
			throw new SfcSerializationException(SfcStrings.SfcInvalidDeserialization, innerException2);
		}
		cache.CreateHierarchy(obj, rootUri, UnParentedReferences);
		return obj;
	}

	private void Deserialize(XmlReader reader, string instanceUri, out object instance, SfcObjectState state)
	{
		if (!reader.IsStartElement())
		{
			throw new XmlException();
		}
		string text = SfcRegistration.Domains.GetDomainForNamespaceQualifier(reader.Prefix).DomainNamespace + ".";
		Type objectTypeFromFullName = SfcRegistration.GetObjectTypeFromFullName(text + reader.LocalName);
		List<SfcInstanceSerializedData> serializedData = new List<SfcInstanceSerializedData>();
		ParseXmlData(reader, ref serializedData, isUpgrade: false);
		instance = CreateInstanceFromSerializedData(objectTypeFromFullName, instanceUri, serializedData);
		if (instance is SfcInstance)
		{
			((SfcInstance)instance).State = state;
		}
		else if (instance is IAlienObject)
		{
			((IAlienObject)instance).SetObjectState(state);
		}
		cache.Add(instanceUri, instance, OnCollision.Fail);
	}

	private void ParseXmlData(XmlReader reader, ref List<SfcInstanceSerializedData> serializedData, bool isUpgrade)
	{
		reader.ReadStartElement();
		while (reader.IsStartElement())
		{
			string localName = reader.LocalName;
			string text = null;
			bool flag = false;
			bool flag2 = false;
			if (reader.AttributeCount > 0)
			{
				text = reader.GetAttribute("type");
				flag = true;
				if (reader.IsEmptyElement)
				{
					flag2 = true;
				}
			}
			reader.ReadStartElement();
			if (localName == "Parent")
			{
				reader.ReadToFollowing("sml:Uri");
				object value = reader.ReadElementContentAsObject();
				reader.ReadEndElement();
				reader.ReadEndElement();
				SfcInstanceSerializedData item = new SfcInstanceSerializedData(SfcSerializedTypes.Parent, localName, localName, value);
				serializedData.Add(item);
			}
			else if (reader.IsStartElement())
			{
				if (reader.LocalName == "Collection")
				{
					XmlReader xmlReader = reader.ReadSubtree();
					List<string> list = new List<string>();
					while (xmlReader.ReadToFollowing("sml:Uri"))
					{
						object obj = reader.ReadElementContentAsObject();
						list.Add(obj.ToString());
					}
					SfcInstanceSerializedData item2 = new SfcInstanceSerializedData(SfcSerializedTypes.Collection, localName, localName, list);
					serializedData.Add(item2);
					if (!reader.IsEmptyElement)
					{
						reader.ReadEndElement();
					}
					else
					{
						reader.ReadStartElement();
					}
					reader.ReadEndElement();
				}
				else if (reader.LocalName == "Reference")
				{
					reader.ReadToFollowing("sml:Uri");
					object value2 = reader.ReadElementContentAsObject();
					reader.ReadEndElement();
					reader.ReadEndElement();
					SfcInstanceSerializedData item3 = new SfcInstanceSerializedData(SfcSerializedTypes.Reference, localName, localName, value2);
					serializedData.Add(item3);
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				object xmlContent = SfcUtility.GetXmlContent(reader, text, flag2);
				SfcInstanceSerializedData item4 = new SfcInstanceSerializedData(SfcSerializedTypes.Property, localName, text, xmlContent);
				serializedData.Add(item4);
				if (!flag2)
				{
					reader.ReadEndElement();
				}
			}
		}
	}

	internal object CreateInstanceFromSerializedData(Type instanceType, string instanceUri, List<SfcInstanceSerializedData> serializedData)
	{
		object obj = SfcRegistration.CreateObject(instanceType.FullName);
		if (obj == null)
		{
			throw new SfcSerializationException(SfcStrings.SfcInvalidSerializationInstance(instanceType.Name));
		}
		if (obj is IAlienRoot)
		{
			((IAlienRoot)obj).DesignModeInitialize();
		}
		if (!string.IsNullOrEmpty(instanceUri) && obj is IAlienObject)
		{
			SetParent(obj as IAlienObject, instanceUri);
		}
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(instanceType);
		foreach (SfcInstanceSerializedData serializedDatum in serializedData)
		{
			if (serializedDatum.SerializedType != SfcSerializedTypes.Property)
			{
				continue;
			}
			SfcMetadataRelation sfcMetadataRelation = null;
			foreach (SfcMetadataRelation property in sfcMetadataDiscovery.Properties)
			{
				if (property.PropertyName.Equals(serializedDatum.Name, StringComparison.Ordinal))
				{
					sfcMetadataRelation = property;
					break;
				}
			}
			if (sfcMetadataRelation == null)
			{
				throw new SfcNonSerializablePropertyException(SfcStrings.SfcNonSerializableProperty(serializedDatum.Name));
			}
			IXmlSerializationAdapter serializationAdapter = GetSerializationAdapter(sfcMetadataRelation);
			if (obj is SfcInstance)
			{
				SfcInstance sfcInstance = obj as SfcInstance;
				int num = sfcInstance.Properties.LookupID(serializedDatum.Name);
				if (num < 0)
				{
					throw new SfcNonSerializablePropertyException(SfcStrings.SfcNonSerializableProperty(serializedDatum.Name));
				}
				SfcProperty sfcProperty = sfcInstance.Properties[serializedDatum.Name];
				sfcProperty.Value = GetPropertyValueFromXmlString(serializedDatum.Value.ToString(), sfcProperty.Type, serializationAdapter);
				sfcProperty.Retrieved = true;
			}
			else
			{
				if (!(obj is IAlienObject))
				{
					continue;
				}
				IAlienObject alienObject = obj as IAlienObject;
				try
				{
					Type propertyType = alienObject.GetPropertyType(serializedDatum.Name);
					object propertyValueFromXmlString = GetPropertyValueFromXmlString(serializedDatum.Value.ToString(), propertyType, serializationAdapter);
					alienObject.SetPropertyValue(serializedDatum.Name, propertyType, propertyValueFromXmlString);
				}
				catch (TargetInvocationException ex)
				{
					if (ex.InnerException.GetType() == typeof(PropertyNotAvailableException))
					{
						continue;
					}
					throw;
				}
				catch (Exception innerException)
				{
					throw new SfcNonSerializablePropertyException(SfcStrings.SfcNonSerializableProperty(serializedDatum.Name), innerException);
				}
			}
		}
		return obj;
	}

	private static object GetPropertyValueFromXmlString(string xmlString, Type propType, IXmlSerializationAdapter serializationAdapter)
	{
		object deserializedObject;
		if (serializationAdapter != null)
		{
			XmlReader reader = XmlReader.Create(new StringReader(xmlString));
			serializationAdapter.ReadXml(reader, out deserializedObject);
		}
		else
		{
			deserializedObject = SfcUtility.GetXmlValue(xmlString, propType);
			if (deserializedObject.GetType().Equals(typeof(string)))
			{
				deserializedObject = SfcSecureString.XmlUnEscape(deserializedObject.ToString());
			}
		}
		return deserializedObject;
	}

	private void SetParent(IAlienObject instance, string instanceUri)
	{
		List<string> smlFragments = SfcCache.GetSmlFragments(instanceUri.Substring(1), smlUnEscape: false);
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(instance.GetType());
		int num = ((smlFragments[smlFragments.Count - 1] == sfcMetadataDiscovery.ElementTypeName) ? 1 : 2);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < smlFragments.Count - num; i++)
		{
			stringBuilder.Append("/");
			stringBuilder.Append(smlFragments[i]);
		}
		string text = stringBuilder.ToString();
		object value = null;
		if (!string.IsNullOrEmpty(text))
		{
			if (!cache.Instances.TryGetValue(text, out value))
			{
				throw new SfcSerializationException(SfcStrings.SfcInvalidDeserializationMissingParent(instanceUri, text));
			}
			PropertyInfo property = instance.GetType().GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public);
			if (property != null && property.CanWrite)
			{
				property.SetValue(instance, value, null);
			}
		}
	}

	private static IXmlSerializationAdapter GetSerializationAdapter(SfcMetadataRelation propertyRelation)
	{
		TraceHelper.Assert(propertyRelation != null);
		IXmlSerializationAdapter xmlSerializationAdapter = null;
		Attribute attribute = propertyRelation.RelationshipAttributes[typeof(SfcSerializationAdapterAttribute)];
		if (attribute != null)
		{
			TraceHelper.Assert(attribute is SfcSerializationAdapterAttribute);
			SfcSerializationAdapterAttribute sfcSerializationAdapterAttribute = attribute as SfcSerializationAdapterAttribute;
			try
			{
				object obj = Activator.CreateInstance(sfcSerializationAdapterAttribute.SfcSerializationAdapterType);
				xmlSerializationAdapter = obj as IXmlSerializationAdapter;
				TraceHelper.Assert(xmlSerializationAdapter != null, "Specified serialization adapter of type " + sfcSerializationAdapterAttribute.SfcSerializationAdapterType.Name + " does not implement IXmlSerializationAdapter");
			}
			catch (MissingMethodException ex)
			{
				TraceHelper.Assert(condition: false, "Serialization adapter is specified but cannot be constructed using default constructor. Caught exception: " + ex.Message);
			}
		}
		return xmlSerializationAdapter;
	}
}
