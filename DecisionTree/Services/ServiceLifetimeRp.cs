namespace DecisionTree.Services;

internal sealed class ServiceLifetimeRp
{
    private readonly ITreeService _treeService;
    
    public ServiceLifetimeRp(
        ITreeService treeService) =>
        (_treeService) =(treeService);
    public void ExecuteService()
    {
        _treeService.ExecuteTree();
    }
}