using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Client.DTOs;
using VoltFlow.Service.Core.Models.Client.Requests;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Pagination;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ClientRepository : CacheRepository<ClientCacheDTO, ClientDTO, Client>, IClientRepository
    {
        public ClientRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

        override
        public ClientDTO MapToDto(Client e) => new ClientDTO
        {
            IdClient = e.IdClient,
            Email = e.Email,
            Name = e.Name,
            Phone = e.Phone,
            StatusClient = e.statusClient
        };

        public async Task AddAsync(Client newClient, CancellationToken ct)
        {
            await _context.Set<Client>().AddAsync(newClient, ct);
            await _context.SaveChangesAsync(ct);

            ResetStaticCache();
        }

        public async Task<ServiceResponse<PagedResultDTO<ClientDTO>>> GetAllClientsAsync(string? email, int page, int size,CancellationToken ct)
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
            {
                var sourceResponse = ServiceResponse<IEnumerable<ClientDTO>>.Result(cache.Items);
                return PagedHelper.ToPagedResponse(
                  sourceResponse,
                  email,
                  eg => eg.Email,
                  page,
                  size
              );
            }

            var dbQuery = _context.Set<Client>().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(email))
            {
                var search = email.Trim().ToLower();
                dbQuery = dbQuery.Where(e => e.Name.ToLower().Contains(search));
            }

            var dbTotalCount = await dbQuery.CountAsync();
            var dbItems = await dbQuery
                .OrderBy(e => e.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(e => MapToDto(e))
                .ToListAsync();

            var dbPagedResult = new PagedResultDTO<ClientDTO>(dbItems, dbTotalCount, page, size);

            return ServiceResponse<PagedResultDTO<ClientDTO>>.Result(dbPagedResult);

        }

        public async Task<ServiceResponse<ClientDTO>> UpdateCleintAsync(ClientRequest request, CancellationToken ct)
        {
           var client = await _context.Set<Client>().FindAsync(new object[] { request.IdClient }, ct);

            if (string.IsNullOrEmpty(client.Email))
            {
                return ServiceResponse<ClientDTO>.Failure("Client must have an email to update profile.");
            }

            client.Email = request.Email ?? client.Email;
            client.statusClient = request.StatusClient ?? client.statusClient;

            _context.Set<Client>().Update(client);
            await _context.SaveChangesAsync(ct);

            ResetStaticCache();

            return ServiceResponse<ClientDTO>.Success(MapToDto(client));
   
        }
    }
}
