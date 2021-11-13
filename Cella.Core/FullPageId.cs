namespace Cella.Core;

public record struct FullPageId(byte DatabaseId, ushort FileId, uint PageId);