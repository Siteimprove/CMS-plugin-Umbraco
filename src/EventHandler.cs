using SiteImprove.Umbraco.Plugin.MenuActions;
using SiteImprove.Umbraco.Plugin.Models;
using System.Linq;
using System.Net;
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
            //force all outgoing connections to TLS 1.2 first
            //(it still falls back to 1.1 / 1.0 if the remote doesn't support 1.2).
            if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) == false)
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            }

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