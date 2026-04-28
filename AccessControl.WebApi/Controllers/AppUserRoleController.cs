using AccessControl.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.WebApi.Controllers
{
    [Route("api/access/[controller]")]
    [ApiController]
    [Authorize]
    public class AppUserRoleController : ControllerBase
    {
        #region Initialize
        private readonly IAppUserRoleService _appUserRoleService;
        private readonly IMapper _mapper;
        private readonly ILogger<AppUserRoleController> _logger;

        public AppUserRoleController(IAppUserRoleService appUserRoleService, IMapper mapper, ILogger<AppUserRoleController> logger)
        {
            _appUserRoleService = appUserRoleService;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion Initialize

        #region Properties

        /// <summary>
        /// Get danh sách roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("getuserroleid")]
        public async Task<IActionResult> GetUserRoleId(string userId)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/appuserrole/getuserroleid", "GET");
                var model = await _appUserRoleService.GetAllUserRole(userId);

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
