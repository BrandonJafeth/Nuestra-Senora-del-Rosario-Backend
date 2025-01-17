using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.DTOS
{
    public class DonationTypeDto
    {
        public int Id_DonationType { get; set; }
        public string Name_DonationType { get; set; }

        // Lista de métodos de donación asociados al tipo de donación
        public ICollection<MethodDonationDto> MethodDonations { get; set; }
    }
}
