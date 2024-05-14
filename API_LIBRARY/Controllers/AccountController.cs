using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API_LIBRARY.Interfaces;
using API_LIBRARY.DTO;
using System.Linq;
using System.Threading.Tasks;

namespace API_LIBRARY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly IBookRepository _bookRepository;

        public AccountController(IBookRepository bookRepository, ITokenRepository tokenRepository, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _bookRepository = bookRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            if (registerRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid registration request.");
            }

            var identityUser = new IdentityUser
            {
                UserName = registerRequestDTO.Username,
                Email = registerRequestDTO.Username
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDTO.Password);
            if (identityResult.Succeeded)
            {
                if (registerRequestDTO.Roles != null && registerRequestDTO.Roles.Any())
                {
                    var rolesResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDTO.Roles);
                    if (!rolesResult.Succeeded)
                    {
                        return BadRequest("Failed to add roles to the user.");
                    }
                }
                return Ok("Registration successful! Please login.");
            }

            var errors = identityResult.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            if (loginRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid login request.");
            }

            var user = await _userManager.FindByEmailAsync(loginRequestDTO.Username);
            if (user != null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
                if (checkPasswordResult)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

                    var response = new LoginResponseDTO
                    {
                        JwtToken = jwtToken
                    };

                    return Ok(response);
                }
            }

            return Unauthorized("Username or password incorrect.");
        }
    }
}
