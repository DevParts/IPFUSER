namespace Microsoft.SqlServer.Management.Smo;

public enum EndpointAuthenticationOrder
{
	Ntlm = 1,
	Kerberos,
	Negotiate,
	Certificate,
	NtlmCertificate,
	KerberosCertificate,
	NegotiateCertificate,
	CertificateNtlm,
	CertificateKerberos,
	CertificateNegotiate
}
