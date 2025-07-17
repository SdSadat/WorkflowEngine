using Infonetica.WorkflowEngine.Endpoints;
using Infonetica.WorkflowEngine.Services;
using Infonetica.WorkflowEngine.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<InMemoryDataStore>();
builder.Services.AddScoped<WorkflowService>();
builder.Services.AddScoped<WorkflowDefinitionValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapWorkflowEndpoints();

app.MapGet("/",()=> { return Results.Ok("Welcome to the Infonetica Workflow Engine API! Use /swagger for documentation."); });

app.Run();