using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public class CategoryBase : AgentObjectBase, ICreatable, IDroppable, IDropIfExists, IRenamable, IScriptable
{
	internal CategoryBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	protected internal CategoryBase()
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal virtual string GetCategoryClassName()
	{
		return string.Empty;
	}

	internal virtual int GetCategoryClass()
	{
		return 0;
	}

	internal virtual string GetCategoryTypeName()
	{
		return GetCatTypeName(CategoryType.None);
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string objectType = GetType().InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string;
		ScriptIncludeHeaders(stringBuilder, sp, objectType);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_AGENT_CATEGORY, "NOT", SqlSmoObject.SqlString(Name), GetCategoryClass());
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("BEGIN");
			stringBuilder.Append(Globals.newline);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC {0}msdb.dbo.sp_add_category @class=N'{1}', @type=N'{2}', @name=N'{3}'", Job.GetReturnCode(sp), GetCategoryClassName(), GetCategoryTypeName(), SqlSmoObject.SqlString(Name));
		if (sp.Agent.InScriptJob)
		{
			stringBuilder.Append(Globals.newline);
			Job.AddCheckErrorCode(stringBuilder);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("END");
			stringBuilder.Append(Globals.newline);
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string objectType = GetType().InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string;
		ScriptIncludeHeaders(stringBuilder, sp, objectType);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_AGENT_CATEGORY, "", SqlSmoObject.SqlString(Name), GetCategoryClass());
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_category @class=N'{0}', @name=N'{1}'", new object[2]
		{
			GetCategoryClassName(),
			SqlSmoObject.SqlString(Name)
		});
		queries.Add(stringBuilder.ToString());
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection queries, ScriptingPreferences sp, string newName)
	{
		queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_category @class=N'{0}', @name=N'{1}', @new_name=N'{2}'", new object[3]
		{
			GetCategoryClassName(),
			SqlSmoObject.SqlString(Name),
			SqlSmoObject.SqlString(newName)
		}));
	}

	protected string GetCatTypeName(CategoryType ct)
	{
		return ct switch
		{
			CategoryType.LocalJob => "LOCAL", 
			CategoryType.MultiServerJob => "MULTI-SERVER", 
			CategoryType.None => "NONE", 
			_ => throw new InternalSmoErrorException(ExceptionTemplatesImpl.UnknownCategoryType(ct.ToString())), 
		};
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}
}
