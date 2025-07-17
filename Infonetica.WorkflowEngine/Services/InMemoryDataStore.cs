using System.Collections.Concurrent;
using Infonetica.WorkflowEngine.Models;

namespace Infonetica.WorkflowEngine.Services;

// data store class for in memoery storage and acess of worrkflow definition and instances
// it uses concurrent dictionary to allow thread-safe operations (prolly not needed in this case but good practice ig)

public class InMemoryDataStore
{
    public ConcurrentDictionary<string, WorkflowDefinition> WorkflowDefinitions { get; } = new();
    public ConcurrentDictionary<Guid, WorkflowInstance> WorkflowInstances { get; } = new();
}