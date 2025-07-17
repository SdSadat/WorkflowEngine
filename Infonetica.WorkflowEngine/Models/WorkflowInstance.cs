namespace Infonetica.WorkflowEngine.Models;

// Id: unique guid for the workflow instance
// DefinitionId: identifier for the workflow definition this instance belongs to
// CurrentState: current state of the workflow instance
// History: list of changes made to the workflow instance
public class WorkflowInstance
{
    public Guid Id { get; }
    public string DefinitionId { get; }
    public string CurrentState { get; set; }
    public List<Change> History { get; }

    public WorkflowInstance(string definitionId, string initialState)
    {
        Id = Guid.NewGuid();
        DefinitionId = definitionId;
        CurrentState = initialState;
        History = new List<Change>();
    }
}