using MediatR;

namespace VoltFlow.Service.Core.Models.Category
{
    public class GetCategoryByNameQuery : IRequest<ServiceResponse<CategoryDTO>>
    {
        public string Name { get; }
        public GetCategoryByNameQuery(string name)
        {
            Name = name;
        }
    }
}
