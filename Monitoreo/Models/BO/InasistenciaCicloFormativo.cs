using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class InasistenciaCicloFormativo
    {
        public int ID { get; set; }

        public virtual int CalendarioCicloFormativoID { get; set; }
        public virtual CalendarioCicloFormativo CalendarioCicloFormativo { get; set; }

        public int PersonaId { get; set; }
        [Display(Name = "Persona", ResourceType = typeof(Resources.T))]
        public virtual Persona Persona { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Multiline")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentario", ResourceType = typeof(Resources.T))]
        public string Comentario { get; set; }
    }
}