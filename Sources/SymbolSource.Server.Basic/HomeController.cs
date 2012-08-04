using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace SymbolSource.Server.Basic
{
    public class InfoViewModel
    {
        public string VisualStudioUrl;
        public string NuGetPushUrl;
        public string OpenWrapUrl;
        public string SrcSrvPathTest;
        public KeyValuePair<string, string> NuGetSmokeTest;
        public KeyValuePair<string, string> OpenWrapSmokeTest;
        public KeyValuePair<string, string> NuGetPushTest;
        public KeyValuePair<string, string> OpenWrapPushTest;
        public KeyValuePair<string, string> NuGetFeedTest;
    }

    public class HomeController : Controller
    {
        private string GetAbsoluteUrl(string relativeUrl)
        {
            return Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath.TrimEnd('/') + relativeUrl;
        }

        public ActionResult Index()
        {
            return View(new InfoViewModel
                {
                    VisualStudioUrl = GetAbsoluteUrl("/WinDbg/pdb"),
                    NuGetPushUrl = GetAbsoluteUrl("/NuGet"),
                    OpenWrapUrl = GetAbsoluteUrl("/OpenWrap"),
                    SrcSrvPathTest = Directory.Exists(ConfigurationManager.AppSettings["SrcSrvPath"]) ? "OK" : "Directory not found",
                    NuGetSmokeTest = InlineTest("SmokeTest", new { url = GetAbsoluteUrl("/NuGet/FeedService.mvc") }),
                    OpenWrapSmokeTest = InlineTest("SmokeTest", new { url = GetAbsoluteUrl("/OpenWrap/index.wraplist") }),
                    NuGetPushTest = InlineTest("NuGetPushTest", null),
                    NuGetFeedTest = InlineTest("NuGetFeedTest", null),
                    OpenWrapPushTest = InlineTest("OpenWrapPushTest", null),
                });
        }

        private KeyValuePair<string, string> InlineTest(string action, object routeValues)
        {
            var url = GetAbsoluteUrl(Url.Action(action, routeValues));
            using (var client = new WebClient())
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = client.DownloadString(url);
                stopwatch.Stop();
                result = result.Substring(0, Math.Min(20, result.Length));
                result = string.Format("{0} ({1} ms)", result, stopwatch.ElapsedMilliseconds);
                return new KeyValuePair<string, string>(url, result);
            }
        }

        public ActionResult SmokeTest(string url)
        {
            try
            {
                using (var client = new WebClient())
                    client.DownloadData(url);
                
                return Content("OK");
            }
            catch (WebException e)
            {
                var result = (HttpWebResponse)e.Response;
                using (var stream = result.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding.GetEncoding(result.ContentEncoding)))
                    return Content(string.Format("{0} - {1}\n\n{2}", result.StatusCode, result.StatusDescription, reader.ReadToEnd()));
            }
        }

        public ActionResult NuGetPushTest()
        {
            var helper = new Gateway.NuGet.Core.TestHelper();
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Packages.DemoLibrary.nupkg"))
                helper.Push(GetAbsoluteUrl("/NuGet"), "Test", stream);

            return Content("OK");
        }

        public ActionResult NuGetFeedTest()
        {
            var helper = new Gateway.NuGet.Core.TestHelper();
            var count = helper.Count(GetAbsoluteUrl("/NuGet/FeedService.mvc"), new NetworkCredential("Test", "Test"));
            return Content(string.Format("{0} packages", count));
        }

        public ActionResult OpenWrapPushTest()
        {
            var helper = new Gateway.OpenWrap.Core.TestHelper();
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Packages.demolibrary.wrap"))
                helper.Push(GetAbsoluteUrl("/OpenWrap"), new NetworkCredential("Test", "Test"), "Test", stream);

            return Content("OK");
        }
    }
}
