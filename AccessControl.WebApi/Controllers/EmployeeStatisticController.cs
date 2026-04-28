using AccessControl.Model.ExportExModel;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using AccessControl.Service;
using AccessControl.WebApi.Infrastructure.Extentsions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace AccessControl.WebApi.Controllers
{
    [Route("api/access/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeStatisticController : ControllerBase
    {
        #region Initialize
        private readonly IEmployeeStatisticService _employeeStatisticService;
        private readonly IMapper _mapper;
        private readonly IHeaderExcelService _excelService;
        private readonly IWebHostEnvironment _environment;

        public EmployeeStatisticController(IEmployeeStatisticService employeeStatisticService, IMapper mapper, IHeaderExcelService excelService, IWebHostEnvironment environment)
        {
            _employeeStatisticService = employeeStatisticService;
            _mapper = mapper;
            _excelService = excelService;
            _environment = environment;
        }
        #endregion Initialize

        #region Properties

        /// <summary>
        /// Thống kê danh sách nhân viên
        /// </summary>
        /// <param name="depId"></param>
        /// <param name="regId"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("getall")]
        [Authorize(Roles = "ViewEmStatistic")]
        public async Task<IActionResult> GetAllPaging(string depId, string regId, int status, int page = 0, int pageSize = 100)
        {
            try
            {
                List<int>? lstDepId = JsonConvert.DeserializeObject<List<int>>(depId);
                List<int>? lstRegId = JsonConvert.DeserializeObject<List<int>>(regId);
                var result = await _employeeStatisticService.GetAllPaging(lstDepId, lstRegId, status, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Xuất excel danh sách nhân viên
        /// </summary>
        /// <param name="depId"></param>
        /// <param name="regId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost("exportex")]
        [Authorize(Roles = "ExEmStatistic")]
        public async Task<IActionResult> ExportEx(string depId, string regId, int status)
        {
            try
            {
                List<int>? lstDepId = JsonConvert.DeserializeObject<List<int>>(depId);
                List<int>? lstRegId = JsonConvert.DeserializeObject<List<int>>(regId);
                var data = await _employeeStatisticService.GetAll(lstDepId, lstRegId, status);

                string sWebRootFolder = _environment.WebRootPath + "/SaveFileExcel";
                string fileName = string.Concat("Thong_ke_nhan_vien" + DateTime.Now.ToString("yyyyMMddhhmm") + ".xlsx");

                string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, "SaveFileExcel/" + fileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, fileName));

                HeaderExcel headerExcel = _excelService.GetTop1();

                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(Path.Combine(sWebRootFolder, fileName));
                }

                int i = 1;
                List<ExportExcelEmployeeStatistic> lstdata = new List<ExportExcelEmployeeStatistic>();
                foreach (var item in data)
                {
                    ExportExcelEmployeeStatistic dt = new ExportExcelEmployeeStatistic();
                    dt.STT = i++;
                    dt.EmName = item.EmName;
                    dt.EmCode = item.EmCode;
                    dt.EmGender = item.EmGender == "M" ? "Nam" : "Nữ";
                    dt.EmBirthdate = item.EmBirthdate?.ToString("dd-MM-yyyy");
                    dt.EmAddress = item.EmAddress;
                    dt.EmPhone = item.EmPhone;
                    dt.DepName = item.DepName;
                    dt.RegName = item.RegName;
                    dt.EmStatus = item.EmStatus == true ? "Hoạt động" : "Đã xóa";
                    lstdata.Add(dt);
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(file))
                {
                    ExcelWorksheet ws;
                    ws = pck.Workbook.Worksheets.Add("Thống kê danh sách nhân viên");
                    SetHeaderExcel setHeader = new SetHeaderExcel();
                    setHeader.SetHeader(ref ws, headerExcel);
                    ws.Cells[1, 2, 1, 10].Merge = true;
                    using (ExcelRange Rng = ws.Cells[2, 1, 2, 10])
                    {
                        Rng.Merge = true;
                        Rng.Value = "THỐNG KÊ DANH SÁCH NHÂN VIÊN";
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Size = 17;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Style.Font.Name = "Times New Roman";
                    }
                    using (ExcelRange Rng = ws.Cells[3, 1, 3, 10])
                    {
                        Rng.Merge = true;
                        Rng.Value = "Ngày xuất thống kê : " + DateTime.Now.ToString("dd-MM-yyyy");
                        Rng.Style.Font.Size = 12;
                        Rng.Style.Font.Italic = true;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Style.Font.Name = "Times New Roman";
                    }
                    using (ExcelRange Rng = ws.Cells[4, 1, lstdata.Count() + 4, 10])
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Style.Font.Name = "Times New Roman";
                        Rng.AutoFilter = false;
                    }
                    using (ExcelRange Rng = ws.Cells[5, 2, lstdata.Count() + 5, 2])
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        Rng.Style.Font.Name = "Times New Roman";
                        Rng.AutoFilter = false;
                    }
                    using (ExcelRange Rng = ws.Cells[5, 6, lstdata.Count() + 5, 6])
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        Rng.Style.Font.Name = "Times New Roman";
                        Rng.AutoFilter = false;
                    }
                    using (ExcelRange Rng = ws.Cells[5, 8, lstdata.Count() + 5, 9])
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        Rng.Style.Font.Name = "Times New Roman";
                        Rng.AutoFilter = false;
                    }
                    ws.Cells["A4"].LoadFromCollection(lstdata, true, TableStyles.Light18);
                    ws.Column(2).Width = 25;
                    ws.Column(3).Width = 18;
                    ws.Column(4).Width = 12;
                    ws.Column(5).Width = 16;
                    ws.Column(6).Width = 35;
                    ws.Column(7).Width = 14;
                    ws.Column(8).Width = 25;
                    ws.Column(9).Width = 25;
                    ws.Column(10).Width = 18;
                    pck.Save();
                }
                ExcelResponse excelResponse = new ExcelResponse(URL);
                return Ok(excelResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion Properties
    }
}
