using AppR.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web.Helpers;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mail;
using System.Net;
using System.Reflection;

namespace AppR.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration Configuration;
        public HomeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        private string GetConnectionString()
        {
            return Configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
		{
			return View();
        }

        public IActionResult CreateNewUser()
        {
            return View();
        }
        public IActionResult Login()
        {
            if (Request.Cookies["rcbctellerlessusername"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
           
        }

		public IActionResult ResetPassword()
		{
			return View();
		}

		public IActionResult Logout()
        {
            Response.Cookies.Delete("rcbctellerlessusername");
			Response.Cookies.Delete("rcbctellerlogin");
			Response.Cookies.Delete("rcbctellername");
			return RedirectToAction("Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public JsonResult Register(string Username, string Password, string EmployeeName, string Email, string Mobileno,
            string GrpDept, string Role)
        {
			string Salt = Crypto.GenerateSalt();
            string password = Password + Salt;
            string HashPassword = Crypto.HashPassword(password);

			string InsertStatus = "0";
			using (SqlConnection con = new SqlConnection(GetConnectionString()))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.Connection = con;
					cmd.Parameters.Clear();
					cmd.CommandText = "" +
						"BEGIN " +
							"IF NOT EXISTS (SELECT* FROM UsersInformation " +
							"WHERE UserId = @UserId) " +
						"BEGIN " +
							"INSERT INTO UsersInformation (UserId, HashPassword, Salt, EmployeeName, Email, " +
							"MobileNumber, GroupDept, UserRole, UserStatus, DateAdded, loginAttempt) " +
							"VALUES(@UserId, @HashPassword, @Salt, @EmployeeName, @Email, @MobileNumber, @GroupDept, @UserRole, @UserStatus, @DateAdded, @loginAttempt)" +
							"END " +
						"END";
					cmd.Parameters.AddWithValue("@UserId", Username.Replace("\'", "\''"));
					cmd.Parameters.AddWithValue("@HashPassword", HashPassword);
					cmd.Parameters.AddWithValue("@Salt", Salt);
					cmd.Parameters.AddWithValue("@EmployeeName", EmployeeName);
					cmd.Parameters.AddWithValue("@Email", Email);
					cmd.Parameters.AddWithValue("@MobileNumber", Mobileno);
					cmd.Parameters.AddWithValue("@GroupDept", GrpDept);
					cmd.Parameters.AddWithValue("@UserRole", Role);
					cmd.Parameters.AddWithValue("@UserStatus", "0");
					cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss tt"));
					cmd.Parameters.AddWithValue("@loginAttempt", "0");
					con.Open();
					int a = cmd.ExecuteNonQuery();
					if (a > 0) { InsertStatus = "1"; }
					con.Close();
				}
			}
            return Json(InsertStatus);
        }
        public JsonResult LoginAjax(string Username, string Password)
        {
            string salt = ""; //read from database
            string HashedPass = ""; //read from database
            string PlainPass = Password;
            bool result;
			string EmployeeName = "";
			string LastLogin = DateTime.Now.ToString("dd MMMM yyyy hh:mm tt");
			UserModel _userModel = new UserModel();
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("Select *from UsersInformation where UserId='"+ Username+"'", con);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    salt = sdr["Salt"].ToString();
                    HashedPass = sdr["HashPassword"].ToString();
					EmployeeName = Convert.ToString(sdr["EmployeeName"]);
				}
                con.Close();
                PlainPass = PlainPass + salt; // append salt key
                result = Crypto.VerifyHashedPassword(HashedPass, PlainPass); //verify password
            }
            Trace.WriteLine(result);
            if (result == true)
            {
                Response.Cookies.Append("rcbctellerlessusername", Username);
				Response.Cookies.Append("rcbctellerlogin", LastLogin);
				Response.Cookies.Append("rcbctellername", EmployeeName);
			}
            return Json(result);
        }

        public JsonResult CheckUser()
        {
            string loginStatus = "";
            if (Request.Cookies["rcbctellerlessusername"] == null)
            {
                loginStatus = "Invalid";
            }
            else
            {
                loginStatus = "Success";
            }
            return Json(loginStatus);
        }

		public JsonResult SearchUser(string Username)
		{
			UserModel _userModel = new UserModel();
			using (SqlConnection con = new SqlConnection(GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand("Select *from UsersInformation where UserId='" + Username + "'", con);
				con.Open();
				SqlDataReader sdr = cmd.ExecuteReader();
				while (sdr.Read())
				{
                    _userModel.UserId = Convert.ToString(sdr["UserId"]);
					_userModel.EmployeeName = Convert.ToString(sdr["EmployeeName"]);
					_userModel.Email = Convert.ToString(sdr["Email"]);
					_userModel.MobileNumber = Convert.ToString(sdr["MobileNumber"]);
					_userModel.GroupDept = Convert.ToString(sdr["GroupDept"]);
					_userModel.UserRole = Convert.ToString(sdr["UserRole"]);
				}
				con.Close();
			}
            Trace.WriteLine(JsonConvert.SerializeObject(_userModel));
			return Json(_userModel);
		}

        public JsonResult RegeneratePassword(string Username, string Password, string EmployeeName, string Email, string Mobileno,
			string GrpDept, string Role)
        {
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789./<>";
			var stringChars = new char[10];
			var random = new Random();

			for (int i = 0; i < stringChars.Length; i++)
			{
				Trace.WriteLine(i);
				stringChars[i] = chars[random.Next(chars.Length)];
			}
			var finalString = new string(stringChars);

			string Salt = Crypto.GenerateSalt();
			string password = finalString + Salt;
			string HashPassword = Crypto.HashPassword(password);

			string bodyMsg = "<head>" +
					   "<style>" +
					   "body{" +
						   "font-family: calibri;" +
						   "}" +
						"</style>" +
					   "</head>" +
					   "<body>" +
						   "<p>Good Day!<br>" +
							"<br>" +
							"User ID: " + Username+ "<br>" +
							"New Password: <font color=red>" + finalString + "</font> <br>" +
							"<br>" +
							"<font color=red>*Note: This is a system generated e-mail.Please do not reply.</font>" +
							"</p>" +
						 "</body>";

			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress("arlene@yuna.somee.com");
			mailMessage.To.Add("arlene.lunar11@gmail.com");
			mailMessage.Subject = "Subject";
			mailMessage.Body = bodyMsg;
			mailMessage.IsBodyHtml = true;

			SmtpClient smtpClient = new SmtpClient();
			smtpClient.Host = "smtp.yuna.somee.com";
			smtpClient.Port = 26;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential("arlene@yuna.somee.com", "12345678");
			smtpClient.EnableSsl = false;

			try
			{
				smtpClient.Send(mailMessage);
				Trace.WriteLine("Email Sent Successfully.");
				//update password in Database
				var sql = "Update UsersInformation set HashPassword=@HashPassword, Salt=@Salt where UserId='" + Username + "'";
				using (var connection = new SqlConnection(GetConnectionString()))
				{
					using (var command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@HashPassword", HashPassword);
						command.Parameters.AddWithValue("@Salt", Salt);
						// repeat for all variables....
						connection.Open();
						command.ExecuteNonQuery();
					}
					connection.Close();
				}
				return Json("sent");
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Error: " + ex.Message);
				return Json("fail");
			}
        }
	}
}