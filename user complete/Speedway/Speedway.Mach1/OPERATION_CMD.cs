using System;
using System.Collections;

namespace Speedway.Mach1;

[Serializable]
public class OPERATION_CMD
{
	public enum SOURCE
	{
		STORE_TO_MODEM,
		RESET_TO_FACTORY_DEFAULT
	}

	public enum FREQUENCY_SET_MODE
	{
		CENTER_FREQUENCY,
		CHOOSE_FROM_REGULATORY,
		CHOOSE_FROM_LIST,
		REDUCED_POWER_FREQUENCY_LIST
	}

	public enum MEASUREMENT_BANDWIDTH
	{
		HZ_100K = 1,
		HZ_300K = 3
	}

	public enum ANTENNA : byte
	{
		ANT1 = 1,
		ANT2 = 2,
		ANT3 = 4,
		ANT4 = 8
	}

	public enum MEMORY_BANK
	{
		RESERVED = 0,
		EPC = 1,
		TID = 2,
		USER = 3,
		DEFAULT = 1
	}

	public enum SET_ANTENNA_RESULT
	{
		SUCCESS,
		PORT_NOT_AVAILABLE
	}

	public enum SET_FREQUENCY_RESULT
	{
		SUCCESS,
		ERROR,
		FREQUENCY_LIST_OUT_OF_RANGE,
		AUTOSET_NOT_VALID
	}

	public enum SET_GEN2_PARAMS_RESULT
	{
		SUCCESS,
		COMBINATION_NOT_SUPPORTED,
		COMBINATION_NOT_SUPPORTED_BY_REGULATORY,
		MODE_ID_NOT_SUPPORTED,
		SESSION_AND_INVENTORY_MODE_COMBINATION_NOT_SUPPORTED
	}

	public enum SET_RX_SENSITIVITY_MODE
	{
		MAXIMUM_SENSITIVITY,
		FIXED_PER_ANTENNA
	}

	public enum SET_TX_POWER_RESULT
	{
		SUCESS,
		ERROR_DISALLOWED,
		ERROR_NOT_SUPPORTED,
		ERROR_REQUIRE_PROFESSIONAL_INSTALLATION
	}

	public enum REPORT_INVENTORY_RESULT
	{
		SUCCESS_BUFFER_WILL_BE_FLUSH,
		SUCCESS_FLUSH_IN_PROCESS,
		SUCCESS_BUFFER_EMPTY,
		ERROR
	}

	public enum INVENTORY_RESULT
	{
		SUCCESS,
		FAIL_CONFIGURATION_ERROR,
		GEN2READLENGTH_EXCEED,
		PROFILE_SEQUENCING_DISABLED,
		PROFILE_SEQUENCE_INDEX_INVALID
	}

	[Serializable]
	public class INVENTORY_PRAMA
	{
		public enum INVENTORY_FILTER_OPERATION
		{
			A,
			A_OR_B,
			A_AND_B,
			NONE
		}

		public enum INVENTORY_HALT_OPERATION
		{
			A,
			A_OR_B,
			A_AND_B,
			HALT_EVERY_TAG
		}

		public enum LOGIC
		{
			EQUALS,
			NOT_EQUAL,
			GREATER_THAN,
			LESS_THAN
		}

		[Serializable]
		public class INVENTORY_FILTER_CONDITION
		{
			public INVENTORY_FILTER_OPERATION filter_operation;

			public MEMORY_BANK a_filter_memory_bank = MEMORY_BANK.EPC;

			public MEMORY_BANK b_filter_memory_bank = MEMORY_BANK.EPC;

			public ushort a_bit_offset;

			public ushort b_bit_offset;

			public ushort a_length;

			public ushort b_length;

			public string a_pattern;

			public string b_pattern;

			public LOGIC a_compare;

			public LOGIC b_compare;

			public INVENTORY_FILTER_CONDITION()
			{
				a_pattern = string.Empty;
				b_pattern = string.Empty;
			}
		}

		[Serializable]
		public class INVENTORY_HALT_CONDITIONS
		{
			public INVENTORY_HALT_OPERATION halt_operation;

			public MEMORY_BANK halt_a_memory_bank = MEMORY_BANK.EPC;

			public MEMORY_BANK halt_b_memory_bank = MEMORY_BANK.EPC;

			public ushort halt_a_bit_offset;

			public ushort halt_b_bit_offset;

			public ushort halt_a_length;

			public ushort halt_b_length;

			public string halt_a_mask;

			public string halt_b_mask;

			public string halt_a_value;

			public string halt_b_value;

			public LOGIC halt_a_compare;

			public LOGIC halt_b_compare;

			public INVENTORY_HALT_CONDITIONS()
			{
				halt_a_mask = string.Empty;
				halt_a_value = string.Empty;
				halt_b_mask = string.Empty;
				halt_b_value = string.Empty;
			}
		}

		public bool enable_inventory_filter;

		public bool enable_halt_filter;

		public INVENTORY_FILTER_CONDITION inventory_filter_condition;

		public INVENTORY_HALT_CONDITIONS inventory_halt_condition;

		public MEMORY_BANK read_memory_bank = MEMORY_BANK.EPC;

		public ushort read_word_memory_address;

		public byte read_length;

		public short estimated_tag_population = -1;

		public short estimated_tag_time_in_field = -1;

		public short emptyFieldTimeOut = -1;

		public short fieldPingInterval = -1;

		public short profileSequenceIndex = -1;

		public bool reportNullEPCs;

		public INVENTORY_PRAMA()
		{
			inventory_filter_condition = new INVENTORY_FILTER_CONDITION();
			inventory_halt_condition = new INVENTORY_HALT_CONDITIONS();
		}

		public INVENTORY_PRAMA(bool enable_inventory_filter, bool enable_halt_filter)
		{
			this.enable_halt_filter = enable_halt_filter;
			this.enable_inventory_filter = enable_inventory_filter;
			inventory_filter_condition = new INVENTORY_FILTER_CONDITION();
			inventory_halt_condition = new INVENTORY_HALT_CONDITIONS();
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[1024];
			int num = 0;
			try
			{
				if (enable_inventory_filter)
				{
					array[num++] = 1;
					array[num++] = (byte)inventory_filter_condition.filter_operation;
					array[num++] = 2;
					array[num++] = (byte)inventory_filter_condition.a_filter_memory_bank;
					array[num++] = 3;
					array[num++] = (byte)(inventory_filter_condition.a_bit_offset >> 8);
					array[num++] = (byte)(inventory_filter_condition.a_bit_offset & 0xFF);
					array[num++] = 4;
					array[num++] = (byte)(inventory_filter_condition.a_length >> 8);
					array[num++] = (byte)(inventory_filter_condition.a_length & 0xFF);
					byte[] array2 = Util.ConvertBinaryStringArrayToBytes(inventory_filter_condition.a_pattern, inventory_filter_condition.a_length);
					if (array2.Length > 0)
					{
						array[num++] = 5;
						array[num++] = (byte)((array2.Length & 0x300) >> 8);
						array[num++] = (byte)(array2.Length & 0xFF);
						for (int i = 0; i < array2.Length; i++)
						{
							array[num++] = array2[i];
						}
					}
					array[num++] = 6;
					array[num++] = (byte)inventory_filter_condition.a_compare;
					switch (inventory_filter_condition.filter_operation)
					{
					case INVENTORY_FILTER_OPERATION.A_OR_B:
					case INVENTORY_FILTER_OPERATION.A_AND_B:
					{
						array[num++] = 7;
						array[num++] = (byte)inventory_filter_condition.b_filter_memory_bank;
						array[num++] = 8;
						array[num++] = (byte)(inventory_filter_condition.b_bit_offset >> 8);
						array[num++] = (byte)(inventory_filter_condition.b_bit_offset & 0xFF);
						array[num++] = 9;
						array[num++] = (byte)(inventory_filter_condition.b_length >> 8);
						array[num++] = (byte)(inventory_filter_condition.b_length & 0xFF);
						byte[] array3 = Util.ConvertBinaryStringArrayToBytes(inventory_filter_condition.b_pattern, inventory_filter_condition.b_length);
						if (array3.Length > 0)
						{
							array[num++] = 10;
							array[num++] = (byte)((array3.Length & 0x300) >> 8);
							array[num++] = (byte)(array3.Length & 0xFF);
							for (int j = 0; j < array3.Length; j++)
							{
								array[num++] = array3[j];
							}
						}
						array[num++] = 11;
						array[num++] = (byte)inventory_filter_condition.b_compare;
						break;
					}
					}
				}
				if (read_memory_bank != MEMORY_BANK.EPC)
				{
					array[num++] = 12;
					array[num++] = (byte)read_memory_bank;
					array[num++] = 13;
					array[num++] = (byte)(read_word_memory_address >> 8);
					array[num++] = (byte)(read_word_memory_address & 0xFF);
					array[num++] = 14;
					array[num++] = read_length;
				}
				if (enable_halt_filter)
				{
					if (inventory_halt_condition.halt_operation == INVENTORY_HALT_OPERATION.HALT_EVERY_TAG)
					{
						array[num++] = 15;
						array[num++] = 0;
						array[num++] = 18;
						array[num++] = 0;
						array[num++] = 0;
					}
					else
					{
						array[num++] = 15;
						array[num++] = (byte)inventory_halt_condition.halt_operation;
						array[num++] = 18;
						array[num++] = (byte)(inventory_halt_condition.halt_a_length >> 8);
						array[num++] = (byte)(inventory_halt_condition.halt_a_length & 0xFF);
						array[num++] = 16;
						array[num++] = (byte)inventory_halt_condition.halt_a_memory_bank;
						array[num++] = 17;
						array[num++] = (byte)(inventory_halt_condition.halt_a_bit_offset >> 8);
						array[num++] = (byte)(inventory_halt_condition.halt_a_bit_offset & 0xFF);
						byte[] array4 = Util.ConvertBinaryStringArrayToBytes(inventory_halt_condition.halt_a_mask, inventory_halt_condition.halt_a_length);
						if (array4.Length > 0)
						{
							array[num++] = 19;
							array[num++] = (byte)((array4.Length & 0x300) >> 8);
							array[num++] = (byte)(array4.Length & 0xFF);
							for (int k = 0; k < array4.Length; k++)
							{
								array[num++] = array4[k];
							}
						}
						byte[] array5 = Util.ConvertBinaryStringArrayToBytes(inventory_halt_condition.halt_a_value, inventory_halt_condition.halt_a_length);
						if (array5.Length > 0)
						{
							array[num++] = 20;
							array[num++] = (byte)((array5.Length & 0x300) >> 8);
							array[num++] = (byte)(array5.Length & 0xFF);
							for (int l = 0; l < array5.Length; l++)
							{
								array[num++] = array5[l];
							}
						}
						array[num++] = 21;
						array[num++] = (byte)inventory_halt_condition.halt_a_compare;
						switch (inventory_halt_condition.halt_operation)
						{
						case INVENTORY_HALT_OPERATION.A_OR_B:
						case INVENTORY_HALT_OPERATION.A_AND_B:
						{
							array[num++] = 22;
							array[num++] = (byte)inventory_halt_condition.halt_b_memory_bank;
							array[num++] = 23;
							array[num++] = (byte)(inventory_halt_condition.halt_b_bit_offset >> 8);
							array[num++] = (byte)(inventory_halt_condition.halt_b_bit_offset & 0xFF);
							array[num++] = 24;
							array[num++] = (byte)(inventory_halt_condition.halt_b_length >> 8);
							array[num++] = (byte)(inventory_halt_condition.halt_b_length & 0xFF);
							array4 = Util.ConvertBinaryStringArrayToBytes(inventory_halt_condition.halt_b_mask, inventory_halt_condition.halt_b_length);
							if (array4.Length > 0)
							{
								array[num++] = 25;
								array[num++] = (byte)((array4.Length & 0x300) >> 8);
								array[num++] = (byte)(array4.Length & 0xFF);
								for (int m = 0; m < array4.Length; m++)
								{
									array[num++] = array4[m];
								}
							}
							byte[] array6 = Util.ConvertBinaryStringArrayToBytes(inventory_halt_condition.halt_b_value, inventory_halt_condition.halt_b_length);
							if (array6.Length > 0)
							{
								array[num++] = 26;
								array[num++] = (byte)((array6.Length & 0x300) >> 8);
								array[num++] = (byte)(array6.Length & 0xFF);
								for (int n = 0; n < array6.Length; n++)
								{
									array[num++] = array6[n];
								}
							}
							array[num++] = 27;
							array[num++] = (byte)inventory_halt_condition.halt_b_compare;
							break;
						}
						}
					}
				}
				if (estimated_tag_population >= 0)
				{
					array[num++] = 28;
					array[num++] = (byte)(estimated_tag_population >> 8);
					array[num++] = (byte)(estimated_tag_population & 0xFF);
				}
				if (estimated_tag_time_in_field >= 0)
				{
					array[num++] = 29;
					array[num++] = (byte)(estimated_tag_time_in_field >> 8);
					array[num++] = (byte)(estimated_tag_time_in_field & 0xFF);
				}
				if (emptyFieldTimeOut >= 0 && fieldPingInterval >= 0)
				{
					array[num++] = 30;
					array[num++] = (byte)(emptyFieldTimeOut >> 8);
					array[num++] = (byte)(emptyFieldTimeOut & 0xFF);
					array[num++] = 31;
					array[num++] = (byte)(fieldPingInterval >> 8);
					array[num++] = (byte)(fieldPingInterval & 0xFF);
				}
				if (profileSequenceIndex > -1)
				{
					array[num++] = 32;
					array[num++] = (byte)(profileSequenceIndex & 0xFF);
				}
				if (reportNullEPCs)
				{
					array[num++] = 33;
					array[num++] = 1;
				}
				byte[] array7;
				if (num == 0)
				{
					array7 = null;
				}
				else
				{
					array7 = new byte[num];
					Array.Copy(array, array7, num);
				}
				return array7;
			}
			catch
			{
				return null;
			}
		}
	}

	[Serializable]
	public class GEN2_PARAM
	{
		public enum SESSION
		{
			SESSION_0,
			SESSION_1,
			SESSION_2,
			SESSION_3
		}

		public enum GEN2_LINK_MODE
		{
			BY_APPLICATION,
			SELF_CONFIGURE_DENSE,
			BY_MODE_ID_OF_APPLICATION,
			SELF_CONFIGURE_SINGLE
		}

		public enum MODEM_MODULATION
		{
			PR_ASK,
			DSB_ASK
		}

		public enum TARI
		{
			US_6_25,
			US_7_14,
			US_8_33,
			US_10_0,
			US_12_5,
			US_16_67,
			US_20_0,
			US_25_0
		}

		public enum PIE
		{
			P_1_5_vs_1,
			P_1_67_vs_1,
			P_2_0_vs_1
		}

		public enum PW
		{
			SHORT,
			LONG
		}

		public enum TR_FREQUENCY
		{
			HZ_40K,
			HZ_64K,
			HZ_80K,
			HZ_128K,
			HZ_160K,
			HZ_213_3K,
			HZ_256K,
			HZ_320K,
			HZ_640K
		}

		public enum TR_LINK_MODULATION
		{
			FM0,
			MILLER_M2,
			MILLER_M4,
			MILLER_M8
		}

		public enum DEVIDE_RATIO
		{
			RATIO_64_3,
			RATIO_8
		}

		public enum INVENTORY_SEARCH_MODE
		{
			DEFAULT,
			SINGLE_TARGET_INVENTORY,
			SINGLE_TARGET_INVENTORY_WITH_SUPPRESSED_DUPLICATE_REDUNDANCY
		}

		public enum MODE_ID
		{
			MAX_THROUGHPUT,
			HYBRID_MODE,
			DENSE_READER_M4,
			DENSE_READER_M8,
			MAX_THROUGH_PUT_MILLER
		}

		public SESSION session = SESSION.SESSION_1;

		public GEN2_LINK_MODE auto_set_mode = GEN2_LINK_MODE.SELF_CONFIGURE_DENSE;

		public MODEM_MODULATION modem_modulation;

		public TARI tari = TARI.US_12_5;

		public PIE pie;

		public PW pw;

		public TR_FREQUENCY tag_to_reader_link_rate = TR_FREQUENCY.HZ_160K;

		public TR_LINK_MODULATION tag_to_reader_link_modulation;

		public bool handle_reporting;

		public DEVIDE_RATIO devide_ratio;

		public MODE_ID mode_id;

		public INVENTORY_SEARCH_MODE inv_search_mode;

		public GEN2_PARAM()
		{
		}

		public GEN2_PARAM(byte[] data)
		{
			try
			{
				session = (SESSION)data[0];
				auto_set_mode = (GEN2_LINK_MODE)data[1];
				int num = 2;
				while (num < data.Length)
				{
					switch (data[num])
					{
					case 1:
						num++;
						modem_modulation = (MODEM_MODULATION)data[num];
						num++;
						break;
					case 2:
						num++;
						tari = (TARI)data[num];
						num++;
						break;
					case 3:
						num++;
						pie = (PIE)data[num];
						num++;
						break;
					case 4:
						num++;
						pw = (PW)data[num];
						num++;
						break;
					case 5:
						num++;
						tag_to_reader_link_rate = (TR_FREQUENCY)data[num];
						num++;
						break;
					case 6:
						num++;
						tag_to_reader_link_modulation = (TR_LINK_MODULATION)data[num];
						num++;
						break;
					case 7:
						num++;
						handle_reporting = data[num] == 1;
						num++;
						break;
					case 8:
						num++;
						devide_ratio = (DEVIDE_RATIO)data[num];
						num++;
						break;
					case 9:
						num++;
						mode_id = (MODE_ID)(ushort)(data[num] * 256 + data[num + 1]);
						num += 2;
						break;
					case 10:
						num++;
						inv_search_mode = (INVENTORY_SEARCH_MODE)data[num];
						num++;
						break;
					}
				}
			}
			catch
			{
			}
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[1024];
			int num = 0;
			array[num++] = (byte)session;
			array[num++] = (byte)auto_set_mode;
			if (auto_set_mode == GEN2_LINK_MODE.BY_APPLICATION)
			{
				array[num++] = 1;
				array[num++] = (byte)modem_modulation;
				array[num++] = 2;
				array[num++] = (byte)tari;
				array[num++] = 3;
				array[num++] = (byte)pie;
				array[num++] = 4;
				array[num++] = (byte)pw;
				array[num++] = 5;
				array[num++] = (byte)tag_to_reader_link_rate;
				array[num++] = 6;
				array[num++] = (byte)tag_to_reader_link_modulation;
				array[num++] = 7;
				array[num++] = (byte)(handle_reporting ? 1u : 0u);
				array[num++] = 8;
				array[num++] = (byte)devide_ratio;
			}
			else if (auto_set_mode == GEN2_LINK_MODE.BY_MODE_ID_OF_APPLICATION)
			{
				array[num++] = 9;
				array[num++] = (byte)((ushort)mode_id >> 8);
				array[num++] = (byte)((ushort)mode_id & 0xFF);
			}
			array[num++] = 10;
			array[num++] = (byte)inv_search_mode;
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, num);
			return array2;
		}
	}

	[Serializable]
	public class SUPPORTED_GEN2_PARAMS_RSP
	{
		public ushort num_supported_sets;

		public GEN2_PARAM.MODEM_MODULATION[] modulations;

		public GEN2_PARAM.TARI[] taris;

		public GEN2_PARAM.PIE[] pies;

		public GEN2_PARAM.PW[] pws;

		public GEN2_PARAM.TR_FREQUENCY[] tr_link_frequencyes;

		public GEN2_PARAM.TR_LINK_MODULATION[] tr_link_modulations;

		public GEN2_PARAM.DEVIDE_RATIO[] d_ratios;

		public GEN2_PARAM.MODE_ID[] mode_ids;

		public GEN2_PARAM.INVENTORY_SEARCH_MODE[] inv_search_modes;

		public SUPPORTED_GEN2_PARAMS_RSP(byte[] data)
		{
			int num = 0;
			try
			{
				num_supported_sets = data[0];
				num++;
				int num2 = data[num] * 256 + data[num + 1];
				num += 2;
				modulations = new GEN2_PARAM.MODEM_MODULATION[num2];
				for (int i = 0; i < num2; i++)
				{
					modulations[i] = (GEN2_PARAM.MODEM_MODULATION)data[num++];
				}
				num2 = data[num++] * 256 + data[num++];
				taris = new GEN2_PARAM.TARI[num2];
				for (int j = 0; j < num2; j++)
				{
					taris[j] = (GEN2_PARAM.TARI)data[num++];
				}
				num2 = data[num++] * 256 + data[num++];
				pies = new GEN2_PARAM.PIE[num2];
				for (int k = 0; k < num2; k++)
				{
					pies[k] = (GEN2_PARAM.PIE)data[num++];
				}
				num2 = data[num++] * 256 + data[num++];
				pws = new GEN2_PARAM.PW[num2];
				for (int l = 0; l < num2; l++)
				{
					pws[l] = (GEN2_PARAM.PW)data[num++];
				}
				num2 = data[num++] * 256 + data[num++];
				tr_link_frequencyes = new GEN2_PARAM.TR_FREQUENCY[num2];
				for (int m = 0; m < num2; m++)
				{
					tr_link_frequencyes[m] = (GEN2_PARAM.TR_FREQUENCY)data[num++];
				}
				num2 = data[num++] * 256 + data[num++];
				tr_link_modulations = new GEN2_PARAM.TR_LINK_MODULATION[num2];
				for (int n = 0; n < num2; n++)
				{
					tr_link_modulations[n] = (GEN2_PARAM.TR_LINK_MODULATION)data[num++];
				}
				num2 = data[num++] * 256 + data[num++];
				d_ratios = new GEN2_PARAM.DEVIDE_RATIO[num2];
				for (int num3 = 0; num3 < num2; num3++)
				{
					d_ratios[num3] = (GEN2_PARAM.DEVIDE_RATIO)data[num++];
				}
				while (num < data.Length)
				{
					if (num < data.Length && data[num] == 1)
					{
						try
						{
							num++;
							num2 = data[num++] * 256 + data[num++];
							mode_ids = new GEN2_PARAM.MODE_ID[num2];
							for (int num4 = 0; num4 < num2; num4++)
							{
								mode_ids[num4] = (GEN2_PARAM.MODE_ID)data[num++];
							}
						}
						catch
						{
						}
					}
					if (num >= data.Length || data[num] != 2)
					{
						continue;
					}
					try
					{
						num++;
						num2 = data[num++] * 256 + data[num++];
						inv_search_modes = new GEN2_PARAM.INVENTORY_SEARCH_MODE[num2];
						for (int num5 = 0; num5 < num2; num5++)
						{
							inv_search_modes[num5] = (GEN2_PARAM.INVENTORY_SEARCH_MODE)data[num++];
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class OCS_VERSION_RSP
	{
		public string version = string.Empty;

		public OCS_VERSION_RSP(byte[] data)
		{
			try
			{
				version = $"v.{data[0]}.{data[1]}.{data[2]}";
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class TAG_LOCK_OPERATION
	{
		public class OPERATION
		{
			public bool enable_read_writeble_bit;

			public bool read_writeble;

			public bool enable_perma_lock_bit;

			public bool perma_lock;
		}

		public OPERATION kill_pwd;

		public OPERATION access_pwd;

		public OPERATION epc_memory;

		public OPERATION tid_memory;

		public OPERATION user_memory;

		public TAG_LOCK_OPERATION()
		{
			kill_pwd = new OPERATION();
			access_pwd = new OPERATION();
			epc_memory = new OPERATION();
			tid_memory = new OPERATION();
			user_memory = new OPERATION();
		}

		public byte[] ToByteArray()
		{
			BitArray bitArray = new BitArray(32);
			bitArray.SetAll(value: false);
			bitArray.Set(12, kill_pwd.enable_read_writeble_bit);
			bitArray.Set(13, kill_pwd.enable_perma_lock_bit);
			bitArray.Set(14, access_pwd.enable_read_writeble_bit);
			bitArray.Set(15, access_pwd.enable_perma_lock_bit);
			bitArray.Set(16, epc_memory.enable_read_writeble_bit);
			bitArray.Set(17, epc_memory.enable_perma_lock_bit);
			bitArray.Set(18, tid_memory.enable_read_writeble_bit);
			bitArray.Set(19, tid_memory.enable_perma_lock_bit);
			bitArray.Set(20, user_memory.enable_read_writeble_bit);
			bitArray.Set(21, user_memory.enable_perma_lock_bit);
			bitArray.Set(22, kill_pwd.read_writeble);
			bitArray.Set(23, kill_pwd.perma_lock);
			bitArray.Set(24, access_pwd.read_writeble);
			bitArray.Set(25, access_pwd.perma_lock);
			bitArray.Set(26, epc_memory.read_writeble);
			bitArray.Set(27, epc_memory.perma_lock);
			bitArray.Set(28, tid_memory.read_writeble);
			bitArray.Set(29, tid_memory.perma_lock);
			bitArray.Set(30, user_memory.read_writeble);
			bitArray.Set(31, user_memory.perma_lock);
			try
			{
				string text = string.Empty;
				for (int i = 0; i < bitArray.Length; i++)
				{
					text += (bitArray[i] ? "1" : "0");
				}
				return Util.ConvertBinaryStringArrayToBytes(text, 0);
			}
			catch
			{
				return null;
			}
		}
	}

	[Serializable]
	public class OPTIONAL_INVENTORY_REPORT_PARAM
	{
		public enum INVENTORY_REPORTING_MODE
		{
			IMMEDIATE_REPORT,
			ACCUMULATED_REPORT
		}

		public enum ADD_BEHAVIOR
		{
			DONT_REPORT_WHEN_ADDED,
			REPORT_WHEN_ADDED
		}

		public enum INVENTORY_ATTEMPT_COUNT_REPORTING
		{
			DONT_REPORT,
			REPORT
		}

		public enum DROP_BEHAVIOR
		{
			DONT_REPORT_WHEN_DROPPED,
			REPORT_WHEN_DROPPED
		}

		public enum BUFFER_FULL_BEHAVIOR
		{
			DROP_NEWEST_INVENOTRY_NTF,
			DROP_ALL_ENTRIES
		}

		public INVENTORY_REPORTING_MODE inventory_report_mode = INVENTORY_REPORTING_MODE.ACCUMULATED_REPORT;

		public ADD_BEHAVIOR add_behavior;

		public INVENTORY_ATTEMPT_COUNT_REPORTING inventory_attemp_count_reporting;

		public DROP_BEHAVIOR drop_behavior;

		public BUFFER_FULL_BEHAVIOR buffer_full_behavior = BUFFER_FULL_BEHAVIOR.DROP_ALL_ENTRIES;
	}

	public const byte GET_OCS_VERSION = 0;

	public const byte LOAD_FROM_PROFILE = 1;

	public const byte STORE_TO_PROFILE = 2;

	public const byte SET_TX_POWER = 5;

	public const byte GET_TX_POWER = 6;

	public const byte SET_ANTENNA = 7;

	public const byte GET_ANTENNA = 8;

	public const byte SET_REGULATORY_REGION = 9;

	public const byte GET_CAPABILITY = 10;

	public const byte SET_TX_FREQUENCY = 11;

	public const byte GET_TX_FREQUENCY = 12;

	public const byte SET_GEN2_PARAMS = 13;

	public const byte GET_GEN2_PARAMS = 14;

	public const byte GET_SUPPORTED_GEN2_PARAMS = 28;

	public const byte CHECK_ANTENNA = 29;

	public const byte SET_INVENTORY_REPORT = 30;

	public const byte SET_LBT_PARAMS = 31;

	public const byte GET_LBT_PARAMS = 32;

	public const byte INVENTORY = 19;

	public const byte INVENTORY_CONTINUE = 21;

	public const byte MODEM_STOP = 22;

	public const byte RF_SURVEY = 20;

	public const byte TAG_READ = 23;

	public const byte TAG_WRITE = 24;

	public const byte TAG_LOCK = 25;

	public const byte TAG_KILL = 26;

	public const byte TAG_CUSTOM = 27;

	public const byte REPORT_INVENTORY = 33;

	public const byte SET_RX_CONFIG = 34;

	public const byte GET_RX_CONFIG = 35;

	public const byte SET_PROFILE_SEQUENCE = 36;

	public const byte ANTENNA_PORT_1 = 1;

	public const byte ANTENNA_PORT_2 = 2;

	public const byte ANTENNA_PORT_3 = 4;

	public const byte ANTENNA_PORT_4 = 8;

	public static byte[] GENERATE_GET_OCS_VERSION_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 0, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_LOAD_FROM_PROFILE_CMD(bool include_timestamp, ushort profile_index)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 1, include_timestamp, new byte[1] { (byte)profile_index });
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_STORE_TO_PROFILE_CMD(bool include_timestamp, ushort idx, SOURCE dst)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 2, include_timestamp, new byte[2]
		{
			(byte)idx,
			(byte)dst
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_INVENTORY_CMD(bool include_timestamp, INVENTORY_PRAMA prama)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 19, include_timestamp, prama.ToByteArray());
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_TX_POWER_CMD(bool include_timestamp, byte[] txPower)
	{
		byte[] array;
		if (txPower.Length == 1)
		{
			array = new byte[1] { txPower[0] };
		}
		else
		{
			array = new byte[txPower.Length + 3];
			array[0] = txPower[0];
			array[1] = 1;
			array[2] = 0;
			array[3] = (byte)(txPower.Length - 1);
			Array.Copy(txPower, 1, array, 4, txPower.Length - 1);
		}
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 5, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_TX_POWER_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 6, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_ANTENNA_CMD(bool include_timerstamp, byte antennas)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 7, include_timerstamp, new byte[1] { antennas });
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_ANTENNA_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 8, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_REGULATORY_REGION(bool include_timestamp, REGULATORY_REGION region)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 9, include_timestamp, new byte[2]
		{
			(byte)((ushort)region >> 8),
			(byte)((ushort)region & 0xFF)
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_TX_FREQUENCY(bool include_timestamp, FREQUENCY_SET_MODE mode, ushort center_frequency_index, ushort[] frequency_list, ushort[] reduced_power_frequence_list)
	{
		byte[] array;
		switch (mode)
		{
		case FREQUENCY_SET_MODE.CENTER_FREQUENCY:
			array = new byte[4]
			{
				0,
				1,
				(byte)(center_frequency_index >> 8),
				(byte)(center_frequency_index & 0xFF)
			};
			break;
		case FREQUENCY_SET_MODE.CHOOSE_FROM_LIST:
		{
			array = new byte[5 + frequency_list.Length];
			array[0] = (byte)mode;
			array[1] = 2;
			array[2] = (byte)(((frequency_list.Length * 2) & 0xFF00) >> 8);
			array[3] = (byte)((frequency_list.Length * 2) & 0xFF);
			for (int j = 0; j < frequency_list.Length; j++)
			{
				array[4 + 2 * j] = (byte)(frequency_list[j] >> 8);
				array[5 + 2 * j] = (byte)(frequency_list[j] & 0xFF);
			}
			break;
		}
		case FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST:
		{
			array = new byte[5 + reduced_power_frequence_list.Length];
			array[0] = (byte)mode;
			array[1] = 2;
			array[2] = (byte)(((reduced_power_frequence_list.Length * 2) & 0xFF00) >> 8);
			array[3] = (byte)((reduced_power_frequence_list.Length * 2) & 0xFF);
			for (int i = 0; i < reduced_power_frequence_list.Length; i++)
			{
				array[4 + 2 * i] = (byte)(reduced_power_frequence_list[i] >> 8);
				array[5 + 2 * i] = (byte)(reduced_power_frequence_list[i] & 0xFF);
			}
			break;
		}
		case FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY:
			array = new byte[1] { (byte)mode };
			break;
		default:
			array = new byte[1] { 1 };
			break;
		}
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 11, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_TX_FREQUENCY(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 12, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_GEN2_PARAMS_CMD(bool include_timestamp, GEN2_PARAM param)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 13, include_timestamp, param.ToByteArray());
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_GEN2_PARAMS_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 14, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_GEN2_PARAMS_CMD(bool report_search_mode, bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 14, include_timestamp, new byte[2]
		{
			1,
			(byte)(report_search_mode ? 1u : 0u)
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_SUPPORTED_GEN2_PARAMS_CMD(bool include_timestamp, bool includeModeId, bool includeInventorySearchMode)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 28, include_timestamp, new byte[4]
		{
			1,
			(byte)(includeModeId ? 1u : 0u),
			2,
			(byte)(includeInventorySearchMode ? 1u : 0u)
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_CHECK_ANTENNA_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 29, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_CAPABILITIES_CMD(bool include_timestamp, bool reportPower, bool frequey)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 10, include_timestamp, new byte[4]
		{
			1,
			(byte)(reportPower ? 1u : 0u),
			2,
			(byte)(frequey ? 1u : 0u)
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_INVENTORY_REPORT(bool include_timestamp, bool reportStatus, OPTIONAL_INVENTORY_REPORT_PARAM param)
	{
		byte[] array = new byte[11]
		{
			(!reportStatus) ? ((byte)1) : ((byte)0),
			1,
			(byte)param.inventory_report_mode,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};
		if (param.inventory_report_mode == OPTIONAL_INVENTORY_REPORT_PARAM.INVENTORY_REPORTING_MODE.ACCUMULATED_REPORT)
		{
			array[3] = 2;
			array[4] = (byte)param.add_behavior;
			array[5] = 3;
			array[6] = (byte)param.inventory_attemp_count_reporting;
			array[7] = 4;
			array[8] = (byte)param.drop_behavior;
			array[9] = 5;
			array[10] = (byte)param.buffer_full_behavior;
			MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 30, include_timestamp, array);
			return mACH1_FRAME.PACKET;
		}
		byte[] array2 = new byte[3];
		Array.Copy(array, array2, 3);
		MACH1_FRAME mACH1_FRAME2 = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 30, include_timestamp, array2);
		return mACH1_FRAME2.PACKET;
	}

	public static byte[] GENERATE_SET_LBT_PARAMS_CMD(bool include_timestamp, bool autoSet)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 31, include_timestamp, new byte[1] { (!autoSet) ? ((byte)1) : ((byte)0) });
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_LBT_PARAMS_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 32, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_INVENTORY_CONTINUE_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 21, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_RX_CONFIG_CMD(SET_RX_SENSITIVITY_MODE mode, short[] sensitivities, bool include_timestamp)
	{
		byte[] array;
		if (mode == SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY)
		{
			array = new byte[1] { 0 };
		}
		else
		{
			array = new byte[4 + sensitivities.Length];
			array[0] = 1;
			array[1] = 1;
			array[2] = (byte)((sensitivities.Length & 0xFF00) >> 8);
			array[3] = (byte)(sensitivities.Length & 0xFF);
			for (int i = 0; i < sensitivities.Length; i++)
			{
				array[4 + i] = (byte)((-sensitivities[i] - 1) ^ 0xFF);
			}
		}
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 34, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_RX_CONFIG_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 35, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_MODEM_STOP_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 22, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_RF_SURVEY_CMD(bool include_timestamp, ushort low_frequency_index, ushort high_frequency_index, MEASUREMENT_BANDWIDTH bw, byte antennas, ushort sampleCount)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 20, include_timestamp, new byte[8]
		{
			(byte)(low_frequency_index >> 8),
			(byte)(low_frequency_index & 0xFF),
			(byte)(high_frequency_index >> 8),
			(byte)(high_frequency_index & 0xFF),
			(byte)bw,
			antennas,
			(byte)(sampleCount >> 8),
			(byte)(sampleCount & 0xFF)
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_TAG_READ_CMD(bool include_timestamp, MEMORY_BANK mb, ushort addr, byte len)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 23, include_timestamp, new byte[4]
		{
			(byte)mb,
			(byte)(addr >> 8),
			(byte)(addr & 0xFF),
			len
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_TAG_READ_CMD(bool include_timestamp, MEMORY_BANK mb, ushort addr, byte len, uint pass)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 23, include_timestamp, new byte[9]
		{
			(byte)mb,
			(byte)(addr >> 8),
			(byte)(addr & 0xFF),
			len,
			1,
			(byte)(pass >> 24),
			(byte)((pass & 0xFF0000) >> 16),
			(byte)((pass & 0xFF00) >> 8),
			(byte)(pass & 0xFF)
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_TAG_WRITE_CMD(bool include_timestamp, MEMORY_BANK mb, ushort addr, ushort[] data, bool block_write)
	{
		int num = 2 * data.Length;
		byte[] array = new byte[6 + num];
		array[0] = (byte)mb;
		array[1] = (byte)(addr >> 8);
		array[2] = (byte)(addr & 0xFF);
		array[3] = (byte)((num & 0x300) >> 8);
		array[4] = (byte)(num & 0xFF);
		for (int i = 0; i < data.Length; i++)
		{
			array[5 + 2 * i] = (byte)(data[i] >> 8);
			array[6 + 2 * i] = (byte)(data[i] & 0xFF);
		}
		array[5 + num] = ((!block_write) ? ((byte)1) : ((byte)0));
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 24, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_TAG_WRITE_CMD(bool include_timestamp, MEMORY_BANK mb, ushort addr, ushort[] data, bool block_write, uint pass)
	{
		int num = 2 * data.Length;
		byte[] array = new byte[11 + num];
		array[0] = (byte)mb;
		array[1] = (byte)(addr >> 8);
		array[2] = (byte)(addr & 0xFF);
		array[3] = (byte)((num & 0x300) >> 8);
		array[4] = (byte)(num & 0xFF);
		for (int i = 0; i < data.Length; i++)
		{
			array[5 + 2 * i] = (byte)(data[i] >> 8);
			array[6 + 2 * i] = (byte)(data[i] & 0xFF);
		}
		array[5 + num] = ((!block_write) ? ((byte)1) : ((byte)0));
		array[6 + num] = 1;
		array[7 + num] = (byte)(pass >> 24);
		array[8 + num] = (byte)((pass & 0xFF0000) >> 16);
		array[9 + num] = (byte)((pass & 0xFF00) >> 8);
		array[10 + num] = (byte)(pass & 0xFF);
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 24, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_TAG_LOCK_CMD(bool include_timestamp, TAG_LOCK_OPERATION tlo)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 25, include_timestamp, tlo.ToByteArray());
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_TAG_LOCK_CMD(bool include_timestamp, TAG_LOCK_OPERATION tlo, uint pass)
	{
		byte[] array = new byte[9];
		Array.Copy(tlo.ToByteArray(), array, 4);
		array[4] = 1;
		array[5] = (byte)(pass >> 24);
		array[6] = (byte)((pass & 0xFF0000) >> 16);
		array[7] = (byte)((pass & 0xFF00) >> 8);
		array[8] = (byte)(pass & 0xFF);
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 25, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_TAG_KILL_CMD(bool include_timestamp, uint pass)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 26, include_timestamp, new byte[4]
		{
			(byte)(pass >> 24),
			(byte)((pass & 0xFF0000) >> 16),
			(byte)((pass & 0xFF00) >> 8),
			(byte)(pass & 0xFF)
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_REPORT_INVENTORY_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.OPERATION_MODEL, 33, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_PROFILE_SEQUENCE_CMD(bool include_timestamp, bool enabled, ArrayList sequence, ArrayList durations)
	{
		if (sequence.Count != durations.Count)
		{
			return null;
		}
		byte[] array = ((!enabled) ? new byte[1] : new byte[7 + sequence.Count + durations.Count * 2]);
		int num = 0;
		array[num++] = (byte)(enabled ? 1u : 0u);
		if (enabled)
		{
			array[num++] = 1;
			array[num++] = (byte)(sequence.Count >> 8);
			array[num++] = (byte)(sequence.Count & 0xFF);
			for (int i = 0; i < sequence.Count; i++)
			{
				array[num++] = (byte)sequence[i];
			}
			array[num++] = 2;
			array[num++] = (byte)(durations.Count >> 8);
			array[num++] = (byte)(durations.Count & 0xFF);
			for (int j = 0; j < durations.Count; j++)
			{
				array[num++] = (byte)((int)durations[j] >> 8);
				array[num++] = (byte)((int)durations[j] & 0xFF);
			}
		}
		return array;
	}
}
