using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.PlanMejora
{
    public class Meta
    {
        public int ID { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        // Indicadores
        public virtual ICollection<IndicadorPlan> Indicadores { get; set; }

        //ObjetivoID
        public int ObjetivoId { get; set; }
        public virtual Objetivo Objetivo { get; set; }

        //PeriodoId
        public int PeriodoMetaId { get; set; }
        public virtual PeriodoMeta PeriodoMeta { get; set; }

    }
}