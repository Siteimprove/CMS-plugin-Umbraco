using Umbraco.Web.Models.Trees;

namespace SiteImprove.Umbraco.Plugin
{
    [ActionMenuItem("SiteImproveMenuActions", "Start")]
    public class SiteImproveStartMenuItem : ActionMenuItem
    {
        public SiteImproveStartMenuItem()
        {
            this.Icon = "map-alt";
            this.Name = "Start SiteImprove";
            this.SeperatorBefore = true;
        }
    }
}
