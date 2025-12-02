using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class UrnTypeKey : IComparable
{
	private string uniqueUrnType;

	private ObjectOrder createOrder = ObjectOrder.uninitialized;

	internal ObjectOrder CreateOrder
	{
		get
		{
			if (createOrder.Equals(ObjectOrder.uninitialized))
			{
				createOrder = SetCreateOrder();
			}
			return createOrder;
		}
	}

	private ObjectOrder SetCreateOrder()
	{
		return uniqueUrnType switch
		{
			"unresolvedentity" => ObjectOrder.unresolvedentity, 
			"server" => ObjectOrder.server, 
			"settings" => ObjectOrder.settings, 
			"oledbprovidersettings" => ObjectOrder.oledbprovidersettings, 
			"useroptions" => ObjectOrder.useroptions, 
			"filestreamsettings" => ObjectOrder.filestreamsettings, 
			"fulltextservice" => ObjectOrder.fulltextservice, 
			"cryptographicprovider" => ObjectOrder.cryptographicprovider, 
			"credential" => ObjectOrder.credential, 
			"database" => ObjectOrder.database, 
			"login" => ObjectOrder.login, 
			"masterassembly" => ObjectOrder.masterassembly, 
			"mastercertificate" => ObjectOrder.mastercertificate, 
			"masterasymmetrickey" => ObjectOrder.masterasymmetrickey, 
			"certificatekeylogin" => ObjectOrder.certificatekeylogin, 
			"roleserver" => ObjectOrder.roleserver, 
			"serverpermission" => ObjectOrder.serverpermission, 
			"serverassociation" => ObjectOrder.serverassociation, 
			"serverownership" => ObjectOrder.serverownership, 
			"linkedserver" => ObjectOrder.linkedserver, 
			"audit" => ObjectOrder.audit, 
			"userdefinedmessage" => ObjectOrder.userdefinedmessage, 
			"httpendpoint" => ObjectOrder.httpendpoint, 
			"endpoint" => ObjectOrder.endpoint, 
			"databaseencryptionkey" => ObjectOrder.databaseencryptionkey, 
			"masterkey" => ObjectOrder.masterkey, 
			"applicationrole" => ObjectOrder.applicationrole, 
			"user" => ObjectOrder.user, 
			"userassembly" => ObjectOrder.userassembly, 
			"usercertificate" => ObjectOrder.usercertificate, 
			"userasymmetrickey" => ObjectOrder.userasymmetrickey, 
			"certificatekeyuser" => ObjectOrder.certificatekeyuser, 
			"roledatabase" => ObjectOrder.roledatabase, 
			"databasepermission" => ObjectOrder.databasepermission, 
			"databaseassociation" => ObjectOrder.databaseassociation, 
			"databaseownership" => ObjectOrder.databaseownership, 
			"sqlassembly" => ObjectOrder.sqlassembly, 
			"externalLibrary" => ObjectOrder.externalLibrary, 
			"asymmetrickey" => ObjectOrder.asymmetrickey, 
			"certificate" => ObjectOrder.certificate, 
			"symmetrickeys" => ObjectOrder.symmetrickeys, 
			"schema" => ObjectOrder.schema, 
			"defaultdatabase" => ObjectOrder.defaultdatabase, 
			"fulltextcatalog" => ObjectOrder.fulltextcatalog, 
			"fulltextstoplist" => ObjectOrder.fulltextstoplist, 
			"searchpropertylist" => ObjectOrder.searchpropertylist, 
			"searchproperty" => ObjectOrder.searchproperty, 
			"partitionfunction" => ObjectOrder.partitionfunction, 
			"partitionscheme" => ObjectOrder.partitionscheme, 
			"rule" => ObjectOrder.rule, 
			"xmlschemacollection" => ObjectOrder.xmlschemacollection, 
			"userdefineddatatype" => ObjectOrder.userdefineddatatype, 
			"userdefinedtype" => ObjectOrder.userdefinedtype, 
			"sequence" => ObjectOrder.sequence, 
			"userdefinedtabletype" => ObjectOrder.userdefinedtabletype, 
			"userdefinedaggregate" => ObjectOrder.userdefinedaggregate, 
			"storedprocedure" => ObjectOrder.storedprocedure, 
			"servicebroker" => ObjectOrder.servicebroker, 
			"messagetype" => ObjectOrder.messagetype, 
			"servicecontract" => ObjectOrder.servicecontract, 
			"servicequeue" => ObjectOrder.servicequeue, 
			"brokerservice" => ObjectOrder.brokerservice, 
			"serviceroute" => ObjectOrder.serviceroute, 
			"remoteservicebinding" => ObjectOrder.remoteservicebinding, 
			"brokerpriority" => ObjectOrder.brokerpriority, 
			"synonym" => ObjectOrder.synonym, 
			"scalarudf" => ObjectOrder.scalarudf, 
			"regulartable" => ObjectOrder.regulartable, 
			"userdefinedfunction" => ObjectOrder.userdefinedfunction, 
			"table" => ObjectOrder.table, 
			"view" => ObjectOrder.view, 
			"tableviewudf" => ObjectOrder.tableviewudf, 
			"creatingudf" => ObjectOrder.creatingudf, 
			"creatingtable" => ObjectOrder.creatingtable, 
			"creatingview" => ObjectOrder.creatingview, 
			"clusteredindex" => ObjectOrder.clusteredindex, 
			"data" => ObjectOrder.data, 
			"nonclusteredindex" => ObjectOrder.nonclusteredindex, 
			"primaryxmlindex" => ObjectOrder.primaryxmlindex, 
			"secondaryxmlindex" => ObjectOrder.secondaryxmlindex, 
			"selectivexmlindex" => ObjectOrder.selectivexmlindex, 
			"secondaryselectivexmlindex" => ObjectOrder.secondaryselectivexmlindex, 
			"index" => ObjectOrder.index, 
			"fulltextindex" => ObjectOrder.fulltextindex, 
			"defaultcolumn" => ObjectOrder.defaultcolumn, 
			"foreignkey" => ObjectOrder.foreignkey, 
			"check" => ObjectOrder.check, 
			"creatingsproc" => ObjectOrder.creatingsproc, 
			"nonschemaboundsproc" => ObjectOrder.nonschemaboundsproc, 
			"trigger" => ObjectOrder.trigger, 
			"statistic" => ObjectOrder.statistic, 
			"planguide" => ObjectOrder.planguide, 
			"databaseauditspecification" => ObjectOrder.databaseauditspecification, 
			"ddltriggerdatabase" => ObjectOrder.ddltriggerdatabase, 
			"ddltriggerdatabaseenable" => ObjectOrder.ddltriggerdatabaseenable, 
			"ddltriggerdatabasedisable" => ObjectOrder.ddltriggerdatabasedisable, 
			"extendedproperty" => ObjectOrder.extendedproperty, 
			"resourcepool" => ObjectOrder.resourcepool, 
			"externalresourcepool" => ObjectOrder.externalresourcepool, 
			"workloadgroup" => ObjectOrder.workloadgroup, 
			"resourcegovernor" => ObjectOrder.resourcegovernor, 
			"mail" => ObjectOrder.mail, 
			"mailprofile" => ObjectOrder.mailprofile, 
			"mailaccount" => ObjectOrder.mailaccount, 
			"mailserver" => ObjectOrder.mailserver, 
			"configurationvalue" => ObjectOrder.configurationvalue, 
			"job" => ObjectOrder.job, 
			"step" => ObjectOrder.step, 
			"operator" => ObjectOrder.@operator, 
			"operatorcategory" => ObjectOrder.operatorcategory, 
			"jobcategory" => ObjectOrder.jobcategory, 
			"alertcategory" => ObjectOrder.alertcategory, 
			"schedule" => ObjectOrder.schedule, 
			"targetservergroup" => ObjectOrder.targetservergroup, 
			"alert" => ObjectOrder.alert, 
			"backupdevice" => ObjectOrder.backupdevice, 
			"proxyaccount" => ObjectOrder.proxyaccount, 
			"jobserver" => ObjectOrder.jobserver, 
			"alertsystem" => ObjectOrder.alertsystem, 
			"serverauditspecification" => ObjectOrder.serverauditspecification, 
			"ddltriggerserver" => ObjectOrder.ddltriggerserver, 
			"ddltriggerserverenable" => ObjectOrder.ddltriggerserverenable, 
			"ddltriggerserverdisable" => ObjectOrder.ddltriggerserverdisable, 
			"availabilitygroup" => ObjectOrder.availabilitygroup, 
			"availabilityreplica" => ObjectOrder.availabilityreplica, 
			"availabilitydatabase" => ObjectOrder.availabilitydatabase, 
			"availabilitygrouplistener" => ObjectOrder.availabilitygrouplistener, 
			"availabilitygrouplisteneripaddress" => ObjectOrder.availabilitygrouplisteneripaddress, 
			"columnstoreindex" => ObjectOrder.columnstoreindex, 
			"clusteredcolumnstoreindex" => ObjectOrder.clusteredcolumnstoreindex, 
			"securitypolicy" => ObjectOrder.securitypolicy, 
			"securitypredicate" => ObjectOrder.securitypredicate, 
			"externaldatasource" => ObjectOrder.externaldatasource, 
			"externalfileformat" => ObjectOrder.externalfileformat, 
			"columnmasterkey" => ObjectOrder.columnmasterkey, 
			"columnencryptionkey" => ObjectOrder.columnencryptionkey, 
			"columnencryptionkeyvalue" => ObjectOrder.columnencryptionkeyvalue, 
			"querystoreoptions" => ObjectOrder.querystoreoptions, 
			"databasescopedcredential" => ObjectOrder.databasescopedcredential, 
			"databasescopedconfiguration" => ObjectOrder.databasescopedconfiguration, 
			"resumableIndex" => ObjectOrder.resumableindex, 
			_ => ObjectOrder.@default, 
		};
	}

	public UrnTypeKey(Urn urn)
	{
		uniqueUrnType = GetUniqueUrnType(urn);
	}

	private string GetUniqueUrnType(Urn urn)
	{
		try
		{
			string type = urn.Type;
			string parentUrnType = ((urn.Parent != null) ? urn.Parent.Type : string.Empty);
			return GetUniqueSmoType(type, parentUrnType);
		}
		catch (Exception innerException)
		{
			throw new SmoException("Invalid Urn", innerException);
		}
	}

	public UrnTypeKey(string urnTypeKey)
	{
		uniqueUrnType = urnTypeKey.ToLower(SmoApplication.DefaultCulture);
	}

	public UrnTypeKey(string urnType, string parentUrnType)
	{
		uniqueUrnType = GetUniqueSmoType(urnType, parentUrnType);
	}

	private string GetUniqueSmoType(string urnType, string parentUrnType)
	{
		switch (urnType.ToLower(SmoApplication.DefaultCulture))
		{
		case "default":
		case "ddltrigger":
		case "role":
			return urnType.ToLower(SmoApplication.DefaultCulture) + parentUrnType.ToLower(SmoApplication.DefaultCulture);
		default:
			return urnType.ToLower(SmoApplication.DefaultCulture);
		}
	}

	public override int GetHashCode()
	{
		return uniqueUrnType.GetHashCode();
	}

	public override string ToString()
	{
		return uniqueUrnType;
	}

	public override bool Equals(object obj)
	{
		if (obj is UrnTypeKey urnTypeKey)
		{
			return uniqueUrnType.Equals(urnTypeKey.uniqueUrnType, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public int CompareTo(object obj)
	{
		if (obj is UrnTypeKey)
		{
			UrnTypeKey urnTypeKey = (UrnTypeKey)obj;
			return CreateOrder.CompareTo(urnTypeKey.CreateOrder);
		}
		throw new ArgumentException("Object is not a UrnTypeKey.");
	}
}
