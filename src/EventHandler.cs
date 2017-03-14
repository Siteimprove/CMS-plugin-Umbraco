using SiteImprove.Umbraco.Plugin.MenuActions;
using SiteImprove.Umbraco.Plugin.Models;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web.Trees;

namespace SiteImprove.Umbraco.Plugin
{
    public class EventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;

            this.AddDbTable(applicationContext);
        }

        private void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            var node = sender.Umbraco.TypedContent(e.NodeId);

            if (sender.TreeAlias == "content" && node != null)
            {
                e.Menu.Items.Add(new SiteImproveRecheckMenuItem());
            }
        }

        private void AddDbTable(ApplicationContext applicationContext)
        {
            var db = applicationContext.DatabaseContext.Database;
            
            if (!db.TableExist(Constants.SiteImproveDbTalbe))
            {
                db.CreateTable<SiteImproveSettingsModel>(false);
                return;
            }
            
            // Handle legacy
            var row = db.Query<SiteImproveSettingsModel>("SELECT TOP 1 * FROM " + Constants.SiteImproveDbTalbe).FirstOrDefault();
            if (row != null && !row.Installed)
            {
                db.CreateTable<SiteImproveSettingsModel>(true);
            }
        }
    }
}