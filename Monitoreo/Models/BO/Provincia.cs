using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class Provincia
    {
        public int Id { get; set; }

        [Display(Name = "Codigo", ResourceType = typeof(Resources.T))]
        public string Codigo { get; set; }

        [Display(Name = "Nombre", ResourceType = typeof(Resources.T))]
        public string Nombre { get; set; }

        public virtual ICollection<Municipio> Municipios { get; set; }
    }
}