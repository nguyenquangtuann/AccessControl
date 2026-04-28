using AccessControl.Data.Infrastructure.Extentsions;
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
    public interface IAppRolesService
    {
        Task<AppRole> GetDetail(string id);

        Task<IQueryable<AppRole>> GetAll(string keyword);

        Task<IQueryable<AppRole>> GetAll();

        Task<AppRole> Add(AppRole appRole);

        Task<AppRole> Update(AppRole appRole);

        Task<AppRole> Delete(string id);

        Task<bool> AddRolesToGroup(List<AppRoleGroup> roleGroups, int groupId);

        Task<IQueryable<AppRole>> GetListRoleByGroupId(int? groupId);

        Task<IQueryable<AppRoleMapping>> GetTreeRoles();
        Task<IQueryable<AppMenuMapping>> GetTreeMenuByUserId(string userId);

        Task<IQueryable<AppUser>> GetListUserByGroupId(int groupId);
    }
    public class AppRolesService : IAppRolesService
    {
        private readonly IAppRolesRepository _appRoleRepository;
        private readonly IAppRoleGroupRepository _appRoleGroupRepository;
        public AppRolesService(IAppRolesRepository appRolesRepository, IAppRoleGroupRepository appRoleGroupRepository)
        {
            _appRoleRepository = appRolesRepository;
            _appRoleGroupRepository = appRoleGroupRepository;
        }

        public async Task<AppRole> Add(AppRole appRole)
        {
            if (await _appRoleRepository.CheckContainsAsync(r => r.Description == appRole.Description))
                throw new NameDuplicatedException("Tên quyền đã tồn tại!");
            if (await _appRoleRepository.CheckContainsAsync(r => r.Name == appRole.Name))
                throw new NameDuplicatedException("Mã quyền đã tồn tại!");
            return await _appRoleRepository.AddASync(appRole);
        }

        public async Task<bool> AddRolesToGroup(List<AppRoleGroup> roleGroups, int groupId)
        {
            await _appRoleGroupRepository.DeleteMulti(x => x.GroupId == groupId);
            foreach (var roleGroup in roleGroups)
            {
                await _appRoleGroupRepository.AddASync(roleGroup);
            }
            return true;
        }

        public async Task<AppRole> Delete(string id)
        {
            return await _appRoleRepository.DeleteAsync(id);
        }

        public async Task<IQueryable<AppRole>> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
                return await _appRoleRepository.GetAllAsync(x => x.Name.ToUpper().Contains(keyword.ToUpper()) || x.Description.ToUpper().Contains(keyword.ToUpper()));
            else
                return await _appRoleRepository.GetAllAsync();
        }

        public async Task<IQueryable<AppRole>> GetAll()
        {
            return await _appRoleRepository.GetAllAsync();
        }

        public async Task<AppRole> GetDetail(string id)
        {
            return await _appRoleRepository.GetSingleByConditionAsync(s => s.Id == id);
        }

        public async Task<IQueryable<AppRole>> GetListRoleByGroupId(int? groupId)
        {
            return await _appRoleRepository.GetListRoleByGroupId(groupId);
        }

        public async Task<IQueryable<AppRoleMapping>> GetTreeRoles()
        {
            return await _appRoleRepository.GetTreeRoles();
        }

        public async Task<AppRole> Update(AppRole appRole)
        {
            if (await _appRoleRepository.CheckContainsAsync(x => x.Description == appRole.Description && x.Id != appRole.Id))
                throw new NameDuplicatedException("Tên quyền đã tồn tại!");
            if (await _appRoleRepository.CheckContainsAsync(r => r.Name == appRole.Name && r.Id != appRole.Id))
                throw new NameDuplicatedException("Mã quyền đã tồn tại!");
            return await _appRoleRepository.UpdateASync(appRole);
        }
        public async Task<IQueryable<AppMenuMapping>> GetTreeMenuByUserId(string userId)
        {
            return await _appRoleRepository.GetTreeMenuByUserId(userId);
        }

        public async Task<IQueryable<AppUser>> GetListUserByGroupId(int groupId)
        {
            return await _appRoleRepository.GetListUserByGroupId(groupId);
        }
    }
}
