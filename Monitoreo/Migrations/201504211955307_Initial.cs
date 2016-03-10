namespace Monitoreo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Personal",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        PersonaId = c.Int(nullable: false),
                        CentroId = c.Int(nullable: false),
                        FechaContratacion = c.DateTime(nullable: false),
                        FuncionesEjerce = c.Int(nullable: false),
                        Tanda = c.Int(nullable: false),
                        DistritoId = c.Int(),
                        RegionalId = c.Int(),
                        FormacionAcademica = c.Int(),
                        AnosDeEjercicio = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Distrito_Id = c.Int(),
                        Regional_Id = c.Int(),
                        Centro_Id = c.Int(),
                        Centro_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Centro", t => t.CentroId, cascadeDelete: true)
                .ForeignKey("dbo.Persona", t => t.PersonaId, cascadeDelete: true)
                .ForeignKey("dbo.Distrito", t => t.Distrito_Id)
                .ForeignKey("dbo.Regional", t => t.Regional_Id)
                .ForeignKey("dbo.Distrito", t => t.DistritoId)
                .ForeignKey("dbo.Regional", t => t.RegionalId)
                .ForeignKey("dbo.Centro", t => t.Centro_Id)
                .ForeignKey("dbo.Centro", t => t.Centro_Id1)
                .Index(t => t.PersonaId)
                .Index(t => t.CentroId)
                .Index(t => t.DistritoId)
                .Index(t => t.RegionalId)
                .Index(t => t.Distrito_Id)
                .Index(t => t.Regional_Id)
                .Index(t => t.Centro_Id)
                .Index(t => t.Centro_Id1);
            
            CreateTable(
                "dbo.Centro",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nombre = c.String(),
                        SectorEducativo = c.Int(nullable: false),
                        Telefono_Fijo = c.String(),
                        Telefono_Celular = c.String(),
                        Telefono_Otro = c.String(),
                        Email = c.String(),
                        ProvinciaId = c.Int(),
                        MunicipioId = c.Int(),
                        Sector = c.String(),
                        Calle = c.String(),
                        Aulas = c.Int(nullable: false),
                        ComponenteId = c.Int(),
                        Niveles = c.Int(nullable: false),
                        Ciclos = c.Int(nullable: false),
                        Tandas = c.Int(nullable: false),
                        SalonMultiUso = c.Int(nullable: false),
                        LabInformatica = c.Int(nullable: false),
                        BanosSuficientes = c.Boolean(nullable: false),
                        Biblioteca = c.Boolean(nullable: false),
                        FacilidadesDeAccesibilidad = c.Boolean(nullable: false),
                        Lavamanos = c.Int(nullable: false),
                        InstrumentosDeGestionActualizados = c.Int(nullable: false),
                        EscuelaPadresMadres = c.Boolean(nullable: false),
                        FrecuenciaActividades_JuntaDeCentro = c.Int(nullable: false),
                        FrecuenciaActividades_EquipoDeGestion = c.Int(nullable: false),
                        FrecuenciaActividades_APMAE = c.Int(nullable: false),
                        FrecuenciaActividades_ComitesDePadresYMadresPorCurso = c.Int(nullable: false),
                        FrecuenciaActividades_AsambleaDeMaestros = c.Int(nullable: false),
                        FrecuenciaActividades_ConsejosDeCurso = c.Int(nullable: false),
                        FrecuenciaActividades_ConsejoEstudiantil = c.Int(nullable: false),
                        DirectorId = c.Int(),
                        RedId = c.Int(nullable: false),
                        Distrito_Id = c.Int(),
                        Red_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Componente", t => t.ComponenteId)
                .ForeignKey("dbo.Distrito", t => t.Distrito_Id)
                .ForeignKey("dbo.Red", t => t.Red_Id)
                .ForeignKey("dbo.Personal", t => t.DirectorId)
                .ForeignKey("dbo.Municipio", t => t.MunicipioId)
                .ForeignKey("dbo.Provincia", t => t.ProvinciaId)
                .ForeignKey("dbo.Red", t => t.RedId, cascadeDelete: true)
                .Index(t => t.ProvinciaId)
                .Index(t => t.MunicipioId)
                .Index(t => t.ComponenteId)
                .Index(t => t.DirectorId)
                .Index(t => t.RedId)
                .Index(t => t.Distrito_Id)
                .Index(t => t.Red_Id);
            
            CreateTable(
                "dbo.Componente",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Distrito",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nombre = c.String(),
                        RegionalId = c.Int(nullable: false),
                        ProvinciaId = c.Int(),
                        MunicipioId = c.Int(),
                        Sector = c.String(),
                        Calle = c.String(),
                        Telefono_Fijo = c.String(),
                        Telefono_Celular = c.String(),
                        Telefono_Otro = c.String(),
                        CorreoElectronico = c.String(),
                        SitioWeb = c.String(),
                        CentroSedeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Centro", t => t.CentroSedeId)
                .ForeignKey("dbo.Municipio", t => t.MunicipioId)
                .ForeignKey("dbo.Provincia", t => t.ProvinciaId)
                .ForeignKey("dbo.Regional", t => t.RegionalId)
                .Index(t => t.RegionalId)
                .Index(t => t.ProvinciaId)
                .Index(t => t.MunicipioId)
                .Index(t => t.CentroSedeId);
            
            CreateTable(
                "dbo.Municipio",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nombre = c.String(),
                        ProvinciaId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Provincia", t => t.ProvinciaId)
                .Index(t => t.ProvinciaId);
            
            CreateTable(
                "dbo.Provincia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Persona",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Cedula = c.String(),
                        Nombres = c.String(nullable: false, maxLength: 50),
                        PrimerApellido = c.String(nullable: false),
                        SegundoApellido = c.String(),
                        FechaNacimiento = c.DateTime(),
                        LugarNacimiento = c.String(),
                        Sexo = c.Int(),
                        Telefono_Fijo = c.String(),
                        Telefono_Celular = c.String(),
                        Telefono_Otro = c.String(),
                        ProvinciaId = c.Int(),
                        MunicipioId = c.Int(),
                        Sector = c.String(),
                        Calle = c.String(),
                        GrupoEtnicoId = c.Int(),
                        Discapacidades = c.Int(nullable: false),
                        Comentarios = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrupoEtnico", t => t.GrupoEtnicoId)
                .ForeignKey("dbo.Municipio", t => t.MunicipioId)
                .ForeignKey("dbo.Provincia", t => t.ProvinciaId)
                .Index(t => t.ProvinciaId)
                .Index(t => t.MunicipioId)
                .Index(t => t.GrupoEtnicoId);
            
            CreateTable(
                "dbo.GrupoEtnico",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DocenteMateria",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocenteId = c.Int(nullable: false),
                        Nivel = c.Int(nullable: false),
                        Ciclo = c.Int(nullable: false),
                        Tanda = c.Int(nullable: false),
                        Area = c.Int(nullable: false),
                        Grados = c.Int(nullable: false),
                        SeccionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Personal", t => t.DocenteId, cascadeDelete: true)
                .ForeignKey("dbo.Seccion", t => t.SeccionId)
                .Index(t => t.DocenteId)
                .Index(t => t.SeccionId);
            
            CreateTable(
                "dbo.Seccion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Red",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nombre = c.String(),
                        DistritoId = c.Int(nullable: false),
                        CentroSedeId = c.Int(),
                        Distrito_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Centro", t => t.CentroSedeId)
                .ForeignKey("dbo.Distrito", t => t.DistritoId)
                .ForeignKey("dbo.Distrito", t => t.Distrito_Id)
                .Index(t => t.DistritoId)
                .Index(t => t.CentroSedeId)
                .Index(t => t.Distrito_Id);
            
            CreateTable(
                "dbo.Regional",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nombre = c.String(),
                        DirectorId = c.Int(),
                        ProvinciaId = c.Int(),
                        MunicipioId = c.Int(),
                        Sector = c.String(),
                        Calle = c.String(),
                        Telefono_Fijo = c.String(),
                        Telefono_Celular = c.String(),
                        Telefono_Otro = c.String(),
                        CorreoElectronico = c.String(),
                        SitioWeb = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Personal", t => t.DirectorId)
                .ForeignKey("dbo.Municipio", t => t.MunicipioId)
                .ForeignKey("dbo.Provincia", t => t.ProvinciaId)
                .Index(t => t.DirectorId)
                .Index(t => t.ProvinciaId)
                .Index(t => t.MunicipioId);
            
            CreateTable(
                "dbo.Estudiante",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PersonaId = c.Int(nullable: false),
                        Matricula = c.String(nullable: false),
                        CentroId = c.Int(nullable: false),
                        SeccionId = c.Int(),
                        Nivel = c.Int(nullable: false),
                        Ciclo = c.Int(nullable: false),
                        Tanda = c.Int(nullable: false),
                        Grado = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Centro", t => t.CentroId, cascadeDelete: true)
                .ForeignKey("dbo.Persona", t => t.PersonaId, cascadeDelete: true)
                .ForeignKey("dbo.Seccion", t => t.SeccionId)
                .Index(t => t.PersonaId)
                .Index(t => t.CentroId)
                .Index(t => t.SeccionId);
            
            CreateTable(
                "dbo.Evaluacion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Titulo = c.String(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        TipoEvaluacion = c.Int(nullable: false),
                        FormaEvaluacion = c.Int(nullable: false),
                        CicloFormativoId = c.Int(nullable: false),
                        TipoDeObjeto = c.Int(nullable: false),
                        ModoEntradaEvaluacion = c.Int(nullable: false),
                        Acompanante_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CicloFormativo", t => t.CicloFormativoId, cascadeDelete: true)
                .ForeignKey("dbo.Personal", t => t.Acompanante_Id)
                .Index(t => t.CicloFormativoId)
                .Index(t => t.Acompanante_Id);
            
            CreateTable(
                "dbo.CicloFormativo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tema = c.String(),
                        tipo = c.Int(nullable: false),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFinalizacion = c.DateTime(nullable: false),
                        SuperCicloFormativoId = c.Int(nullable: false),
                        Nivel = c.Int(nullable: false),
                        Area = c.Int(nullable: false),
                        Ciclo = c.Int(nullable: false),
                        DuracionTotal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SuperCicloFormativo", t => t.SuperCicloFormativoId, cascadeDelete: true)
                .Index(t => t.SuperCicloFormativoId);
            
            CreateTable(
                "dbo.ActividadFormativa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActividadFormativaBaseId = c.Int(nullable: false),
                        CicloFormativoId = c.Int(nullable: false),
                        Duracion = c.Int(nullable: false),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActividadFormativaBase", t => t.ActividadFormativaBaseId, cascadeDelete: true)
                .ForeignKey("dbo.CicloFormativo", t => t.CicloFormativoId, cascadeDelete: true)
                .Index(t => t.ActividadFormativaBaseId)
                .Index(t => t.CicloFormativoId);
            
            CreateTable(
                "dbo.ActividadFormativaBase",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tipo = c.Int(nullable: false),
                        Duracion = c.Int(nullable: false),
                        Organizacion = c.String(),
                        Actores = c.String(),
                        ModoEvaluacion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ausencia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PersonaId = c.Int(nullable: false),
                        CalendarioCicloFormativoId = c.Int(nullable: false),
                        Tipo = c.Int(nullable: false),
                        Comentario = c.String(),
                        ActividadFormativa_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CalendarioCicloFormativo", t => t.CalendarioCicloFormativoId, cascadeDelete: true)
                .ForeignKey("dbo.Persona", t => t.PersonaId, cascadeDelete: true)
                .ForeignKey("dbo.ActividadFormativa", t => t.ActividadFormativa_Id)
                .Index(t => t.PersonaId)
                .Index(t => t.CalendarioCicloFormativoId)
                .Index(t => t.ActividadFormativa_Id);
            
            CreateTable(
                "dbo.CalendarioCicloFormativo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        horas = c.Int(nullable: false),
                        TipoEvento = c.Int(nullable: false),
                        CicloFormativoID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CicloFormativo", t => t.CicloFormativoID, cascadeDelete: true)
                .Index(t => t.CicloFormativoID);
            
            CreateTable(
                "dbo.Inscripcion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CicloFormativoId = c.Int(nullable: false),
                        GrupoCicloFormativoId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        ParticipanteId = c.Int(nullable: false),
                        Rol = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CicloFormativo", t => t.CicloFormativoId, cascadeDelete: true)
                .ForeignKey("dbo.GrupoCicloFormativo", t => t.GrupoCicloFormativoId, cascadeDelete: true)
                .ForeignKey("dbo.Persona", t => t.ParticipanteId, cascadeDelete: true)
                .Index(t => t.CicloFormativoId)
                .Index(t => t.GrupoCicloFormativoId)
                .Index(t => t.ParticipanteId);
            
            CreateTable(
                "dbo.GrupoCicloFormativo",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        codigo = c.String(),
                        nombre = c.String(),
                        CicloFormativoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CicloFormativo", t => t.CicloFormativoId)
                .Index(t => t.CicloFormativoId);
            
            CreateTable(
                "dbo.Pregunta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(nullable: false),
                        NivelDominio = c.Int(nullable: false),
                        TipoEvaluacion = c.Int(nullable: false),
                        CicloFormativoId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CicloFormativo", t => t.CicloFormativoId)
                .Index(t => t.CicloFormativoId);
            
            CreateTable(
                "dbo.PreguntaOpcion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PreguntaId = c.Int(nullable: false),
                        Titulo = c.String(nullable: false),
                        Valor = c.String(nullable: false),
                        Correcta = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pregunta", t => t.PreguntaId, cascadeDelete: true)
                .Index(t => t.PreguntaId);
            
            CreateTable(
                "dbo.SuperCicloFormativo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nombre = c.String(),
                        Nivel = c.Int(nullable: false),
                        Area = c.Int(nullable: false),
                        Ciclo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EvaluacionNotaRaw",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nota = c.Int(nullable: false),
                        notaMaxima = c.Int(nullable: false),
                        ParticipanteId = c.Int(nullable: false),
                        EvaluacionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evaluacion", t => t.EvaluacionId, cascadeDelete: true)
                .ForeignKey("dbo.Persona", t => t.ParticipanteId, cascadeDelete: true)
                .Index(t => t.ParticipanteId)
                .Index(t => t.EvaluacionId);
            
            CreateTable(
                "dbo.Respuesta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        Digitador = c.String(),
                        EvaluacionId = c.Int(nullable: false),
                        PreguntaId = c.Int(nullable: false),
                        ParticipanteId = c.Int(nullable: false),
                        Valor = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evaluacion", t => t.EvaluacionId, cascadeDelete: true)
                .ForeignKey("dbo.Persona", t => t.ParticipanteId, cascadeDelete: true)
                .ForeignKey("dbo.Pregunta", t => t.PreguntaId, cascadeDelete: true)
                .Index(t => t.EvaluacionId)
                .Index(t => t.PreguntaId)
                .Index(t => t.ParticipanteId);
            
            CreateTable(
                "dbo.AusenciaReporte",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        titulo = c.String(),
                        CicloFormativoId = c.Int(nullable: false),
                        SeccionId = c.Int(nullable: false),
                        NumeroPersonasObjetivo = c.Int(nullable: false),
                        AsistenciaObjetivo = c.Int(nullable: false),
                        NumeroHorasObjetivo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CicloFormativo", t => t.CicloFormativoId, cascadeDelete: true)
                .ForeignKey("dbo.Seccion", t => t.SeccionId, cascadeDelete: true)
                .Index(t => t.CicloFormativoId)
                .Index(t => t.SeccionId);
            
            CreateTable(
                "dbo.IndicadorDesagregacion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IndicadorFecha",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        mes = c.Int(nullable: false),
                        cuarto = c.Int(nullable: false),
                        anio = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.indicadorResultado",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        valor = c.Int(nullable: false),
                        valorEsperado = c.Int(nullable: false),
                        IndicadorID = c.Int(nullable: false),
                        IndicadorFechaID = c.Int(nullable: false),
                        IndicadorDesagregacionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.IndicadorPlan", t => t.IndicadorID, cascadeDelete: true)
                .ForeignKey("dbo.IndicadorDesagregacion", t => t.IndicadorDesagregacionID, cascadeDelete: true)
                .ForeignKey("dbo.IndicadorFecha", t => t.IndicadorFechaID, cascadeDelete: true)
                .Index(t => t.IndicadorID)
                .Index(t => t.IndicadorFechaID)
                .Index(t => t.IndicadorDesagregacionID);
            
            CreateTable(
                "dbo.IndicadorPlan",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        nombre = c.String(),
                        codigo = c.String(),
                        tipo = c.Int(nullable: false),
                        valorLineaBase = c.Int(nullable: false),
                        descripcion = c.String(),
                        formula = c.String(),
                        Frecuencia = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.indicadorResultado", "IndicadorFechaID", "dbo.IndicadorFecha");
            DropForeignKey("dbo.indicadorResultado", "IndicadorDesagregacionID", "dbo.IndicadorDesagregacion");
            DropForeignKey("dbo.indicadorResultado", "IndicadorID", "dbo.IndicadorPlan");
            DropForeignKey("dbo.AusenciaReporte", "SeccionId", "dbo.Seccion");
            DropForeignKey("dbo.AusenciaReporte", "CicloFormativoId", "dbo.CicloFormativo");
            DropForeignKey("dbo.Respuesta", "PreguntaId", "dbo.Pregunta");
            DropForeignKey("dbo.Respuesta", "ParticipanteId", "dbo.Persona");
            DropForeignKey("dbo.Respuesta", "EvaluacionId", "dbo.Evaluacion");
            DropForeignKey("dbo.EvaluacionNotaRaw", "ParticipanteId", "dbo.Persona");
            DropForeignKey("dbo.EvaluacionNotaRaw", "EvaluacionId", "dbo.Evaluacion");
            DropForeignKey("dbo.Evaluacion", "CicloFormativoId", "dbo.CicloFormativo");
            DropForeignKey("dbo.CicloFormativo", "SuperCicloFormativoId", "dbo.SuperCicloFormativo");
            DropForeignKey("dbo.PreguntaOpcion", "PreguntaId", "dbo.Pregunta");
            DropForeignKey("dbo.Pregunta", "CicloFormativoId", "dbo.CicloFormativo");
            DropForeignKey("dbo.Inscripcion", "ParticipanteId", "dbo.Persona");
            DropForeignKey("dbo.Inscripcion", "GrupoCicloFormativoId", "dbo.GrupoCicloFormativo");
            DropForeignKey("dbo.GrupoCicloFormativo", "CicloFormativoId", "dbo.CicloFormativo");
            DropForeignKey("dbo.Inscripcion", "CicloFormativoId", "dbo.CicloFormativo");
            DropForeignKey("dbo.ActividadFormativa", "CicloFormativoId", "dbo.CicloFormativo");
            DropForeignKey("dbo.Ausencia", "ActividadFormativa_Id", "dbo.ActividadFormativa");
            DropForeignKey("dbo.Ausencia", "PersonaId", "dbo.Persona");
            DropForeignKey("dbo.Ausencia", "CalendarioCicloFormativoId", "dbo.CalendarioCicloFormativo");
            DropForeignKey("dbo.CalendarioCicloFormativo", "CicloFormativoID", "dbo.CicloFormativo");
            DropForeignKey("dbo.ActividadFormativa", "ActividadFormativaBaseId", "dbo.ActividadFormativaBase");
            DropForeignKey("dbo.Centro", "RedId", "dbo.Red");
            DropForeignKey("dbo.Centro", "ProvinciaId", "dbo.Provincia");
            DropForeignKey("dbo.Personal", "Centro_Id1", "dbo.Centro");
            DropForeignKey("dbo.Centro", "MunicipioId", "dbo.Municipio");
            DropForeignKey("dbo.Estudiante", "SeccionId", "dbo.Seccion");
            DropForeignKey("dbo.Estudiante", "PersonaId", "dbo.Persona");
            DropForeignKey("dbo.Estudiante", "CentroId", "dbo.Centro");
            DropForeignKey("dbo.Personal", "Centro_Id", "dbo.Centro");
            DropForeignKey("dbo.Centro", "DirectorId", "dbo.Personal");
            DropForeignKey("dbo.Personal", "RegionalId", "dbo.Regional");
            DropForeignKey("dbo.Personal", "DistritoId", "dbo.Distrito");
            DropForeignKey("dbo.Distrito", "RegionalId", "dbo.Regional");
            DropForeignKey("dbo.Regional", "ProvinciaId", "dbo.Provincia");
            DropForeignKey("dbo.Personal", "Regional_Id", "dbo.Regional");
            DropForeignKey("dbo.Regional", "MunicipioId", "dbo.Municipio");
            DropForeignKey("dbo.Regional", "DirectorId", "dbo.Personal");
            DropForeignKey("dbo.Red", "Distrito_Id", "dbo.Distrito");
            DropForeignKey("dbo.Red", "DistritoId", "dbo.Distrito");
            DropForeignKey("dbo.Red", "CentroSedeId", "dbo.Centro");
            DropForeignKey("dbo.Centro", "Red_Id", "dbo.Red");
            DropForeignKey("dbo.Distrito", "ProvinciaId", "dbo.Provincia");
            DropForeignKey("dbo.Personal", "Distrito_Id", "dbo.Distrito");
            DropForeignKey("dbo.DocenteMateria", "SeccionId", "dbo.Seccion");
            DropForeignKey("dbo.DocenteMateria", "DocenteId", "dbo.Personal");
            DropForeignKey("dbo.Personal", "PersonaId", "dbo.Persona");
            DropForeignKey("dbo.Persona", "ProvinciaId", "dbo.Provincia");
            DropForeignKey("dbo.Persona", "MunicipioId", "dbo.Municipio");
            DropForeignKey("dbo.Persona", "GrupoEtnicoId", "dbo.GrupoEtnico");
            DropForeignKey("dbo.Personal", "CentroId", "dbo.Centro");
            DropForeignKey("dbo.Distrito", "MunicipioId", "dbo.Municipio");
            DropForeignKey("dbo.Municipio", "ProvinciaId", "dbo.Provincia");
            DropForeignKey("dbo.Distrito", "CentroSedeId", "dbo.Centro");
            DropForeignKey("dbo.Centro", "Distrito_Id", "dbo.Distrito");
            DropForeignKey("dbo.Centro", "ComponenteId", "dbo.Componente");
            DropIndex("dbo.indicadorResultado", new[] { "IndicadorDesagregacionID" });
            DropIndex("dbo.indicadorResultado", new[] { "IndicadorFechaID" });
            DropIndex("dbo.indicadorResultado", new[] { "IndicadorID" });
            DropIndex("dbo.AusenciaReporte", new[] { "SeccionId" });
            DropIndex("dbo.AusenciaReporte", new[] { "CicloFormativoId" });
            DropIndex("dbo.Respuesta", new[] { "ParticipanteId" });
            DropIndex("dbo.Respuesta", new[] { "PreguntaId" });
            DropIndex("dbo.Respuesta", new[] { "EvaluacionId" });
            DropIndex("dbo.EvaluacionNotaRaw", new[] { "EvaluacionId" });
            DropIndex("dbo.EvaluacionNotaRaw", new[] { "ParticipanteId" });
            DropIndex("dbo.PreguntaOpcion", new[] { "PreguntaId" });
            DropIndex("dbo.Pregunta", new[] { "CicloFormativoId" });
            DropIndex("dbo.GrupoCicloFormativo", new[] { "CicloFormativoId" });
            DropIndex("dbo.Inscripcion", new[] { "ParticipanteId" });
            DropIndex("dbo.Inscripcion", new[] { "GrupoCicloFormativoId" });
            DropIndex("dbo.Inscripcion", new[] { "CicloFormativoId" });
            DropIndex("dbo.CalendarioCicloFormativo", new[] { "CicloFormativoID" });
            DropIndex("dbo.Ausencia", new[] { "ActividadFormativa_Id" });
            DropIndex("dbo.Ausencia", new[] { "CalendarioCicloFormativoId" });
            DropIndex("dbo.Ausencia", new[] { "PersonaId" });
            DropIndex("dbo.ActividadFormativa", new[] { "CicloFormativoId" });
            DropIndex("dbo.ActividadFormativa", new[] { "ActividadFormativaBaseId" });
            DropIndex("dbo.CicloFormativo", new[] { "SuperCicloFormativoId" });
            DropIndex("dbo.Evaluacion", new[] { "Acompanante_Id" });
            DropIndex("dbo.Evaluacion", new[] { "CicloFormativoId" });
            DropIndex("dbo.Estudiante", new[] { "SeccionId" });
            DropIndex("dbo.Estudiante", new[] { "CentroId" });
            DropIndex("dbo.Estudiante", new[] { "PersonaId" });
            DropIndex("dbo.Regional", new[] { "MunicipioId" });
            DropIndex("dbo.Regional", new[] { "ProvinciaId" });
            DropIndex("dbo.Regional", new[] { "DirectorId" });
            DropIndex("dbo.Red", new[] { "Distrito_Id" });
            DropIndex("dbo.Red", new[] { "CentroSedeId" });
            DropIndex("dbo.Red", new[] { "DistritoId" });
            DropIndex("dbo.DocenteMateria", new[] { "SeccionId" });
            DropIndex("dbo.DocenteMateria", new[] { "DocenteId" });
            DropIndex("dbo.Persona", new[] { "GrupoEtnicoId" });
            DropIndex("dbo.Persona", new[] { "MunicipioId" });
            DropIndex("dbo.Persona", new[] { "ProvinciaId" });
            DropIndex("dbo.Municipio", new[] { "ProvinciaId" });
            DropIndex("dbo.Distrito", new[] { "CentroSedeId" });
            DropIndex("dbo.Distrito", new[] { "MunicipioId" });
            DropIndex("dbo.Distrito", new[] { "ProvinciaId" });
            DropIndex("dbo.Distrito", new[] { "RegionalId" });
            DropIndex("dbo.Centro", new[] { "Red_Id" });
            DropIndex("dbo.Centro", new[] { "Distrito_Id" });
            DropIndex("dbo.Centro", new[] { "RedId" });
            DropIndex("dbo.Centro", new[] { "DirectorId" });
            DropIndex("dbo.Centro", new[] { "ComponenteId" });
            DropIndex("dbo.Centro", new[] { "MunicipioId" });
            DropIndex("dbo.Centro", new[] { "ProvinciaId" });
            DropIndex("dbo.Personal", new[] { "Centro_Id1" });
            DropIndex("dbo.Personal", new[] { "Centro_Id" });
            DropIndex("dbo.Personal", new[] { "Regional_Id" });
            DropIndex("dbo.Personal", new[] { "Distrito_Id" });
            DropIndex("dbo.Personal", new[] { "RegionalId" });
            DropIndex("dbo.Personal", new[] { "DistritoId" });
            DropIndex("dbo.Personal", new[] { "CentroId" });
            DropIndex("dbo.Personal", new[] { "PersonaId" });
            DropTable("dbo.IndicadorPlan");
            DropTable("dbo.indicadorResultado");
            DropTable("dbo.IndicadorFecha");
            DropTable("dbo.IndicadorDesagregacion");
            DropTable("dbo.AusenciaReporte");
            DropTable("dbo.Respuesta");
            DropTable("dbo.EvaluacionNotaRaw");
            DropTable("dbo.SuperCicloFormativo");
            DropTable("dbo.PreguntaOpcion");
            DropTable("dbo.Pregunta");
            DropTable("dbo.GrupoCicloFormativo");
            DropTable("dbo.Inscripcion");
            DropTable("dbo.CalendarioCicloFormativo");
            DropTable("dbo.Ausencia");
            DropTable("dbo.ActividadFormativaBase");
            DropTable("dbo.ActividadFormativa");
            DropTable("dbo.CicloFormativo");
            DropTable("dbo.Evaluacion");
            DropTable("dbo.Estudiante");
            DropTable("dbo.Regional");
            DropTable("dbo.Red");
            DropTable("dbo.Seccion");
            DropTable("dbo.DocenteMateria");
            DropTable("dbo.GrupoEtnico");
            DropTable("dbo.Persona");
            DropTable("dbo.Provincia");
            DropTable("dbo.Municipio");
            DropTable("dbo.Distrito");
            DropTable("dbo.Componente");
            DropTable("dbo.Centro");
            DropTable("dbo.Personal");
        }
    }
}
