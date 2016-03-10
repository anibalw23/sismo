using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class Acompanante
    {
        public int Id { get; set; }

        public string Email { get; set; }

        [Required]
        public int PersonaId { get; set; }
        public virtual Persona Persona { get; set; }

        public virtual Centro Centro { get; set; }
        public int centroId { get; set; }
       
    }
}