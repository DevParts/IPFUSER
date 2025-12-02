using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SchemaNamedKey<T> : SfcKey, IEquatable<SchemaNamedKey<T>> where T : SfcInstance
{
	private string keySchema;

	private string keyName;

	public string Schema => keySchema;

	public string Name => keyName;

	public sealed override Type InstanceType => typeof(T);

	protected virtual string UrnName => typeof(T).Name;

	public SchemaNamedKey()
	{
		keySchema = string.Empty;
		keyName = string.Empty;
	}

	public SchemaNamedKey(string name)
		: this("dbo", name)
	{
	}

	public SchemaNamedKey(string schema, string name)
	{
		if (schema == null || schema.Length == 0)
		{
			throw new ArgumentNullException("schema");
		}
		if (name == null || name.Length == 0)
		{
			throw new ArgumentNullException("name");
		}
		keySchema = schema;
		keyName = name;
	}

	public SchemaNamedKey(SchemaNamedKey<T> other)
	{
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		keySchema = other.Schema;
		keyName = other.Name;
	}

	public SchemaNamedKey(IDictionary<string, object> fields)
	{
		keySchema = (string)fields["Schema"];
		keyName = (string)fields["Name"];
	}

	public override bool Equals(object obj)
	{
		if (object.ReferenceEquals(obj, null) || !(obj is SchemaNamedKey<T>))
		{
			return false;
		}
		return Equals((SchemaNamedKey<T>)obj);
	}

	public override bool Equals(SfcKey other)
	{
		if (object.ReferenceEquals(other, null) || !(other is SchemaNamedKey<T>))
		{
			return false;
		}
		return Equals((SchemaNamedKey<T>)other);
	}

	public bool Equals(SchemaNamedKey<T> other)
	{
		if (object.ReferenceEquals(this, other))
		{
			return true;
		}
		if (object.ReferenceEquals(other, null))
		{
			return false;
		}
		if (string.CompareOrdinal(Name, other.Name) == 0)
		{
			return string.CompareOrdinal(Schema, other.Schema) == 0;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode() ^ Schema.GetHashCode();
	}

	public override string ToString()
	{
		return Schema + "." + Name;
	}

	public new static bool Equals(object leftOperand, object rightOperand)
	{
		if (object.ReferenceEquals(leftOperand, null))
		{
			if (!object.ReferenceEquals(rightOperand, null))
			{
				return false;
			}
			return true;
		}
		return leftOperand.Equals(rightOperand);
	}

	public static bool operator ==(SchemaNamedKey<T> leftOperand, SchemaNamedKey<T> rightOperand)
	{
		if (object.ReferenceEquals(leftOperand, null))
		{
			if (!object.ReferenceEquals(rightOperand, null))
			{
				return false;
			}
			return true;
		}
		return leftOperand.Equals(rightOperand);
	}

	public static bool operator !=(SchemaNamedKey<T> leftOperand, SchemaNamedKey<T> rightOperand)
	{
		if (object.ReferenceEquals(leftOperand, null))
		{
			if (!object.ReferenceEquals(rightOperand, null))
			{
				return true;
			}
			return false;
		}
		return !leftOperand.Equals(rightOperand);
	}

	public override string GetUrnFragment()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}[@Name='{1}' and @Schema = '{2}']", new object[3]
		{
			UrnName,
			SfcSecureString.EscapeSquote(keyName),
			SfcSecureString.EscapeSquote(Schema)
		});
	}
}
