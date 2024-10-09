using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class DeductionCreateDto
    {
        public string Type { get; set; }  // Tipo de deducción
        public decimal Amount { get; set; }  // Monto de la deducción
    }

}
