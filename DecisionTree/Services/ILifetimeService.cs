using Microsoft.Extensions.DependencyInjection;

namespace DecisionTree.Services;

public interface ILifetimeService
{
    Guid Id { get; }

    ServiceLifetime Lifetime { get; }
}