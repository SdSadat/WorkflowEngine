using System.ComponentModel.DataAnnotations;
using Infonetica.WorkflowEngine.Models;
using Infonetica.WorkflowEngine.Validators;

namespace Infonetica.WorkflowEngine.Services;


public class WorkflowService
{
    // initialize readonly field for datastore
    private readonly InMemoryDataStore _dataStore;
    private readonly WorkflowDefinitionValidator _validator;
    public WorkflowService(InMemoryDataStore dataStore, WorkflowDefinitionValidator validator)
    {
        // contruct the value of datastore and validator
        _dataStore = dataStore;
        _validator = validator;
    }

    // utility function to create a workflow definition
    public (WorkflowDefinition? definition, string? error) CreateDefinition(WorkflowDefinition definition)
    {

        // validate the definition using the validator
        var validationError = _validator.Validate(definition);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            return (null, validationError);
        }
       // check if definition id already exists in the in-memory data store
        if (_dataStore.WorkflowDefinitions.ContainsKey(definition.Id))
        {
            return (null, $"Workflow definition with ID '{definition.Id}' already exists.");
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