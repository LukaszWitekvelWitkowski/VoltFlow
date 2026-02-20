using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.ClientAddress.Request;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ClientAddressRepository : CacheRepository<ClientAddressCacheDTO, ClientAddressDTO, ClientAddress>, IClientAdressRepository
    {
        public ClientAddressRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

        public async Task<ServiceResponse<ClientAddressDTO>> AddOrUpdateAddressAsync(ClientAddressRequest addressDto, CancellationToken ct)
        {
            // Szukamy adresu powiązanego z klientem (zakładając, że szukamy po clientId i IdAddress)
            var existingAddress = await _context.Set<ClientAddress>()
                .FirstOrDefaultAsync(a => a.ClientId == addressDto.ClientId && a.IdAddress == addressDto.IdAddress, ct);

            ClientAddressDTO result;

            if (existingAddress == null)
            {
                var newAddress = new ClientAddress
                {
                    ClientId = addressDto.ClientId,
                    City = addressDto.City,
                    Street = addressDto.Street,
                    ZipCode = addressDto.ZipCode,
                    NumberStreet = addressDto.NumberStreet,
                    AddressType = addressDto.AddressType,
                    LocationNumber = addressDto.LocationNumber,
                    IsObsolete = addressDto.IsObsolete
                };
                _context.Set<ClientAddress>().Add(newAddress);

                   result = MapToDto(newAddress);
            }
            else
            {
                existingAddress.City = addressDto.City;
                existingAddress.Street = addressDto.Street;
                existingAddress.ZipCode = addressDto.ZipCode;
                existingAddress.NumberStreet = addressDto.NumberStreet;
                existingAddress.AddressType = addressDto.AddressType;
                existingAddress.LocationNumber = addressDto.LocationNumber;
                existingAddress.IsObsolete = addressDto.IsObsolete;

                result = MapToDto(existingAddress);
            }

            await _context.SaveChangesAsync(ct);
            ResetStaticCache(); 

            return ServiceResponse<ClientAddressDTO>.Result(result);
        }

        public async Task<ServiceResponse<ClientAddressCacheDTO>> GetClientAddressFromCacheAsync(int clientId, CancellationToken ct)
        {
            var cache = await GetOrUpdateCacheAsync();

            if (cache != null)
            {
                // Filtrujemy dane z cache dla konkretnego klienta
                var clientAddresses = cache.Items.Where(a => a.IdAddress == clientId);
                return ServiceResponse<ClientAddressCacheDTO>.Result(new ClientAddressCacheDTO { Items = clientAddresses });
            }

            // Fallback do bazy danych
            var dbData = await _context.Set<ClientAddress>()
                .AsNoTracking()
                .Where(a => a.ClientId == clientId)
                .Select(e => MapToDto(e))
                .ToListAsync(ct);

            return ServiceResponse<ClientAddressCacheDTO>.Result(new ClientAddressCacheDTO { Items = dbData });
        }

        override
        public  ClientAddressDTO MapToDto(ClientAddress e)
        {
            return new ClientAddressDTO
            {
                IdAddress = e.IdAddress,
                City = e.City,
                Street = e.Street,
                ZipCode = e.ZipCode,
                NumberStreet = e.NumberStreet,
                AddressType = e.AddressType,
                LocationNumber = e.LocationNumber,
                IsObsolete = e.IsObsolete
            };
        }


    }
}
