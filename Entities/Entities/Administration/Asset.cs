using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class Asset
    {
        public int IdAsset { get; set; }
        public string AssetName { get; set; }

        // Must be unique in the DB
        public string SerialNumber { get; set; }

        // Must be unique in the DB
        public string Plate { get; set; }

        public decimal OriginalCost { get; set; }
        public DateTime PurchaseDate { get; set; }

        public string Location { get; set; }

        public string AssetCondition { get; set; }

        public int IdAssetCategory { get; set; }
        public AssetCategory AssetCategory { get; set; }

        // Optional relationship with Model, can be null
        public int? IdModel { get; set; }
        public Model Model { get; set; }

        public int? IdLaw { get; set; }
        public Law Law { get; set; }
    }
}
