using System;

namespace Microsoft.SqlServer.Management.Common;

public interface IRenewableToken
{
	DateTimeOffset TokenExpiry { get; }

	string Resource { get; }

	string Tenant { get; }

	string UserId { get; }

	string GetAccessToken();
}
