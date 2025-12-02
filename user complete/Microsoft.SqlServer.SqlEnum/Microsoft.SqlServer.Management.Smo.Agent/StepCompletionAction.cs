namespace Microsoft.SqlServer.Management.Smo.Agent;

public enum StepCompletionAction
{
	QuitWithSuccess = 1,
	QuitWithFailure,
	GoToNextStep,
	GoToStep
}
