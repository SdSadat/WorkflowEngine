namespace Infonetica.WorkflowEngine.Models;
public record Action(
    string Id,
    bool Enabled,
    List<string> FromStates,
    string ToState
);