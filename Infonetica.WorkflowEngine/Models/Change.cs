namespace Infonetica.WorkflowEngine.Models;

public record Change(
    DateTime Timestamp,
    string Action,
    string FromState,
    string ToState
);