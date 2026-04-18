using System.Collections.Generic;
using Xunit;
using WpfApp1;

namespace WpfApp1.Tests
{
    public class TriangleTests
    {
        [Theory]
        [InlineData("5", "5", "5")]
        [InlineData("10.5", "10.5", "10.5")]
        [InlineData("1000", "1000", "1000")]
        public void CalculateTriangle_Equilateral_ReturnsCorrectType(string a, string b, string c)
        {
            var (type, _) = TriangleCalculator.CalculateTriangle(a, b, c);
            Assert.Equal("равносторонний", type);
        }

        [Theory]
        [InlineData("5", "5", "3")]
        [InlineData("12.5", "12.5", "10")]
        public void CalculateTriangle_IsoscelesAB_ReturnsCorrectType(string a, string b, string c)
        {
            var (type, _) = TriangleCalculator.CalculateTriangle(a, b, c);
            Assert.Equal("равнобедренный", type);
        }

        [Theory]
        [InlineData("3", "5", "5")]
        [InlineData("10", "12.5", "12.5")]
        public void CalculateTriangle_IsoscelesBC_ReturnsCorrectType(string a, string b, string c)
        {
            var (type, _) = TriangleCalculator.CalculateTriangle(a, b, c);
            Assert.Equal("равнобедренный", type);
        }

        [Theory]
        [InlineData("5", "3", "5")]
        [InlineData("12.5", "10", "12.5")]
        public void CalculateTriangle_IsoscelesAC_ReturnsCorrectType(string a, string b, string c)
        {
            var (type, _) = TriangleCalculator.CalculateTriangle(a, b, c);
            Assert.Equal("равнобедренный", type);
        }

        [Theory]
        [InlineData("3", "4", "5")] 
        [InlineData("5", "12", "13")]
        [InlineData("7.5", "8.2", "10.1")]
        public void CalculateTriangle_Scalene_ReturnsCorrectType(string a, string b, string c)
        {
            var (type, _) = TriangleCalculator.CalculateTriangle(a, b, c);
            Assert.Equal("разносторонний", type);
        }

        [Theory]
        [InlineData("1", "2", "10")]
        [InlineData("10", "1", "2")]
        [InlineData("2", "10", "1")]
        [InlineData("1", "2", "3")]
        public void CalculateTriangle_NotATriangle_ReturnsErrorTuple(string a, string b, string c)
        {
            var (type, coords) = TriangleCalculator.CalculateTriangle(a, b, c);
            Assert.Equal("не треугольник", type);
            Assert.Equal(new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) }, coords);
        }

        [Theory]
        [InlineData("0", "0", "0")]
        [InlineData("-5", "4", "5")]
        [InlineData("3", "-4", "5")]
        public void CalculateTriangle_ZeroOrNegative_ReturnsErrorTuple(string a, string b, string c)
        {
            var (type, coords) = TriangleCalculator.CalculateTriangle(a, b, c);
            Assert.Equal("", type);
            Assert.Equal(new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) }, coords);
        }

        [Theory]
        [InlineData("a", "b", "c")]
        [InlineData("", "4", "5")]
        [InlineData("3", " ", "5")]
        [InlineData(null, "4", "5")]
        public void CalculateTriangle_InvalidInput_ReturnsErrorTuple(string a, string b, string c)
        {
            var (type, coords) = TriangleCalculator.CalculateTriangle(a, b, c);
            Assert.Equal("", type);
            Assert.Equal(new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) }, coords);
        }
        
        [Fact]
        public void CalculateTriangle_ValidInput_ReturnsScaledCoordinates()
        {
            var (_, coords) = TriangleCalculator.CalculateTriangle("3", "4", "5");
            
            Assert.Equal(3, coords.Count);
            Assert.DoesNotContain((-1, -1), coords);
            Assert.DoesNotContain((-2, -2), coords);
        }
    }
}
