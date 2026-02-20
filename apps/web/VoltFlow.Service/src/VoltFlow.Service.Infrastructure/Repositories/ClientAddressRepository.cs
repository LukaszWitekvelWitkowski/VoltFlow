using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Entities;
using VoltFlow.Service.Core.Models.Auth;
using VoltFlow.Service.Core.Models.ClientAddress.DTOs;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Infrastructure.Data;

namespace VoltFlow.Service.Infrastructure.Repositories
{
    public class ClientAddressRepository : CacheRepository<ClientAddresesDTO, ClientAddressDTO, ClientAddress>, IClientAdressRepository
    {
        public ClientAddressRepository(VoltFlowDbContext context, IConfiguration configuration) : base(context, configuration)
        {
        }

        public ServiceResponse<Result> AddOrUpdateAddressAsync(int clientId, ClientAddressDTO addressDto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<ClientAddressDTO>> GetClientAddressAsync(int clientId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public override ClientAddressDTO MapToDto(ClientAddress e)
        {
            throw new NotImplementedException();
        }
    }
}
