using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcCache
{
	private Dictionary<string, object> instances;

	private Dictionary<string, bool> deserializedStore;

	private Dictionary<string, bool> typeUrnFragmentTable;

	internal Dictionary<string, object> Instances => instances;

	public SfcCache()
	{
		instances = new Dictionary<string, object>();
		deserializedStore = new Dictionary<string, bool>();
		typeUrnFragmentTable = new Dictionary<string, bool>();
	}

	public void Add(string uri, object obj, OnCollision onCollision)
	{
		instances.Add(uri, obj);
		deserializedStore.Add(uri, value: false);
		DiscoverKeysOfTypes(obj);
	}

	private void DiscoverKeysOfTypes(object instance)
	{
		Type type = instance.GetType();
		if (SfcMetadataDiscovery.GetTypeKeys(type).Count > 0)
		{
			string elementTypeName = new SfcMetadataDiscovery(type).ElementTypeName;
			if (!typeUrnFragmentTable.ContainsKey(elementTypeName))
			{
				typeUrnFragmentTable.Add(elementTypeName, value: true);
			}
		}
	}

	private string GetContainerType(Type containedType, string relationshipName)
	{
		string result = null;
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(containedType);
		foreach (SfcMetadataRelation readOnlyCollectionRelation in sfcMetadataDiscovery.ReadOnlyCollectionRelations)
		{
			if ((readOnlyCollectionRelation.Relationship == SfcRelationship.ChildContainer || readOnlyCollectionRelation.Relationship == SfcRelationship.ObjectContainer) && relationshipName == readOnlyCollectionRelation.ElementTypeName)
			{
				result = readOnlyCollectionRelation.PropertyName;
			}
		}
		return result;
	}

	private static string GetSingletonPropertyNameFromType(Type containingType, Type singletonType)
	{
		string result = string.Empty;
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(containingType);
		foreach (SfcMetadataRelation readOnlyCollectionRelation in sfcMetadataDiscovery.ReadOnlyCollectionRelations)
		{
			if ((readOnlyCollectionRelation.Relationship == SfcRelationship.ChildObject || readOnlyCollectionRelation.Relationship == SfcRelationship.Object) && readOnlyCollectionRelation.Type == singletonType)
			{
				result = readOnlyCollectionRelation.PropertyName;
			}
		}
		return result;
	}

	internal static List<string> GetSmlFragments(string smlUri, bool smlUnEscape)
	{
		List<string> list = new List<string>();
		int i = 0;
		int startPos = 0;
		for (; i < smlUri.Length; i++)
		{
			if (smlUri[i] == '/')
			{
				list.Add(GetSmlSegment(smlUri, startPos, i, smlUnEscape));
				startPos = i + 1;
			}
			else if (smlUri[i] == '_')
			{
				i++;
				if (i >= smlUri.Length)
				{
					throw new ArgumentException("The string not properly escaped");
				}
			}
		}
		list.Add(GetSmlSegment(smlUri, startPos, i, smlUnEscape));
		return list;
	}

	private static string GetSmlSegment(string smlUri, int startPos, int sepPos, bool smlUnEscape)
	{
		if (sepPos == startPos)
		{
			return "";
		}
		string text = smlUri.Substring(startPos, sepPos - startPos);
		if (smlUnEscape)
		{
			text = SfcSecureString.SmlUnEscape(text);
		}
		return text;
	}

	private void ParseUri(string subUri, out List<string> fragments, out List<bool> typeBits)
	{
		bool flag = true;
		fragments = new List<string>();
		typeBits = new List<bool>();
		fragments = GetSmlFragments(subUri, smlUnEscape: false);
		foreach (string fragment in fragments)
		{
			if (flag)
			{
				typeBits.Add(item: true);
				if (typeUrnFragmentTable.ContainsKey(fragment))
				{
					flag = false;
				}
			}
			else
			{
				typeBits.Add(item: false);
				flag = true;
			}
		}
	}

	private bool IsParent(string possibleParent, string possibleChild)
	{
		new Regex("[^" + '_' + "]/");
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		list = GetSmlFragments(possibleParent, smlUnEscape: true);
		list2 = GetSmlFragments(possibleChild, smlUnEscape: true);
		if (list2.Count < list.Count)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != list2[i])
			{
				return false;
			}
		}
		return true;
	}

	private void AddToContainer(ObjectContainer oldContainer, ObjectContainer newContainer)
	{
		object sfcInstance = oldContainer.SfcInstance;
		string uri = oldContainer.Uri;
		_ = newContainer.Uri;
		string smlUri = uri.Substring(newContainer.Uri.Length + 1);
		List<string> smlFragments = GetSmlFragments(smlUri, smlUnEscape: true);
		if (smlFragments.Count == 2)
		{
			Dictionary<string, ObjectContainer> dictionary = new Dictionary<string, ObjectContainer>();
			dictionary.Add(smlFragments[0], new ObjectContainer(oldContainer.SfcInstance, oldContainer.Uri));
			string containerType = GetContainerType(newContainer.SfcInstance.GetType(), smlFragments[0]);
			newContainer.Collections.Add(containerType, dictionary);
		}
		else
		{
			string text = GetSingletonPropertyNameFromType(newContainer.SfcInstance.GetType(), oldContainer.SfcInstance.GetType());
			if (string.IsNullOrEmpty(text))
			{
				text = smlFragments[0];
			}
			newContainer.Children.Add(text, new ObjectContainer(oldContainer.SfcInstance, oldContainer.Uri));
		}
		if (sfcInstance is SfcInstance)
		{
			PropertyInfo property = sfcInstance.GetType().GetProperty("Parent", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			property.SetValue(oldContainer.SfcInstance, newContainer.SfcInstance, null);
		}
	}

	private void AddToContainer(string uri, ObjectContainer rootContainer, List<ObjectContainer> virtualRootContainers)
	{
		if (deserializedStore.ContainsKey(uri) && deserializedStore[uri])
		{
			return;
		}
		object obj = instances[uri];
		string uri2 = rootContainer.Uri;
		string text = uri.Substring(rootContainer.Uri.Length + 1);
		ParseUri(text, out var fragments, out var typeBits);
		ObjectContainer objectContainer = rootContainer;
		for (int i = 0; i < fragments.Count; i++)
		{
			string text2 = fragments[i];
			string text3 = "";
			bool flag = false;
			if (!((i >= fragments.Count - 1) ? typeBits[i] : typeBits[i + 1]))
			{
				text3 = fragments[++i];
				string containerType = GetContainerType(objectContainer.SfcInstance.GetType(), text2);
				if (containerType == null)
				{
					throw new SfcNonSerializableTypeException(SfcStrings.SfcNonSerializableType(objectContainer.SfcInstance.GetType().Name));
				}
				if (!objectContainer.Collections.ContainsKey(containerType))
				{
					Dictionary<string, ObjectContainer> value = new Dictionary<string, ObjectContainer>();
					objectContainer.Collections.Add(containerType, value);
				}
				if (!objectContainer.Collections[containerType].ContainsKey(text3))
				{
					objectContainer.Collections[containerType].Add(text3, new ObjectContainer(obj, uri));
				}
				objectContainer = objectContainer.Collections[containerType][text3];
			}
			else
			{
				string text4 = GetSingletonPropertyNameFromType(objectContainer.SfcInstance.GetType(), obj.GetType());
				if (string.IsNullOrEmpty(text4))
				{
					text4 = text2;
				}
				if (!objectContainer.Children.ContainsKey(text2))
				{
					ObjectContainer value2 = new ObjectContainer(obj, uri);
					objectContainer.Children.Add(text4, value2);
				}
				objectContainer = objectContainer.Children[text4];
			}
			if (i == fragments.Count - 1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(uri2);
				List<string> smlFragments = GetSmlFragments(text, smlUnEscape: false);
				int num = smlFragments.Count - 1;
				num = (typeBits[num] ? (num - 1) : (num - 2));
				for (int j = 0; j <= num; j++)
				{
					stringBuilder.Append("/");
					stringBuilder.Append(smlFragments[j]);
				}
				if (!deserializedStore[stringBuilder.ToString()])
				{
					deserializedStore[stringBuilder.ToString()] = true;
					CheckAndAddNonRootInstances(stringBuilder.ToString(), rootContainer, virtualRootContainers);
				}
				if (obj is SfcInstance)
				{
					PropertyInfo property = obj.GetType().GetProperty("Parent", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
					property.SetValue(obj, instances[stringBuilder.ToString()], null);
				}
			}
		}
	}

	private void CreateObjectModel(ObjectContainer rootContainer)
	{
		ObjectContainer objectContainer = rootContainer;
		object sfcInstance = objectContainer.SfcInstance;
		Queue<ObjectContainer> queue = new Queue<ObjectContainer>();
		queue.Enqueue(objectContainer);
		while (queue.Count > 0)
		{
			objectContainer = queue.Dequeue();
			sfcInstance = objectContainer.SfcInstance;
			foreach (string key in objectContainer.Collections.Keys)
			{
				PropertyInfo property = sfcInstance.GetType().GetProperty(key);
				object value = property.GetValue(sfcInstance, null);
				Dictionary<string, ObjectContainer> dictionary = objectContainer.Collections[key];
				MethodInfo methodInfo = null;
				foreach (ObjectContainer value2 in dictionary.Values)
				{
					if (methodInfo == null)
					{
						methodInfo = value.GetType().GetMethod("Add", new Type[1] { value2.SfcInstance.GetType() });
					}
					object[] parameters = new object[1] { value2.SfcInstance };
					methodInfo.Invoke(value, parameters);
					queue.Enqueue(value2);
				}
			}
			foreach (string key2 in objectContainer.Children.Keys)
			{
				PropertyInfo property2 = sfcInstance.GetType().GetProperty(key2);
				if (property2.CanWrite)
				{
					property2.SetValue(sfcInstance, objectContainer.Children[key2].SfcInstance, null);
				}
				queue.Enqueue(objectContainer.Children[key2]);
			}
		}
	}

	internal void CheckAndAddNonRootInstances(string uri, ObjectContainer rootContainer, List<ObjectContainer> virtualRootContainers)
	{
		if (!uri.StartsWith(rootContainer.Uri, StringComparison.Ordinal))
		{
			bool flag = false;
			ObjectContainer[] array = new ObjectContainer[virtualRootContainers.Count];
			virtualRootContainers.CopyTo(array);
			ObjectContainer[] array2 = array;
			foreach (ObjectContainer objectContainer in array2)
			{
				if (IsParent(objectContainer.Uri, uri))
				{
					AddToContainer(uri, objectContainer, virtualRootContainers);
					flag = true;
				}
				else if (IsParent(uri, objectContainer.Uri))
				{
					ObjectContainer objectContainer2 = new ObjectContainer(instances[uri], uri);
					AddToContainer(objectContainer, objectContainer2);
					virtualRootContainers.Remove(objectContainer);
					virtualRootContainers.Add(objectContainer2);
					flag = true;
				}
			}
			if (!flag)
			{
				virtualRootContainers.Add(new ObjectContainer(instances[uri], uri));
			}
		}
		else
		{
			AddToContainer(uri, rootContainer, virtualRootContainers);
		}
		deserializedStore[uri] = true;
	}

	internal void CreateHierarchy(object root, string rootUri, List<object> unParentedReferences)
	{
		ObjectContainer objectContainer = new ObjectContainer(root, rootUri);
		deserializedStore[objectContainer.Uri] = true;
		List<ObjectContainer> list = new List<ObjectContainer>();
		foreach (string key in instances.Keys)
		{
			if (!key.Equals(rootUri))
			{
				CheckAndAddNonRootInstances(key, objectContainer, list);
			}
		}
		CreateObjectModel(objectContainer);
		foreach (ObjectContainer item in list)
		{
			unParentedReferences.Add(item.SfcInstance);
			CreateObjectModel(item);
		}
	}
}
