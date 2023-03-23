using DecisionTree.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DecisionTree.Services;

public interface ITreeService : ILifetimeService
{
    ServiceLifetime ILifetimeService.Lifetime => ServiceLifetime.Scoped;
    public void GenerateTreesFirstNode(string[][,] attributes);
    void WriteTree(Node root);
    Node CreateTreeNodes(Node node);
    double CalculateEntropy(string[,] attributes, string[,] data);
    string[,] CalculateMaxGain(string[,] attributesGain, double totalGain);
    void ExecuteTree();
}