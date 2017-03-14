using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Web;

namespace SiteImprove.Umbraco.Plugin.Models
{
    public class SiteImproveSettingsHelper
    {
        private DatabaseContext Ctx { get; set; }
        private UmbracoHelper Umbraco { get; set; }

        public SiteImproveSettingsHelper(DatabaseContext ctx, UmbracoHelper umbraco)
        {
            this.Ctx = ctx;
            this.Umbraco = umbraco;

            this.InitializeModel();
        }
        
        /// <summary>
        /// Returns the token that exist in the first row
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task<string> GetToken()
        {
            var result = GetFirstRow<SiteImproveSettingsModel>(Constants.SiteImproveDbTalbe);
            if (result == null)
            {
                // Token did not exist in database, fetch from SiteImprove
                string token = await RequestTokenAsync();

                var row = new SiteImproveSettingsModel { Token = token };
                Ctx.Database.Insert(row);

                return token;
            }

            return result.Token;
        }

        /// <summary>
        /// Updates the token in the first row, if row not created => create it
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task<string> GetNewToken()
        {
            var row = GetFirstRow<SiteImproveSettingsModel>(Constants.SiteImproveDbTalbe);
            if(row == null)
            {
                return await GetToken();
            }

            row.Token = await RequestTokenAsync();
            Ctx.Database.Update(row);

            return row.Token;
        }

        /// <summary>
        /// Get node id's that will execute the Siteimprove recrawling mehtod
        /// </summary>
        /// <returns></returns>
        public string GetCrawlIds()
        {
            var row = GetFirstRow<SiteImproveSettingsModel>(Constants.SiteImproveDbTalbe);
            if(row == null)
            {
                this.InitializeModel();
                return null;
            }

            // Handle legacy
            if(row.Installed == false)
            {
                row.Installed = true;

                var publishedRootPages = this.Umbraco.TypedContentAtRoot();
                row.CrawlIds = publishedRootPages.Any() ? publishedRootPages.First().Id.ToString() : null;
                Ctx.Database.Update(row);
            }

            return row.CrawlIds;
        }
        
        public void SetCrawlIds(string ids)
        {
            ids = ids ?? "";

            var row = GetFirstRow<SiteImproveSettingsModel>(Constants.SiteImproveDbTalbe);
            if(row == null)
            {
                row = GenerateDefaultModel(ids);
                Ctx.Database.Insert(row);
                return;
            }

            row.CrawlIds = ids;
            Ctx.Database.Update(row);
        }



        private void InitializeModel()
        {
            var row = GetFirstRow<SiteImproveSettingsModel>(Constants.SiteImproveDbTalbe);
            if (row == null)
            {
                row = GenerateDefaultModel();
                this.Ctx.Database.Insert(row);
            }
        }

        private SiteImproveSettingsModel GenerateDefaultModel()
        {
            var publishedRootPages = this.Umbraco.TypedContentAtRoot();
            
            return new SiteImproveSettingsModel
            {
                Installed = true,
                Token = RequestToken(),
                CrawlIds = publishedRootPages.Any() ? publishedRootPages.First().Id.ToString() : null
            };
        }
        private SiteImproveSettingsModel GenerateDefaultModel(string crawlingIds)
        {
            var model = GenerateDefaultModel();
            model.CrawlIds = crawlingIds;

            return model;
        }

        private T GetFirstRow<T>(string table) where T : class
        {
            var query = Ctx.Database.Query<T>("SELECT TOP 1 * FROM " + table);
            return query.Any() ? query.First() : null;
        }

        private async Task<string> RequestTokenAsync()
        {
            using (var client = new HttpClient())
            {
                string response = await client.GetStringAsync(Constants.SiteImproveTokenUrl);
                return JObject.Parse(response).GetValue("token").ToString();
            }
        }

        private string RequestToken()
        {
            using (var client = new HttpClient())
            {
                string response = client.GetStringAsync(Constants.SiteImproveTokenUrl).Result;
                return JObject.Parse(response).GetValue("token").ToString();
            }
        }
    }
}