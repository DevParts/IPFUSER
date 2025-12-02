using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcKeyChain : IEquatable<SfcKeyChain>, IUrn
{
	private SfcKey m_ThisKey;

	private SfcKeyChain m_Parent;

	public SfcKeyChain Parent
	{
		get
		{
			return m_Parent;
		}
		internal set
		{
			m_Parent = value;
		}
	}

	public SfcKey LeafKey
	{
		get
		{
			return m_ThisKey;
		}
		internal set
		{
			m_ThisKey = value;
		}
	}

	public DomainRootKey RootKey
	{
		get
		{
			SfcKeyChain sfcKeyChain = this;
			while (sfcKeyChain.m_Parent != null)
			{
				sfcKeyChain = sfcKeyChain.m_Parent;
			}
			return (DomainRootKey)sfcKeyChain.m_ThisKey;
		}
	}

	public bool IsRooted
	{
		get
		{
			SfcKeyChain sfcKeyChain = this;
			while (sfcKeyChain.m_Parent != null)
			{
				sfcKeyChain = sfcKeyChain.m_Parent;
			}
			if (sfcKeyChain.m_ThisKey != null)
			{
				return sfcKeyChain.m_ThisKey is DomainRootKey;
			}
			return false;
		}
	}

	public bool IsConnected
	{
		get
		{
			SfcKeyChain sfcKeyChain = this;
			while (sfcKeyChain.m_Parent != null)
			{
				sfcKeyChain = sfcKeyChain.m_Parent;
			}
			if (sfcKeyChain.m_ThisKey != null && sfcKeyChain.m_ThisKey is DomainRootKey)
			{
				return ((DomainRootKey)sfcKeyChain.m_ThisKey).Domain.ConnectionContext.Mode != SfcConnectionContextMode.Offline;
			}
			return false;
		}
	}

	public ISfcDomain Domain => RootKey.Domain;

	public Urn Urn => new Urn(this);

	XPathExpression IUrn.XPathExpression => new XPathExpression(ToString());

	string IUrn.Value
	{
		get
		{
			return ToString();
		}
		set
		{
			throw new InvalidOperationException();
		}
	}

	string IUrn.DomainInstanceName => Domain.DomainInstanceName;

	internal SfcKeyChain(DomainRootKey topKey)
	{
		m_ThisKey = topKey;
		m_Parent = null;
	}

	internal SfcKeyChain(SfcKey key, SfcKeyChain parent)
	{
		m_ThisKey = key;
		m_Parent = parent;
	}

	public SfcKeyChain(Urn urn, ISfcDomain domain)
	{
		XPathExpression xPathExpression = urn.XPathExpression;
		for (int i = 0; i < xPathExpression.Length; i++)
		{
			XPathExpressionBlock xpBlock = xPathExpression[i];
			SfcKey key = domain.GetKey(new XPathExpressionBlockImpl(xpBlock));
			if (i == 0)
			{
				SfcInstance sfcInstance = (SfcInstance)domain;
				if (!key.Equals(sfcInstance.AbstractIdentityKey))
				{
					throw new SfcInvalidQueryExpressionException(SfcStrings.BadQueryForConnection(key.ToString(), sfcInstance.ToString()));
				}
				m_Parent = null;
				m_ThisKey = key;
			}
			else
			{
				m_Parent = new SfcKeyChain(m_ThisKey, m_Parent);
				m_ThisKey = key;
			}
		}
	}

	private bool BodyEquals(SfcKeyChain kc)
	{
		if (m_ThisKey.GetType() != kc.m_ThisKey.GetType())
		{
			return false;
		}
		SfcKeyChain sfcKeyChain = this;
		SfcKeyChain sfcKeyChain2 = kc;
		do
		{
			if (sfcKeyChain.m_Parent != null && !sfcKeyChain.m_ThisKey.Equals(sfcKeyChain2.m_ThisKey))
			{
				return false;
			}
			sfcKeyChain = sfcKeyChain.m_Parent;
			sfcKeyChain2 = sfcKeyChain2.m_Parent;
		}
		while (sfcKeyChain != null && sfcKeyChain2 != null);
		if (sfcKeyChain == null)
		{
			return sfcKeyChain2 == null;
		}
		return false;
	}

	public bool ClientEquals(SfcKeyChain otherKeychain)
	{
		if (object.ReferenceEquals(this, otherKeychain))
		{
			return true;
		}
		if ((object)otherKeychain == null)
		{
			return false;
		}
		if (Domain != otherKeychain.Domain)
		{
			return false;
		}
		return BodyEquals(otherKeychain);
	}

	public bool ServerEquals(SfcKeyChain otherKeychain)
	{
		if (object.ReferenceEquals(this, otherKeychain))
		{
			return true;
		}
		if ((object)otherKeychain == null)
		{
			return false;
		}
		if (Domain.DomainName != otherKeychain.Domain.DomainName || Domain.DomainInstanceName != otherKeychain.Domain.DomainInstanceName)
		{
			return false;
		}
		return BodyEquals(otherKeychain);
	}

	private bool BodyDescendant(SfcKeyChain kc)
	{
		SfcKeyChain sfcKeyChain = this;
		SfcKeyChain sfcKeyChain2 = kc;
		Stack<SfcKey> stack = new Stack<SfcKey>();
		Stack<SfcKey> stack2 = new Stack<SfcKey>();
		while (sfcKeyChain != null && sfcKeyChain2 != null)
		{
			stack.Push(sfcKeyChain.m_ThisKey);
			stack2.Push(sfcKeyChain2.m_ThisKey);
			sfcKeyChain = sfcKeyChain.m_Parent;
			sfcKeyChain2 = sfcKeyChain2.m_Parent;
		}
		if (sfcKeyChain != null || sfcKeyChain2 == null)
		{
			return false;
		}
		while (sfcKeyChain2 != null)
		{
			stack2.Push(sfcKeyChain2.m_ThisKey);
			sfcKeyChain2 = sfcKeyChain2.m_Parent;
		}
		stack.Pop();
		stack2.Pop();
		while (stack.Count != 0)
		{
			if (!stack.Peek().Equals(stack2.Peek()))
			{
				return false;
			}
			stack.Pop();
			stack2.Pop();
		}
		return true;
	}

	public bool IsClientAncestorOf(SfcKeyChain otherKeychain)
	{
		if (object.ReferenceEquals(this, otherKeychain))
		{
			return true;
		}
		if ((object)otherKeychain == null)
		{
			return false;
		}
		if (Domain != otherKeychain.Domain)
		{
			return false;
		}
		return BodyDescendant(otherKeychain);
	}

	public bool IsServerAncestorOf(SfcKeyChain otherKeychain)
	{
		if (object.ReferenceEquals(this, otherKeychain))
		{
			return true;
		}
		if ((object)otherKeychain == null)
		{
			return false;
		}
		if (Domain.DomainName != otherKeychain.Domain.DomainName || Domain.DomainInstanceName != otherKeychain.Domain.DomainInstanceName)
		{
			return false;
		}
		return BodyDescendant(otherKeychain);
	}

	public override bool Equals(object obj)
	{
		SfcKeyChain sfcKeyChain = obj as SfcKeyChain;
		if (sfcKeyChain == null)
		{
			return false;
		}
		return Equals(sfcKeyChain);
	}

	public override int GetHashCode()
	{
		int num = 0;
		SfcKeyChain sfcKeyChain = this;
		do
		{
			num ^= sfcKeyChain.m_ThisKey.GetHashCode();
			sfcKeyChain = sfcKeyChain.m_Parent;
		}
		while (sfcKeyChain != null);
		return num;
	}

	public bool Equals(SfcKeyChain otherKeychain)
	{
		return ClientEquals(otherKeychain);
	}

	public static bool operator ==(SfcKeyChain leftOperand, SfcKeyChain rightOperand)
	{
		return leftOperand?.Equals(rightOperand) ?? ((object)rightOperand == null);
	}

	public static bool operator !=(SfcKeyChain leftOperand, SfcKeyChain rightOperand)
	{
		return !(leftOperand == rightOperand);
	}

	public SfcInstance GetObject()
	{
		if (LeafKey is DomainRootKey)
		{
			return (SfcInstance)Domain;
		}
		if (Parent == null || LeafKey == null)
		{
			return null;
		}
		SfcInstance sfcInstance = Parent.GetObject();
		string name = LeafKey.InstanceType.Name;
		ISfcCollection childCollection = sfcInstance.GetChildCollection(name);
		return childCollection.GetObjectByKey(LeafKey);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		SfcKeyChain sfcKeyChain = this;
		do
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Insert(0, "/");
			}
			stringBuilder.Insert(0, sfcKeyChain.m_ThisKey.GetUrnFragment());
			sfcKeyChain = sfcKeyChain.m_Parent;
		}
		while (sfcKeyChain != null);
		return stringBuilder.ToString();
	}
}
