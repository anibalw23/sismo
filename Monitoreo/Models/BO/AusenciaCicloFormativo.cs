using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public enum TandaAusencia
    {
        Matutina = 1, Vespertina
    }


    public class AusenciaCicloFormativo
    {
        public int ID { get; set; }


        public int CalendarioCicloFormativoId { get; set; }
        [Display(Name = "CalendarioCicloFormativo")]
        public virtual CalendarioCicloFormativo CalendarioCicloFormativo { get; set; }

        [ForeignKey("Participante")]
        public int ParticipanteId { get; set; }
        public virtual Personal Participante { get; set; }
        
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "Tipo" )]
        public TipoDeAusencia Tipo { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Multiline")]
        [DataType(DataType.MultilineText)]
        public string Comentario { get; set; }
    }
}