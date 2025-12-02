using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class EndpointPayload : SqlSmoObject
{
	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Endpoint Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Endpoint;
		}
	}

	internal EndpointPayload(Endpoint parentEndpoint, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentEndpoint;
		SetServerObject(parentEndpoint.GetServerObject());
		m_comparer = ((Endpoint)singletonParent).Parent.Databases["master"].StringComparer;
	}

	public override string ToString()
	{
		return Parent.ToString();
	}

	internal abstract void Script(StringBuilder sb, ScriptingPreferences sp);

	internal void ScriptAuthenticationAndEncryption(StringBuilder sb, ScriptingPreferences sp, bool needsComma)
	{
		object propValueOptional = GetPropValueOptional("EndpointAuthenticationOrder");
		if (propValueOptional != null)
		{
			if (needsComma)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			needsComma = true;
			sb.Append("AUTHENTICATION = ");
			string empty = string.Empty;
			switch ((EndpointAuthenticationOrder)propValueOptional)
			{
			case EndpointAuthenticationOrder.Ntlm:
				sb.Append("WINDOWS NTLM");
				break;
			case EndpointAuthenticationOrder.Kerberos:
				sb.Append("WINDOWS KERBEROS");
				break;
			case EndpointAuthenticationOrder.Negotiate:
				sb.Append("WINDOWS NEGOTIATE");
				break;
			case EndpointAuthenticationOrder.Certificate:
				empty = (string)GetPropValue("Certificate");
				if (empty.Length == 0)
				{
					throw new PropertyNotSetException("Certificate");
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "CERTIFICATE {0}", new object[1] { SqlSmoObject.MakeSqlBraket(empty) });
				break;
			case EndpointAuthenticationOrder.CertificateNtlm:
				empty = (string)GetPropValue("Certificate");
				if (empty.Length == 0)
				{
					throw new PropertyNotSetException("Certificate");
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "CERTIFICATE {0} WINDOWS NTLM", new object[1] { SqlSmoObject.MakeSqlBraket(empty) });
				break;
			case EndpointAuthenticationOrder.CertificateKerberos:
				empty = (string)GetPropValue("Certificate");
				if (empty.Length == 0)
				{
					throw new PropertyNotSetException("Certificate");
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "CERTIFICATE {0} WINDOWS KERBEROS", new object[1] { SqlSmoObject.MakeSqlBraket(empty) });
				break;
			case EndpointAuthenticationOrder.CertificateNegotiate:
				empty = (string)GetPropValue("Certificate");
				if (empty.Length == 0)
				{
					throw new PropertyNotSetException("Certificate");
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "CERTIFICATE {0} WINDOWS NEGOTIATE", new object[1] { SqlSmoObject.MakeSqlBraket(empty) });
				break;
			case EndpointAuthenticationOrder.NtlmCertificate:
				empty = (string)GetPropValue("Certificate");
				if (empty.Length == 0)
				{
					throw new PropertyNotSetException("Certificate");
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "WINDOWS NTLM CERTIFICATE {0} ", new object[1] { SqlSmoObject.MakeSqlBraket(empty) });
				break;
			case EndpointAuthenticationOrder.KerberosCertificate:
				empty = (string)GetPropValue("Certificate");
				if (empty.Length == 0)
				{
					throw new PropertyNotSetException("Certificate");
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "WINDOWS KERBEROS CERTIFICATE {0} ", new object[1] { SqlSmoObject.MakeSqlBraket(empty) });
				break;
			case EndpointAuthenticationOrder.NegotiateCertificate:
				empty = (string)GetPropValue("Certificate");
				if (empty.Length == 0)
				{
					throw new PropertyNotSetException("Certificate");
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "WINDOWS NEGOTIATE CERTIFICATE {0} ", new object[1] { SqlSmoObject.MakeSqlBraket(empty) });
				break;
			default:
				throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(typeof(EndpointAuthenticationOrder).Name));
			}
			sb.Append(Globals.newline);
		}
		object propValueOptional2 = GetPropValueOptional("EndpointEncryptionAlgorithm");
		object propValueOptional3 = GetPropValueOptional("EndpointEncryption");
		if (propValueOptional2 == null && propValueOptional3 == null)
		{
			return;
		}
		if (needsComma)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
		}
		needsComma = true;
		EndpointEncryption endpointEncryption = (EndpointEncryption)GetPropValue("EndpointEncryption");
		EndpointEncryptionAlgorithm endpointEncryptionAlgorithm = GetPropValueOptional("EndpointEncryptionAlgorithm", EndpointEncryptionAlgorithm.None);
		sb.Append("ENCRYPTION = ");
		switch (endpointEncryption)
		{
		case EndpointEncryption.Disabled:
			sb.Append("DISABLED");
			if (endpointEncryptionAlgorithm != EndpointEncryptionAlgorithm.None)
			{
				throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(typeof(EndpointEncryptionAlgorithm).Name));
			}
			break;
		case EndpointEncryption.Supported:
			sb.Append("SUPPORTED");
			endpointEncryptionAlgorithm = (EndpointEncryptionAlgorithm)GetPropValue("EndpointEncryptionAlgorithm");
			if (endpointEncryptionAlgorithm == EndpointEncryptionAlgorithm.None)
			{
				throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(typeof(EndpointEncryptionAlgorithm).Name));
			}
			break;
		case EndpointEncryption.Required:
			sb.Append("REQUIRED");
			endpointEncryptionAlgorithm = (EndpointEncryptionAlgorithm)GetPropValue("EndpointEncryptionAlgorithm");
			if (endpointEncryptionAlgorithm == EndpointEncryptionAlgorithm.None)
			{
				throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(typeof(EndpointEncryptionAlgorithm).Name));
			}
			break;
		default:
			throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(typeof(EndpointEncryption).Name));
		}
		switch (endpointEncryptionAlgorithm)
		{
		case EndpointEncryptionAlgorithm.RC4:
			sb.Append(" ALGORITHM RC4");
			break;
		case EndpointEncryptionAlgorithm.Aes:
			sb.Append(" ALGORITHM AES");
			break;
		case EndpointEncryptionAlgorithm.AesRC4:
			sb.Append(" ALGORITHM AES RC4");
			break;
		case EndpointEncryptionAlgorithm.RC4Aes:
			sb.Append(" ALGORITHM RC4 AES");
			break;
		default:
			throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(typeof(EndpointEncryptionAlgorithm).Name));
		case EndpointEncryptionAlgorithm.None:
			break;
		}
	}
}
