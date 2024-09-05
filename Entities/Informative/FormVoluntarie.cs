using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Informative
{
    public class FormVoluntarie
    {
        public int Id_FormVoluntarie { get; set; }
        public string Vn_Name { get; set; }
        public string Vn_Lastname1 { get; set; }
        public string Vn_Lastname2 { get; set; }
        public int Vn_Cedula { get; set; }
        public string Vn_Phone { get; set; }
        public string Vn_Email { get; set; }
        public DateTime Delivery_Date { get; set; }
        public DateTime End_Date { get; set; }

        // Foreign Key - VoluntarieType
        public int Id_VoluntarieType { get; set; }
        public VoluntarieType VoluntarieType { get; set; }
    }
}
