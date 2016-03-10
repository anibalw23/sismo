using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.PlanMejora
{
    public class Actividad
    {
        public int ID { get; set; }
        public string nombre { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }

        //ObjetivoID
        public int ObjetivoId { get; set; }
        public virtual Objetivo Objetivo { get; set; }

        //Actividades Especificas
        public virtual ICollection<ActividadEspecifica> actividadesEspecificas { get; set; }
    }
}