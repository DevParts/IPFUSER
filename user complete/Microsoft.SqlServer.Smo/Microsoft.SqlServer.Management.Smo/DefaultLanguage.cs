using System.Globalization;

namespace Microsoft.SqlServer.Management.Smo;

public class DefaultLanguage
{
	private SqlSmoObject smoObj;

	private string parentPropertyName;

	private string lcidPropertyName;

	private string namePropertyName;

	private string name = string.Empty;

	private int lcid = -1;

	public int Lcid
	{
		get
		{
			if (!IsProperlyInitialized())
			{
				return lcid;
			}
			object value = smoObj.GetPropertyOptional(lcidPropertyName).Value;
			if (value != null)
			{
				return (int)value;
			}
			return -1;
		}
		set
		{
			SetProperty(lcidPropertyName, isLcid: true, value, withConsistencyCheck: true);
		}
	}

	public string Name
	{
		get
		{
			if (!IsProperlyInitialized())
			{
				return name;
			}
			object value = smoObj.GetPropertyOptional(namePropertyName).Value;
			if (value != null)
			{
				return (string)value;
			}
			return string.Empty;
		}
		set
		{
			SetProperty(namePropertyName, isLcid: false, value, withConsistencyCheck: true);
		}
	}

	private DefaultLanguage()
	{
	}

	internal DefaultLanguage(SqlSmoObject smoObj, string parentPropertyName)
	{
		this.smoObj = smoObj;
		this.parentPropertyName = parentPropertyName;
		lcidPropertyName = string.Format(CultureInfo.InvariantCulture, "{0}Lcid", new object[1] { this.parentPropertyName });
		namePropertyName = string.Format(CultureInfo.InvariantCulture, "{0}Name", new object[1] { this.parentPropertyName });
	}

	private void SetProperty(string propertyName, bool isLcid, object value, bool withConsistencyCheck)
	{
		if (!IsProperlyInitialized())
		{
			if (isLcid)
			{
				lcid = (int)value;
			}
			else
			{
				name = value as string;
			}
		}
		else if (withConsistencyCheck)
		{
			smoObj.Properties.SetValueWithConsistencyCheck(propertyName, value);
		}
		else
		{
			smoObj.Properties.Get(propertyName).SetValue(value);
			smoObj.Properties.Get(propertyName).SetRetrieved(retrieved: true);
		}
	}

	internal bool IsProperlyInitialized()
	{
		if (lcidPropertyName != null && namePropertyName != null)
		{
			return smoObj != null;
		}
		return false;
	}

	internal DefaultLanguage Copy(SqlSmoObject smoObj, string parentPropertyName)
	{
		DefaultLanguage defaultLanguage = new DefaultLanguage(smoObj, parentPropertyName);
		defaultLanguage.SetProperty(defaultLanguage.namePropertyName, isLcid: false, Name, withConsistencyCheck: false);
		defaultLanguage.SetProperty(defaultLanguage.lcidPropertyName, isLcid: true, Lcid, withConsistencyCheck: false);
		return defaultLanguage;
	}

	internal void VerifyBothLcidAndNameNotDirty(bool isLanguageValueNoneAllowed)
	{
		if (smoObj.IsSupportedProperty(lcidPropertyName))
		{
			Property propertyOptional = smoObj.GetPropertyOptional(lcidPropertyName);
			Property propertyOptional2 = smoObj.GetPropertyOptional(namePropertyName);
			if (propertyOptional.Dirty && (isLanguageValueNoneAllowed || (int)propertyOptional.Value >= 0) && propertyOptional2.Dirty && (isLanguageValueNoneAllowed || !string.IsNullOrEmpty(propertyOptional2.Value.ToString())))
			{
				throw new SmoException(ExceptionTemplatesImpl.MutuallyExclusiveProperties(string.Format(CultureInfo.InvariantCulture, "{0}.Lcid", new object[1] { parentPropertyName }), string.Format(CultureInfo.InvariantCulture, "{0}.Name", new object[1] { parentPropertyName })));
			}
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DefaultLanguage defaultLanguage))
		{
			return false;
		}
		if (defaultLanguage.Lcid == Lcid)
		{
			return defaultLanguage.Name == Name;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Lcid;
	}
}
