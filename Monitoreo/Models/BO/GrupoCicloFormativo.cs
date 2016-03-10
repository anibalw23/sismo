using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class GrupoCicloFormativo
    {
        public int ID { get; set; }
        
        //public string codigo { get; set; }
        //public string nombre { get; set; }

        public int CentroID { get; set; }
        public virtual Centro Centro { get; set; }

        public int CicloFormativoId { get; set; }
        public virtual CicloFormativo CicloFormativo { get; set; }

        public virtual ICollection<Inscripcion> inscripciones { get; set; }
    }
}