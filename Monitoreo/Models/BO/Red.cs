using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class Red
    {
        public int Id { get; set; }

        [Display(Name = "Codigo", ResourceType = typeof(Resources.T))]
        public string Codigo { get; set; }

        [Display(Name = "Nombre", ResourceType = typeof(Resources.T))]
        public string Nombre { get; set; }


        public int DistritoId { get; set; }

        [Display(Name = "Distrito", ResourceType = typeof(Resources.T))]
        public virtual Distrito Distrito { get; set; }

        public int? CentroSedeId { get; set; }

        [Display(Name = "CentroSede", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.ReadOnly(true)]
        public virtual Centro CentroSede { get; set; }

        [Display(Name = "Centros", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Centro> Centros { get; set; }
    }
}