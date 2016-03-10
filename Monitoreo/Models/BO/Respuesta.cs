using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class Respuesta
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha", ResourceType = typeof(Resources.T))]
        public DateTime Fecha { get; set; }

        [Display(Name = "Digitador", ResourceType = typeof(Resources.T))]
        public string Digitador { get; set; }

        public int EvaluacionId { get; set; }
        [Display(Name = "Evaluacion", ResourceType = typeof(Resources.T))]
        public virtual Evaluacion Evaluacion { get; set; }

        public int PreguntaId { get; set; }
        [Display(Name = "Pregunta", ResourceType = typeof(Resources.T))]
        public virtual Pregunta Pregunta { get; set; }

        [ForeignKey("Participante")]
        public int ParticipanteId { get; set; }
        [Display(Name = "Participante", ResourceType = typeof(Resources.T))]
        public virtual Persona Participante { get; set; }

        [Display(Name = "Valor", ResourceType = typeof(Resources.T))]
        public string Valor { get; set; }
    }
}