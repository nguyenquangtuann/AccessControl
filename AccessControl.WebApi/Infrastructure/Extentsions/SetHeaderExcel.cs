using AccessControl.Model.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;

namespace AccessControl.WebApi.Infrastructure.Extentsions
{
    public class SetHeaderExcel
    {
        public void SetHeader(ref ExcelWorksheet worksheet, HeaderExcel headerExcel)
        {
            int rowIndex = 0;
            int colIndex = 0;
            FileInfo img = new FileInfo(headerExcel.Logo);
            if (img != null)
            {
                ExcelPicture pic = worksheet.Drawings.AddPicture("Sample", (FileInfo)img);
                pic.SetPosition(rowIndex, headerExcel.MarginLogo, colIndex, headerExcel.MarginLogo);
                pic.SetSize(headerExcel.WidthLogo, headerExcel.HeightLogo);
            }

            worksheet.Protection.IsProtected = false;
            worksheet.Protection.AllowSelectLockedCells = false;
            worksheet.Column(1).Width = (headerExcel.WidthLogo + (headerExcel.MarginLogo) * 2) / 7.4;
            worksheet.Row(1).Height = headerExcel.HeightLogo;

            var r1 = worksheet.Cells[1, 2].RichText.Add(headerExcel.CompanyName + "\r\n");
            r1.Bold = true;
            r1.Size = 20;
            r1.FontName = "Times New Roman";
            var r2 = worksheet.Cells[1, 2].RichText.Add(headerExcel.TitleAddress);
            r2.Bold = true;
            r2.Size = 11;
            r2.FontName = "Times New Roman";
            var r3 = worksheet.Cells[1, 2].RichText.Add(headerExcel.CompanyAddress);
            r3.Bold = false;
            r3.FontName = "Times New Roman";


            worksheet.Cells[1, 2].Style.WrapText = true;
            worksheet.Cells[1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }
}
