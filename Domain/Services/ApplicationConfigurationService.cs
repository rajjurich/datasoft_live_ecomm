using Domain.Core;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IApplicationConfigurationService
    {
        Task<ApplicationConfiguration> Get();
    }
    public class ApplicationConfigurationService : IApplicationConfigurationService
    {
        private readonly IEntityRepository<ApplicationConfiguration> _applicationConfigurationRepository;
        public ApplicationConfigurationService(IEntityRepository<ApplicationConfiguration> applicationConfigurationRepository)
        {
            _applicationConfigurationRepository = applicationConfigurationRepository;
        }
        public async Task<ApplicationConfiguration> Get()
        {
            return await _applicationConfigurationRepository.GetAll().FirstOrDefaultAsync();
        }
    }
}
