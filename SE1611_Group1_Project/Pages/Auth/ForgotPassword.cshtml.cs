using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_Project.Models;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace SE1611_Group1_Project.Pages.Authen
{
    public class ForgotPasswordModel : PageModel
    {
		private readonly FoodOrderContext _context;

		[BindProperty]   
        public string InputEmail { get; set; }
		public string MsgErr { get; set; }
        public ForgotPasswordModel(FoodOrderContext context)
        {
            _context= context;
        }
		public void OnGet()
        {
        }

		public IActionResult OnPost()
		{
			var user = _context.Users.SingleOrDefault(u => u.Email == InputEmail);

			if (user != null)
			{
				// Generate a new password for the user
				var newPassword = GenerateNewPassword();

				// Save the new password to the database
				user.Password = newPassword;
				_context.SaveChanges();

				// Send the new password to the user's email address
				SendPasswordEmail(user.Email, newPassword);

				return RedirectToPage("/Auth/Login");
			}
			MsgErr = "Email not valid!!";
			return Page();
		}

		private string GenerateNewPassword()
		{
			const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=";
			const int passwordLength = 8;

			var random = new Random();
			var newPassword = new StringBuilder();

			for (int i = 0; i < passwordLength; i++)
			{
				int index = random.Next(allowedChars.Length);
				newPassword.Append(allowedChars[index]);
			}

			return newPassword.ToString();
		}
		private void SendPasswordEmail(string email, string newPassword)
		{
			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("managementcoffeeG1@gmail.com", "xdswzxbizecjpskr"),
				//chatgpt.1202@gmail.com
				EnableSsl = true,
				UseDefaultCredentials = false
			};

			var message = new MailMessage(
				"managementcoffeeG1@gmail.com",
				email,
				"Reset Password Management Coffee",
				$"{newPassword} là mật khẩu mới được cung cấp dành riêng cho bạn từ hệ thống Management Coffee. Hãy sử dụng mật khẩu này để đăng nhập và thay đổi mật khẩu của bạn!");

			smtpClient.Send(message);
		}
	}
}
