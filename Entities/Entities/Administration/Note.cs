using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Administration
{
    public class Note
    {
        [Key]
        public int Id_Note { get; set; }  // Primary key

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; }  // Reason or title of the note

        [Required]
        public DateTime NoteDate { get; set; }  // Date provided by the user

        public string Description { get; set; }  // Detailed description of the note

        public DateTime CreatedAt { get; set; }
    }

}