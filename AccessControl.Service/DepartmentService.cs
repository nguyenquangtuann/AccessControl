using AccessControl.Data.Infrastructure.Extentsions;
using AccessControl.Data.Repositories;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IDepartmentService
    {
        Task<IQueryable<Department>> GetAll();
        Task<PagingResult<DepartmentViewModel>> GetAllPaging(int page, int pageSize, string keyword);
        Task<Department> GetById(int id);
        Task<Department> Create(Department department);
        Task<Department> Update(Department department);
        Task<Department> Delete(int id);
    }
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<Department> Create(Department department)
        {
            return await _departmentRepository.CheckContainsAsync(x => x.DepName == department.DepName) ? throw new NameDuplicatedException("Tên phòng ban đã tồn tại") : await _departmentRepository.AddASync(department);
        }

        public async Task<Department> Delete(int id)
        {
            return await _departmentRepository.DeleteAsync(id);
        }

        public async Task<IQueryable<Department>> GetAll()
        {
            return await _departmentRepository.GetAllAsync(x => x.DepStatus == true);
        }

        public async Task<PagingResult<DepartmentViewModel>> GetAllPaging(int page, int pageSize, string keyword)
        {
            return await _departmentRepository.GetListPaging(page, pageSize, keyword);
        }

        public async Task<Department> GetById(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task<Department> Update(Department department)
        {
            return await _departmentRepository.CheckContainsAsync(x => x.DepId != department.DepId && x.DepName == department.DepName) ? throw new NameDuplicatedException("Tên phòng ban đã tồn tại") : await _departmentRepository.UpdateASync(department);
        }
    }
}
