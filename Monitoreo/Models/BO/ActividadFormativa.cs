using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public enum TipoActividadFormativa : int
    {
        Presencial = 1, Acompanamiento, Virtual
    }

    public interface IActividadFormativa
    {
        int Id { get; set; }

        TipoActividadFormativa Tipo { get; set; }

        int Duracion { get; set; }

        string Organizacion { get; set; }

        string Actores { get; set; }

        string ModoEvaluacion { get; set; }
    }

    public class ActividadFormativa
    {
        public int Id { get; set; }

        public int ActividadFormativaBaseId { get; set; }
        [Display(Name = "ActividadFormativaBase", ResourceType = typeof(Resources.T))]
        public ActividadFormativaBase ActividadFormativaBase { get; set; }

        public int CicloFormativoId { get; set; }
        [Display(Name = "CicloFormativo", ResourceType = typeof(Resources.T))]
        public CicloFormativo CicloFormativo { get; set; }

        [Display(Name = "Duracion", ResourceType = typeof(Resources.T))]
        public int Duracion { get; set; }

        [Display(Name = "FechaInicio", ResourceType = typeof(Resources.T))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "FechaFin", ResourceType = typeof(Resources.T))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Ausencias", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Ausencia> Ausencias { get; set; }
    }

    public class ActividadFormativaBase : IActividadFormativa
    {
        public int Id { get; set; }

        public TipoActividadFormativa Tipo { get; set; }

        [Display(Name = "Duracion", ResourceType = typeof(Resources.T))]
        public int Duracion { get; set; }

        [Display(Name = "Organizacion", ResourceType = typeof(Resources.T))]
        public string Organizacion { get; set; }

        public string Actores { get; set; }

        [Display(Name = "ModoEvaluacion", ResourceType = typeof(Resources.T))]
        public string ModoEvaluacion { get; set; }
    }
}