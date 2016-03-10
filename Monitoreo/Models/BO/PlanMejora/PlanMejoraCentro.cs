using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.PlanMejora
{
    public class PlanMejoraCentro
    {
        public int ID { get; set; }
        public string nombre { get; set; }
        public string periodo { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        //Centro
        public int CentroId { get; set; }
        public virtual Centro Centro { get; set; }

        //Objetivos
        public virtual ICollection<Objetivo> Objetivos { get; set; }

    }
}