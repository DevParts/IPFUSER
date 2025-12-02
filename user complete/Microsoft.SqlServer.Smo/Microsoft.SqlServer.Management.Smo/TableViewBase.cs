using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class TableViewBase : TableViewTableTypeBase, IExtendedProperties, IScriptable
{
	private TriggerCollection m_Triggers;

	private StatisticCollection m_Statistics;

	private List<string> keysForPermissionWithGrantOption;

	internal FullTextIndex m_FullTextIndex;

	internal bool m_bFullTextIndexInitialized;

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Trigger), SfcObjectFlags.Design)]
	public TriggerCollection Triggers
	{
		get
		{
			if (this is View)
			{
				ThrowIfBelowVersion80();
			}
			CheckObjectState();
			if (m_Triggers == null)
			{
				m_Triggers = new TriggerCollection(this);
			}
			return m_Triggers;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Statistic))]
	public StatisticCollection Statistics
	{
		get
		{
			if (this is View)
			{
				ThrowIfBelowVersion80();
			}
			CheckObjectState();
			if (m_Statistics == null)
			{
				m_Statistics = new StatisticCollection(this);
			}
			return m_Statistics;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.ZeroToOne)]
	public FullTextIndex FullTextIndex
	{
		get
		{
			if (this is View)
			{
				ThrowIfBelowVersion90();
			}
			CheckObjectState();
			ThrowIfCloudAndVersionBelow12("FullTextIndex");
			if (this is View)
			{
				this.ThrowIfNotSupported(typeof(View));
			}
			else
			{
				this.ThrowIfNotSupported(typeof(Table));
			}
			if (!m_bFullTextIndexInitialized && base.State != SqlSmoState.Creating && !base.IsDesignMode)
			{
				m_FullTextIndex = InitializeFullTextIndex();
				m_bFullTextIndexInitialized = true;
			}
			return m_FullTextIndex;
		}
	}

	internal TableViewBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_Triggers = null;
		m_Statistics = null;
	}

	protected internal TableViewBase()
	{
	}

	public void UpdateStatistics()
	{
		CheckObjectState(throwIfNotCreated: true);
		UpdateStatistics(StatisticsTarget.All, StatisticsScanType.Default, 0, recompute: true);
	}

	public void UpdateStatistics(StatisticsTarget affectType, StatisticsScanType scanType)
	{
		CheckObjectState(throwIfNotCreated: true);
		UpdateStatistics(affectType, scanType, 0, recompute: true);
	}

	public void UpdateStatistics(StatisticsTarget affectType, StatisticsScanType scanType, int sampleValue)
	{
		CheckObjectState(throwIfNotCreated: true);
		UpdateStatistics(affectType, scanType, sampleValue, recompute: true);
	}

	public void UpdateStatistics(StatisticsTarget affectType, StatisticsScanType scanType, int sampleValue, bool recompute)
	{
		CheckObjectState(throwIfNotCreated: true);
		ExecutionManager.ExecuteNonQuery(Statistic.UpdateStatistics(GetDatabaseName(), FormatFullNameForScripting(new ScriptingPreferences()), "", scanType, affectType, !recompute, sampleValue));
	}

	public DataTable EnumFragmentation()
	{
		return EnumFragmentation(FragmentationOption.Fast);
	}

	public DataTable EnumFragmentation(FragmentationOption fragmentationOption)
	{
		try
		{
			if (this is View && base.ServerVersion.Major < 8)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.ViewFragInfoNotInV7).SetHelpContext("ViewFragInfoNotInV7");
			}
			CheckObjectState();
			string text = string.Format(SmoApplication.DefaultCulture, "{0}/Index/{1}", new object[2]
			{
				base.Urn.Value,
				GetFragOptionString(fragmentationOption)
			});
			Request request = new Request(text);
			request.ParentPropertiesRequests = new PropertiesRequest[1];
			PropertiesRequest propertiesRequest = new PropertiesRequest();
			propertiesRequest.Fields = new string[2] { "Name", "ID" };
			propertiesRequest.OrderByList = new OrderBy[1]
			{
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
			request.ParentPropertiesRequests[0] = propertiesRequest;
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumFragmentation, this, ex);
		}
	}

	public DataTable EnumFragmentation(FragmentationOption fragmentationOption, int partitionNumber)
	{
		try
		{
			if (this is View && base.ServerVersion.Major < 8)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.ViewFragInfoNotInV7).SetHelpContext("ViewFragInfoNotInV7");
			}
			CheckObjectState();
			if (base.ServerVersion.Major < 9)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidParamForVersion("EnumFragmentation", "partitionNumber", GetSqlServerVersionName())).SetHelpContext("InvalidParamForVersion");
			}
			string text = string.Format(SmoApplication.DefaultCulture, "{0}/Index/{1}[@PartitionNumber={2}]", new object[3]
			{
				base.Urn.Value,
				GetFragOptionString(fragmentationOption),
				partitionNumber
			});
			Request request = new Request(text);
			request.ParentPropertiesRequests = new PropertiesRequest[1];
			PropertiesRequest propertiesRequest = new PropertiesRequest();
			propertiesRequest.Fields = new string[2] { "Name", "ID" };
			propertiesRequest.OrderByList = new OrderBy[1]
			{
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
			request.ParentPropertiesRequests[0] = propertiesRequest;
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumFragmentation, this, ex);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		m_bFullTextIndexInitialized = false;
		keysForPermissionWithGrantOption = null;
	}

	public void ReCompileReferences()
	{
		ReCompile(Name, Schema);
	}

	private string GetDatabaseName()
	{
		return SqlSmoObject.MakeSqlBraket(base.ParentColl.ParentInstance.InternalName);
	}

	internal List<string> GetKeysForPermissionWithGrantOptionFromCache()
	{
		if (keysForPermissionWithGrantOption == null)
		{
			keysForPermissionWithGrantOption = new List<string>();
			foreach (UserPermission permission in Permissions)
			{
				if (permission.PermissionState == PermissionState.GrantWithGrant)
				{
					keysForPermissionWithGrantOption.Add(GetKeyToMatchColumnPermissions(permission.ObjectClass.ToString(), permission.Grantee, permission.GranteeType.ToString(), permission.Grantor, permission.GrantorType.ToString(), permission.Code.ToString()));
				}
			}
		}
		return keysForPermissionWithGrantOption;
	}

	internal static string GetKeyToMatchColumnPermissions(string permissionClass, string grantee, string granteeType, string grantor, string grantorType, string permissionName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(permissionClass);
		stringBuilder.Append("_");
		stringBuilder.Append(grantee);
		stringBuilder.Append("_");
		stringBuilder.Append(granteeType);
		stringBuilder.Append("_");
		stringBuilder.Append(grantor);
		stringBuilder.Append("_");
		stringBuilder.Append(grantorType);
		stringBuilder.Append("_");
		stringBuilder.Append(permissionName.ToUpperInvariant());
		return stringBuilder.ToString();
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		AddScriptPermissions(query, PermissionWorker.PermissionEnumKind.Object, sp);
		foreach (Column column in base.Columns)
		{
			column.AddScriptPermissions(query, PermissionWorker.PermissionEnumKind.Column, sp);
		}
	}

	internal void DropFullTextIndexRef()
	{
		m_FullTextIndex = null;
	}

	private FullTextIndex InitializeFullTextIndex()
	{
		Request request = new Request(string.Concat(base.Urn, "/", FullTextIndex.UrnSuffix));
		request.Fields = new string[1] { "UniqueIndexName" };
		DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
		if (1 != enumeratorData.Rows.Count)
		{
			return null;
		}
		if (!(enumeratorData.Rows[0][0] is string name))
		{
			return null;
		}
		return new FullTextIndex(this, new SimpleObjectKey(name), SqlSmoState.Existing);
	}

	internal FullTextIndex InitializeFullTextIndexNoEnum()
	{
		if (m_FullTextIndex == null)
		{
			m_FullTextIndex = new FullTextIndex(this, new SimpleObjectKey(Name), SqlSmoState.Existing);
			m_bFullTextIndexInitialized = true;
		}
		return m_FullTextIndex;
	}
}
