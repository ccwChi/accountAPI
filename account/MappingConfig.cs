using account.Models.DTO;
using account.Models;
using AutoMapper;

namespace account
{
	public class MappingConfig : Profile
	{
		public MappingConfig()
		{
			CreateMap<ApplicationUser, UserDTO>().ReverseMap();
			CreateMap<Account, AccountDTO>().ReverseMap();
			CreateMap<Account, AccountCreateDTO>().ReverseMap();
			CreateMap<Account, AccountUpdateDTO>().ReverseMap();
			CreateMap<Transaction, TransactionCreateDTO>().ReverseMap();
			CreateMap<Transaction, TransactionUpdateDTO>().ReverseMap();
			CreateMap<Transaction, TransactionDTO>().ReverseMap();

			CreateMap<Transaction, TransactionWithAccountDTO>().ReverseMap();
			CreateMap<Account, AccountForTransactionDTO>().ReverseMap();

		}
	}
}
