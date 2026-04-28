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
    public class DepartmentController : ControllerBase
    {
        #region Initialize
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private ILogger<DepartmentController> _logger;
        public DepartmentController(IDepartmentService departmentService, IMapper mapper, ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion Initialize

        #region Properties

        /// <summary>
        /// Lấy danh sách phòng ban
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Department/getall", "GET");
                var result = await _departmentService.GetAll();
                var map = _mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentViewModel>>(result.OrderByDescending(x => x.DepId));
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy phòng ban theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getbyid")]
        [Authorize(Roles = "ViewDep")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Department/getbyid", "GET");
                var result = await _departmentService.GetById(id);
                var map = _mapper.Map<Department, DepartmentViewModel>(result);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách phòng ban phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("getlistpaging")]
        [Authorize(Roles = "ViewDep")]
        public async Task<IActionResult> GetListPaging(int page = 0, int pageSize = 100, string? keyword = null)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Department/getlistpaging", "GET");
                var result = await _departmentService.GetAllPaging(page, pageSize, keyword);
                int totalRow = result.Count;
                var lstPaging = result.Items;
                PaginationSet<DepartmentViewModel> response = new()
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
        /// Thêm mới phòng ban
        /// </summary>
        /// <param name="departmentViewModel"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "CreateDep")]
        public async Task<IActionResult> Create(DepartmentViewModel departmentViewModel)
        {
            _logger.LogInformation("Run endpoint {endpoind} {verb}", "/api/access/Department/create", "POST");
            if (ModelState.IsValid)
            {
                try
                {
                    var map = _mapper.Map<DepartmentViewModel, Department>(departmentViewModel);
                    map.CreatedDate = DateTime.Now;
                    await _departmentService.Create(map);
                    return CreatedAtAction(nameof(Create), new { id = map.DepId }, map);
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
        /// Chỉnh sửa phòng ban
        /// </summary>
        /// <param name="departmentViewModel"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [Authorize(Roles = "UpdateDep")]
        public async Task<IActionResult> Update(DepartmentViewModel departmentViewModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Department/update", "PUT");
            if (ModelState.IsValid)
            {
                try
                {
                    var map = _mapper.Map<DepartmentViewModel, Department>(departmentViewModel);
                    map.UpdatedDate = DateTime.Now;
                    await _departmentService.Update(map);
                    return CreatedAtAction(nameof(Update), new { id = map.DepId }, map);
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
        /// Xóa phòng ban
        /// </summary>
        /// <param name="lstId"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Roles = "DeleteDep")]
        public async Task<IActionResult> Delete(DeleteModel deleteModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/Department/Delete", "DELETE");
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
                        var model = await _departmentService.GetById(item);
                        model.DeleteBy = deleteModel.userId;
                        model.DeleteDate = DateTime.Now;
                        model.DepStatus = false;
                        await _departmentService.Update(model);
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
