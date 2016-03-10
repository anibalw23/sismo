using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Monitoreo.Models
{
    [Flags]
    public enum NivelesCentro : int
    {
        Inicial = 1,
        Primaria = 2,
        Secundaria = 4
    }

    [Flags]
    public enum TandasCentro : int
    {
        Matutina = 1,
        Vespertina = 2,
        Nocturna = 4,
        Extendida = 8
    }

    [Flags]
    public enum Frecuencia : int
    {
        Semanal = 1,
        Quincenal = 2,
        Mensual = 4,
        Cuatrimestral = 8,
        Anual = 16
    }

    public enum SalonMultiUso : int
    {
        [Display(Name = "NoTiene", ResourceType = typeof(Resources.T))]
        NoTiene = 0,
        [Display(Name = "EnMalasCondiciones", ResourceType = typeof(Resources.T))]
        EnMalasCondiciones = 1,
        [Display(Name = "EnBuenasCondiciones", ResourceType = typeof(Resources.T))]
        EnBuenasCondiciones = 2
    }

    public enum LabInformatica : int
    {
        [Display(Name = "NoTiene", ResourceType = typeof(Resources.T))]
        NoTiene = 0,
        [Display(Name = "SinInternet", ResourceType = typeof(Resources.T))]
        SinInternet = 1,
        [Display(Name = "ConInternet", ResourceType = typeof(Resources.T))]
        ConInternet = 2
    }

    public enum Lavamanos : int
    {
        NoTiene = 0,
        SinAguaCorriente = 1,
        ConAguaCorriente = 2
    }

    public enum Sector : int
    {
        Indefinido = 0,
        [Display(Name = "Publico", ResourceType = typeof(Resources.T))]
        Publico = 1,
        Privado = 2,
        Mixto = 3
    }

    [Flags]
    public enum InstrumentosDeGestion : int
    {
        [Display(Name = "DocumentoDeRevision", ResourceType = typeof(Resources.T))]
        DocumentoDeRevision = 1,
        [Display(Name = "ProyectoEducativoDeCentro", ResourceType = typeof(Resources.T))]
        ProyectoEducativoDeCentro = 2,
        [Display(Name = "ProyectoCurricularDeCentro", ResourceType = typeof(Resources.T))]
        ProyectoCurricularDeCentro = 4,
        [Display(Name = "PlanOperativoAnual", ResourceType = typeof(Resources.T))]
        PlanOperativoAnual = 8,
        [Display(Name = "PlanAnualDeCentro", ResourceType = typeof(Resources.T))]
        PlanAnualDeCentro = 16,
        [Display(Name = "PlanDeMejora", ResourceType = typeof(Resources.T))]
        PlanDeMejora = 32,
        [Display(Name = "PlanDeAccion", ResourceType = typeof(Resources.T))]
        PlanDeAccion = 64,
        [Display(Name = "PlanDeAcompanamiento", ResourceType = typeof(Resources.T))]
        PlanDeAcompanamiento = 128,
        [Display(Name = "AutoevaluacionDelCentro", ResourceType = typeof(Resources.T))]
        AutoevaluacionDelCentro = 256,
        [Display(Name = "MemoriaAnual", ResourceType = typeof(Resources.T))]
        MemoriaAnual = 512,
        [Display(Name = "InformeDePeriodo", ResourceType = typeof(Resources.T))]
        InformeDePeriodo = 1024,
        [Display(Name = "DocumentoDeRevisionCurricular", ResourceType = typeof(Resources.T))]
        DocumentoDeRevisionCurricular = 2048
    }

    [ComplexType]
    public class FrecuenciaActividades
    {
        [System.ComponentModel.DataAnnotations.UIHint("FlagsInTR")]
        [Display(Name = "JuntaDeCentro", ResourceType = typeof(Resources.T))]
        public Frecuencia JuntaDeCentro { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("FlagsInTR")]
        [Display(Name = "EquipoDeGestion", ResourceType = typeof(Resources.T))]
        public Frecuencia EquipoDeGestion { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("FlagsInTR")]
        [Display(Name = "APMAE", ResourceType = typeof(Resources.T))]
        public Frecuencia APMAE { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("FlagsInTR")]
        [Display(Name = "ComitesDePadresYMadresPorCurso", ResourceType = typeof(Resources.T))]
        public Frecuencia ComitesDePadresYMadresPorCurso { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("FlagsInTR")]
        [Display(Name = "AsambleaDeMaestros", ResourceType = typeof(Resources.T))]
        public Frecuencia AsambleaDeMaestros { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("FlagsInTR")]
        [Display(Name = "ConsejosDeCurso", ResourceType = typeof(Resources.T))]
        public Frecuencia ConsejosDeCurso { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("FlagsInTR")]
        [Display(Name = "ConsejoEstudiantil", ResourceType = typeof(Resources.T))]
        public Frecuencia ConsejoEstudiantil { get; set; }
    }

    public class Centro
    {
        public Centro()
        {
            this.FrecuenciaActividades = new FrecuenciaActividades();
            this.Telefono = new Telefono();
        }

        /******************************/
        public int Id { get; set; }

        [Display(Name = "Codigo", ResourceType = typeof(Resources.T))]
        public string Codigo { get; set; }

        public string Nombre { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "SectorEducativo", ResourceType = typeof(Resources.T))]
        public Sector SectorEducativo { get; set; }

        [Display(Name = "Telefono", ResourceType = typeof(Resources.T))]
        public Telefono Telefono { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("EmailAddress")]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; }

        public int? ProvinciaId { get; set; }
        public virtual Provincia Provincia { get; set; }

        public int? MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }

        public string Sector { get; set; }

        public string Calle { get; set; }

        public string Longitud { get; set; }
        public string Latitud { get; set; }

        //[Range(typeof(Int32),"0","99999")]
        //public int Aulas { get; set; }

        public int? ComponenteId { get; set; }

        [Display(Name = "Componente", ResourceType = typeof(Resources.T))]
        public Componente Componente { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Flags")]
        public NivelesCentro Niveles { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Flags")]
        public DocenteCiclo Ciclos { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Flags")]
        public TandasCentro Tandas { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "SalonMultiUso", ResourceType = typeof(Resources.T))]
        public SalonMultiUso SalonMultiUso { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "LabInformatica", ResourceType = typeof(Resources.T))]
        public LabInformatica LabInformatica { get; set; }

        [Display(Name = "BanosSuficientes", ResourceType = typeof(Resources.T))]
        public bool BanosSuficientes { get; set; }

        public bool Biblioteca { get; set; }

        [Display(Name = "FacilidadesDeAccesibilidad", ResourceType = typeof(Resources.T))]
        public bool FacilidadesDeAccesibilidad { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Enum")]
        [Display(Name = "Lavamanos", ResourceType = typeof(Resources.T))]
        public Lavamanos Lavamanos { get; set; }

        [System.ComponentModel.DataAnnotations.UIHint("Flags")]
        [Display(Name = "InstrumentosDeGestionActualizados", ResourceType = typeof(Resources.T))]
        public InstrumentosDeGestion InstrumentosDeGestionActualizados { get; set; }

        [Display(Name = "EscuelaPadresMadres", ResourceType = typeof(Resources.T))]
        public bool EscuelaPadresMadres { get; set; }

        [Display(Name = "FrecuenciaActividades", ResourceType = typeof(Resources.T))]
        public FrecuenciaActividades FrecuenciaActividades { get; set; }
        /******************************/

        [ForeignKey("Director")]
        public int? DirectorId { get; set; }
        [Display(Name = "Director", ResourceType = typeof(Resources.T))]
        public virtual PersonalAdministrativo Director { get; set; }

        public int RedId { get; set; }
        public virtual Red Red { get; set; }

        /******************************/

        //[Display(Name = "Aulas")]
        //public virtual ICollection<Aula> Aulas { get; set; }

        public virtual ICollection<CentroGrado> CentroGrados { get; set; }

        [Display(Name = "PersonalAdministrativo", ResourceType = typeof(Resources.T))]
        public virtual ICollection<PersonalAdministrativo> PersonalAdministrativo { get; set; }

        [Display(Name = "Docentes", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Docente> Docentes { get; set; }

        [Display(Name = "Estudiantes", ResourceType = typeof(Resources.T))]
        public virtual ICollection<Estudiante> Estudiantes { get; set; }
        /******************************/
    }
}