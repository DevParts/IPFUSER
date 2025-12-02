using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IAlienObject
{
	object Resolve(string urnString);

	List<object> Discover();

	void SetPropertyValue(string propertyName, Type propertyType, object value);

	Type GetPropertyType(string propertyName);

	object GetPropertyValue(string propertyName, Type propertyType);

	void SetObjectState(SfcObjectState state);

	ISfcDomainLite GetDomainRoot();

	object GetParent();

	Urn GetUrn();
}
