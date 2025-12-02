namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerTraceEvent
{
	private ServerTraceEventValues m_value;

	internal ServerTraceEventValues Value => m_value;

	public static ServerTraceEvent AssemblyLoad => new ServerTraceEvent(ServerTraceEventValues.AssemblyLoad);

	public static ServerTraceEvent AuditAddloginEvent => new ServerTraceEvent(ServerTraceEventValues.AuditAddloginEvent);

	public static ServerTraceEvent AuditAddDBUserEvent => new ServerTraceEvent(ServerTraceEventValues.AuditAddDBUserEvent);

	public static ServerTraceEvent AuditAddLoginToServerRoleEvent => new ServerTraceEvent(ServerTraceEventValues.AuditAddLoginToServerRoleEvent);

	public static ServerTraceEvent AuditAddMemberToDBRoleEvent => new ServerTraceEvent(ServerTraceEventValues.AuditAddMemberToDBRoleEvent);

	public static ServerTraceEvent AuditAddRoleEvent => new ServerTraceEvent(ServerTraceEventValues.AuditAddRoleEvent);

	public static ServerTraceEvent AuditAppRoleChangePasswordEvent => new ServerTraceEvent(ServerTraceEventValues.AuditAppRoleChangePasswordEvent);

	public static ServerTraceEvent AuditBackupRestoreEvent => new ServerTraceEvent(ServerTraceEventValues.AuditBackupRestoreEvent);

	public static ServerTraceEvent AuditChangeAuditEvent => new ServerTraceEvent(ServerTraceEventValues.AuditChangeAuditEvent);

	public static ServerTraceEvent AuditChangeDatabaseOwner => new ServerTraceEvent(ServerTraceEventValues.AuditChangeDatabaseOwner);

	public static ServerTraceEvent AuditDatabaseManagementEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabaseManagementEvent);

	public static ServerTraceEvent AuditDatabaseObjectAccessEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabaseObjectAccessEvent);

	public static ServerTraceEvent AuditDatabaseObjectGdrEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabaseObjectGdrEvent);

	public static ServerTraceEvent AuditDatabaseObjectManagementEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabaseObjectManagementEvent);

	public static ServerTraceEvent AuditDatabaseObjectTakeOwnershipEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabaseObjectTakeOwnershipEvent);

	public static ServerTraceEvent AuditDatabaseOperationEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabaseOperationEvent);

	public static ServerTraceEvent AuditDatabasePrincipalImpersonationEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabasePrincipalImpersonationEvent);

	public static ServerTraceEvent AuditDatabasePrincipalManagementEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabasePrincipalManagementEvent);

	public static ServerTraceEvent AuditDatabaseScopeGdrEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDatabaseScopeGdrEvent);

	public static ServerTraceEvent AuditDbccEvent => new ServerTraceEvent(ServerTraceEventValues.AuditDbccEvent);

	public static ServerTraceEvent AuditFulltext => new ServerTraceEvent(ServerTraceEventValues.AuditFulltext);

	public static ServerTraceEvent AuditLogin => new ServerTraceEvent(ServerTraceEventValues.AuditLogin);

	public static ServerTraceEvent AuditLoginChangePasswordEvent => new ServerTraceEvent(ServerTraceEventValues.AuditLoginChangePasswordEvent);

	public static ServerTraceEvent AuditLoginChangePropertyEvent => new ServerTraceEvent(ServerTraceEventValues.AuditLoginChangePropertyEvent);

	public static ServerTraceEvent AuditLoginFailed => new ServerTraceEvent(ServerTraceEventValues.AuditLoginFailed);

	public static ServerTraceEvent AuditLoginGdrEvent => new ServerTraceEvent(ServerTraceEventValues.AuditLoginGdrEvent);

	public static ServerTraceEvent AuditLogout => new ServerTraceEvent(ServerTraceEventValues.AuditLogout);

	public static ServerTraceEvent AuditSchemaObjectAccessEvent => new ServerTraceEvent(ServerTraceEventValues.AuditSchemaObjectAccessEvent);

	public static ServerTraceEvent AuditSchemaObjectGdrEvent => new ServerTraceEvent(ServerTraceEventValues.AuditSchemaObjectGdrEvent);

	public static ServerTraceEvent AuditSchemaObjectManagementEvent => new ServerTraceEvent(ServerTraceEventValues.AuditSchemaObjectManagementEvent);

	public static ServerTraceEvent AuditSchemaObjectTakeOwnershipEvent => new ServerTraceEvent(ServerTraceEventValues.AuditSchemaObjectTakeOwnershipEvent);

	public static ServerTraceEvent AuditServerAlterTraceEvent => new ServerTraceEvent(ServerTraceEventValues.AuditServerAlterTraceEvent);

	public static ServerTraceEvent AuditServerObjectGdrEvent => new ServerTraceEvent(ServerTraceEventValues.AuditServerObjectGdrEvent);

	public static ServerTraceEvent AuditServerObjectManagementEvent => new ServerTraceEvent(ServerTraceEventValues.AuditServerObjectManagementEvent);

	public static ServerTraceEvent AuditServerObjectTakeOwnershipEvent => new ServerTraceEvent(ServerTraceEventValues.AuditServerObjectTakeOwnershipEvent);

	public static ServerTraceEvent AuditServerOperationEvent => new ServerTraceEvent(ServerTraceEventValues.AuditServerOperationEvent);

	public static ServerTraceEvent AuditServerPrincipalImpersonationEvent => new ServerTraceEvent(ServerTraceEventValues.AuditServerPrincipalImpersonationEvent);

	public static ServerTraceEvent AuditServerPrincipalManagementEvent => new ServerTraceEvent(ServerTraceEventValues.AuditServerPrincipalManagementEvent);

	public static ServerTraceEvent AuditServerScopeGdrEvent => new ServerTraceEvent(ServerTraceEventValues.AuditServerScopeGdrEvent);

	public static ServerTraceEvent BitmapWarning => new ServerTraceEvent(ServerTraceEventValues.BitmapWarning);

	public static ServerTraceEvent BlockedProcessReport => new ServerTraceEvent(ServerTraceEventValues.BlockedProcessReport);

	public static ServerTraceEvent CpuThresholdExceeded => new ServerTraceEvent(ServerTraceEventValues.CpuThresholdExceeded);

	public static ServerTraceEvent DatabaseMirroringStateChange => new ServerTraceEvent(ServerTraceEventValues.DatabaseMirroringStateChange);

	public static ServerTraceEvent DatabaseSuspectDataPage => new ServerTraceEvent(ServerTraceEventValues.DatabaseSuspectDataPage);

	public static ServerTraceEvent DataFileAutoGrow => new ServerTraceEvent(ServerTraceEventValues.DataFileAutoGrow);

	public static ServerTraceEvent DataFileAutoShrink => new ServerTraceEvent(ServerTraceEventValues.DataFileAutoShrink);

	public static ServerTraceEvent DeadlockGraph => new ServerTraceEvent(ServerTraceEventValues.DeadlockGraph);

	public static ServerTraceEvent DeprecationAnnouncement => new ServerTraceEvent(ServerTraceEventValues.DeprecationAnnouncement);

	public static ServerTraceEvent DeprecationFinalSupport => new ServerTraceEvent(ServerTraceEventValues.DeprecationFinalSupport);

	public static ServerTraceEvent Errorlog => new ServerTraceEvent(ServerTraceEventValues.Errorlog);

	public static ServerTraceEvent Eventlog => new ServerTraceEvent(ServerTraceEventValues.Eventlog);

	public static ServerTraceEvent Exception => new ServerTraceEvent(ServerTraceEventValues.Exception);

	public static ServerTraceEvent ExchangeSpillEvent => new ServerTraceEvent(ServerTraceEventValues.ExchangeSpillEvent);

	public static ServerTraceEvent ExecutionWarnings => new ServerTraceEvent(ServerTraceEventValues.ExecutionWarnings);

	public static ServerTraceEvent FtCrawlAborted => new ServerTraceEvent(ServerTraceEventValues.FtCrawlAborted);

	public static ServerTraceEvent FtCrawlStarted => new ServerTraceEvent(ServerTraceEventValues.FtCrawlStarted);

	public static ServerTraceEvent FtCrawlStopped => new ServerTraceEvent(ServerTraceEventValues.FtCrawlStopped);

	public static ServerTraceEvent HashWarning => new ServerTraceEvent(ServerTraceEventValues.HashWarning);

	public static ServerTraceEvent LockDeadlock => new ServerTraceEvent(ServerTraceEventValues.LockDeadlock);

	public static ServerTraceEvent LockDeadlockChain => new ServerTraceEvent(ServerTraceEventValues.LockDeadlockChain);

	public static ServerTraceEvent LockEscalation => new ServerTraceEvent(ServerTraceEventValues.LockEscalation);

	public static ServerTraceEvent LogFileAutoGrow => new ServerTraceEvent(ServerTraceEventValues.LogFileAutoGrow);

	public static ServerTraceEvent LogFileAutoShrink => new ServerTraceEvent(ServerTraceEventValues.LogFileAutoShrink);

	public static ServerTraceEvent MissingColumnStatistics => new ServerTraceEvent(ServerTraceEventValues.MissingColumnStatistics);

	public static ServerTraceEvent MissingJoinPredicate => new ServerTraceEvent(ServerTraceEventValues.MissingJoinPredicate);

	public static ServerTraceEvent MountTape => new ServerTraceEvent(ServerTraceEventValues.MountTape);

	public static ServerTraceEvent ObjectAltered => new ServerTraceEvent(ServerTraceEventValues.ObjectAltered);

	public static ServerTraceEvent ObjectCreated => new ServerTraceEvent(ServerTraceEventValues.ObjectCreated);

	public static ServerTraceEvent ObjectDeleted => new ServerTraceEvent(ServerTraceEventValues.ObjectDeleted);

	public static ServerTraceEvent OledbCallEvent => new ServerTraceEvent(ServerTraceEventValues.OledbCallEvent);

	public static ServerTraceEvent OledbDatareadEvent => new ServerTraceEvent(ServerTraceEventValues.OledbDatareadEvent);

	public static ServerTraceEvent OledbErrors => new ServerTraceEvent(ServerTraceEventValues.OledbErrors);

	public static ServerTraceEvent OledbProviderInformation => new ServerTraceEvent(ServerTraceEventValues.OledbProviderInformation);

	public static ServerTraceEvent OledbQueryinterfaceEvent => new ServerTraceEvent(ServerTraceEventValues.OledbQueryinterfaceEvent);

	public static ServerTraceEvent Qn_dynamics => new ServerTraceEvent(ServerTraceEventValues.Qn_dynamics);

	public static ServerTraceEvent Qn_parameterTable => new ServerTraceEvent(ServerTraceEventValues.Qn_parameterTable);

	public static ServerTraceEvent Qn_subscription => new ServerTraceEvent(ServerTraceEventValues.Qn_subscription);

	public static ServerTraceEvent Qn_template => new ServerTraceEvent(ServerTraceEventValues.Qn_template);

	public static ServerTraceEvent ServerMemoryChange => new ServerTraceEvent(ServerTraceEventValues.ServerMemoryChange);

	public static ServerTraceEvent ShowplanAllForQueryCompile => new ServerTraceEvent(ServerTraceEventValues.ShowplanAllForQueryCompile);

	public static ServerTraceEvent ShowplanXml => new ServerTraceEvent(ServerTraceEventValues.ShowplanXml);

	public static ServerTraceEvent ShowplanXmlForQueryCompile => new ServerTraceEvent(ServerTraceEventValues.ShowplanXmlForQueryCompile);

	public static ServerTraceEvent ShowplanXmlStatisticsProfile => new ServerTraceEvent(ServerTraceEventValues.ShowplanXmlStatisticsProfile);

	public static ServerTraceEvent SortWarnings => new ServerTraceEvent(ServerTraceEventValues.SortWarnings);

	public static ServerTraceEvent SpCacheinsert => new ServerTraceEvent(ServerTraceEventValues.SpCacheinsert);

	public static ServerTraceEvent SpCachemiss => new ServerTraceEvent(ServerTraceEventValues.SpCachemiss);

	public static ServerTraceEvent SpCacheremove => new ServerTraceEvent(ServerTraceEventValues.SpCacheremove);

	public static ServerTraceEvent SpRecompile => new ServerTraceEvent(ServerTraceEventValues.SpRecompile);

	public static ServerTraceEvent SqlStmtrecompile => new ServerTraceEvent(ServerTraceEventValues.SqlStmtrecompile);

	public static ServerTraceEvent TraceFileClose => new ServerTraceEvent(ServerTraceEventValues.TraceFileClose);

	public static ServerTraceEvent TraceAllEvents => new ServerTraceEvent(ServerTraceEventValues.TraceAllEvents);

	public static ServerTraceEvent TraceClr => new ServerTraceEvent(ServerTraceEventValues.TraceClr);

	public static ServerTraceEvent TraceDatabase => new ServerTraceEvent(ServerTraceEventValues.TraceDatabase);

	public static ServerTraceEvent TraceDeprecation => new ServerTraceEvent(ServerTraceEventValues.TraceDeprecation);

	public static ServerTraceEvent TraceErrorsAndWarnings => new ServerTraceEvent(ServerTraceEventValues.TraceErrorsAndWarnings);

	public static ServerTraceEvent TraceFullText => new ServerTraceEvent(ServerTraceEventValues.TraceFullText);

	public static ServerTraceEvent TraceLocks => new ServerTraceEvent(ServerTraceEventValues.TraceLocks);

	public static ServerTraceEvent TraceObjects => new ServerTraceEvent(ServerTraceEventValues.TraceObjects);

	public static ServerTraceEvent TraceOledb => new ServerTraceEvent(ServerTraceEventValues.TraceOledb);

	public static ServerTraceEvent TracePerformance => new ServerTraceEvent(ServerTraceEventValues.TracePerformance);

	public static ServerTraceEvent TraceQueryNotifications => new ServerTraceEvent(ServerTraceEventValues.TraceQueryNotifications);

	public static ServerTraceEvent TraceSecurityAudit => new ServerTraceEvent(ServerTraceEventValues.TraceSecurityAudit);

	public static ServerTraceEvent TraceServer => new ServerTraceEvent(ServerTraceEventValues.TraceServer);

	public static ServerTraceEvent TraceStoredProcedures => new ServerTraceEvent(ServerTraceEventValues.TraceStoredProcedures);

	public static ServerTraceEvent TraceTsql => new ServerTraceEvent(ServerTraceEventValues.TraceTsql);

	public static ServerTraceEvent TraceUserConfigurable => new ServerTraceEvent(ServerTraceEventValues.TraceUserConfigurable);

	public static ServerTraceEvent Userconfigurable0 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable0);

	public static ServerTraceEvent Userconfigurable1 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable1);

	public static ServerTraceEvent Userconfigurable2 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable2);

	public static ServerTraceEvent Userconfigurable3 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable3);

	public static ServerTraceEvent Userconfigurable4 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable4);

	public static ServerTraceEvent Userconfigurable5 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable5);

	public static ServerTraceEvent Userconfigurable6 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable6);

	public static ServerTraceEvent Userconfigurable7 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable7);

	public static ServerTraceEvent Userconfigurable8 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable8);

	public static ServerTraceEvent Userconfigurable9 => new ServerTraceEvent(ServerTraceEventValues.Userconfigurable9);

	public static ServerTraceEvent UserErrorMessage => new ServerTraceEvent(ServerTraceEventValues.UserErrorMessage);

	public static ServerTraceEvent XqueryStaticType => new ServerTraceEvent(ServerTraceEventValues.XqueryStaticType);

	internal ServerTraceEvent(ServerTraceEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator ServerTraceEventSet(ServerTraceEvent eventValue)
	{
		return new ServerTraceEventSet(eventValue);
	}

	public static ServerTraceEventSet operator +(ServerTraceEvent eventLeft, ServerTraceEvent eventRight)
	{
		ServerTraceEventSet serverTraceEventSet = new ServerTraceEventSet(eventLeft);
		serverTraceEventSet.SetBit(eventRight);
		return serverTraceEventSet;
	}

	public static ServerTraceEventSet Add(ServerTraceEvent eventLeft, ServerTraceEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static ServerTraceEventSet operator |(ServerTraceEvent eventLeft, ServerTraceEvent eventRight)
	{
		ServerTraceEventSet serverTraceEventSet = new ServerTraceEventSet(eventLeft);
		serverTraceEventSet.SetBit(eventRight);
		return serverTraceEventSet;
	}

	public static ServerTraceEventSet BitwiseOr(ServerTraceEvent eventLeft, ServerTraceEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(ServerTraceEvent a, ServerTraceEvent b)
	{
		if ((object)a == null && (object)b == null)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.m_value == b.m_value;
	}

	public static bool operator !=(ServerTraceEvent a, ServerTraceEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as ServerTraceEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
