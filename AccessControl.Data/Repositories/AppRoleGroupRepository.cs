using AccessControl.Data.Infrastructure;
using AccessControl.Model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IAppRoleGroupRepository : IRepository<AppRoleGroup>
    {
        Task<IQueryable<AppRoleGroup>> GetListByGroupId(int groupId);

        Task<IQueryable<AppGroup>> GetGroupListByRoleId(string roleId);
    }
    public class AppRoleGroupRepository : RepositoryBase<AppRoleGroup>, IAppRoleGroupRepository
    {
        private readonly ACSDBContext _context;
        public AppRoleGroupRepository(ACSDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<AppGroup>> GetGroupListByRoleId(string roleId)
        {
            var query = await (from g in _context.AppGroups
                               join rg in _context.AppRoleGroups
                               on g.Id equals rg.GroupId
                               where rg.RoleId == roleId
                               select g).ToListAsync();
            return query.AsQueryable();
        }

        public async Task<IQueryable<AppRoleGroup>> GetListByGroupId(int groupId)
        {
            var query = await (from ag in _context.AppGroups
                               join agr in _context.AppRoleGroups on ag.Id equals agr.GroupId
                               join ar in _context.Roles on agr.RoleId equals ar.Id
                               where agr.GroupId == groupId
                               select new AppRoleGroup
                               {
                                   GroupId = ag.Id,
                                   RoleId = ar.Id
                               }).ToListAsync();
            return query.AsQueryable();
        }
    }
}
