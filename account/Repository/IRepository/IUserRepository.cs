using account.Models.DTO;

namespace account.Repository.IRepository
{
	public interface IUserRepository
	{
		bool IsUniqueUser(string username);
		Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);

		Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
	}
}
