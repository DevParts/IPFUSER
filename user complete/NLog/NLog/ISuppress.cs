using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace NLog;

/// <summary>
/// Obsolete and replaced by <see cref="T:NLog.ILogger" /> with NLog v5.3.
///
/// Provides an interface to execute System.Actions without surfacing any exceptions raised for that action.
/// </summary>
[Obsolete("ISuppress should be replaced with ILogger. Marked obsolete with NLog v5.3")]
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ISuppress
{
	/// <summary>
	/// Runs the provided action. If the action throws, the exception is logged at <c>Error</c> level. The exception is not propagated outside of this method.
	/// </summary>
	/// <param name="action">Action to execute.</param>
	void Swallow(Action action);

	/// <summary>
	/// Runs the provided function and returns its result. If an exception is thrown, it is logged at <c>Error</c> level.
	/// The exception is not propagated outside of this method; a default value is returned instead.
	/// </summary>
	/// <typeparam name="T">Return type of the provided function.</typeparam>
	/// <param name="func">Function to run.</param>
	/// <returns>Result returned by the provided function or the default value of type <typeparamref name="T" /> in case of exception.</returns>
	T? Swallow<T>(Func<T?> func);

	/// <summary>
	/// Runs the provided function and returns its result. If an exception is thrown, it is logged at <c>Error</c> level.
	/// The exception is not propagated outside of this method; a fallback value is returned instead.
	/// </summary>
	/// <typeparam name="T">Return type of the provided function.</typeparam>
	/// <param name="func">Function to run.</param>
	/// <param name="fallback">Fallback value to return in case of exception.</param>
	/// <returns>Result returned by the provided function or fallback value in case of exception.</returns>
	T? Swallow<T>(Func<T?> func, T? fallback);

	/// <summary>
	/// Logs an exception is logged at <c>Error</c> level if the provided task does not run to completion.
	/// </summary>
	/// <param name="task">The task for which to log an error if it does not run to completion.</param>
	/// <remarks>This method is useful in fire-and-forget situations, where application logic does not depend on completion of task. This method is avoids C# warning CS4014 in such situations.</remarks>
	void Swallow(Task task);

	/// <summary>
	/// Returns a task that completes when a specified task to completes. If the task does not run to completion, an exception is logged at <c>Error</c> level. The returned task always runs to completion.
	/// </summary>
	/// <param name="task">The task for which to log an error if it does not run to completion.</param>
	/// <returns>A task that completes in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state when <paramref name="task" /> completes.</returns>
	Task SwallowAsync(Task task);

	/// <summary>
	/// Runs async action. If the action throws, the exception is logged at <c>Error</c> level. The exception is not propagated outside of this method.
	/// </summary>
	/// <param name="asyncAction">Async action to execute.</param>
	/// <returns>A task that completes in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state when <paramref name="asyncAction" /> completes.</returns>
	Task SwallowAsync(Func<Task> asyncAction);

	/// <summary>
	/// Runs the provided async function and returns its result. If the task does not run to completion, an exception is logged at <c>Error</c> level.
	/// The exception is not propagated outside of this method; a default value is returned instead.
	/// </summary>
	/// <typeparam name="TResult">Return type of the provided function.</typeparam>
	/// <param name="asyncFunc">Async function to run.</param>
	/// <returns>A task that represents the completion of the supplied task. If the supplied task ends in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state, the result of the new task will be the result of the supplied task; otherwise, the result of the new task will be the default value of type <typeparamref name="TResult" />.</returns>
	Task<TResult?> SwallowAsync<TResult>(Func<Task<TResult?>> asyncFunc);

	/// <summary>
	/// Runs the provided async function and returns its result. If the task does not run to completion, an exception is logged at <c>Error</c> level.
	/// The exception is not propagated outside of this method; a fallback value is returned instead.
	/// </summary>
	/// <typeparam name="TResult">Return type of the provided function.</typeparam>
	/// <param name="asyncFunc">Async function to run.</param>
	/// <param name="fallback">Fallback value to return if the task does not end in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state.</param>
	/// <returns>A task that represents the completion of the supplied task. If the supplied task ends in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state, the result of the new task will be the result of the supplied task; otherwise, the result of the new task will be the fallback value.</returns>
	Task<TResult?> SwallowAsync<TResult>(Func<Task<TResult?>> asyncFunc, TResult? fallback);
}
