namespace Microsoft.SqlServer.Management.Smo;

public enum ObjectClass
{
	Database = 0,
	ObjectOrColumn = 1,
	Schema = 3,
	User = 200,
	DatabaseRole = 201,
	ApplicationRole = 202,
	SqlAssembly = 5,
	UserDefinedType = 6,
	SecurityExpression = 8,
	XmlNamespace = 10,
	MessageType = 15,
	ServiceContract = 16,
	Service = 17,
	RemoteServiceBinding = 18,
	ServiceRoute = 19,
	FullTextCatalog = 23,
	SearchPropertyList = 31,
	SymmetricKey = 24,
	Server = 100,
	Login = 101,
	ServerPrincipal = 300,
	ServerRole = 301,
	Endpoint = 105,
	Certificate = 25,
	FullTextStopList = 29,
	AsymmetricKey = 26,
	AvailabilityGroup = 108,
	ExternalDataSource = 302,
	ExternalFileFormat = 303
}
