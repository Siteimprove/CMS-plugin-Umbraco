using SiteImprove.Umbraco.Plugin.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace SiteImprove.Umbraco.Plugin.Controllers
{
    public class SiteImproveController : UmbracoAuthorizedApiController
    {
        private SiteImproveSettingsHelper SettingsHelper { get; set; }

        public SiteImproveController()
        {
            this.SettingsHelper = new SiteImproveSettingsHelper(ApplicationContext.DatabaseContext, Umbraco);
        }


        [HttpGet]
        public async Task<HttpResponseMessage> GetSettings()
        {
            var model = new
            {
                token = await SettingsHelper.GetToken(),
                crawlingIds = SettingsHelper.GetCrawlIds()
            };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }


        [HttpGet]
        public async Task<HttpResponseMessage> GetToken()
        {
            return Request.CreateResponse(
                HttpStatusCode.OK,
                await SettingsHelper.GetToken());
        }

        [HttpGet]
        public async Task<HttpResponseMessage> RequestNewToken()
        {
            return Request.CreateResponse(
                HttpStatusCode.OK,
                await SettingsHelper.GetNewToken());
        }

        [HttpGet]
        public HttpResponseMessage GetCrawlingIds()
        {
            return Request.CreateResponse(
                HttpStatusCode.OK,
                SettingsHelper.GetCrawlIds());
        }

        [HttpPost]
        public HttpResponseMessage SetCrawlingIds([FromUri] string ids)
        {
            SettingsHelper.SetCrawlIds(ids);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage GetPageUrl(int pageId)
        {
            var node = Umbraco.TypedContent(pageId);

            var model = new
            {
                success = node != null,
                status = node != null ? "OK" : "No published page with that id",
                url = Umbraco.NiceUrlWithDomain(pageId)
            };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }
    }
}
