using SiteImprove.Umbraco.Plugin.Models;
using SiteImprove.Umbraco.Plugin.MenuActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
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
            if(sender.TreeAlias == "content")
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
            }
        }
    }
}