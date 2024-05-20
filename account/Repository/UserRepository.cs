using account.Data;
using account.Models;
using account.Models.DTO;
using account.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace account.Repository
{
	public class UserRepository : IUserRepository
	{

		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private string secretKey;
		private readonly IMapper _mapper;

		public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager, IMapper mapper, IConfiguration configuration)
		{
			_db = db;
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
		}
		public bool IsUniqueUser(string username)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
			if (user == null)
			{
				return true;
			}
			return false;
		}

		public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
			bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
			if (user == null)
			{
				return new LoginResponseDTO()
				{
					Token = "",
					User = null,
				};
			}
			var role = await _userManager.GetRolesAsync(user);
			Console.WriteLine("role: " + role);
			Console.WriteLine("user: " + user);
			foreach (var r in role)
			{
				Console.WriteLine($"Role: {r}");
			}
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.Id.ToString()),
					new Claim(ClaimTypes.Role, role.FirstOrDefault()),
					new Claim("UserName", user.Name.ToString()),
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
			{
				Token = tokenHandler.WriteToken(token),
				User = _mapper.Map<UserDTO>(user),
				Role = role.FirstOrDefault(),
			};
			return loginResponseDTO;
		}

		public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
		{
			ApplicationUser user = new()
			{
				UserName = registerationRequestDTO.UserName,
				Email = registerationRequestDTO.UserName,
				NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
				Name = registerationRequestDTO.Name,

			};

			try
			{
				var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
				if (result.Succeeded)
				{
					if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
					{
						await _roleManager.CreateAsync(new IdentityRole("admin"));
						await _roleManager.CreateAsync(new IdentityRole("customer"));
						await _roleManager.CreateAsync(new IdentityRole("guest"));
						await _userManager.AddToRoleAsync(user, "admin");
					}
					else
					{
						await _userManager.AddToRoleAsync(user, "customer");
					}
					var userToReturn = _db.ApplicationUsers
						.FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
					return _mapper.Map<UserDTO>(userToReturn);
				}
				else
				{
					foreach (var error in result.Errors)
					{
						// 處理每個錯誤信息
						// 例如，可以將錯誤信息記錄下來，以便進行調試或顯示給用戶
						Console.WriteLine($"Error: {error.Code}, {error.Description}");
					}
				}
			}
			catch (Exception ex)
			{

			}

			return new UserDTO();
		}
	}
}
