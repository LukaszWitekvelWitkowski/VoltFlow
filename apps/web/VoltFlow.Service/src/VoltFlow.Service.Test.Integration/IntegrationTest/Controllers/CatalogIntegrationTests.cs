using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Catalog.DTOs; // Zakładam taką przestrzeń nazw
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Test.Integration.IntegrationTest;

namespace VoltFlow.Service.Test.Integration.Controllers
{
    public class CatalogIntegrationTests : BaseIntegrationTest
    {
        public CatalogIntegrationTests(IntegrationTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCatalogSearch_ShouldReturnEverything_WhenNoFiltersApplied()
        {
                // Arrange
                var category = new Category { Name = "Industrial" };
                DbContext.categories.Add(category);
                await DbContext.SaveChangesAsync();

                var group = new ElementGroup { Name = "Sensors", CategoryId = category.IdCategory };
                DbContext.elementgroups.Add(group);
                await DbContext.SaveChangesAsync();

                DbContext.elements.AddRange(
                    new Element { Name = "Pressure Sensor", ElementGroupId = group.IdElementGroup },
                    new Element { Name = "Flow Meter", ElementGroupId = group.IdElementGroup }
                );
                await DbContext.SaveChangesAsync();

                // Act
                var response = await Client.GetAsync("/api/catalog/search?name=&number=1&size=10");

                // Assert
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PagedResultDTO<ElementTreeDTO>>>();

                Assert.NotNull(result?._Data);
                Assert.NotEmpty(result._Data.Results);

                Assert.Contains(result._Data.Results, x => x.Name == "Pressure Sensor");

        }

        [Fact]
        public async Task GetCatalogSearch_ShouldReturnEmpty_WhenNameDoesNotMatch()
        {
            // Arrange
            var category = new Category { Name = "Electronics" };
            DbContext.categories.Add(category);
            await DbContext.SaveChangesAsync();

            var group = new ElementGroup { Name = "Resistors", CategoryId = category.IdCategory };
            DbContext.elementgroups.Add(group);
            await DbContext.SaveChangesAsync();

            DbContext.elements.Add(new Element { Name = "10k Ohm", ElementGroupId = group.IdElementGroup });
            await DbContext.SaveChangesAsync();


            // Act
            var response = await Client.GetAsync("/api/catalog/search?ElementName=NonExistent&Number=1&Size=10");

            // Assert
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PagedResultDTO<ElementTreeDTO>>>();

            Assert.NotNull(result?._Data);
            Assert.Empty(result._Data.Results);
        }

        [Fact]
        public async Task GetCatalogSearch_ShouldFilterByCategoryName()
        {
            // 1. Arrange - Tworzymy dwa światy (Elektronika i Mechanika)
            var cat1 = new Category { Name = "Electronics" };
            var cat2 = new Category { Name = "Mechanical" };
            DbContext.categories.AddRange(cat1, cat2);
            await DbContext.SaveChangesAsync();

            var group1 = new ElementGroup { Name = "Resistors", CategoryId = cat1.IdCategory };
            var group2 = new ElementGroup { Name = "Gears", CategoryId = cat2.IdCategory };
            DbContext.elementgroups.AddRange(group1, group2);
            await DbContext.SaveChangesAsync();

            DbContext.elements.AddRange(
                new Element { Name = "10k Ohm", ElementGroupId = group1.IdElementGroup },
                new Element { Name = "Big Gear", ElementGroupId = group2.IdElementGroup }
            );
            await DbContext.SaveChangesAsync();

            // 2. Act 
            var response = await Client.GetAsync("/api/catalog/search?CategoryName=Mechanical&Number=1&Size=10");

            // 3. Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PagedResultDTO<ElementTreeDTO>>>();

            Assert.Single(result._Data.Results);
            Assert.Equal("Big Gear", result._Data.Results.First().Name);
            Assert.Equal("Mechanical", result._Data.Results.First().Group.Category.Name);
        }

        [Fact]
        public async Task GetCatalogSearch_ShouldFilterByGroupName()
        {
            // Arrange
            var cat = new Category { Name = "Tools" };
            DbContext.categories.Add(cat);
            await DbContext.SaveChangesAsync();

            var groupA = new ElementGroup { Name = "Drills", CategoryId = cat.IdCategory };
            var groupB = new ElementGroup { Name = "Hammers", CategoryId = cat.IdCategory };
            DbContext.elementgroups.AddRange(groupA, groupB);
            await DbContext.SaveChangesAsync();

            DbContext.elements.AddRange(
                new Element { Name = "Power Drill", ElementGroupId = groupA.IdElementGroup },
                new Element { Name = "Heavy Hammer", ElementGroupId = groupB.IdElementGroup }
            );
            await DbContext.SaveChangesAsync();

            // Act
            var response = await Client.GetAsync("/api/catalog/search?ElementGroupName=Drills&Number=1&Size=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PagedResultDTO<ElementTreeDTO>>>();

            Assert.All(result._Data.Results, item => Assert.Equal("Drills", item.Group.Name));
        }
    }
}