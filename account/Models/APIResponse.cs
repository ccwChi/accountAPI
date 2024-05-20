using System.Net;

namespace account.Models
{
	public class APIResponse
	{
		public APIResponse() { 
			ErrorMessages = new List<string>();
		}
		public List<string> ErrorMessages { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; } = false;
		public object Result { get; set; }
	}
}
