using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoreo.Models
{
    public class Regional
    {
        public Regional()
        {
            this.Telefono = new Telefono();
        }

        public int Id { get; set; }

        [Display(Name = "Codigo", ResourceType = typeof(Resources.T))]
        public string Codigo { get; set; }

        [Display(Name = "Nombre", ResourceType = typeof(Resources.T))]
        public string Nombre { get; set; }

        [ForeignKey("Director")]
        public int? DirectorId { get; set; }

        [Display(Name = "Director", ResourceType = typeof(Resources.T))]
        public virtual PersonalAdministrativo Director { get; set; }

        public int? ProvinciaId { get; set; }

        [Display(Name = "Provincia", ResourceType = typeof(Resources.T))]
        public virtual Provincia Provincia { get; set; }

        public int? MunicipioId { get; set; }

        [Display(Name = "Municipio", ResourceType = typeof(Resources.T))]
        public virtual Municipio Municipio { get; set; }

        [Display(Name = "Sector", ResourceType = typeof(Resources.T))]
        public string Sector { get; set; }

        [Display(Name = "Calle", ResourceType = typeof(Resources.T))]
        public string Calle { get; set; }

        [Display(Name = "Telefono", ResourceType = typeof(Resources.T))]
        public Telefono Telefono { get; set; }

        [Display(Name = "CorreoElectronico", ResourceType = typeof(Resources.T))]
        public string CorreoElectronico { get; set; }

        [Display(Name = "SitioWeb", ResourceType = typeof(Resources.T))]
        public string SitioWeb { get; set; }

        public virtual ICollection<PersonalAdministrativo> Personal { get; set; }
    }
}