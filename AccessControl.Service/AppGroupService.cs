using AccessControl.Data.Infrastructure.Extentsions;
using AccessControl.Data.Repositories;
using AccessControl.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IAppGroupService
    {
        Task<AppGroup> GetDetail(int id);

        Task<AppGroup> GetByName(string name);

        Task<IQueryable<AppGroup>> GetAll(string keyword);

        Task<IQueryable<AppGroup>> GetAll();

        Task<AppGroup> Add(AppGroup appGroup);

        Task<AppGroup> Update(AppGroup appGroup);

        Task<AppGroup> Delete(int id);

        Task<bool> AddUserToGroups(List<AppUserGroup> groups, string userId);

        Task<IQueryable<AppGroup>> GetListGroupByUserId(string userId);

        Task<IQueryable<AppUser>> GetListUserByGroupId(int groupId);
        Task<AppGroup> GetById(int id);
    }
    public class AppGroupService : IAppGroupService
    {
        private readonly IAppGroupRepository _appGroupRepository;
        private readonly IAppUserGroupRepository _appUserGroupRepository;
        public AppGroupService(IAppGroupRepository appGroupRepository, IAppUserGroupRepository appUserGroupRepository)
        {
            _appGroupRepository = appGroupRepository;
            _appUserGroupRepository = appUserGroupRepository;
        }

        public async Task<AppGroup> Add(AppGroup appGroup)
        {
            return await _appGroupRepository.CheckContainsAsync(x => x.GroupCode == appGroup.GroupCode)
                ? throw new NameDuplicatedException("Mã nhóm đã tồn tại")
                : await _appGroupRepository.CheckContainsAsync(x => x.Name == appGroup.Name)
                ? throw new NameDuplicatedException("Tên nhóm đã tồn tại")
                : await _appGroupRepository.AddASync(appGroup);
        }

        public async Task<AppGroup> Delete(int id)
        {
            return await _appGroupRepository.DeleteAsync(id);
        }

        public async Task<IQueryable<AppGroup>> GetAll()
        {
            return await _appGroupRepository.GetAllAsync();
        }

        public async Task<IQueryable<AppGroup>> GetAll(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return await _appGroupRepository.GetAllAsync();
            }
            else
            {
                return await _appGroupRepository.GetAllAsync(x => (x.GroupCode.ToLower().Contains(keyword.Trim()) || x.Name.ToLower().Contains(keyword.Trim()))
                );
            }
        }

        public async Task<AppGroup> GetDetail(int id)
        {
            return await _appGroupRepository.GetByIdAsync(id);
        }

        public async Task<AppGroup> Update(AppGroup appGroup)
        {
            return await _appGroupRepository.CheckContainsAsync(x => x.GroupCode == appGroup.GroupCode && x.Id != appGroup.Id)
                ? throw new NameDuplicatedException("Mã nhóm đã tồn tại")
                : await _appGroupRepository.CheckContainsAsync(x => x.Name == appGroup.Name && x.Id != appGroup.Id)
                ? throw new NameDuplicatedException("Tên nhóm đã tồn tại")
                : await _appGroupRepository.UpdateASync(appGroup);
        }

        public async Task<bool> AddUserToGroups(List<AppUserGroup> userGroups, string userId)
        {
            await _appUserGroupRepository.DeleteMulti(x => x.UserId == userId);
            foreach (AppUserGroup userGroup in userGroups)
            {
                _ = await _appUserGroupRepository.AddASync(userGroup);
            }
            return true;
        }

        public async Task<IQueryable<AppGroup>> GetListGroupByUserId(string userId)
        {
            return await _appGroupRepository.GetListGroupByUserId(userId);
        }

        public async Task<IQueryable<AppUser>> GetListUserByGroupId(int groupId)
        {
            return await _appGroupRepository.GetListUserByGroupId(groupId);
        }

        public async Task<AppGroup> GetByName(string name)
        {
            return await _appGroupRepository.GetSingleByConditionAsync(x => x.GroupCode == name);
        }

        public async Task<AppGroup> GetById(int id)
        {
            return await _appGroupRepository.GetByIdAsync(id);
        }
    }
}
