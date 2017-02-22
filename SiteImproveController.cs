using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;
using Newtonsoft.Json;

namespace SiteImprove.Umbraco.Plugin
{
    public class SiteImproveController : UmbracoAuthorizedApiController
    {
        private const string SiteImprovePropertyAlias = "SiteImproveToken";

        [HttpGet]
        public async Task<IHttpActionResult> GetToken()
        {
            // Get root node of application
            var rootType = Umbraco.TypedContentAtRoot().First();

            try
            {
                // Try to add a "NoGroup" property on the root node
                var created = CreatePropertyIfNotExist(rootType, "Textstring", SiteImprovePropertyAlias, "SiteImprove Token");
                if (created)
                {
                    Logger.Info(this.GetType(), "Created SiteImprove Token Property...");
                    rootType = Umbraco.TypedContentAtRoot().First(); // Get rootType again if created the property
                }
            }
            catch(Exception e)
            {
                Logger.Error(this.GetType(), "Error Creating SiteImprove Property on root type", e);
                return InternalServerError(e);
            }

            // Try to get token, if not exist
            string token = rootType.GetProperty(SiteImprovePropertyAlias).DataValue as string;
            if (string.IsNullOrEmpty(token))
            {
                var contentService = ApplicationContext.Services.ContentService;
                token = await GetNewToken();
                var content = contentService.GetById(rootType.Id);
                content.SetValue(SiteImprovePropertyAlias, token);

                contentService.SaveAndPublishWithStatus(content);
            }

            return Json(token);
        }

        [HttpGet]
        public IHttpActionResult GetPageUrl(int pageId)
        {
            return Json(Umbraco.NiceUrl(pageId));
        }


        private async Task<string> GetNewToken()
        {
            using (var client = new HttpClient())
            {
                return JsonConvert.DeserializeObject<dynamic>(await client.GetStringAsync("https://overlay.siteimprove.com/auth/token"))["token"];
            }
        }

        private bool CreatePropertyIfNotExist(IPublishedContent model, string typeDefName, string newPropAlias, string name)
        {
            var contentType = ApplicationContext.Services.ContentTypeService.GetContentType(model.DocumentTypeAlias);
            var exist = contentType.PropertyTypeExists(newPropAlias);
            
            if (exist)
                return false;

            var typeDefinition = ApplicationContext.Services.DataTypeService.GetAllDataTypeDefinitions().FirstOrDefault(s => s.Name == typeDefName);
            if(typeDefinition == null)
            {
                throw new Exception("Could not find property type definition: " + typeDefName);
            }

            var newProp = new PropertyType(typeDefinition, newPropAlias);
            newProp.Name = name;
            newProp.Description = "SiteImprove Plugin Specific Property";

            contentType.AddPropertyType(newProp);
            ApplicationContext.Services.ContentTypeService.Save(contentType);

            return true;
        }
    }
}
