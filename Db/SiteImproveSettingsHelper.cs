using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace SiteImprove.Umbraco.Plugin.Db
{
    public class SiteImproveSettingsHelper
    {
        public static async Task<string> GetToken(DatabaseContext ctx)
        {
            var query = ctx.Database.Query<SiteImproveSettingsModel>("SELECT TOP 1 Token FROM " + Constants.SiteImproveDbTalbe);
            if (!query.Any())
            {
                // Token did not exist in database, fetch from SiteImprove
                string token = await RequestToken();

                var row = new SiteImproveSettingsModel { Token = token };
                ctx.Database.Insert(row);

                return token;
            }

            return query.First().Token;
        }

        public static async Task<string> GetNewToken(DatabaseContext ctx)
        {
            var query = ctx.Database.Query<SiteImproveSettingsModel>("SELECT TOP 1 Token FROM " + Constants.SiteImproveDbTalbe);
            if(!query.Any())
            {
                return await GetToken(ctx);
            }

            var row = query.First();
            row.Token = await RequestToken();
            ctx.Database.Update(row);

            return row.Token;
        }



        private static async Task<string> RequestToken()
        {
            using (var client = new HttpClient())
            {
                return JsonConvert.DeserializeObject<dynamic>(await client.GetStringAsync(Constants.SiteImproveTokenUrl))["token"];
            }
        }
    }
}