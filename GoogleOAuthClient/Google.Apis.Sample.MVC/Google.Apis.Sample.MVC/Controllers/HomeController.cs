﻿using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Download;
using Google.Apis.Drive.v2;
using Google.Apis.Sample.MVC.Models;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;

namespace Google.Apis.Sample.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public async Task<ActionResult> DriveAsync(CancellationToken cancellationToken)
        {
            ViewBag.Message = "Your drive page.";

            var result = await new AuthorizationCodeMvcApp(this, new AppAuthFlowMetadata()).
                AuthorizeAsync(cancellationToken);

            if (result.Credential == null)
                return new RedirectResult(result.RedirectUri);

            GetGoogleUserProfile(result.Credential.Token.AccessToken);
            return RedirectToAction("About", "Home");
            /*
            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = result.Credential,
                ApplicationName = "ASP.NET Google APIs MVC Sample"
            });

            var listReq = driveService.Files.List();
            listReq.Fields = "items/title,items/id,items/createdDate,items/downloadUrl,items/exportLinks";
            var list = await listReq.ExecuteAsync();
            var items = 
                (from file in list.Items
                         select new FileModel
                         {
                             Title = file.Title,
                             Id = file.Id,
                             CreatedDate = file.CreatedDate,
                             DownloadUrl = file.DownloadUrl ?? 
                                           (file.ExportLinks != null ? file.ExportLinks["application/pdf"] : null),
                         }).OrderBy(f => f.Title).ToList();

            return View(items);
            */
        }

        private void GetGoogleUserProfile(string accessToken)
        {
            var client = new WebClient();
            var profileData = client.DownloadString(string.Format("https://www.googleapis.com/oauth2/v3/userinfo?alt=json&access_token={0}", accessToken));
            if (!string.IsNullOrEmpty(profileData))
            {
                JObject jObject = JObject.Parse(profileData);
            }
        }
    }
}