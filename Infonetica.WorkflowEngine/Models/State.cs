namespace Infonetica.WorkflowEngine.Models;

// Id: Unique identifier for the state
// Name: Name of the state
// Description: Description of the state
// IsInitial: Indicates if this state is the initial state of the workflow
// IsFinal: Indicates if this state is the final state of the workflow

public record State(
    string Id,
    string Name,
    string Description,
    bool IsInitial,
    bool IsFinal,
    bool Enabled
);