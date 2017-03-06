using SiteImprove.Umbraco.Plugin.Db;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace SiteImprove.Umbraco.Plugin
{
    public class SiteImproveController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> GetToken()
        {
            return Request.CreateResponse(
                HttpStatusCode.OK, 
                await SiteImproveSettingsHelper.GetToken(ApplicationContext.DatabaseContext));
        }

        [HttpGet]
        public async Task<HttpResponseMessage> RequestNewToken()
        {
            return Request.CreateResponse(
                HttpStatusCode.OK, 
                await SiteImproveSettingsHelper.GetNewToken(ApplicationContext.DatabaseContext));
        }

        [HttpGet]
        public HttpResponseMessage GetPageUrl(int pageId)
        {
            var node = Umbraco.TypedContent(pageId);
            if(node == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No published page with that id");
            }

            return Request.CreateResponse(
                HttpStatusCode.OK,
                Umbraco.NiceUrlWithDomain(pageId)
                );
        }
    }
}
