using account.Models.DTO;
using account.Models;
using account.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace account.Controllers
{
	[Route("api/v{version:apiVersion}/transaction")]
	[ApiController]
	[Authorize]
	[ApiVersion("2.0")]
	public class TransactionsAPIv2Controller : ControllerBase
	{
		protected APIResponse _response;
		private readonly ITransactionRepository _dbTransaction;
		private readonly IAccountRepository _dbAccount;
		private readonly IMapper _mapper;

		public TransactionsAPIv2Controller(ITransactionRepository dbTransaction, IMapper mapper, IAccountRepository dbAccount)
		{
			_dbTransaction = dbTransaction;
			_dbAccount = dbAccount;
			_mapper = mapper;
			this._response = new();
		}

		[HttpGet]
		public async Task<ActionResult<APIResponse>> GetAccounts([FromQuery] string? transactionType,  string? search)
		{

			var userId = User.FindFirstValue(ClaimTypes.Name);
			IEnumerable<Transaction> transactionList = await _dbTransaction.GetTransactionsByUserIdAsync(userId);

			try
			{
				if (!string.IsNullOrEmpty(transactionType))
				{
					transactionList = transactionList.Where(u => u.TransactionType.ToLower() == transactionType.ToLower());

				}
				if (!string.IsNullOrEmpty(search))
				{
					transactionList = transactionList.Where(u => u.Name.ToLower().Contains(search.ToLower()));
				}
				_response.Result = _mapper.Map<List<TransactionWithAccountDTO>>(transactionList);
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

	}

}
