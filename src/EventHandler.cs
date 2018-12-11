using SiteImprove.Umbraco.Plugin.MenuActions;
using SiteImprove.Umbraco.Plugin.Models;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using Umbraco.Web.Trees;

namespace SiteImprove.Umbraco.Plugin
{
    public class EventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;

            var helper = new UmbracoHelper(UmbracoContext.Current);
            var settingsHelper = new SiteImproveSettingsHelper(applicationContext.DatabaseContext, helper);
            settingsHelper.AddDbTable(applicationContext);
            settingsHelper.InitializeModel();
        }


        private void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            var node = sender.Umbraco.TypedContent(e.NodeId);

            if (sender.TreeAlias == "content" && node != null)
            {
                e.Menu.Items.Add(new SiteImproveRecheckMenuItem());
            }
        }
    }
}