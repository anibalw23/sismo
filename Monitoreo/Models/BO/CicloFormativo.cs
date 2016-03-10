using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{

    public enum TipoModuloFormativo : int
    {
        Seminario_Taller_Profundizacion = 1,
        Acompanamiento_Reflexivo_Practica = 2, 
        AcompanamientoTutorial = 4
    }

    public class CicloFormativo
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Tema", ResourceType = typeof(Resources.T))]
        public string Tema { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public TipoModuloFormativo tipo { get; set; }

        [Display(Name = "CreadoPor")]
        public string CreadoPor { get; set; }

        [Display(Name = "FechaInicio", ResourceType = typeof(Resources.T))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "FechaFinalizacion", ResourceType = typeof(Resources.T))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinalizacion { get; set; }


        public virtual SuperCicloFormativo SuperCicloFormativo { get; set; }
        public int SuperCicloFormativoId { get; set; }

        [Display(Name = "Nivel", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public NivelEducativo Nivel { get; set; }

        [Display(Name = "Area", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public DocenteArea Area { get; set; }

        [Display(Name = "Ciclo", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public DocenteCiclo Ciclo { get; set; }

        [Display(Name = "DuracionTotal", ResourceType = typeof(Resources.T))]
        public int DuracionTotal { get; set;}

        [Display(Name = "ActividadesFormativas", ResourceType = typeof(Resources.T))]
        public virtual ICollection<ActividadFormativa> ActividadesFormativas { get; set; }

        [Display(Name = "Inscripciones", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Inscripcion> Inscripciones { get; set; }

        [Display(Name = "Preguntas", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Pregunta> Preguntas { get; set; }

        [Display(Name = "Formadores", ResourceType = typeof(Resources.T))]
        [NotMapped]
        public ICollection<Inscripcion> Formadores
        { 
            get {
                List<Inscripcion> result = new List<Inscripcion>();
                if (this.Inscripciones != null) result = Inscripciones.Where(x => x.Rol == InscripcionRol.Formador).ToList();
                return result;
            }
        }

        [Display(Name = "Acompanantes", ResourceType = typeof(Resources.T))]
        [NotMapped]
        public ICollection<Inscripcion> Acompanantes
        {
            get
            {
                List<Inscripcion> result = new List<Inscripcion>();
                if (this.Inscripciones != null) result = Inscripciones.Where(x => x.Rol == InscripcionRol.Acompanante).ToList();
                return result;
            }
        }

        [Display(Name = "TutoresVirtuales", ResourceType = typeof(Resources.T))]
        [NotMapped]
        public ICollection<Inscripcion> TutoresVirtuales
        {
            get
            {
                List<Inscripcion> result = new List<Inscripcion>();
                if (this.Inscripciones != null) result = Inscripciones.Where(x => x.Rol == InscripcionRol.TutorVirtual).ToList();
                return result;
            }
        }

        [Display(Name = "Participantes", ResourceType = typeof(Resources.T))]
        [NotMapped]
        public ICollection<Inscripcion> Participantes
        {
            get
            {
                List<Inscripcion> result = new List<Inscripcion>();
                if (this.Inscripciones != null) result = Inscripciones.Where(x => x.Rol == InscripcionRol.Participante).ToList();
                return result;
            }
        }
    }
}