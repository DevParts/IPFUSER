using System;

namespace NLog.Internal;

/// <summary>
/// Controls a single allocated object for reuse (only one active user)
/// </summary>
internal class ReusableObjectCreator<T> where T : class
{
	public struct LockOject : IDisposable
	{
		/// <summary>
		/// Access the acquired reusable object
		/// </summary>
		public readonly T Result;

		private readonly ReusableObjectCreator<T> _owner;

		public LockOject(ReusableObjectCreator<T> owner, T reusableObject)
		{
			Result = reusableObject;
			_owner = owner;
		}

		public void Dispose()
		{
			_owner?.Deallocate(Result);
		}
	}

	protected T? _reusableObject;

	private readonly Action<T> _clearObject;

	private readonly Func<T> _createObject;

	protected ReusableObjectCreator(Func<T> createObject, Action<T> clearObject)
	{
		_reusableObject = createObject();
		_clearObject = clearObject;
		_createObject = createObject;
	}

	/// <summary>
	/// Creates handle to the reusable char[]-buffer for active usage
	/// </summary>
	/// <returns>Handle to the reusable item, that can release it again</returns>
	public LockOject Allocate()
	{
		T reusableObject = _reusableObject ?? _createObject();
		_reusableObject = null;
		return new LockOject(this, reusableObject);
	}

	private void Deallocate(T reusableObject)
	{
		_clearObject(reusableObject);
		_reusableObject = reusableObject;
	}
}
