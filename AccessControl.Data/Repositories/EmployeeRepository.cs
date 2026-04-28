using AccessControl.Data.Infrastructure;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<PagingResult<EmployeeMapping>> GetListPaging(int page, int pageSize, string keyword);
    }
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        private readonly ACSDBContext _context;
        private readonly DbContextOptions<ACSDBContext> _dbContextOptions;
        public EmployeeRepository(ACSDBContext context, DbContextOptions<ACSDBContext> dbContextOptions) : base(context)
        {
            _context = context;
            _dbContextOptions = dbContextOptions;
        }

        public async Task<PagingResult<EmployeeMapping>> GetListPaging(int page, int pageSize, string keyword)
        {
            using ACSDBContext context = new(_dbContextOptions);
            using ACSDBContext context2 = new(_dbContextOptions);
            Task<List<EmployeeMapping>> dataTask;
            Task<int> countTask;
            if (string.IsNullOrEmpty(keyword))
            {
                dataTask = (from em in context.Employees
                            join reg in context.Regencies on em.RegId equals reg.RegId into emreg
                            from reg in emreg.DefaultIfEmpty()
                            join dep in context.Departments on em.DepId equals dep.DepId into emdep
                            from dep in emdep.DefaultIfEmpty()
                            where em.EmStatus == true
                            select new EmployeeMapping
                            {
                                EmId = em.EmId,
                                RegId = reg.RegId,
                                RegName = reg.RegName,
                                DepId = dep.DepId,
                                DepName = dep.DepName,
                                EmName = em.EmName,
                                EmCode = em.EmCode,
                                EmGender = em.EmGender,
                                EmBirthdate = em.EmBirthdate,
                                EmIdentityNumber = em.EmIdentityNumber,
                                EmAddress = em.EmAddress,
                                EmPhone = em.EmPhone,
                                EmEmail = em.EmEmail,
                                EmImage = em.EmImage,
                                EmStatus = em.EmStatus,
                                EditStatus = em.EditStatus,
                                DevIdSynchronized = em.DevIdSynchronized,
                                CreatedBy = em.CreatedBy,
                                CreatedDate = em.CreatedDate,
                                UpdatedBy = em.UpdatedBy,
                                UpdatedDate = em.UpdatedDate,
                                DeleteBy = em.DeleteBy,
                                DeleteDate = em.DeleteDate
                            }).OrderByDescending(x => x.EmId).Skip(page * pageSize).Take(pageSize).ToListAsync();

                countTask = (from em in context2.Employees
                             where em.EmStatus == true
                             select em).CountAsync();
            }
            else
            {
                dataTask = (from em in context.Employees
                            join reg in context.Regencies on em.RegId equals reg.RegId into emreg
                            from reg in emreg.DefaultIfEmpty()
                            join dep in context.Departments on em.DepId equals dep.DepId into emdep
                            from dep in emdep.DefaultIfEmpty()
                            where em.EmName.ToLower().Contains(keyword.Trim().ToLower()) || em.EmCode.ToLower().Contains(keyword.Trim().ToLower()) || reg.RegName.ToLower().Contains(keyword.Trim().ToLower()) || dep.DepName.ToLower().Contains(keyword.Trim().ToLower())
                            select new EmployeeMapping
                            {
                                EmId = em.EmId,
                                RegId = reg.RegId,
                                RegName = reg.RegName,
                                DepId = dep.DepId,
                                DepName = dep.DepName,
                                EmName = em.EmName,
                                EmCode = em.EmCode,
                                EmGender = em.EmGender,
                                EmBirthdate = em.EmBirthdate,
                                EmIdentityNumber = em.EmIdentityNumber,
                                EmAddress = em.EmAddress,
                                EmPhone = em.EmPhone,
                                EmEmail = em.EmEmail,
                                EmImage = em.EmImage,
                                EmStatus = em.EmStatus,
                                EditStatus = em.EditStatus,
                                DevIdSynchronized = em.DevIdSynchronized,
                                CreatedBy = em.CreatedBy,
                                CreatedDate = em.CreatedDate,
                                UpdatedBy = em.UpdatedBy,
                                UpdatedDate = em.UpdatedDate,
                                DeleteBy = em.DeleteBy,
                                DeleteDate = em.DeleteDate
                            }).OrderByDescending(x => x.EmId).Skip(page * pageSize).Take(pageSize).ToListAsync();

                countTask = (from em in context2.Employees
                             join reg in context2.Regencies on em.RegId equals reg.RegId into emreg
                             from reg in emreg.DefaultIfEmpty()
                             join dep in context2.Departments on em.DepId equals dep.DepId into emdep
                             from dep in emdep.DefaultIfEmpty()
                             where em.EmName.ToLower().Contains(keyword.Trim().ToLower()) || em.EmCode.ToLower().Contains(keyword.Trim().ToLower()) || reg.RegName.ToLower().Contains(keyword.Trim().ToLower()) || dep.DepName.ToLower().Contains(keyword.Trim().ToLower())
                             select em).CountAsync();
            }

            await Task.WhenAll(dataTask, countTask);

            return new PagingResult<EmployeeMapping>
            {
                Items = await dataTask,
                Count = await countTask
            };
        }
    }
}
