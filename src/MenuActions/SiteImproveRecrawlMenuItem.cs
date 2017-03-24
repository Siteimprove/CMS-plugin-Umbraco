using Umbraco.Web.Models.Trees;

namespace SiteImprove.Umbraco.Plugin.MenuActions
{
    [ActionMenuItem(Constants.SiteImproveMenuActionFactory, "Recrawl")]
    public class SiteImproveRecrawlMenuItem : ActionMenuItem
    {
        public SiteImproveRecrawlMenuItem()
        {
            this.Icon = "map-alt";
            this.Name = "Recrawl Siteimprove";
            this.SeperatorBefore = true;
        }
    }
}