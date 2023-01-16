namespace Cella.Storage.Files;

public enum PageSize : byte
{
    Kb1,
    Kb2,
    Kb4,
    Kb8,
    Kb16,
    Kb32,
    Kb64,
    Default = PageSize.Kb8
}