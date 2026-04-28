using AccessControl.Data;
using AccessControl.Data.Repositories;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IAppUserService
    {
        Task<IQueryable<AppUser>> GetAll();
        Task<IQueryable<AppUserMapping>> GetAllByMappingAsync(string keyword);
    }
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserRepository _appUserRepository;
        public AppUserService(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<IQueryable<AppUser>> GetAll()
        {
            return await _appUserRepository.GetAllAsync(x=>x.Status == true);
        }

        public async Task<IQueryable<AppUserMapping>> GetAllByMappingAsync(string keyword)
        {
            return await _appUserRepository.GetAllByMapping(keyword);
        }
    }
}
