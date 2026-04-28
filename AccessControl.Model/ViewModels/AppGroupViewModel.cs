using AccessControl.Model.Abtracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.ViewModels
{
    public class AppGroupViewModel : Auditable
    {
        public string? GroupCode { get; set; }

        public string? Name { get; set; }

        public virtual List<AppRoleViewModel>? Roles { get; set; }
    }
}
