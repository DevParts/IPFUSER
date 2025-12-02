using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseDdlTriggerEventSet : EventSetBase
{
	private bool dirty;

	private static DatabaseDdlTriggerEventSet ddlapplicationroleeventsevents;

	private static DatabaseDdlTriggerEventSet ddlassemblyeventsevents;

	private static DatabaseDdlTriggerEventSet ddlasymmetrickeyeventsevents;

	private static DatabaseDdlTriggerEventSet ddlauthorizationdatabaseeventsevents;

	private static DatabaseDdlTriggerEventSet ddlremoteservicebindingeventsevents;

	private static DatabaseDdlTriggerEventSet ddlcertificateeventsevents;

	private static DatabaseDdlTriggerEventSet ddlcolumnencryptionkeyeventsevents;

	private static DatabaseDdlTriggerEventSet ddlcolumnmasterkeyeventsevents;

	private static DatabaseDdlTriggerEventSet ddlcontracteventsevents;

	private static DatabaseDdlTriggerEventSet ddlcryptosignatureeventsevents;

	private static DatabaseDdlTriggerEventSet ddldatabaseleveleventsevents;

	private static DatabaseDdlTriggerEventSet ddldatabaseauditeventsevents;

	private static DatabaseDdlTriggerEventSet ddldatabaseauditspecificationeventsevents;

	private static DatabaseDdlTriggerEventSet ddlmasterkeyeventsevents;

	private static DatabaseDdlTriggerEventSet ddldatabasesecurityeventsevents;

	private static DatabaseDdlTriggerEventSet ddldefaulteventsevents;

	private static DatabaseDdlTriggerEventSet ddldatabaseencryptionkeyeventsevents;

	private static DatabaseDdlTriggerEventSet ddleventnotificationeventsevents;

	private static DatabaseDdlTriggerEventSet ddlextendedpropertyeventsevents;

	private static DatabaseDdlTriggerEventSet ddlfulltextcatalogeventsevents;

	private static DatabaseDdlTriggerEventSet ddlfulltextstoplisteventsevents;

	private static DatabaseDdlTriggerEventSet ddlfunctioneventsevents;

	private static DatabaseDdlTriggerEventSet ddlgdrdatabaseeventsevents;

	private static DatabaseDdlTriggerEventSet ddlindexeventsevents;

	private static DatabaseDdlTriggerEventSet ddllibraryeventsevents;

	private static DatabaseDdlTriggerEventSet ddlmessagetypeeventsevents;

	private static DatabaseDdlTriggerEventSet ddlsymmetrickeyeventsevents;

	private static DatabaseDdlTriggerEventSet ddlpartitioneventsevents;

	private static DatabaseDdlTriggerEventSet ddlplanguideeventsevents;

	private static DatabaseDdlTriggerEventSet ddlbrokerpriorityeventsevents;

	private static DatabaseDdlTriggerEventSet ddlsearchpropertylisteventsevents;

	private static DatabaseDdlTriggerEventSet ddlpartitionfunctioneventsevents;

	private static DatabaseDdlTriggerEventSet ddlpartitionschemeeventsevents;

	private static DatabaseDdlTriggerEventSet ddlqueueeventsevents;

	private static DatabaseDdlTriggerEventSet ddlroleeventsevents;

	private static DatabaseDdlTriggerEventSet ddlrouteeventsevents;

	private static DatabaseDdlTriggerEventSet ddlruleeventsevents;

	private static DatabaseDdlTriggerEventSet ddlschemaeventsevents;

	private static DatabaseDdlTriggerEventSet ddlsecuritypolicyeventsevents;

	private static DatabaseDdlTriggerEventSet ddlsensitivityeventsevents;

	private static DatabaseDdlTriggerEventSet ddlsequenceeventsevents;

	private static DatabaseDdlTriggerEventSet ddlserviceeventsevents;

	private static DatabaseDdlTriggerEventSet ddlssbeventsevents;

	private static DatabaseDdlTriggerEventSet ddlstatisticseventsevents;

	private static DatabaseDdlTriggerEventSet ddlprocedureeventsevents;

	private static DatabaseDdlTriggerEventSet ddlsynonymeventsevents;

	private static DatabaseDdlTriggerEventSet ddltableeventsevents;

	private static DatabaseDdlTriggerEventSet ddltablevieweventsevents;

	private static DatabaseDdlTriggerEventSet ddltriggereventsevents;

	private static DatabaseDdlTriggerEventSet ddltypeeventsevents;

	private static DatabaseDdlTriggerEventSet ddlusereventsevents;

	private static DatabaseDdlTriggerEventSet ddlvieweventsevents;

	private static DatabaseDdlTriggerEventSet ddlxmlschemacollectioneventsevents;

	public override int NumberOfElements => 148;

	public bool Dirty
	{
		get
		{
			return dirty;
		}
		set
		{
			dirty = value;
		}
	}

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

	public bool AddSignature
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

	public bool AddSignatureSchemaObject
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

	public bool AlterApplicationRole
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

	public bool AlterAssembly
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

	public bool AlterAsymmetricKey
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

	public bool AlterAudit
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

	public bool AlterAuthorizationDatabase
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

	public bool AlterBrokerPriority
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

	public bool AlterCertificate
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

	public bool AlterColumnEncryptionKey
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

	public bool AlterDatabaseAuditSpecification
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

	public bool AlterDatabaseEncryptionKey
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

	public bool AlterDatabaseScopedConfiguration
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

	public bool AlterExtendedProperty
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

	public bool AlterExternalLibrary
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

	public bool AlterFulltextCatalog
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

	public bool AlterFulltextIndex
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

	public bool AlterFulltextStoplist
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

	public bool AlterFunction
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

	public bool AlterIndex
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

	public bool AlterMasterKey
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

	public bool AlterMessageType
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

	public bool AlterPartitionFunction
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

	public bool AlterPartitionScheme
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

	public bool AlterPlanGuide
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

	public bool AlterProcedure
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

	public bool AlterQueue
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

	public bool AlterRemoteServiceBinding
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

	public bool AlterRole
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

	public bool AlterRoute
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

	public bool AlterSchema
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

	public bool AlterSearchPropertyList
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

	public bool AlterSecurityPolicy
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

	public bool AlterSequence
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

	public bool AlterService
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

	public bool AlterSymmetricKey
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

	public bool AlterTable
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

	public bool AlterTrigger
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

	public bool AlterUser
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

	public bool AlterView
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

	public bool AlterXmlSchemaCollection
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

	public bool BindDefault
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

	public bool BindRule
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

	public bool CreateApplicationRole
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

	public bool CreateAssembly
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

	public bool CreateAsymmetricKey
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

	public bool CreateAudit
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

	public bool CreateBrokerPriority
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

	public bool CreateCertificate
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

	public bool CreateColumnEncryptionKey
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

	public bool CreateColumnMasterKey
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

	public bool CreateContract
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

	public bool CreateDatabaseAuditSpecification
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

	public bool CreateDatabaseEncryptionKey
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

	public bool CreateDefault
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

	public bool CreateEventNotification
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

	public bool CreateExtendedProperty
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

	public bool CreateExternalLibrary
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

	public bool CreateFulltextCatalog
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

	public bool CreateFulltextIndex
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

	public bool CreateFulltextStoplist
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

	public bool CreateFunction
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

	public bool CreateIndex
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

	public bool CreateMasterKey
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

	public bool CreateMessageType
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

	public bool CreatePartitionFunction
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

	public bool CreatePartitionScheme
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

	public bool CreatePlanGuide
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

	public bool CreateProcedure
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

	public bool CreateQueue
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

	public bool CreateRemoteServiceBinding
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

	public bool CreateRole
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

	public bool CreateRoute
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

	public bool CreateRule
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

	public bool CreateSchema
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

	public bool CreateSearchPropertyList
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

	public bool CreateSecurityPolicy
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

	public bool CreateSequence
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

	public bool CreateService
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

	public bool CreateSpatialIndex
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

	public bool CreateStatistics
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

	public bool CreateSymmetricKey
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

	public bool CreateSynonym
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

	public bool CreateTable
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

	public bool CreateTrigger
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

	public bool CreateType
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

	public bool CreateUser
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

	public bool CreateView
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

	public bool CreateXmlIndex
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

	public bool CreateXmlSchemaCollection
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

	public bool DenyDatabase
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

	public bool DropApplicationRole
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

	public bool DropAssembly
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

	public bool DropAsymmetricKey
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

	public bool DropAudit
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

	public bool DropBrokerPriority
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

	public bool DropCertificate
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

	public bool DropColumnEncryptionKey
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

	public bool DropColumnMasterKey
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

	public bool DropContract
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

	public bool DropDatabaseAuditSpecification
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

	public bool DropDatabaseEncryptionKey
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

	public bool DropDefault
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

	public bool DropEventNotification
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

	public bool DropExtendedProperty
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

	public bool DropExternalLibrary
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

	public bool DropFulltextCatalog
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

	public bool DropFulltextIndex
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

	public bool DropFulltextStoplist
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

	public bool DropFunction
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

	public bool DropIndex
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

	public bool DropMasterKey
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

	public bool DropMessageType
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

	public bool DropPartitionFunction
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

	public bool DropPartitionScheme
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

	public bool DropPlanGuide
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

	public bool DropProcedure
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

	public bool DropQueue
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

	public bool DropRemoteServiceBinding
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

	public bool DropRole
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

	public bool DropRoleMember
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

	public bool DropRoute
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

	public bool DropRule
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

	public bool DropSchema
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

	public bool DropSearchPropertyList
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

	public bool DropSecurityPolicy
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

	public bool DropSensitivityClassification
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

	public bool DropSequence
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

	public bool DropService
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

	public bool DropSignature
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

	public bool DropSignatureSchemaObject
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

	public bool DropStatistics
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

	public bool DropSymmetricKey
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

	public bool DropSynonym
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

	public bool DropTable
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

	public bool DropTrigger
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

	public bool DropType
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

	public bool DropUser
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

	public bool DropView
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

	public bool DropXmlSchemaCollection
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

	public bool GrantDatabase
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

	public bool Rename
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

	public bool RevokeDatabase
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

	public bool UnbindDefault
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

	public bool UnbindRule
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

	public bool UpdateStatistics
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

	public bool DdlApplicationRoleEventsEvents
	{
		get
		{
			return FitsMask(ddlapplicationroleeventsevents);
		}
		set
		{
			SetValue(ddlapplicationroleeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlAssemblyEventsEvents
	{
		get
		{
			return FitsMask(ddlassemblyeventsevents);
		}
		set
		{
			SetValue(ddlassemblyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlAsymmetricKeyEventsEvents
	{
		get
		{
			return FitsMask(ddlasymmetrickeyeventsevents);
		}
		set
		{
			SetValue(ddlasymmetrickeyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlAuthorizationDatabaseEventsEvents
	{
		get
		{
			return FitsMask(ddlauthorizationdatabaseeventsevents);
		}
		set
		{
			SetValue(ddlauthorizationdatabaseeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlRemoteServiceBindingEventsEvents
	{
		get
		{
			return FitsMask(ddlremoteservicebindingeventsevents);
		}
		set
		{
			SetValue(ddlremoteservicebindingeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlCertificateEventsEvents
	{
		get
		{
			return FitsMask(ddlcertificateeventsevents);
		}
		set
		{
			SetValue(ddlcertificateeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlColumnEncryptionKeyEventsEvents
	{
		get
		{
			return FitsMask(ddlcolumnencryptionkeyeventsevents);
		}
		set
		{
			SetValue(ddlcolumnencryptionkeyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlColumnMasterKeyEventsEvents
	{
		get
		{
			return FitsMask(ddlcolumnmasterkeyeventsevents);
		}
		set
		{
			SetValue(ddlcolumnmasterkeyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlContractEventsEvents
	{
		get
		{
			return FitsMask(ddlcontracteventsevents);
		}
		set
		{
			SetValue(ddlcontracteventsevents, value);
			dirty = true;
		}
	}

	public bool DdlCryptoSignatureEventsEvents
	{
		get
		{
			return FitsMask(ddlcryptosignatureeventsevents);
		}
		set
		{
			SetValue(ddlcryptosignatureeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlDatabaseLevelEventsEvents
	{
		get
		{
			return FitsMask(ddldatabaseleveleventsevents);
		}
		set
		{
			SetValue(ddldatabaseleveleventsevents, value);
			dirty = true;
		}
	}

	public bool DdlDatabaseAuditEventsEvents
	{
		get
		{
			return FitsMask(ddldatabaseauditeventsevents);
		}
		set
		{
			SetValue(ddldatabaseauditeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlDatabaseAuditSpecificationEventsEvents
	{
		get
		{
			return FitsMask(ddldatabaseauditspecificationeventsevents);
		}
		set
		{
			SetValue(ddldatabaseauditspecificationeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlMasterKeyEventsEvents
	{
		get
		{
			return FitsMask(ddlmasterkeyeventsevents);
		}
		set
		{
			SetValue(ddlmasterkeyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlDatabaseSecurityEventsEvents
	{
		get
		{
			return FitsMask(ddldatabasesecurityeventsevents);
		}
		set
		{
			SetValue(ddldatabasesecurityeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlDefaultEventsEvents
	{
		get
		{
			return FitsMask(ddldefaulteventsevents);
		}
		set
		{
			SetValue(ddldefaulteventsevents, value);
			dirty = true;
		}
	}

	public bool DdlDatabaseEncryptionKeyEventsEvents
	{
		get
		{
			return FitsMask(ddldatabaseencryptionkeyeventsevents);
		}
		set
		{
			SetValue(ddldatabaseencryptionkeyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlEventNotificationEventsEvents
	{
		get
		{
			return FitsMask(ddleventnotificationeventsevents);
		}
		set
		{
			SetValue(ddleventnotificationeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlExtendedPropertyEventsEvents
	{
		get
		{
			return FitsMask(ddlextendedpropertyeventsevents);
		}
		set
		{
			SetValue(ddlextendedpropertyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlFulltextCatalogEventsEvents
	{
		get
		{
			return FitsMask(ddlfulltextcatalogeventsevents);
		}
		set
		{
			SetValue(ddlfulltextcatalogeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlFulltextStoplistEventsEvents
	{
		get
		{
			return FitsMask(ddlfulltextstoplisteventsevents);
		}
		set
		{
			SetValue(ddlfulltextstoplisteventsevents, value);
			dirty = true;
		}
	}

	public bool DdlFunctionEventsEvents
	{
		get
		{
			return FitsMask(ddlfunctioneventsevents);
		}
		set
		{
			SetValue(ddlfunctioneventsevents, value);
			dirty = true;
		}
	}

	public bool DdlGdrDatabaseEventsEvents
	{
		get
		{
			return FitsMask(ddlgdrdatabaseeventsevents);
		}
		set
		{
			SetValue(ddlgdrdatabaseeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlIndexEventsEvents
	{
		get
		{
			return FitsMask(ddlindexeventsevents);
		}
		set
		{
			SetValue(ddlindexeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlLibraryEventsEvents
	{
		get
		{
			return FitsMask(ddllibraryeventsevents);
		}
		set
		{
			SetValue(ddllibraryeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlMessageTypeEventsEvents
	{
		get
		{
			return FitsMask(ddlmessagetypeeventsevents);
		}
		set
		{
			SetValue(ddlmessagetypeeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlSymmetricKeyEventsEvents
	{
		get
		{
			return FitsMask(ddlsymmetrickeyeventsevents);
		}
		set
		{
			SetValue(ddlsymmetrickeyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlPartitionEventsEvents
	{
		get
		{
			return FitsMask(ddlpartitioneventsevents);
		}
		set
		{
			SetValue(ddlpartitioneventsevents, value);
			dirty = true;
		}
	}

	public bool DdlPlanGuideEventsEvents
	{
		get
		{
			return FitsMask(ddlplanguideeventsevents);
		}
		set
		{
			SetValue(ddlplanguideeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlBrokerPriorityEventsEvents
	{
		get
		{
			return FitsMask(ddlbrokerpriorityeventsevents);
		}
		set
		{
			SetValue(ddlbrokerpriorityeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlSearchPropertyListEventsEvents
	{
		get
		{
			return FitsMask(ddlsearchpropertylisteventsevents);
		}
		set
		{
			SetValue(ddlsearchpropertylisteventsevents, value);
			dirty = true;
		}
	}

	public bool DdlPartitionFunctionEventsEvents
	{
		get
		{
			return FitsMask(ddlpartitionfunctioneventsevents);
		}
		set
		{
			SetValue(ddlpartitionfunctioneventsevents, value);
			dirty = true;
		}
	}

	public bool DdlPartitionSchemeEventsEvents
	{
		get
		{
			return FitsMask(ddlpartitionschemeeventsevents);
		}
		set
		{
			SetValue(ddlpartitionschemeeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlQueueEventsEvents
	{
		get
		{
			return FitsMask(ddlqueueeventsevents);
		}
		set
		{
			SetValue(ddlqueueeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlRoleEventsEvents
	{
		get
		{
			return FitsMask(ddlroleeventsevents);
		}
		set
		{
			SetValue(ddlroleeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlRouteEventsEvents
	{
		get
		{
			return FitsMask(ddlrouteeventsevents);
		}
		set
		{
			SetValue(ddlrouteeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlRuleEventsEvents
	{
		get
		{
			return FitsMask(ddlruleeventsevents);
		}
		set
		{
			SetValue(ddlruleeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlSchemaEventsEvents
	{
		get
		{
			return FitsMask(ddlschemaeventsevents);
		}
		set
		{
			SetValue(ddlschemaeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlSecurityPolicyEventsEvents
	{
		get
		{
			return FitsMask(ddlsecuritypolicyeventsevents);
		}
		set
		{
			SetValue(ddlsecuritypolicyeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlSensitivityEventsEvents
	{
		get
		{
			return FitsMask(ddlsensitivityeventsevents);
		}
		set
		{
			SetValue(ddlsensitivityeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlSequenceEventsEvents
	{
		get
		{
			return FitsMask(ddlsequenceeventsevents);
		}
		set
		{
			SetValue(ddlsequenceeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlServiceEventsEvents
	{
		get
		{
			return FitsMask(ddlserviceeventsevents);
		}
		set
		{
			SetValue(ddlserviceeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlSsbEventsEvents
	{
		get
		{
			return FitsMask(ddlssbeventsevents);
		}
		set
		{
			SetValue(ddlssbeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlStatisticsEventsEvents
	{
		get
		{
			return FitsMask(ddlstatisticseventsevents);
		}
		set
		{
			SetValue(ddlstatisticseventsevents, value);
			dirty = true;
		}
	}

	public bool DdlProcedureEventsEvents
	{
		get
		{
			return FitsMask(ddlprocedureeventsevents);
		}
		set
		{
			SetValue(ddlprocedureeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlSynonymEventsEvents
	{
		get
		{
			return FitsMask(ddlsynonymeventsevents);
		}
		set
		{
			SetValue(ddlsynonymeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlTableEventsEvents
	{
		get
		{
			return FitsMask(ddltableeventsevents);
		}
		set
		{
			SetValue(ddltableeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlTableViewEventsEvents
	{
		get
		{
			return FitsMask(ddltablevieweventsevents);
		}
		set
		{
			SetValue(ddltablevieweventsevents, value);
			dirty = true;
		}
	}

	public bool DdlTriggerEventsEvents
	{
		get
		{
			return FitsMask(ddltriggereventsevents);
		}
		set
		{
			SetValue(ddltriggereventsevents, value);
			dirty = true;
		}
	}

	public bool DdlTypeEventsEvents
	{
		get
		{
			return FitsMask(ddltypeeventsevents);
		}
		set
		{
			SetValue(ddltypeeventsevents, value);
			dirty = true;
		}
	}

	public bool DdlUserEventsEvents
	{
		get
		{
			return FitsMask(ddlusereventsevents);
		}
		set
		{
			SetValue(ddlusereventsevents, value);
			dirty = true;
		}
	}

	public bool DdlViewEventsEvents
	{
		get
		{
			return FitsMask(ddlvieweventsevents);
		}
		set
		{
			SetValue(ddlvieweventsevents, value);
			dirty = true;
		}
	}

	public bool DdlXmlSchemaCollectionEventsEvents
	{
		get
		{
			return FitsMask(ddlxmlschemacollectioneventsevents);
		}
		set
		{
			SetValue(ddlxmlschemacollectioneventsevents, value);
			dirty = true;
		}
	}

	public DatabaseDdlTriggerEventSet()
	{
	}

	public DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEventSet eventSet)
		: base(eventSet)
	{
	}

	public DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent anEvent)
	{
		SetBit(anEvent);
	}

	public DatabaseDdlTriggerEventSet(params DatabaseDdlTriggerEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (DatabaseDdlTriggerEvent bit in events)
		{
			SetBit(bit);
		}
	}

	internal DatabaseDdlTriggerEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	public override EventSetBase Copy()
	{
		return new DatabaseDdlTriggerEventSet(base.Storage);
	}

	internal void SetBit(DatabaseDdlTriggerEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(DatabaseDdlTriggerEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public DatabaseDdlTriggerEventSet Add(DatabaseDdlTriggerEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public DatabaseDdlTriggerEventSet Remove(DatabaseDdlTriggerEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static DatabaseDdlTriggerEventSet operator +(DatabaseDdlTriggerEventSet eventSet, DatabaseDdlTriggerEvent anEvent)
	{
		DatabaseDdlTriggerEventSet databaseDdlTriggerEventSet = new DatabaseDdlTriggerEventSet(eventSet);
		databaseDdlTriggerEventSet.SetBit(anEvent);
		return databaseDdlTriggerEventSet;
	}

	public static DatabaseDdlTriggerEventSet Add(DatabaseDdlTriggerEventSet eventSet, DatabaseDdlTriggerEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static DatabaseDdlTriggerEventSet operator -(DatabaseDdlTriggerEventSet eventSet, DatabaseDdlTriggerEvent anEvent)
	{
		DatabaseDdlTriggerEventSet databaseDdlTriggerEventSet = new DatabaseDdlTriggerEventSet(eventSet);
		databaseDdlTriggerEventSet.ResetBit(anEvent);
		return databaseDdlTriggerEventSet;
	}

	public static DatabaseDdlTriggerEventSet Subtract(DatabaseDdlTriggerEventSet eventSet, DatabaseDdlTriggerEvent anEvent)
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
				stringBuilder.Append(((DatabaseDdlTriggerEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}

	static DatabaseDdlTriggerEventSet()
	{
		ddlapplicationroleeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateApplicationRole, DatabaseDdlTriggerEvent.AlterApplicationRole, DatabaseDdlTriggerEvent.DropApplicationRole);
		ddlassemblyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateAssembly, DatabaseDdlTriggerEvent.AlterAssembly, DatabaseDdlTriggerEvent.DropAssembly);
		ddlasymmetrickeyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateAsymmetricKey, DatabaseDdlTriggerEvent.AlterAsymmetricKey, DatabaseDdlTriggerEvent.DropAsymmetricKey);
		ddlauthorizationdatabaseeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.AlterAuthorizationDatabase);
		ddlremoteservicebindingeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateRemoteServiceBinding, DatabaseDdlTriggerEvent.AlterRemoteServiceBinding, DatabaseDdlTriggerEvent.DropRemoteServiceBinding);
		ddlcertificateeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateCertificate, DatabaseDdlTriggerEvent.AlterCertificate, DatabaseDdlTriggerEvent.DropCertificate);
		ddlcolumnencryptionkeyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateColumnEncryptionKey, DatabaseDdlTriggerEvent.AlterColumnEncryptionKey, DatabaseDdlTriggerEvent.DropColumnEncryptionKey);
		ddlcolumnmasterkeyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateColumnMasterKey, DatabaseDdlTriggerEvent.DropColumnMasterKey);
		ddlcontracteventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateContract, DatabaseDdlTriggerEvent.DropContract);
		ddlcryptosignatureeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.AddSignatureSchemaObject, DatabaseDdlTriggerEvent.DropSignatureSchemaObject, DatabaseDdlTriggerEvent.AddSignature, DatabaseDdlTriggerEvent.DropSignature);
		ddldatabaseleveleventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateTable, DatabaseDdlTriggerEvent.AlterTable, DatabaseDdlTriggerEvent.DropTable, DatabaseDdlTriggerEvent.CreateView, DatabaseDdlTriggerEvent.AlterView, DatabaseDdlTriggerEvent.DropView, DatabaseDdlTriggerEvent.CreateIndex, DatabaseDdlTriggerEvent.AlterIndex, DatabaseDdlTriggerEvent.DropIndex, DatabaseDdlTriggerEvent.CreateXmlIndex, DatabaseDdlTriggerEvent.AlterFulltextIndex, DatabaseDdlTriggerEvent.CreateFulltextIndex, DatabaseDdlTriggerEvent.DropFulltextIndex, DatabaseDdlTriggerEvent.CreateSpatialIndex, DatabaseDdlTriggerEvent.CreateStatistics, DatabaseDdlTriggerEvent.UpdateStatistics, DatabaseDdlTriggerEvent.DropStatistics, DatabaseDdlTriggerEvent.CreateSynonym, DatabaseDdlTriggerEvent.DropSynonym, DatabaseDdlTriggerEvent.CreateFunction, DatabaseDdlTriggerEvent.AlterFunction, DatabaseDdlTriggerEvent.DropFunction, DatabaseDdlTriggerEvent.CreateProcedure, DatabaseDdlTriggerEvent.AlterProcedure, DatabaseDdlTriggerEvent.DropProcedure, DatabaseDdlTriggerEvent.CreateTrigger, DatabaseDdlTriggerEvent.AlterTrigger, DatabaseDdlTriggerEvent.DropTrigger, DatabaseDdlTriggerEvent.CreateEventNotification, DatabaseDdlTriggerEvent.DropEventNotification, DatabaseDdlTriggerEvent.CreateAssembly, DatabaseDdlTriggerEvent.AlterAssembly, DatabaseDdlTriggerEvent.DropAssembly, DatabaseDdlTriggerEvent.CreateType, DatabaseDdlTriggerEvent.DropType, DatabaseDdlTriggerEvent.CreateSequence, DatabaseDdlTriggerEvent.AlterSequence, DatabaseDdlTriggerEvent.DropSequence, DatabaseDdlTriggerEvent.CreateExternalLibrary, DatabaseDdlTriggerEvent.AlterExternalLibrary, DatabaseDdlTriggerEvent.DropExternalLibrary, DatabaseDdlTriggerEvent.AddSensitivityClassification, DatabaseDdlTriggerEvent.DropSensitivityClassification, DatabaseDdlTriggerEvent.CreateCertificate, DatabaseDdlTriggerEvent.AlterCertificate, DatabaseDdlTriggerEvent.DropCertificate, DatabaseDdlTriggerEvent.CreateUser, DatabaseDdlTriggerEvent.AlterUser, DatabaseDdlTriggerEvent.DropUser, DatabaseDdlTriggerEvent.AddRoleMember, DatabaseDdlTriggerEvent.DropRoleMember, DatabaseDdlTriggerEvent.CreateRole, DatabaseDdlTriggerEvent.AlterRole, DatabaseDdlTriggerEvent.DropRole, DatabaseDdlTriggerEvent.CreateApplicationRole, DatabaseDdlTriggerEvent.AlterApplicationRole, DatabaseDdlTriggerEvent.DropApplicationRole, DatabaseDdlTriggerEvent.CreateSchema, DatabaseDdlTriggerEvent.AlterSchema, DatabaseDdlTriggerEvent.DropSchema, DatabaseDdlTriggerEvent.GrantDatabase, DatabaseDdlTriggerEvent.DenyDatabase, DatabaseDdlTriggerEvent.RevokeDatabase, DatabaseDdlTriggerEvent.AlterAuthorizationDatabase, DatabaseDdlTriggerEvent.CreateSymmetricKey, DatabaseDdlTriggerEvent.AlterSymmetricKey, DatabaseDdlTriggerEvent.DropSymmetricKey, DatabaseDdlTriggerEvent.CreateAsymmetricKey, DatabaseDdlTriggerEvent.AlterAsymmetricKey, DatabaseDdlTriggerEvent.DropAsymmetricKey, DatabaseDdlTriggerEvent.AddSignatureSchemaObject, DatabaseDdlTriggerEvent.DropSignatureSchemaObject, DatabaseDdlTriggerEvent.AddSignature, DatabaseDdlTriggerEvent.DropSignature, DatabaseDdlTriggerEvent.CreateMasterKey, DatabaseDdlTriggerEvent.AlterMasterKey, DatabaseDdlTriggerEvent.DropMasterKey, DatabaseDdlTriggerEvent.CreateDatabaseEncryptionKey, DatabaseDdlTriggerEvent.AlterDatabaseEncryptionKey, DatabaseDdlTriggerEvent.DropDatabaseEncryptionKey, DatabaseDdlTriggerEvent.CreateDatabaseAuditSpecification, DatabaseDdlTriggerEvent.AlterDatabaseAuditSpecification, DatabaseDdlTriggerEvent.DropDatabaseAuditSpecification, DatabaseDdlTriggerEvent.CreateAudit, DatabaseDdlTriggerEvent.DropAudit, DatabaseDdlTriggerEvent.AlterAudit, DatabaseDdlTriggerEvent.CreateMessageType, DatabaseDdlTriggerEvent.AlterMessageType, DatabaseDdlTriggerEvent.DropMessageType, DatabaseDdlTriggerEvent.CreateContract, DatabaseDdlTriggerEvent.DropContract, DatabaseDdlTriggerEvent.CreateQueue, DatabaseDdlTriggerEvent.AlterQueue, DatabaseDdlTriggerEvent.DropQueue, DatabaseDdlTriggerEvent.CreateService, DatabaseDdlTriggerEvent.AlterService, DatabaseDdlTriggerEvent.DropService, DatabaseDdlTriggerEvent.CreateRoute, DatabaseDdlTriggerEvent.AlterRoute, DatabaseDdlTriggerEvent.DropRoute, DatabaseDdlTriggerEvent.CreateRemoteServiceBinding, DatabaseDdlTriggerEvent.AlterRemoteServiceBinding, DatabaseDdlTriggerEvent.DropRemoteServiceBinding, DatabaseDdlTriggerEvent.CreateBrokerPriority, DatabaseDdlTriggerEvent.AlterBrokerPriority, DatabaseDdlTriggerEvent.DropBrokerPriority, DatabaseDdlTriggerEvent.CreateXmlSchemaCollection, DatabaseDdlTriggerEvent.AlterXmlSchemaCollection, DatabaseDdlTriggerEvent.DropXmlSchemaCollection, DatabaseDdlTriggerEvent.CreatePartitionFunction, DatabaseDdlTriggerEvent.AlterPartitionFunction, DatabaseDdlTriggerEvent.DropPartitionFunction, DatabaseDdlTriggerEvent.CreatePartitionScheme, DatabaseDdlTriggerEvent.AlterPartitionScheme, DatabaseDdlTriggerEvent.DropPartitionScheme, DatabaseDdlTriggerEvent.BindDefault, DatabaseDdlTriggerEvent.CreateDefault, DatabaseDdlTriggerEvent.DropDefault, DatabaseDdlTriggerEvent.UnbindDefault, DatabaseDdlTriggerEvent.AlterExtendedProperty, DatabaseDdlTriggerEvent.CreateExtendedProperty, DatabaseDdlTriggerEvent.DropExtendedProperty, DatabaseDdlTriggerEvent.AlterFulltextCatalog, DatabaseDdlTriggerEvent.CreateFulltextCatalog, DatabaseDdlTriggerEvent.DropFulltextCatalog, DatabaseDdlTriggerEvent.AlterPlanGuide, DatabaseDdlTriggerEvent.CreatePlanGuide, DatabaseDdlTriggerEvent.DropPlanGuide, DatabaseDdlTriggerEvent.BindRule, DatabaseDdlTriggerEvent.CreateRule, DatabaseDdlTriggerEvent.DropRule, DatabaseDdlTriggerEvent.UnbindRule, DatabaseDdlTriggerEvent.CreateFulltextStoplist, DatabaseDdlTriggerEvent.AlterFulltextStoplist, DatabaseDdlTriggerEvent.DropFulltextStoplist, DatabaseDdlTriggerEvent.CreateSearchPropertyList, DatabaseDdlTriggerEvent.AlterSearchPropertyList, DatabaseDdlTriggerEvent.DropSearchPropertyList, DatabaseDdlTriggerEvent.CreateSecurityPolicy, DatabaseDdlTriggerEvent.AlterSecurityPolicy, DatabaseDdlTriggerEvent.DropSecurityPolicy, DatabaseDdlTriggerEvent.CreateColumnMasterKey, DatabaseDdlTriggerEvent.DropColumnMasterKey, DatabaseDdlTriggerEvent.CreateColumnEncryptionKey, DatabaseDdlTriggerEvent.AlterColumnEncryptionKey, DatabaseDdlTriggerEvent.DropColumnEncryptionKey, DatabaseDdlTriggerEvent.Rename, DatabaseDdlTriggerEvent.AlterDatabaseScopedConfiguration);
		ddldatabaseauditeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateAudit, DatabaseDdlTriggerEvent.DropAudit, DatabaseDdlTriggerEvent.AlterAudit);
		ddldatabaseauditspecificationeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateDatabaseAuditSpecification, DatabaseDdlTriggerEvent.AlterDatabaseAuditSpecification, DatabaseDdlTriggerEvent.DropDatabaseAuditSpecification);
		ddlmasterkeyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateMasterKey, DatabaseDdlTriggerEvent.AlterMasterKey, DatabaseDdlTriggerEvent.DropMasterKey);
		ddldatabasesecurityeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateCertificate, DatabaseDdlTriggerEvent.AlterCertificate, DatabaseDdlTriggerEvent.DropCertificate, DatabaseDdlTriggerEvent.CreateUser, DatabaseDdlTriggerEvent.AlterUser, DatabaseDdlTriggerEvent.DropUser, DatabaseDdlTriggerEvent.AddRoleMember, DatabaseDdlTriggerEvent.DropRoleMember, DatabaseDdlTriggerEvent.CreateRole, DatabaseDdlTriggerEvent.AlterRole, DatabaseDdlTriggerEvent.DropRole, DatabaseDdlTriggerEvent.CreateApplicationRole, DatabaseDdlTriggerEvent.AlterApplicationRole, DatabaseDdlTriggerEvent.DropApplicationRole, DatabaseDdlTriggerEvent.CreateSchema, DatabaseDdlTriggerEvent.AlterSchema, DatabaseDdlTriggerEvent.DropSchema, DatabaseDdlTriggerEvent.GrantDatabase, DatabaseDdlTriggerEvent.DenyDatabase, DatabaseDdlTriggerEvent.RevokeDatabase, DatabaseDdlTriggerEvent.AlterAuthorizationDatabase, DatabaseDdlTriggerEvent.CreateSymmetricKey, DatabaseDdlTriggerEvent.AlterSymmetricKey, DatabaseDdlTriggerEvent.DropSymmetricKey, DatabaseDdlTriggerEvent.CreateAsymmetricKey, DatabaseDdlTriggerEvent.AlterAsymmetricKey, DatabaseDdlTriggerEvent.DropAsymmetricKey, DatabaseDdlTriggerEvent.AddSignatureSchemaObject, DatabaseDdlTriggerEvent.DropSignatureSchemaObject, DatabaseDdlTriggerEvent.AddSignature, DatabaseDdlTriggerEvent.DropSignature, DatabaseDdlTriggerEvent.CreateMasterKey, DatabaseDdlTriggerEvent.AlterMasterKey, DatabaseDdlTriggerEvent.DropMasterKey, DatabaseDdlTriggerEvent.CreateDatabaseEncryptionKey, DatabaseDdlTriggerEvent.AlterDatabaseEncryptionKey, DatabaseDdlTriggerEvent.DropDatabaseEncryptionKey, DatabaseDdlTriggerEvent.CreateDatabaseAuditSpecification, DatabaseDdlTriggerEvent.AlterDatabaseAuditSpecification, DatabaseDdlTriggerEvent.DropDatabaseAuditSpecification, DatabaseDdlTriggerEvent.CreateAudit, DatabaseDdlTriggerEvent.DropAudit, DatabaseDdlTriggerEvent.AlterAudit);
		ddldefaulteventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.BindDefault, DatabaseDdlTriggerEvent.CreateDefault, DatabaseDdlTriggerEvent.DropDefault, DatabaseDdlTriggerEvent.UnbindDefault);
		ddldatabaseencryptionkeyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateDatabaseEncryptionKey, DatabaseDdlTriggerEvent.AlterDatabaseEncryptionKey, DatabaseDdlTriggerEvent.DropDatabaseEncryptionKey);
		ddleventnotificationeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateEventNotification, DatabaseDdlTriggerEvent.DropEventNotification);
		ddlextendedpropertyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.AlterExtendedProperty, DatabaseDdlTriggerEvent.CreateExtendedProperty, DatabaseDdlTriggerEvent.DropExtendedProperty);
		ddlfulltextcatalogeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.AlterFulltextCatalog, DatabaseDdlTriggerEvent.CreateFulltextCatalog, DatabaseDdlTriggerEvent.DropFulltextCatalog);
		ddlfulltextstoplisteventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateFulltextStoplist, DatabaseDdlTriggerEvent.AlterFulltextStoplist, DatabaseDdlTriggerEvent.DropFulltextStoplist);
		ddlfunctioneventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateFunction, DatabaseDdlTriggerEvent.AlterFunction, DatabaseDdlTriggerEvent.DropFunction);
		ddlgdrdatabaseeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.GrantDatabase, DatabaseDdlTriggerEvent.DenyDatabase, DatabaseDdlTriggerEvent.RevokeDatabase);
		ddlindexeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateIndex, DatabaseDdlTriggerEvent.AlterIndex, DatabaseDdlTriggerEvent.DropIndex, DatabaseDdlTriggerEvent.CreateXmlIndex, DatabaseDdlTriggerEvent.AlterFulltextIndex, DatabaseDdlTriggerEvent.CreateFulltextIndex, DatabaseDdlTriggerEvent.DropFulltextIndex, DatabaseDdlTriggerEvent.CreateSpatialIndex);
		ddllibraryeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateExternalLibrary, DatabaseDdlTriggerEvent.AlterExternalLibrary, DatabaseDdlTriggerEvent.DropExternalLibrary);
		ddlmessagetypeeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateMessageType, DatabaseDdlTriggerEvent.AlterMessageType, DatabaseDdlTriggerEvent.DropMessageType);
		ddlsymmetrickeyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateSymmetricKey, DatabaseDdlTriggerEvent.AlterSymmetricKey, DatabaseDdlTriggerEvent.DropSymmetricKey);
		ddlpartitioneventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreatePartitionFunction, DatabaseDdlTriggerEvent.AlterPartitionFunction, DatabaseDdlTriggerEvent.DropPartitionFunction, DatabaseDdlTriggerEvent.CreatePartitionScheme, DatabaseDdlTriggerEvent.AlterPartitionScheme, DatabaseDdlTriggerEvent.DropPartitionScheme);
		ddlplanguideeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.AlterPlanGuide, DatabaseDdlTriggerEvent.CreatePlanGuide, DatabaseDdlTriggerEvent.DropPlanGuide);
		ddlbrokerpriorityeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateBrokerPriority, DatabaseDdlTriggerEvent.AlterBrokerPriority, DatabaseDdlTriggerEvent.DropBrokerPriority);
		ddlsearchpropertylisteventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateSearchPropertyList, DatabaseDdlTriggerEvent.AlterSearchPropertyList, DatabaseDdlTriggerEvent.DropSearchPropertyList);
		ddlpartitionfunctioneventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreatePartitionFunction, DatabaseDdlTriggerEvent.AlterPartitionFunction, DatabaseDdlTriggerEvent.DropPartitionFunction);
		ddlpartitionschemeeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreatePartitionScheme, DatabaseDdlTriggerEvent.AlterPartitionScheme, DatabaseDdlTriggerEvent.DropPartitionScheme);
		ddlqueueeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateQueue, DatabaseDdlTriggerEvent.AlterQueue, DatabaseDdlTriggerEvent.DropQueue);
		ddlroleeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.AddRoleMember, DatabaseDdlTriggerEvent.DropRoleMember, DatabaseDdlTriggerEvent.CreateRole, DatabaseDdlTriggerEvent.AlterRole, DatabaseDdlTriggerEvent.DropRole);
		ddlrouteeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateRoute, DatabaseDdlTriggerEvent.AlterRoute, DatabaseDdlTriggerEvent.DropRoute);
		ddlruleeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.BindRule, DatabaseDdlTriggerEvent.CreateRule, DatabaseDdlTriggerEvent.DropRule, DatabaseDdlTriggerEvent.UnbindRule);
		ddlschemaeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateSchema, DatabaseDdlTriggerEvent.AlterSchema, DatabaseDdlTriggerEvent.DropSchema);
		ddlsecuritypolicyeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateSecurityPolicy, DatabaseDdlTriggerEvent.AlterSecurityPolicy, DatabaseDdlTriggerEvent.DropSecurityPolicy);
		ddlsensitivityeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.AddSensitivityClassification, DatabaseDdlTriggerEvent.DropSensitivityClassification);
		ddlsequenceeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateSequence, DatabaseDdlTriggerEvent.AlterSequence, DatabaseDdlTriggerEvent.DropSequence);
		ddlserviceeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateService, DatabaseDdlTriggerEvent.AlterService, DatabaseDdlTriggerEvent.DropService);
		ddlssbeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateMessageType, DatabaseDdlTriggerEvent.AlterMessageType, DatabaseDdlTriggerEvent.DropMessageType, DatabaseDdlTriggerEvent.CreateContract, DatabaseDdlTriggerEvent.DropContract, DatabaseDdlTriggerEvent.CreateQueue, DatabaseDdlTriggerEvent.AlterQueue, DatabaseDdlTriggerEvent.DropQueue, DatabaseDdlTriggerEvent.CreateService, DatabaseDdlTriggerEvent.AlterService, DatabaseDdlTriggerEvent.DropService, DatabaseDdlTriggerEvent.CreateRoute, DatabaseDdlTriggerEvent.AlterRoute, DatabaseDdlTriggerEvent.DropRoute, DatabaseDdlTriggerEvent.CreateRemoteServiceBinding, DatabaseDdlTriggerEvent.AlterRemoteServiceBinding, DatabaseDdlTriggerEvent.DropRemoteServiceBinding, DatabaseDdlTriggerEvent.CreateBrokerPriority, DatabaseDdlTriggerEvent.AlterBrokerPriority, DatabaseDdlTriggerEvent.DropBrokerPriority);
		ddlstatisticseventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateStatistics, DatabaseDdlTriggerEvent.UpdateStatistics, DatabaseDdlTriggerEvent.DropStatistics);
		ddlprocedureeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateProcedure, DatabaseDdlTriggerEvent.AlterProcedure, DatabaseDdlTriggerEvent.DropProcedure);
		ddlsynonymeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateSynonym, DatabaseDdlTriggerEvent.DropSynonym);
		ddltableeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateTable, DatabaseDdlTriggerEvent.AlterTable, DatabaseDdlTriggerEvent.DropTable);
		ddltablevieweventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateTable, DatabaseDdlTriggerEvent.AlterTable, DatabaseDdlTriggerEvent.DropTable, DatabaseDdlTriggerEvent.CreateView, DatabaseDdlTriggerEvent.AlterView, DatabaseDdlTriggerEvent.DropView, DatabaseDdlTriggerEvent.CreateIndex, DatabaseDdlTriggerEvent.AlterIndex, DatabaseDdlTriggerEvent.DropIndex, DatabaseDdlTriggerEvent.CreateXmlIndex, DatabaseDdlTriggerEvent.AlterFulltextIndex, DatabaseDdlTriggerEvent.CreateFulltextIndex, DatabaseDdlTriggerEvent.DropFulltextIndex, DatabaseDdlTriggerEvent.CreateSpatialIndex, DatabaseDdlTriggerEvent.CreateStatistics, DatabaseDdlTriggerEvent.UpdateStatistics, DatabaseDdlTriggerEvent.DropStatistics);
		ddltriggereventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateTrigger, DatabaseDdlTriggerEvent.AlterTrigger, DatabaseDdlTriggerEvent.DropTrigger);
		ddltypeeventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateType, DatabaseDdlTriggerEvent.DropType);
		ddlusereventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateUser, DatabaseDdlTriggerEvent.AlterUser, DatabaseDdlTriggerEvent.DropUser);
		ddlvieweventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateView, DatabaseDdlTriggerEvent.AlterView, DatabaseDdlTriggerEvent.DropView);
		ddlxmlschemacollectioneventsevents = new DatabaseDdlTriggerEventSet(DatabaseDdlTriggerEvent.CreateXmlSchemaCollection, DatabaseDdlTriggerEvent.AlterXmlSchemaCollection, DatabaseDdlTriggerEvent.DropXmlSchemaCollection);
	}
}
