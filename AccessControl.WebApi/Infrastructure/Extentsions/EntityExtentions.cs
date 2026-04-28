using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using AccessControl.WebApi.Common.Ultilities;

namespace AIOAcessControl.WebApi.Infrastructure.Extentsions
{
    public static class EntityExtentions
    {
        public static void UpdateGroup(this AppGroup group, AppGroupViewModel groupViewModel)
        {
            group.Id = groupViewModel.Id;
            group.Name = groupViewModel.Name;
            group.GroupCode = groupViewModel.GroupCode;
            group.IsDeleted = groupViewModel.IsDeleted;
            group.CreatedBy = groupViewModel.CreatedBy;
            group.UpdatedBy = groupViewModel.UpdatedBy;
        }
        public static void UpdateUser(this AppUser appUser, AppUserViewModel appUserViewModel, string action = "add")
        {
            if (action == "add")
                appUser.Id = Guid.NewGuid().ToString();
            else
            {
                appUser.Id = appUserViewModel.Id;
            }
            appUser.Email = appUserViewModel.Email;
            appUser.UserName = appUserViewModel.UserName;
            appUser.PhoneNumber = appUserViewModel.PhoneNumber;
            appUser.NormalizedUserName = appUserViewModel.UserName;
            appUser.Image = appUserViewModel.Image.Base64ToJpeg();
            appUser.FullName = appUserViewModel.FullName;
            appUser.IsDeleted = appUserViewModel.IsDeleted;
            appUser.CreatedBy = appUserViewModel.CreatedBy;
            appUser.UpdatedBy = appUserViewModel.UpdatedBy;
            appUser.EM_ID = appUserViewModel.EmId;
            appUser.Status = appUserViewModel.Status;
        }
        public static void UpdateApplicationRole(this AppRole appRole, AppRoleViewModel appRoleViewModel, string action = "add")
        {
            if (action == "update")
                appRole.Id = appRoleViewModel.Id;
            else
                appRole.Id = Guid.NewGuid().ToString();
            appRole.Name = appRoleViewModel.Name;
            appRole.Description = appRoleViewModel.Description;
            appRole.NormalizedName = appRoleViewModel.Name;
            appRole.ParentId = appRoleViewModel.ParentId;
            appRole.IsDeleted = appRoleViewModel.IsDeleted;
            appRole.CreatedBy = appRoleViewModel.CreatedBy;
            appRole.UpdatedBy = appRoleViewModel.UpdatedBy;
            appRole.Icon = appRoleViewModel.Icon;
            appRole.Link = appRoleViewModel.Link;
            appRole.ActiveLink = appRoleViewModel.ActiveLink;
            appRole.OrderBy = appRoleViewModel.OrderBy;
        }
    }
}