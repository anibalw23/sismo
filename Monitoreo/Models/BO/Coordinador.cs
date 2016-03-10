using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public class Coordinador 
    {
        public int Id { get; set; }

        public string Email { get; set; }

        [Required]
        public int PersonaId { get; set; }
        public virtual Persona Persona { get; set; }

        public virtual ICollection<CoordinadorRedes> CoordinadorRedes { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public NivelEducativo nivel { get; set; }

    }
}