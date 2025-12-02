using System;
using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class ObjectInSchemaEventsWorker : EventsWorkerBase
{
	private ScriptSchemaObjectBase target;

	protected override SqlSmoObject Target => target;

	protected virtual string ObjectType => target.GetType().Name;

	internal ObjectInSchemaEventsWorker(ScriptSchemaObjectBase target, Type eventSetType, Type eventEnumType)
		: base(target, eventSetType, eventEnumType)
	{
		this.target = target;
	}

	protected override EventQuery CreateWqlQuery(string eventClass)
	{
		return EventsWorkerBase.CreateWqlQueryForSourceObject(eventClass, target.ParentColl.ParentInstance.InternalName, target.Schema, target.Name, ObjectType);
	}
}
