using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class AssetUpdateDto
    {
        public string AssetName { get; set; }
        public string SerialNumber { get; set; }
        public string Plate { get; set; }
        public decimal OriginalCost { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Location { get; set; }
        public string AssetCondition { get; set; }
        public int IdAssetCategory { get; set; }
        public int? IdModel { get; set; }
    }
}
