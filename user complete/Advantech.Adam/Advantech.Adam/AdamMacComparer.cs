using System.Collections;

namespace Advantech.Adam;

public class AdamMacComparer : IComparer
{
	int IComparer.Compare(object x, object y)
	{
		AdamInformation adamInformation = (AdamInformation)x;
		AdamInformation adamInformation2 = (AdamInformation)y;
		for (int i = 0; i < 6; i++)
		{
			if (adamInformation.Mac[i] > adamInformation2.Mac[i])
			{
				return 1;
			}
			if (adamInformation.Mac[i] < adamInformation2.Mac[i])
			{
				return -1;
			}
		}
		return 0;
	}
}
