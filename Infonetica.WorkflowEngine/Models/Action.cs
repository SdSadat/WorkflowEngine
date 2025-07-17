namespace Infonetica.WorkflowEngine.Models;

// Id: Unique identifier for the action
// Name: Name of the action
// Description: Description of the action
// Enabled: Indicates if the action is enabled
// FromStates: List of states from which this action can be executed
// ToState: The state to which this action transitions


public record Action(
    string Id,
    string Name,
    string Description,
    bool Enabled,
    List<string> FromStates,
    string ToState
);