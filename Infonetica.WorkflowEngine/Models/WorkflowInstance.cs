namespace Infonetica.WorkflowEngine.Models;

/// <summary>
/// Represents a running instance of a workflow definition.
/// </summary>
public class WorkflowInstance
{
    public Guid Id { get; }
    public string DefinitionId { get; }
    public string CurrentState { get; set; }
    public List<(string Action, DateTime Timestamp)> History { get; }

    public WorkflowInstance(string definitionId, string initialState)
    {
        Id = Guid.NewGuid();
        DefinitionId = definitionId;
        CurrentState = initialState;
        History = new List<(string, DateTime)>();
    }
}