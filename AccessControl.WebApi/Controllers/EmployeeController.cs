using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using AccessControl.Service;
using AccessControl.WebApi.Infrastructure.Core;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using Newtonsoft.Json;

namespace AccessControl.WebApi.Controllers
{
    [Route("api/access/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        #region Initialize
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private ILogger<EmployeeController> _logger;
        private readonly IWebHostEnvironment _environment;
        public EmployeeController(IEmployeeService employeeService, IMapper mapper, ILogger<EmployeeController> logger, IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
        }

        #endregion Initialize

        #region Properties

        /// <summary>
        /// Lấy danh sách nhân viên
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Employee/getall", "GET");
                var result = await _employeeService.GetAll();
                var map = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(result);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getbyid")]
        [Authorize(Roles = "ViewEm")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Employee/getbyid", "GET");
                var result = await _employeeService.GetById(id);
                var map = _mapper.Map<Employee, EmployeeViewModel>(result);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("getlistpaging")]
        [Authorize(Roles = "ViewEm")]
        public async Task<IActionResult> GetListPaging(int page = 0, int pageSize = 100, string? keyword = null)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Employee/getlistpaging", "GET");
                var result = await _employeeService.GetListPaging(page, pageSize, keyword);
                int totalRow = result.Count;
                var lstPaging = result.Items;
                PaginationSet<EmployeeMapping> response = new()
                {
                    TotalCount = totalRow,
                    Items = lstPaging,
                    Page = page,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thêm mới nhân viên
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "CreateEm")]
        public async Task<IActionResult> Create(EmployeeViewModel employeeViewModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Employee/create", "POST");
            if (ModelState.IsValid)
            {
                try
                {
                    string filePath = _environment.WebRootPath + "/ResizeFace";
                    var map = _mapper.Map<EmployeeViewModel, Employee>(employeeViewModel);
                    map.CreatedDate = DateTime.Now;
                    map.EditStatus = true;
                    map.EmImage = ConvertImg(filePath, map.EmImage);
                    await _employeeService.Create(map);
                    return CreatedAtAction(nameof(Create), new { id = map.EmId }, map);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// chỉnh sửa
        /// </summary>
        /// <param name="employeeViewModel"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [Authorize(Roles = "UpdateEm")]
        public async Task<IActionResult> Update(EmployeeViewModel employeeViewModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Employee/update", "PUT");
            if (ModelState.IsValid)
            {
                try
                {
                    string filePath = _environment.WebRootPath + "/ResizeFace";
                    var map = _mapper.Map<EmployeeViewModel, Employee>(employeeViewModel);
                    map.UpdatedDate = DateTime.Now;
                    map.EditStatus = true;
                    map.EmImage = ConvertImg(filePath, map.EmImage);
                    await _employeeService.Update(map);
                    return CreatedAtAction(nameof(Update), new { id = map.EmId }, map);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Roles = "DeleteEm")]
        public async Task<IActionResult> Delete(DeleteModel model)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Employee", "DELETE");
            if (ModelState.IsValid)
            {
                List<int> result = new List<int>();
                int countSuccess = 0;
                int countFailed = 0;
                try
                {
                    var lstItem = JsonConvert.DeserializeObject<List<int>>(model.lstId);
                    foreach (var item in lstItem)
                    {
                        var data = await _employeeService.GetById(item);
                        data.EditStatus = true;
                        data.EmStatus = false;
                        data.DeleteBy = model.userId;
                        data.DeleteDate = DateTime.Now;
                        await _employeeService.Update(data);
                        countSuccess++;
                    }
                }
                catch (Exception ex)
                {
                    countFailed++;
                }
                result.Add(countSuccess);
                result.Add(countFailed);
                return Ok(result);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        private byte[] ConvertImg(string filePath, byte[] byteImg)
        {
            try
            {
                if (byteImg == null || byteImg.Length == 0)
                {
                    return byteImg;
                }
                byte[] result = byteImg;
                if (!Directory.Exists(filePath))
                {
                    _ = Directory.CreateDirectory(filePath);
                }
                DateTime now = DateTime.Now;
                string fileName = now.ToString("yyyyMMddHHmmss") + ".jpg";
                string fullPath = Path.Combine(filePath, fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                System.IO.File.WriteAllBytes(fullPath, byteImg);
                bool checkExist = System.IO.File.Exists(fullPath);
                if (checkExist)
                {
                    Image img;
                    using (Stream stream = System.IO.File.OpenRead(fullPath))
                    {
                        img = Image.FromStream(stream);
                    }
                    if (!img.RawFormat.Equals(ImageFormat.Jpeg))
                    {
                        img.Save(fullPath, ImageFormat.Jpeg);
                        result = System.IO.File.ReadAllBytes(fullPath);
                    }

                    using (Image i = Image.FromFile(fullPath))
                    {
                        RotateImageIfNeeded(i);

                        int fullHDWidth = 2304;
                        int fullHDHeight = 2304;

                        // Kiểm tra độ phân giải ảnh
                        if (i.Width > fullHDWidth || i.Height > fullHDHeight)
                        {
                            Console.WriteLine("Ảnh lớn hơn Full HD, bắt đầu giảm kích thước...");

                            // Tính tỷ lệ giảm kích thước để giữ nguyên aspect ratio
                            float ratioX = (float)fullHDWidth / i.Width;
                            float ratioY = (float)fullHDHeight / i.Height;
                            float ratio = Math.Min(ratioX, ratioY); // Chọn tỷ lệ nhỏ hơn để giữ aspect ratio

                            int newWidth = (int)(i.Width * ratio);
                            int newHeight = (int)(i.Height * ratio);

                            // Giảm kích thước ảnh
                            using Bitmap resizedImage = new(newWidth, newHeight);
                            using Graphics g = Graphics.FromImage(resizedImage);
                            // Thiết lập các thuộc tính để đảm bảo chất lượng ảnh tốt nhất
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;

                            // Vẽ ảnh với kích thước mới
                            g.DrawImage(i, 0, 0, newWidth, newHeight);

                            // Lưu ảnh mới với chất lượng cao
                            i.Dispose();
                            resizedImage.Save(fullPath, ImageFormat.Jpeg);

                        }
                    }

                    result = System.IO.File.ReadAllBytes(fullPath);
                    long maxFileSize = 512 * 1024; // 768KB , 512
                    if (result.Length > maxFileSize)
                    {
                        byte[] jpgBytes;
                        using (Image originalImage = Image.FromFile(fullPath))
                        {
                            long quality = 100L;
                            do
                            {
                                using (MemoryStream ms = new())
                                {
                                    EncoderParameters encoderParams = new(1);
                                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                                    ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
                                    originalImage.Save(ms, jpegCodec, encoderParams);
                                    jpgBytes = ms.ToArray();
                                }
                                quality -= 5;

                            } while (jpgBytes.Length > maxFileSize && quality > 0);
                        }
                        System.IO.File.WriteAllBytes(fullPath, jpgBytes);
                        result = System.IO.File.ReadAllBytes(fullPath);
                    }
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    catch { }
                }
                return result;
            }
            catch (Exception)
            {
                return byteImg;
            }
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                {
                    return codec;
                }
            }
            return null;
        }

        private void RotateImageIfNeeded(Image img)
        {
            // Mã số Exif cho orientation là 0x112 (274)
            const int ExifOrientationId = 0x112;

            if (img.PropertyIdList.Contains(ExifOrientationId))
            {
                // Lấy giá trị của Exif orientation
                int orientation = img.GetPropertyItem(ExifOrientationId).Value[0];

                // Dựa trên giá trị của Exif orientation, xoay ảnh cho đúng hướng
                switch (orientation)
                {
                    case 1:
                        // Normal, không cần xoay
                        break;
                    case 2:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipX); // Flip horizontal
                        break;
                    case 3:
                        img.RotateFlip(RotateFlipType.Rotate180FlipNone); // Rotate 180
                        break;
                    case 4:
                        img.RotateFlip(RotateFlipType.Rotate180FlipX); // Rotate 180 and flip horizontal
                        break;
                    case 5:
                        img.RotateFlip(RotateFlipType.Rotate90FlipX); // Rotate 90 and flip horizontal
                        break;
                    case 6:
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone); // Rotate 90
                        break;
                    case 7:
                        img.RotateFlip(RotateFlipType.Rotate270FlipX); // Rotate 270 and flip horizontal
                        break;
                    case 8:
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone); // Rotate 270
                        break;
                }

                // Xóa orientation để tránh ảnh hưởng khi lưu lại
                img.RemovePropertyItem(ExifOrientationId);
            }
        }

        #endregion Properties
    }
}
