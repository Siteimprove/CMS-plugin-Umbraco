using Umbraco.Web.Models.Trees;

namespace SiteImprove.Umbraco.Plugin
{
    [ActionMenuItem("SiteImproveMenuActions", "Check")]
    public class SiteImproveMenuItem : ActionMenuItem
    {
        public SiteImproveMenuItem()
        {
            this.Name = "Check SiteImprove";
            this.SeperatorBefore = true;
        }
    }
}
