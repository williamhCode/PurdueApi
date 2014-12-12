﻿using PurdueIo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PurdueIo.Controllers
{
	[RoutePrefix("Students")]
    public class StudentController : ApiController
    {
		[Route("Authenticate")]
		[HttpPost]
		public async Task<IHttpActionResult> PostAuthenticate()
		{
			string[] creds = ParseAuthorization(Request);

			CatalogApi.CatalogApi api = new CatalogApi.CatalogApi(creds[0], creds[1]);

			//Checks to see if the credentials are correct
			bool correct = false;
			try
			{
				correct = await api.HasValidCredentials();
			}
			catch (Exception e)
			{
				return BadRequest("And error occured trying to verify credentials: " + e.ToString());
			}

			if (!correct)
			{
				return BadRequest("Invalid Credentials");
			}

			return Ok("Authenticated");
		}

		[Route("GetSchedule")]
		[HttpGet]
		public IHttpActionResult GetSchedule()
		{
			return null;
		}

		[Route("AddCrns")]
		[HttpPost]
		public async Task<IHttpActionResult> PostAddCrns(StudentAddCourseModel model)
		{
			string[] creds = ParseAuthorization(Request);

			if(model == null)
			{
				return BadRequest("No body");
			}

			if(model.pin == null)
			{
				return BadRequest("No specified pin");
			}

			if(model.crnList == null)
			{
				return BadRequest("No specified CRNs");
			}
			CatalogApi.CatalogApi api = new CatalogApi.CatalogApi(creds[0], creds[1]);

			//Checks to see if the credentials are correct
			bool correct = false;
			try
			{
				correct = await api.HasValidCredentials();
			}
			catch (Exception e)
			{
				return BadRequest("And error occured trying to verify credentials: " + e.ToString());
			}

			if (!correct)
			{
				return BadRequest("Invalid Credentials");
			}

			//Attemps to add the classes
			try
			{
				await api.AddCrn(model.termCode, model.pin, model.crnList);
			}
			catch (Exception e)
			{
				return BadRequest("An exception occured: ");
			}

			//No code to return from the async task it just goes, yolo
			return Ok("Added");
		}

		[Route("DropCrns")]
		[HttpPost]
		public IHttpActionResult DropClass()
		{
			return null;
		}

		private string[] ParseAuthorization(HttpRequestMessage request)
		{
			var he = request.Headers;
			if(!he.Contains("Authorization"))
			{
				throw new  Exception("No authorization header");
			}

			string auth = request.Headers.Authorization.ToString();

			if(auth == null || auth.Length == 0 || ! auth.StartsWith("Basic"))
			{
				throw new Exception("Invalid authorization header");
			}

			string base64Creds = auth.Substring(6);
			string[] creds = Encoding.ASCII.GetString(Convert.FromBase64String(base64Creds)).Split(new char[] { ':' });

			if(creds.Length != 2 || string.IsNullOrEmpty(creds[0]) || string.IsNullOrEmpty(creds[1]))
			{
				throw new Exception("Invalid authorization credentials, missing either the username or password");
			}

			return creds;
		}

		//Not used anymore, it actually does not save any space or reduce work since
		//new code need to be written to handel the output
		private async Task<string> Authenticate(CatalogApi.CatalogApi api)
		{
			bool correct = false;
			try
			{
				correct = await api.HasValidCredentials();
			}
			catch (Exception e)
			{
				return "And error occured trying to verify credentials: " + e.ToString();
			}

			if (!correct)
			{
				return "Invalid Credentials";
			}

			return null;
		}
    }
}