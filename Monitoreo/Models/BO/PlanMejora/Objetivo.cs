using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.PlanMejora
{
    public class Objetivo
    {
        public int ID { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }

        //PlanId
        public virtual PlanMejoraCentro PlanMejoraCentro { get; set; }
        public int PlanMejoraCentroId { get; set; }
      
        //AmbitoId
        public virtual AmbitoObjetivo AmbitoObjetivo { get; set; }
        public int AmbitoObjetivoId { get; set; }
       
        //Metas
        public virtual ICollection<Meta> Metas { get; set; }

    }
}