using System.Collections.Specialized;
using System.Threading;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class ExecuteNonQueryThread
{
	private ExecutionManager em;

	private StringCollection queries;

	private ServerMessageEventHandler dbccMessageHandler;

	private bool errorsAsMessages;

	private bool retry;

	public ExecuteNonQueryThread(ExecutionManager em, StringCollection queries, ServerMessageEventHandler dbccMessageHandler, bool errorsAsMessages)
		: this(em, queries, dbccMessageHandler, errorsAsMessages, retry: true)
	{
	}

	public ExecuteNonQueryThread(ExecutionManager em, StringCollection queries, ServerMessageEventHandler dbccMessageHandler, bool errorsAsMessages, bool retry)
	{
		this.em = em;
		this.queries = queries;
		this.dbccMessageHandler = dbccMessageHandler;
		this.errorsAsMessages = errorsAsMessages;
		this.retry = retry;
	}

	public void Start()
	{
		if (SqlContext.IsAvailable)
		{
			throw new SmoException(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
		}
		Thread thread = new Thread(ThreadProc);
		thread.Start();
	}

	private void ThreadProc()
	{
		ExecuteNonQueryCompletedEventArgs args = null;
		try
		{
			if (dbccMessageHandler != null)
			{
				em.ExecuteNonQueryWithMessage(queries, dbccMessageHandler, errorsAsMessages, retry);
			}
			else
			{
				em.ExecuteNonQuery(queries, retry);
			}
			args = new ExecuteNonQueryCompletedEventArgs(ExecutionStatus.Succeeded, null);
		}
		catch (SmoException lastException)
		{
			args = new ExecuteNonQueryCompletedEventArgs(ExecutionStatus.Failed, lastException);
		}
		catch (ExecutionFailureException lastException2)
		{
			args = new ExecuteNonQueryCompletedEventArgs(ExecutionStatus.Failed, lastException2);
		}
		finally
		{
			em.OnExecuteNonQueryCompleted(args);
			if (em.AsyncWaitHandle != null)
			{
				em.AsyncWaitHandle.Set();
			}
		}
	}
}
