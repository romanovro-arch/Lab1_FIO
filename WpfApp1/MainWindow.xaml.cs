using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Serilog;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ConfigureLogger();
            Log.Information("Приложение запущено");
        }

        private void ConfigureLogger()
        {
            string template = "{Timestamp:HH:mm:ss} | [{Level:u3}] | {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: template)
                .WriteTo.File("logs/file_.txt", outputTemplate: template)
                .CreateLogger();
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            string s1 = TxtSideA.Text;
            string s2 = TxtSideB.Text;
            string s3 = TxtSideC.Text;

            var (type, coords) = CalculateTriangle(s1, s2, s3);

            TxtResult.Text = $"Тип: {type}\nКоординаты: A{coords[0]}, B{coords[1]}, C{coords[2]}";
            DrawTriangle(coords);
        }

        public (string, List<(int, int)>) CalculateTriangle(string s1, string s2, string s3)
        {
            try
            {
                if (!float.TryParse(s1, out float a) || !float.TryParse(s2, out float b) || !float.TryParse(s3, out float c) || a <= 0 || b <= 0 || c <= 0)
                {
                    LogUnsuccessfulRequest(s1, s2, s3, "Некорректные (нечисловые или отрицательные) входные данные", null);
                    return ("", new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) });
                }

                if (a + b <= c || a + c <= b || b + c <= a)
                {
                    LogUnsuccessfulRequest(s1, s2, s3, "не треугольник", null);
                    return ("не треугольник", new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) });
                }

                string type = "разносторонний";
                if (a == b && b == c) type = "равносторонний";
                else if (a == b || b == c || a == c) type = "равнобедренный";

                float x1 = 0, y1 = 0;
                float x2 = a, y2 = 0;
                float x3 = (a * a + b * b - c * c) / (2 * a);
                float y3 = (float)Math.Sqrt(Math.Max(0, b * b - x3 * x3));

                float minX = Math.Min(x1, Math.Min(x2, x3));
                float maxX = Math.Max(x1, Math.Max(x2, x3));
                float minY = Math.Min(y1, Math.Min(y2, y3));
                float maxY = Math.Max(y1, Math.Max(y2, y3));

                float width = maxX - minX;
                float height = maxY - minY;
                float scale = 100f / Math.Max(width, height);

                int finalX1 = (int)Math.Round((x1 - minX) * scale);
                int finalY1 = (int)Math.Round((y1 - minY) * scale);
                int finalX2 = (int)Math.Round((x2 - minX) * scale);
                int finalY2 = (int)Math.Round((y2 - minY) * scale);
                int finalX3 = (int)Math.Round((x3 - minX) * scale);
                int finalY3 = (int)Math.Round((y3 - minY) * scale);

                var coords = new List<(int, int)> { (finalX1, finalY1), (finalX2, finalY2), (finalX3, finalY3) };

                Log.Information("Успешный запрос | Параметры: {A}, {B}, {C} | Результат: {Type}, Координаты: {Coords}", s1, s2, s3, type, coords);

                return (type, coords);
            }
            catch (Exception ex)
            {
                LogUnsuccessfulRequest(s1, s2, s3, "Ошибка выполнения программы", ex);
                return ("", new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) });
            }
        }

        private void LogUnsuccessfulRequest(string s1, string s2, string s3, string reason, Exception ex)
        {
            if (ex != null)
            {
                Log.Error(ex, "Неуспешный запрос | Параметры: {A}, {B}, {C} | Причина: {Reason}", s1, s2, s3, reason);
            }
            else
            {
                Log.Warning("Неуспешный запрос | Параметры: {A}, {B}, {C} | Причина: {Reason}", s1, s2, s3, reason);
            }
        }

        private void DrawTriangle(List<(int, int)> coords)
        {
            DrawingCanvas.Children.Clear();

            if (coords[0].Item1 < 0) return;

            Polygon myPolygon = new Polygon
            {
                Stroke = Brushes.Black,
                Fill = Brushes.LightBlue,
                StrokeThickness = 2
            };

            myPolygon.Points.Add(new Point(coords[0].Item1, 100 - coords[0].Item2));
            myPolygon.Points.Add(new Point(coords[1].Item1, 100 - coords[1].Item2));
            myPolygon.Points.Add(new Point(coords[2].Item1, 100 - coords[2].Item2));

            DrawingCanvas.Children.Add(myPolygon);
        }

        protected override void OnClosed(EventArgs e)
        {
            Log.CloseAndFlush();
            base.OnClosed(e);
        }
    }
}