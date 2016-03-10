using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    [Flags]
    public enum PersonalFuncion : int
    {
        NoDefinida = 0,
        Directora = 1,
        SubDirectora = 2,
        CoordinadoraDocente = 4,
        SecretariaDocente = 8,
        Docente = 16,
        Orientadora = 32,
        Bibliotecaria = 64,
        ApoyoAdministrativo = 128
    }

    [Flags]
    public enum PersonalTanda : int
    {
        Matutina = 1,
        Vespertina = 2,
        Extendida = 4,
        Nocturna = 8,
        MatutinaVespertina = 16
    }

    public class Personal
    {
        public int Id { get; set; }

        //[Display(Name = "Codigo", ResourceType = typeof(Resources.T))]
        //public string Codigo { get; set; }

        [Required]
        public int PersonaId { get; set; }
        public virtual Persona Persona { get; set; }

        [Required]
        public int CentroId { get; set; }
        public virtual Centro Centro { get; set; }

        [Display(Name = "FechaContratacion", ResourceType = typeof(Resources.T))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaContratacion { get; set; }

        [Display(Name = "FuncionesEjerce", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Flags")]
        public PersonalFuncion FuncionesEjerce {get;set;}

        [System.ComponentModel.DataAnnotations.UIHint("Flags")]
        [Display(Name = "Tanda", ResourceType = typeof(Resources.T))]
        public PersonalTanda Tanda { get; set; }

        public bool isActive { get; set; } // indica si el docente esta o no activo


        public virtual ICollection<InscripcionActividadAcompanamiento> inscripcionesActividadesacompanamiento { get; set; }
    }
}