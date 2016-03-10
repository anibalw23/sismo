using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class AusenciaReporte
    {
        public int ID { get; set; }

        public String titulo { get; set; }

        public int CicloFormativoId { get; set; }
        [Display(Name = "CicloFormativo", ResourceType = typeof(Resources.T))]
        public virtual CicloFormativo CicloFormativo { get; set; }

        public int SeccionId { get; set; }
        [Display(Name = "Grupo", ResourceType = typeof(Resources.T))]
        public Seccion Seccion { get; set; }

        [Display(Name = "Personas Objetivo")]
        public int NumeroPersonasObjetivo { get; set; }
        [Display(Name = "%Asistencia Objetivo")]
        public int AsistenciaObjetivo { get; set; }
        [Display(Name = "Horas Objetivo")]
        public int NumeroHorasObjetivo { get; set; }


    }
}