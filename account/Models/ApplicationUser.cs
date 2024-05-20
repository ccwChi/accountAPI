using Microsoft.AspNetCore.Identity;

namespace account.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; }
	}
}
