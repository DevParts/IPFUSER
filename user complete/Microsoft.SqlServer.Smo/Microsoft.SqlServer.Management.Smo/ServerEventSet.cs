using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerEventSet : EventSetBase
{
	public override int NumberOfElements => 210;

	public bool AddRoleMember
	{
		get
		{
			return base.Storage[0];
		}
		set
		{
			base.Storage[0] = value;
		}
	}

	public bool AddSensitivityClassification
	{
		get
		{
			return base.Storage[1];
		}
		set
		{
			base.Storage[1] = value;
		}
	}

	public bool AddServerRoleMember
	{
		get
		{
			return base.Storage[2];
		}
		set
		{
			base.Storage[2] = value;
		}
	}

	public bool AddSignature
	{
		get
		{
			return base.Storage[3];
		}
		set
		{
			base.Storage[3] = value;
		}
	}

	public bool AddSignatureSchemaObject
	{
		get
		{
			return base.Storage[4];
		}
		set
		{
			base.Storage[4] = value;
		}
	}

	public bool AlterApplicationRole
	{
		get
		{
			return base.Storage[5];
		}
		set
		{
			base.Storage[5] = value;
		}
	}

	public bool AlterAssembly
	{
		get
		{
			return base.Storage[6];
		}
		set
		{
			base.Storage[6] = value;
		}
	}

	public bool AlterAsymmetricKey
	{
		get
		{
			return base.Storage[7];
		}
		set
		{
			base.Storage[7] = value;
		}
	}

	public bool AlterAudit
	{
		get
		{
			return base.Storage[8];
		}
		set
		{
			base.Storage[8] = value;
		}
	}

	public bool AlterAuthorizationDatabase
	{
		get
		{
			return base.Storage[9];
		}
		set
		{
			base.Storage[9] = value;
		}
	}

	public bool AlterAuthorizationServer
	{
		get
		{
			return base.Storage[10];
		}
		set
		{
			base.Storage[10] = value;
		}
	}

	public bool AlterAvailabilityGroup
	{
		get
		{
			return base.Storage[11];
		}
		set
		{
			base.Storage[11] = value;
		}
	}

	public bool AlterBrokerPriority
	{
		get
		{
			return base.Storage[12];
		}
		set
		{
			base.Storage[12] = value;
		}
	}

	public bool AlterCertificate
	{
		get
		{
			return base.Storage[13];
		}
		set
		{
			base.Storage[13] = value;
		}
	}

	public bool AlterColumnEncryptionKey
	{
		get
		{
			return base.Storage[14];
		}
		set
		{
			base.Storage[14] = value;
		}
	}

	public bool AlterCredential
	{
		get
		{
			return base.Storage[15];
		}
		set
		{
			base.Storage[15] = value;
		}
	}

	public bool AlterCryptographicProvider
	{
		get
		{
			return base.Storage[16];
		}
		set
		{
			base.Storage[16] = value;
		}
	}

	public bool AlterDatabase
	{
		get
		{
			return base.Storage[17];
		}
		set
		{
			base.Storage[17] = value;
		}
	}

	public bool AlterDatabaseAuditSpecification
	{
		get
		{
			return base.Storage[18];
		}
		set
		{
			base.Storage[18] = value;
		}
	}

	public bool AlterDatabaseEncryptionKey
	{
		get
		{
			return base.Storage[19];
		}
		set
		{
			base.Storage[19] = value;
		}
	}

	public bool AlterDatabaseScopedConfiguration
	{
		get
		{
			return base.Storage[20];
		}
		set
		{
			base.Storage[20] = value;
		}
	}

	public bool AlterEndpoint
	{
		get
		{
			return base.Storage[21];
		}
		set
		{
			base.Storage[21] = value;
		}
	}

	public bool AlterEventSession
	{
		get
		{
			return base.Storage[22];
		}
		set
		{
			base.Storage[22] = value;
		}
	}

	public bool AlterExtendedProperty
	{
		get
		{
			return base.Storage[23];
		}
		set
		{
			base.Storage[23] = value;
		}
	}

	public bool AlterExternalLibrary
	{
		get
		{
			return base.Storage[24];
		}
		set
		{
			base.Storage[24] = value;
		}
	}

	public bool AlterExternalResourcePool
	{
		get
		{
			return base.Storage[25];
		}
		set
		{
			base.Storage[25] = value;
		}
	}

	public bool AlterFulltextCatalog
	{
		get
		{
			return base.Storage[26];
		}
		set
		{
			base.Storage[26] = value;
		}
	}

	public bool AlterFulltextIndex
	{
		get
		{
			return base.Storage[27];
		}
		set
		{
			base.Storage[27] = value;
		}
	}

	public bool AlterFulltextStoplist
	{
		get
		{
			return base.Storage[28];
		}
		set
		{
			base.Storage[28] = value;
		}
	}

	public bool AlterFunction
	{
		get
		{
			return base.Storage[29];
		}
		set
		{
			base.Storage[29] = value;
		}
	}

	public bool AlterIndex
	{
		get
		{
			return base.Storage[30];
		}
		set
		{
			base.Storage[30] = value;
		}
	}

	public bool AlterInstance
	{
		get
		{
			return base.Storage[31];
		}
		set
		{
			base.Storage[31] = value;
		}
	}

	public bool AlterLinkedServer
	{
		get
		{
			return base.Storage[32];
		}
		set
		{
			base.Storage[32] = value;
		}
	}

	public bool AlterLogin
	{
		get
		{
			return base.Storage[33];
		}
		set
		{
			base.Storage[33] = value;
		}
	}

	public bool AlterMasterKey
	{
		get
		{
			return base.Storage[34];
		}
		set
		{
			base.Storage[34] = value;
		}
	}

	public bool AlterMessage
	{
		get
		{
			return base.Storage[35];
		}
		set
		{
			base.Storage[35] = value;
		}
	}

	public bool AlterMessageType
	{
		get
		{
			return base.Storage[36];
		}
		set
		{
			base.Storage[36] = value;
		}
	}

	public bool AlterPartitionFunction
	{
		get
		{
			return base.Storage[37];
		}
		set
		{
			base.Storage[37] = value;
		}
	}

	public bool AlterPartitionScheme
	{
		get
		{
			return base.Storage[38];
		}
		set
		{
			base.Storage[38] = value;
		}
	}

	public bool AlterPlanGuide
	{
		get
		{
			return base.Storage[39];
		}
		set
		{
			base.Storage[39] = value;
		}
	}

	public bool AlterProcedure
	{
		get
		{
			return base.Storage[40];
		}
		set
		{
			base.Storage[40] = value;
		}
	}

	public bool AlterQueue
	{
		get
		{
			return base.Storage[41];
		}
		set
		{
			base.Storage[41] = value;
		}
	}

	public bool AlterRemoteServer
	{
		get
		{
			return base.Storage[42];
		}
		set
		{
			base.Storage[42] = value;
		}
	}

	public bool AlterRemoteServiceBinding
	{
		get
		{
			return base.Storage[43];
		}
		set
		{
			base.Storage[43] = value;
		}
	}

	public bool AlterResourceGovernorConfig
	{
		get
		{
			return base.Storage[44];
		}
		set
		{
			base.Storage[44] = value;
		}
	}

	public bool AlterResourcePool
	{
		get
		{
			return base.Storage[45];
		}
		set
		{
			base.Storage[45] = value;
		}
	}

	public bool AlterRole
	{
		get
		{
			return base.Storage[46];
		}
		set
		{
			base.Storage[46] = value;
		}
	}

	public bool AlterRoute
	{
		get
		{
			return base.Storage[47];
		}
		set
		{
			base.Storage[47] = value;
		}
	}

	public bool AlterSchema
	{
		get
		{
			return base.Storage[48];
		}
		set
		{
			base.Storage[48] = value;
		}
	}

	public bool AlterSearchPropertyList
	{
		get
		{
			return base.Storage[49];
		}
		set
		{
			base.Storage[49] = value;
		}
	}

	public bool AlterSecurityPolicy
	{
		get
		{
			return base.Storage[50];
		}
		set
		{
			base.Storage[50] = value;
		}
	}

	public bool AlterSequence
	{
		get
		{
			return base.Storage[51];
		}
		set
		{
			base.Storage[51] = value;
		}
	}

	public bool AlterServerAudit
	{
		get
		{
			return base.Storage[52];
		}
		set
		{
			base.Storage[52] = value;
		}
	}

	public bool AlterServerAuditSpecification
	{
		get
		{
			return base.Storage[53];
		}
		set
		{
			base.Storage[53] = value;
		}
	}

	public bool AlterServerConfiguration
	{
		get
		{
			return base.Storage[54];
		}
		set
		{
			base.Storage[54] = value;
		}
	}

	public bool AlterServerRole
	{
		get
		{
			return base.Storage[55];
		}
		set
		{
			base.Storage[55] = value;
		}
	}

	public bool AlterService
	{
		get
		{
			return base.Storage[56];
		}
		set
		{
			base.Storage[56] = value;
		}
	}

	public bool AlterServiceMasterKey
	{
		get
		{
			return base.Storage[57];
		}
		set
		{
			base.Storage[57] = value;
		}
	}

	public bool AlterSymmetricKey
	{
		get
		{
			return base.Storage[58];
		}
		set
		{
			base.Storage[58] = value;
		}
	}

	public bool AlterTable
	{
		get
		{
			return base.Storage[59];
		}
		set
		{
			base.Storage[59] = value;
		}
	}

	public bool AlterTrigger
	{
		get
		{
			return base.Storage[60];
		}
		set
		{
			base.Storage[60] = value;
		}
	}

	public bool AlterUser
	{
		get
		{
			return base.Storage[61];
		}
		set
		{
			base.Storage[61] = value;
		}
	}

	public bool AlterView
	{
		get
		{
			return base.Storage[62];
		}
		set
		{
			base.Storage[62] = value;
		}
	}

	public bool AlterWorkloadGroup
	{
		get
		{
			return base.Storage[63];
		}
		set
		{
			base.Storage[63] = value;
		}
	}

	public bool AlterXmlSchemaCollection
	{
		get
		{
			return base.Storage[64];
		}
		set
		{
			base.Storage[64] = value;
		}
	}

	public bool BindDefault
	{
		get
		{
			return base.Storage[65];
		}
		set
		{
			base.Storage[65] = value;
		}
	}

	public bool BindRule
	{
		get
		{
			return base.Storage[66];
		}
		set
		{
			base.Storage[66] = value;
		}
	}

	public bool CreateApplicationRole
	{
		get
		{
			return base.Storage[67];
		}
		set
		{
			base.Storage[67] = value;
		}
	}

	public bool CreateAssembly
	{
		get
		{
			return base.Storage[68];
		}
		set
		{
			base.Storage[68] = value;
		}
	}

	public bool CreateAsymmetricKey
	{
		get
		{
			return base.Storage[69];
		}
		set
		{
			base.Storage[69] = value;
		}
	}

	public bool CreateAudit
	{
		get
		{
			return base.Storage[70];
		}
		set
		{
			base.Storage[70] = value;
		}
	}

	public bool CreateAvailabilityGroup
	{
		get
		{
			return base.Storage[71];
		}
		set
		{
			base.Storage[71] = value;
		}
	}

	public bool CreateBrokerPriority
	{
		get
		{
			return base.Storage[72];
		}
		set
		{
			base.Storage[72] = value;
		}
	}

	public bool CreateCertificate
	{
		get
		{
			return base.Storage[73];
		}
		set
		{
			base.Storage[73] = value;
		}
	}

	public bool CreateColumnEncryptionKey
	{
		get
		{
			return base.Storage[74];
		}
		set
		{
			base.Storage[74] = value;
		}
	}

	public bool CreateColumnMasterKey
	{
		get
		{
			return base.Storage[75];
		}
		set
		{
			base.Storage[75] = value;
		}
	}

	public bool CreateContract
	{
		get
		{
			return base.Storage[76];
		}
		set
		{
			base.Storage[76] = value;
		}
	}

	public bool CreateCredential
	{
		get
		{
			return base.Storage[77];
		}
		set
		{
			base.Storage[77] = value;
		}
	}

	public bool CreateCryptographicProvider
	{
		get
		{
			return base.Storage[78];
		}
		set
		{
			base.Storage[78] = value;
		}
	}

	public bool CreateDatabase
	{
		get
		{
			return base.Storage[79];
		}
		set
		{
			base.Storage[79] = value;
		}
	}

	public bool CreateDatabaseAuditSpecification
	{
		get
		{
			return base.Storage[80];
		}
		set
		{
			base.Storage[80] = value;
		}
	}

	public bool CreateDatabaseEncryptionKey
	{
		get
		{
			return base.Storage[81];
		}
		set
		{
			base.Storage[81] = value;
		}
	}

	public bool CreateDefault
	{
		get
		{
			return base.Storage[82];
		}
		set
		{
			base.Storage[82] = value;
		}
	}

	public bool CreateEndpoint
	{
		get
		{
			return base.Storage[83];
		}
		set
		{
			base.Storage[83] = value;
		}
	}

	public bool CreateEventNotification
	{
		get
		{
			return base.Storage[84];
		}
		set
		{
			base.Storage[84] = value;
		}
	}

	public bool CreateEventSession
	{
		get
		{
			return base.Storage[85];
		}
		set
		{
			base.Storage[85] = value;
		}
	}

	public bool CreateExtendedProcedure
	{
		get
		{
			return base.Storage[86];
		}
		set
		{
			base.Storage[86] = value;
		}
	}

	public bool CreateExtendedProperty
	{
		get
		{
			return base.Storage[87];
		}
		set
		{
			base.Storage[87] = value;
		}
	}

	public bool CreateExternalLibrary
	{
		get
		{
			return base.Storage[88];
		}
		set
		{
			base.Storage[88] = value;
		}
	}

	public bool CreateExternalResourcePool
	{
		get
		{
			return base.Storage[89];
		}
		set
		{
			base.Storage[89] = value;
		}
	}

	public bool CreateFulltextCatalog
	{
		get
		{
			return base.Storage[90];
		}
		set
		{
			base.Storage[90] = value;
		}
	}

	public bool CreateFulltextIndex
	{
		get
		{
			return base.Storage[91];
		}
		set
		{
			base.Storage[91] = value;
		}
	}

	public bool CreateFulltextStoplist
	{
		get
		{
			return base.Storage[92];
		}
		set
		{
			base.Storage[92] = value;
		}
	}

	public bool CreateFunction
	{
		get
		{
			return base.Storage[93];
		}
		set
		{
			base.Storage[93] = value;
		}
	}

	public bool CreateIndex
	{
		get
		{
			return base.Storage[94];
		}
		set
		{
			base.Storage[94] = value;
		}
	}

	public bool CreateLinkedServer
	{
		get
		{
			return base.Storage[95];
		}
		set
		{
			base.Storage[95] = value;
		}
	}

	public bool CreateLinkedServerLogin
	{
		get
		{
			return base.Storage[96];
		}
		set
		{
			base.Storage[96] = value;
		}
	}

	public bool CreateLogin
	{
		get
		{
			return base.Storage[97];
		}
		set
		{
			base.Storage[97] = value;
		}
	}

	public bool CreateMasterKey
	{
		get
		{
			return base.Storage[98];
		}
		set
		{
			base.Storage[98] = value;
		}
	}

	public bool CreateMessage
	{
		get
		{
			return base.Storage[99];
		}
		set
		{
			base.Storage[99] = value;
		}
	}

	public bool CreateMessageType
	{
		get
		{
			return base.Storage[100];
		}
		set
		{
			base.Storage[100] = value;
		}
	}

	public bool CreatePartitionFunction
	{
		get
		{
			return base.Storage[101];
		}
		set
		{
			base.Storage[101] = value;
		}
	}

	public bool CreatePartitionScheme
	{
		get
		{
			return base.Storage[102];
		}
		set
		{
			base.Storage[102] = value;
		}
	}

	public bool CreatePlanGuide
	{
		get
		{
			return base.Storage[103];
		}
		set
		{
			base.Storage[103] = value;
		}
	}

	public bool CreateProcedure
	{
		get
		{
			return base.Storage[104];
		}
		set
		{
			base.Storage[104] = value;
		}
	}

	public bool CreateQueue
	{
		get
		{
			return base.Storage[105];
		}
		set
		{
			base.Storage[105] = value;
		}
	}

	public bool CreateRemoteServer
	{
		get
		{
			return base.Storage[106];
		}
		set
		{
			base.Storage[106] = value;
		}
	}

	public bool CreateRemoteServiceBinding
	{
		get
		{
			return base.Storage[107];
		}
		set
		{
			base.Storage[107] = value;
		}
	}

	public bool CreateResourcePool
	{
		get
		{
			return base.Storage[108];
		}
		set
		{
			base.Storage[108] = value;
		}
	}

	public bool CreateRole
	{
		get
		{
			return base.Storage[109];
		}
		set
		{
			base.Storage[109] = value;
		}
	}

	public bool CreateRoute
	{
		get
		{
			return base.Storage[110];
		}
		set
		{
			base.Storage[110] = value;
		}
	}

	public bool CreateRule
	{
		get
		{
			return base.Storage[111];
		}
		set
		{
			base.Storage[111] = value;
		}
	}

	public bool CreateSchema
	{
		get
		{
			return base.Storage[112];
		}
		set
		{
			base.Storage[112] = value;
		}
	}

	public bool CreateSearchPropertyList
	{
		get
		{
			return base.Storage[113];
		}
		set
		{
			base.Storage[113] = value;
		}
	}

	public bool CreateSecurityPolicy
	{
		get
		{
			return base.Storage[114];
		}
		set
		{
			base.Storage[114] = value;
		}
	}

	public bool CreateSequence
	{
		get
		{
			return base.Storage[115];
		}
		set
		{
			base.Storage[115] = value;
		}
	}

	public bool CreateServerAudit
	{
		get
		{
			return base.Storage[116];
		}
		set
		{
			base.Storage[116] = value;
		}
	}

	public bool CreateServerAuditSpecification
	{
		get
		{
			return base.Storage[117];
		}
		set
		{
			base.Storage[117] = value;
		}
	}

	public bool CreateServerRole
	{
		get
		{
			return base.Storage[118];
		}
		set
		{
			base.Storage[118] = value;
		}
	}

	public bool CreateService
	{
		get
		{
			return base.Storage[119];
		}
		set
		{
			base.Storage[119] = value;
		}
	}

	public bool CreateSpatialIndex
	{
		get
		{
			return base.Storage[120];
		}
		set
		{
			base.Storage[120] = value;
		}
	}

	public bool CreateStatistics
	{
		get
		{
			return base.Storage[121];
		}
		set
		{
			base.Storage[121] = value;
		}
	}

	public bool CreateSymmetricKey
	{
		get
		{
			return base.Storage[122];
		}
		set
		{
			base.Storage[122] = value;
		}
	}

	public bool CreateSynonym
	{
		get
		{
			return base.Storage[123];
		}
		set
		{
			base.Storage[123] = value;
		}
	}

	public bool CreateTable
	{
		get
		{
			return base.Storage[124];
		}
		set
		{
			base.Storage[124] = value;
		}
	}

	public bool CreateTrigger
	{
		get
		{
			return base.Storage[125];
		}
		set
		{
			base.Storage[125] = value;
		}
	}

	public bool CreateType
	{
		get
		{
			return base.Storage[126];
		}
		set
		{
			base.Storage[126] = value;
		}
	}

	public bool CreateUser
	{
		get
		{
			return base.Storage[127];
		}
		set
		{
			base.Storage[127] = value;
		}
	}

	public bool CreateView
	{
		get
		{
			return base.Storage[128];
		}
		set
		{
			base.Storage[128] = value;
		}
	}

	public bool CreateWorkloadGroup
	{
		get
		{
			return base.Storage[129];
		}
		set
		{
			base.Storage[129] = value;
		}
	}

	public bool CreateXmlIndex
	{
		get
		{
			return base.Storage[130];
		}
		set
		{
			base.Storage[130] = value;
		}
	}

	public bool CreateXmlSchemaCollection
	{
		get
		{
			return base.Storage[131];
		}
		set
		{
			base.Storage[131] = value;
		}
	}

	public bool DenyDatabase
	{
		get
		{
			return base.Storage[132];
		}
		set
		{
			base.Storage[132] = value;
		}
	}

	public bool DenyServer
	{
		get
		{
			return base.Storage[133];
		}
		set
		{
			base.Storage[133] = value;
		}
	}

	public bool DropApplicationRole
	{
		get
		{
			return base.Storage[134];
		}
		set
		{
			base.Storage[134] = value;
		}
	}

	public bool DropAssembly
	{
		get
		{
			return base.Storage[135];
		}
		set
		{
			base.Storage[135] = value;
		}
	}

	public bool DropAsymmetricKey
	{
		get
		{
			return base.Storage[136];
		}
		set
		{
			base.Storage[136] = value;
		}
	}

	public bool DropAudit
	{
		get
		{
			return base.Storage[137];
		}
		set
		{
			base.Storage[137] = value;
		}
	}

	public bool DropAvailabilityGroup
	{
		get
		{
			return base.Storage[138];
		}
		set
		{
			base.Storage[138] = value;
		}
	}

	public bool DropBrokerPriority
	{
		get
		{
			return base.Storage[139];
		}
		set
		{
			base.Storage[139] = value;
		}
	}

	public bool DropCertificate
	{
		get
		{
			return base.Storage[140];
		}
		set
		{
			base.Storage[140] = value;
		}
	}

	public bool DropColumnEncryptionKey
	{
		get
		{
			return base.Storage[141];
		}
		set
		{
			base.Storage[141] = value;
		}
	}

	public bool DropColumnMasterKey
	{
		get
		{
			return base.Storage[142];
		}
		set
		{
			base.Storage[142] = value;
		}
	}

	public bool DropContract
	{
		get
		{
			return base.Storage[143];
		}
		set
		{
			base.Storage[143] = value;
		}
	}

	public bool DropCredential
	{
		get
		{
			return base.Storage[144];
		}
		set
		{
			base.Storage[144] = value;
		}
	}

	public bool DropCryptographicProvider
	{
		get
		{
			return base.Storage[145];
		}
		set
		{
			base.Storage[145] = value;
		}
	}

	public bool DropDatabase
	{
		get
		{
			return base.Storage[146];
		}
		set
		{
			base.Storage[146] = value;
		}
	}

	public bool DropDatabaseAuditSpecification
	{
		get
		{
			return base.Storage[147];
		}
		set
		{
			base.Storage[147] = value;
		}
	}

	public bool DropDatabaseEncryptionKey
	{
		get
		{
			return base.Storage[148];
		}
		set
		{
			base.Storage[148] = value;
		}
	}

	public bool DropDefault
	{
		get
		{
			return base.Storage[149];
		}
		set
		{
			base.Storage[149] = value;
		}
	}

	public bool DropEndpoint
	{
		get
		{
			return base.Storage[150];
		}
		set
		{
			base.Storage[150] = value;
		}
	}

	public bool DropEventNotification
	{
		get
		{
			return base.Storage[151];
		}
		set
		{
			base.Storage[151] = value;
		}
	}

	public bool DropEventSession
	{
		get
		{
			return base.Storage[152];
		}
		set
		{
			base.Storage[152] = value;
		}
	}

	public bool DropExtendedProcedure
	{
		get
		{
			return base.Storage[153];
		}
		set
		{
			base.Storage[153] = value;
		}
	}

	public bool DropExtendedProperty
	{
		get
		{
			return base.Storage[154];
		}
		set
		{
			base.Storage[154] = value;
		}
	}

	public bool DropExternalLibrary
	{
		get
		{
			return base.Storage[155];
		}
		set
		{
			base.Storage[155] = value;
		}
	}

	public bool DropExternalResourcePool
	{
		get
		{
			return base.Storage[156];
		}
		set
		{
			base.Storage[156] = value;
		}
	}

	public bool DropFulltextCatalog
	{
		get
		{
			return base.Storage[157];
		}
		set
		{
			base.Storage[157] = value;
		}
	}

	public bool DropFulltextIndex
	{
		get
		{
			return base.Storage[158];
		}
		set
		{
			base.Storage[158] = value;
		}
	}

	public bool DropFulltextStoplist
	{
		get
		{
			return base.Storage[159];
		}
		set
		{
			base.Storage[159] = value;
		}
	}

	public bool DropFunction
	{
		get
		{
			return base.Storage[160];
		}
		set
		{
			base.Storage[160] = value;
		}
	}

	public bool DropIndex
	{
		get
		{
			return base.Storage[161];
		}
		set
		{
			base.Storage[161] = value;
		}
	}

	public bool DropLinkedServer
	{
		get
		{
			return base.Storage[162];
		}
		set
		{
			base.Storage[162] = value;
		}
	}

	public bool DropLinkedServerLogin
	{
		get
		{
			return base.Storage[163];
		}
		set
		{
			base.Storage[163] = value;
		}
	}

	public bool DropLogin
	{
		get
		{
			return base.Storage[164];
		}
		set
		{
			base.Storage[164] = value;
		}
	}

	public bool DropMasterKey
	{
		get
		{
			return base.Storage[165];
		}
		set
		{
			base.Storage[165] = value;
		}
	}

	public bool DropMessage
	{
		get
		{
			return base.Storage[166];
		}
		set
		{
			base.Storage[166] = value;
		}
	}

	public bool DropMessageType
	{
		get
		{
			return base.Storage[167];
		}
		set
		{
			base.Storage[167] = value;
		}
	}

	public bool DropPartitionFunction
	{
		get
		{
			return base.Storage[168];
		}
		set
		{
			base.Storage[168] = value;
		}
	}

	public bool DropPartitionScheme
	{
		get
		{
			return base.Storage[169];
		}
		set
		{
			base.Storage[169] = value;
		}
	}

	public bool DropPlanGuide
	{
		get
		{
			return base.Storage[170];
		}
		set
		{
			base.Storage[170] = value;
		}
	}

	public bool DropProcedure
	{
		get
		{
			return base.Storage[171];
		}
		set
		{
			base.Storage[171] = value;
		}
	}

	public bool DropQueue
	{
		get
		{
			return base.Storage[172];
		}
		set
		{
			base.Storage[172] = value;
		}
	}

	public bool DropRemoteServer
	{
		get
		{
			return base.Storage[173];
		}
		set
		{
			base.Storage[173] = value;
		}
	}

	public bool DropRemoteServiceBinding
	{
		get
		{
			return base.Storage[174];
		}
		set
		{
			base.Storage[174] = value;
		}
	}

	public bool DropResourcePool
	{
		get
		{
			return base.Storage[175];
		}
		set
		{
			base.Storage[175] = value;
		}
	}

	public bool DropRole
	{
		get
		{
			return base.Storage[176];
		}
		set
		{
			base.Storage[176] = value;
		}
	}

	public bool DropRoleMember
	{
		get
		{
			return base.Storage[177];
		}
		set
		{
			base.Storage[177] = value;
		}
	}

	public bool DropRoute
	{
		get
		{
			return base.Storage[178];
		}
		set
		{
			base.Storage[178] = value;
		}
	}

	public bool DropRule
	{
		get
		{
			return base.Storage[179];
		}
		set
		{
			base.Storage[179] = value;
		}
	}

	public bool DropSchema
	{
		get
		{
			return base.Storage[180];
		}
		set
		{
			base.Storage[180] = value;
		}
	}

	public bool DropSearchPropertyList
	{
		get
		{
			return base.Storage[181];
		}
		set
		{
			base.Storage[181] = value;
		}
	}

	public bool DropSecurityPolicy
	{
		get
		{
			return base.Storage[182];
		}
		set
		{
			base.Storage[182] = value;
		}
	}

	public bool DropSensitivityClassification
	{
		get
		{
			return base.Storage[183];
		}
		set
		{
			base.Storage[183] = value;
		}
	}

	public bool DropSequence
	{
		get
		{
			return base.Storage[184];
		}
		set
		{
			base.Storage[184] = value;
		}
	}

	public bool DropServerAudit
	{
		get
		{
			return base.Storage[185];
		}
		set
		{
			base.Storage[185] = value;
		}
	}

	public bool DropServerAuditSpecification
	{
		get
		{
			return base.Storage[186];
		}
		set
		{
			base.Storage[186] = value;
		}
	}

	public bool DropServerRole
	{
		get
		{
			return base.Storage[187];
		}
		set
		{
			base.Storage[187] = value;
		}
	}

	public bool DropServerRoleMember
	{
		get
		{
			return base.Storage[188];
		}
		set
		{
			base.Storage[188] = value;
		}
	}

	public bool DropService
	{
		get
		{
			return base.Storage[189];
		}
		set
		{
			base.Storage[189] = value;
		}
	}

	public bool DropSignature
	{
		get
		{
			return base.Storage[190];
		}
		set
		{
			base.Storage[190] = value;
		}
	}

	public bool DropSignatureSchemaObject
	{
		get
		{
			return base.Storage[191];
		}
		set
		{
			base.Storage[191] = value;
		}
	}

	public bool DropStatistics
	{
		get
		{
			return base.Storage[192];
		}
		set
		{
			base.Storage[192] = value;
		}
	}

	public bool DropSymmetricKey
	{
		get
		{
			return base.Storage[193];
		}
		set
		{
			base.Storage[193] = value;
		}
	}

	public bool DropSynonym
	{
		get
		{
			return base.Storage[194];
		}
		set
		{
			base.Storage[194] = value;
		}
	}

	public bool DropTable
	{
		get
		{
			return base.Storage[195];
		}
		set
		{
			base.Storage[195] = value;
		}
	}

	public bool DropTrigger
	{
		get
		{
			return base.Storage[196];
		}
		set
		{
			base.Storage[196] = value;
		}
	}

	public bool DropType
	{
		get
		{
			return base.Storage[197];
		}
		set
		{
			base.Storage[197] = value;
		}
	}

	public bool DropUser
	{
		get
		{
			return base.Storage[198];
		}
		set
		{
			base.Storage[198] = value;
		}
	}

	public bool DropView
	{
		get
		{
			return base.Storage[199];
		}
		set
		{
			base.Storage[199] = value;
		}
	}

	public bool DropWorkloadGroup
	{
		get
		{
			return base.Storage[200];
		}
		set
		{
			base.Storage[200] = value;
		}
	}

	public bool DropXmlSchemaCollection
	{
		get
		{
			return base.Storage[201];
		}
		set
		{
			base.Storage[201] = value;
		}
	}

	public bool GrantDatabase
	{
		get
		{
			return base.Storage[202];
		}
		set
		{
			base.Storage[202] = value;
		}
	}

	public bool GrantServer
	{
		get
		{
			return base.Storage[203];
		}
		set
		{
			base.Storage[203] = value;
		}
	}

	public bool Rename
	{
		get
		{
			return base.Storage[204];
		}
		set
		{
			base.Storage[204] = value;
		}
	}

	public bool RevokeDatabase
	{
		get
		{
			return base.Storage[205];
		}
		set
		{
			base.Storage[205] = value;
		}
	}

	public bool RevokeServer
	{
		get
		{
			return base.Storage[206];
		}
		set
		{
			base.Storage[206] = value;
		}
	}

	public bool UnbindDefault
	{
		get
		{
			return base.Storage[207];
		}
		set
		{
			base.Storage[207] = value;
		}
	}

	public bool UnbindRule
	{
		get
		{
			return base.Storage[208];
		}
		set
		{
			base.Storage[208] = value;
		}
	}

	public bool UpdateStatistics
	{
		get
		{
			return base.Storage[209];
		}
		set
		{
			base.Storage[209] = value;
		}
	}

	public ServerEventSet()
	{
	}

	public ServerEventSet(ServerEventSet eventSet)
		: base(eventSet)
	{
	}

	public ServerEventSet(ServerEvent anEvent)
	{
		SetBit(anEvent);
	}

	public ServerEventSet(params ServerEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (ServerEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new ServerEventSet(base.Storage);
	}

	internal ServerEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(ServerEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(ServerEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public ServerEventSet Add(ServerEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public ServerEventSet Remove(ServerEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static ServerEventSet operator +(ServerEventSet eventSet, ServerEvent anEvent)
	{
		ServerEventSet serverEventSet = new ServerEventSet(eventSet);
		serverEventSet.SetBit(anEvent);
		return serverEventSet;
	}

	public static ServerEventSet Add(ServerEventSet eventSet, ServerEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static ServerEventSet operator -(ServerEventSet eventSet, ServerEvent anEvent)
	{
		ServerEventSet serverEventSet = new ServerEventSet(eventSet);
		serverEventSet.ResetBit(anEvent);
		return serverEventSet;
	}

	public static ServerEventSet Subtract(ServerEventSet eventSet, ServerEvent anEvent)
	{
		return eventSet - anEvent;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name + ": ");
		int num = 0;
		bool flag = true;
		foreach (bool item in base.Storage)
		{
			if (item)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(((ServerEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
