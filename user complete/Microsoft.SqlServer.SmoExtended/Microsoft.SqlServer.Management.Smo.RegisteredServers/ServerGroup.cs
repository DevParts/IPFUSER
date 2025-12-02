using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.RegisteredServers;

[Obsolete("Instead use namespace Microsoft.SqlServer.Management.RegisteredServers")]
public sealed class ServerGroup : ServerGroupBase
{
	internal class PropertyMetadataProvider : Microsoft.SqlServer.Management.Smo.PropertyMetadataProvider
	{
		internal static StaticMetadata[] staticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("Description", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Path", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ServerType", expensive: false, readOnly: true, typeof(Guid))
		};

		public override int Count => 3;

		public override int PropertyNameToIDLookup(string propertyName)
		{
			return propertyName switch
			{
				"Description" => 0, 
				"Path" => 1, 
				"ServerType" => 2, 
				_ => -1, 
			};
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			return staticMetadata[id];
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public ServerGroup Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as ServerGroup;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Description
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Description");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Description", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Path => (string)base.Properties.GetValueWithNullReplacement("Path");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public Guid ServerType => (Guid)base.Properties.GetValueWithNullReplacement("ServerType");

	public override Urn Urn => new Urn(string.Format(SmoApplication.DefaultCulture, "{0}/{1}[@Path='{2}' and @Name='{3}']", Microsoft.SqlServer.Management.Smo.RegisteredServers.ServerType.UrnSuffix, UrnSuffix, Urn.EscapeString(base.ParentColl.ParentInstance.CollectionPath), Urn.EscapeString(base.Name)));

	protected internal override Urn UrnSkeleton => new Urn(string.Format(SmoApplication.DefaultCulture, "{0}/{1}", new object[2]
	{
		Microsoft.SqlServer.Management.Smo.RegisteredServers.ServerType.UrnSuffix,
		UrnSuffix
	}));

	internal static string UrnSuffix => "ServerGroup";

	public ServerGroup()
	{
	}

	public ServerGroup(ServerGroup serverGroup, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = serverGroup;
	}

	internal override Microsoft.SqlServer.Management.Smo.PropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider();
	}

	public ServerGroup(string name)
		: base(SqlServerRegistrations.ServerGroups, name)
	{
	}

	internal ServerGroup(RegSvrCollectionBase parentColl, string name)
		: base(parentColl, name)
	{
	}
}
