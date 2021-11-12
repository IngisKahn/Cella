namespace Cella.Core;

public class DatabaseFile
{
    public string Name { get; set; }
    public string FileName { get; set; }

    public DatabaseFile(string name, string fileName)
    {
        this.Name = name;
        this.FileName = fileName;
    }
}