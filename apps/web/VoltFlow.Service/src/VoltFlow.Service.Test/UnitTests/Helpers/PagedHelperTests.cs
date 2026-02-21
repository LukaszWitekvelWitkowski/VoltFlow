using VoltFlow.Service.Core.Abstractions.Generic;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Pagination;

namespace VoltFlow.Service.Test.UnitTests.Helpers
{
    public class PagedHelperTests
    {
        private readonly List<TestDto> _sourceData;

        public PagedHelperTests()
        {
            // We prepare 5 test objects
            _sourceData = new List<TestDto>
            {
                new() { Id = 1, Name = "Apple" },
                new() { Id = 2, Name = "Banana" },
                new() { Id = 3, Name = "Cherry" },
                new() { Id = 4, Name = "Date" },
                new() { Id = 5, Name = "Elderberry" }
            };
        }

        [Theory]
        [InlineData(1, 2, 2)] // Page 1, size 2 -> 2 elements
        [InlineData(3, 2, 1)] // Page 3, size 2 -> 1 element (last)
        [InlineData(1, 10, 5)] // Page 1, size 10 -> all 5
        public void ToPagedResponse_ShouldReturnCorrectNumberOfItems(int page, int size, int expectedCount)
        {
            // Arrange
            var source = new TestCacheData<TestDto>(_sourceData);

            // Act
            var result = PagedHelper.ToPagedResponse(source, null, x => x.Name, page, size);

            // Assert
            Assert.Equal(expectedCount, result.Results.Count());
            Assert.Equal(5, result.TotalCount);
        }

        [Fact]
        public void ToPagedResponse_ShouldFilterByName_CaseInsensitive()
        {
            // Arrange
            var source = new TestCacheData<TestDto>(_sourceData);

            // Act
            var result = PagedHelper.ToPagedResponse(source, "ANAN", x => x.Name, 1, 10);

            // Assert
            Assert.Single(result.Results);
            Assert.Equal("Banana", result.Results.First().Name);
            Assert.Equal(1, result.TotalCount); // TotalCount po filtrze powinien wynosić 1
        }

        [Fact]
        public void ToPagedResponse_ShouldHandleEmptySource()
        {
            // Arrange
            var source = new TestCacheData<TestDto>(new List<TestDto>());

            // Act
            var result = PagedHelper.ToPagedResponse(source, "Any", x => x.Name, 1, 10);

            // Assert
            Assert.Empty(result.Results);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public void ToPagedResponse_ShouldReturnEmpty_WhenPageNumberTooHigh()
        {
            // Arrange
            var source = new TestCacheData<TestDto>(_sourceData);

            // Act
            var result = PagedHelper.ToPagedResponse(source, null, x => x.Name, 10, 2);

            // Assert
            Assert.Empty(result.Results);
            Assert.Equal(5, result.TotalCount); 
        }

        private class TestDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class TestCacheData<T> : ICacheData<T> where T : class
        {
            // Właściwość wymagana przez Twój interfejs
            public IEnumerable<T> Items { get; set; } = new List<T>();

            // Metoda wymagana przez Twój interfejs
            public void insert(IEnumerable<T> enumerable)
            {
                Items = enumerable;
            }

            // Opcjonalny konstruktor dla wygody w testach
            public TestCacheData() { }

            public TestCacheData(IEnumerable<T> initialData)
            {
                Items = initialData;
            }
        }
    }
}
