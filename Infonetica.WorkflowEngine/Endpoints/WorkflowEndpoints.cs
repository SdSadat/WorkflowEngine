using Infonetica.WorkflowEngine.Models;
using Infonetica.WorkflowEngine.Services;
using Microsoft.AspNetCore.Mvc;

namespace Infonetica.WorkflowEngine.Endpoints;

public static class WorkflowEndpoints
{
    public static void MapWorkflowEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api");

        group.MapPost("/definitions", ([FromBody] WorkflowDefinition definition, WorkflowService service) =>
        {
            var (newDefinition, error) = service.CreateDefinition(definition);
            return error != null ? Results.BadRequest(new { message = error }) : Results.Created($"/api/definitions/{newDefinition!.Id}", newDefinition);
        }).WithTags("Workflow Configuration");

        group.MapGet("/definitions/{id}", (string id, InMemoryDataStore store) =>
        {
            return store.WorkflowDefinitions.TryGetValue(id, out var definition)
                ? Results.Ok(definition)
                : Results.NotFound();
        }).WithTags("Workflow Configuration");
        
        group.MapGet("/definitions", (InMemoryDataStore store) =>
        {
            return Results.Ok(store.WorkflowDefinitions.Values);
        }).WithTags("Workflow Configuration");


        group.MapPost("/instances", ([FromBody] StartInstanceRequest request, WorkflowService service) =>
        {
            var (newInstance, error) = service.StartInstance(request.DefinitionId);
            return error != null ? Results.NotFound(new { message = error }) : Results.Ok(newInstance);
        }).WithTags("Runtime");

        group.MapGet("/instances/{id}", (Guid id, InMemoryDataStore store) =>
        {
            return store.WorkflowInstances.TryGetValue(id, out var instance)
                ? Results.Ok(new { instance.Id, instance.CurrentState, instance.History })
                : Results.NotFound();
        }).WithTags("Runtime");
        
        group.MapGet("/instances", (InMemoryDataStore store) =>
        {
            return Results.Ok(store.WorkflowInstances.Values.Select(i => new { i.Id, i.DefinitionId, i.CurrentState }));
        }).WithTags("Runtime");

        group.MapPost("/instances/{id}/execute", (Guid id, [FromBody] ExecuteActionRequest request, WorkflowService service) =>
        {
            var (updatedInstance, error) = service.ExecuteAction(id, request.ActionId);
            return error != null ? Results.BadRequest(new { message = error }) : Results.Ok(updatedInstance);
        }).WithTags("Runtime");
    }

    public record StartInstanceRequest(string DefinitionId);
    public record ExecuteActionRequest(string ActionId);
}