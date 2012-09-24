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
        public string NuGetFeedUrl;
        public string OpenWrapUrl;
        public string SrcSrvPathTest;
        public KeyValuePair<string, string> NuGetSmokeTest;
        public KeyValuePair<string, string> OpenWrapSmokeTest;
        public KeyValuePair<string, string> NuGetPushTest;
        public KeyValuePair<string, string> OpenWrapPushTest;
        public KeyValuePair<string, string> NuGetFeedTest;
        public KeyValuePair<string, string> OpenWrapFeedTest;
    }

    public class HomeController : Controller
    {
        private string GetAbsoluteUrl(string relativeUrl)
        {     
            return Request.Url.GetLeftPart(UriPartial.Authority) + relativeUrl;
        }

        private string GetVisualStudioUrl()
        {
            return GetAbsoluteUrl(Url.Content("~/WinDbg/pdb"));
        }

        private string GetNuGetPushUrl()
        {
            return GetAbsoluteUrl(Url.Content("~/NuGet"));
        }

        private string GetNuGetFeedUrl()
        {
            return GetAbsoluteUrl(Url.Content("~/NuGet/FeedService.mvc"));
        }

        private string GetOpenWrapUrl()
        {
            return GetAbsoluteUrl(Url.Content("~/OpenWrap"));
        }

        public ActionResult Index()
        {
            return View(new InfoViewModel
                {
                    VisualStudioUrl = GetVisualStudioUrl(),
                    NuGetPushUrl = GetNuGetPushUrl(),
                    NuGetFeedUrl = GetNuGetFeedUrl(),
                    OpenWrapUrl = GetOpenWrapUrl(),
                    SrcSrvPathTest = Directory.Exists(ConfigurationManager.AppSettings["SrcSrvPath"]) ? "OK" : "Directory not found",
                    NuGetSmokeTest = InlineTest(Url.Action("SmokeTest", new { url = Url.Content("~/NuGet/FeedService.mvc") })),
                    OpenWrapSmokeTest = InlineTest(Url.Action("SmokeTest", new { url = Url.Content("~/OpenWrap/index.wraplist") })),
                    NuGetPushTest = InlineTest(Url.Action("NuGetPushTest")),
                    NuGetFeedTest = InlineTest(Url.Action("NuGetFeedTest")),
                    OpenWrapPushTest = InlineTest(Url.Action("OpenWrapPushTest")),
                    OpenWrapFeedTest = InlineTest(Url.Action("OpenWrapFeedTest")),
                });
        }

        private KeyValuePair<string, string> InlineTest(string url)
        {
            url = GetAbsoluteUrl(url);

            using (var client = new WebClient())
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = DownloadString(url);
                stopwatch.Stop();
                result = result.Substring(0, Math.Min(20, result.Length));
                result = string.Format("{0} ({1} ms)", result, stopwatch.ElapsedMilliseconds);
                return new KeyValuePair<string, string>(url, result);
            }
        }

        private string DownloadString(string url)
        {
            try
            {
                using (var client = new WebClient())
                    return client.DownloadString(url);
            }
            catch (WebException e)
            {
                var result = (HttpWebResponse) e.Response;
                using (var stream = result.GetResponseStream())
                using (
                    var reader = new StreamReader(stream, string.IsNullOrEmpty(result.ContentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(result.ContentEncoding)))
                    return string.Format("{0} - {1}\n\n{2}", result.StatusCode, result.StatusDescription, reader.ReadToEnd());
            }
        }

        public ActionResult SmokeTest(string url)
        {
            using (var client = new WebClient())
                client.DownloadData(GetAbsoluteUrl(url));

            return Content("OK");
        }

        public ActionResult NuGetPushTest()
        {
            var helper = new Gateway.NuGet.Core.TestHelper();
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Packages.DemoLibrary.nupkg"))
                helper.Push(GetNuGetPushUrl(), "Test", stream);

            return Content("OK");
        }

        public ActionResult NuGetFeedTest()
        {
            var helper = new Gateway.NuGet.Core.TestHelper();
            var count = helper.Count(GetNuGetFeedUrl(), new NetworkCredential("Test", "Test"));
            return Content(string.Format("OK - {0} package(s)", count));
        }

        public ActionResult OpenWrapPushTest()
        {
            var helper = new Gateway.OpenWrap.Core.TestHelper();
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Packages.demolibrary.wrap"))
                helper.Push(GetOpenWrapUrl(), new NetworkCredential("Test", "Test"), "Test", stream);

            return Content("OK");
        }

        public ActionResult OpenWrapFeedTest()
        {
            var helper = new Gateway.OpenWrap.Core.TestHelper();
            var count = helper.Count(GetOpenWrapUrl(), new NetworkCredential("Test", "Test"));
            return Content(string.Format("OK - {0} package(s)", count));
        }
    }
}
