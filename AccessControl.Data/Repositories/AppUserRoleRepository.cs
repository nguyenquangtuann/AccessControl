using AccessControl.Data.Infrastructure;
using AccessControl.Model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IAppUserRoleRepository : IRepository<AppUserRole>
    {
        Task<List<string>> GetListRoleCheck(string userName);
        Task<IQueryable<string>> GetListRole(string userId);
    }
    public class AppUserRoleRepository : RepositoryBase<AppUserRole>, IAppUserRoleRepository
    {
        private readonly ACSDBContext _context;
        public AppUserRoleRepository(ACSDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<string>> GetListRoleCheck(string userName)
        {
            var query = await (from r in _context.Roles
                               join rg in _context.AppRoleGroups on r.Id equals rg.RoleId
                               join ug in _context.AppUserGroups on rg.GroupId equals ug.GroupId
                               join u in _context.Users on ug.UserId equals u.Id
                               where u.UserName == userName
                               select r.Name).ToListAsync();
            return query;
        }

        public async Task<IQueryable<string>> GetListRole(string userId)
        {
            return (await (from ar in _context.AppRoles
                           join arg in _context.AppRoleGroups on ar.Id equals arg.RoleId into approleGroup
                           from arg in approleGroup.DefaultIfEmpty()

                           join ag in _context.AppGroups on arg.GroupId equals ag.Id into appGroup
                           from ag in appGroup.DefaultIfEmpty()

                           join aug in _context.AppUserGroups on ag.Id equals aug.GroupId into appUserGroup
                           from aug in appUserGroup.DefaultIfEmpty()

                           join au in _context.AppUsers on aug.UserId equals au.Id into appUser
                           from au in appUser.DefaultIfEmpty()
                           where au.Id == userId
                           select ar.Name).ToListAsync()).AsQueryable();
        }
    }
}
