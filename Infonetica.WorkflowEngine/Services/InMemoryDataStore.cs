using System.Collections.Concurrent;
using Infonetica.WorkflowEngine.Models;

namespace Infonetica.WorkflowEngine.Services;

public class InMemoryDataStore
{
    public ConcurrentDictionary<string, WorkflowDefinition> WorkflowDefinitions { get; } = new();
    public ConcurrentDictionary<Guid, WorkflowInstance> WorkflowInstances { get; } = new();
}