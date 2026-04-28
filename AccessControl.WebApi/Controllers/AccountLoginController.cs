using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using AccessControl.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AccessControl.WebApi.Controllers
{
    [Route("api/access/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountLoginController : ControllerBase
    {
        #region Initialize
        protected readonly ILogger<AccountLoginController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdentityService _identityService;
        private readonly AppSettings _appSettings;
        public AccountLoginController(ILogger<AccountLoginController> logger, UserManager<AppUser> userManager, IIdentityService identityService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _userManager = userManager;
            _identityService = identityService;
            _appSettings = appSettings.Value;
        }

        #endregion Initialize

        #region Properties

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel model)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/accountlogin/login", "POST");
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    return Unauthorized();
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                List<string> roles = (List<string>)await _userManager.GetRolesAsync(user);
                if (!passwordValid)
                {
                    return Unauthorized();
                }
                var token = _identityService.GenerateJwtToken(user.Id, user.UserName, roles, _appSettings.Secret);

                return new LoginResponseModel
                {
                    Id = user.Id,
                    EmId = user.EM_ID,
                    Access_token = "Bearer " + token,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Image = user.Image,
                    Phone = user.PhoneNumber
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Lấy hồ sơ người dùng
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet("getprofileasync")]
        public async Task<ActionResult<LoginResponseModel>> GetProfileAsync(string userName, string password)
        {
            try
            {
                _logger.LogInformation("Run endpoint {endpoint} {verb}", "/api/access/accountlogin/getprofileasync", "GET");
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return Unauthorized();
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, password);

                if (!passwordValid)
                {
                    return Unauthorized();
                }
                return new LoginResponseModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Image = user.Image,
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Kiểm tra mật khẩu
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet("Checkpassword")]
        public async Task<bool> CheckPasswd(string userName, string pass)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return false;
            }
            else
            {
                return await _userManager.CheckPasswordAsync(user, pass);
            }

        }

        #endregion Properties
    }
}
