using DecisionTree.Extensions;
using DecisionTree.Models;
using DecisionTree.Services.FileServices;
using Microsoft.Extensions.DependencyInjection;

namespace DecisionTree.Services.TreeServices;

public class TreeService : ITreeService
{
    private readonly IFileService _fileService;
    private int[,] rowAndColumnCount;
    private string[,] datas;
    private string[,] attributesGain;
    private double generalEntropy;
    private string[][,] generalAttributes;
    private Node tree;
    private string rootNodeName;
    private List<string> attributes;
    private string[,] outlookAttributes; 
    private string[,] temperatureAttributes;
    private string[,] humidityAttributes;
    private string[,] windyAttributes;
    private string[,] playAttributes;
    private string depth = "";
    private int checkDepth = 0;
    private int totalDepth = 0;
    
    public TreeService(IFileService fileService)
    {
        _fileService = fileService;
        rowAndColumnCount = fileService.FindFileRowAndColumnCount();
        datas = fileService.ReadFile();
        attributesGain = new string[4, 4];
        tree = new Node();
        outlookAttributes =  new string[3, 2] { { "sunny", "outlook" }, { "overcast", "outlook" }, { "rainy", "outlook" } };
        temperatureAttributes = new string[3,2] { { "hot", "temperature" }, { "mild", "temperature" }, { "cool", "temperature" } };
        humidityAttributes = new string[2,2] {{ "high", "humidity" }, { "normal", "humidity" } };
        windyAttributes = new string[2,2] { { "TRUE", "windy" }, { "FALSE", "windy" } };
        playAttributes = new string[2,2] {{ "yes", "play" }, { "no", "play" } };
        attributes = new List<string>() {"outlook","temperature","humidity","windy","play"};
        generalAttributes = new[]
        {
            outlookAttributes,
            temperatureAttributes,
            humidityAttributes,
            windyAttributes
        };
        generalEntropy = CalculateEntropy(null, null);
        
    }

    public void ExecuteTree()
    {
        GenerateTreesFirstNode(generalAttributes);
    }
    
    public void GenerateTreesFirstNode(string[][,] attributes)
    {
        if (attributes.IsNullOrNotAny())
            return;

        for (int i = 0; i < attributes.Length; i++)
        {
            double entropy = CalculateEntropy(attributes[i], datas);
            attributesGain[i, 0] = attributes[i][0, 1];
            attributesGain[i, 1] = entropy.ToString();
        }

        var nodeGain = CalculateMaxGain(attributesGain, generalEntropy);

        var newNode = new Node()
        {
            Object = nodeGain,
            NodeName = nodeGain[0, 0],
            ValueData = new List<List<string>>(),
            ValueList = new List<string>()
        };

        var valueList = new List<string>();
        
        for(int i=0;i<attributes.GetLength(0);i++)
        {
            for(int j=0;j<attributes[i].GetLength(0);j++)
            {
                if(attributes[i][j,1] == newNode.NodeName)
                {
                    valueList.Add(attributes[i][j, 0]);
                }
            }
        }

        newNode.ValueList = valueList;
        
        for(int i=0;i<newNode.ValueList.Count;i++)
        {
            var dataList = new List<string>();
            for (int k = 0; k < datas.GetLength(0); k++)
            {
                for(int t=0;t<datas.GetLength(1);t++)
                {
                    if (newNode.ValueList[i] == datas[k, t])
                    {
                        dataList.Add(datas[k, 0]);
                        dataList.Add(datas[k, 1]);
                        dataList.Add(datas[k, 2]);
                        dataList.Add(datas[k, 3]);
                        dataList.Add(datas[k, 4]);
                    }
                }
                   
            }
            newNode.ValueData.Add(dataList);
        }

        tree = newNode;
        tree = CreateTreeNodes(newNode);
        rootNodeName = tree.NodeName;
        WriteTree(tree);

        Console.ReadLine();
    }

    public void WriteTree(Node root)
    {
        if(root.ValueList == null)
        {
            depth += "  ";
            Console.WriteLine("Leaf :" + root.NodeName);
        }
        else
        {
            depth += "  ";
            Console.WriteLine("Node :" + root.NodeName);
        }
          
        if (root.ValueList != null)
        {
            for (int i = 0; i < root.ChildrenNode.Count; i++)
            {
                Console.WriteLine(depth+"Branch: " + root.ValueList[i]);
                if (root.ChildrenNode != null)
                {
                    depth += "  ";
                    checkDepth++;
                    totalDepth++;
                    Console.Write(depth);
                        
                    WriteTree(root.ChildrenNode[i]);
                    if (depth.Length > 4)
                    {
                        depth = depth.Remove(depth.Length - 4);
                    }
                       
                    if(i == root.ChildrenNode.Count-1 && root.ParentNode == rootNodeName)
                    {
                        depth = "  ";
                    }
                }
            }
        }
    }
    
    public Node CreateTreeNodes(Node node)
    {
        node.ChildrenNode = new List<Node>();
        var newAttributeList = new List<string>();
        
        for(int i=0;i<attributes.Count;i++)
        {
            if(node.NodeName != attributes[i])
            {
                newAttributeList.Add(attributes[i]);
            }
        }
        
        var newAttributesTable = new string[newAttributeList.Count-1][,];
        for(int i=0;i<newAttributeList.Count-1;i++)
        {
            for(int k=0;k<generalAttributes.GetLength(0);k++)
            {
                for(int l=0;l<generalAttributes[k].GetLength(0);l++)
                {
                    if (newAttributeList[i] == generalAttributes[k][l, 1])
                    {
                        newAttributesTable[i] = generalAttributes[k];
                    }
                }
            }
        }
        attributes = new List<string>();
        attributes = newAttributeList;
        attributesGain = null;
        
        for (int j = 0; j < node.ValueList.Count; j++)
        {
            double entropy;
            string lastNode = null;
            var newDataSet = new string[node.ValueData[j].Count / 5, 5];
            int ctAttrLength = 0;
            attributesGain = new string[newAttributesTable.Length, 4];
            
            if (newAttributesTable.Length >0)
            {
                ctAttrLength = newAttributesTable.Length;
            }
            else
            {
                ctAttrLength = 1;
            }
            
            for (int i = 0; i < ctAttrLength; i++)
            {
                newDataSet = new string[node.ValueData[j].Count / 5, 5];
                int ct = 0;
                for (int m = 0; m < node.ValueData[j].Count; m++)
                {
                    for (int l = 0; l < 5; l++)
                    {
                        if (ct < node.ValueData[j].Count)
                        {
                            newDataSet[m, l] = node.ValueData[j][ct];
                            ct++;
                        }
                    }
                }
                
                int checkYes = 0;
                int checkNo = 0;
               
                for(int ctEnt=0;ctEnt<newDataSet.GetLength(0);ctEnt++)
                {
                    if(newDataSet[ctEnt,4] == "no")
                    {
                        checkNo++;
                    }
                    if (newDataSet[ctEnt, 4] == "yes")
                    {
                        checkYes++;
                    }
                }
                
                if(checkNo != 0 && checkYes == 0)
                {
                    lastNode = "no";
                }
                else if(checkNo==0 && checkYes != 0)
                {
                    lastNode = "yes";
                }
                else
                {
                    if(newDataSet.GetLength(0)>0)
                    {
                        entropy = CalculateEntropy(newAttributesTable[i], newDataSet);
                        attributesGain[i, 0] = newAttributesTable[i][0, 1];
                        attributesGain[i, 1] = entropy.ToString();
                    }
                }
            }
            
            if(newDataSet.GetLength(0)> 0)
            {
                if (lastNode == null)
                {
                    var nodeGain = CalculateMaxGain(attributesGain, generalEntropy);
                    var newNode = new Node()
                    {
                        Object = nodeGain,
                        NodeName = nodeGain[0, 0],
                        ValueData = new List<List<string>>(),
                        ValueList = new List<string>(),
                        ParentNode = node.NodeName
                    };
                    
                    var valueList = new List<string>();
                    for (int h = 0; h < newAttributesTable.GetLength(0); h++)
                    {
                        for (int g = 0; g < newAttributesTable[h].GetLength(0); g++)
                        {
                            if (newAttributesTable[h][g, 1] == newNode.NodeName)
                            {
                                valueList.Add(newAttributesTable[h][g, 0]);
                            }
                        }
                    }

                    newNode.ValueList = valueList;
                    
                    for (int h = 0; h < newNode.ValueList.Count; h++)
                    {
                        var dataList = new List<string>();
                        for (int g = 0; g < newDataSet.GetLength(0); g++)
                        {
                            for (int f = 0; f < newDataSet.GetLength(1); f++)
                            {
                                if (newNode.ValueList[h] == newDataSet[g, f])
                                {
                                    dataList.Add(newDataSet[g, 0]);
                                    dataList.Add(newDataSet[g, 1]);
                                    dataList.Add(newDataSet[g, 2]);
                                    dataList.Add(newDataSet[g, 3]);
                                    dataList.Add(newDataSet[g, 4]);
                                }
                            }
                        }
                        newNode.ValueData.Add(dataList);
                    }
                    
                    node.ChildrenNode.Add(newNode);
                    CreateTreeNodes(newNode);
                }
                else
                {
                    var newNode = new Node()
                    {
                        NodeName = lastNode,
                        Object = new string[1, 2] { { lastNode, "0" } },
                        ParentNode = node.NodeName
                    };
                    node.ChildrenNode.Add(newNode);
                }
            }
        } 
        return node;
    }
    
    public double CalculateEntropy(string[,] attributes, string[,] data)
    {
        if (attributes == null)
        {
            double yesAttribute = 0, noAttribute = 0, entropy = 0;

            for (int i = 0; i < rowAndColumnCount[0,0]; i++)
            {
                for (int j = 4; j < rowAndColumnCount[0, 1]; j++)
                {
                    if (datas[i, j] == "yes")
                    {
                        yesAttribute++;
                    }
                    else
                    {
                        noAttribute++;
                    }
                }
            }
            
            double totalAttributes = yesAttribute + noAttribute;
            entropy = -((yesAttribute / totalAttributes) * Math.Log((yesAttribute / totalAttributes), 2.0)) 
                      - ((noAttribute / totalAttributes) * Math.Log((noAttribute / totalAttributes), 2.0));
            return entropy;
        }

        int attributeIndex = 0;
        var check = string.Empty;
        string[,,] attributesTable = new string[attributes.GetLength(0), 2, 1];

        for (int i = 0; i < attributes.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(0); j++)
            {
                for (int k = 0; k < data.GetLength(1); k++)
                {
                    if (data[j, k] == attributes[i,0])
                    {
                        attributeIndex = k;
                        check = "checked";
                        break;
                    }
                }
                if(check == "checked")
                {
                    break;
                }
            }
            
            if (check == "checked")
            {
                break;
            }
        }
        
        for (int i = 0; i < attributes.GetLength(0); i++)
        {
            double yesAttributes = 0, noAttributes = 0;
            
            for (int j = 0; j < data.GetLength(0); j++)
            {
                for (int k = 4; k < data.GetLength(1); k+=4)
                {
                    if (attributes[i, 0] == data[j, attributeIndex])
                    {
                        if (data[j, k] == "yes")
                        {
                            yesAttributes++;
                        }
                        else
                        {
                            noAttributes++;
                        }
                    }
                }
            }

            attributesTable[i, 0, 0] = yesAttributes.ToString();
            attributesTable[i, 1, 0] = noAttributes.ToString();
        }

        double entrp = 0;
        for (int i = 0; i < attributesTable.GetLength(0); i++)
        {
            double ent = 0, total = 0;

            for (int j = 0; j < attributesTable.GetLength(1); j++)
            {
                total += Convert.ToDouble(attributesTable[i, j, 0]);
            }
            
            if (Convert.ToDouble(attributesTable[i, 0, 0]) == 0 || Convert.ToDouble(attributesTable[i, 1, 0]) == 0)
            {
                ent= 0;
            }
            else
            {
                ent = -((Convert.ToDouble(attributesTable[i, 0, 0]) / total) * Math.Log((Convert.ToDouble(attributesTable[i, 0, 0]) / total), 2.0)) 
                          - ((Convert.ToDouble(attributesTable[i, 1, 0]) / total) * Math.Log((Convert.ToDouble(attributesTable[i, 1, 0]) / total), 2.0));
            }
            
            entrp += (total / datas.Length / 5) * ent;
        }

        return entrp;
    }
    
    public string[,] CalculateMaxGain(string[,] attributesGain, double totalGain)
    {
        int control = 0;
        double max = 0;
        var node = new string[1, 2];

        for (int i = 0; i < attributesGain.GetLength(0);i++)
        {
            if (Convert.ToDouble(attributesGain[i, 1]) == 0)
            {
                control++;
            }
        }

        if (control == attributesGain.GetLength(0))
        {
            node[0, 0] = attributesGain[0, 0];
            node[0, 1] = attributesGain[0, 1];
            return node;
        }

        for (int i = 0; i < attributesGain.GetLength(0); i++)
        {
            double calculatedGain = totalGain - Convert.ToDouble(attributesGain[i,1]);
            if (calculatedGain > max)
            {
                max = calculatedGain;
                node[0, 0] = attributesGain[i, 0];
                node[0, 1] = attributesGain[i, 1];
            }
        }

        return node;
    }

    public Guid Id { get; } = Guid.NewGuid();
}