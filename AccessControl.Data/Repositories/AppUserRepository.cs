using AccessControl.Data.Infrastructure;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        Task<IQueryable<AppUserMapping>> GetAllByMapping(string keyword);
    }
    public class AppUserRepository : RepositoryBase<AppUser>, IAppUserRepository
    {
        private readonly ACSDBContext _context;
        public AppUserRepository(ACSDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<AppUserMapping>> GetAllByMapping(string keyword)
        {

            if (string.IsNullOrEmpty(keyword))
            {
                var query = (await (from u in _context.AppUsers
                                    join ug in _context.AppUserGroups on u.Id equals ug.UserId into userGroup
                                    from ug in userGroup.DefaultIfEmpty()
                                    join g in _context.AppGroups on ug.GroupId equals g.Id into appGroup
                                    from g in appGroup.DefaultIfEmpty()
                                    join em in _context.Employees on u.EM_ID equals em.EmId into emu
                                    from em in emu.DefaultIfEmpty()
                                        //where u.Id != "144d5520-2550-474a-b805-bbd991ad2f71"
                                    where u.Status == true
                                    select new AppUserMapping
                                    {
                                        CreatedBy = u.CreatedBy,
                                        CreatedDate = u.CreatedDate,
                                        DeletedBy = u.DeletedBy,
                                        DeletedDate = u.DeletedDate,
                                        Email = u.Email,
                                        FullName = u.FullName,
                                        Id = u.Id,
                                        Image = u.Image,
                                        IsDeleted = u.IsDeleted,
                                        PhoneNumber = u.PhoneNumber,
                                        UpdatedDate = u.UpdatedDate,
                                        UpdatedBy = u.UpdatedBy,
                                        UserName = u.UserName,
                                        GroupId = (from g1 in _context.AppUserGroups where g1.UserId == u.Id select g1.GroupId).FirstOrDefault(),
                                        EmId = u.EM_ID,
                                        EmName = em.EmName,
                                        Status = u.Status
                                    }).ToListAsync()).AsQueryable();
                return query;
            }
            else
            {
                var query = (await (from u in _context.AppUsers
                                    join ug in _context.AppUserGroups on u.Id equals ug.UserId into userGroup
                                    from ug in userGroup.DefaultIfEmpty()
                                    join g in _context.AppGroups on ug.GroupId equals g.Id into appGroup
                                    from g in appGroup.DefaultIfEmpty()
                                    join em in _context.Employees on u.EM_ID equals em.EmId into emu
                                    from em in emu.DefaultIfEmpty()
                                        //where u.Id != "144d5520-2550-474a-b805-bbd991ad2f71"
                                    select new AppUserMapping
                                    {
                                        CreatedBy = u.CreatedBy,
                                        CreatedDate = u.CreatedDate,
                                        DeletedBy = u.DeletedBy,
                                        DeletedDate = u.DeletedDate,
                                        Email = u.Email,
                                        FullName = u.FullName,
                                        Id = u.Id,
                                        Image = u.Image,
                                        IsDeleted = u.IsDeleted,
                                        PhoneNumber = u.PhoneNumber,
                                        UpdatedDate = u.UpdatedDate,
                                        UpdatedBy = u.UpdatedBy,
                                        UserName = u.UserName,
                                        GroupId = (from g1 in _context.AppUserGroups where g1.UserId == u.Id select g1.GroupId).FirstOrDefault(),
                                        EmId = u.EM_ID,
                                        EmName = em.EmName,
                                        Status = u.Status
                                    }).ToListAsync()).AsQueryable();
                return query.Where(x =>
                    x.FullName.ToLower().Contains(keyword.Trim()) ||
                    x.UserName.ToLower().Contains(keyword.Trim()) ||
                    (!string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Contains(keyword.Trim())) ||
                    (!string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToLower().Contains(keyword.Trim())) ||
                    (!string.IsNullOrEmpty(x.EmName) && x.EmName.ToLower().Contains(keyword.Trim()))
                );
            }
        }
    }
}
