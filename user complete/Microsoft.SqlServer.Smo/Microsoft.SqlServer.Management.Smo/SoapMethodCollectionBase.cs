using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class SoapMethodCollectionBase : SimpleObjectCollectionBase
{
	internal SoapMethodCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new SoapMethodComparer(StringComparer));
	}

	public void Remove(string name)
	{
		Remove(new SoapMethodKey(name, GetDefaultNamespace()));
	}

	public void Remove(string name, string methodNamespace)
	{
		Remove(new SoapMethodKey(name, methodNamespace));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || attribute.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		string text = urn.GetAttribute("Namespace");
		if (text == null || text.Length == 0)
		{
			text = GetDefaultNamespace();
		}
		return new SoapMethodKey(attribute, text);
	}

	internal static string GetDefaultNamespace()
	{
		return string.Empty;
	}

	public bool Contains(string name, string methodNamespace)
	{
		return Contains(new SoapMethodKey(name, methodNamespace));
	}

	public new bool Contains(string name)
	{
		return Contains(new SoapMethodKey(name, GetDefaultNamespace()));
	}
}
