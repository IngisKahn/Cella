namespace Cella.Core;

public readonly record struct FullPageId(DatabaseId DatabaseId, FileId FileId, PageId PageId);