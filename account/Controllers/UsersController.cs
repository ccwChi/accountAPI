using account.Models;
using account.Models.DTO;
using account.Repository.IRepository;
using Asp.Versioning;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace account.Controllers
{
	[Route("api/UserAuth")]
	[ApiController]
	[ApiVersionNeutral]
	public class UsersController : Controller
	{
		private readonly IUserRepository _userRepo;
		private readonly APIResponse _response;
		public UsersController(IUserRepository userRepo)
		{
			_userRepo = userRepo;
			this._response = new ();
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
		{
			var loginResponse = await _userRepo.Login(model);
			if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username or Password is incorrect");
				return BadRequest(_response);
			}
			_response.StatusCode = HttpStatusCode.OK;
			_response.Result = loginResponse;
			return Ok(_response);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
		{
			bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
			if (!ifUserNameUnique)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username already used");
				return BadRequest(_response);
			}
			var user = await _userRepo.Register(model);
			if (user == null)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Error while registering");
				return BadRequest(_response);
			}
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}
	}
}
