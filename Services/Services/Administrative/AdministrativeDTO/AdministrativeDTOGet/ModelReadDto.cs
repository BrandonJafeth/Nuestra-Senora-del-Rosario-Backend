using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ModelReadDto
    {
        public int IdModel { get; set; }
        public string ModelName { get; set; }

        // Podrías incluir la marca
        public int IdBrand { get; set; }
        public string BrandName { get; set; }  // Opcional, para devolver el nombre de la marca
    }
}
