using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class MedicalHistory
    {
        [Key]
        public int Id_MedicalHistory { get; set; }  // Identificador único del historial

        [ForeignKey("Resident")]
        public int Id_Resident { get; set; }        // Clave foránea que relaciona con el residente
        public Resident Resident { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now; // Fecha de creación

        // Se actualiza cada vez que se edita el historial
        public DateTime? EditDate { get; set; }

        [Required, MaxLength(1000)]
        public string Diagnosis { get; set; }      // Resumen o descripción del diagnóstico

        [MaxLength(1000)]
        public string Treatment { get; set; }       // Descripción del tratamiento o intervención aplicada

        public string Observations { get; set; }     // Notas adicionales del personal médico

    }
}
