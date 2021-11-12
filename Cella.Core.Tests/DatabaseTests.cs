using System.Linq;
using Xunit;

namespace Cella.Core.Tests
{
    public class DatabaseTests
    {
        [Fact]
        public void CreationHasNameAndFiles()
        {
            var db = new Database(
                "OrderEntryDb",
                new(new("OrderEntryDb", "m:\\OEDb.mdf")),
                new FileGroup[]
                {
                    new("Entities", new DatabaseFile[] {new("OrderEntry_Entities_F1", "n:\\OEEntities_F1.ndf")}),
                    new("Orders",
                        new DatabaseFile[]
                        {
                            new("OrderEntry_Orders_F1", "o:\\OEOrders_F1.ndf"),
                            new("OrderEntry_Orders_F2", "p:\\OEOrders_F2.ndf")
                        })
                }, new DatabaseFile[] {new("OrderEntryDb_log", "l:\\OrderEntryDb_log.ldf")});
            Assert.Equal("OrderEntryDb", db.Name);
            var pfg = db.PrimaryFileGroup;
            Assert.Equal("primary", pfg.Name);
            Assert.Single(pfg.DataFiles, f => f.Name == "OrderEntryDb" && f.FileName == "m:\\OEDb.mdf");
            var sfg = db.FileGroups.Skip(1).First();
            Assert.Equal("Entities", sfg.Name);
            Assert.Single(sfg.DataFiles, f => f.Name == "OrderEntry_Entities_F1" && f.FileName == "n:\\OEEntities_F1.ndf"); 
            sfg = db.FileGroups.Skip(2).First();
            Assert.Equal("Orders", sfg.Name);
            Assert.Single(sfg.DataFiles.Take(1), f => f.Name == "OrderEntry_Orders_F1" && f.FileName == "o:\\OEOrders_F1.ndf");
            Assert.Single(sfg.DataFiles.Skip(1).Take(1), f => f.Name == "OrderEntry_Orders_F2" && f.FileName == "p:\\OEOrders_F2.ndf");
            Assert.Equal("OrderEntryDb_log", db.LogFiles.Single().Name);
            Assert.Equal("l:\\OrderEntryDb_log.ldf", db.LogFiles.Single().FileName);
        }
    }
}