using AccessControl.Data.Infrastructure.Extentsions;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using AccessControl.Service;
using AccessControl.WebApi.Infrastructure.Core;
using AIOAcessControl.WebApi.Infrastructure.Extentsions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessControl.WebApi.Controllers
{
    [Route("api/access/[controller]")]
    [ApiController]
    [Authorize]
    public class AppRoleController : ControllerBase
    {
        #region Initialize
        private readonly IAppRolesService _appRolesService;
        private readonly IMapper _mapper;
        private readonly ILogger<AppRoleController> _logger;
        private readonly IAppRoleGroupService _appRoleGroupService;
        private readonly UserManager<AppUser> _userManager;
        public AppRoleController(IAppRolesService appRolesService, IMapper mapper, ILogger<AppRoleController> logger, IAppRoleGroupService appRoleGroupService, UserManager<AppUser> userManager)
        {
            _appRolesService = appRolesService;
            _mapper = mapper;
            _logger = logger;
            _appRoleGroupService = appRoleGroupService;
            _userManager = userManager;
        }
        #endregion Initialize

        #region Properties

        /// <summary>
        /// Lấy danh sách phân trang quyền người dùng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("getpaging")]
        [Authorize(Roles = "ViewRole")]
        public async Task<IActionResult> GetPaging(int page = 0, int pageSize = 100, string? keyword = null)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/approles/getpaging", "GET");
                int totalRow = 0;
                var model = await _appRolesService.GetAll(keyword);
                totalRow = model.Count();
                var paging = model.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize);
                IEnumerable<AppRoleViewModel> modelVm = _mapper.Map<IEnumerable<AppRole>, IEnumerable<AppRoleViewModel>>(paging);

                PaginationSet<AppRoleViewModel> pagedSet = new PaginationSet<AppRoleViewModel>()
                {
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                    Items = modelVm
                };

                return Ok(pagedSet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get danh sách quyền
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/aioaccesscontrol/approles/getall", "GET");
                var model = await _appRolesService.GetAll();
                IEnumerable<AppRoleViewModel> modelVm = _mapper.Map<IEnumerable<AppRole>, IEnumerable<AppRoleViewModel>>(model.OrderByDescending(x => x.CreatedDate));

                return Ok(modelVm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách quyền tree view
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettreeroles")]
        [Authorize(Roles = "ViewRole")]
        public async Task<IActionResult> GetTreeRoles()
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/approles/gettreeroles", "GET");
                var model = await _appRolesService.GetTreeRoles();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách quyền theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getbyid")]
        [Authorize(Roles = "ViewRole")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/approles/getbyid", "GET");
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(nameof(id) + "Không có giá trị");
                }
                var data = await _appRolesService.GetDetail(id);
                var model = _mapper.Map<AppRole, AppRoleViewModel>(data);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// thêm mới quyền
        /// </summary>
        /// <param name="appRoleViewModel"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "CreateRole")]
        public async Task<IActionResult> Create(AppRoleViewModel appRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/approles/create", "POST");
                    AppRole appRole = new AppRole();
                    appRole.UpdateApplicationRole(appRoleViewModel, "add");
                    appRole.CreatedDate = DateTime.Now;
                    var role = await _appRolesService.Add(appRole);
                    var result = _mapper.Map<AppRole, AppRoleViewModel>(role);
                    return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
                }
                catch (NameDuplicatedException dex)
                {
                    return BadRequest(dex);
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
        /// chỉnh sửa quyền
        /// </summary>
        /// <param name="appRoleViewModel"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [Authorize(Roles = "UpdateRole")]
        public async Task<IActionResult> Update(AppRoleViewModel appRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/approles/upadte", "PUT");
                    var data = await _appRolesService.GetDetail(appRoleViewModel.Id);
                    data.UpdateApplicationRole(appRoleViewModel, "update");
                    data.UpdatedDate = DateTime.Now;
                    var result = await _appRolesService.Update(data);
                    return CreatedAtAction(nameof(Update), result);
                }
                catch (NameDuplicatedException dex)
                {
                    return BadRequest(dex);
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
        /// Xóa quyền
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Roles = "DeleteRole")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/approles/delete", "DELETE");
                int countsuccess = 0;
                int countfail = 0;
                List<string> result = new List<string>();
                List<string> lstItem = JsonConvert.DeserializeObject<List<string>>(id);
                foreach (var roleId in lstItem)
                {
                    try
                    {
                        var role = await _appRolesService.GetDetail(roleId);
                        var groupRole = await _appRoleGroupService.GetGroupListByRoleId(roleId);
                        foreach (var gr in groupRole)
                        {
                            var lstUser = await _appRolesService.GetListUserByGroupId(gr.Id);
                            foreach (var user in lstUser)
                            {
                                var userId = await _userManager.FindByIdAsync(user.Id);
                                await _userManager.RemoveFromRoleAsync(userId, role.Name);
                            }
                        }
                        await _appRolesService.Delete(roleId);
                        countsuccess++;
                    }
                    catch (Exception ex)
                    {
                        countfail++;
                    }
                }
                result.Add("Xóa thành công: " + countsuccess);
                result.Add("Lỗi: " + countfail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy tree view bằng userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("gettreeviewbyuser")]
        public async Task<IActionResult> GetTreeViewByUser(string userId)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/approles/gettreeviewbyuser", "GET");
                var result = await _appRolesService.GetTreeMenuByUserId(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách quyền bằng group id
        /// </summary>
        /// <param name="grId"></param>
        /// <returns></returns>
        [HttpGet("getlistbygroupid")]
        public async Task<IActionResult> GetListByGroupId(int grId)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/assess/approles/getlistbygroupid", "GET");
                var model = await _appRolesService.GetListRoleByGroupId(grId);
                var result = _mapper.Map<IEnumerable<AppRole>, IEnumerable<AppRoleViewModel>>(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion Properties
    }
}
