using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class AssetReadDto
    {
        public int IdAsset { get; set; }
        public string AssetName { get; set; }
        public string SerialNumber { get; set; }
        public string Plate { get; set; }
        public decimal OriginalCost { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Location { get; set; } 

        public string AssetCondition { get; set; }
        // Relaciones (IDs):
        public int IdAssetCategory { get; set; }
        public int? IdModel { get; set; }

        // Campos adicionales para lectura (opcionales):
        public string CategoryName { get; set; }  // Podrías cargarlo desde AssetCategory
        public string ModelName { get; set; }     // Podrías cargarlo desde Model
        public string BrandName { get; set; }     // O desde la relación Model -> Brand

        public string LawName { get; set; }
    }
}
