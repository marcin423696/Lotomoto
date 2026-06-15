using System.ComponentModel.DataAnnotations;

namespace Lotomoto.Models
{
	public class User
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "E-mail jest wymagany")]
		[EmailAddress(ErrorMessage = "Niepoprawny format e-mail")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Has³o jest wymagane")]
		[MinLength(6, ErrorMessage = "Has³o musi mieæ minimum 6 znaków")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Imiê jest wymagane")]
		public string FirstName { get; set; }
	}
}