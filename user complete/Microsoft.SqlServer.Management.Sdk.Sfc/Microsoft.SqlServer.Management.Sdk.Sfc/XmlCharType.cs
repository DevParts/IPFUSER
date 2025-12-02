using System.Text;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal sealed class XmlCharType
{
	public const int FWHITESPACE = 1;

	public const int FLETTER = 2;

	public const int FSTARTNAME = 4;

	public const int FNAME = 8;

	public const int FCHARDATA = 16;

	public const int FPUBLICID = 32;

	public const char MAXWCHAR = '\uffff';

	public const int MAXCHARDATA = 1114111;

	private const string s_PublicID = "\n\n\r\r !#%';==?Z__az";

	private static byte[] s_CharProperties;

	static XmlCharType()
	{
		s_CharProperties = new byte[65536];
		StringBuilder stringBuilder = new StringBuilder(65536);
		SetProperties("\t\n\r\r  ", 1);
		stringBuilder.Append("AZazÀÖØö");
		stringBuilder.Append("øıĴľŁňŊž");
		stringBuilder.Append("ƀǃǍǰǴǵǺȗ");
		stringBuilder.Append("ɐʨʻˁΆΆΈΊ");
		stringBuilder.Append("ΌΌΎΡΣώϐϖ");
		stringBuilder.Append("ϚϚϜϜϞϞϠϠ");
		stringBuilder.Append("ϢϳЁЌЎяёќ");
		stringBuilder.Append("ўҁҐӄӇӈӋӌ");
		stringBuilder.Append("ӐӫӮӵӸӹԱՖ");
		stringBuilder.Append("ՙՙաֆאתװײ");
		stringBuilder.Append("ءغفيٱڷںھ");
		stringBuilder.Append("ۀێېۓەەۥۦ");
		stringBuilder.Append("अहऽऽक़ॡঅঌ");
		stringBuilder.Append("এঐওনপরলল");
		stringBuilder.Append("শহড়ঢ়য়ৡৰৱ");
		stringBuilder.Append("ਅਊਏਐਓਨਪਰ");
		stringBuilder.Append("ਲਲ਼ਵਸ਼ਸਹਖ਼ੜ");
		stringBuilder.Append("ਫ਼ਫ਼ੲੴઅઋઍઍ");
		stringBuilder.Append("એઑઓનપરલળ");
		stringBuilder.Append("વહઽઽૠૠଅଌ");
		stringBuilder.Append("ଏଐଓନପରଲଳ");
		stringBuilder.Append("ଶହଽଽଡ଼ଢ଼ୟୡ");
		stringBuilder.Append("அஊஎஐஒகஙச");
		stringBuilder.Append("ஜஜஞடணதநப");
		stringBuilder.Append("மவஷஹఅఌఎఐ");
		stringBuilder.Append("ఒనపళవహౠౡ");
		stringBuilder.Append("ಅಌಎಐಒನಪಳ");
		stringBuilder.Append("ವಹೞೞೠೡഅഌ");
		stringBuilder.Append("എഐഒനപഹൠൡ");
		stringBuilder.Append("กฮะะาำเๅ");
		stringBuilder.Append("ກຂຄຄງຈຊຊ");
		stringBuilder.Append("ຍຍດທນຟມຣ");
		stringBuilder.Append("ລລວວສຫອຮ");
		stringBuilder.Append("ະະາຳຽຽເໄ");
		stringBuilder.Append("ཀཇཉཀྵႠჅაჶ");
		stringBuilder.Append("ᄀᄀᄂᄃᄅᄇᄉᄉ");
		stringBuilder.Append("ᄋᄌᄎᄒᄼᄼᄾᄾ");
		stringBuilder.Append("ᅀᅀᅌᅌᅎᅎᅐᅐ");
		stringBuilder.Append("ᅔᅕᅙᅙᅟᅡᅣᅣ");
		stringBuilder.Append("ᅥᅥᅧᅧᅩᅩᅭᅮ");
		stringBuilder.Append("ᅲᅳᅵᅵᆞᆞᆨᆨ");
		stringBuilder.Append("ᆫᆫᆮᆯᆷᆸᆺᆺ");
		stringBuilder.Append("ᆼᇂᇫᇫᇰᇰᇹᇹ");
		stringBuilder.Append("ḀẛẠỹἀἕἘἝ");
		stringBuilder.Append("ἠὅὈὍὐὗὙὙ");
		stringBuilder.Append("ὛὛὝὝὟώᾀᾴ");
		stringBuilder.Append("ᾶᾼιιῂῄῆῌ");
		stringBuilder.Append("ῐΐῖΊῠῬῲῴ");
		stringBuilder.Append("ῶῼΩΩKÅ℮℮");
		stringBuilder.Append("ↀↂ〇〇〡〩ぁゔ");
		stringBuilder.Append("ァヺㄅㄬ一龥가힣");
		SetProperties(stringBuilder.ToString(), 2);
		stringBuilder.Length = 0;
		stringBuilder.Append("::AZ__az");
		stringBuilder.Append("ÀÖØöøıĴľ");
		stringBuilder.Append("ŁňŊžƀǃǍǰ");
		stringBuilder.Append("ǴǵǺȗɐʨʻˁ");
		stringBuilder.Append("ΆΆΈΊΌΌΎΡ");
		stringBuilder.Append("ΣώϐϖϚϚϜϜ");
		stringBuilder.Append("ϞϞϠϠϢϳЁЌ");
		stringBuilder.Append("ЎяёќўҁҐӄ");
		stringBuilder.Append("ӇӈӋӌӐӫӮӵ");
		stringBuilder.Append("ӸӹԱՖՙՙաֆ");
		stringBuilder.Append("אתװײءغفي");
		stringBuilder.Append("ٱڷںھۀێېۓ");
		stringBuilder.Append("ەەۥۦअहऽऽ");
		stringBuilder.Append("क़ॡঅঌএঐওন");
		stringBuilder.Append("পরললশহড়ঢ়");
		stringBuilder.Append("য়ৡৰৱਅਊਏਐ");
		stringBuilder.Append("ਓਨਪਰਲਲ਼ਵਸ਼");
		stringBuilder.Append("ਸਹਖ਼ੜਫ਼ਫ਼ੲੴ");
		stringBuilder.Append("અઋઍઍએઑઓન");
		stringBuilder.Append("પરલળવહઽઽ");
		stringBuilder.Append("ૠૠଅଌଏଐଓନ");
		stringBuilder.Append("ପରଲଳଶହଽଽ");
		stringBuilder.Append("ଡ଼ଢ଼ୟୡஅஊஎஐ");
		stringBuilder.Append("ஒகஙசஜஜஞட");
		stringBuilder.Append("ணதநபமவஷஹ");
		stringBuilder.Append("అఌఎఐఒనపళ");
		stringBuilder.Append("వహౠౡಅಌಎಐ");
		stringBuilder.Append("ಒನಪಳವಹೞೞ");
		stringBuilder.Append("ೠೡഅഌഎഐഒന");
		stringBuilder.Append("പഹൠൡกฮะะ");
		stringBuilder.Append("าำเๅກຂຄຄ");
		stringBuilder.Append("ງຈຊຊຍຍດທ");
		stringBuilder.Append("ນຟມຣລລວວ");
		stringBuilder.Append("ສຫອຮະະາຳ");
		stringBuilder.Append("ຽຽເໄཀཇཉཀྵ");
		stringBuilder.Append("ႠჅაჶᄀᄀᄂᄃ");
		stringBuilder.Append("ᄅᄇᄉᄉᄋᄌᄎᄒ");
		stringBuilder.Append("ᄼᄼᄾᄾᅀᅀᅌᅌ");
		stringBuilder.Append("ᅎᅎᅐᅐᅔᅕᅙᅙ");
		stringBuilder.Append("ᅟᅡᅣᅣᅥᅥᅧᅧ");
		stringBuilder.Append("ᅩᅩᅭᅮᅲᅳᅵᅵ");
		stringBuilder.Append("ᆞᆞᆨᆨᆫᆫᆮᆯ");
		stringBuilder.Append("ᆷᆸᆺᆺᆼᇂᇫᇫ");
		stringBuilder.Append("ᇰᇰᇹᇹḀẛẠỹ");
		stringBuilder.Append("ἀἕἘἝἠὅὈὍ");
		stringBuilder.Append("ὐὗὙὙὛὛὝὝ");
		stringBuilder.Append("Ὗώᾀᾴᾶᾼιι");
		stringBuilder.Append("ῂῄῆῌῐΐῖΊ");
		stringBuilder.Append("ῠῬῲῴῶῼΩΩ");
		stringBuilder.Append("KÅ℮℮ↀↂ〇〇");
		stringBuilder.Append("〡〩ぁゔァヺㄅㄬ");
		stringBuilder.Append("一龥가힣");
		SetProperties(stringBuilder.ToString(), 4);
		stringBuilder.Length = 0;
		stringBuilder.Append("-.0:AZ__");
		stringBuilder.Append("az··ÀÖØö");
		stringBuilder.Append("øıĴľŁňŊž");
		stringBuilder.Append("ƀǃǍǰǴǵǺȗ");
		stringBuilder.Append("ɐʨʻˁːˑ\u0300\u0345");
		stringBuilder.Append("\u0360\u0361ΆΊΌΌΎΡ");
		stringBuilder.Append("ΣώϐϖϚϚϜϜ");
		stringBuilder.Append("ϞϞϠϠϢϳЁЌ");
		stringBuilder.Append("Ўяёќўҁ\u0483\u0486");
		stringBuilder.Append("ҐӄӇӈӋӌӐӫ");
		stringBuilder.Append("ӮӵӸӹԱՖՙՙ");
		stringBuilder.Append("աֆ\u0591\u05a1\u05a3\u05b9\u05bb\u05bd");
		stringBuilder.Append("\u05bf\u05bf\u05c1\u05c2\u05c4\u05c4את");
		stringBuilder.Append("װײءغـ\u0652٠٩");
		stringBuilder.Append("\u0670ڷںھۀێېۓ");
		stringBuilder.Append("ە\u06e8\u06ea\u06ed۰۹\u0901\u0903");
		stringBuilder.Append("अह\u093c\u094d\u0951\u0954क़\u0963");
		stringBuilder.Append("०९\u0981\u0983অঌএঐ");
		stringBuilder.Append("ওনপরললশহ");
		stringBuilder.Append("\u09bc\u09bc\u09be\u09c4\u09c7\u09c8\u09cb\u09cd");
		stringBuilder.Append("\u09d7\u09d7ড়ঢ়য়\u09e3০ৱ");
		stringBuilder.Append("\u0a02\u0a02ਅਊਏਐਓਨ");
		stringBuilder.Append("ਪਰਲਲ਼ਵਸ਼ਸਹ");
		stringBuilder.Append("\u0a3c\u0a3c\u0a3e\u0a42\u0a47\u0a48\u0a4b\u0a4d");
		stringBuilder.Append("ਖ਼ੜਫ਼ਫ਼੦ੴ\u0a81\u0a83");
		stringBuilder.Append("અઋઍઍએઑઓન");
		stringBuilder.Append("પરલળવહ\u0abc\u0ac5");
		stringBuilder.Append("\u0ac7\u0ac9\u0acb\u0acdૠૠ૦૯");
		stringBuilder.Append("\u0b01\u0b03ଅଌଏଐଓନ");
		stringBuilder.Append("ପରଲଳଶହ\u0b3c\u0b43");
		stringBuilder.Append("\u0b47\u0b48\u0b4b\u0b4d\u0b56\u0b57ଡ଼ଢ଼");
		stringBuilder.Append("ୟୡ୦୯\u0b82ஃஅஊ");
		stringBuilder.Append("எஐஒகஙசஜஜ");
		stringBuilder.Append("ஞடணதநபமவ");
		stringBuilder.Append("ஷஹ\u0bbe\u0bc2\u0bc6\u0bc8\u0bca\u0bcd");
		stringBuilder.Append("\u0bd7\u0bd7௧௯\u0c01\u0c03అఌ");
		stringBuilder.Append("ఎఐఒనపళవహ");
		stringBuilder.Append("\u0c3e\u0c44\u0c46\u0c48\u0c4a\u0c4d\u0c55\u0c56");
		stringBuilder.Append("ౠౡ౦౯\u0c82\u0c83ಅಌ");
		stringBuilder.Append("ಎಐಒನಪಳವಹ");
		stringBuilder.Append("\u0cbe\u0cc4\u0cc6\u0cc8\u0cca\u0ccd\u0cd5\u0cd6");
		stringBuilder.Append("ೞೞೠೡ೦೯\u0d02\u0d03");
		stringBuilder.Append("അഌഎഐഒനപഹ");
		stringBuilder.Append("\u0d3e\u0d43\u0d46\u0d48\u0d4a\u0d4d\u0d57\u0d57");
		stringBuilder.Append("ൠൡ൦൯กฮะ\u0e3a");
		stringBuilder.Append("เ\u0e4e๐๙ກຂຄຄ");
		stringBuilder.Append("ງຈຊຊຍຍດທ");
		stringBuilder.Append("ນຟມຣລລວວ");
		stringBuilder.Append("ສຫອຮະ\u0eb9\u0ebbຽ");
		stringBuilder.Append("ເໄໆໆ\u0ec8\u0ecd໐໙");
		stringBuilder.Append("\u0f18\u0f19༠༩\u0f35\u0f35\u0f37\u0f37");
		stringBuilder.Append("\u0f39\u0f39\u0f3eཇཉཀྵ\u0f71\u0f84");
		stringBuilder.Append("\u0f86ྋ\u0f90\u0f95\u0f97\u0f97\u0f99\u0fad");
		stringBuilder.Append("\u0fb1\u0fb7\u0fb9\u0fb9ႠჅაჶ");
		stringBuilder.Append("ᄀᄀᄂᄃᄅᄇᄉᄉ");
		stringBuilder.Append("ᄋᄌᄎᄒᄼᄼᄾᄾ");
		stringBuilder.Append("ᅀᅀᅌᅌᅎᅎᅐᅐ");
		stringBuilder.Append("ᅔᅕᅙᅙᅟᅡᅣᅣ");
		stringBuilder.Append("ᅥᅥᅧᅧᅩᅩᅭᅮ");
		stringBuilder.Append("ᅲᅳᅵᅵᆞᆞᆨᆨ");
		stringBuilder.Append("ᆫᆫᆮᆯᆷᆸᆺᆺ");
		stringBuilder.Append("ᆼᇂᇫᇫᇰᇰᇹᇹ");
		stringBuilder.Append("ḀẛẠỹἀἕἘἝ");
		stringBuilder.Append("ἠὅὈὍὐὗὙὙ");
		stringBuilder.Append("ὛὛὝὝὟώᾀᾴ");
		stringBuilder.Append("ᾶᾼιιῂῄῆῌ");
		stringBuilder.Append("ῐΐῖΊῠῬῲῴ");
		stringBuilder.Append("ῶῼ\u20d0\u20dc\u20e1\u20e1ΩΩ");
		stringBuilder.Append("KÅ℮℮ↀↂ々々");
		stringBuilder.Append("〇〇〡\u302f〱〵ぁゔ");
		stringBuilder.Append("\u3099\u309aゝゞァヺーヾ");
		stringBuilder.Append("ㄅㄬ一龥가힣");
		SetProperties(stringBuilder.ToString(), 8);
		SetProperties("\t\n\r\r \ud7ff\udc00\udeff\ue000\ufffd", 16);
		SetProperties("\n\n\r\r !#%';==?Z__az", 32);
		stringBuilder.Length = 0;
		stringBuilder = null;
	}

	private XmlCharType()
	{
	}

	private static void SetProperties(string ranges, byte value)
	{
		for (int i = 0; i < ranges.Length; i += 2)
		{
			int j = ranges[i];
			for (int num = ranges[i + 1]; j <= num; j++)
			{
				s_CharProperties[j] |= value;
			}
		}
	}

	public static bool IsWhiteSpace(char ch)
	{
		return (s_CharProperties[(uint)ch] & 1) != 0;
	}

	public static bool IsLetter(char ch)
	{
		return (s_CharProperties[(uint)ch] & 2) != 0;
	}

	public static bool IsDigit(char ch)
	{
		if (ch >= '0')
		{
			return ch <= '9';
		}
		return false;
	}

	public static bool IsHexDigit(char ch)
	{
		if ((ch < '0' || ch > '9') && (ch < 'a' || ch > 'f'))
		{
			if (ch >= 'A')
			{
				return ch <= 'F';
			}
			return false;
		}
		return true;
	}

	public static bool IsCombiningChar(char ch)
	{
		return false;
	}

	public static bool IsExtender(char ch)
	{
		return ch == '·';
	}

	public static bool IsNameChar(char ch)
	{
		return (s_CharProperties[(uint)ch] & 8) != 0;
	}

	public static bool IsStartNameChar(char ch)
	{
		return (s_CharProperties[(uint)ch] & 4) != 0;
	}

	public static bool IsNCNameChar(char ch)
	{
		if (IsNameChar(ch))
		{
			return ch != ':';
		}
		return false;
	}

	public static bool IsStartNCNameChar(char ch)
	{
		if (IsStartNameChar(ch))
		{
			return ch != ':';
		}
		return false;
	}

	public static bool IsCharData(char ch)
	{
		return (s_CharProperties[(uint)ch] & 0x10) != 0;
	}

	public static bool IsPubidChar(char ch)
	{
		return (s_CharProperties[(uint)ch] & 0x20) != 0;
	}
}
