using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public enum PersonaSexo
    {
        Masculino = 1 , Femenino = 2
    }

    [Flags]
    public enum PersonaDiscapacidad : int
    {
        Motriz = 1,
        Visual = 2,
        Mental = 4,
        Auditiva = 8
    }

    [ComplexType]
    public class Telefono
    {
        [System.ComponentModel.DataAnnotations.Phone]
        [System.ComponentModel.DataAnnotations.UIHint("Phone")]
        public string Fijo { get; set; }

        [System.ComponentModel.DataAnnotations.Phone]
        [System.ComponentModel.DataAnnotations.UIHint("Phone")]
        public string Celular { get; set; }

        [System.ComponentModel.DataAnnotations.Phone]
        [System.ComponentModel.DataAnnotations.UIHint("Phone")]
        public string Otro { get; set; }
    }

    public class Persona
    {
        public Persona()
        {
            this.Telefono = new Telefono();
        }

        public int Id { get; set; }

        //[Required] Wagner: La cédula no es obligatoria para los estudiantes
        [Display(Name = "Cedula", ResourceType = typeof(Resources.T))]
        [RegularExpression("^\\d{3}-\\d{7}-\\d{1}$$", ErrorMessage = "Cédula Invalida. Ejemplo: 001-0000001-1")]
        public string Cedula { get; set; }

        [Display(Name = "NombreCompleto", ResourceType = typeof(Resources.T))]
        public string NombreCompleto
        {
            get
            {
                return String.Format("{0} {1} {2}", this.Nombres, this.PrimerApellido, this.SegundoApellido);
            }
        }

        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Nombres { get; set; }

        [Required]
        [Display(Name = "PrimerApellido", ResourceType = typeof(Resources.T))]
        public string PrimerApellido { get; set; }

        [Display(Name = "SegundoApellido", ResourceType = typeof(Resources.T))]
        public string SegundoApellido { get; set; }

        [EmailAddress]
        public string mail { get; set; }

        [Display(Name = "FechaNacimiento", ResourceType = typeof(Resources.T))]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "LugarNacimiento", ResourceType = typeof(Resources.T))]
        public string LugarNacimiento { get; set; }

        [Display(Name = "Sexo", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public PersonaSexo? Sexo { get; set; }

        [Display(Name = "Telefono", ResourceType = typeof(Resources.T))]
        public Telefono Telefono { get; set; }

        public int? ProvinciaId { get; set; }
        public virtual Provincia Provincia { get; set; }

        public int? MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }

        public string Sector { get; set; }

        public string Calle { get; set; }

        public int? GrupoEtnicoId { get; set; }

        [Display(Name = "GrupoEtnico", ResourceType = typeof(Resources.T))]
        public virtual GrupoEtnico GrupoEtnico { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Flags")]
        public PersonaDiscapacidad Discapacidades { get; set; }

        public string Comentarios { get; set; }

        public virtual ICollection<Inscripcion> inscripciones { get; set; }

        public virtual ICollection<Evaluacion> evaluaciones { get; set; } //esto lo agrege ahora

        public int Edad 
        { 
            get 
            {
                if (this.FechaNacimiento.HasValue)
                {
                    return DateTime.Now.Subtract(this.FechaNacimiento.Value).Days / 365;
                }
                else return 0;
            }
          
        }
    }
}