using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class SchemaCustomResolver
{
	public static object Resolve(object instance, params object[] parameters)
	{
		Database database = SfcResolverHelper.GetDatabase(instance);
		return database.Schemas[SfcResolverHelper.GetSchemaName(instance)];
	}

	public static object ResolveUrn(object instance, params object[] parameters)
	{
		Database database = SfcResolverHelper.GetDatabase(instance);
		return new Urn(database.Urn.ToString() + string.Format(SmoApplication.DefaultCulture, "/Schema[@Name = '{0}']", new object[1] { SfcSecureString.EscapeSquote(SfcResolverHelper.GetSchemaName(instance)) }));
	}
}
