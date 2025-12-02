using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[DisplayDescriptionKey("NameDesc")]
[CLSCompliant(false)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.FacetSR")]
[DisplayNameKey("NameName")]
public interface INameFacet : IDmfFacet
{
	[DisplayNameKey("NameName")]
	[DisplayDescriptionKey("NameDesc")]
	string Name { get; }
}
