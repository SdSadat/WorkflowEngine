namespace Infonetica.WorkflowEngine.Models;

// Id: Unique identifier for the workflow definition
// Name: Name of the workflow definition
// Description: Description of the workflow definition
// States: List of states that the workflow can be in
// Actions: List of actions that can be performed in the workflow
public record WorkflowDefinition{
public string Id { get; init; } = string.Empty;
   public string Name { get; init; } = string.Empty;
   public string Description { get; init; } = string.Empty;
   public List<State> States { get; init; } = new List<State>();
   public List<Action> Actions { get; init; } = new List<Action>();
};