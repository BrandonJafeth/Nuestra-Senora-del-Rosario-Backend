using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.DTOS.CreatesDto
{
   public class NoteUpdateDTO
    {

        public string Reason { get; set; } 

        public DateTime NoteDate { get; set; }  

        public string Description { get; set; } 
    }
}
