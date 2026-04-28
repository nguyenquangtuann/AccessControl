using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.ViewModels
{
    public class AppUserViewModel : IdentityUser
    {
        public string? FullName { get; set; }
        public byte[]? Image { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { set; get; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? EmId { get; set; }
        public string? EmName { get; set; }
        public int? GroupId { get; set; }
        public bool Status { get; set; }
    }
}
