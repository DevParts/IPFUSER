using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("RENAME", "SEQUENCE")]
[CLSCompliant(false)]
[StateChangeEvent("CREATE_TABLE", "TABLE")]
[StateChangeEvent("ALTER_TABLE", "TABLE")]
[StateChangeEvent("RENAME", "TABLE")]
[StateChangeEvent("CREATE_VIEW", "VIEW")]
[StateChangeEvent("ALTER_VIEW", "VIEW")]
[StateChangeEvent("RENAME", "VIEW")]
[StateChangeEvent("CREATE_FUNCTION", "FUNCTION")]
[StateChangeEvent("ALTER_FUNCTION", "FUNCTION")]
[StateChangeEvent("RENAME", "FUNCTION")]
[StateChangeEvent("CREATE_PROCEDURE", "PROCEDURE")]
[StateChangeEvent("ALTER_PROCEDURE", "PROCEDURE")]
[StateChangeEvent("RENAME", "PROCEDURE")]
[StateChangeEvent("CREATE_SYNONYM", "SYNONYM")]
[StateChangeEvent("CREATE_SEQUENCE", "SEQUENCE")]
[StateChangeEvent("ALTER_SEQUENCE", "SEQUENCE")]
[StateChangeEvent("ALTER_SCHEMA", "SEQUENCE")]
[StateChangeEvent("CREATE_TYPE", "TYPE")]
[StateChangeEvent("RENAME", "TYPE")]
[StateChangeEvent("CREATE_XML_SCHEMA_COLLECTION", "XMLSCHEMACOLLECTION")]
[StateChangeEvent("ALTER_XML_SCHEMA_COLLECTION", "XMLSCHEMACOLLECTION")]
[StateChangeEvent("RENAME", "XMLSCHEMACOLLECTION")]
[StateChangeEvent("ALTER_SCHEMA", "TABLE")]
[StateChangeEvent("ALTER_SCHEMA", "VIEW")]
[StateChangeEvent("ALTER_SCHEMA", "FUNCTION")]
[StateChangeEvent("ALTER_SCHEMA", "PROCEDURE")]
[StateChangeEvent("ALTER_SCHEMA", "SYNONYM")]
[DisplayNameKey("MultipartNameName")]
[DisplayDescriptionKey("MultipartNameDesc")]
[StateChangeEvent("ALTER_SCHEMA", "XMLSCHEMACOLLECTION")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.FacetSR")]
[StateChangeEvent("ALTER_SCHEMA", "TYPE")]
public interface IMultipartNameFacet : IDmfFacet
{
	[DisplayDescriptionKey("NameDesc")]
	[DisplayNameKey("NameName")]
	string Name { get; }

	[DisplayDescriptionKey("SchemaDesc")]
	[DisplayNameKey("SchemaName")]
	string Schema { get; }
}
