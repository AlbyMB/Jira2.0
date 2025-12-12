namespace Final_Grp6_PROG3340.Models
{
	public class RegisterDto
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class LoginDto
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class GoogleLoginDto
	{
		public string IdToken { get; set; }
	}
}
