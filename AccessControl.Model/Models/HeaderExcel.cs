using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class HeaderExcel
    {
        public int Id { get; set; }
        public string? Logo { get; set; }
        public int WidthLogo { get; set; }
        public int HeightLogo { get; set; }
        public int MarginLogo { get; set; }
        public string? CompanyName { get; set; }
        public string? TitleAddress { get; set; }
        public string? CompanyAddress { get; set; }
    }
}
