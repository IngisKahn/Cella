namespace Cella.Core.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Storage;
    using Storage.DataSpaces;
    using Storage.Files;
    using Xunit;

    public class DatabaseTests
    {
        public class MockFileMill : IFileMill
        {
            public IDatabaseFile Create(FileGroup fileGroup, FileOptions options) => options.Type switch
            {
                DatabaseFileType.FileStream => throw new NotImplementedException(),
                _ => new MockManagedFile(fileGroup, 0, options.Name, options.PhysicalName, options.Type)
            };

            public IManagedFile CreateManaged(FileGroup fileGroup, FileOptions options) => new MockManagedFile(fileGroup, 0, options.Name, options.PhysicalName, options.Type);

            public class MockDatabaseFile : IDatabaseFile
            {
                public DataSpace DataSpace { get; }
                public Guid Guid { get; }
                public ushort Id { get; }
                public bool IsMediaReadOnly { get; init; }
                public bool IsNameReserved { get; set; }
                public bool IsReadOnly { get; set; }
                public bool IsSparse { get; init; }
                public string Name { get; }
                public string PhysicalName { get; }
                public DatabaseFileState State { get; set; }
                public DatabaseFileType Type { get; }
                public decimal CreateLsn { get; set; }
                public decimal DropLsn { get; set; }
                public decimal ReadOnlyLsn { get; set; }
                public decimal ReadWriteLsn { get; set; }
                public decimal DifferentialBaseLsn { get; set; }


                public MockDatabaseFile(DataSpace fileGroup, ushort id, string name, string physicalName, DatabaseFileType type)
                    : this(fileGroup, id, name, physicalName, type, Guid.NewGuid()) { }
                public MockDatabaseFile(DataSpace fileGroup, ushort id, string name, string physicalName, DatabaseFileType type, Guid guid)
                {
                    this.DataSpace = fileGroup;
                    this.Id = id;
                    this.Name = name;
                    this.PhysicalName = physicalName;
                    this.Guid = guid;
                    this.Type = type;
                }

                public void Create()
                {
                }

                public void Validate()
                {
                }
            }

            public class MockManagedFile : MockDatabaseFile, IManagedFile
            {
                public MockManagedFile(DataSpace fileGroup, ushort id, string name, string physicalName, DatabaseFileType type)
                    : base(fileGroup, id, name, physicalName, type)
                {
                }

                public MockManagedFile(DataSpace fileGroup, ushort id, string name, string physicalName, DatabaseFileType type, Guid guid)
                    : base(fileGroup, id, name, physicalName, type, guid)
                {
                }

                public uint AutoGrowthAmount { get; init; }
                public AutoGrowthType AutoGrowthType { get; init; }
                public int InitialSize { get; init; }
                public int MaximumSize { get; init; }
                public int Size { get; set; }
                public ValueTask DisposeAsync() => throw new NotImplementedException();

                public void Grow()
                {
                    switch (this.AutoGrowthType)
                    {
                        case AutoGrowthType.ByPercent:
                            this.Size = (int)(this.Size * (this.AutoGrowthAmount / 100d));
                            break;
                        case AutoGrowthType.ByExtent:
                            this.Size += (int)this.AutoGrowthAmount;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        [Fact]
        public void CreationHasNameAndFiles()
        {
            MockFileMill fm = new();
            Database db = new(fm, new("OrderEntryDb")
            {
                PrimaryFiles = new[] { new FileOptions("OrderEntryDb", "m:\\OEDb.mdf") },
                FileGroups = new []
                {
                    new FileGroupOptions("Entities", new []{ new FileOptions("OrderEntry_Entities_F1", "n:\\OEEntities_F1.ndf") }),
                    new FileGroupOptions("Orders", new[]
                    {
                        new FileOptions("OrderEntry_Orders_F1", "o:\\OEOrders_F1.ndf"),
                        new FileOptions("OrderEntry_Orders_F2", "p:\\OEOrders_F2.ndf")
                    })
                },
                LogFiles = new [] { new FileOptions("OrderEntryDb_log", "l:\\OrderEntryDb_log.ldf") { Type = DatabaseFileType.Log } }
            }); 
            Assert.Equal("OrderEntryDb", db.Name);
            var pfg = db.PrimaryFileGroup;
            Assert.Equal("primary", pfg.Name);
            Assert.Single(pfg.DataFiles, f => f.Name == "OrderEntryDb" && f.PhysicalName == "m:\\OEDb.mdf");
            var sfg = db.FileGroups.Skip(1).First();
            Assert.Equal("Entities", sfg.Name);
            Assert.Single(sfg.DataFiles, f => f.Name == "OrderEntry_Entities_F1" && f.PhysicalName == "n:\\OEEntities_F1.ndf"); 
            sfg = db.FileGroups.Skip(2).First();
            Assert.Equal("Orders", sfg.Name);
            Assert.Single(sfg.DataFiles.Take(1), f => f.Name == "OrderEntry_Orders_F1" && f.PhysicalName == "o:\\OEOrders_F1.ndf");
            Assert.Single(sfg.DataFiles.Skip(1).Take(1), f => f.Name == "OrderEntry_Orders_F2" && f.PhysicalName == "p:\\OEOrders_F2.ndf");
            Assert.Equal("OrderEntryDb_log", db.LogFiles.DataFiles.Single().Name);
            Assert.Equal("l:\\OrderEntryDb_log.ldf", db.LogFiles.DataFiles.Single().PhysicalName);
        }
    }
}