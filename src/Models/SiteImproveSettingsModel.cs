using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace SiteImprove.Umbraco.Plugin.Models
{
    [TableName(Constants.SiteImproveDbTalbe)]
    [PrimaryKey("id", autoIncrement = true)]
    public class SiteImproveSettingsModel
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Token")]
        public string Token { get; set; }
    }
}
