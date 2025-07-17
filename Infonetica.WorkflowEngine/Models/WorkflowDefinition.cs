namespace Infonetica.WorkflowEngine.Models;

// Id: Unique identifier for the workflow definition
// Name: Name of the workflow definition
// Description: Description of the workflow definition
// States: List of states that the workflow can be in
// Actions: List of actions that can be performed in the workflow
public record WorkflowDefinition(
    string Id,
    string Name,
    string Description,
    List<State> States,
    List<Action> Actions
);