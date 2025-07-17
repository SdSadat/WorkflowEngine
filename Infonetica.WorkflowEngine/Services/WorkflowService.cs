using Infonetica.WorkflowEngine.Models;

namespace Infonetica.WorkflowEngine.Services;


public class WorkflowService
{
    // initialize readonly field for datastore
    private readonly InMemoryDataStore _dataStore;

    public WorkflowService(InMemoryDataStore dataStore)
    {
        // contruct the value of datastore
        _dataStore = dataStore;
    }

    // utility function to create a workflow definition
    public (WorkflowDefinition? definition, string? error) CreateDefinition(WorkflowDefinition definition)
    {
        // null check

        if(definition.States == null || definition.Actions == null)
        {
            return (null, "Workflow definition must have states and actions defined.");
        }

        if (string.IsNullOrWhiteSpace(definition.Id))
        {
            return (null, "Workflow definition ID cannot be null or empty.");
        }
       // check if definition id already exists in the in-memory data store
        if (_dataStore.WorkflowDefinitions.ContainsKey(definition.Id))
        {
            return (null, $"Workflow definition with ID '{definition.Id}' already exists.");
        }


        // check if only one initial state
        if (definition.States.Count(s => s.IsInitial) != 1)
        {
            return (null, "A workflow definition must have exactly one initial state.");
        }

        // check for uniqueness of each state id :)
        bool ContainsDuplicateStateIds = false;
        var StateIdsMap = new Dictionary<string, int>();
        var DuplicateStateIds = new List<string>();
        foreach (var state in definition.States)
        {
            if (StateIdsMap.ContainsKey(state.Id))
            {
                StateIdsMap[state.Id]++;
                ContainsDuplicateStateIds = true;
                if (!DuplicateStateIds.Contains(state.Id))
                {
                    DuplicateStateIds.Add(state.Id);
                }
            }
            else
            {
                StateIdsMap[state.Id] = 1;
            }
        }
        if (ContainsDuplicateStateIds)
        {
            return (null, "State IDs within a definition must be unique. Duplicate IDs: " + "[" + string.Join(", ", DuplicateStateIds) + "]");
        }

        bool ContainsDuplicateActionIds = false;
        var ActionIdsMap = new Dictionary<string, int>();
        var DuplicateActionIds = new List<string>();
        foreach (var Action in definition.Actions)
        {
            if (ActionIdsMap.ContainsKey(Action.Id))
            {
                ActionIdsMap[Action.Id]++;
                ContainsDuplicateActionIds = true;
                if (!DuplicateActionIds.Contains(Action.Id))
                {
                    DuplicateActionIds.Add(Action.Id);
                }
            }
            else
            {
                ActionIdsMap[Action.Id] = 1;
            }
        }
        if (ContainsDuplicateActionIds)
        {
            return (null, "Action IDs within a definition must be unique. Duplicate IDs: " + "[" +string.Join(", ", DuplicateActionIds) + "]");
        }

        // validation of definition done, now add to db
        _dataStore.WorkflowDefinitions[definition.Id] = definition;
        return (definition, null);
    }


    // utility function to start instance of workflow
    public (WorkflowInstance? instance, string? error) StartInstance(string definitionId)
    {
        // check in memory data store if definition id exists
        if (!_dataStore.WorkflowDefinitions.TryGetValue(definitionId, out var definition))
        {
            return (null, $"Workflow definition with ID '{definitionId}' not found.");
        }

        // check for initial state in the defintion-- (shouldn't be the case tho)
        var initialState = definition.States.SingleOrDefault(s => s.IsInitial);
        if (initialState == null)
        {
            return (null, "Cannot start instance: Definition is invalid and lacks an initial state.");
        }

        // create a new instance and add it to Db
        var instance = new WorkflowInstance(definitionId, initialState.Id);
        _dataStore.WorkflowInstances[instance.Id] = instance;

        return (instance, null);
    }

    // utility function to execute action on a workflow instance
    public (WorkflowInstance? instance, string? error) ExecuteAction(Guid instanceId, string actionId)
    {   
        if (Guid.Empty == instanceId)
        {
            return (null, "Instance ID cannot be empty.");
        }
        
        // null check for action id
        if (string.IsNullOrWhiteSpace(actionId))
        {
            return (null, "Action ID cannot be null or empty.");
        }

        // check if instance exists in db
        if (!_dataStore.WorkflowInstances.TryGetValue(instanceId, out var instance))
        {
            return (null, "Workflow instance not found.");
        }

        // check if the definition is present for this instance in db
        if (!_dataStore.WorkflowDefinitions.TryGetValue(instance.DefinitionId, out var definition))
        {
            return (null, "Underlying workflow definition for this instance not found.");
        }

        // validate if instance is in final state
        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState != null && currentState.IsFinal)
        {
            return (null, "Cannot execute action: The workflow instance is in a final state.");
        }

        // check if action id exists in the workflow definition
        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);


        // validating action-------------

        // check if action id exists in the workflow definition
        if (action == null)
        {
            return (null, $"Action '{actionId}' is not part of the instance's definition.");
        }

        // check if action is enabled (precedence over state check :) 
        if (!action.Enabled)
        {
            return (null, $"Action '{actionId}' is disabled.");
        }

        // achecking if current state is present in the action's fromstates

        if (!action.FromStates.Contains(instance.CurrentState))
        {
            return (null, $"Action '{actionId}' cannot be executed from the current state '{instance.CurrentState}'.");
        }

        // check if actions's to state is present in the workflow definition states
        if (!definition.States.Any(s => s.Id == action.ToState))
        {
            return (null, $"Action '{actionId}' transitions to an unknown state '{action.ToState}'.");
        }

        // action is now validated , now add the execution to the history and update the new state:

        // add change to history
        instance.History.Add(new Change(
            Timestamp: DateTime.UtcNow,
            Action: actionId,
            FromState: instance.CurrentState,
            ToState: action.ToState
        ));

        // update the current state
        instance.CurrentState = action.ToState;


        return (instance, null);
    }
}