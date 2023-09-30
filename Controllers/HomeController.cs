using AppR.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web.Helpers;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                }
                con.Close();
                PlainPass = PlainPass + salt; // append salt key
                result = Crypto.VerifyHashedPassword(HashedPass, PlainPass); //verify password
            }
            Trace.WriteLine(result);
            if (result == true)
            {
                Response.Cookies.Append("rcbctellerlessusername", Username);
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
	}
}