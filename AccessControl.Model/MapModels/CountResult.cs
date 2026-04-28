using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.MapModels
{
    public class CountResult
    {
        [Key]
        public int Total { get; set; }
    }
}
