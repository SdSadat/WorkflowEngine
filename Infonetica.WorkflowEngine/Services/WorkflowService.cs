using Infonetica.WorkflowEngine.Models;

namespace Infonetica.WorkflowEngine.Services;


public class WorkflowService
{
    private readonly InMemoryDataStore _dataStore;

    public WorkflowService(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }


    public (WorkflowDefinition? definition, string? error) CreateDefinition(WorkflowDefinition definition)
    {
       
                if (_dataStore.WorkflowDefinitions.ContainsKey(definition.Id))
                {
                    return (null, $"Workflow definition with ID '{definition.Id}' already exists.");
                }

        if (definition.States.Count(s => s.IsInitial) != 1)
        {
            return (null, "A workflow definition must have exactly one initial state.");
        }

        var stateIds = new HashSet<string>(definition.States.Select(s => s.Id));
        if (stateIds.Count != definition.States.Count)
        {
            return (null, "State IDs within a definition must be unique.");
        }

        _dataStore.WorkflowDefinitions[definition.Id] = definition;
        return (definition, null);
    }


    public (WorkflowInstance? instance, string? error) StartInstance(string definitionId)
    {
        if (!_dataStore.WorkflowDefinitions.TryGetValue(definitionId, out var definition))
        {
            return (null, $"Workflow definition with ID '{definitionId}' not found.");
        }

        var initialState = definition.States.SingleOrDefault(s => s.IsInitial);
        if (initialState == null)
        {
            return (null, "Cannot start instance: Definition is invalid and lacks an initial state.");
        }

        var instance = new WorkflowInstance(definitionId, initialState.Id);
        _dataStore.WorkflowInstances[instance.Id] = instance;
        return (instance, null);
    }

    public (WorkflowInstance? instance, string? error) ExecuteAction(Guid instanceId, string actionId)
    {
        if (!_dataStore.WorkflowInstances.TryGetValue(instanceId, out var instance))
        {
            return (null, "Workflow instance not found.");
        }

        if (!_dataStore.WorkflowDefinitions.TryGetValue(instance.DefinitionId, out var definition))
        {
            return (null, "Underlying workflow definition for this instance not found.");
        }

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState != null && currentState.IsFinal)
        {
            return (null, "Cannot execute action: The workflow instance is in a final state.");
        }

        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        
        if (action == null)
        {
            return (null, $"Action '{actionId}' is not part of the instance's definition.");
        }

        if (!action.Enabled)
        {
            return (null, $"Action '{actionId}' is disabled.");
        }

        if (!action.FromStates.Contains(instance.CurrentState))
        {
            return (null, $"Action '{actionId}' cannot be executed from the current state '{instance.CurrentState}'.");
        }

        if (!definition.States.Any(s => s.Id == action.ToState))
        {
            return (null, $"Action '{actionId}' transitions to an unknown state '{action.ToState}'.");
        }
        
        instance.CurrentState = action.ToState;
        instance.History.Add((actionId, DateTime.UtcNow));

        return (instance, null);
    }
}