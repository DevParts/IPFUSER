using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[CLSCompliant(false)]
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
public class SfcReferenceAttribute : Attribute
{
	internal const string SfcReferenceResolverFactoryMethodName = "GetReferenceResolver";

	internal const string SfcReferenceCollectionResolverFactoryMethodName = "GetReferenceCollectionResolver";

	private string[] m_args;

	private string m_pathExpression;

	private ISfcReferenceResolver m_sfcReferenceResolver;

	private Delegate m_resolver;

	private Delegate m_urnResolver;

	private string[] m_keys;

	private Type m_type;

	public string UrnTemplate => m_pathExpression;

	public string[] Arguments => m_args;

	public Delegate Resolver => m_resolver;

	public ISfcReferenceResolver InstanceResolver => m_sfcReferenceResolver;

	public string[] Keys
	{
		get
		{
			return m_keys;
		}
		set
		{
			m_keys = value;
		}
	}

	public Type Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	public SfcReferenceAttribute(Type resolverType)
		: this(resolverType, (string[])null)
	{
	}

	public SfcReferenceAttribute(Type resolverType, string[] parameters)
	{
		if (resolverType == null)
		{
			throw new ArgumentNullException("resolverType", SfcStrings.SfcNullArgumentToSfcReferenceAttribute(typeof(SfcReferenceAttribute).Name));
		}
		m_args = parameters;
		MethodInfo method = resolverType.GetMethod("GetReferenceResolver", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		TraceHelper.Assert(method != null, string.Format(CultureInfo.InvariantCulture, "{0} resolver factory method on type '{1}' not found. Note: this method must be defined as a static method.", new object[2] { "GetReferenceResolver", resolverType.FullName }));
		Delegate obj = method.CreateDelegate(typeof(SfcReferenceResolverFactoryDelegate));
		m_sfcReferenceResolver = ((SfcReferenceResolverFactoryDelegate)obj)(parameters);
		if (m_sfcReferenceResolver == null)
		{
			throw new InvalidOperationException(SfcStrings.SfcNullInvalidSfcReferenceResolver(resolverType.Name, typeof(ISfcReferenceResolver).Name));
		}
	}

	public SfcReferenceAttribute(Type referenceType, string urnTemplate, params string[] parameters)
	{
		m_pathExpression = urnTemplate;
		m_args = parameters;
		m_type = referenceType;
	}

	public SfcReferenceAttribute(Type referenceType, Type resolverType, string methodName, params string[] parameters)
		: this(referenceType, null, resolverType, methodName, parameters)
	{
	}

	public SfcReferenceAttribute(Type referenceType, string[] keys, string urnTemplate, params string[] parameters)
	{
		m_keys = keys;
		m_pathExpression = urnTemplate;
		m_args = parameters;
		m_type = referenceType;
	}

	public SfcReferenceAttribute(Type referenceType, string[] keys, Type resolverType, string methodName, params string[] parameters)
	{
		m_keys = keys;
		MethodInfo method = resolverType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		TraceHelper.Assert(method != null, string.Format(CultureInfo.InvariantCulture, "{0} method on type {1} not found. Note: this method must be defined as a static method.", new object[2] { methodName, resolverType.FullName }));
		m_resolver = method.CreateDelegate(typeof(ReferenceResolverDelegate));
		method = resolverType.GetMethod("ResolveUrn", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (method != null)
		{
			m_urnResolver = method.CreateDelegate(typeof(ReferenceResolverDelegate));
		}
		m_args = parameters;
		m_type = referenceType;
	}

	public T Resolve<T, S>(S instance)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance", SfcStrings.SfcNullArgumentToResolve);
		}
		if (m_sfcReferenceResolver != null)
		{
			if (!(m_sfcReferenceResolver is ISfcReferenceResolver<T, S> sfcReferenceResolver))
			{
				throw new InvalidOperationException(SfcStrings.SfcNullInvalidSfcReferenceResolver(m_sfcReferenceResolver.GetType().Name, typeof(ISfcReferenceResolver<T, S>).Name));
			}
			return sfcReferenceResolver.Resolve(instance, m_args);
		}
		return (T)ResolveDelegateOrPath(instance);
	}

	public object Resolve(object instance)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance", SfcStrings.SfcNullArgumentToResolve);
		}
		if (m_sfcReferenceResolver != null)
		{
			return m_sfcReferenceResolver.Resolve(instance, m_args);
		}
		return ResolveDelegateOrPath(instance);
	}

	private object ResolveDelegateOrPath(object instance)
	{
		if ((object)m_resolver != null)
		{
			return ((ReferenceResolverDelegate)m_resolver)(instance, AttributeUtilities.GetValuesOfProperties(instance, m_args));
		}
		object result = null;
		if (m_pathExpression != null && m_args != null && m_args.Length != 0)
		{
			SfcObjectQuery sfcObjectQuery = null;
			string text = string.Format(CultureInfo.InvariantCulture, m_pathExpression, AttributeUtilities.GetValuesOfProperties(instance, m_args));
			Urn urn = new Urn(text);
			SqlStoreConnection sqlStoreConnection = null;
			ISfcDomain sfcDomain = null;
			if (instance is IAlienObject)
			{
				IAlienObject alienObject = instance as IAlienObject;
				return alienObject.Resolve(text);
			}
			if (instance is SfcInstance)
			{
				sfcDomain = ((SfcInstance)instance).KeyChain.Domain;
				if (sfcDomain.ConnectionContext.Mode == SfcConnectionContextMode.Offline)
				{
					return null;
				}
				sqlStoreConnection = sfcDomain.GetConnection() as SqlStoreConnection;
			}
			string name = urn.XPathExpression[0].Name;
			if (string.Compare(name, "Server", StringComparison.OrdinalIgnoreCase) == 0)
			{
				TraceHelper.Assert(condition: false);
			}
			else
			{
				foreach (SfcDomainInfo domain in SfcRegistration.Domains)
				{
					if (string.Compare(name, domain.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						sfcDomain = domain.RootType.Assembly().CreateInstance(domain.RootTypeFullName, ignoreCase: false, BindingFlags.CreateInstance, null, new object[1] { sqlStoreConnection }, CultureInfo.InvariantCulture, null) as ISfcDomain;
						sfcObjectQuery = new SfcObjectQuery(sfcDomain);
						break;
					}
				}
			}
			int num = 0;
			foreach (object item in sfcObjectQuery.ExecuteIterator(new SfcQueryExpression(text), null, null))
			{
				result = item;
				num++;
			}
			_ = 1;
		}
		return result;
	}

	public Urn GetUrn(object instance)
	{
		try
		{
			if ((object)m_urnResolver != null)
			{
				return (Urn)((ReferenceResolverDelegate)m_urnResolver)(instance, AttributeUtilities.GetValuesOfProperties(instance, m_args));
			}
			if (m_pathExpression == null)
			{
				return null;
			}
			string value = string.Format(m_pathExpression, AttributeUtilities.GetValuesOfProperties(instance, m_args));
			return new Urn(value);
		}
		catch (TargetInvocationException ex)
		{
			if (ex.InnerException != null && string.Compare(ex.InnerException.GetType().FullName, "Microsoft.SqlServer.Management.Smo.UnknownPropertyException", StringComparison.Ordinal) == 0)
			{
				throw new SfcUnsupportedVersionException(SfcStrings.PropertyNotsupported, ex);
			}
			throw;
		}
	}
}
