using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerTraceEventSet : EventSetBase
{
	public override int NumberOfElements => 118;

	public bool AssemblyLoad
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

	public bool AuditAddloginEvent
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

	public bool AuditAddDBUserEvent
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

	public bool AuditAddLoginToServerRoleEvent
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

	public bool AuditAddMemberToDBRoleEvent
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

	public bool AuditAddRoleEvent
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

	public bool AuditAppRoleChangePasswordEvent
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

	public bool AuditBackupRestoreEvent
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

	public bool AuditChangeAuditEvent
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

	public bool AuditChangeDatabaseOwner
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

	public bool AuditDatabaseManagementEvent
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

	public bool AuditDatabaseObjectAccessEvent
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

	public bool AuditDatabaseObjectGdrEvent
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

	public bool AuditDatabaseObjectManagementEvent
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

	public bool AuditDatabaseObjectTakeOwnershipEvent
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

	public bool AuditDatabaseOperationEvent
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

	public bool AuditDatabasePrincipalImpersonationEvent
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

	public bool AuditDatabasePrincipalManagementEvent
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

	public bool AuditDatabaseScopeGdrEvent
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

	public bool AuditDbccEvent
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

	public bool AuditFulltext
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

	public bool AuditLogin
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

	public bool AuditLoginChangePasswordEvent
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

	public bool AuditLoginChangePropertyEvent
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

	public bool AuditLoginFailed
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

	public bool AuditLoginGdrEvent
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

	public bool AuditLogout
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

	public bool AuditSchemaObjectAccessEvent
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

	public bool AuditSchemaObjectGdrEvent
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

	public bool AuditSchemaObjectManagementEvent
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

	public bool AuditSchemaObjectTakeOwnershipEvent
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

	public bool AuditServerAlterTraceEvent
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

	public bool AuditServerObjectGdrEvent
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

	public bool AuditServerObjectManagementEvent
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

	public bool AuditServerObjectTakeOwnershipEvent
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

	public bool AuditServerOperationEvent
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

	public bool AuditServerPrincipalImpersonationEvent
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

	public bool AuditServerPrincipalManagementEvent
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

	public bool AuditServerScopeGdrEvent
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

	public bool BitmapWarning
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

	public bool BlockedProcessReport
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

	public bool CpuThresholdExceeded
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

	public bool DatabaseMirroringStateChange
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

	public bool DatabaseSuspectDataPage
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

	public bool DataFileAutoGrow
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

	public bool DataFileAutoShrink
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

	public bool DeadlockGraph
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

	public bool DeprecationAnnouncement
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

	public bool DeprecationFinalSupport
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

	public bool Errorlog
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

	public bool Eventlog
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

	public bool Exception
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

	public bool ExchangeSpillEvent
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

	public bool ExecutionWarnings
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

	public bool FtCrawlAborted
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

	public bool FtCrawlStarted
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

	public bool FtCrawlStopped
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

	public bool HashWarning
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

	public bool LockDeadlock
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

	public bool LockDeadlockChain
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

	public bool LockEscalation
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

	public bool LogFileAutoGrow
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

	public bool LogFileAutoShrink
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

	public bool MissingColumnStatistics
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

	public bool MissingJoinPredicate
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

	public bool MountTape
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

	public bool ObjectAltered
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

	public bool ObjectCreated
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

	public bool ObjectDeleted
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

	public bool OledbCallEvent
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

	public bool OledbDatareadEvent
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

	public bool OledbErrors
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

	public bool OledbProviderInformation
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

	public bool OledbQueryinterfaceEvent
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

	public bool Qn_dynamics
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

	public bool Qn_parameterTable
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

	public bool Qn_subscription
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

	public bool Qn_template
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

	public bool ServerMemoryChange
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

	public bool ShowplanAllForQueryCompile
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

	public bool ShowplanXml
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

	public bool ShowplanXmlForQueryCompile
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

	public bool ShowplanXmlStatisticsProfile
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

	public bool SortWarnings
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

	public bool SpCacheinsert
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

	public bool SpCachemiss
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

	public bool SpCacheremove
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

	public bool SpRecompile
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

	public bool SqlStmtrecompile
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

	public bool TraceFileClose
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

	public bool TraceAllEvents
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

	public bool TraceClr
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

	public bool TraceDatabase
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

	public bool TraceDeprecation
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

	public bool TraceErrorsAndWarnings
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

	public bool TraceFullText
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

	public bool TraceLocks
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

	public bool TraceObjects
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

	public bool TraceOledb
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

	public bool TracePerformance
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

	public bool TraceQueryNotifications
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

	public bool TraceSecurityAudit
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

	public bool TraceServer
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

	public bool TraceStoredProcedures
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

	public bool TraceTsql
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

	public bool TraceUserConfigurable
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

	public bool Userconfigurable0
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

	public bool Userconfigurable1
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

	public bool Userconfigurable2
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

	public bool Userconfigurable3
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

	public bool Userconfigurable4
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

	public bool Userconfigurable5
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

	public bool Userconfigurable6
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

	public bool Userconfigurable7
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

	public bool Userconfigurable8
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

	public bool Userconfigurable9
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

	public bool UserErrorMessage
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

	public bool XqueryStaticType
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

	public ServerTraceEventSet()
	{
	}

	public ServerTraceEventSet(ServerTraceEventSet eventSet)
		: base(eventSet)
	{
	}

	public ServerTraceEventSet(ServerTraceEvent anEvent)
	{
		SetBit(anEvent);
	}

	public ServerTraceEventSet(params ServerTraceEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (ServerTraceEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new ServerTraceEventSet(base.Storage);
	}

	internal ServerTraceEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(ServerTraceEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(ServerTraceEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public ServerTraceEventSet Add(ServerTraceEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public ServerTraceEventSet Remove(ServerTraceEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static ServerTraceEventSet operator +(ServerTraceEventSet eventSet, ServerTraceEvent anEvent)
	{
		ServerTraceEventSet serverTraceEventSet = new ServerTraceEventSet(eventSet);
		serverTraceEventSet.SetBit(anEvent);
		return serverTraceEventSet;
	}

	public static ServerTraceEventSet Add(ServerTraceEventSet eventSet, ServerTraceEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static ServerTraceEventSet operator -(ServerTraceEventSet eventSet, ServerTraceEvent anEvent)
	{
		ServerTraceEventSet serverTraceEventSet = new ServerTraceEventSet(eventSet);
		serverTraceEventSet.ResetBit(anEvent);
		return serverTraceEventSet;
	}

	public static ServerTraceEventSet Subtract(ServerTraceEventSet eventSet, ServerTraceEvent anEvent)
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
				stringBuilder.Append(((ServerTraceEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
