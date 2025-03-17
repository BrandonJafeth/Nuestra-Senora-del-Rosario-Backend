using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class AssetCategory
    {
        public int IdAssetCategory { get; set; }
        public string CategoryName { get; set; }

        // One-to-many: one category can have many assets
        public ICollection<Asset> Assets { get; set; }
    }
}
