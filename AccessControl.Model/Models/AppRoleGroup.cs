using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.Models
{
    public class AppRoleGroup
    {
        [Key]
        [Column(Order = 1)]
        public int GroupId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 2)]
        [StringLength(450)]
        public string RoleId { set; get; }

        [ForeignKey("RoleId")]
        public virtual AppRole AppRole { set; get; }

        [ForeignKey("GroupId")]
        public virtual AppGroup AppGroup { set; get; }
    }
}
