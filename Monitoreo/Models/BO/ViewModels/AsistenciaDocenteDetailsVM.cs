using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels
{
    public class AsistenciaDocenteDetailsVM
    {
        public int calendarioCicloId { get; set; }
        public string nombreActividad { get; set; }
        public DateTime fecha { get; set; }
        public int horas { get; set;}
        public bool asistio { get; set; }
        public int cicloFormativoID { get; set; }
    }
}