namespace Infonetica.WorkflowEngine.Models;

public record State(
    string Id,
    bool IsInitial,
    bool IsFinal,
    bool Enabled
);