using AppR.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AppR.Controllers
{
    public class MaintenanceController : Controller
    {
		private readonly IConfiguration Configuration;
		public MaintenanceController(IConfiguration _configuration)
		{
			Configuration = _configuration;
		}
		private string GetConnectionString()
		{
			return Configuration.GetConnectionString("DefaultConnection");
		}

		public IActionResult EmailTemplate()
        {
            return View();
        }

		public JsonResult BrowseEmailType(string EmailType)
		{
			EmailTemplate _model = new EmailTemplate();
			using (SqlConnection con = new SqlConnection(GetConnectionString()))
			{
				SqlCommand cmd = new SqlCommand("Select *from EmailTemplate where EmailType='" + EmailType + "'", con);
				con.Open();
				SqlDataReader sdr = cmd.ExecuteReader();
				while (sdr.Read())
				{
					_model.Subject = Convert.ToString(sdr["Subject"]);
					_model.Content = Convert.ToString(sdr["Content"]);
				}
				con.Close();
			}
			Trace.WriteLine(JsonConvert.SerializeObject(_model));
			return Json(_model);
		}
		public JsonResult UpdateEmailType(string Subject, string Content, string EmailType)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(GetConnectionString()))
				{
					SqlCommand cmd = new SqlCommand("Update EmailTemplate set  Subject=@Subject, Content=@Content where EmailType='" + EmailType + "'", con);
					con.Open();
					SqlDataReader sdr = cmd.ExecuteReader();
					while (sdr.Read())
					{
						cmd.Parameters.AddWithValue("@Subject", Subject);
						cmd.Parameters.AddWithValue("@Content", Content);
					}
					con.Close();
				}
				return Json("Success");
			}
			catch (Exception ex) { 
				return Json(ex.Message);
			}
		}
	}
}
