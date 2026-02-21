using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class BaseRepository
    {
         protected readonly VoltFlowDbContext _context;

        public BaseRepository(VoltFlowDbContext context, IConfiguration configuration)
        {
            _context = context;
        }
    }
}
