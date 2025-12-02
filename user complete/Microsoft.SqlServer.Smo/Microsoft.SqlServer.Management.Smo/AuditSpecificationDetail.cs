using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public struct AuditSpecificationDetail
{
	private AuditActionType action;

	private string objectClass;

	private string objectName;

	private string objectSchema;

	private string principal;

	private static readonly Hashtable stringToEnum = new Hashtable();

	public AuditActionType Action => action;

	public string ObjectClass => objectClass;

	public string ObjectName => objectName;

	public string ObjectSchema => objectSchema;

	public string Principal => principal;

	public static Hashtable StringToEnum
	{
		get
		{
			if (stringToEnum.Count == 0)
			{
				AuditActionTypeConverter auditActionTypeConverter = new AuditActionTypeConverter();
				foreach (int value in Enum.GetValues(typeof(AuditActionType)))
				{
					stringToEnum.Add(auditActionTypeConverter.ConvertToInvariantString((AuditActionType)value), (AuditActionType)value);
				}
			}
			return stringToEnum;
		}
	}

	public AuditSpecificationDetail(AuditActionType action, string objectClass, string objectSchema, string objectName, string principal)
	{
		this.action = action;
		this.objectClass = objectClass;
		this.objectSchema = objectSchema;
		this.objectName = objectName;
		this.principal = principal;
	}

	public AuditSpecificationDetail(AuditActionType action, string objectSchema, string objectName, string principal)
	{
		this.action = action;
		objectClass = string.Empty;
		this.objectSchema = objectSchema;
		this.objectName = objectName;
		this.principal = principal;
	}

	public AuditSpecificationDetail(AuditActionType action, string objectName, string principal)
	{
		this.action = action;
		objectClass = string.Empty;
		objectSchema = string.Empty;
		this.objectName = objectName;
		this.principal = principal;
	}

	public AuditSpecificationDetail(AuditActionType action)
	{
		this.action = action;
		objectClass = string.Empty;
		objectSchema = string.Empty;
		objectName = string.Empty;
		principal = string.Empty;
	}
}
