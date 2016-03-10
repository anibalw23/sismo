using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class Municipio
    {
        public int Id { get; set; }

        [Display(Name = "Codigo", ResourceType = typeof(Resources.T))]
        public string Codigo { get; set; }

        public string Nombre { get; set; }

        public int? ProvinciaId { get; set; }
        public virtual Provincia Provincia { get; set; }
    }
}