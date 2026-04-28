using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.MapModels
{
    public class AppMenuMapping
    {
        public string Id { get; set; }

        public string MenuName { get; set; }

        public string? ParentId { get; set; }

        public string Icon { get; set; }

        public string Link { get; set; }

        public string ActiveLink { get; set; }

        public List<AppMenuMapping> Childrens { get; set; }
    }

    public class AppMenuMappingSQL
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string? ParentId { get; set; }

        public string Icon { get; set; }

        public string Link { get; set; }

        public string ActiveLink { get; set; }

        public int OrderBy { get; set; }
    }
}
