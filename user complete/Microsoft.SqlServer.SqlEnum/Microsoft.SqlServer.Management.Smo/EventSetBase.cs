using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class EventSetBase
{
	private BitArray m_storage;

	protected internal BitArray Storage
	{
		get
		{
			return m_storage;
		}
		set
		{
			m_storage = value;
		}
	}

	public abstract int NumberOfElements { get; }

	public EventSetBase()
	{
		m_storage = new BitArray(NumberOfElements);
	}

	public EventSetBase(EventSetBase eventSetBase)
	{
		m_storage = eventSetBase.Storage;
	}

	public abstract EventSetBase Copy();

	internal void SetBitAt(int idx, bool value)
	{
		m_storage[idx] = value;
	}

	internal bool GetBitAt(int idx)
	{
		return m_storage[idx];
	}

	protected void SetValue(EventSetBase options, bool value)
	{
		if (value)
		{
			m_storage.Or(options.m_storage);
			return;
		}
		BitArray bitArray = (BitArray)options.m_storage.Clone();
		m_storage.And(bitArray.Not());
	}

	protected bool FitsMask(EventSetBase mask)
	{
		for (int i = 0; i < mask.NumberOfElements; i++)
		{
			if (mask.m_storage[i] && !m_storage[i])
			{
				return false;
			}
		}
		return true;
	}

	protected bool HasCommonBits(EventSetBase optionsCompare)
	{
		if (optionsCompare == null)
		{
			return true;
		}
		BitArray bitArray = (BitArray)m_storage.Clone();
		BitArray bitArray2 = bitArray.And(optionsCompare.m_storage);
		for (int i = 0; i < NumberOfElements; i++)
		{
			if (bitArray2[i] != optionsCompare.m_storage[i])
			{
				return false;
			}
		}
		return true;
	}
}
