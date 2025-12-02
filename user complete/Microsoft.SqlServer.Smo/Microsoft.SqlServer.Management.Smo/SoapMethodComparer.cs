using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class SoapMethodComparer : ObjectComparerBase
{
	internal SoapMethodComparer(IComparer stringComparer)
		: base(stringComparer)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		SoapMethodKey soapMethodKey = (SoapMethodKey)obj1;
		SoapMethodKey soapMethodKey2 = obj2 as SoapMethodKey;
		if (soapMethodKey2 != null)
		{
			string x = ((soapMethodKey.Namespace != null) ? soapMethodKey.Namespace : SoapMethodCollectionBase.GetDefaultNamespace());
			string y = ((soapMethodKey2.Namespace != null) ? soapMethodKey2.Namespace : SoapMethodCollectionBase.GetDefaultNamespace());
			int num = stringComparer.Compare(x, y);
			if (num != 0)
			{
				return num;
			}
		}
		string y2 = ((soapMethodKey2 != null) ? soapMethodKey2.Name : ((SimpleObjectKey)obj2).Name);
		return stringComparer.Compare(soapMethodKey.Name, y2);
	}
}
