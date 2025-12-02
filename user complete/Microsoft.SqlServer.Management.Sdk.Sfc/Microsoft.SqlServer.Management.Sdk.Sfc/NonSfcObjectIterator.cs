using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class NonSfcObjectIterator : IEnumerable, IEnumerator
{
	private IAlienRoot _nonSfcRoot;

	private List<string> _nonSfcQueryResults;

	private int _curRow = -1;

	object IEnumerator.Current => CreateNonSfcObjectFromString(_nonSfcQueryResults[_curRow]);

	public NonSfcObjectIterator(IAlienRoot nonSfcRoot, SfcObjectQueryMode activeQueriesMode, SfcQueryExpression query, string[] fields, OrderBy[] orderByFields)
	{
		_nonSfcRoot = nonSfcRoot;
		_nonSfcQueryResults = _nonSfcRoot.SfcHelper_GetSmoObjectQuery(query.ToUrn().ToString(), fields, orderByFields);
	}

	private object CreateNonSfcObjectFromString(string str)
	{
		return _nonSfcRoot.SfcHelper_GetSmoObject(str);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this;
	}

	bool IEnumerator.MoveNext()
	{
		_curRow++;
		return _curRow < _nonSfcQueryResults.Count;
	}

	void IEnumerator.Reset()
	{
		_curRow = -1;
	}
}
