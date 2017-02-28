using SiteImprove.Umbraco.Plugin.Db;
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
            //this.AddSiteImproveSection(applicationContext);
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
            var ctx = applicationContext.DatabaseContext;
            var db = new DatabaseSchemaHelper(ctx.Database, applicationContext.ProfilingLogger.Logger, ctx.SqlSyntax);

            if (!db.TableExist(Constants.SiteImproveDbTalbe))
            {
                db.CreateTable<SiteImproveSettingsModel>(false);
            }
        }

        private void AddSiteImproveSection(ApplicationContext applicationContext)
        {
            applicationContext.Services.SectionService.MakeNew("SiteImprove", "SiteImprove", "icon-car");
        }
    }
}