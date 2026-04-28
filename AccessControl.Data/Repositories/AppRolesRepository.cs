using AccessControl.Data.Infrastructure;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IAppRolesRepository : IRepository<AppRole>
    {
        Task<IQueryable<AppRole>> GetListRoleByGroupId(int? groupId);
        Task<IQueryable<AppRoleMapping>> GetTreeRoles();
        Task<IQueryable<AppMenuMapping>> GetTreeMenuByUserId(string userId);
        Task<IQueryable<AppUser>> GetListUserByGroupId(int groupId);
    }
    public class AppRolesRepository : RepositoryBase<AppRole>, IAppRolesRepository
    {
        private readonly ACSDBContext _context;
        public AppRolesRepository(ACSDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<AppRole>> GetListRoleByGroupId(int? groupId)
        {
            var query = await(from g in _context.AppRoles
                              join ug in _context.AppRoleGroups on g.Id equals ug.RoleId
                              where ug.GroupId == groupId
                              select g).ToListAsync();
            return query.AsQueryable();
        }

        public async Task<IQueryable<AppUser>> GetListUserByGroupId(int groupId)
        {
            var listUsers = (await(from aug in _context.AppUserGroups
                                   join au in _context.AppUsers on aug.UserId equals au.Id
                                   where aug.GroupId == groupId
                                   select new AppUser
                                   {
                                       Id = au.Id,
                                       UserName = au.UserName,
                                       Email = au.Email,
                                       PhoneNumber = au.PhoneNumber,
                                       NormalizedUserName = au.NormalizedUserName,
                                       Image = au.Image,
                                       FullName = au.FullName,
                                       IsDeleted = au.IsDeleted,
                                       CreatedBy = au.CreatedBy,
                                       UpdatedBy = au.UpdatedBy

                                   }).ToListAsync());
            return listUsers.AsQueryable();
        }

        public async Task<IQueryable<AppMenuMapping>> GetTreeMenuByUserId(string userId)
        {
            var parameters = new SqlParameter[]
              {
                    new SqlParameter("@userId",SqlDbType.NVarChar){Value = userId}
              };
            var listChildren = _context.GetMenuList.FromSqlRaw("dbo.GetMenuList @userId", parameters).ToList();
            var listParents = listChildren.Select(x => x.ParentId).ToList().Distinct();

            List<AppMenuMapping> listChildrenNew = new List<AppMenuMapping>();
            foreach (var child in listChildren)
            {
                listChildrenNew.Add(new AppMenuMapping() { Id = child.Id, MenuName = child.Description, ParentId = child.ParentId, Icon = child.Icon, Link = child.Link, ActiveLink = child.ActiveLink });
            }

            var parents = (await(from ar in _context.AppRoles

                                 orderby ar.OrderBy

                                 select new AppMenuMapping
                                 {
                                     MenuName = ar.Description,
                                     ParentId = ar.ParentId,
                                     Id = ar.Id,
                                     Icon = ar.Icon,
                                     Link = ar.Link,
                                     ActiveLink = ar.ActiveLink,
                                     Childrens = new List<AppMenuMapping>()

                                 }).ToListAsync()).AsQueryable();
            parents = parents.Where(x => listParents.Contains(x.Id));
            List<AppMenuMapping> menus = new List<AppMenuMapping>();
            foreach (var parent in parents)
            {
                var children = listChildrenNew.Where(x => x.ParentId == parent.Id).ToList();
                parent.Childrens = children;
                menus.Add(parent);

            }

            return menus.AsQueryable();
        }

        public async Task<IQueryable<AppRoleMapping>> GetTreeRoles()
        {
            var query = await(from r in _context.AppRoles
                              where r.ParentId == null
                              orderby r.OrderBy
                              select new AppRoleMapping
                              {
                                  Id = r.Id,
                                  Description = r.Description,
                                  Name = r.Name,
                                  ParentId = r.ParentId,
                                  Childrens = (from r1 in _context.AppRoles
                                               where r1.ParentId == r.Id
                                               orderby r1.OrderBy
                                               select new AppRoleMapping
                                               {
                                                   Id = r1.Id,
                                                   ParentId = r1.ParentId,
                                                   Description = r1.Description,
                                                   Name = r1.Name,
                                                   Childrens =
                                               (from r2 in _context.AppRoles where r2.ParentId == r1.Id select new AppRoleMapping { ParentId = r2.ParentId, Name = r2.Name, Id = r2.Id, Description = r2.Description }).ToList()
                                               }).ToList()
                              }).ToListAsync();
            return query.AsQueryable();
        }
    }
}
