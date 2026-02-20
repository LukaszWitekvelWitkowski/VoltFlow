using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Infrastructure.Repositories;

namespace VoltFlow.Service.Test.Integration.IntegrationTest.Controllers
{
    public class ElementIntegrationTests : BaseIntegrationTest
    {
        public ElementIntegrationTests(IntegrationTestFactory factory) : base(factory)
        {
        }

        #region Helper Methods
        // Senior Tip: Metoda pomocnicza, żeby nie powtarzać tworzenia drabinki w każdym teście
        private async Task<int> CreateHierarchyAndGetGroupIdAsync()
        {
            var category = new Category { Name = "Test Category" };
            DbContext.categories.Add(category);
            await DbContext.SaveChangesAsync();

            var group = new ElementGroup { Name = "Test Group", CategoryId = category.IdCategory };
            DbContext.elementgroups.Add(group);
            await DbContext.SaveChangesAsync();

            return group.IdElementGroup;
        }
        #endregion

        [Fact]
        public async Task CreateElement_ShouldReturnCreated_AndPersistInDb()
        {
            // Arrange
            var groupId = await CreateHierarchyAndGetGroupIdAsync();
            var request = new CreateElementRequest(
                "New Sensor",         // name
                groupId,              // elementGroupId
                null                  // description (lub podaj opis jeśli potrzebny)
            );

            // Act
            var response = await Client.PostAsJsonAsync("/api/element", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<ElementDTO>>();

            Assert.Equal("New Sensor", result._Data.Name);
            Assert.True(DbContext.elements.Any(x => x.Name == "New Sensor"));
        }

        [Fact]
        public async Task GetElementById_ShouldReturn404_WhenElementDoesNotExist()
        {
            // Act
            var response = await Client.GetAsync("/api/element/99999");
            var rawJson = await response.Content.ReadAsStringAsync();

            // To nam pokaże w logach testu co przyszło, jeśli asercja padnie
            Console.WriteLine($"Raw JSON: {rawJson}");

            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<ElementDTO>>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.NotNull(result);
            // Jeśli tu nadal jest 0, sprawdź czy w rawJson w ogóle jest klucz "_StatusCode"
            Assert.Equal(404, result._StatusCode);
        }

        [Fact]
        public async Task GetElements_ShouldReturnAllRecords()
        {
            // Arrange
            var groupId = await CreateHierarchyAndGetGroupIdAsync();
            DbContext.elements.AddRange(
                new Element { Name = "E1", ElementGroupId = groupId },
                new Element { Name = "E2", ElementGroupId = groupId }
            );
            await DbContext.SaveChangesAsync();

            // Act
            var response = await Client.GetAsync("/api/element");

            // Assert
            response.EnsureSuccessStatusCode();

            // ZMIANA: Zamiast IEnumerable<ElementDTO>, użyj klasy ElementsDTO 
            // (lub sprawdź, co dokładnie zwraca Twój GetElementsQueryHandler)
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<ElementsDTO>>();

            Assert.NotNull(result?._Data);
            Assert.True(result._Data.Items.Count() >= 2);
        }

        [Fact]
        public async Task UpdateElement_ShouldModifyName()
        {
            CacheRepository<CategoriesDTO, CategoryDTO, Category>.ResetStaticCache();
            CacheRepository<ElementGroupsDTO, ElementGroupDTO, Element>.ResetStaticCache();
            CacheRepository<ElementsDTO, ElementDTO, Element>.ResetStaticCache();

            // Arrange
            var groupId = await CreateHierarchyAndGetGroupIdAsync();
            var element = new Element { Name = "Old Name", ElementGroupId = groupId };
            DbContext.elements.Add(element);
            await DbContext.SaveChangesAsync();

            var request = new UpdateElementRequest
            {
                Id = element.IdElement,
                Name = "Updated Name",
                ElementGroupId = groupId
            };

            // Act
            var response = await Client.PutAsJsonAsync("/api/element", request);

            // Assert
            response.EnsureSuccessStatusCode();

            // Odświeżamy z bazy
            var updated = DbContext.elements.AsNoTracking().First(x => x.IdElement == element.IdElement);
            Assert.Equal("Updated Name", updated.Name);
        }

        [Fact]
        public async Task GetElementsSearch_ShouldFilterByName()
        {
            // Arrange
            var groupId = await CreateHierarchyAndGetGroupIdAsync();
            DbContext.elements.AddRange(
                new Element { Name = "Pressure Sensor", ElementGroupId = groupId },
                new Element { Name = "Temperature Gauge", ElementGroupId = groupId }
            );
            await DbContext.SaveChangesAsync();

            // Act
            var response = await Client.GetAsync("/api/element/search?name=Sensor&size=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PagedResultDTO<ElementDTO>>>();

            Assert.Contains(result._Data.Results, x => x.Name.Contains("Sensor"));
            Assert.DoesNotContain(result._Data.Results, x => x.Name.Contains("Gauge"));
        }
    }
}