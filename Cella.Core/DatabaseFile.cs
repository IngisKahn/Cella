namespace Cella.Core;

public enum AutoGrowthType
{
    ByPercent,
    ByMegabytes
}

public class DatabaseFile
{
    public string Name { get; }
    public string FileName { get; }
    public int InitialSize { get; set; }
    public AutoGrowthType AutoGrowthType { get; set; }
    public uint AutoGrowthAmount { get; set; }

    public DatabaseFile(string name, string fileName)
    {
        this.Name = name;
        this.FileName = fileName;
    }
}