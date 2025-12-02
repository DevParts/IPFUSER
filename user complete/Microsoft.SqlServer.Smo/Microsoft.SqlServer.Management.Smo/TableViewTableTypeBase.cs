using System;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class TableViewTableTypeBase : ScriptSchemaObjectBase, IExtendedProperties, IScriptable
{
	private IndexCollection m_Indexes;

	private ColumnCollection m_Columns;

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcKey(1)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	[SfcKey(0)]
	[SfcReference(typeof(Schema), typeof(SchemaCustomResolver), "Resolve", new string[] { })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[CLSCompliant(false)]
	public override string Schema
	{
		get
		{
			return base.Schema;
		}
		set
		{
			base.Schema = value;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Index), SfcObjectFlags.Design)]
	public virtual IndexCollection Indexes
	{
		get
		{
			if (this is View)
			{
				ThrowIfBelowVersion80();
			}
			CheckObjectState();
			if (m_Indexes == null)
			{
				m_Indexes = new IndexCollection(this);
				if (this is UserDefinedTableType && base.State == SqlSmoState.Existing)
				{
					m_Indexes.LockCollection(ExceptionTemplatesImpl.ReasonObjectAlreadyCreated(UserDefinedTableType.UrnSuffix));
				}
			}
			return m_Indexes;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(Column), SfcObjectFlags.NaturalOrder | SfcObjectFlags.Design)]
	public ColumnCollection Columns
	{
		get
		{
			CheckObjectState();
			if (m_Columns == null)
			{
				m_Columns = new ColumnCollection(this);
				if (this is View view)
				{
					SetCollectionTextMode(view.TextMode, m_Columns);
				}
			}
			return m_Columns;
		}
	}

	internal TableViewTableTypeBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_Indexes = null;
	}

	protected internal TableViewTableTypeBase()
	{
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		AddScriptPermissions(query, PermissionWorker.PermissionEnumKind.Object, sp);
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_Indexes != null)
		{
			m_Indexes.MarkAllDropped();
		}
		if (m_ExtendedProperties != null)
		{
			m_ExtendedProperties.MarkAllDropped();
		}
		if (m_Columns != null)
		{
			m_Columns.MarkAllDropped();
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public override void Refresh()
	{
		base.Refresh();
	}
}
