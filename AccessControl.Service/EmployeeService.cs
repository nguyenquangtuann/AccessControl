using AccessControl.Data.Infrastructure.Extentsions;
using AccessControl.Data.Repositories;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IEmployeeService
    {
        Task<IQueryable<Employee>> GetAll();
        Task<Employee> GetById(int id);
        Task<PagingResult<EmployeeMapping>> GetListPaging(int page, int pageSize, string keyword);
        Task<Employee> Create(Employee employee);
        Task<Employee> Update(Employee employee);
        Task<Employee> DeleteById(int id);
    }
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> Create(Employee employee)
        {
            return await _employeeRepository.CheckContainsAsync(x => x.EmCode == employee.EmCode) ? throw new NameDuplicatedException("Mã nhân viên đã tồn tại") : await _employeeRepository.AddASync(employee);
        }

        public async Task<Employee> DeleteById(int id)
        {
            return await _employeeRepository.DeleteAsync(id);
        }

        public async Task<IQueryable<Employee>> GetAll()
        {
            return await _employeeRepository.GetAllAsync(x => x.EmStatus == true);
        }

        public async Task<Employee> GetById(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<PagingResult<EmployeeMapping>> GetListPaging(int page, int pageSize, string keyword)
        {
            return await _employeeRepository.GetListPaging(page, pageSize, keyword);
        }

        public async Task<Employee> Update(Employee employee)
        {
            return await _employeeRepository.CheckContainsAsync(x => x.EmCode == employee.EmCode && x.EmId != employee.EmId) ? throw new NameDuplicatedException("Mã nhân viên đã tồn tại!") : await _employeeRepository.UpdateASync(employee);
        }
    }
}
