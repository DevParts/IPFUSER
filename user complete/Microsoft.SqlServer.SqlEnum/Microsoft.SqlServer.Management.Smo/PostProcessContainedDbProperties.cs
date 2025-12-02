using System.Data;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessContainedDbProperties : PostProcess
{
	private DataTable dt;

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (GetTriggeredInt32(dp, 0) != 0)
		{
			return data;
		}
		if (dt == null)
		{
			string query = "select \r\ncase \r\n    when cfg.configuration_id = 124 -- configuration id for default language\r\n    then (select lcid from sys.syslanguages as sl where sl.langid = cfg.value_in_use) -- getting default language LCID from default language langid\r\n    else cfg.value_in_use\r\nend as value,\r\ncase \r\n    when cfg.configuration_id = 124 -- configuration id for default language\r\n    then (select name collate catalog_default from sys.syslanguages as sl where sl.langid = cfg.value_in_use) -- getting default language name from default language langid\r\n    when cfg.configuration_id = 1126 -- configuration id for default fulltext language\r\n    then ISNULL((select name collate catalog_default from sys.fulltext_languages as fl where fl.lcid = cfg.value_in_use), N'') -- getting default fulltext language name from default fulltext language lcid\r\n    else null\r\nend as name,\r\ncfg.configuration_id as configuration_id\r\nfrom sys.configurations as cfg\r\nwhere cfg.configuration_id in (115, 124, 1126, 1127, 1555) \r\norder by cfg.configuration_id asc";
			dt = ExecuteSql.ExecuteWithResults(query, base.ConnectionInfo);
		}
		switch (name)
		{
		case "NestedTriggersEnabled":
			data = dt.Rows[0][0];
			break;
		case "DefaultLanguageLcid":
			data = dt.Rows[1][0];
			break;
		case "DefaultLanguageName":
			data = dt.Rows[1][1];
			break;
		case "DefaultFullTextLanguageLcid":
			data = dt.Rows[2][0];
			break;
		case "DefaultFullTextLanguageName":
			data = dt.Rows[2][1];
			break;
		case "TwoDigitYearCutoff":
			data = dt.Rows[3][0];
			break;
		case "TransformNoiseWords":
			data = dt.Rows[4][0];
			break;
		}
		return data;
	}
}
