using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.PlanMejora
{
    public class IndicadorPlan
    {
        public int ID { get; set; }
        public string codigo { get; set; }
        public string indicador { get; set; }
        //Meta
        public int MetaId { get; set; }
        public virtual Meta Meta { get; set; }
    }
}