using Infonetica.WorkflowEngine.Models;
using System.Text;

namespace Infonetica.WorkflowEngine.Validators;


public class WorkflowDefinitionValidator
{
    public string Validate(WorkflowDefinition definition)
    {
        var errors = new StringBuilder();

        if (string.IsNullOrWhiteSpace(definition.Id))
        {
            errors.AppendLine("Workflow definition ID cannot be empty.");
        }

        if (definition.States is null || definition.Actions is null)
        {
            errors.AppendLine("Definition must contain 'states' and 'actions' arrays.");
            return errors.ToString();
        }
        
        ValidateStates(definition, errors);
        ValidateActions(definition, errors);

        return errors.ToString();
    }

    private void ValidateStates(WorkflowDefinition definition, StringBuilder errors)
    {
        if (!definition.States.Any())
        {
            errors.AppendLine("Workflow must have at least one state.");
            return; 
        }

       
        if (definition.States.Count(s => s.IsInitial) != 1)
        {
            errors.AppendLine("Workflow must have exactly one initial state.");
        }

        var stateIds = new HashSet<string>();
        foreach (var state in definition.States)
        {
            if (string.IsNullOrWhiteSpace(state.Id))
            {
                errors.AppendLine("State ID cannot be empty.");
            }
            else if (!stateIds.Add(state.Id))
            {
                errors.AppendLine($"Duplicate state ID found: '{state.Id}'.");
            }
        }
    }

    private void ValidateActions(WorkflowDefinition definition, StringBuilder errors)
    {
        if(!definition.Actions.Any())
        {
            errors.AppendLine("Workflow must have at least one action.");
            return; 
        }
        var stateIds = new HashSet<string>(definition.States.Select(s => s.Id));
        var actionIds = new HashSet<string>();

        foreach (var action in definition.Actions)
        {
            if (string.IsNullOrWhiteSpace(action.Id))
            {
                errors.AppendLine("Action ID cannot be empty.");
            }
            else if (!actionIds.Add(action.Id))
            {
                errors.AppendLine($"Duplicate action ID found: '{action.Id}'.");
            }

            if (!stateIds.Contains(action.ToState))
            {
                errors.AppendLine($"Action '{action.Id}' refers to an unknown toState: '{action.ToState}'.");
            }

            if (action.FromStates is null || !action.FromStates.Any())
            {
                errors.AppendLine($"Action '{action.Id}' must have at least one fromState.");
            }
            else
            {
                foreach (var fromState in action.FromStates)
                {
                    if (!stateIds.Contains(fromState))
                    {
                        errors.AppendLine($"Action '{action.Id}' refers to an unknown fromState: '{fromState}'.");
                    }
                }
            }
        }
    }
}