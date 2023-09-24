using AppR.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web.Helpers;

namespace AppR.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration Configuration;
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

        public JsonResult Register(string Username, string Password)
        {
            string Salt = Crypto.GenerateSalt();
            string password = Password + Salt;
            string HashPassword = Crypto.HashPassword(password);
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "INSERT INTO UsersInformation ([UserId],[HashPassword],[Salt])" +
                    " VALUES (@UserId,@HashPassword,@Salt)";
                    cmd.Parameters.AddWithValue("@UserId", Username);
                    cmd.Parameters.AddWithValue("@HashPassword", HashPassword);
                    cmd.Parameters.AddWithValue("@Salt", Salt);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return Json(null);
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
    }
}