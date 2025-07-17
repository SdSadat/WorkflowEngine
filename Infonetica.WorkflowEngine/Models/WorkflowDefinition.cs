namespace Infonetica.WorkflowEngine.Models;

/// <summary>
/// Defines the structure of a workflow, including its states and actions.
/// </summary>
/// <param name="Id">The unique identifier for the workflow definition.</param>
/// <param name="States">The collection of states in the workflow.</param>
/// <param name="Actions">The collection of actions (transitions) in the workflow.</param>
public record WorkflowDefinition(
    string Id,
    string Name,
    string Description,
    List<State> States,
    List<Action> Actions
);