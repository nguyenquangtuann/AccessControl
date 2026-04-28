using AutoMapper;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;

namespace AccessControl.WebApi.Infrastructure.Extentsions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppGroup, AppGroupViewModel>().ReverseMap();
            CreateMap<AppUser, AppUserViewModel>().ReverseMap();
            CreateMap<AppRole, AppRoleViewModel>().ReverseMap();
            CreateMap<Department, DepartmentViewModel>().ReverseMap();
            CreateMap<Regency, RegencyViewModel>().ReverseMap();
            CreateMap<Employee, EmployeeViewModel>().ReverseMap();
        }
    }
}