using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

public class DataType : IXmlSerializable
{
	private SqlSmoObject parent;

	private string name = string.Empty;

	internal SqlDataType sqlDataType;

	private string schema = string.Empty;

	private int maximumLength;

	private int numericPrecision;

	private int numericScale;

	private XmlDocumentConstraint xmlDocumentConstraint;

	public static DataType BigInt => new DataType(SqlDataType.BigInt);

	public static DataType HierarchyId => new DataType(SqlDataType.HierarchyId);

	public static DataType Bit => new DataType(SqlDataType.Bit);

	public static DataType DateTime => new DataType(SqlDataType.DateTime);

	public static DataType Float => new DataType(SqlDataType.Float);

	public static DataType Geography => new DataType(SqlDataType.Geography);

	public static DataType Geometry => new DataType(SqlDataType.Geometry);

	public static DataType Image => new DataType(SqlDataType.Image);

	public static DataType Int => new DataType(SqlDataType.Int);

	public static DataType Money => new DataType(SqlDataType.Money);

	public static DataType NText => new DataType(SqlDataType.NText);

	public static DataType NVarCharMax => new DataType(SqlDataType.NVarCharMax);

	public static DataType Real => new DataType(SqlDataType.Real);

	public static DataType SmallDateTime => new DataType(SqlDataType.SmallDateTime);

	public static DataType SmallInt => new DataType(SqlDataType.SmallInt);

	public static DataType SmallMoney => new DataType(SqlDataType.SmallMoney);

	public static DataType Text => new DataType(SqlDataType.Text);

	public static DataType Timestamp => new DataType(SqlDataType.Timestamp);

	public static DataType TinyInt => new DataType(SqlDataType.TinyInt);

	public static DataType UniqueIdentifier => new DataType(SqlDataType.UniqueIdentifier);

	public static DataType VarBinaryMax => new DataType(SqlDataType.VarBinaryMax);

	public static DataType VarCharMax => new DataType(SqlDataType.VarCharMax);

	public static DataType Variant => new DataType(SqlDataType.Variant);

	public static DataType SysName => new DataType(SqlDataType.SysName);

	public static DataType Date => new DataType(SqlDataType.Date);

	internal SqlSmoObject Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Name"));
			}
			if (sqlDataType == SqlDataType.Xml || sqlDataType == SqlDataType.UserDefinedDataType || SqlDataType == SqlDataType.UserDefinedTableType || sqlDataType == SqlDataType.UserDefinedType)
			{
				name = value;
				if (parent != null)
				{
					if (sqlDataType == SqlDataType.Xml)
					{
						parent.Properties.Get("XmlSchemaNamespace").Value = name;
					}
					else
					{
						parent.Properties.Get("DataType").Value = name;
					}
				}
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.CantSetTypeName(sqlDataType.ToString()));
		}
	}

	public SqlDataType SqlDataType
	{
		get
		{
			if (HasTypeChangedToReal())
			{
				return SqlDataType.Real;
			}
			return sqlDataType;
		}
		set
		{
			if (Enum.IsDefined(typeof(SqlDataType), value))
			{
				sqlDataType = value;
				name = GetSqlName(value);
				if (parent == null)
				{
					return;
				}
				if (sqlDataType != SqlDataType.UserDefinedDataType && SqlDataType != SqlDataType.UserDefinedTableType && sqlDataType != SqlDataType.UserDefinedType)
				{
					parent.Properties.Get("DataType").Value = GetSqlName(sqlDataType);
					if (sqlDataType == SqlDataType.NVarCharMax || sqlDataType == SqlDataType.VarBinaryMax || sqlDataType == SqlDataType.VarCharMax)
					{
						parent.Properties.Get("Length").Value = -1;
					}
					if (parent.ServerVersion.Major >= 9)
					{
						schema = "sys";
					}
					else
					{
						schema = "dbo";
					}
				}
				else
				{
					schema = string.Empty;
				}
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.UnknownSqlDataType(value.ToString()));
		}
	}

	public string Schema
	{
		get
		{
			return schema;
		}
		set
		{
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Schema"));
			}
			if (sqlDataType == SqlDataType.Xml || sqlDataType == SqlDataType.UserDefinedDataType || SqlDataType == SqlDataType.UserDefinedTableType || sqlDataType == SqlDataType.UserDefinedType)
			{
				schema = value;
				if (parent != null)
				{
					if (sqlDataType == SqlDataType.Xml)
					{
						parent.Properties.Get("XmlSchemaNamespaceSchema").Value = schema;
					}
					else
					{
						parent.Properties.Get("DataTypeSchema").Value = schema;
					}
				}
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.CantSetTypeSchema(sqlDataType.ToString()));
		}
	}

	public int MaximumLength
	{
		get
		{
			return maximumLength;
		}
		set
		{
			maximumLength = value;
			if (parent != null && sqlDataType != SqlDataType.NVarCharMax && sqlDataType != SqlDataType.VarBinaryMax && sqlDataType != SqlDataType.VarCharMax)
			{
				parent.Properties.Get("Length").Value = maximumLength;
			}
		}
	}

	public int NumericPrecision
	{
		get
		{
			if (parent != null && parent.Properties.Get("NumericPrecision").Value != null)
			{
				return (int)parent.Properties.Get("NumericPrecision").Value;
			}
			return numericPrecision;
		}
		set
		{
			numericPrecision = value;
			if (parent != null)
			{
				parent.Properties.Get("NumericPrecision").Value = numericPrecision;
			}
		}
	}

	public int NumericScale
	{
		get
		{
			return numericScale;
		}
		set
		{
			numericScale = value;
			if (parent != null)
			{
				parent.Properties.Get("NumericScale").Value = numericScale;
			}
		}
	}

	public XmlDocumentConstraint XmlDocumentConstraint
	{
		get
		{
			return xmlDocumentConstraint;
		}
		set
		{
			xmlDocumentConstraint = value;
			if (parent != null)
			{
				parent.Properties.Get("XmlDocumentConstraint").Value = xmlDocumentConstraint;
			}
		}
	}

	public bool IsNumericType
	{
		get
		{
			bool result = false;
			switch (sqlDataType)
			{
			case SqlDataType.BigInt:
			case SqlDataType.Decimal:
			case SqlDataType.Float:
			case SqlDataType.Int:
			case SqlDataType.Money:
			case SqlDataType.Real:
			case SqlDataType.SmallInt:
			case SqlDataType.SmallMoney:
			case SqlDataType.TinyInt:
			case SqlDataType.Numeric:
				result = true;
				break;
			case SqlDataType.UserDefinedDataType:
				if (numericScale != 0)
				{
					result = true;
				}
				break;
			}
			return result;
		}
	}

	public bool IsStringType
	{
		get
		{
			bool result = false;
			switch (sqlDataType)
			{
			case SqlDataType.Char:
			case SqlDataType.NChar:
			case SqlDataType.NText:
			case SqlDataType.NVarChar:
			case SqlDataType.NVarCharMax:
			case SqlDataType.Text:
			case SqlDataType.VarChar:
			case SqlDataType.VarCharMax:
				result = true;
				break;
			}
			return result;
		}
	}

	public DataType()
	{
	}

	public DataType(SqlDataType sqlDataType)
	{
		switch (sqlDataType)
		{
		case SqlDataType.BigInt:
		case SqlDataType.Binary:
		case SqlDataType.Bit:
		case SqlDataType.Char:
		case SqlDataType.DateTime:
		case SqlDataType.Image:
		case SqlDataType.Int:
		case SqlDataType.Money:
		case SqlDataType.NChar:
		case SqlDataType.NText:
		case SqlDataType.NVarChar:
		case SqlDataType.NVarCharMax:
		case SqlDataType.Real:
		case SqlDataType.SmallDateTime:
		case SqlDataType.SmallInt:
		case SqlDataType.SmallMoney:
		case SqlDataType.Text:
		case SqlDataType.Timestamp:
		case SqlDataType.TinyInt:
		case SqlDataType.UniqueIdentifier:
		case SqlDataType.VarBinary:
		case SqlDataType.VarBinaryMax:
		case SqlDataType.VarChar:
		case SqlDataType.VarCharMax:
		case SqlDataType.Variant:
		case SqlDataType.Xml:
		case SqlDataType.SysName:
		case SqlDataType.Date:
		case SqlDataType.HierarchyId:
		case SqlDataType.Geometry:
		case SqlDataType.Geography:
			this.sqlDataType = sqlDataType;
			name = GetSqlName(sqlDataType);
			break;
		case SqlDataType.Decimal:
		case SqlDataType.Numeric:
			this.sqlDataType = sqlDataType;
			name = GetSqlName(sqlDataType);
			NumericPrecision = 18;
			NumericScale = 0;
			break;
		case SqlDataType.Float:
			this.sqlDataType = sqlDataType;
			name = GetSqlName(sqlDataType);
			NumericPrecision = 53;
			break;
		case SqlDataType.Time:
		case SqlDataType.DateTimeOffset:
		case SqlDataType.DateTime2:
			this.sqlDataType = sqlDataType;
			name = GetSqlName(sqlDataType);
			NumericScale = 7;
			break;
		default:
			throw new SmoException(ExceptionTemplatesImpl.DataTypeUnsupported(sqlDataType.ToString()));
		}
	}

	public DataType(SqlDataType sqlDataType, int precisionOrMaxLengthOrScale)
	{
		switch (sqlDataType)
		{
		case SqlDataType.Binary:
		case SqlDataType.Char:
		case SqlDataType.Image:
		case SqlDataType.NChar:
		case SqlDataType.NText:
		case SqlDataType.NVarChar:
		case SqlDataType.Text:
		case SqlDataType.VarBinary:
		case SqlDataType.VarChar:
			this.sqlDataType = sqlDataType;
			MaximumLength = precisionOrMaxLengthOrScale;
			name = GetSqlName(sqlDataType);
			break;
		case SqlDataType.Decimal:
		case SqlDataType.Numeric:
			this.sqlDataType = sqlDataType;
			NumericPrecision = precisionOrMaxLengthOrScale;
			NumericScale = 0;
			name = GetSqlName(sqlDataType);
			break;
		case SqlDataType.Float:
			this.sqlDataType = sqlDataType;
			NumericPrecision = precisionOrMaxLengthOrScale;
			name = GetSqlName(sqlDataType);
			break;
		case SqlDataType.Time:
		case SqlDataType.DateTimeOffset:
		case SqlDataType.DateTime2:
			this.sqlDataType = sqlDataType;
			NumericScale = precisionOrMaxLengthOrScale;
			name = GetSqlName(sqlDataType);
			break;
		default:
			throw new SmoException(ExceptionTemplatesImpl.DataTypeUnsupported(sqlDataType.ToString()));
		}
	}

	public DataType(SqlDataType sqlDataType, int precision, int scale)
	{
		if (sqlDataType == SqlDataType.Decimal || sqlDataType == SqlDataType.Numeric)
		{
			this.sqlDataType = sqlDataType;
			NumericPrecision = precision;
			NumericScale = scale;
			name = GetSqlName(sqlDataType);
			return;
		}
		throw new SmoException(ExceptionTemplatesImpl.DataTypeUnsupported(sqlDataType.ToString()));
	}

	public DataType(SqlDataType sqlDataType, string type)
	{
		switch (sqlDataType)
		{
		case SqlDataType.UserDefinedDataType:
		case SqlDataType.UserDefinedType:
		case SqlDataType.Xml:
		case SqlDataType.UserDefinedTableType:
			this.sqlDataType = sqlDataType;
			Name = type;
			break;
		default:
			throw new SmoException(ExceptionTemplatesImpl.DataTypeUnsupported(sqlDataType.ToString()));
		}
	}

	public DataType(SqlDataType sqlDataType, string type, string schema)
	{
		switch (sqlDataType)
		{
		case SqlDataType.UserDefinedDataType:
		case SqlDataType.UserDefinedType:
		case SqlDataType.Xml:
		case SqlDataType.UserDefinedTableType:
			this.sqlDataType = sqlDataType;
			Name = type;
			Schema = schema;
			break;
		default:
			throw new SmoException(ExceptionTemplatesImpl.DataTypeUnsupported(sqlDataType.ToString()));
		}
	}

	public DataType(XmlSchemaCollection xmlSchemaCollection)
	{
		CheckInputObject(xmlSchemaCollection);
		sqlDataType = SqlDataType.Xml;
		name = xmlSchemaCollection.Name;
		schema = xmlSchemaCollection.Schema;
	}

	public DataType(UserDefinedDataType userDefinedDataType)
	{
		CheckInputObject(userDefinedDataType);
		sqlDataType = SqlDataType.UserDefinedDataType;
		name = userDefinedDataType.Name;
		schema = userDefinedDataType.Schema;
		MaximumLength = userDefinedDataType.Length;
		NumericPrecision = userDefinedDataType.NumericPrecision;
		NumericScale = userDefinedDataType.NumericScale;
	}

	public DataType(UserDefinedTableType userDefinedTableType)
	{
		CheckInputObject(userDefinedTableType);
		sqlDataType = SqlDataType.UserDefinedTableType;
		name = userDefinedTableType.Name;
		schema = userDefinedTableType.Schema;
	}

	public DataType(UserDefinedType userDefinedType)
	{
		CheckInputObject(userDefinedType);
		sqlDataType = SqlDataType.UserDefinedType;
		name = userDefinedType.Name;
		schema = userDefinedType.Schema;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		DataType dt = obj as DataType;
		return IsEqualInAllAspects(dt);
	}

	public bool Equals(DataType dt)
	{
		return IsEqualInAllAspects(dt);
	}

	public override int GetHashCode()
	{
		return (int)sqlDataType;
	}

	private bool IsEqualInAllAspects(DataType dt)
	{
		if (dt == null)
		{
			return false;
		}
		if (sqlDataType != dt.sqlDataType)
		{
			return false;
		}
		if (name != dt.name)
		{
			return false;
		}
		if (schema != dt.schema)
		{
			return false;
		}
		if (maximumLength != dt.maximumLength)
		{
			return false;
		}
		if (numericPrecision != dt.numericPrecision)
		{
			return false;
		}
		if (numericScale != dt.numericScale)
		{
			return false;
		}
		return true;
	}

	public static DataType Binary(int maxLength)
	{
		return new DataType(SqlDataType.Binary, maxLength);
	}

	public static DataType Char(int maxLength)
	{
		return new DataType(SqlDataType.Char, maxLength);
	}

	public static DataType Decimal(int scale, int precision)
	{
		return new DataType(SqlDataType.Decimal, precision, scale);
	}

	public static DataType Numeric(int scale, int precision)
	{
		return new DataType(SqlDataType.Numeric, precision, scale);
	}

	public static DataType NChar(int maxLength)
	{
		return new DataType(SqlDataType.NChar, maxLength);
	}

	public static DataType NVarChar(int maxLength)
	{
		return new DataType(SqlDataType.NVarChar, maxLength);
	}

	public static DataType UserDefinedDataType(string type, string schema)
	{
		return new DataType(SqlDataType.UserDefinedDataType, type, schema);
	}

	public static DataType UserDefinedDataType(string type)
	{
		return new DataType(SqlDataType.UserDefinedDataType, type, string.Empty);
	}

	public static DataType UserDefinedTableType(string type, string schema)
	{
		return new DataType(SqlDataType.UserDefinedTableType, type, schema);
	}

	public static DataType UserDefinedTableType(string type)
	{
		return new DataType(SqlDataType.UserDefinedTableType, type, string.Empty);
	}

	public static DataType UserDefinedType(string type, string schema)
	{
		return new DataType(SqlDataType.UserDefinedType, type, schema);
	}

	public static DataType UserDefinedType(string type)
	{
		return new DataType(SqlDataType.UserDefinedType, type, string.Empty);
	}

	public static DataType VarBinary(int maxLength)
	{
		return new DataType(SqlDataType.VarBinary, maxLength);
	}

	public static DataType VarChar(int maxLength)
	{
		return new DataType(SqlDataType.VarChar, maxLength);
	}

	public static DataType Xml(string type)
	{
		return new DataType(SqlDataType.Xml, type, string.Empty);
	}

	public static DataType Xml(string type, string schema)
	{
		return new DataType(SqlDataType.Xml, type, schema);
	}

	public static DataType Xml(string type, string schema, XmlDocumentConstraint xmlDocumentConstraint)
	{
		DataType dataType = new DataType(SqlDataType.Xml, type, schema);
		dataType.XmlDocumentConstraint = xmlDocumentConstraint;
		return dataType;
	}

	public static DataType Time(int scale)
	{
		return new DataType(SqlDataType.Time, scale);
	}

	public static DataType DateTimeOffset(int scale)
	{
		return new DataType(SqlDataType.DateTimeOffset, scale);
	}

	public static DataType DateTime2(int scale)
	{
		return new DataType(SqlDataType.DateTime2, scale);
	}

	public override string ToString()
	{
		return Name;
	}

	private void CheckInputObject(SqlSmoObject input)
	{
		if (input.State == SqlSmoState.Creating || input.State == SqlSmoState.Dropped)
		{
			throw new SmoException(ExceptionTemplatesImpl.NoPendingObjForDataType(input.State.ToString()));
		}
		if (input.State == SqlSmoState.Creating)
		{
			throw new SmoException(ExceptionTemplatesImpl.NeedExistingObjForDataType(input.FullQualifiedName));
		}
	}

	internal DataType Clone()
	{
		DataType dataType = new DataType();
		dataType.Parent = null;
		dataType.sqlDataType = SqlDataType;
		dataType.name = Name;
		dataType.schema = Schema;
		dataType.numericPrecision = NumericPrecision;
		dataType.numericScale = NumericScale;
		dataType.maximumLength = MaximumLength;
		return dataType;
	}

	internal void ReadFromPropBag(SqlSmoObject sqlObject)
	{
		string propValueOptional = sqlObject.GetPropValueOptional("DataType", string.Empty);
		string propValueOptional2 = sqlObject.GetPropValueOptional("SystemType", string.Empty);
		if (sqlObject is Parameter && !(sqlObject is NumberedStoredProcedureParameter) && parent.ServerVersion.Major >= 10 && sqlObject.GetPropValueOptional("IsReadOnly", defaultValue: false))
		{
			sqlDataType = SqlDataType.UserDefinedTableType;
			name = propValueOptional;
			schema = sqlObject.GetPropValueOptional("DataTypeSchema", string.Empty);
			return;
		}
		if (!(sqlObject is Sequence))
		{
			maximumLength = sqlObject.GetPropValueOptional("Length", 0);
		}
		numericPrecision = sqlObject.GetPropValueOptional("NumericPrecision", 0);
		numericScale = sqlObject.GetPropValueOptional("NumericScale", 0);
		if (propValueOptional == propValueOptional2 || "sysname" == propValueOptional)
		{
			name = propValueOptional;
			sqlDataType = SqlToEnum(propValueOptional);
			if (sqlDataType == SqlDataType.NVarChar && maximumLength <= 0)
			{
				sqlDataType = SqlDataType.NVarCharMax;
			}
			else if (sqlDataType == SqlDataType.VarBinary && maximumLength <= 0)
			{
				sqlDataType = SqlDataType.VarBinaryMax;
			}
			else if (sqlDataType == SqlDataType.VarChar && maximumLength <= 0)
			{
				sqlDataType = SqlDataType.VarCharMax;
			}
			else if (sqlDataType == SqlDataType.Xml)
			{
				name = sqlObject.GetPropValueOptional("XmlSchemaNamespace", string.Empty);
				schema = sqlObject.GetPropValueOptional("XmlSchemaNamespaceSchema", string.Empty);
				xmlDocumentConstraint = sqlObject.GetPropValueOptional("XmlDocumentConstraint", XmlDocumentConstraint.Default);
			}
		}
		else if (propValueOptional2.Length > 0)
		{
			sqlDataType = SqlDataType.UserDefinedDataType;
			name = propValueOptional;
			schema = sqlObject.GetPropValueOptional("DataTypeSchema", string.Empty);
		}
		else
		{
			sqlDataType = SqlDataType.UserDefinedType;
			name = propValueOptional;
			schema = sqlObject.GetPropValueOptional("DataTypeSchema", string.Empty);
		}
	}

	public string GetSqlName(SqlDataType sqldt)
	{
		return sqldt switch
		{
			SqlDataType.BigInt => "bigint", 
			SqlDataType.Binary => "binary", 
			SqlDataType.Bit => "bit", 
			SqlDataType.Char => "char", 
			SqlDataType.DateTime => "datetime", 
			SqlDataType.Decimal => "decimal", 
			SqlDataType.Numeric => "numeric", 
			SqlDataType.Float => "float", 
			SqlDataType.Geography => "geography", 
			SqlDataType.Geometry => "geometry", 
			SqlDataType.Image => "image", 
			SqlDataType.Int => "int", 
			SqlDataType.Money => "money", 
			SqlDataType.NChar => "nchar", 
			SqlDataType.NText => "ntext", 
			SqlDataType.NVarChar => "nvarchar", 
			SqlDataType.NVarCharMax => "nvarchar", 
			SqlDataType.Real => "real", 
			SqlDataType.SmallDateTime => "smalldatetime", 
			SqlDataType.SmallInt => "smallint", 
			SqlDataType.SmallMoney => "smallmoney", 
			SqlDataType.Text => "text", 
			SqlDataType.Timestamp => "timestamp", 
			SqlDataType.TinyInt => "tinyint", 
			SqlDataType.UniqueIdentifier => "uniqueidentifier", 
			SqlDataType.UserDefinedDataType => string.Empty, 
			SqlDataType.UserDefinedTableType => string.Empty, 
			SqlDataType.UserDefinedType => string.Empty, 
			SqlDataType.VarBinary => "varbinary", 
			SqlDataType.HierarchyId => "hierarchyid", 
			SqlDataType.VarBinaryMax => "varbinary", 
			SqlDataType.VarChar => "varchar", 
			SqlDataType.VarCharMax => "varchar", 
			SqlDataType.Variant => "sql_variant", 
			SqlDataType.Xml => "", 
			SqlDataType.SysName => "sysname", 
			SqlDataType.Date => "date", 
			SqlDataType.Time => "time", 
			SqlDataType.DateTimeOffset => "datetimeoffset", 
			SqlDataType.DateTime2 => "datetime2", 
			_ => string.Empty, 
		};
	}

	internal static SqlDataType UserDefinedDataTypeToEnum(UserDefinedDataType uddt)
	{
		SqlDataType sqlDataType = SqlToEnum(uddt.SystemType);
		switch (sqlDataType)
		{
		case SqlDataType.NVarChar:
		case SqlDataType.VarBinary:
		case SqlDataType.VarChar:
			if (uddt.MaxLength == -1)
			{
				switch (sqlDataType)
				{
				case SqlDataType.NVarChar:
					sqlDataType = SqlDataType.NVarCharMax;
					break;
				case SqlDataType.VarBinary:
					sqlDataType = SqlDataType.VarBinaryMax;
					break;
				case SqlDataType.VarChar:
					sqlDataType = SqlDataType.VarCharMax;
					break;
				}
			}
			break;
		case SqlDataType.UserDefinedDataType:
			TraceHelper.Assert(condition: false, "UDDT cannot have another UDDT as the system type");
			break;
		}
		return sqlDataType;
	}

	public static SqlDataType SqlToEnum(string sqlTypeName)
	{
		SqlDataType result = SqlDataType.None;
		if (string.IsNullOrEmpty(sqlTypeName))
		{
			return result;
		}
		switch (sqlTypeName.ToLowerInvariant())
		{
		case "bigint":
			result = SqlDataType.BigInt;
			break;
		case "binary":
			result = SqlDataType.Binary;
			break;
		case "bit":
			result = SqlDataType.Bit;
			break;
		case "char":
			result = SqlDataType.Char;
			break;
		case "datetime":
			result = SqlDataType.DateTime;
			break;
		case "datetime2":
			result = SqlDataType.DateTime2;
			break;
		case "time":
			result = SqlDataType.Time;
			break;
		case "date":
			result = SqlDataType.Date;
			break;
		case "datetimeoffset":
			result = SqlDataType.DateTimeOffset;
			break;
		case "hierarchyid":
			result = SqlDataType.HierarchyId;
			break;
		case "geometry":
			result = SqlDataType.Geometry;
			break;
		case "geography":
			result = SqlDataType.Geography;
			break;
		case "decimal":
			result = SqlDataType.Decimal;
			break;
		case "float":
			result = SqlDataType.Float;
			break;
		case "image":
			result = SqlDataType.Image;
			break;
		case "int":
			result = SqlDataType.Int;
			break;
		case "money":
			result = SqlDataType.Money;
			break;
		case "nchar":
			result = SqlDataType.NChar;
			break;
		case "ntext":
			result = SqlDataType.NText;
			break;
		case "nvarchar":
			result = SqlDataType.NVarChar;
			break;
		case "nvarcharmax":
			result = SqlDataType.NVarCharMax;
			break;
		case "real":
			result = SqlDataType.Real;
			break;
		case "smalldatetime":
			result = SqlDataType.SmallDateTime;
			break;
		case "smallint":
			result = SqlDataType.SmallInt;
			break;
		case "smallmoney":
			result = SqlDataType.SmallMoney;
			break;
		case "text":
			result = SqlDataType.Text;
			break;
		case "timestamp":
			result = SqlDataType.Timestamp;
			break;
		case "tinyint":
			result = SqlDataType.TinyInt;
			break;
		case "uniqueidentifier":
			result = SqlDataType.UniqueIdentifier;
			break;
		case "userdefinedtype":
			result = SqlDataType.UserDefinedType;
			break;
		case "varbinary":
			result = SqlDataType.VarBinary;
			break;
		case "varbinarymax":
			result = SqlDataType.VarBinaryMax;
			break;
		case "varchar":
			result = SqlDataType.VarChar;
			break;
		case "varcharmax":
			result = SqlDataType.VarCharMax;
			break;
		case "sql_variant":
			result = SqlDataType.Variant;
			break;
		case "xml":
			result = SqlDataType.Xml;
			break;
		case "sysname":
			result = SqlDataType.SysName;
			break;
		case "numeric":
			result = SqlDataType.Numeric;
			break;
		case "userdefineddatatype":
			result = SqlDataType.UserDefinedDataType;
			break;
		}
		return result;
	}

	private static bool IsSystemDataType80(SqlDataType dataType)
	{
		switch (dataType)
		{
		case SqlDataType.BigInt:
		case SqlDataType.Binary:
		case SqlDataType.Bit:
		case SqlDataType.Char:
		case SqlDataType.DateTime:
		case SqlDataType.Decimal:
		case SqlDataType.Float:
		case SqlDataType.Image:
		case SqlDataType.Int:
		case SqlDataType.Money:
		case SqlDataType.NChar:
		case SqlDataType.NText:
		case SqlDataType.NVarChar:
		case SqlDataType.Real:
		case SqlDataType.SmallDateTime:
		case SqlDataType.SmallInt:
		case SqlDataType.SmallMoney:
		case SqlDataType.Text:
		case SqlDataType.Timestamp:
		case SqlDataType.TinyInt:
		case SqlDataType.UniqueIdentifier:
		case SqlDataType.VarBinary:
		case SqlDataType.VarChar:
		case SqlDataType.Variant:
		case SqlDataType.SysName:
		case SqlDataType.Numeric:
			return true;
		default:
			return false;
		}
	}

	private static bool IsSystemDataType90(SqlDataType dataType)
	{
		if (IsSystemDataType80(dataType))
		{
			return true;
		}
		switch (dataType)
		{
		case SqlDataType.NVarCharMax:
		case SqlDataType.VarBinaryMax:
		case SqlDataType.VarCharMax:
		case SqlDataType.Xml:
			return true;
		default:
			return false;
		}
	}

	private static bool IsSystemDataType100(SqlDataType dataType)
	{
		if (IsSystemDataType90(dataType))
		{
			return true;
		}
		switch (dataType)
		{
		case SqlDataType.Date:
		case SqlDataType.Time:
		case SqlDataType.DateTimeOffset:
		case SqlDataType.DateTime2:
		case SqlDataType.HierarchyId:
		case SqlDataType.Geometry:
		case SqlDataType.Geography:
			return true;
		default:
			return false;
		}
	}

	internal static bool IsSystemDataType(SqlDataType dataType, SqlServerVersion targetVersion)
	{
		return targetVersion switch
		{
			SqlServerVersion.Version80 => IsSystemDataType80(dataType), 
			SqlServerVersion.Version90 => IsSystemDataType90(dataType), 
			_ => IsSystemDataType100(dataType), 
		};
	}

	internal static bool IsDataTypeSupportedOnTargetVersion(SqlDataType dataType, SqlServerVersion targetVersion)
	{
		if (IsSystemDataType(dataType, targetVersion))
		{
			return true;
		}
		if (targetVersion >= SqlServerVersion.Version100 && dataType == SqlDataType.UserDefinedTableType)
		{
			return true;
		}
		if (dataType == SqlDataType.UserDefinedType || dataType == SqlDataType.UserDefinedDataType)
		{
			return true;
		}
		TraceHelper.Assert(condition: false, "Unknown data type {0} - has IsDataTypeSupportedOnTargetVersion been updated with the latest supported types?");
		return false;
	}

	internal static bool IsDataTypeSupportedOnCloud(SqlDataType dataType)
	{
		return dataType != SqlDataType.UserDefinedType;
	}

	internal static bool IsTypeFloatStateCreating(string sqlType, SqlSmoObject sObj)
	{
		if (sObj.StringComparer.Compare(sqlType, "float") == 0 && sObj.State == SqlSmoState.Creating)
		{
			return true;
		}
		return false;
	}

	private bool HasTypeChangedToReal()
	{
		if (sqlDataType == SqlDataType.Float && parent != null && parent.Properties.Get("DataType").Value != null && ((string)parent.Properties.Get("DataType").Value).Equals("real", StringComparison.Ordinal))
		{
			return true;
		}
		return false;
	}

	XmlSchema IXmlSerializable.GetSchema()
	{
		return null;
	}

	void IXmlSerializable.ReadXml(XmlReader reader)
	{
		reader.ReadStartElement();
		sqlDataType = (SqlDataType)Enum.Parse(typeof(SqlDataType), reader.ReadElementContentAsString("SqlDataType", string.Empty));
		name = reader.ReadElementContentAsString("Name", string.Empty);
		schema = reader.ReadElementContentAsString("Schema", string.Empty);
		maximumLength = reader.ReadElementContentAsInt("MaximumLength", string.Empty);
		numericPrecision = reader.ReadElementContentAsInt("NumericPrecision", string.Empty);
		numericScale = reader.ReadElementContentAsInt("NumericScale", string.Empty);
		xmlDocumentConstraint = (XmlDocumentConstraint)Enum.Parse(typeof(XmlDocumentConstraint), reader.ReadElementContentAsString("XmlDocumentConstraint", string.Empty));
		reader.ReadEndElement();
	}

	void IXmlSerializable.WriteXml(XmlWriter writer)
	{
		IFormatProvider defaultCulture = SmoApplication.DefaultCulture;
		writer.WriteElementString("SqlDataType", Enum.GetName(typeof(SqlDataType), SqlDataType));
		writer.WriteElementString("Name", Name);
		writer.WriteElementString("Schema", Schema);
		writer.WriteElementString("MaximumLength", MaximumLength.ToString(defaultCulture));
		writer.WriteElementString("NumericPrecision", NumericPrecision.ToString(defaultCulture));
		writer.WriteElementString("NumericScale", NumericScale.ToString(defaultCulture));
		writer.WriteElementString("XmlDocumentConstraint", Enum.GetName(typeof(XmlDocumentConstraint), XmlDocumentConstraint));
	}
}
