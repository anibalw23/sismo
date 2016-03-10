using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.PlanMejora
{
    public class ActividadEspecifica
    {
        public int ID { get; set; }
        public string nombre { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public string observaciones { get; set; }
        //ActividadID
        public int ActividadId { get; set; }
        public virtual Actividad Actividad { get; set; }

    }
}