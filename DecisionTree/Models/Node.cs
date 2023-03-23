namespace DecisionTree.Models;

public class Node
{
    public List<Node> ChildrenNode { get; set; }
    
    public string[,] Object { get; set; }
    
    public List<string> ValueList { get; set; }
    
    public List<List<string>> ValueData { get; set; }
    
    public string NodeName { get; set; }
    
    public string ParentNode { get; set; }
    
    public int Depth { get; set; }
}