using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class GradoLookup
    {
        public int Id { get; set; }
        //Nivel
        public int nivelId { get; set; }
        public virtual NivelGrado nivel { get; set; }
        //Ciclo
        public int cicloId { get; set; }
        public virtual CicloGrado ciclo { get; set; }
        //grado
        public string grado { get; set; }
    }
}