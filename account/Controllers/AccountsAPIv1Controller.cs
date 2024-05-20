using account.Models;
using account.Models.DTO;
using account.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace account.Controllers
{
	[Route("api/v{version:apiVersion}/accounts")]
	[ApiController]
	//[Authorize]
	[ApiVersion("1.0")]
	public class AccountsAPIv1Controller : ControllerBase
	{
		protected APIResponse _response;
		private readonly IAccountRepository _dbAccount;
		private readonly IMapper _mapper;

		public AccountsAPIv1Controller(IAccountRepository dbAccount, IMapper mapper)
		{
			_dbAccount = dbAccount;
			_mapper = mapper;
			this._response = new();
		}

		[HttpGet]
		public async Task<ActionResult<APIResponse>> GetAccounts([FromQuery] string? search)
		{
			var userId = User.FindFirstValue(ClaimTypes.Name);
			IEnumerable<Account> accountsList = await _dbAccount.GetAccountsByUserIdAsync(userId);

			try
			{
				if (!string.IsNullOrEmpty(search))
				{
					accountsList = accountsList.Where(u => u.AccountName.ToLower().Contains(search.ToLower()));
				}
				_response.Result = _mapper.Map<List<AccountDTO>>(accountsList); ;
				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				_response.ErrorMessages = new List<string>();
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.ErrorMessages = new List<string>() { ex.ToString() };
				_response.StatusCode = HttpStatusCode.InternalServerError;
				return BadRequest(_response);
			}

		}

		[HttpGet("{id}", Name = "GetAccount")]
		public async Task<ActionResult<APIResponse>> GetAccount(int id)
		{
			try
			{
				if (id <= 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				var account = await _dbAccount.GetAccountByIdAsync(id);

				if (account == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}

				_response.Result = _mapper.Map<Account>(account);
				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{

				_response.ErrorMessages = new List<string> { ex.ToString() };
				_response.StatusCode = HttpStatusCode.InternalServerError;
				return BadRequest(_response);
			}
		}


		[HttpPost]
		public async Task<IActionResult> CreateAccount([FromBody] AccountCreateDTO createDTO)
		{
			try
			{
				var userId = User.FindFirstValue(ClaimTypes.Name);
				//var userName = User.FindFirstValue("UserName");
				if (createDTO == null)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				Account account = _mapper.Map<Account>(createDTO);
				account.UserId = userId;
				account.CreatedAt = DateTime.UtcNow;
				account.UpdateAt = DateTime.UtcNow;
				await _dbAccount.CreateAsync(account);

				_response.Result = _mapper.Map<Account>(account);
				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.ErrorMessages = new List<string>() { ex.ToString() };
				_response.StatusCode = HttpStatusCode.InternalServerError;
				return BadRequest(_response);
			}
		}


		[HttpDelete]
		public async Task<ActionResult<APIResponse>> DeleteAccount(int id)
		{
			try
			{

				if (id == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}
				var account = await _dbAccount.GetAsync(u => u.AccountId == id);
				if (account == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}
				await _dbAccount.RemoveAsync(account);
				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.NoContent;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.ErrorMessages = new List<string>() { ex.ToString() };
				_response.StatusCode = HttpStatusCode.InternalServerError;
				return BadRequest(_response);
			}
		}


		[HttpPut("{id:int}", Name = "UpdateAccount")]
		public async Task<ActionResult> UpdateAccount(int id, [FromBody] AccountUpdateDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || id != updateDTO.AccountId)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				Account account = await _dbAccount.GetAccountByIdAsync(id);
				if (account == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}

				// 更新其他字段
				account.AccountName = updateDTO.AccountName;
				account.AccountType = updateDTO.AccountType;
				account.Balance = updateDTO.Balance;
				account.UpdateAt = DateTime.UtcNow;
				await _dbAccount.UpdateAsync(account);

				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				_response.Result = account;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.ErrorMessages = new List<string>() { ex.ToString() };
				_response.StatusCode = HttpStatusCode.InternalServerError;
				return BadRequest(_response);
			}
		}

	}
}
