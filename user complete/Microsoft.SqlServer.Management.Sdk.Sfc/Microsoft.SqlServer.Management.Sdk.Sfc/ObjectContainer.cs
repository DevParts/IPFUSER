using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class ObjectContainer
{
	private object sfcInstance;

	private Dictionary<string, Dictionary<string, ObjectContainer>> collections;

	private Dictionary<string, ObjectContainer> children;

	private string uri;

	public Dictionary<string, Dictionary<string, ObjectContainer>> Collections => collections;

	public Dictionary<string, ObjectContainer> Children => children;

	public string Uri
	{
		get
		{
			return uri;
		}
		set
		{
			uri = value;
		}
	}

	public object SfcInstance => sfcInstance;

	public ObjectContainer(object sfcInstance, string uri)
	{
		collections = new Dictionary<string, Dictionary<string, ObjectContainer>>();
		children = new Dictionary<string, ObjectContainer>();
		this.sfcInstance = sfcInstance;
		Uri = uri;
	}
}
