using AccessControl.Data.Infrastructure.Extentsions;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using AccessControl.Service;
using AccessControl.WebApi.Infrastructure.Core;
using AIOAcessControl.WebApi.Infrastructure.Extentsions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccessControl.WebApi.Controllers
{
    [Route("api/access/[controller]")]
    [ApiController]
    [Authorize]
    public class AppUserController : ControllerBase
    {
        #region Initialize
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserService _applicationUserService;
        private readonly IMapper _mapper;
        private readonly IAppGroupService _applicationGroupService;
        private readonly IAppRolesService _applicationRoleService;
        protected readonly ILogger<AppUserController> _logger;

        public AppUserController(UserManager<AppUser> userManager, IAppUserService applicationUserService, IMapper mapper, IAppGroupService applicationGroupService, IAppRolesService applicationRoleService, ILogger<AppUserController> logger)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _mapper = mapper;
            _applicationGroupService = applicationGroupService;
            _applicationRoleService = applicationRoleService;
            _logger = logger;
        }
        #endregion Initialize

        #region Properties

        /// <summary>
        /// Lấy danh sách tài khoản đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appuser/getall", "GET");
                var result = await _applicationUserService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// lấy danh sách tài khoản đăng nhập theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getbyid")]
        [Authorize(Roles = "ViewUser")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appuser/getbyid", "GET");
                var appuser = await _userManager.FindByIdAsync(id);
                var result = _mapper.Map<AppUser, AppUserViewModel>(appuser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách tài khoản phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("getpaging")]
        [Authorize(Roles = "ViewUser")]
        public async Task<IActionResult> GetPaging(int page = 0, int pageSize = 10, string? keyword = null)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appuser/getpaging", "GET");
                var data = await _applicationUserService.GetAllByMappingAsync(keyword);
                int totalRow = 0;
                totalRow = data.Count();
                var paging = data.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize);
                PaginationSet<AppUserMapping> paginationSet = new()
                {
                    Items = paging,
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
                };

                return Ok(paginationSet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thêm mới tài khoản
        /// </summary>
        /// <param name="appUserViewModel"></param>
        /// <returns></returns>
        /// <exception cref="NameDuplicatedException"></exception>
        [HttpPost("create")]
        [Authorize(Roles = "CreateUser")]
        public async Task<IActionResult> Create(AppUserViewModel appUserViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appuser/create", "POST");
                    AppUser appUser = new AppUser();
                    appUser.UpdateUser(appUserViewModel, "add");
                    appUser.CreatedDate = DateTime.Now;
                    var result = await _userManager.CreateAsync(appUser, appUserViewModel.PasswordHash);
                    if (result.Succeeded)
                    {
                        List<AppUserGroup> groups = new List<AppUserGroup>();
                        if (appUserViewModel.GroupId > 0)
                        {
                            groups.Add(new AppUserGroup()
                            {
                                GroupId = appUserViewModel.GroupId,
                                UserId = appUser.Id
                            });
                            var lstRole = await _applicationRoleService.GetListRoleByGroupId(appUserViewModel.GroupId);
                            foreach (var item in lstRole)
                            {
                                await _userManager.RemoveFromRoleAsync(appUser, item.Name);
                                await _userManager.AddToRoleAsync(appUser, item.Name);
                            }
                        }
                        await _applicationGroupService.AddUserToGroups(groups, appUser.Id);
                        return CreatedAtAction(nameof(Create), appUser);
                    }
                    else
                    {
                        var errorMessage = result.Errors.FirstOrDefault()?.Description ?? "Không thể tạo tài khoản";
                        return BadRequest(new { message = errorMessage });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Chỉnh sửa tài khoản đăng nhập
        /// </summary>
        /// <param name="appUserViewModel"></param>
        /// <returns></returns>
        /// <exception cref="NameDuplicatedException"></exception>
        [HttpPut("update")]
        [Authorize(Roles = "UpdateUser")]
        public async Task<IActionResult> Update(AppUserViewModel appUserViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appuser/update", "PUT");
                    AppUser appUser = new AppUser();
                    appUser = await _userManager.FindByIdAsync(appUserViewModel.Id);
                    appUser.UpdateUser(appUserViewModel, "update");
                    appUser.UpdatedDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(appUserViewModel.PasswordHash))
                    {
                        appUser.PasswordHash = _userManager.PasswordHasher.HashPassword(appUser, appUserViewModel.PasswordHash);
                    }
                    var result = await _userManager.UpdateAsync(appUser);
                    if (result.Succeeded)
                    {
                        List<AppUserGroup> lstGroups = new List<AppUserGroup>();
                        if (appUserViewModel.GroupId > 0)
                        {
                            var lstRole = await _applicationRoleService.GetAll();
                            foreach (var item in lstRole)
                            {
                                await _userManager.RemoveFromRoleAsync(appUser, item.Name);
                            }

                            lstGroups.Add(new AppUserGroup()
                            {
                                GroupId = appUserViewModel.GroupId,
                                UserId = appUserViewModel.Id
                            });

                            var lstRole1 = await _applicationRoleService.GetListRoleByGroupId(appUserViewModel.GroupId);
                            foreach (var item1 in lstRole1)
                            {
                                await _userManager.AddToRoleAsync(appUser, item1.Name);
                            }
                        }
                        await _applicationGroupService.AddUserToGroups(lstGroups, appUserViewModel.Id);
                        return CreatedAtAction(nameof(Create), appUser);
                    }
                    else
                    {
                        if (result != null)
                        {
                            if (result.Errors.Count() > 0)
                            {
                                throw new NameDuplicatedException(result.Errors.ToList()[0].Description);
                            }
                        }
                        return BadRequest(result);
                    }
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
        /// Đổi mật khẩu tài khoản
        /// </summary>
        /// <param name="appUserViewModel"></param>
        /// <returns></returns>
        [HttpPut("changepassword")]
        [Authorize(Roles = "ChangePassword")]
        public async Task<IActionResult> ChangePassword(AppUserViewModel appUserViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appuser/changepassword", "PUT");
                    AppUser appUser = new AppUser();
                    appUser = await _userManager.FindByIdAsync(appUserViewModel.Id);
                    if (!string.IsNullOrEmpty(appUserViewModel.PasswordHash))
                    {
                        appUser.PasswordHash = _userManager.PasswordHasher.HashPassword(appUser, appUserViewModel.PasswordHash);
                    }
                    var result = await _userManager.UpdateAsync(appUser);
                    return result.Succeeded ? CreatedAtAction(nameof(Create), appUser) : BadRequest(result.Errors);
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
        /// Xóa tài khoản
        /// </summary>
        /// <param name="lstId"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [Authorize(Roles = "DeleteUser")]
        public async Task<IActionResult> Delete(DeleteModel deleteModel)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appuser/delete", "DELETE");
                int countSuccess = 0;
                int countFail = 0;
                List<string> result = new List<string>();
                var lstItem = JsonConvert.DeserializeObject<List<string>>(deleteModel.lstId);
                foreach (var item in lstItem)
                {
                    try
                    {
                        AppUser appUser = new AppUser();
                        appUser = await _userManager.FindByIdAsync(item);
                        appUser.DeletedBy = deleteModel.userId;
                        appUser.DeletedDate = DateTime.Now;
                        appUser.Status = false;
                        var lstRole = await _applicationRoleService.GetAll();
                        foreach (var role in lstRole)
                        {
                            await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                        }
                        var res = await _userManager.UpdateAsync(appUser);
                        countSuccess++;
                    }
                    catch (Exception)
                    {
                        countFail++;
                    }
                }
                result.Add("Xóa thành công: " + countSuccess);
                result.Add("Lỗi: " + countFail);
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
