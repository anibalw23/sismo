using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class Seccion
    {
        public int Id { get; set; }
        [Display(Name = "Numero", ResourceType = typeof(Resources.T))]
        public string Numero { get; set; }
    }
}