using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1
{
    public class TriangleRecord
    {
        public string SideA { get; set; }
        public string SideB { get; set; }
        public string SideC { get; set; }
        public string TriangleType { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IDatabase
    {
        void AddRecord(TriangleRecord record);
        TriangleRecord GetRecord(string a, string b, string c);
        void DeleteRecord(string a, string b, string c);
    }

    public class InMemoryDatabase : IDatabase
    {
        private readonly List<TriangleRecord> _records = new List<TriangleRecord>();

        public void AddRecord(TriangleRecord record) => _records.Add(record);

        public TriangleRecord GetRecord(string a, string b, string c) => 
            _records.FirstOrDefault(r => r.SideA == a && r.SideB == b && r.SideC == c);

        public void DeleteRecord(string a, string b, string c) => 
            _records.RemoveAll(r => r.SideA == a && r.SideB == b && r.SideC == c);
    }

    public interface IUserInterface
    {
        (string, string, string) GetUserInput();
    }

    public class WpfUserInterface : IUserInterface
    {
        private readonly string _a, _b, _c;
        public WpfUserInterface(string a, string b, string c) { _a = a; _b = b; _c = c; }
        public (string, string, string) GetUserInput() => (_a, _b, _c);
    }

    public interface IExternalService
    {
        void SendData(string data);
    }

    public class EmailService : IExternalService
    {
        public void SendData(string data)
        {
            Console.WriteLine($"Отправлено на email: {data}"); 
        }
    }

    public class TriangleController
    {
        private readonly IUserInterface _ui;
        private readonly IDatabase _db;
        private readonly IExternalService _externalService;

        public TriangleController(IUserInterface ui, IDatabase db, IExternalService externalService)
        {
            _ui = ui;
            _db = db;
            _externalService = externalService;
        }

        public string ProcessData()
        {
            var (a, b, c) = _ui.GetUserInput();
            var record = _db.GetRecord(a, b, c);
            string resultMessage;

            if (record != null)
            {
                resultMessage = string.IsNullOrEmpty(record.ErrorMessage) ? record.TriangleType : record.ErrorMessage;
            }
            else
            {
                var (type, _) = TriangleCalculator.CalculateTriangle(a, b, c);
                string error = (type == "" || type == "не треугольник") ? "Ошибка входных данных или не треугольник" : "";
                
                record = new TriangleRecord
                {
                    SideA = a, SideB = b, SideC = c,
                    TriangleType = type,
                    ErrorMessage = error
                };
                _db.AddRecord(record);

                resultMessage = string.IsNullOrEmpty(error) ? type : error;
            }

            _externalService.SendData($"Результат для треугольника ({a}, {b}, {c}): {resultMessage}");
            return resultMessage;
        }
    }
}
