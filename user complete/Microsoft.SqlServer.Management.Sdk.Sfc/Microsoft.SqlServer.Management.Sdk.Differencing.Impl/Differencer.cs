using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal class Differencer : IDifferencer
{
	protected interface IDiffContext
	{
		NodeItemNamesAdapterProvider NodeItemNamesAdapterProvider { get; }

		AvailablePropertyValueProvider SourceAvailablePropertyValueProvider { get; }

		AvailablePropertyValueProvider TargetAvailablePropertyValueProvider { get; }

		ContainerSortingProvider ContainerSortingProvider { get; }

		PropertyComparerProvider PropertyComparerProvider { get; }

		void Push(ISfcSimpleNode source, ISfcSimpleNode target);

		void Add(IDiffEntry entry);

		bool IsTypeEmitted(DiffType type);
	}

	protected class LateActivatedDiffgram : Diffgram, IDiffgram, IEnumerable<IDiffEntry>, IEnumerable
	{
		private Differencer differencer;

		private DiffType emitDiffTypes;

		private NodeItemNamesAdapterProvider nameProvider;

		private AvailablePropertyValueProvider sourceValueProvider;

		private AvailablePropertyValueProvider targetValueProvider;

		private ContainerSortingProvider sortProvider;

		private PropertyComparerProvider propComparer;

		private ISfcSimpleNode source;

		private ISfcSimpleNode target;

		public Differencer Differencer => differencer;

		public NodeItemNamesAdapterProvider NodeItemNamesAdapterProvider => nameProvider;

		public AvailablePropertyValueProvider SourceAvailablePropertyValueProvider => sourceValueProvider;

		public AvailablePropertyValueProvider TargetAvailablePropertyValueProvider => targetValueProvider;

		public ContainerSortingProvider ContainerSortingProvider => sortProvider;

		public PropertyComparerProvider PropertyComparerProvider => propComparer;

		public DiffType EmitDiffTypes => emitDiffTypes;

		public ISfcSimpleNode SourceSimpleNode => source;

		public ISfcSimpleNode TargetSimpleNode => target;

		public LateActivatedDiffgram(Differencer differencer, NodeItemNamesAdapterProvider nameProvider, AvailablePropertyValueProvider sourceValueProvider, AvailablePropertyValueProvider targetValueProvider, ContainerSortingProvider sortProvider, PropertyComparerProvider propComparer, DiffType emitDiffTypes, ISfcSimpleNode source, ISfcSimpleNode target)
			: base(source.ObjectReference, target.ObjectReference)
		{
			this.differencer = differencer;
			this.nameProvider = nameProvider;
			this.sourceValueProvider = sourceValueProvider;
			this.targetValueProvider = targetValueProvider;
			this.sortProvider = sortProvider;
			this.propComparer = propComparer;
			this.emitDiffTypes = emitDiffTypes;
			this.source = source;
			this.target = target;
			TraceHelper.Trace("Differencing", "Diffgram: created late-activated diffgram.");
		}

		public override IEnumerator<IDiffEntry> GetEnumerator()
		{
			TraceHelper.Trace("Differencing", "Diffgram: entering GetEnumerator");
			LateActivatedDiffEntryEnumerator lateActivatedDiffEntryEnumerator = new LateActivatedDiffEntryEnumerator(this);
			lateActivatedDiffEntryEnumerator.Push(SourceSimpleNode, TargetSimpleNode);
			TraceHelper.Trace("Differencing", "Diffgram: exiting GetEnumerator");
			return lateActivatedDiffEntryEnumerator;
		}
	}

	private class LateActivatedDiffEntryEnumerator : IEnumerator<IDiffEntry>, IDisposable, IEnumerator, IDiffContext
	{
		private LateActivatedDiffgram envelope;

		private Stack<Pair<ISfcSimpleNode>> stack = new Stack<Pair<ISfcSimpleNode>>();

		private Queue<IDiffEntry> result = new Queue<IDiffEntry>();

		private IDiffEntry current;

		public NodeItemNamesAdapterProvider NodeItemNamesAdapterProvider => envelope.NodeItemNamesAdapterProvider;

		public AvailablePropertyValueProvider SourceAvailablePropertyValueProvider => envelope.SourceAvailablePropertyValueProvider;

		public AvailablePropertyValueProvider TargetAvailablePropertyValueProvider => envelope.TargetAvailablePropertyValueProvider;

		public ContainerSortingProvider ContainerSortingProvider => envelope.ContainerSortingProvider;

		public PropertyComparerProvider PropertyComparerProvider => envelope.PropertyComparerProvider;

		public IDiffEntry Current
		{
			get
			{
				if (current == null)
				{
					throw new InvalidOperationException();
				}
				return current;
			}
		}

		object IEnumerator.Current => Current;

		public LateActivatedDiffEntryEnumerator(LateActivatedDiffgram envelop)
		{
			envelope = envelop;
		}

		public bool IsTypeEmitted(DiffType type)
		{
			return (envelope.EmitDiffTypes & type) != 0;
		}

		public void Push(ISfcSimpleNode source, ISfcSimpleNode target)
		{
			TraceHelper.Assert(source != null && target != null, "assert added node is not null");
			TraceHelper.Assert(source.Urn != null && target.Urn != null, "assert added node's urn is not null");
			TraceHelper.Trace("Differencing", "DiffEntryEnumerator: pushing a pair {0} and {1} for later comparison.", source.Urn, target.Urn);
			Pair<ISfcSimpleNode> item = new Pair<ISfcSimpleNode>(source, target);
			stack.Push(item);
			TraceHelper.Trace("Differencing", "DiffEntryEnumerator: pushed.");
		}

		public void Add(IDiffEntry entry)
		{
			TraceHelper.Assert(entry != null, "assert enqueueing entry is not null");
			TraceHelper.Trace("Differencing", "DiffEntryEnumerator: enqueueing result {0}.", entry);
			result.Enqueue(entry);
			TraceHelper.Trace("Differencing", "DiffEntryEnumerator: enqueued result.");
		}

		public bool MoveNext()
		{
			TraceHelper.Trace("Differencing", "DiffEntryEnumerator: entering MoveNext.");
			current = null;
			while (true)
			{
				if (result.Count > 0)
				{
					current = result.Dequeue();
					TraceHelper.Trace("Differencing", "DiffEntryEnumerator: exiting MoveNext (returning true).");
					return true;
				}
				if (stack.Count <= 0)
				{
					break;
				}
				Pair<ISfcSimpleNode> pair = stack.Pop();
				envelope.Differencer.ProcessNodes(this, pair.Source, pair.Target);
			}
			TraceHelper.Trace("Differencing", "DiffEntryEnumerator: exiting MoveNext (no more to compare. returning false.).");
			return false;
		}

		public void Reset()
		{
			current = null;
			stack.Clear();
			result.Clear();
		}

		public void Dispose()
		{
		}
	}

	private class DummySfcSimpleNode : ISfcSimpleNode
	{
		public object ObjectReference
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ISfcSimpleMap<string, object> Properties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ISfcSimpleMap<string, ISfcSimpleList> RelatedContainers
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ISfcSimpleMap<string, ISfcSimpleNode> RelatedObjects
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Urn Urn => "<empty>";
	}

	internal const string ComponentName = "Differencing";

	private static readonly AvailablePropertyValueProvider DEFAULT_AVAILABLE_PROPERTY_VALUE_PROVIDER = new DefaultAvailablePropertyValueProvider();

	private static readonly ContainerSortingProvider DEFAULT_SORT_PROVIDER = new DefaultContainerSortingProvider();

	private static readonly PropertyComparerProvider DEFAULT_PROP_COMPARER = new DefaultPropertyComparerProvider();

	private static readonly ISfcSimpleNode DUMMY_NODE = new DummySfcSimpleNode();

	private readonly ProviderRegistry registry;

	private DiffType emittedChangeType;

	internal Differencer(ProviderRegistry registry)
	{
		this.registry = registry;
		SetTypeEmitted(DiffType.Updated);
		SetTypeEmitted(DiffType.Created);
		SetTypeEmitted(DiffType.Deleted);
		UnsetTypeEmitted(DiffType.Equivalent);
	}

	public bool IsTypeEmitted(DiffType type)
	{
		return (emittedChangeType & type) != 0;
	}

	public void SetTypeEmitted(DiffType type)
	{
		emittedChangeType |= type;
	}

	public void UnsetTypeEmitted(DiffType type)
	{
		emittedChangeType &= ~type;
	}

	public IDiffgram CompareGraphs(object source, object target)
	{
		TraceHelper.Trace("Differencing", "CompareGraphs: entering public method.");
		if (source == null)
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: argument null 'source'.");
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: argument null 'target'.");
			throw new ArgumentNullException("target");
		}
		if (source.GetType() != target.GetType())
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: argument types do not match.");
			throw new ArgumentException(StringDifferencing.MismatchType(source.ToString(), target.ToString()));
		}
		SfcNodeAdapterProvider sfcNodeAdapterProvider = FindNodeAdapterProvider(source);
		if (sfcNodeAdapterProvider == null)
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: cannot find node adapter that can navigate the specified input.");
			throw new ArgumentException(StringDifferencing.NotRecognizedGraph);
		}
		ISfcSimpleNode sfcSimpleNode = AdaptNode(sfcNodeAdapterProvider, source);
		ISfcSimpleNode sfcSimpleNode2 = AdaptNode(sfcNodeAdapterProvider, target);
		NodeItemNamesAdapterProvider nodeItemNamesAdapterProvider = FindNameProvider(sfcSimpleNode);
		if (nodeItemNamesAdapterProvider == null)
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: cannot find name (metadata) provider that can navigate the specified input.");
			throw new ArgumentException(StringDifferencing.CannotFindMetadataProvider);
		}
		AvailablePropertyValueProvider availablePropertyValueProvider = FindAvailableValueProvider(sfcSimpleNode);
		if (availablePropertyValueProvider == null)
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: cannot find value available provider. default is used.");
			availablePropertyValueProvider = DEFAULT_AVAILABLE_PROPERTY_VALUE_PROVIDER;
		}
		AvailablePropertyValueProvider availablePropertyValueProvider2 = FindAvailableValueProvider(sfcSimpleNode2);
		if (availablePropertyValueProvider2 == null)
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: cannot find value available provider. default is used.");
			availablePropertyValueProvider2 = DEFAULT_AVAILABLE_PROPERTY_VALUE_PROVIDER;
		}
		PropertyComparerProvider propertyComparerProvider = FindPropertyComparerProvider(sfcSimpleNode, sfcSimpleNode2);
		if (propertyComparerProvider == null)
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: cannot find property comparer provider. default is used.");
			propertyComparerProvider = DEFAULT_PROP_COMPARER;
		}
		ContainerSortingProvider containerSortingProvider = FindContainerSortingProvider(sfcSimpleNode, sfcSimpleNode2);
		if (containerSortingProvider == null)
		{
			TraceHelper.Trace("Differencing", "CompareGraphs: cannot find value sorting provider. default is used.");
			containerSortingProvider = DEFAULT_SORT_PROVIDER;
		}
		TraceHelper.Trace("Differencing", "CompareGraphs: parameter verified.");
		LateActivatedDiffgram result = new LateActivatedDiffgram(this, nodeItemNamesAdapterProvider, availablePropertyValueProvider, availablePropertyValueProvider2, containerSortingProvider, propertyComparerProvider, emittedChangeType, sfcSimpleNode, sfcSimpleNode2);
		TraceHelper.Trace("Differencing", "CompareGraphs: exiting public method.");
		return result;
	}

	protected void ProcessNodes(IDiffContext context, ISfcSimpleNode source, ISfcSimpleNode target)
	{
		if (object.ReferenceEquals(DUMMY_NODE, target))
		{
			foreach (string relatedContainerName in GetRelatedContainerNames(context.NodeItemNamesAdapterProvider, source))
			{
				WalkCreatedList(context, source.RelatedContainers[relatedContainerName]);
			}
			{
				foreach (string relatedObjectName in GetRelatedObjectNames(context.NodeItemNamesAdapterProvider, source))
				{
					ISfcSimpleNode sfcSimpleNode = source.RelatedObjects[relatedObjectName];
					if (sfcSimpleNode != null)
					{
						EmitCreatedEntry(context, sfcSimpleNode);
					}
				}
				return;
			}
		}
		if (object.ReferenceEquals(DUMMY_NODE, source))
		{
			foreach (string relatedContainerName2 in GetRelatedContainerNames(context.NodeItemNamesAdapterProvider, target))
			{
				WalkDeletedList(context, target.RelatedContainers[relatedContainerName2]);
			}
			{
				foreach (string relatedObjectName2 in GetRelatedObjectNames(context.NodeItemNamesAdapterProvider, target))
				{
					ISfcSimpleNode sfcSimpleNode2 = target.RelatedObjects[relatedObjectName2];
					if (sfcSimpleNode2 != null)
					{
						EmitDeletedEntry(context, sfcSimpleNode2);
					}
				}
				return;
			}
		}
		CompareNodes(context, source, target);
	}

	protected void CompareNodes(IDiffContext context, ISfcSimpleNode source, ISfcSimpleNode target)
	{
		TraceHelper.Assert(source != null && target != null, "assert input nodes are not null");
		TraceHelper.Assert(source.Urn != null && target.Urn != null, "assert input nodes' Urns are not null");
		TraceHelper.Trace("Differencing", "CompareNodes: comparing two nodes {0} and {1}.", source.Urn, target.Urn);
		CompareProperties(context, source, target);
		TraceHelper.Trace("Differencing", "CompareNodes: looping all related containers (collection) of the node.");
		foreach (string relatedContainerName in GetRelatedContainerNames(context.NodeItemNamesAdapterProvider, source))
		{
			bool naturalOrder = GetNaturalOrder(context.NodeItemNamesAdapterProvider, source, relatedContainerName);
			CompareRelatedContainer(context, source.RelatedContainers[relatedContainerName], target.RelatedContainers[relatedContainerName], naturalOrder);
		}
		TraceHelper.Trace("Differencing", "CompareNodes: looped all related containers (collection) of the node.");
		TraceHelper.Trace("Differencing", "CompareNodes: looping all related object (singleton) of the node.");
		foreach (string relatedObjectName in GetRelatedObjectNames(context.NodeItemNamesAdapterProvider, source))
		{
			CompareRelatedObject(context, source.RelatedObjects[relatedObjectName], target.RelatedObjects[relatedObjectName]);
		}
		TraceHelper.Trace("Differencing", "CompareNodes: looped all related object (singleton) of the node.");
		TraceHelper.Trace("Differencing", "CompareNodes: compared two nodes.");
	}

	protected void CompareRelatedContainer(IDiffContext context, ISfcSimpleList source, ISfcSimpleList target, bool naturalOrder)
	{
		TraceHelper.Assert(source != null && target != null, "assert input is not null");
		TraceHelper.Trace("Differencing", "CompareRelatedContainer: comparing element in two container.");
		IEnumerator<ISfcSimpleNode> enumerator = null;
		IEnumerator<ISfcSimpleNode> enumerator2 = null;
		try
		{
			if (naturalOrder)
			{
				TraceHelper.Trace("Differencing", "CompareRelatedContainer: use natural order (no sorting).");
				enumerator = source.GetEnumerator();
				enumerator2 = target.GetEnumerator();
			}
			else
			{
				bool flag;
				using (IEnumerator<ISfcSimpleNode> enumerator3 = source.GetEnumerator())
				{
					flag = enumerator3.MoveNext();
				}
				bool flag2;
				using (IEnumerator<ISfcSimpleNode> enumerator4 = target.GetEnumerator())
				{
					flag2 = enumerator4.MoveNext();
				}
				if (flag && flag2)
				{
					TraceHelper.Trace("Differencing", "CompareRelatedContainer: use sorting.");
					IEnumerable<ISfcSimpleNode> sortedSource = null;
					IEnumerable<ISfcSimpleNode> sortedTarget = null;
					GetSortedLists(context.ContainerSortingProvider, source, target, out sortedSource, out sortedTarget);
					enumerator = sortedSource.GetEnumerator();
					enumerator2 = sortedTarget.GetEnumerator();
				}
				else
				{
					enumerator = source.GetEnumerator();
					enumerator2 = target.GetEnumerator();
				}
			}
			ISfcSimpleNode sfcSimpleNode = null;
			ISfcSimpleNode sfcSimpleNode2 = null;
			IComparer<ISfcSimpleNode> comparer = null;
			while (true)
			{
				TraceHelper.Trace("Differencing", "CompareRelatedContainer: move cursor.");
				if (sfcSimpleNode == null && enumerator.MoveNext())
				{
					sfcSimpleNode = enumerator.Current;
				}
				if (sfcSimpleNode2 == null && enumerator2.MoveNext())
				{
					sfcSimpleNode2 = enumerator2.Current;
				}
				if (sfcSimpleNode == null && sfcSimpleNode2 == null)
				{
					break;
				}
				int num = 0;
				if (sfcSimpleNode == null)
				{
					num = 1;
				}
				else if (sfcSimpleNode2 == null)
				{
					num = -1;
				}
				else
				{
					if (comparer == null)
					{
						comparer = GetComparer(context.ContainerSortingProvider, source, target);
					}
					num = CompareIdentities(sfcSimpleNode, sfcSimpleNode2, comparer);
				}
				if (num < 0)
				{
					TraceHelper.Trace("Differencing", "CompareRelatedContainer: could not find matched element on the other side (created case).");
					EmitCreatedEntry(context, sfcSimpleNode);
					sfcSimpleNode = null;
				}
				else if (num > 0)
				{
					TraceHelper.Trace("Differencing", "CompareRelatedContainer: could not find matched element on the other side (deleted case).");
					EmitDeletedEntry(context, sfcSimpleNode2);
					sfcSimpleNode2 = null;
				}
				else
				{
					TraceHelper.Trace("Differencing", "CompareRelatedContainer: found matched elements. push for later comparison (breadth first)");
					context.Push(sfcSimpleNode, sfcSimpleNode2);
					sfcSimpleNode = null;
					sfcSimpleNode2 = null;
				}
			}
		}
		finally
		{
			Dispose(enumerator);
			Dispose(enumerator2);
		}
		TraceHelper.Trace("Differencing", "CompareRelatedContainer: compared element in two containers.");
	}

	protected void CompareRelatedObject(IDiffContext context, ISfcSimpleNode source, ISfcSimpleNode target)
	{
		if (source != null || target != null)
		{
			if (target == null)
			{
				TraceHelper.Trace("Differencing", "CompareRelatedObject: related object is null (create case).");
				EmitCreatedEntry(context, source);
			}
			else if (source == null)
			{
				TraceHelper.Trace("Differencing", "CompareRelatedObject: related object is null (delete case).");
				EmitDeletedEntry(context, target);
			}
			else
			{
				TraceHelper.Trace("Differencing", "CompareRelatedContainer: found matched elements. push for later comparison (breadth first)");
				context.Push(source, target);
			}
		}
	}

	protected void CompareProperties(IDiffContext context, ISfcSimpleNode source, ISfcSimpleNode target)
	{
		TraceHelper.Trace("Differencing", "CompareGraphs: comparing properties of two nodes.");
		IDictionary<string, IPair<object>> dictionary = null;
		foreach (string propertyName in GetPropertyNames(context.NodeItemNamesAdapterProvider, source))
		{
			if (GetIsValueAvailable(context.SourceAvailablePropertyValueProvider, source, propertyName) && GetIsValueAvailable(context.TargetAvailablePropertyValueProvider, target, propertyName) && !context.PropertyComparerProvider.Compare(source, target, propertyName))
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, IPair<object>>();
				}
				object source2 = source.Properties[propertyName];
				object target2 = target.Properties[propertyName];
				dictionary.Add(propertyName, new Pair<object>(source2, target2));
			}
		}
		if (dictionary != null)
		{
			EmitUpdatedEntry(context, source, target, dictionary);
		}
		else
		{
			EmitEquivalentEntry(context, source, target);
		}
		TraceHelper.Trace("Differencing", "CompareGraphs: compared properties of two nodes.");
	}

	protected int CompareIdentities(ISfcSimpleNode left, ISfcSimpleNode right, IComparer<ISfcSimpleNode> comparer)
	{
		if (left == null && right == null)
		{
			return 0;
		}
		if (left == null)
		{
			return -1;
		}
		if (right == null)
		{
			return 1;
		}
		if (left.Urn == null)
		{
			throw new ArgumentNullException("left.Urn");
		}
		if (right.Urn == null)
		{
			throw new ArgumentNullException("right.Urn");
		}
		return comparer.Compare(left, right);
	}

	private void EmitEquivalentEntry(IDiffContext context, ISfcSimpleNode left, ISfcSimpleNode right)
	{
		if (context.IsTypeEmitted(DiffType.Equivalent))
		{
			DiffEntry diffEntry = new DiffEntry();
			diffEntry.Source = left.Urn;
			diffEntry.Target = right.Urn;
			diffEntry.ChangeType = DiffType.Equivalent;
			context.Add(diffEntry);
		}
	}

	private void EmitUpdatedEntry(IDiffContext context, ISfcSimpleNode left, ISfcSimpleNode right, IDictionary<string, IPair<object>> props)
	{
		if (context.IsTypeEmitted(DiffType.Updated))
		{
			DiffEntry diffEntry = new DiffEntry();
			diffEntry.Source = left.Urn;
			diffEntry.Target = right.Urn;
			diffEntry.Properties = props;
			diffEntry.ChangeType = DiffType.Updated;
			context.Add(diffEntry);
		}
	}

	private void EmitCreatedEntry(IDiffContext context, ISfcSimpleNode left)
	{
		if (context.IsTypeEmitted(DiffType.Created))
		{
			DiffEntry diffEntry = new DiffEntry();
			diffEntry.Source = left.Urn;
			diffEntry.Target = null;
			diffEntry.ChangeType = DiffType.Created;
			context.Add(diffEntry);
			context.Push(left, DUMMY_NODE);
		}
	}

	private void EmitDeletedEntry(IDiffContext context, ISfcSimpleNode right)
	{
		if (context.IsTypeEmitted(DiffType.Deleted))
		{
			DiffEntry diffEntry = new DiffEntry();
			diffEntry.Target = right.Urn;
			diffEntry.Source = null;
			diffEntry.ChangeType = DiffType.Deleted;
			context.Add(diffEntry);
			context.Push(DUMMY_NODE, right);
		}
	}

	protected void WalkCreatedList(IDiffContext context, ISfcSimpleList list)
	{
		foreach (ISfcSimpleNode item in list)
		{
			EmitCreatedEntry(context, item);
		}
	}

	protected void WalkDeletedList(IDiffContext context, ISfcSimpleList list)
	{
		foreach (ISfcSimpleNode item in list)
		{
			EmitDeletedEntry(context, item);
		}
	}

	private static void Dispose(IDisposable disposable)
	{
		if (disposable == null)
		{
			return;
		}
		try
		{
			disposable.Dispose();
		}
		catch (Exception ex)
		{
			if (IsSystemGeneratedException(ex))
			{
				throw ex;
			}
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "Exception occurs in cleanup: {0}.", ex);
		}
	}

	protected ISfcSimpleNode AdaptNode(SfcNodeAdapterProvider provider, object node)
	{
		TraceHelper.Trace("Differencing", "AdaptNode: obtaining adapter for node {0}.", node);
		try
		{
			ISfcSimpleNode graphAdapter = provider.GetGraphAdapter(node);
			TraceHelper.Trace("Differencing", "AdaptNode: obtained adapter for node {0}.", node);
			return graphAdapter;
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "AdaptNode: exception occurred {0}.", ex);
			throw new ArgumentException("node", ex);
		}
		catch (Exception ex2)
		{
			if (IsSystemGeneratedException(ex2))
			{
				throw ex2;
			}
			TraceHelper.LogExCatch(ex2);
			TraceHelper.Trace("Differencing", "AdaptNode: exception occurred {0}.", ex2);
			string message = StringDifferencing.FailedProviderLookup(provider.ToString(), node.ToString());
			throw new InvalidOperationException(message, ex2);
		}
	}

	protected IEnumerable<string> GetRelatedContainerNames(NodeItemNamesAdapterProvider provider, ISfcSimpleNode node)
	{
		TraceHelper.Trace("Differencing", "GetRelatedContainerNames: obtaining container (meta) names provider {0}.", node);
		try
		{
			IEnumerable<string> relatedContainerNames = provider.GetRelatedContainerNames(node);
			TraceHelper.Trace("Differencing", "GetRelatedContainerNames: obtained container (meta) names provider {0}.", node);
			return relatedContainerNames;
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "GetRelatedContainerNames: exception occurred {0}.", ex);
			throw new ArgumentException("node", ex);
		}
		catch (Exception ex2)
		{
			if (IsSystemGeneratedException(ex2))
			{
				throw ex2;
			}
			TraceHelper.LogExCatch(ex2);
			TraceHelper.Trace("Differencing", "GetRelatedContainerNames: exception occurred {0}.", ex2);
			string message = StringDifferencing.FailedProviderOperation(provider.ToString(), node.ToString());
			throw new InvalidOperationException(message, ex2);
		}
	}

	protected bool GetNaturalOrder(NodeItemNamesAdapterProvider provider, ISfcSimpleNode node, string name)
	{
		TraceHelper.Trace("Differencing", "GetNaturalOrder: determining if it is natural order {0}.", node);
		try
		{
			bool flag = provider.IsContainerInNatrualOrder(node, name);
			TraceHelper.Trace("Differencing", "GetNaturalOrder: determined {0}.", flag);
			return flag;
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "GetNaturalOrder: exception occurred {0}.", ex);
			throw new ArgumentException("node", ex);
		}
		catch (Exception ex2)
		{
			if (IsSystemGeneratedException(ex2))
			{
				throw ex2;
			}
			TraceHelper.LogExCatch(ex2);
			TraceHelper.Trace("Differencing", "GetNaturalOrder: exception occurred {0}.", ex2);
			string message = StringDifferencing.FailedProviderOperation(provider.ToString(), node.ToString());
			throw new InvalidOperationException(message, ex2);
		}
	}

	protected bool GetIsValueAvailable(AvailablePropertyValueProvider provider, ISfcSimpleNode node, string name)
	{
		TraceHelper.Trace("Differencing", "GetIsValueAvailable: determining if it property is available {0}.", node);
		try
		{
			bool flag = provider.IsValueAvailable(node, name);
			TraceHelper.Trace("Differencing", "GetIsValueAvailable: determined {0}.", flag);
			return flag;
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "GetIsValueAvailable: exception occurred {0}.", ex);
			throw new ArgumentException("node", ex);
		}
		catch (Exception ex2)
		{
			if (IsSystemGeneratedException(ex2))
			{
				throw ex2;
			}
			TraceHelper.LogExCatch(ex2);
			TraceHelper.Trace("Differencing", "GetIsValueAvailable: exception occurred {0}.", ex2);
			string message = StringDifferencing.FailedProviderOperation(provider.ToString(), node.ToString());
			throw new InvalidOperationException(message, ex2);
		}
	}

	protected void GetSortedLists(ContainerSortingProvider provider, ISfcSimpleList source, ISfcSimpleList target, out IEnumerable<ISfcSimpleNode> sortedSource, out IEnumerable<ISfcSimpleNode> sortedTarget)
	{
		TraceHelper.Trace("Differencing", "GetSortedLists: obtaining sorted lists {0} and {1}.", source, target);
		try
		{
			IEnumerable<ISfcSimpleNode> sortedSource2 = null;
			IEnumerable<ISfcSimpleNode> sortedTarget2 = null;
			provider.SortLists(source, target, out sortedSource2, out sortedTarget2);
			sortedSource = sortedSource2;
			sortedTarget = sortedTarget2;
			TraceHelper.Trace("Differencing", "GetSortedLists: obtained sorted lists {0}.", source);
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "GetSortedList: exception occurred {0}.", ex);
			throw new ArgumentException("list", ex);
		}
		catch (Exception ex2)
		{
			if (IsSystemGeneratedException(ex2))
			{
				throw ex2;
			}
			TraceHelper.LogExCatch(ex2);
			TraceHelper.Trace("Differencing", "GetSortedList: exception occurred {0}.", ex2);
			string message = StringDifferencing.FailedProviderOperation(provider.ToString(), source.ToString());
			throw new InvalidOperationException(message, ex2);
		}
	}

	protected IComparer<ISfcSimpleNode> GetComparer(ContainerSortingProvider provider, ISfcSimpleList list, ISfcSimpleList list2)
	{
		TraceHelper.Trace("Differencing", "GetComparer: obtaining comparer {0}.", list);
		try
		{
			IComparer<ISfcSimpleNode> comparer = provider.GetComparer(list, list2);
			TraceHelper.Trace("Differencing", "GetComparer: obtained comparer {0}.", list);
			return comparer;
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "GetComparer: exception occurred {0}.", ex);
			throw new ArgumentException("list", ex);
		}
		catch (Exception ex2)
		{
			if (IsSystemGeneratedException(ex2))
			{
				throw ex2;
			}
			TraceHelper.LogExCatch(ex2);
			TraceHelper.Trace("Differencing", "GetComparer: exception occurred {0}.", ex2);
			string message = StringDifferencing.FailedProviderOperation(provider.ToString(), list.ToString());
			throw new InvalidOperationException(message, ex2);
		}
	}

	protected IEnumerable<string> GetRelatedObjectNames(NodeItemNamesAdapterProvider provider, ISfcSimpleNode node)
	{
		TraceHelper.Trace("Differencing", "GetRelatedObjectNames: obtaining related object name for node {0}.", node);
		try
		{
			IEnumerable<string> relatedObjectNames = provider.GetRelatedObjectNames(node);
			TraceHelper.Trace("Differencing", "GetRelatedObjectNames: obtained related object name for node.");
			return relatedObjectNames;
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "GetRelatedObjectNames: exception occurred {0}.", ex);
			throw new ArgumentException("node", ex);
		}
		catch (Exception ex2)
		{
			if (IsSystemGeneratedException(ex2))
			{
				throw ex2;
			}
			TraceHelper.LogExCatch(ex2);
			TraceHelper.Trace("Differencing", "GetRelatedObjectNames: exception occurred {0}.", ex2);
			string message = StringDifferencing.FailedProviderOperation(provider.ToString(), node.ToString());
			throw new InvalidOperationException(message, ex2);
		}
	}

	protected IEnumerable<string> GetPropertyNames(NodeItemNamesAdapterProvider provider, ISfcSimpleNode node)
	{
		TraceHelper.Trace("Differencing", "GetPropertyNames: obtaining prop names for node {0}.", node);
		try
		{
			IEnumerable<string> propertyNames = provider.GetPropertyNames(node);
			TraceHelper.Trace("Differencing", "GetPropertyNames: obtained prop names for node.");
			return propertyNames;
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
			TraceHelper.Trace("Differencing", "GetPropertyNames: exception occurred {0}.", ex);
			throw new ArgumentException("node", ex);
		}
		catch (Exception ex2)
		{
			if (IsSystemGeneratedException(ex2))
			{
				throw ex2;
			}
			TraceHelper.LogExCatch(ex2);
			TraceHelper.Trace("Differencing", "GetPropertyNames: exception occurred {0}.", ex2);
			string message = StringDifferencing.FailedProviderOperation(provider.ToString(), node.ToString());
			throw new InvalidOperationException(message, ex2);
		}
	}

	protected SfcNodeAdapterProvider FindNodeAdapterProvider(object node)
	{
		TraceHelper.Trace("Differencing", "AdaptNode: finding adapter for node {0}.", node);
		foreach (SfcNodeAdapterProvider sfcNodeAdapterProvider in registry.SfcNodeAdapterProviders)
		{
			try
			{
				if (sfcNodeAdapterProvider.IsGraphSupported(node))
				{
					TraceHelper.Trace("Differencing", "AdaptNode: found adapter for node.");
					return sfcNodeAdapterProvider;
				}
			}
			catch (Exception ex)
			{
				if (IsSystemGeneratedException(ex))
				{
					throw ex;
				}
				TraceHelper.LogExCatch(ex);
				TraceHelper.Trace("Differencing", "AdaptNode: exception occurred {0}.", ex);
				string message = StringDifferencing.FailedProviderLookup(sfcNodeAdapterProvider.ToString(), node.ToString());
				throw new InvalidOperationException(message, ex);
			}
		}
		TraceHelper.Trace("Differencing", "AdaptNode: not found");
		return null;
	}

	protected NodeItemNamesAdapterProvider FindNameProvider(ISfcSimpleNode node)
	{
		TraceHelper.Trace("Differencing", "FindNameProvider: finding name provider for node {0}.", node);
		foreach (NodeItemNamesAdapterProvider nodeItemNameAdapterProvider in registry.NodeItemNameAdapterProviders)
		{
			try
			{
				if (nodeItemNameAdapterProvider.IsGraphSupported(node))
				{
					TraceHelper.Trace("Differencing", "FindNameProvider: found name provider for node.");
					return nodeItemNameAdapterProvider;
				}
			}
			catch (Exception ex)
			{
				if (IsSystemGeneratedException(ex))
				{
					throw ex;
				}
				TraceHelper.LogExCatch(ex);
				TraceHelper.Trace("Differencing", "FindNameProvider: exception occurred {0}.", ex);
				string message = StringDifferencing.FailedProviderLookup(nodeItemNameAdapterProvider.ToString(), node.ToString());
				throw new InvalidOperationException(message, ex);
			}
		}
		TraceHelper.Trace("Differencing", "FindNameProvider: not found");
		return null;
	}

	protected AvailablePropertyValueProvider FindAvailableValueProvider(ISfcSimpleNode node)
	{
		TraceHelper.Trace("Differencing", "FindAvailableValueProvider: finding provider for node {0}.", node);
		foreach (AvailablePropertyValueProvider availablePropertyValueProvider in registry.AvailablePropertyValueProviders)
		{
			try
			{
				if (availablePropertyValueProvider.IsGraphSupported(node))
				{
					TraceHelper.Trace("Differencing", "FindAvailableValueProvider: found provider for node.");
					return availablePropertyValueProvider;
				}
			}
			catch (Exception ex)
			{
				if (IsSystemGeneratedException(ex))
				{
					throw ex;
				}
				TraceHelper.LogExCatch(ex);
				TraceHelper.Trace("Differencing", "FindAvailableValueProvider: exception occurred {0}.", ex);
				string message = StringDifferencing.FailedProviderLookup(availablePropertyValueProvider.ToString(), node.ToString());
				throw new InvalidOperationException(message, ex);
			}
		}
		TraceHelper.Trace("Differencing", "FindAvailableValueProvider: not found");
		return null;
	}

	protected ContainerSortingProvider FindContainerSortingProvider(ISfcSimpleNode source, ISfcSimpleNode target)
	{
		TraceHelper.Trace("Differencing", "FindContainerSortingProvider: finding provider for node {0} and {1}.", source, target);
		foreach (ContainerSortingProvider containerSortingProvider in registry.ContainerSortingProviders)
		{
			try
			{
				if (containerSortingProvider.AreGraphsSupported(source, target))
				{
					TraceHelper.Trace("Differencing", "FindContainerSortingProvider: found provider for node {0} and {1}.", source, target);
					return containerSortingProvider;
				}
			}
			catch (Exception ex)
			{
				if (IsSystemGeneratedException(ex))
				{
					throw ex;
				}
				TraceHelper.LogExCatch(ex);
				TraceHelper.Trace("Differencing", "FindContainerSortingProvider: exception occurred {0}.", ex);
				string message = StringDifferencing.FailedProviderLookup(containerSortingProvider.ToString(), source.ToString());
				throw new InvalidOperationException(message, ex);
			}
		}
		TraceHelper.Trace("Differencing", "FindContainerSortingProvider: not found");
		return null;
	}

	protected PropertyComparerProvider FindPropertyComparerProvider(ISfcSimpleNode source, ISfcSimpleNode target)
	{
		TraceHelper.Trace("Differencing", "FindPropertyComparerProvider: finding provider for node {0} and {1}.", source, target);
		foreach (PropertyComparerProvider propertyComparerProvider in registry.PropertyComparerProviders)
		{
			try
			{
				if (propertyComparerProvider.AreGraphsSupported(source, target))
				{
					TraceHelper.Trace("Differencing", "FindPropertyComparerProvider: found provider for node {0} and {1}.", source, target);
					return propertyComparerProvider;
				}
			}
			catch (Exception ex)
			{
				if (IsSystemGeneratedException(ex))
				{
					throw ex;
				}
				TraceHelper.LogExCatch(ex);
				TraceHelper.Trace("Differencing", "FindPropertyComparerProvider: exception occurred {0}.", ex);
				string message = StringDifferencing.FailedProviderLookup(propertyComparerProvider.ToString(), source.ToString());
				throw new InvalidOperationException(message, ex);
			}
		}
		TraceHelper.Trace("Differencing", "FindPropertyComparerProvider: not found");
		return null;
	}

	internal static bool IsSystemGeneratedException(Exception e)
	{
		if (e is OutOfMemoryException)
		{
			return true;
		}
		if (e is StackOverflowException)
		{
			return true;
		}
		if (e is COMException || e is SEHException)
		{
			return true;
		}
		return false;
	}
}
