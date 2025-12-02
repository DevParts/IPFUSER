using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class ServiceBroker : SqlSmoObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1 };

		private static int[] cloudVersionCount;

		private static int sqlDwPropertyCount;

		internal static StaticMetadata[] sqlDwStaticMetadata;

		internal static StaticMetadata[] cloudStaticMetadata;

		internal static StaticMetadata[] staticMetadata;

		public override int Count
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return sqlDwPropertyCount;
					}
					int num = ((currentVersionIndex < cloudVersionCount.Length) ? currentVersionIndex : (cloudVersionCount.Length - 1));
					return cloudVersionCount[num];
				}
				int num2 = ((currentVersionIndex < versionCount.Length) ? currentVersionIndex : (versionCount.Length - 1));
				return versionCount[num2];
			}
		}

		protected override int[] VersionCount
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return new int[1] { sqlDwPropertyCount };
					}
					return cloudVersionCount;
				}
				return versionCount;
			}
		}

		internal PropertyMetadataProvider(ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
			: base(version, databaseEngineType, databaseEngineEdition)
		{
		}

		public override int PropertyNameToIDLookup(string propertyName)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return -1;
				}
				return -1;
			}
			string text;
			if ((text = propertyName) != null && text == "PolicyHealthState")
			{
				return 0;
			}
			return -1;
		}

		internal new static int[] GetVersionArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return new int[1] { sqlDwPropertyCount };
				}
				return cloudVersionCount;
			}
			return versionCount;
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata[id];
				}
				return cloudStaticMetadata[id];
			}
			return staticMetadata[id];
		}

		internal new static StaticMetadata[] GetStaticMetadataArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata;
				}
				return cloudStaticMetadata;
			}
			return staticMetadata;
		}

		static PropertyMetadataProvider()
		{
			int[] array = new int[3];
			cloudVersionCount = array;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[1]
			{
				new StaticMetadata("PolicyHealthState", expensive: false, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

	private MessageTypeCollection m_MessageTypes;

	private ServiceContractCollection m_ServiceContracts;

	private BrokerServiceCollection m_BrokerServices;

	private ServiceQueueCollection m_ServiceQueues;

	private ServiceRouteCollection m_ServiceRoutess;

	private RemoteServiceBindingCollection m_RemoteServiceBindings;

	private BrokerPriorityCollection m_BrokerPriorities;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Database Parent
	{
		get
		{
			return singletonParent as Database;
		}
		internal set
		{
			SetParentImpl(value);
		}
	}

	public static string UrnSuffix => "ServiceBroker";

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(MessageType))]
	public MessageTypeCollection MessageTypes
	{
		get
		{
			CheckObjectState();
			if (m_MessageTypes == null)
			{
				m_MessageTypes = new MessageTypeCollection(this, GetComparerFromCollation("Latin1_General_BIN"));
			}
			return m_MessageTypes;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ServiceContract))]
	public ServiceContractCollection ServiceContracts
	{
		get
		{
			CheckObjectState();
			if (m_ServiceContracts == null)
			{
				m_ServiceContracts = new ServiceContractCollection(this, GetComparerFromCollation("Latin1_General_BIN"));
			}
			return m_ServiceContracts;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(BrokerService))]
	public BrokerServiceCollection Services
	{
		get
		{
			CheckObjectState();
			if (m_BrokerServices == null)
			{
				m_BrokerServices = new BrokerServiceCollection(this, GetComparerFromCollation("Latin1_General_BIN"));
			}
			return m_BrokerServices;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ServiceQueue))]
	public ServiceQueueCollection Queues
	{
		get
		{
			CheckObjectState();
			if (m_ServiceQueues == null)
			{
				m_ServiceQueues = new ServiceQueueCollection(this);
			}
			return m_ServiceQueues;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ServiceRoute))]
	public ServiceRouteCollection Routes
	{
		get
		{
			CheckObjectState();
			if (m_ServiceRoutess == null)
			{
				m_ServiceRoutess = new ServiceRouteCollection(this);
			}
			return m_ServiceRoutess;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(RemoteServiceBinding))]
	public RemoteServiceBindingCollection RemoteServiceBindings
	{
		get
		{
			CheckObjectState();
			if (m_RemoteServiceBindings == null)
			{
				m_RemoteServiceBindings = new RemoteServiceBindingCollection(this);
			}
			return m_RemoteServiceBindings;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(BrokerPriority))]
	public BrokerPriorityCollection Priorities
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion100();
			if (m_BrokerPriorities == null)
			{
				m_BrokerPriorities = new BrokerPriorityCollection(this, GetComparerFromCollation("Latin1_General_BIN"));
			}
			return m_BrokerPriorities;
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal ServiceBroker(Database parentdb, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentdb;
		SetServerObject(parentdb.GetServerObject());
		m_comparer = parentdb.StringComparer;
		m_MessageTypes = null;
		m_ServiceContracts = null;
		m_ServiceQueues = null;
		m_BrokerServices = null;
		m_ServiceRoutess = null;
		m_BrokerPriorities = null;
	}

	internal ServiceBroker()
	{
	}

	internal override void ValidateParent(SqlSmoObject newParent)
	{
		singletonParent = newParent;
		m_comparer = newParent.StringComparer;
		SetServerObject(newParent.GetServerObject());
		this.ThrowIfNotSupported(typeof(ServiceBroker));
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	protected internal override string GetDBName()
	{
		return Parent.Name;
	}
}
