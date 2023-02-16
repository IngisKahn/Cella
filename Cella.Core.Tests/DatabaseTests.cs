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
        public class MockFileMill : FileMill
        {
            public DatabaseFile Create(FileId fileId, FileOptions options) => options.Type switch
            {
                DatabaseFileType.FileStream => throw new NotImplementedException(),
                _ => new MockManagedFile(fileId, options.Name, options.PhysicalName, options.Type)
            };

            public ManagedFile CreateManaged(FileId fileId, FileOptions options) => new MockManagedFile(fileId, options.Name, options.PhysicalName, options.Type);
            public class MockManagedFile : ManagedFile
            {
                public MockManagedFile(FileId id, string name, string physicalName, DatabaseFileType type)
                    : base(id, name, physicalName, type)
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

        [Fact]
        public void PrimaryAndLogFilesDefault()
        {
            MockFileMill fm = new();
            Database db = new(fm, new("PrimaryAndLogFilesDefault"));
            Assert.Single(db.PrimaryFileGroup.DataFiles);
            Assert.Single(db.FileGroups);
            Assert.Single(db.LogFiles.DataFiles);
        }

        [Fact]
        public void FileSavesAndLoadsItsData()
        {
            FileMill fm = new();
            //FileGroup fg = new PrimaryFileGroup(null, fm);
            //ManagedFile file = new()
        }
    }
}