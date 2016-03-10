using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class Componente
    {
        public int Id { get; set; }

        [Display(Name = "Descripcion", ResourceType = typeof(Resources.T))]
        public string Descripcion { get; set; }
    }
}