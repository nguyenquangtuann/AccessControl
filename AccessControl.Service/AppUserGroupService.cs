using AccessControl.Data.Repositories;
using AccessControl.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IAppUserGroupService
    {
        Task DeleteMultipleByGroupId(int groupId);

        Task<IQueryable<AppGroup>> GetAppGroupByUserId(string userId);
    }
    public class AppUserGroupService : IAppUserGroupService
    {
        private readonly IAppUserGroupRepository _appUserGroupRepository;
        public AppUserGroupService(IAppUserGroupRepository appUserGroupRepository)
        {
            _appUserGroupRepository = appUserGroupRepository;
        }

        public async Task DeleteMultipleByGroupId(int groupId)
        {
            await _appUserGroupRepository.DeleteMulti(x => x.GroupId == groupId);
        }

        public async Task<IQueryable<AppGroup>> GetAppGroupByUserId(string userId)
        {
            return await _appUserGroupRepository.GetAppGroupByUserId(userId);
        }
    }
}
