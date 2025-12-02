using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class SecurityPredicateObjectKey : ObjectKeyBase
{
	protected int securityPredicateID;

	internal static readonly StringCollection fields;

	public int SecurityPredicateID
	{
		get
		{
			return securityPredicateID;
		}
		set
		{
			securityPredicateID = value;
		}
	}

	public override string UrnFilter => string.Format(SmoApplication.DefaultCulture, "@SecurityPredicateID={0}", new object[1] { securityPredicateID });

	public override bool IsNull => false;

	public SecurityPredicateObjectKey(int securityPredicateID)
	{
		this.securityPredicateID = securityPredicateID;
	}

	static SecurityPredicateObjectKey()
	{
		fields = new StringCollection();
		fields.Add("SecurityPredicateID");
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "{0}", new object[1] { SecurityPredicateID });
	}

	public override StringCollection GetFieldNames()
	{
		return fields;
	}

	public override ObjectKeyBase Clone()
	{
		return new SecurityPredicateObjectKey(SecurityPredicateID);
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new SecurityPredicateObjectComparer();
	}
}
