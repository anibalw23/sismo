using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class EvaluacionNotaRaw
    {
        public int Id { get; set; }
        public int nota { get; set; }

        public int notaMaxima { get; set; }

        [ForeignKey("Participante")]
        public int ParticipanteId { get; set; }
        [Display(Name = "Participante", ResourceType = typeof(Resources.T))]
        public virtual Persona Participante { get; set; }

        public int EvaluacionId { get; set; }
        [Display(Name = "Evaluacion", ResourceType = typeof(Resources.T))]
        public virtual Evaluacion Evaluacion { get; set; }



    }
}