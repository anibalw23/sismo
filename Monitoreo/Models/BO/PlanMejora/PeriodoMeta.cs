using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.PlanMejora
{
    public class PeriodoMeta
    {
        public int ID { get; set; }
        public string periodo { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
    }
}