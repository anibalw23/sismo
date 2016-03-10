using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.EvaluacionAcompanamiento
{
    public class EvaluacionAcompanamientoRespuesta
    {
        public int Id { get; set; }

        public int EvaluacionAcompId { get; set; }
        [Display(Name = "Evaluacion")]
        public virtual EvaluacionAcompanamiento EvaluacionAcomp { get; set; }

        public int ? InscripcionActividadAcompanamientoId { get; set; }
        [Display(Name = "Actividad Acompañamiento", ResourceType = typeof(Resources.T))]
        public virtual InscripcionActividadAcompanamiento InscripcionActividadAcompanamiento { get; set; }


        public int PreguntaAcompId { get; set; }
        [Display(Name = "Pregunta")]
        public virtual EvaluacionAcompanamientoPregunta PreguntaAcomp { get; set; }
      

        [Display(Name = "Valor", ResourceType = typeof(Resources.T))]
        public string Valor { get; set; }

    }
}