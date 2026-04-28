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
    public interface IAppUserGroupRepository : IRepository<AppUserGroup>
    {
        Task<IQueryable<AppGroup>> GetAppGroupByUserId(string userId);
    }
    public class AppUserGroupRepository : RepositoryBase<AppUserGroup>, IAppUserGroupRepository
    {
        private readonly ACSDBContext _context;
        public AppUserGroupRepository(ACSDBContext context):base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<AppGroup>> GetAppGroupByUserId(string userId)
        {
            var query = await(from g in _context.AppGroups
                              join ug in _context.AppUserGroups on g.Id equals ug.GroupId
                              where ug.UserId == userId
                              select g).ToListAsync();
            return query.AsQueryable();
        }
    }
}
