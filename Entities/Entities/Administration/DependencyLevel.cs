using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Administration
{
    public class DependencyLevel
    {
        [Key]
        public int Id_DependencyLevel { get; set; }

        [Required, MaxLength(50)]
        public string LevelName { get; set; }  // Nombre del nivel de dependencia
    }
}
