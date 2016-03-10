using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public enum InscripcionRol
    {
        [Display(Name = "Participante", ResourceType = typeof(Resources.T))]
        Participante,
        [Display(Name = "Formador", ResourceType = typeof(Resources.T))]
        Formador,
        [Display(Name = "Acompanante", ResourceType = typeof(Resources.T))]
        Acompanante,
        [Display(Name = "TutorVirtual", ResourceType = typeof(Resources.T))]
        TutorVirtual

    }

    public class Inscripcion
    {
        public int Id { get; set; }

        public int CicloFormativoId { get; set; }
        [Display(Name = "CicloFormativo", ResourceType = typeof(Resources.T))]
        public virtual CicloFormativo CicloFormativo { get; set; }

        public int GrupoCicloFormativoId { get; set; }
        [Display(Name = "Grupo", ResourceType = typeof(Resources.T))]
        public Models.BO.GrupoCicloFormativo GrupoCicloFormativo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha", ResourceType = typeof(Resources.T))]
        public DateTime Fecha { get; set; }

        [ForeignKey("Participante")]
        public int ParticipanteId { get; set; }
        [Display(Name = "Participante", ResourceType = typeof(Resources.T))]
        public virtual Persona Participante { get; set; }

        [Display(Name = "Rol", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public InscripcionRol Rol { get; set; }
    }
}