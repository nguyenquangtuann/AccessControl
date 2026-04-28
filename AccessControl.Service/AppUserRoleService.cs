using AccessControl.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IAppUserRoleService
    {
        Task<IQueryable<string>> GetAllUserRole(string userId);

        Task DeleteMultipleAppUserRoleByRoleId(string roleId);

        Task<List<string>> GetListRoleCheck(string userName);
    }
    public class AppUserRoleService : IAppUserRoleService
    {
        private readonly IAppUserRoleRepository _appUserRoleRepository;
        public AppUserRoleService(IAppUserRoleRepository appUserRoleRepository)
        {
            _appUserRoleRepository = appUserRoleRepository;
        }

        public async Task<IQueryable<string>> GetAllUserRole(string userId)
        {
            return await _appUserRoleRepository.GetListRole(userId);
        }

        public async Task DeleteMultipleAppUserRoleByRoleId(string roleId)
        {
            await _appUserRoleRepository.DeleteMulti(x => x.RoleId == roleId);
        }


        public async Task<List<string>> GetListRoleCheck(string userName)
        {
            return await _appUserRoleRepository.GetListRoleCheck(userName);
        }
    }
}
