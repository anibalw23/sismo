using Monitoreo.Models.BO.EvaluacionAcompanamiento;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{

    public enum CategoriaSuperCiclo : int
    {
        Docentes = 1,
        Gestion = 2,
        Transversal = 4
    }


    public class SuperCicloFormativo
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }
        public virtual ICollection<CicloFormativo> CiclosFormativos { get; set; }

        public virtual ICollection<EvaluacionAcompanamiento> EvaluacionAcompanamiento { get; set; }

        [Display(Name = "Nivel", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public NivelEducativo Nivel { get; set; }

        [Display(Name = "Area", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public DocenteArea Area { get; set; }

        [Display(Name = "Ciclo", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public DocenteCiclo Ciclo { get; set; }

        [Display(Name = "FechaInicio", ResourceType = typeof(Resources.T))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "FechaFinalizacion", ResourceType = typeof(Resources.T))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinalizacion { get; set; }

        [Display(Name = "Creado Por")]
        public string CreadoPor { get; set; }

        //Categoria del Ciclo Formativo
        [Display(Name = "Categoria")]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public CategoriaSuperCiclo CategoriaSuperCiclo { get; set; }

    }
}