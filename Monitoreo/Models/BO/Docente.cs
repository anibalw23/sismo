using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    public enum NivelEducativo : int
    {
        [Display(Name = "NA", ResourceType = typeof(Resources.T))]
        NA = 1,
        Inicial = 2,
        Primaria = 4,
        Secundaria = 8,
        Todos = 16
    }

    [Flags]
    public enum DocenteArea : int
    {
        [Display(Name = "CuatroBasicas", ResourceType = typeof(Resources.T))]
        CuatroBasicas = 1,
        [Display(Name = "Matemática", ResourceType = typeof(Resources.T))]
        Matemática = 2,
        [Display(Name = "Inicial", ResourceType = typeof(Resources.T))]
        Inicial = 4,
        [Display(Name = "Sociales", ResourceType = typeof(Resources.T))]
        Sociales = 8,
        [Display(Name = "Lengua_Española", ResourceType = typeof(Resources.T))]
        LenguaEspañola = 16,
        [Display(Name = "Educación_Artística", ResourceType = typeof(Resources.T))]
        EducaciónArtística = 32,
        [Display(Name = "Ciencias_Naturales", ResourceType = typeof(Resources.T))]
        CienciasNaturales = 64,
        [Display(Name = "Educación_Física", ResourceType = typeof(Resources.T))]
        EducaciónFísica = 128,
        [Display(Name = "Lenguas_Extranjeras", ResourceType = typeof(Resources.T))]
        LenguasExtranjeras = 256,
        [Display(Name = "Formación_Humana", ResourceType = typeof(Resources.T))]
        FormaciónHumana = 1024,
        [Display(Name = "Informática", ResourceType = typeof(Resources.T))]
        Informática = 2048,
        [Display(Name = "Agropecuaria", ResourceType = typeof(Resources.T))]
        Agropecuaria = 4096,
        [Display(Name = "NA", ResourceType = typeof(Resources.T))]
        NA = 8192,
        [Display(Name = "Biblioteca", ResourceType = typeof(Resources.T))]
        Biblioteca = 16384,
        [Display(Name = "Todos")]
        Todos = 32768
    }

    [Flags]
    public enum DocenteCiclo : int
    {
        PrimerCiclo = 1, SegundoCiclo = 2, SeptimoOctavo = 4, NA = 8, Todos = 16
    }

    [Flags]
    public enum DocenteFormacionAcademica : int
    {
        [Display(Name = "Tecnico", ResourceType = typeof(Resources.T))]
        Tecnico = 1,
        [Display(Name = "TecnicoProfesional", ResourceType = typeof(Resources.T))]
        TecnicoProfesional = 2,
        Universitario = 4, 
        Especialidad = 8,
        [Display(Name = "Maestria", ResourceType = typeof(Resources.T))]
        Maestria = 16, 
        Doctorado = 32
    }

    [Flags]
    public enum Grado : int
    {
        [Display(Name = "Pre-Primario")]
        PrePrimario = 16384,
        [Display(Name = "Pre-Kinder")]
        PreKinder = 32768,
        [Display(Name = "Kinder")]
        Kinder = 65536,
        [Display(Name = "Inicial", ResourceType = typeof(Resources.T))]
        NivelInicial = 1,
        Primero = 2,
        Segundo = 4,
        Tercero = 8,
        Cuarto = 16,
        Quinto = 32,
        Sexto = 64,
        Septimo = 128,
        Octavo = 256,
        [Display(Name = "PrimeroSecundaria", ResourceType = typeof(Resources.T))]
        PrimeroSecundaria = 512,
        [Display(Name = "SegundoSecundaria", ResourceType = typeof(Resources.T))]
        SegundoSecundaria = 1024,
        [Display(Name = "TerceroSecundaria", ResourceType = typeof(Resources.T))]
        TerceroSecundaria = 2048,
        [Display(Name = "CuartoSecundaria", ResourceType = typeof(Resources.T))]
        CuartoSecundaria = 4096,
        [Display(Name = "NA", ResourceType = typeof(Resources.T))]
        NA = 8192,
    }



    public class DocenteMateria
    {
        public int Id { get; set; }

        public int DocenteId { get; set; }
        public Docente Docente { get; set; }

        [Display(Name = "Nivel", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public NivelEducativo Nivel { get; set; }

        [Display(Name = "Ciclo", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        public DocenteCiclo Ciclo { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "Tanda", ResourceType = typeof(Resources.T))]
        public PersonalTanda Tanda { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "Area", ResourceType = typeof(Resources.T))]
        public DocenteArea Area { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "Grados", ResourceType = typeof(Resources.T))]
        public Grado Grados { get; set; }

        public int? SeccionId { get; set; }
        [Display(Name = "Seccion", ResourceType = typeof(Resources.T))]
        public virtual Seccion Seccion { get; set; }
    }

    public class Docente : Personal
    {
        public Docente()
        {
            this.Materias = new List<DocenteMateria>();
        }

        [Display(Name = "FormacionAcademica", ResourceType = typeof(Resources.T))]
        [System.ComponentModel.DataAnnotations.UIHint("Flags")]
        public DocenteFormacionAcademica FormacionAcademica { get; set; }

        [Display(Name = "AnosDeEjercicio", ResourceType = typeof(Resources.T))]
        public int? AnosDeEjercicio { get; set; }

        [UIHint("DocenteMateriaList")]
        public virtual List<DocenteMateria> Materias { get; set; }

        [UIHint("DocenteMateriaList")]
        public virtual List<PersonalMateria> PersonalMaterias { get; set; }

    }
}