namespace Infonetica.WorkflowEngine.Models;

// basic unit for history of changes in a workflow instance
// Timestamp: when the change occurred
// Action: the action that was performed
// FromState: the state before the action was performed
// ToState: the state after the action was performed

public record Change(
    DateTime Timestamp,
    string Action,
    string FromState,
    string ToState
);