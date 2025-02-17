using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class DependencyHistory
    {
        [Key]
        public int Id_History { get; set; }

        [ForeignKey("Resident")]
        public int Id_Resident { get; set; }
        public Resident Resident { get; set; }  // Relación con Resident (Residente)

        [ForeignKey("DependencyLevel")]
        public int Id_DependencyLevel { get; set; }
        public DependencyLevel DependencyLevel { get; set; }  // Relación con DependencyLevel (Nivel de dependencia)
    }

}
