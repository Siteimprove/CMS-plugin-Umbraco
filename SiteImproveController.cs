using SiteImprove.Umbraco.Plugin.Db;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace SiteImprove.Umbraco.Plugin
{
    public class SiteImproveController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> GetToken()
        {
            return Json(await SiteImproveSettingsHelper.GetToken(ApplicationContext.DatabaseContext));
        }

        [HttpGet]
        public async Task<IHttpActionResult> RequestNewToken()
        {
            return Json(await SiteImproveSettingsHelper.GetNewToken(ApplicationContext.DatabaseContext));
        }

        [HttpGet]
        public IHttpActionResult GetPageUrl(int pageId)
        {
            return Json(Umbraco.NiceUrl(pageId));
        }
    }
}
