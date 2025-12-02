namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessAuditProperties : PostProcess
{
	private int maximumFileSizeInAcceptedRange = -1;

	private AuditFileSizeUnit maximumFileSizeUnit;

	protected override bool SupportDataReader => false;

	private PostProcessAuditProperties()
	{
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		switch (name)
		{
		case "MaximumFileSize":
			data = GetMaximumFileSize(dp);
			break;
		case "MaximumFileSizeUnit":
			data = GetMaximumFileSizeUnit(dp);
			break;
		}
		return data;
	}

	private int GetMaximumFileSize(DataProvider dp)
	{
		if (maximumFileSizeInAcceptedRange < 0)
		{
			GetMaxFileSizeValueInAcceptedRangeAndUnit(dp);
		}
		return maximumFileSizeInAcceptedRange;
	}

	private AuditFileSizeUnit GetMaximumFileSizeUnit(DataProvider dp)
	{
		if (maximumFileSizeInAcceptedRange < 0)
		{
			GetMaxFileSizeValueInAcceptedRangeAndUnit(dp);
		}
		return maximumFileSizeUnit;
	}

	private long GetMaximumFileSizeInMegaBytes(DataProvider dp)
	{
		if (GetTriggeredObject(dp, 0) != null)
		{
			return (long)GetTriggeredObject(dp, 0);
		}
		return -1L;
	}

	private void GetMaxFileSizeValueInAcceptedRangeAndUnit(DataProvider dp)
	{
		long num = 0L;
		maximumFileSizeUnit = AuditFileSizeUnit.Mb;
		num = GetMaximumFileSizeInMegaBytes(dp);
		maximumFileSizeUnit = ConvertFileSizeToAcceptedFormat1(ref num);
		maximumFileSizeInAcceptedRange = (int)num;
	}

	private AuditFileSizeUnit ConvertFileSizeToAcceptedFormat1(ref long maxFileSize)
	{
		long num = maxFileSize;
		if (maxFileSize > int.MaxValue)
		{
			double num2 = num / 1024;
			maxFileSize = (long)num2;
			if ((double)maxFileSize < num2)
			{
				maxFileSize++;
			}
			if (maxFileSize > int.MaxValue)
			{
				num2 = num / 1048576;
				maxFileSize = (int)num2;
				if ((double)maxFileSize < num2)
				{
					maxFileSize++;
				}
				if (maxFileSize > int.MaxValue)
				{
					maxFileSize = 0L;
					return AuditFileSizeUnit.Mb;
				}
				return AuditFileSizeUnit.Tb;
			}
			return AuditFileSizeUnit.Gb;
		}
		return AuditFileSizeUnit.Mb;
	}
}
