namespace DecisionTree.Services.FileServices;

public class FileService : IFileService
{
    private string[,] datas; 

    public FileService()
    {
        datas = new string[FindFileRowAndColumnCount()[0,0], FindFileRowAndColumnCount()[0,1]];
    }
    
    public int[,] FindFileRowAndColumnCount()
    {
        var reader = new StreamReader("test.txt");
        var content = reader.ReadToEnd();
        var rows = content.Split('\n');
        var columns = rows[0].Split(',');
        int[,] indexes = new int[1, 2];
        indexes[0,0] = rows.Length;
        indexes[0, 1] = columns.Length;
        return indexes;

    }
    
    public string[,] ReadFile()
    {
        var reader = new StreamReader("test.txt");
        string content = reader.ReadToEnd();
        var rows = content.Split('\n');
        for (int i = 0; i <rows.Length; i++)
        { 
            var values = rows[i].Split(',');
            for (int j = 0; j < FindFileRowAndColumnCount()[0, 1]; j++)
            {
                datas[i, j] = values[j];
            }
        }

        return datas;
    }
    
    public Guid Id { get; } = Guid.NewGuid();
}