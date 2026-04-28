using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using AccessControl.Service;
using AccessControl.WebApi.Infrastructure.Core;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessControl.WebApi.Controllers
{
    [Route("api/access/[controller]")]
    [ApiController]
    [Authorize]
    public class RegencyController : ControllerBase
    {
        #region Initialize
        private readonly IRegencyService _regencyService;
        private readonly IMapper _mapper;
        private ILogger<RegencyController> _logger;
        public RegencyController(IRegencyService regencyService, IMapper mapper, ILogger<RegencyController> logger)
        {
            _regencyService = regencyService;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion Initialize

        #region Properties

        /// <summary>
        /// Lấy danh sách chức vụ
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Regency/getall", "GET");
                var result = await _regencyService.GetAll();
                var map = _mapper.Map<IEnumerable<Regency>, IEnumerable<RegencyViewModel>>(result.OrderByDescending(x => x.RegId));
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách chức vụ theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getbyid")]
        [Authorize(Roles = "ViewReg")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Regency/getbyid", "GET");
                var result = await _regencyService.GetById(id);
                var map = _mapper.Map<Regency, RegencyViewModel>(result);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách theo phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("getlistpaging")]
        [Authorize(Roles = "ViewReg")]
        public async Task<IActionResult> GetListPaging(int page = 0, int pageSize = 100, string? keyword = null)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Regency/getlistpaging", "GET");
                var result = await _regencyService.GetListPaging(page, pageSize, keyword);
                int totalRow = result.Count;
                var lstPaging = result.Items;
                PaginationSet<RegencyViewModel> response = new()
                {
                    Items = lstPaging,
                    Page = page,
                    TotalCount = totalRow,
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
        /// Thêm mới
        /// </summary>
        /// <param name="regencyViewModel"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "CreateReg")]
        public async Task<IActionResult> Create(RegencyViewModel regencyViewModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Regency/create", "POST");
            if (ModelState.IsValid)
            {
                try
                {
                    var map = _mapper.Map<RegencyViewModel, Regency>(regencyViewModel);
                    map.CreatedDate = DateTime.Now;
                    await _regencyService.Create(map);
                    return CreatedAtAction(nameof(Create), new { id = map.RegId }, map);
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
        /// Chỉnh sửa
        /// </summary>
        /// <param name="regencyViewModel"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [Authorize(Roles = "UpdateReg")]
        public async Task<IActionResult> Update(RegencyViewModel regencyViewModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Regency/update", "PUT");
            if (ModelState.IsValid)
            {
                try
                {
                    var map = _mapper.Map<RegencyViewModel, Regency>(regencyViewModel);
                    map.UpdatedDate = DateTime.Now;
                    await _regencyService.Update(map);
                    return CreatedAtAction(nameof(Update), new { id = map.RegId }, map);
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
        /// <param name="deleteModel"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Roles = "DeleteReg")]
        public async Task<IActionResult> Delele(DeleteModel deleteModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Regency/delete", "DELETE");
            if (ModelState.IsValid)
            {
                List<int> result = new List<int>();
                int countSuccess = 0;
                int countFailed = 0;
                try
                {
                    var lstItem = JsonConvert.DeserializeObject<List<int>>(deleteModel.lstId);
                    foreach (var item in lstItem)
                    {
                        var data = await _regencyService.GetById(item);
                        data.DeleteDate = DateTime.Now;
                        data.DeleteBy = deleteModel.userId;
                        data.RegStatus = false;
                        await _regencyService.Update(data);
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

        #endregion Properties
    }
}
