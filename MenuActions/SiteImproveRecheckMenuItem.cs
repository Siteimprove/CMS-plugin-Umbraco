using Umbraco.Web.Models.Trees;

namespace SiteImprove.Umbraco.Plugin.MenuActions
{
    [ActionMenuItem(Constants.SiteImproveMenuActionFactory, "Recheck")]
    class SiteImproveRecheckMenuItem : ActionMenuItem
    {
        public SiteImproveRecheckMenuItem()
        {
            this.Icon = "conversation";
            this.Name = "Recheck SiteImprove";
            this.SeperatorBefore = true;
        }
    }
}
