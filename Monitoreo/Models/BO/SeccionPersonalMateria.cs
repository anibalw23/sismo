using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class SeccionPersonalMateria
    {
        public int ID { get; set; }
        //SeccionAula
        public virtual SeccionAula SeccionAula { get; set; }
        public int? SeccionAulaID { get; set; }
        //PersonalMateria
        public virtual PersonalMateria PersonalMateria { get; set; }
        public int PersonalMateriaID { get; set; }

    }
}