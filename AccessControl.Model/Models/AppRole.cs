using AccessControl.Model.Abtracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class AppRole : IdentityRole, IAuditable
    {
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public string? Description { get; set; }
        public string? ParentId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? Icon { get; set; }
        public string? Link { get; set; }
        public string? ActiveLink { get; set; }
        public int? OrderBy { get; set; }
        int IAuditable.Id { get; set; }
    }
}
