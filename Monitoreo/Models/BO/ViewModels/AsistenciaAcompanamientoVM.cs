using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.ViewModels
{
    public class AsistenciaAcompanamientoVM
    {
        public int actividadAcompId { get; set; }
        public int inscripcionID { get; set; }
        public TipoAcompanamiento tipo { get; set; }
        public int tipoAcompanamientoNum { get; set; }
        public DateTime fecha { get; set; }
        public int horas { get; set; }
        public bool asistio { get; set; }
        public string Area { get; set; }

        public string emptyRow { get; set; }

        public List<EvaluacionAcompanamiento.EvaluacionAcompanamiento> evaluacionesAcompanamiento { get; set; }
    }
}