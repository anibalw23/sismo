using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.DAL
{
    public struct Categorias
    {
        // Esto lo Agrege yo
        public const string catMantenimiento = "Matenimiento";
        //public const string catInstituciones = "Instituciones";
        //public const string catCiclosFormativos = "Ciclos Formativos";
        //public const string catIndicadores = "Indicadores";
       // public const string catIndicadores = "Reportes";

        public const string catAdm = "Administración";
        public const string catOp = "Operativo";
        public const string catSeg = "Seguimiento";
    }

    public class MonitoreoContext : DbContext
    {
        public MonitoreoContext() : base("DefaultConnection") { }
        [Browsable(false)]
        public DbSet<SeccionAula> SeccionesAula { get; set; }
        [Browsable(false)]
        public DbSet<InscripcionActividadAcompanamiento> InscripcionesActividadesAcompanamiento { get; set; }

        [Browsable(false)]
        public DbSet<SeccionPersonalMateria> SeccionesPersonalMateria { get; set; }

        [Category(Categorias.catAdm)]
        public DbSet<GrupoEtnico> GruposEtnicos { get; set; }

        [Category(Categorias.catAdm)]
        public DbSet<Monitoreo.Models.Provincia> Provincias { get; set; }

        [Category(Categorias.catAdm)]
        public DbSet<Monitoreo.Models.Municipio> Municipios { get; set; }

        [Category(Categorias.catAdm)]
        public DbSet<Monitoreo.Models.Persona> Personas { get; set; }


        [Category(Categorias.catOp)]
        public DbSet<Regional> Regionales { get; set; }

        [Category(Categorias.catOp)]
        public DbSet<Distrito> Distritos { get; set; }

        [Category(Categorias.catOp)]
        public DbSet<Red> Redes { get; set; }

        [Category(Categorias.catOp)]
        public DbSet<Centro> Centros { get; set; }

        [Browsable(false)]
        public DbSet<PersonalAdministrativo> PersonalAdministrativo { get; set; }

        [Browsable(false)]
        public DbSet<CoordinadorRedes> CoordinadoresRedes { get; set; }

        [Browsable(false)]
        public DbSet<Docente> Docentes { get; set; }

        [Browsable(false)]
        public DbSet<DocenteMateria> DocenteMaterias { get; set; }

        [Browsable(false)]
        public DbSet<Estudiante> Estudiantes { get; set; }

        //public DbSet<Aula> Aulas { get; set; }

        [Browsable(false)]
        public DbSet<Monitoreo.Models.ActividadFormativa> ActividadesFormativas { get; set; }

        //[Category(Categorias.catOp)]
        //public DbSet<CursoTaller> CursosTalleres { get; set; }

        [Browsable(false)]
        public DbSet<Inscripcion> Inscripciones { get; set; }

        [Category(Categorias.catSeg)]
        public DbSet<Monitoreo.Models.BO.Ausencia> Ausencias { get; set; }

        [Category(Categorias.catSeg)]
        public DbSet<Monitoreo.Models.BO.Pregunta> Preguntas { get; set; }

        [Browsable(false)]
        public DbSet<Monitoreo.Models.BO.PreguntaOpcion> PreguntasOpciones { get; set; }

        [Category(Categorias.catSeg)]
        public DbSet<Monitoreo.Models.BO.Evaluacion> Evaluaciones { get; set; }

        [Browsable(false)]
        public DbSet<Monitoreo.Models.BO.Respuesta> Respuestas { get; set; }

        [Category(Categorias.catAdm)]
        public DbSet<Monitoreo.Models.Seccion> Secciones { get; set; }

        [Category(Categorias.catAdm)]
        public DbSet<Monitoreo.Models.ActividadFormativaBase> ActividadFormativaBases { get; set; }

        [Browsable(false)]
        public DbSet<Monitoreo.Models.Personal> Personal { get; set; }

        [Browsable(false)]
        public DbSet<Monitoreo.Models.BO.PersonalMateria> PersonalMaterias { get; set; }

        [Category(Categorias.catAdm)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.Componente> Componentes { get; set; }

        [Category(Categorias.catSeg)]
        [DisplayName("Calendario Ciclo Formativo")]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.CalendarioCicloFormativo> CalendarioCicloFormativoes { get; set; }

        [Category(Categorias.catSeg)]
        [DisplayName("Reporte Ausencias")]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.AusenciaReporte> AusenciaReportes { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Persona>().HasOptional(a => a.GrupoEtnico).WithMany().HasForeignKey(u => u.GrupoEtnicoId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Distrito>().HasRequired(a => a.Regional).WithMany().HasForeignKey(u => u.RegionalId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Red>().HasRequired(a => a.Distrito).WithMany().HasForeignKey(u => u.DistritoId).WillCascadeOnDelete(false);

            modelBuilder.Entity<GrupoCicloFormativo>().HasRequired(a => a.CicloFormativo).WithMany().HasForeignKey(u => u.CicloFormativoId).WillCascadeOnDelete(false);
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.Indicador> Indicadors { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.indicadorResultado> indicadorResultadoes { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.IndicadorDesagregacion> IndicadorDesagregacions { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.IndicadorFecha> IndicadorFechas { get; set; }

        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.EvaluacionNotaRaw> EvaluacionNotasRaw { get; set; }
        
        [Category(Categorias.catOp)]
        public System.Data.Entity.DbSet<Monitoreo.Models.SuperCicloFormativo> SuperCicloFormativoes { get; set; }

        [Category(Categorias.catOp)]
        public DbSet<CicloFormativo> CiclosFormativos { get; set; }


        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.GrupoCicloFormativo> GruposCiclosFormativos { get; set; }

        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.Acompanante> Acompanantes { get; set; }
       
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.Coordinador> Coordinadors { get; set; }

        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.CicloGrado> CicloGradoes { get; set; }

        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.NivelGrado> NivelGradoes { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.Aula> Aulas { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.Materia> Materias { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PeriodoEscolar> PeriodoEscolars { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.GradoLookup> GradoLookups { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.CentroGrado> CentroGradoes { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.ActividadAcompanamiento> ActividadAcompanamientoes { get; set; }
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.EvaluacionAcompanamiento.EvaluacionAcompanamiento> EvaluacionAcompanamientoes { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.EvaluacionAcompanamiento.EvaluacionAcompanamientoPregunta> EvaluacionAcompanamientoPreguntas { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.EvaluacionAcompanamiento.PreguntaOpcionAcompanamiento> PreguntaOpcionAcompanamientoes { get; set; }
        [Browsable(false)]
        public System.Data.Entity.DbSet<Monitoreo.Models.BO.EvaluacionAcompanamiento.EvaluacionAcompanamientoRespuesta> EvaluacionAcompanamientoRespuestas { get; set; }

        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PlanMejora.PlanMejoraCentro> PlanMejoraCentroes { get; set; }

        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PlanMejora.AmbitoObjetivo> AmbitoObjetivoes { get; set; }

        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PlanMejora.Objetivo> Objetivoes { get; set; }

        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PlanMejora.PeriodoMeta> PeriodoMetas { get; set; }

        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PlanMejora.Meta> Metas { get; set; }

        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PlanMejora.IndicadorPlan> IndicadorPlans { get; set; }

        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PlanMejora.Actividad> Actividads { get; set; }

        public System.Data.Entity.DbSet<Monitoreo.Models.BO.PlanMejora.ActividadEspecifica> ActividadEspecificas { get; set; }
        //public System.Data.Entity.DbSet<Monitoreo.Models.BO.InasistenciaCicloFormativo> InasistenciaCicloFormativoes { get; set; }

    }
}