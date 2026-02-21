using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;
using VoltFlow.Service.Infrastructure.Repositories;

namespace VoltFlow.Service.Test.Integration.IntegrationTest.Controllers
{
    public class ElementGroupIntegrationTests : BaseIntegrationTest
    {
        public ElementGroupIntegrationTests(IntegrationTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetElementGroups_ShouldReturnAllGroups_FromDatabase()
        {
            CacheRepository<ElementGroupCacheDTO, ElementGroupDTO, ElementGroup>.ResetStaticCache();

            // 1. Arrange - Najpierw tworzymy kategorię-rodzica
            var category = new Category { Name = "Test Category" };
            DbContext.categories.Add(category);
            await DbContext.SaveChangesAsync(); // Zapisujemy, żeby baza nadała IdCategory

            // 2. Teraz dodajemy grupy, przypisując im Id stworzonej kategorii
            DbContext.elementgroups.AddRange(
                new ElementGroup { Name = "Group A", CategoryId = category.IdCategory },
                new ElementGroup { Name = "Group B", CategoryId = category.IdCategory }
            );
            await DbContext.SaveChangesAsync();

            // 3. Act
            var response = await Client.GetAsync("/api/elementgroup");

            // 4. Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<ElementGroupCacheDTO>>();
            Assert.NotNull(result?._Data);
            Assert.Equal(2, result._Data.Items.Count());
        }


        [Fact]
        public async Task GetElementGroupById_ShouldReturn404_WhenGroupDoesNotExist()
        {
            // Act
            var response = await Client.GetAsync("/api/elementgroup/9999");

            // Assert
            // Zamiast ReadFromJsonAsync, najpierw pobierz tekst, żeby uniknąć wybuchu parsera
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NotFound || content.StartsWith("Grupa"))
            {
                Assert.Contains("nie istnieje", content);
                return;
            }

            var result = JsonSerializer.Deserialize<ServiceResponse<ElementGroupDTO>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(404, result._StatusCode);
        }

        [Fact]
        public async Task CreateElementGroup_ShouldReturnCreated_WhenCategoryExists()
        {
            // 1. Arrange - Najpierw musimy stworzyć kategorię, bo ElementGroup jej potrzebuje
            var category = new Category { Name = "Base Category" };
            DbContext.categories.Add(category);
            await DbContext.SaveChangesAsync();

            // Tworzymy request z poprawnym Id kategorii
            var request = new CreateElementGroupRequest("New Group", category.IdCategory);

            // 2. Act
            var response = await Client.PostAsJsonAsync("/api/elementgroup", request);

            // 3. Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<ElementGroupDTO>>();

            Assert.Equal("New Group", result._Data.Name);

            // Weryfikacja w bazie - czy grupa została przypisana do dobrej kategorii
            var groupInDb = DbContext.elementgroups.FirstOrDefault(x => x.Name == "New Group");
            Assert.NotNull(groupInDb);
            Assert.Equal(category.IdCategory, groupInDb.CategoryId);
        }

        [Fact]
        public async Task UpdateElementGroup_ShouldUpdateExistingRecord()
        {
            // 1. Arrange - Tworzymy kategorię, bo grupa bez niej nie przejdzie zapisu
            var category = new Category { Name = "Category for Update" };
            DbContext.categories.Add(category);
            await DbContext.SaveChangesAsync();

            // 2. Tworzymy grupę, którą będziemy aktualizować
            var group = new ElementGroup
            {
                Name = "Initial Name",
                CategoryId = category.IdCategory 
            };
            DbContext.elementgroups.Add(group);
            await DbContext.SaveChangesAsync();

            // Przygotowujemy żądanie aktualizacji
            var request = new UpdateElementGroupRequest
            {
                IdElementGroup = group.IdElementGroup,
                Name = "Brand New Name",
                CategoryId = category.IdCategory 
            };

            // 3. Act
            var response = await Client.PutAsJsonAsync("/api/elementgroup", request);

            // 4. Assert
            response.EnsureSuccessStatusCode();

            // Pobieramy świeże dane z bazy (AsNoTracking, aby ominąć cache EF)
            var updatedGroup = await DbContext.elementgroups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdElementGroup == group.IdElementGroup);

            Assert.NotNull(updatedGroup);
            Assert.Equal("Brand New Name", updatedGroup.Name);
        }

        [Fact]
        public async Task GetElementGroupSearch_ShouldReturnPaginatedResults()
        {
            // 1. Arrange - Tworzymy kategorię (rodzica)
            var category = new Category { Name = "Search Test Category" };
            DbContext.categories.Add(category);
            await DbContext.SaveChangesAsync();

            // 2. Dodajemy grupy przypisane do tej kategorii
            DbContext.elementgroups.AddRange(
                new ElementGroup { Name = "Alpha", CategoryId = category.IdCategory },
                new ElementGroup { Name = "Beta", CategoryId = category.IdCategory },
                new ElementGroup { Name = "Gamma", CategoryId = category.IdCategory }
            );
            await DbContext.SaveChangesAsync();

            // 3. Act
            var response = await Client.GetAsync("/api/elementgroup/search?name=a&number=1&size=10");

            // 4. Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PagedResultDTO<ElementGroupDTO>>>();

            Assert.NotNull(result?._Data);
            Assert.True(result._Data.TotalCount >= 2);
        }
    }
}
