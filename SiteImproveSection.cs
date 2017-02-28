using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.businesslogic;
using umbraco.interfaces;
using Umbraco.Web.Mvc;

namespace SiteImprove.Umbraco.Plugin
{
    [PluginController("SiteImprove")]
    [Application("SiteImprove", "SiteImprove", "icon-car", 99)]
    public class SiteImproveSection : IApplication
    {
    }
}
