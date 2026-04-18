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

            var (type, coords) = TriangleCalculator.CalculateTriangle(s1, s2, s3);

            TxtResult.Text = $"Тип: {type}\nКоординаты: A{coords[0]}, B{coords[1]}, C{coords[2]}";
            DrawTriangle(coords);
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
