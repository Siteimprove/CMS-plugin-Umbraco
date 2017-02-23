using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Trees;

namespace SiteImprove.Umbraco.Plugin
{
    public class EventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;
            base.ApplicationStarted(umbracoApplication, applicationContext);
        }
        
        private void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if(sender.TreeAlias == "content")
            {
                e.Menu.Items.Add(new SiteImproveRecheckMenuItem());
            }
        }
    }
}