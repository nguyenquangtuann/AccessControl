using AccessControl.Model.Abtracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class AppUser : IdentityUser, IAuditable
    {
        public string FullName { get; set; }
        public byte[] Image { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? EM_ID { get; set; }
        int IAuditable.Id { get; set; }

        public async Task<IdentityResult> GenerateUserIdentityAsync(UserManager<AppUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
