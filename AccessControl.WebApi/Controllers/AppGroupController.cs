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
    public class AppGroupController : ControllerBase
    {
        #region Initialize
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppGroupService _appGroupService;
        private readonly IAppRolesService _appRolesService;
        private readonly IMapper _mapper;
        private ILogger<AppGroupController> _logger;
        private readonly IAppRoleGroupService _appRoleGroupService;
        private readonly IAppUserGroupService _appUserGroupService;
        public AppGroupController(UserManager<AppUser> userManager, IAppGroupService appGroupService, IAppRolesService appRolesService, IMapper mapper, ILogger<AppGroupController> logger, IAppRoleGroupService appRoleGroupService, IAppUserGroupService appUserGroupService)
        {
            _userManager = userManager;
            _appGroupService = appGroupService;
            _appRolesService = appRolesService;
            _mapper = mapper;
            _logger = logger;
            _appRoleGroupService = appRoleGroupService;
            _appUserGroupService = appUserGroupService;
        }

        #endregion Initialize

        #region Properties

        /// <summary>
        /// Lấy danh sách phân nhóm người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appgroup/getall", "GET");
                var data = await _appGroupService.GetAll();
                var map = _mapper.Map<IEnumerable<AppGroup>, IEnumerable<AppGroupViewModel>>(data.OrderByDescending(x => x.Id));
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy phân nhóm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getbyid")]
        [Authorize(Roles = "ViewGroup")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appgroup/getbyid", "GET");
                var data = await _appGroupService.GetById(id);
                var map = _mapper.Map<AppGroup, AppGroupViewModel>(data);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy phân nhóm phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("getpaging")]
        [Authorize(Roles = "ViewGroup")]
        public async Task<IActionResult> GetPaging(int page = 0, int pageSize = 100, string? keyword = null)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appgroup/getpaging", "GET");
                var data = await _appGroupService.GetAll(keyword);
                var result = _mapper.Map<IEnumerable<AppGroup>, IEnumerable<AppGroupViewModel>>(data.OrderByDescending(x => x.Id).Skip(page * pageSize).Take(pageSize));
                int totalRow = 0;
                totalRow = data.Count();
                var paging = new PaginationSet<AppGroupViewModel>()
                {
                    Items = result,
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
                };
                return Ok(paging);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thêm mới phân nhóm
        /// </summary>
        /// <param name="appGroupViewModel"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "CreateGroup")]
        public async Task<IActionResult> Create(AppGroupViewModel appGroupViewModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appgroup/create", "POST");
            if (ModelState.IsValid)
            {
                var newAppGroup = new AppGroup();
                newAppGroup.UpdateGroup(appGroupViewModel);
                newAppGroup.CreatedDate = DateTime.Now;
                try
                {
                    var appGroup = await _appGroupService.Add(newAppGroup);

                    var listRoleGroup = new List<AppRoleGroup>();
                    if (appGroupViewModel.Roles != null)
                    {
                        foreach (var role in appGroupViewModel.Roles)
                        {
                            listRoleGroup.Add(new AppRoleGroup()
                            {
                                GroupId = appGroup.Id,
                                RoleId = role.Id
                            });
                        }
                        await _appRolesService.AddRolesToGroup(listRoleGroup, appGroup.Id);
                    }
                    return CreatedAtAction(nameof(Create), new { id = newAppGroup.Id }, newAppGroup);
                }
                catch (NameDuplicatedException dex)
                {
                    return BadRequest(dex);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Chỉnh sửa phân nhóm
        /// </summary>
        /// <param name="appGroupViewModel"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [Authorize(Roles = "UpdateGroup")]
        public async Task<IActionResult> Update(AppGroupViewModel appGroupViewModel)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appgroup/update", "PUT");
            if (ModelState.IsValid)
            {
                var newAppGroup = await _appGroupService.GetById(appGroupViewModel.Id);
                newAppGroup.UpdateGroup(appGroupViewModel);
                newAppGroup.UpdatedDate = DateTime.Now;
                try
                {
                    var appGroup = await _appGroupService.Update(newAppGroup);

                    // Xóa quyền người dùng
                    var listRole = await _appRolesService.GetListRoleByGroupId(appGroup.Id);
                    var listUsers = await _appRolesService.GetListUserByGroupId(appGroup.Id);
                    foreach (var user in listUsers)
                    {
                        foreach (var role in listRole)
                        {
                            var userId = await _userManager.FindByIdAsync(user.Id);
                            await _userManager.RemoveFromRoleAsync(userId, role.Name);
                        }
                    }

                    var listRoleGroup = new List<AppRoleGroup>();
                    if (appGroupViewModel.Roles != null)
                    {
                        foreach (var role in appGroupViewModel.Roles)
                        {
                            listRoleGroup.Add(new AppRoleGroup()
                            {
                                GroupId = appGroup.Id,
                                RoleId = role.Id
                            });
                        }
                    }
                    await _appRolesService.AddRolesToGroup(listRoleGroup, appGroup.Id);

                    // thêm quyền cho tài khoản
                    listRole = await _appRolesService.GetListRoleByGroupId(appGroup.Id);
                    foreach (var user in listUsers)
                    {
                        var userId = await _userManager.FindByIdAsync(user.Id);
                        await _userManager.AddToRolesAsync(userId, listRole.Select(x => x.Name));
                    }

                    return CreatedAtAction(nameof(Update), new { id = appGroupViewModel.Id }, appGroupViewModel);
                }
                catch (NameDuplicatedException dex)
                {
                    return BadRequest(dex);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Xóa phân nhóm
        /// </summary>
        /// <param name="lstId"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Roles = "DeleteGroup")]
        public async Task<IActionResult> Delete(string lstId)
        {
            _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appgroup/delete", "DELETE");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    int countSuccess = 0;
                    int countError = 0;
                    List<string> result = new List<string>();
                    var listItem = JsonConvert.DeserializeObject<List<int>>(lstId);
                    foreach (var id in listItem)
                    {
                        try
                        {
                            var listRole = await _appRolesService.GetListRoleByGroupId(id);
                            var listUsers = (await _appRolesService.GetListUserByGroupId(id)).ToList();
                            foreach (var user in listUsers)
                            {
                                foreach (var role in listRole)
                                {
                                    var userId = await _userManager.FindByIdAsync(user.Id);
                                    await _userManager.RemoveFromRoleAsync(userId, role.Name);
                                }
                            }
                            await _appRoleGroupService.DeleteMultipleAppRoleGroupByGroupId(id);
                            await _appUserGroupService.DeleteMultipleByGroupId(id);
                            await _appGroupService.Delete(id);
                            foreach (var user in listUsers)
                            {
                                var groupListContainUser = await _appUserGroupService.GetAppGroupByUserId(user.Id);
                                var userId = await _userManager.FindByIdAsync(user.Id);
                                foreach (var gr in groupListContainUser)
                                {
                                    var roles = await _appRolesService.GetListRoleByGroupId(gr.Id);
                                    await _userManager.AddToRolesAsync(userId, roles.Select(x => x.Name));
                                }
                            }
                            countSuccess++;
                        }
                        catch (Exception)
                        {
                            countError++;
                        }
                    }
                    result.Add("Xóa thành công: " + countSuccess);
                    result.Add("Lỗi" + countError);

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        /// <summary>
        /// Lấy danh sách nhóm theo id User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("getlistgroupbyuser")]
        [Authorize(Roles = "ViewGroup")]
        public async Task<IActionResult> GetListGroupByUser(string userId)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appgroup/getlistgroupbyuser", "GET");
                var result = await _appGroupService.GetListGroupByUserId(userId);
                var mapping = _mapper.Map<IEnumerable<AppGroup>, IEnumerable<AppGroupViewModel>>(result);
                return Ok(mapping);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Danh sách phân quyền treeview
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettreeroles")]
        public async Task<IActionResult> GetTreeRoles()
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/approle/gettreeroles", "GET");
                var model = await _appRolesService.GetTreeRoles();

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        #endregion Properties
    }
}
