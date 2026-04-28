using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.Models
{
    public class AppUserGroup
    {
        [StringLength(450)]
        [Key]
        [Column(Order = 1)]
        public string? UserId { set; get; }
        [Column(Order = 2)]
        public int? GroupId { set; get; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { set; get; }

        [ForeignKey("GroupId")]
        public virtual AppGroup AppGroup { set; get; }
    }
}
