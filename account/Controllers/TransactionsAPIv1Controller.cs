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
	[ApiVersion("1.0")]
	public class TransactionsAPIv1Controller : ControllerBase
	{
		protected APIResponse _response;
		private readonly ITransactionRepository _dbTransaction;
		private readonly IAccountRepository _dbAccount;
		private readonly IMapper _mapper;

		public TransactionsAPIv1Controller(ITransactionRepository dbTransaction, IMapper mapper, IAccountRepository dbAccount)
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
				_response.Result = _mapper.Map<List<TransactionDTO>>(transactionList);
				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				_response.ErrorMessages = new List<string>();
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.ErrorMessages = new List<string>() { ex.ToString() };
				_response.StatusCode=HttpStatusCode.InternalServerError;
				return BadRequest(_response);
			}

		}

		[HttpGet("{id}", Name = "GetTransaction")]
		public async Task<ActionResult<APIResponse>> GetTransaction(int id)
		{
			try
			{
				if (id <= 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				var transaction = await _dbTransaction.GetTransactionByIdAsync(id);

				if (transaction == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}
				
				_response.Result = _mapper.Map<TransactionDTO>(transaction);
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
		public async Task<IActionResult> CreateAccount([FromBody] TransactionCreateDTO createDTO)
		{
			try
			{
				if (createDTO == null)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				var account = await _dbAccount.GetAccountByIdAsync(createDTO.AccountId);
				if (account == null)
				{
					ModelState.AddModelError("AccountId", "The specified account does not exist.");
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessages.Add("The specified account does not exist.");
					return BadRequest(_response);
				}

				var userId = User.FindFirstValue(ClaimTypes.Name);
				if (account.UserId != userId)
				{
					ModelState.AddModelError("AccountId", "The account does not belong to the current user.");
					_response.StatusCode = HttpStatusCode.Unauthorized;
					_response.ErrorMessages.Add("The account does not belong to the current user.");
					return Unauthorized(_response);
				}

				Transaction transaction = _mapper.Map<Transaction>(createDTO);
				transaction.CreatedAt = DateTime.UtcNow;
				transaction.UpadateAt = DateTime.UtcNow;

				await _dbTransaction.CreateAsync(transaction);

				_response.Result = _mapper.Map<TransactionDTO>(transaction);
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
		public async Task<ActionResult<APIResponse>> DeleteTransaction(int id)
		{
			try
			{

				if (id == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}
				var transaction = await _dbTransaction.GetAsync(u => u.TransactionId == id);
				if (transaction == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}
				await _dbTransaction.RemoveAsync(transaction);
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


		[HttpPut("{id:int}", Name = "UpdateTransaction")]
		public async Task<ActionResult> UpdateTransaction(int id, [FromBody] TransactionUpdateDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || id != updateDTO.TransactionId)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				Transaction existingTransaction = await _dbTransaction.GetTransactionByIdAsync(id);
				if (existingTransaction == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}

				existingTransaction.Name = updateDTO.Name;
				existingTransaction.Amount = updateDTO.Amount;
				existingTransaction.TransactionType = updateDTO.TransactionType;
				existingTransaction.Description = updateDTO.Description;
				existingTransaction.UpadateAt = DateTime.UtcNow;
				await _dbTransaction.UpdateAsync(existingTransaction);

				TransactionDTO transactionDTO = _mapper.Map<TransactionDTO>(existingTransaction);

				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				_response.Result = transactionDTO;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.ErrorMessages = new List<string>() { ex.ToString() };
				_response.StatusCode = HttpStatusCode.InternalServerError;
				return new ObjectResult(_response)
				{
					StatusCode = (int)HttpStatusCode.InternalServerError
				};
			}
		}
	}

}
