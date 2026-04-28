using AccessControl.Data.Infrastructure;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<PagingResult<DepartmentViewModel>> GetListPaging(int page, int pageSize, string keyword);
    }
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        private readonly ACSDBContext _context;
        private readonly DbContextOptions<ACSDBContext> _dbContextOptions;
        public DepartmentRepository(ACSDBContext context, DbContextOptions<ACSDBContext> dbContextOptions) : base(context)
        {
            _context = context;
            _dbContextOptions = dbContextOptions;
        }

        public async Task<PagingResult<DepartmentViewModel>> GetListPaging(int page, int pageSize, string keyword)
        {
            using ACSDBContext dbcontext = new(_dbContextOptions);
            using ACSDBContext dbcontext2 = new(_dbContextOptions);
            Task<List<DepartmentViewModel>> lstDepTask;
            Task<int> countTask;
            if (string.IsNullOrEmpty(keyword))
            {
                lstDepTask = (from dep in dbcontext.Departments
                              where dep.DepStatus == true
                              select new DepartmentViewModel
                              {
                                  DepId = dep.DepId,
                                  DepName = dep.DepName,
                                  DepDescription = dep.DepDescription,
                                  DepStatus = dep.DepStatus,
                                  CreatedBy = dep.CreatedBy,
                                  CreatedDate = dep.CreatedDate,
                                  UpdatedBy = dep.UpdatedBy,
                                  UpdatedDate = dep.UpdatedDate,
                                  DeleteBy = dep.DeleteBy,
                                  DeleteDate = dep.DeleteDate
                              }).OrderByDescending(x => x.DepId).Skip(page * pageSize).Take(pageSize).ToListAsync();

                countTask = (from dep in dbcontext2.Departments
                             where dep.DepStatus == true
                             select dep).CountAsync();
            }
            else
            {
                lstDepTask = (from dep in dbcontext.Departments
                              where dep.DepName.ToLower().Contains(keyword.Trim().ToLower())
                              select new DepartmentViewModel
                              {
                                  DepId = dep.DepId,
                                  DepName = dep.DepName,
                                  DepDescription = dep.DepDescription,
                                  DepStatus = dep.DepStatus,
                                  CreatedBy = dep.CreatedBy,
                                  CreatedDate = dep.CreatedDate,
                                  UpdatedBy = dep.UpdatedBy,
                                  UpdatedDate = dep.UpdatedDate,
                                  DeleteBy = dep.DeleteBy,
                                  DeleteDate = dep.DeleteDate
                              }).OrderByDescending(x => x.DepId).Skip(page * pageSize).Take(pageSize).ToListAsync();

                countTask = (from dep in dbcontext2.Departments
                             where dep.DepName.ToLower().Contains(keyword.Trim().ToLower())
                             select dep).CountAsync();
            }
            await Task.WhenAll(lstDepTask, countTask);
            return new PagingResult<DepartmentViewModel>
            {
                Count = await countTask,
                Items = await lstDepTask
            };
        }
    }
}
