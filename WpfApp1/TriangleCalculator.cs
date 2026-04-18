using System;
using System.Collections.Generic;
using System.Globalization;
using Serilog;

namespace WpfApp1
{
    public class TriangleCalculator
    {
        public static (string, List<(int, int)>) CalculateTriangle(string s1, string s2, string s3)
        {
            try
            {
                bool isAValid = float.TryParse(s1, NumberStyles.Any, CultureInfo.InvariantCulture, out float a);
                bool isBValid = float.TryParse(s2, NumberStyles.Any, CultureInfo.InvariantCulture, out float b);
                bool isCValid = float.TryParse(s3, NumberStyles.Any, CultureInfo.InvariantCulture, out float c);

                if (!isAValid || !isBValid || !isCValid || a <= 0 || b <= 0 || c <= 0)
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

                float epsilon = 0.0001f;
                bool abEqual = Math.Abs(a - b) < epsilon;
                bool bcEqual = Math.Abs(b - c) < epsilon;
                bool acEqual = Math.Abs(a - c) < epsilon;

                if (abEqual && bcEqual) type = "равносторонний";
                else if (abEqual || bcEqual || acEqual) type = "равнобедренный";

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

                Log.Logger?.Information("Успешный запрос | Параметры: {A}, {B}, {C} | Результат: {Type}", s1, s2, s3, type);

                return (type, coords);
            }
            catch (Exception ex)
            {
                LogUnsuccessfulRequest(s1, s2, s3, "Ошибка выполнения программы", ex);
                return ("", new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) });
            }
        }

        private static void LogUnsuccessfulRequest(string s1, string s2, string s3, string reason, Exception ex)
        {
            if (Log.Logger == null) return;

            if (ex != null) Log.Error(ex, "Неуспешный запрос | Причина: {Reason}", reason);
            else Log.Warning("Неуспешный запрос | Причина: {Reason}", reason);
        }
    }
}
