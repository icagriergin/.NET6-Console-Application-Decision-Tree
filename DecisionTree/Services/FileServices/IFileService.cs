using Microsoft.Extensions.DependencyInjection;

namespace DecisionTree.Services.FileServices;

public interface IFileService : ILifetimeService
{ 
    ServiceLifetime ILifetimeService.Lifetime => ServiceLifetime.Scoped;
    string[,] ReadFile();

    int[,] FindFileRowAndColumnCount();
}