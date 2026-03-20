namespace NetCord.Services.Commands;

public readonly record struct CommandExecutionResult(IExecutionResult ExecutionResult, bool ContinueNextOverload);
