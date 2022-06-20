using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
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
        }

        public void InitializeModel()
        {
            var row = GetFirstRow<SiteImproveSettingsModel>(Constants.SiteImproveDbTalbe);
            if (row == null)
            {
                row = GenerateDefaultModel();
                this.Ctx.Database.Insert(row);
            }
        }

        public void AddDbTable(ApplicationContext applicationContext)
        {
            var db = applicationContext.DatabaseContext.Database;

            if (!db.TableExist(Constants.SiteImproveDbTalbe))
            {
                db.CreateTable<SiteImproveSettingsModel>(false);
                return;
            }

            // Handle legacy
            var row = db
                .Query<SiteImproveSettingsModel>(
                    SiteImproveSettingsHelper.SelectTopQuery(
                        applicationContext.DatabaseContext.DatabaseProvider, 1, Constants.SiteImproveDbTalbe))
                .FirstOrDefault();

            if (row != null && !row.Installed)
            {
                db.CreateTable<SiteImproveSettingsModel>(true);
            }
        }

        public static string SelectTopQuery(DatabaseProviders databaseProviders, int number, string table)
        {
            switch (databaseProviders)
            {
                case DatabaseProviders.SqlAzure:
                case DatabaseProviders.SqlServerCE:
                case DatabaseProviders.SqlServer:
                    return string.Format("SELECT TOP {0} * FROM {1}", number, table);

                case DatabaseProviders.PostgreSQL:
                case DatabaseProviders.SQLite:
                case DatabaseProviders.MySql:
                    return string.Format("SELECT * FROM {1} LIMIT {0}", number, table);

                case DatabaseProviders.Oracle:
                    return string.Format("SELECT * FROM {1} WHERE ROWNUM<={0}", number, table);

                default:
                    return string.Format("SELECT TOP {0} * FROM {1}", number, table);
            }
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
            if (row == null)
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

            // Handle legacy
            if (row.Installed == false)
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
            if (row == null)
            {
                row = GenerateDefaultModel(ids);
                Ctx.Database.Insert(row);
                return;
            }

            row.CrawlIds = ids;
            Ctx.Database.Update(row);
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
            var query = Ctx.Database.Query<T>(SelectTopQuery(Ctx.DatabaseProvider, 1, table));
            return query.Any() ? query.First() : null;
        }

        /// <summary>
        /// A simple helper to append the cms param to the SiteImproveTokenUrl
        /// to accurately pass to the API the cms version of Umbraco calling to it.
        /// Example Url: https://my2.siteimprove.com/auth/token?cms=Umbraco-7.15.6
        /// 
        /// This is a "fix" for Umbraco 7 use of this plugin as the my2.siteimprove/auth/token
        /// endpoint now requires a cms param. 
        /// </summary>
        /// <returns></returns>
        private string GetSiteImproveTokenUrlWithCmsParam()
        {
            var siTokenUrlWithCmsParam = Constants.SiteImproveTokenUrl;

            if (siTokenUrlWithCmsParam.Contains("?") == false)
            {
                siTokenUrlWithCmsParam += "?";
            }
            else
            {
                siTokenUrlWithCmsParam += "&";
            }

            var cmsParam = "cms=Umbraco-7.0.0";

            // attempt to get specific Umbraco version...
            var umbVersion = ConfigurationManager.AppSettings["umbracoConfigurationStatus"] as string;
            if (string.IsNullOrWhiteSpace(umbVersion) == false)
            {
                cmsParam = $"cms=Umbraco-{umbVersion}";
            }

            // concatenate the cms param...
            siTokenUrlWithCmsParam = siTokenUrlWithCmsParam + cmsParam;

            return siTokenUrlWithCmsParam;
        }

        private async Task<string> RequestTokenAsync()
        {
            using (var client = new HttpClient())
            {
                string response = await client.GetStringAsync(GetSiteImproveTokenUrlWithCmsParam());
                return JObject.Parse(response).GetValue("token").ToString();
            }
        }

        private string RequestToken()
        {
            using (var client = new HttpClient())
            {
                string response = client.GetStringAsync(GetSiteImproveTokenUrlWithCmsParam()).Result;
                return JObject.Parse(response).GetValue("token").ToString();
            }
        }
    }
}