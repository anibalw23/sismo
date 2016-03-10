using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public enum TipoDeAusencia
    {
        Justificada = 1, NoJustificada
    }

    public class Ausencia
    {
        public int Id { get; set; }

        //public int ActividadFormativaId { get; set; }

        //[Display(Name = "ActividadFormativa", ResourceType = typeof(Resources.T))]
        //public virtual ActividadFormativa ActividadFormativa { get; set; }

        public int PersonaId { get; set; }
        [Display(Name = "Persona", ResourceType = typeof(Resources.T))]
        public virtual Persona Persona { get; set; }


        public int CalendarioCicloFormativoId { get; set; }
        public virtual CalendarioCicloFormativo CalendarioCicloFormativo { get; set; }


        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "Tipo", ResourceType = typeof(Resources.T))]
        public TipoDeAusencia Tipo { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Multiline")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentario", ResourceType = typeof(Resources.T))]
        public string Comentario { get; set; }
    }
}