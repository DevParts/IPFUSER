using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class SmoObjectBase : ISfcValidate
{
	private object userData;

	private SqlSmoState m_state;

	internal PropertyBagState propertyBagState;

	public object UserData
	{
		get
		{
			return userData;
		}
		set
		{
			userData = value;
		}
	}

	public SqlSmoState State => m_state;

	internal virtual bool ShouldNotifyPropertyChange => false;

	internal virtual bool ShouldNotifyPropertyMetadataChange => false;

	public void SetState(SqlSmoState state)
	{
		if (m_state != state)
		{
			m_state = state;
			OnStateChanged();
		}
	}

	internal void SetState(PropertyBagState state)
	{
		propertyBagState = state;
	}

	internal virtual void ValidateProperty(Property prop, object value)
	{
	}

	internal virtual object GetPropertyDefaultValue(string propname)
	{
		return null;
	}

	internal virtual object OnPropertyMissing(string propname, bool useDefaultValue)
	{
		if (useDefaultValue)
		{
			return GetPropertyDefaultValue(propname);
		}
		return null;
	}

	internal virtual void OnPropertyChanged(string propname)
	{
	}

	internal virtual void OnPropertyMetadataChanged(string propname)
	{
	}

	internal virtual void OnStateChanged()
	{
		OnPropertyChanged("State");
	}

	[CLSCompliant(false)]
	public virtual ValidationState Validate(string methodName, params object[] arguments)
	{
		return new ValidationState();
	}
}
