namespace Cella.Core.Tests
{
    using System.Linq;
    using Xunit;

    public class DatabaseTests
    {
        [Fact]
        public void CreationHasNameAndFiles()
        {
            //var db = new Database(
            //    "OrderEntryDb",
            //    new(new(null, 0, "OrderEntryDb", "m:\\OEDb.mdf", DatabaseFileType.Pages)),
            //    new FileGroup[]
            //    {
            //        new("Entities", new DatabaseFile[] {new(null, 0, "OrderEntry_Entities_F1", "n:\\OEEntities_F1.ndf", DatabaseFileType.Pages) }),
            //        new("Orders",
            //            new DatabaseFile[]
            //            {
            //                new(null, 0, "OrderEntry_Orders_F1", "o:\\OEOrders_F1.ndf", DatabaseFileType.Pages),
            //                new(null, 0, "OrderEntry_Orders_F2", "p:\\OEOrders_F2.ndf", DatabaseFileType.Pages)
            //            })
            //    }, new DatabaseFile[] {new(null, 0, "OrderEntryDb_log", "l:\\OrderEntryDb_log.ldf", DatabaseFileType.Log) });
            //Assert.Equal("OrderEntryDb", db.Name);
            //var pfg = db.PrimaryFileGroup;
            //Assert.Equal("primary", pfg.Name);
            //Assert.Single(pfg.DataFiles, f => f.Name == "OrderEntryDb" && f.PhysicalName == "m:\\OEDb.mdf");
            //var sfg = db.FileGroups.Skip(1).First();
            //Assert.Equal("Entities", sfg.Name);
            //Assert.Single(sfg.DataFiles, f => f.Name == "OrderEntry_Entities_F1" && f.PhysicalName == "n:\\OEEntities_F1.ndf"); 
            //sfg = db.FileGroups.Skip(2).First();
            //Assert.Equal("Orders", sfg.Name);
            //Assert.Single(sfg.DataFiles.Take(1), f => f.Name == "OrderEntry_Orders_F1" && f.PhysicalName == "o:\\OEOrders_F1.ndf");
            //Assert.Single(sfg.DataFiles.Skip(1).Take(1), f => f.Name == "OrderEntry_Orders_F2" && f.PhysicalName == "p:\\OEOrders_F2.ndf");
            //Assert.Equal("OrderEntryDb_log", db.LogFiles.Single().Name);
            //Assert.Equal("l:\\OrderEntryDb_log.ldf", db.LogFiles.Single().PhysicalName);
        }
    }
}