using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class Pathology
    {

        public int Id_Pathology { get; set; }

        public string Name_Pathology { get; set; }


        #region Relations 
        public ICollection<ResidentPathology> ResidentPathologies { get; set; }
        #endregion 
    }

}


