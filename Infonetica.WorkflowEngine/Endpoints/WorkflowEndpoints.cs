using Infonetica.WorkflowEngine.Models;
using Infonetica.WorkflowEngine.Services;
using Infonetica.WorkflowEngine.Validators;
using Microsoft.AspNetCore.Mvc;


// Endpoint for workflow-related operations

namespace Infonetica.WorkflowEngine.Endpoints;

public static class WorkflowEndpoints
{
    public static void MapWorkflowEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api");

        // Endpoint to create a new workflow definition
        // accepts and validates a WorkflowDefinition object, returns created definition if valid
        group.MapPost("/definitions", ([FromBody] WorkflowDefinition definition, WorkflowService service) =>
        {
            var (newDefinition, err) = service.CreateDefinition(definition);
            return err != null ? Results.BadRequest(new { message = err, error = true  }) : Results.Created($"/api/definitions/{newDefinition!.Id}", newDefinition);
        }).WithTags("Workflow Configuration");


        // Endpoint to fetch a woorkflow definition by Id, take definition Id as a parameter
        group.MapGet("/definitions/{id}", (string id, InMemoryDataStore store) =>
        {
            return store.WorkflowDefinitions.TryGetValue(id, out var definition)
                ? Results.Ok(definition)
                : Results.NotFound( new { message = "Workflow Definition with given ID not found.", error = true});
        }).WithTags("Workflow Configuration");


        // Endpoint to fetch all workflow definitions
        group.MapGet("/definitions", (InMemoryDataStore store) =>
        {
            return Results.Ok(store.WorkflowDefinitions.Values);
        }).WithTags("Workflow Configuration");

        // Endpoint to start an instance from a workflow definition Id
        group.MapPost("/instances", ([FromBody] StartInstanceRequest request, WorkflowService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.DefinitionId))
            {
                return Results.BadRequest(new { message = "Definition ID cannot be empty, Please refer schema requirements", error = true });
            }
            var (newInstance, err) = service.StartInstance(request.DefinitionId);
            return err != null ? Results.NotFound(new { message = err, error = true }) : Results.Ok(newInstance);
        }).WithTags("Runtime");

        // Endpoint to fetch a workflow instance by Id
        group.MapGet("/instances/{id}", (Guid id, InMemoryDataStore store) =>
        {
            return store.WorkflowInstances.TryGetValue(id, out var instance)
                ? Results.Ok(new { instance.Id, instance.CurrentState, instance.History })
                : Results.NotFound(new { message = "Workflow Instance with given ID not found.", error = true });
        }).WithTags("Runtime");

        group.MapGet("/instances", (InMemoryDataStore store) =>
        {
            return Results.Ok(store.WorkflowInstances.Values.Select(i => new { i.Id, i.DefinitionId, i.CurrentState, i.History }));
        }).WithTags("Runtime");

        group.MapPost("/instances/{id}/execute", (Guid id, [FromBody] ExecuteActionRequest request, WorkflowService service) =>
        {
            if(string.IsNullOrWhiteSpace(request.ActionId))
            {
                return Results.BadRequest(new { message = "Action ID cannot be empty, Please refer schema requirements", error = true });
            }
            var (updatedInstance, err) = service.ExecuteAction(id, request.ActionId);
            return err != null ? Results.BadRequest(new { message = err, error = true }) : Results.Ok(updatedInstance);
        }).WithTags("Runtime");
    }

    public record StartInstanceRequest
    {
        public string DefinitionId { get; set; } = string.Empty;
    };
    public record ExecuteActionRequest
    {
        public string ActionId { get; set; } = string.Empty;
    };
}