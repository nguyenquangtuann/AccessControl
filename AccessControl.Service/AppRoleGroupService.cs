using AccessControl.Data;
using AccessControl.Data.Repositories;
using AccessControl.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IAppRoleGroupService
    {
        Task<IQueryable<AppRoleGroup>> GetListByGroupId(int groupId);

        Task<IQueryable<AppGroup>> GetGroupListByRoleId(string roleId);

        Task DeleteMultipleAppRoleGroupByGroupId(int groupId);

        Task DeleteMultipleAppRoleGroupByRoleId(string roleId);
    }
    public class AppRoleGroupService : IAppRoleGroupService
    {
        private readonly IAppRoleGroupRepository _roleGroupRepository;
        public AppRoleGroupService(IAppRoleGroupRepository roleGroupRepository)
        {
            _roleGroupRepository = roleGroupRepository;
        }
        public async Task<IQueryable<AppRoleGroup>> GetListByGroupId(int groupId)
        {
            return await _roleGroupRepository.GetListByGroupId(groupId);
        }

        public async Task DeleteMultipleAppRoleGroupByGroupId(int groupId)
        {
            await _roleGroupRepository.DeleteMulti(x => x.GroupId == groupId);
        }

        public async Task DeleteMultipleAppRoleGroupByRoleId(string roleId)
        {
            await _roleGroupRepository.DeleteMulti(x => x.RoleId == roleId);
        }

        public async Task<IQueryable<AppGroup>> GetGroupListByRoleId(string roleId)
        {
            return await _roleGroupRepository.GetGroupListByRoleId(roleId);
        }
    }
}
