namespace Infonetica.WorkflowEngine.Models;
public record Action(
    string Id,
    string Name,
    string Description,
    bool Enabled,
    List<string> FromStates,
    string ToState
);