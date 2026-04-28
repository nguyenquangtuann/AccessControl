using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.ViewModels
{
    public class LoginResponseModel
    {
        public string Id { get; set; }
        public int? EmId { get; set; }
        public string Access_token { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public byte[] Image { get; set; }
        public string Phone { get; set; }
    }
}
