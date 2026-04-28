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
    public interface IAppGroupRepository : IRepository<AppGroup>
    {
        Task<IQueryable<AppGroup>> GetListGroupByUserId(string userId);

        Task<IQueryable<AppUser>> GetListUserByGroupId(int groupId);
    }
    public class AppGroupRepository : RepositoryBase<AppGroup>, IAppGroupRepository
    {
        private readonly ACSDBContext _context;
        public AppGroupRepository(ACSDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<AppGroup>> GetListGroupByUserId(string userId)
        {
            var query = await(from g in _context.AppGroups
                              join ug in _context.AppUserGroups
                              on g.Id equals ug.GroupId
                              where ug.UserId == userId
                              select g).ToListAsync();
            return query.AsQueryable();
        }

        public async Task<IQueryable<AppUser>> GetListUserByGroupId(int groupId)
        {
            var query = await(from g in _context.AppGroups
                              join ug in _context.AppUserGroups
                              on g.Id equals ug.GroupId
                              join u in _context.Users
                              on ug.UserId equals u.Id
                              where ug.GroupId == groupId
                              select u).ToListAsync();
            return (IQueryable<AppUser>)query;
        }
    }
}
