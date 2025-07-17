namespace Infonetica.WorkflowEngine.Models;

public record State(
    string Id, 
    string Name,
    string Description,
    bool IsInitial,
    bool IsFinal,
    bool Enabled
);