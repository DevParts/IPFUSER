using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
[CLSCompliant(false)]
public class SfcReferenceCollectionAttribute : Attribute
{
	private ISfcReferenceCollectionResolver resolver;

	private string[] args;

	public ISfcReferenceCollectionResolver CollectionResolver => resolver;

	public string[] Arguments => args;

	public SfcReferenceCollectionAttribute(Type resolverType)
		: this(resolverType, null, null)
	{
	}

	public SfcReferenceCollectionAttribute(Type resolverType, params string[] parameters)
	{
		if (resolverType == null)
		{
			throw new ArgumentNullException("resolverType", SfcStrings.SfcNullArgumentToSfcReferenceAttribute(typeof(SfcReferenceCollectionAttribute).Name));
		}
		args = parameters;
		MethodInfo method = resolverType.GetMethod("GetReferenceCollectionResolver", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		TraceHelper.Assert(method != null, string.Format(CultureInfo.InvariantCulture, "{0} resolver factory method on type '{1}' not found. Note: this method must be defined as a static method.", new object[2] { "GetReferenceCollectionResolver", resolverType.FullName }));
		Delegate obj = method.CreateDelegate(typeof(SfcReferenceCollectionResolverFactoryDelegate));
		resolver = ((SfcReferenceCollectionResolverFactoryDelegate)obj)(parameters);
		if (resolver == null)
		{
			throw new InvalidOperationException(SfcStrings.SfcNullInvalidSfcReferenceResolver(resolverType.Name, typeof(ISfcReferenceCollectionResolver).Name));
		}
	}

	public IEnumerable ResolveCollection(object instance)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance", SfcStrings.SfcNullArgumentToResolveCollection);
		}
		return resolver.ResolveCollection(instance, args);
	}

	public IEnumerable<T> ResolveCollection<T, S>(S instance)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance", SfcStrings.SfcNullArgumentToResolveCollection);
		}
		if (!(resolver is ISfcReferenceCollectionResolver<T, S> sfcReferenceCollectionResolver))
		{
			throw new InvalidOperationException(SfcStrings.SfcNullInvalidSfcReferenceResolver(resolver.GetType().Name, typeof(ISfcReferenceCollectionResolver<T, S>).Name));
		}
		return sfcReferenceCollectionResolver.ResolveCollection(instance, args);
	}
}
