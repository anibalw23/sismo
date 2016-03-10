using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO.EvaluacionAcompanamiento
{

  

    public class PreguntaOpcionAcompanamiento
    {
        public int Id { get; set; }

        public int EvalAcompPreguntaId { get; set; }
        [Display(Name = "Pregunta")]
        public virtual EvaluacionAcompanamientoPregunta Pregunta { get; set; }

        [Display(Name = "Titulo", ResourceType = typeof(Resources.T))]
        [Required]
        public string Titulo { get; set; }

        [Display(Name = "Valor", ResourceType = typeof(Resources.T))]
        [Required]
        public string Valor { get; set; }

        [Display(Name = "Correcta", ResourceType = typeof(Resources.T))]
        public bool Correcta { get; set; }
    }

    public class EvaluacionAcompanamientoPregunta
    {
        public EvaluacionAcompanamientoPregunta()
        {
            Opciones = new List<PreguntaOpcionAcompanamiento>();
        }

        public int Id { get; set; }

        [Display(Name = "Descripcion", ResourceType = typeof(Resources.T))]
        [Required]
        public string Descripcion { get; set; }

        [Display(Name = "Código")]
        public string codigo { get; set; }

        public int? EvaluacionAcompId { get; set; }
        [Display(Name = "Evaluacion")]
        public virtual EvaluacionAcompanamiento EvaluacionAcomp { get; set; }
        
        public virtual List<PreguntaOpcionAcompanamiento> Opciones { get; set; }

    }
}