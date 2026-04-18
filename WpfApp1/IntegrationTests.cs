using Xunit;
using WpfApp1;

namespace WpfApp1.Tests
{
    public class StubUserInterface : IUserInterface
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }

        public (string, string, string) GetUserInput() => (A, B, C);
    }

    public class MockExternalService : IExternalService
    {
        public string SentData { get; private set; }

        public void SendData(string data)
        {
            SentData = data;
        }
    }

    public class IntegrationTests
    {
        [Fact]
        public void ProcessData_NotInDatabase_CalculatesAndSavesToDbAndSendsEmail()
        {
            var db = new InMemoryDatabase();
            var ui = new StubUserInterface { A = "3", B = "4", C = "5" };
            var emailMock = new MockExternalService();
            var controller = new TriangleController(ui, db, emailMock);

            string result = controller.ProcessData();

            Assert.Equal("разносторонний", result);
            var recordInDb = db.GetRecord("3", "4", "5");
            Assert.NotNull(recordInDb);
            Assert.Equal("разносторонний", recordInDb.TriangleType);
            Assert.Contains("разносторонний", emailMock.SentData);
        }

        [Fact]
        public void ProcessData_AlreadyInDatabase_ReturnsFromDbAndSendsEmail()
        {
            var db = new InMemoryDatabase();
            db.AddRecord(new TriangleRecord { SideA = "5", SideB = "5", SideC = "5", TriangleType = "взят из БД", ErrorMessage = "" });
            
            var ui = new StubUserInterface { A = "5", B = "5", C = "5" };
            var emailMock = new MockExternalService();
            var controller = new TriangleController(ui, db, emailMock);

            string result = controller.ProcessData();

            Assert.Equal("взят из БД", result);
            Assert.Contains("взят из БД", emailMock.SentData);
        }

        [Fact]
        public void ProcessData_InvalidInput_SavesErrorToDbAndSendsEmail()
        {
            var db = new InMemoryDatabase();
            var ui = new StubUserInterface { A = "-1", B = "2", C = "3" };
            var emailMock = new MockExternalService();
            var controller = new TriangleController(ui, db, emailMock);

            string result = controller.ProcessData();

            Assert.Equal("Ошибка входных данных или не треугольник", result);
            var recordInDb = db.GetRecord("-1", "2", "3");
            Assert.NotNull(recordInDb);
            Assert.Equal("Ошибка входных данных или не треугольник", recordInDb.ErrorMessage);
            Assert.Contains("Ошибка", emailMock.SentData);
        }

        [Fact]
        public void Database_AddAndGetRecord_ReturnsCorrectRecord()
        {
            var db = new InMemoryDatabase();
            var record = new TriangleRecord { SideA = "10", SideB = "10", SideC = "10", TriangleType = "равносторонний" };

            db.AddRecord(record);
            var fetchedRecord = db.GetRecord("10", "10", "10");

            Assert.NotNull(fetchedRecord);
            Assert.Equal("равносторонний", fetchedRecord.TriangleType);
        }

        [Fact]
        public void Database_DeleteRecord_RemovesRecordSuccessfully()
        {
            var db = new InMemoryDatabase();
            db.AddRecord(new TriangleRecord { SideA = "7", SideB = "7", SideC = "7", TriangleType = "равносторонний" });

            db.DeleteRecord("7", "7", "7");
            var fetchedRecord = db.GetRecord("7", "7", "7");

            Assert.Null(fetchedRecord);
        }
    }
}
