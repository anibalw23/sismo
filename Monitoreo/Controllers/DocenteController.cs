using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models;
using Monitoreo.Models.DAL;
using Monitoreo.Models.BO;
using Monitoreo.Models.BO.ViewModels;
using Monitoreo.Models.BO.EvaluacionAcompanamiento;
using System.Threading.Tasks;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Data.Entity.Validation;
using Monitoreo.Models.BO.ViewModels.SuperCicloFormativoVm;
using Monitoreo.Models.BO.ViewModels.CalendarioCicloFormativoVm;
using System.Text;

namespace Monitoreo.Controllers
{

    [Authorize]
    public class DocenteController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Docentes
        [Route("Docentes")]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Index()
        {
            var personal = db.Docentes.Take(10).Include(d => d.Centro).Include(d => d.Persona);
            return View(personal.ToList());
        }

        public ActionResult DocentesFilterByCriteria(){
            ViewBag.Distrito = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            var Gradolist = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(Grado)))
            {
                int val = (int)item;
                string b = Enum.GetName(typeof(Grado), item);
                Gradolist.Add((int)val, b);
            }
            ViewBag.Gradolist = new SelectList(Gradolist, "Key", "Value");


            var Arealist = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(DocenteArea)))
            {
                int val = (int)item;
                string b = Enum.GetName(typeof(DocenteArea), item);
                Arealist.Add((int)val, b);
            }
            ViewBag.Arealist = new SelectList(Arealist, "Key", "Value");
            return PartialView();
        }


        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public async Task<JsonResult> GetDocenteByCentrosAreasGrados(int[] centrosIds, int[] areasIds, int[] gradosIDs)
        {     
            //Areas
            DocenteArea docArea = new DocenteArea();
            if (areasIds.Count() > 1)
            {
                foreach (var area in areasIds)
                {
                    docArea = (DocenteArea)area | docArea;
                }
            }
            else {
                docArea = (DocenteArea)areasIds[0];
            }

            //Grados
            Grado docGrado = new Grado();
            if (gradosIDs.Count() > 1)
            {
                foreach (var grado in gradosIDs)
                {
                    docGrado = (Grado)grado | docGrado;
                }
            }
            else {
                docGrado = (Grado)gradosIDs[0];
            }
           

            List<Docente> docentes = new List<Docente>();
            docentes = await db.Docentes.Where(c => centrosIds.Contains(c.CentroId))
                                        .Where(m => m.Materias.Any(x => (x.Area & (docArea)) != 0))
                                        .Where(m => m.Materias.Any(x => (x.Grados & (docGrado)) != 0))
                                        .Distinct()
                                        .ToListAsync();

            var jsonData = new
            {
                data = docentes.Select(y => new
                {
                    id = y.Id,
                    cedula = y.Persona.Cedula,
                    nombre = y.Persona.Nombres + " " + y.Persona.PrimerApellido + " " + y.Persona.SegundoApellido,
                    areas = y.Materias.Select(g => g.Area.ToString()).Distinct(),
                    grados = y.Materias.Select(g => g.Grados.ToString()).Distinct()
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        static IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }


        //[Authorize(Roles = "Administrador, Acompanante")]
        [HttpPost]
        public ActionResult GetDataJson(DatatablesParams values)
        {
            var docentes = db.Docentes.Include(p => p.Persona).Select(x => new { x.Id, x.Persona, CentroNombre = x.Centro.Nombre, isActive = x.isActive });
            var recordsTotal = docentes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = docentes.Select(x => new { DT_RowId = x.Id, Cedula = x.Persona.Cedula, Nombre = x.Persona.Nombres + " " + x.Persona.PrimerApellido + " " + x.Persona.SegundoApellido, Centro = x.CentroNombre, x.isActive });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Cedula.Contains(searchValue) || x.Nombre.Contains(searchValue)
                    );

                    recordsFiltered = data.Count();
                }
            }
            data = data.OrderBy(n => n.Nombre);

            // Preparando respuesta y ejecutando consulta
            var jsonData = new
            {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.Skip(from).Take(limit).ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public async Task<JsonResult> GetActividadesAcompompanamientoIds(int superCicloFormativoId)
        {

            var actividadesAcompanamiento = await db.ActividadAcompanamientoes.AsNoTracking().Select(x => new { x.ID, x.TipoAcompanamiento, x.SuperCicloFormativoId }).Where(s => s.SuperCicloFormativoId == superCicloFormativoId).ToListAsync();
            var jsonData = new
            {
                data = actividadesAcompanamiento.Select(y => new
                {
                    id = y.ID,
                    tipo = y.TipoAcompanamiento.ToString(),
                    tipoAcompId = (int)y.TipoAcompanamiento,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        //[OutputCache(Duration = 1800, VaryByCustom = "User", VaryByParam = "superCicloFormativoId;docenteId", Location = OutputCacheLocation.Server)]
        public async Task<JsonResult> GetActividadesPresencialesByCicloByPersona(int superCicloFormativoId, int docenteId)
        {
            List<AsistenciaDocenteDetailsVM> asistenciasActividadPresencial = new List<AsistenciaDocenteDetailsVM>();
            List<CicloFormativo> ciclosFormativos = new List<CicloFormativo>();
            Persona persona = db.Personal.Find(docenteId).Persona;
            var inscripciones = persona.inscripciones.Select(x => new { x.CicloFormativoId, x.CicloFormativo }).Where(s => s.CicloFormativo.SuperCicloFormativoId == superCicloFormativoId).ToList();

            foreach (var insc in inscripciones)
            {
                //List<CalendarioCicloFormativo> calendariosCiclosFormativos = new List<CalendarioCicloFormativo>();
                var calendarios = await db.CalendarioCicloFormativoes.AsNoTracking().Select(x => new { x.CicloFormativoID, x.Fecha, x.horas, x.Id, x.TipoEvento }).Where(c => c.CicloFormativoID == insc.CicloFormativoId).ToListAsync();
                foreach (var cal in calendarios)
                {
                    AsistenciaDocenteDetailsVM asistencia = new AsistenciaDocenteDetailsVM();
                    asistencia.nombreActividad = insc.CicloFormativo.Tema;
                    asistencia.fecha = cal.Fecha;
                    asistencia.horas = cal.horas;
                    asistencia.calendarioCicloId = cal.Id;
                    asistencia.cicloFormativoID = cal.CicloFormativoID;
                    bool noAsistio = await db.Ausencias.AsNoTracking().Select(x => new { x.CalendarioCicloFormativoId, x.PersonaId }).Where(p => p.PersonaId == persona.Id).AnyAsync(cale => cale.CalendarioCicloFormativoId == cal.Id);
                    if (noAsistio)
                    {
                        asistencia.asistio = false;
                    }
                    else
                    {
                        asistencia.asistio = true;
                    }
                    asistenciasActividadPresencial.Add(asistencia);
                }
            }




            var jsonData = new
            {
                data = asistenciasActividadPresencial.Select(y => new
                {
                    nombre = y.nombreActividad,
                    fecha = y.fecha.ToString(),
                    horas = y.horas,
                    asistio = y.asistio,
                    calendarioCicloId = y.calendarioCicloId,
                    cicloFormativoID = y.cicloFormativoID
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        //[OutputCache(Duration = 1800, VaryByCustom = "User", SqlDependency = "SysmoDbv4:InscripcionActividadAcompanamiento", VaryByParam = "superCicloFormativoId;docenteId;tipoEval;tipoActividad", Location = OutputCacheLocation.Server)]
        public async Task<JsonResult> GetInscripcionesActividadByCicloPersonaId(int superCicloFormativoId, int? docenteId, int tipoEval, int tipoActividad)
        {
            List<AsistenciaAcompanamientoVM> asistenciasActividadesAcompanamiento = new List<AsistenciaAcompanamientoVM>();
            List<ActividadAcompanamiento> actividadesAcompanamiento = new List<ActividadAcompanamiento>();
            List<InscripcionActividadAcompanamiento> inscripciones = new List<InscripcionActividadAcompanamiento>();

            TipoAcompanamiento tipoActividadAcomp = new TipoAcompanamiento();
            TipoEvaluacionAcompanamiento tipoEvaluacion = new TipoEvaluacionAcompanamiento();

            switch (tipoActividad)
            {
                case 1:
                    tipoActividadAcomp = TipoAcompanamiento.AcompanamientoAula;
                    tipoEvaluacion = TipoEvaluacionAcompanamiento.Acompanamientoaula;
                    break;
                case 2:
                    tipoActividadAcomp = TipoAcompanamiento.AcompanamientoTutorial;
                    tipoEvaluacion = TipoEvaluacionAcompanamiento.AcompanamientoTutorial;
                    break;
                case 3:
                    tipoActividadAcomp = TipoAcompanamiento.ClaseModelo;
                    tipoEvaluacion = TipoEvaluacionAcompanamiento.ClaseModelo;
                    break;
                case 4:
                    tipoActividadAcomp = TipoAcompanamiento.GrupoPedagogico;
                    tipoEvaluacion = TipoEvaluacionAcompanamiento.GrupoPedagogico;
                    break;

            }

            var evaluacionesAcompanamiento = await db.EvaluacionAcompanamientoes.AsNoTracking().Where(s => s.SuperCicloFormativo.Any(x => x.Id == superCicloFormativoId)).Select(x => new { x.Id, x.Titulo, x.TipoEvaluacionAcomp }).Where(t => t.TipoEvaluacionAcomp == tipoEvaluacion).ToListAsync();


            Personal persona = new Personal();

            var personaVar = await db.Personal.AsNoTracking().Select(x => new { inscripcionesActividadesacompanamiento = x.inscripcionesActividadesacompanamiento.Where(t => t.ActividadAcompanamiento.TipoAcompanamiento == tipoActividadAcomp).Where(s => s.ActividadAcompanamiento.SuperCicloFormativoId == superCicloFormativoId).ToList(), x.Id }).Where(p => p.Id == docenteId).SingleOrDefaultAsync();
            persona.Id = personaVar.Id;
            persona.inscripcionesActividadesacompanamiento = personaVar.inscripcionesActividadesacompanamiento;

            if (persona != null)
            {
                inscripciones = persona.inscripcionesActividadesacompanamiento.ToList();//.Where(t => t.ActividadAcompanamiento.TipoAcompanamiento == tipoActividadAcomp).Where(s => s.ActividadAcompanamiento.SuperCicloFormativoId == superCicloFormativoId).ToList();
                foreach (var ins in inscripciones)
                {
                    AsistenciaAcompanamientoVM asistencia = new AsistenciaAcompanamientoVM();
                    asistencia.inscripcionID = ins.ID;
                    asistencia.actividadAcompId = ins.actividadAcompanamientoID;
                    asistencia.tipo = ins.ActividadAcompanamiento.TipoAcompanamiento;
                    asistencia.emptyRow = "";
                    asistencia.Area = ins.Area.ToString();
                    asistencia.fecha = ins.fecha;
                    asistencia.horas = ins.horas;
                    asistencia.asistio = true;
                    //asistencia.evaluacionesAcompanamiento = evaluacionesAcompanamiento; 
                    asistenciasActividadesAcompanamiento.Add(asistencia);
                }
            }

            var jsonData = new
            {
                data = asistenciasActividadesAcompanamiento.Select(y => new
                {
                    tipo = y.tipo.ToString(),
                    fecha = y.fecha.ToShortDateString(),
                    horas = y.horas,
                    asistio = y.asistio,
                    evals = evaluacionesAcompanamiento.Select(x => new { idEval = x.Id, Titulo = x.Titulo }),
                    inscripcionId = y.inscripcionID,
                    actividadAcompId = y.actividadAcompId,
                    empty = y.emptyRow,
                    area = y.Area
                    //tipoAcompanamientoNum = y.tipoAcompanamientoNum
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public async Task<JsonResult> InsripcionActividadesAcompanamientoByCicloPersonaId(int superCicloFormativoId, int docenteId, string Tipo, string fecha, int horas)
        {
            List<AsistenciaAcompanamientoVM> asistenciasActividadesAcompanamiento = new List<AsistenciaAcompanamientoVM>();
            List<ActividadAcompanamiento> actividadesAcompanamiento = new List<ActividadAcompanamiento>();
            List<InscripcionActividadAcompanamiento> inscripciones = new List<InscripcionActividadAcompanamiento>();


            InscripcionActividadAcompanamiento inscripcion = new InscripcionActividadAcompanamiento();
            //ActividadAcompanamiento actividad = await db.ActividadAcompanamientoes.AsNoTracking().Where(c => c.SuperCicloFormativoId == superCicloFormativoId).Where(t => t.TipoAcompanamiento.ToString() == Tipo).SingleOrDefaultAsync();
            int actividadAcompanamientoID = db.ActividadAcompanamientoes.AsNoTracking().Select(x => new { x.ID, x.SuperCicloFormativoId, x.TipoAcompanamiento }).Where(c => c.SuperCicloFormativoId == superCicloFormativoId).Where(t => t.TipoAcompanamiento.ToString() == Tipo).SingleOrDefault().ID;
            inscripcion.actividadAcompanamientoID = actividadAcompanamientoID;
            inscripcion.fecha = DateTime.Parse(fecha);
            inscripcion.horas = Math.Abs(Convert.ToInt32(horas));
            inscripcion.personalID = docenteId;
            db.InscripcionesActividadesAcompanamiento.Add(inscripcion);
            await db.SaveChangesAsync();

            var jsonData = new
            {
                data = asistenciasActividadesAcompanamiento.Select(y => new
                {
                    tipo = y.tipo.ToString(),
                    fecha = y.fecha.ToString(),
                    horas = y.horas,
                    asistio = y.asistio,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DocenteDetailsAcompanante(int id)
        {
            DocenteDetailsAcompananteVM docenteDetails = new DocenteDetailsAcompananteVM();
            string cedula = User.Identity.Name;
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            List<CicloFormativo> ciclosDocente = new List<CicloFormativo>();
            Acompanante acompanante = new Acompanante();

            List<AsistenciaAcompanamientoVM> asistenciasActividadesAcompanamiento = new List<AsistenciaAcompanamientoVM>();
            docenteDetails.asistenciasAcompanamientos = asistenciasActividadesAcompanamiento;


            Personal personal = await db.Personal.FindAsync(id);  //Cambie esto ahora (probar *)
            if (personal != null)
            {
                docenteDetails.cedula = personal.Persona.Cedula;
                docenteDetails.nombreDocente = personal.Persona.NombreCompleto;
                docenteDetails.sexo = personal.Persona.Sexo.ToString();
                docenteDetails.tanda = personal.Tanda.ToString();
                docenteDetails.telefono = personal.Persona.Telefono.Celular;
                docenteDetails.edad = personal.Persona.Edad;

            }
            else
            {
                ModelState.AddModelError("Error2345", "no se encontró el usuario");
            }

            var ciclosFormativos = await db.SuperCicloFormativoes.AsNoTracking().Select(x => new { x.Id, x.nombre, x.CategoriaSuperCiclo, x.FechaFinalizacion, x.FechaInicio, x.CiclosFormativos }).Where(s => s.CategoriaSuperCiclo == CategoriaSuperCiclo.Docentes).Where(f => f.FechaInicio < DateTime.Now).Where(f => f.FechaFinalizacion > DateTime.Now).OrderBy(f => f.FechaInicio.Year).ThenBy(y => y.FechaInicio.Month).ThenBy(y => y.FechaInicio.Day).ToListAsync();
            List<SuperCicloFormativoVM> ciclosFormativosVm = new List<SuperCicloFormativoVM>();

            /*Esto lo agrege para poner solo los ciclos formativos en que esta inscrito el docente*/
            List<int> superIds = new List<int>();
            var inscripcionesParticipante = personal != null ? db.Inscripciones.AsNoTracking().Select(x => new { x.CicloFormativo.SuperCicloFormativoId, x.ParticipanteId }).Where(p => p.ParticipanteId == personal.PersonaId).ToList(): null;

            if (inscripcionesParticipante != null) {
                var superCiclosIds = inscripcionesParticipante.Select(x => new { x.SuperCicloFormativoId }).Distinct().ToList();
                foreach (var super in superCiclosIds)
                {
                    superIds.Add(super.SuperCicloFormativoId);
                }
            }            
            /*End*/

            foreach (var ciclo in ciclosFormativos.Where(s => superIds.Contains(s.Id)))
            {
                ciclosFormativosVm.Add(new SuperCicloFormativoVM { Id = ciclo.Id, nombre = ciclo.nombre });
            }
            docenteDetails.ciclosFormativos.AddRange(ciclosFormativosVm);

            return View(docenteDetails);
        }

        public ActionResult DocenteDetailsCoordinador(int id)
        {
            DocenteDetailsAcompananteVM docenteDetails = new DocenteDetailsAcompananteVM();
            string cedula = User.Identity.Name;
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            List<CicloFormativo> ciclosDocente = new List<CicloFormativo>();
            Coordinador coordinador = new Coordinador();

            List<AsistenciaAcompanamientoVM> asistenciasActividadesAcompanamiento = new List<AsistenciaAcompanamientoVM>();
            docenteDetails.asistenciasAcompanamientos = asistenciasActividadesAcompanamiento;

            //coordinador = db.Coordinadors.Where(p => p.Persona.Cedula == cedula).SingleOrDefault();
            Docente docente = id != null ? db.Docentes.Find(id) : null;
            if (docente != null)
            {
                docenteDetails.cedula = docente.Persona.Cedula;
                docenteDetails.nombreDocente = docente.Persona.NombreCompleto;
                docenteDetails.sexo = docente.Persona.Sexo.ToString();
                docenteDetails.tanda = docente.Tanda.ToString();
                docenteDetails.telefono = docente.Persona.Telefono.Celular;
                docenteDetails.edad = docente.Persona.Edad;
            }
            else
            {
                ModelState.AddModelError("Error2345", "Error, contacte su administrador!");
            }
            var ciclosFormativos = db.SuperCicloFormativoes.AsNoTracking().Select(x => new { x.Id, x.nombre, x.CategoriaSuperCiclo, x.FechaFinalizacion, x.FechaInicio }).Where(s => s.CategoriaSuperCiclo == CategoriaSuperCiclo.Docentes).Where(f => f.FechaInicio < DateTime.Now).Where(f => f.FechaFinalizacion > DateTime.Now).OrderBy(f => f.FechaInicio.Year).ThenBy(y => y.FechaInicio.Month).ThenBy(y => y.FechaInicio.Day).ToList();
            List<SuperCicloFormativoVM> ciclosFormativosVm = new List<SuperCicloFormativoVM>();

            /*Esto lo agrege para poner solo los ciclos formativos en que esta inscrito el docente*/
            List<int> superIds = new List<int>();
            var inscripcionesParticipante = db.Inscripciones.AsNoTracking().Select(x => new { x.CicloFormativo.SuperCicloFormativoId, x.ParticipanteId }).Where(p => p.ParticipanteId == docente.PersonaId).ToList();

            if (inscripcionesParticipante != null)
            {
                var superCiclosIds = inscripcionesParticipante.Select(x => new { x.SuperCicloFormativoId }).Distinct().ToList();
                foreach (var super in superCiclosIds)
                {
                    superIds.Add(super.SuperCicloFormativoId);
                }
            }
            /*End*/


            foreach (var ciclo in ciclosFormativos.Where(s => superIds.Contains(s.Id)))
            {
                ciclosFormativosVm.Add(new SuperCicloFormativoVM { Id = ciclo.Id, nombre = ciclo.nombre });
            }
            docenteDetails.ciclosFormativos.AddRange(ciclosFormativosVm);


            return View(docenteDetails);
        }



        public async Task<ActionResult> DocenteDetailsCoordinadorInicial(int id, int centroID)
        {
            DocenteDetailsAcompananteVM docenteDetails = new DocenteDetailsAcompananteVM();
            string cedula = User.Identity.Name;
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            List<CicloFormativo> ciclosDocente = new List<CicloFormativo>();
            Coordinador coordinador = new Coordinador();

            List<AsistenciaAcompanamientoVM> asistenciasActividadesAcompanamiento = new List<AsistenciaAcompanamientoVM>();
            docenteDetails.asistenciasAcompanamientos = asistenciasActividadesAcompanamiento;

            //coordinador = await db.Coordinadors.Where(p => p.Persona.Cedula == cedula).SingleOrDefaultAsync();
            Docente docente = db.Docentes.Find(id);
            docenteDetails.cedula = docente.Persona.Cedula;
            docenteDetails.nombreDocente = docente.Persona.NombreCompleto;
            docenteDetails.sexo = docente.Persona.Sexo.ToString();
            docenteDetails.tanda = docente.Tanda.ToString();
            docenteDetails.telefono = docente.Persona.Telefono.Celular;
            docenteDetails.edad = docente.Persona.Edad;

            var ciclosFormativos = await db.SuperCicloFormativoes.AsNoTracking().Select(x => new { x.Id, x.nombre, x.CategoriaSuperCiclo, x.FechaFinalizacion, x.FechaInicio }).Where(s => s.CategoriaSuperCiclo == CategoriaSuperCiclo.Docentes).Where(f => f.FechaInicio < DateTime.Now).Where(f => f.FechaFinalizacion > DateTime.Now).OrderBy(f => f.FechaInicio.Year).ThenBy(y => y.FechaInicio.Month).ThenBy(y => y.FechaInicio.Day).ToListAsync();
            List<SuperCicloFormativoVM> ciclosFormativosVm = new List<SuperCicloFormativoVM>();

            /*Esto lo agrege para poner solo los ciclos formativos en que esta inscrito el docente*/
            List<int> superIds = new List<int>();
            var inscripcionesParticipante = db.Inscripciones.AsNoTracking().Select(x => new { x.CicloFormativo.SuperCicloFormativoId, x.ParticipanteId }).Where(p => p.ParticipanteId == docente.PersonaId).ToList();

            if (inscripcionesParticipante != null)
            {
                var superCiclosIds = inscripcionesParticipante.Select(x => new { x.SuperCicloFormativoId }).Distinct().ToList();
                foreach (var super in superCiclosIds)
                {
                    superIds.Add(super.SuperCicloFormativoId);
                }
            }
            /*End*/


            foreach (var ciclo in ciclosFormativos.Where(s => superIds.Contains(s.Id)))
            {
                ciclosFormativosVm.Add(new SuperCicloFormativoVM { Id = ciclo.Id, nombre = ciclo.nombre });
            }
            docenteDetails.ciclosFormativos.AddRange(ciclosFormativosVm);


            //  ViewBag.Coordinador = coordinador;
            ViewBag.centroID = centroID;
            return View(docenteDetails);
        }




        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult CentroDocentes(int CentroId)
        {
            List<Docente> docentesList = new List<Docente>();

            var docentes = db.Docentes.AsNoTracking().Include(d => d.Centro).Include(p => p.Persona).Where(s => s.CentroId == CentroId).OrderBy(p => p.Persona.Nombres);
            docentesList = docentes.ToList();
            ViewBag.MasterType = "Centro";
            ViewBag.MasterId = CentroId;

            return PartialView(docentesList.ToList());
        }

        // GET: /Docente/5/Details
        [Authorize(Roles = "Administrador, Acompanante")]
        public async Task<ActionResult> Details(int? id)
        {

            ViewBag.ciclosFormativos = await db.CiclosFormativos.AsNoTracking().Where(i => i.Inscripciones.Any(d => d.ParticipanteId == id)).ToListAsync();
            Docente docente = new Docente();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                docente = db.Docentes.Find(id);
                if (docente == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception exp)
            {
                var msj = exp.Message;
                ModelState.AddModelError("1", "El participante no es un Docente, es personal administrativo");
                return RedirectToAction("Details", "PersonalAdministrativo", new { id = id });
            }


            return View(docente);
        }

        // GET: Docentes/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult CreateModal()
        {
            ViewBag.CentroId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }).OrderBy(n => n.Nombre), "Id", "Nombre");
            ViewBag.SeccionId = new SelectList(db.Secciones.Select(x => new { x.Id, x.Numero }), "Id", "Numero");
            ViewBag.Sexo = Enum.GetValues(typeof(PersonaSexo)).Cast<PersonaSexo>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateModal(Docente docente)
        {
            string result = "OK";
            //Crea una nueva persona si no existe
            string cedula = "";
            string nombre = "";
            string apellido = "";
            string sexo = "";
            string fechaNacimiento = "";
            bool ok = true;
            cedula = Request.Form["cedula"];
            nombre = Request.Form["nombres"];
            apellido = Request.Form["apellido"];
            sexo = Request.Form["sexo"];
            fechaNacimiento = Request.Form["fechaNacimiento"];
            if (cedula != null && cedula != "")
            {
                ModelState.Remove("PersonaId");
                //Verifica la cedula
                Regex regex = new Regex("^\\d{3}-\\d{7}-\\d{1}$$");
                Match match = regex.Match(cedula);
                if (!match.Success)
                {
                    result = "ERROR_CEDULA";
                    ok = false;
                    var data = new { result };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            if (ModelState.IsValid && ok == true)
            {
                bool isRepeated = false;
                if (cedula != null && cedula != "")
                {
                    Persona persona = new Persona();
                    persona.Cedula = cedula;
                    persona.Nombres = (nombre == null ? " " : nombre);
                    persona.Sexo = (sexo == null ? PersonaSexo.Femenino : (PersonaSexo)Enum.Parse(typeof(PersonaSexo), sexo));
                    persona.FechaNacimiento = (fechaNacimiento == null ? DateTime.Now : Convert.ToDateTime(fechaNacimiento));
                    persona.PrimerApellido = (apellido == null ? "-" : apellido); ;
                    persona.SegundoApellido = " ";
                    bool isRepeatedPersona = await db.Personas.AsNoTracking().Select(x => new { x.Cedula }).AnyAsync(c => c.Cedula == cedula);
                    if (!isRepeatedPersona)
                    {
                        db.Personas.Add(persona);
                        await db.SaveChangesAsync();
                        docente.PersonaId = persona.Id;
                    }
                    else
                    {
                        persona = await db.Personas.AsNoTracking().Where(c => c.Cedula == cedula).SingleOrDefaultAsync();
                        docente.PersonaId = persona.Id;
                    }
                }

                isRepeated = await db.Docentes.AsNoTracking().Select(x => new { x.PersonaId, x.CentroId }).Where(d => d.PersonaId == docente.PersonaId).AnyAsync(c => c.CentroId == docente.CentroId);

                if (!isRepeated)  //Verifica que el docente no esta repetido en el mismo centro
                {
                    docente.FuncionesEjerce = PersonalFuncion.Docente;
                    if (docente.AnosDeEjercicio == null)
                    {
                        docente.AnosDeEjercicio = 0;
                    }

                    db.Docentes.Add(docente);
                    foreach (DocenteMateria materia in docente.Materias)
                    {
                        DocenteMateria docenteMatTemp = translateDocenteMateria(materia);
                        materia.Ciclo = docenteMatTemp.Ciclo;
                        materia.Nivel = docenteMatTemp.Nivel;
                        materia.Area = docenteMatTemp.Area;
                    }
                    await db.SaveChangesAsync();
                }
                else
                {
                    result = "ERROR_DOCENTE_REPETIDO";
                }
            }
            else
            {
                result = "ERROR";
            }

            ViewBag.CentroId = new SelectList(db.Centros.OrderBy(n => n.Nombre), "Id", "Nombre", docente.CentroId);
            ViewBag.SeccionId = new SelectList(db.Secciones.Select(x => new { x.Id, x.Numero }), "Id", "Numero");
            ViewBag.Sexo = Enum.GetValues(typeof(PersonaSexo)).Cast<PersonaSexo>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            var jsonData = new
            {
                result = result,
                url = new UrlHelper(Request.RequestContext).Action("Index", "Docente")
            };
            return Json(jsonData);
        }






        // GET: Docentes/Create
        [Route("Docentes/Create")]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Create()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre");
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero");

            if (Request.Params["Modal"] != null)
            {
                return PartialView();
            }

            return View();
        }

        // POST: Docentes/Create
        [Route("Docentes/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Docente docente)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];


            if (ModelState.IsValid)
            {
                bool isRepeated = false;
                if (ViewBag.MasterId != null)
                {
                    int centroID = Convert.ToInt32(ViewBag.MasterId);
                    isRepeated = db.Docentes.AsNoTracking().Where(d => d.PersonaId == docente.PersonaId).Any(c => c.CentroId == centroID);

                    if (!isRepeated)  //Verifica que el docente no esta repetido en el mismo centro
                    {
                        docente.FuncionesEjerce = PersonalFuncion.Docente;
                        if (docente.AnosDeEjercicio == null)
                        {
                            docente.AnosDeEjercicio = 0;
                        }

                        db.Docentes.Add(docente);
                        foreach (DocenteMateria materia in docente.Materias)
                        {
                            DocenteMateria docenteMatTemp = translateDocenteMateria(materia);
                            materia.Ciclo = docenteMatTemp.Ciclo;
                            materia.Nivel = docenteMatTemp.Nivel;
                            materia.Area = docenteMatTemp.Area;
                        }
                        db.SaveChanges();
                        return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                    }
                    else
                    {
                        ModelState.AddModelError("2012", "El docente esta repetido");
                        return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                    }

                }

            }
            else
            {
                ModelState.AddModelError("2011", "Debe llenar todos los campos requeridos");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre", docente.CentroId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero");
            return View(docente);
        }

        public DocenteMateria translateDocenteMateria(DocenteMateria docMate)
        {
            bool found = false;
            if (docMate.Grados == Grado.NivelInicial || docMate.Grados == Grado.Kinder || docMate.Grados == Grado.PrePrimario || docMate.Grados == Grado.PreKinder)
            {
                docMate.Ciclo = DocenteCiclo.SegundoCiclo;
                docMate.Area = DocenteArea.NA;
                docMate.Nivel = NivelEducativo.Inicial;
                found = true;
            }
            if (docMate.Grados == Grado.Primero || docMate.Grados == Grado.Segundo || docMate.Grados == Grado.Tercero)
            {
                docMate.Ciclo = DocenteCiclo.PrimerCiclo;
                docMate.Nivel = NivelEducativo.Primaria;
                found = true;
            }
            if (docMate.Grados == Grado.Cuarto || docMate.Grados == Grado.Quinto || docMate.Grados == Grado.Sexto)
            {
                docMate.Ciclo = DocenteCiclo.SegundoCiclo;
                docMate.Nivel = NivelEducativo.Primaria;
                found = true;
            }
            if (docMate.Grados == Grado.Septimo || docMate.Grados == Grado.Octavo)
            {
                docMate.Ciclo = DocenteCiclo.SeptimoOctavo;
                docMate.Nivel = NivelEducativo.Primaria;
                found = true;
            }

            if (docMate.Grados == Grado.PrimeroSecundaria || docMate.Grados == Grado.SegundoSecundaria || docMate.Grados == Grado.TerceroSecundaria || docMate.Grados == Grado.CuartoSecundaria)
            {
                docMate.Ciclo = DocenteCiclo.NA;
                docMate.Nivel = NivelEducativo.Secundaria;
                found = true;
            }
            if (found == false)
            {
                docMate.Ciclo = DocenteCiclo.NA;
                docMate.Nivel = NivelEducativo.NA;
            }

            return docMate;
        }


        // GET: Docentes/Create
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult CreatePersonalMateria(int MasterId)
        {

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre");
            ViewBag.Areas = new SelectList(db.Materias, "Id", "nombreMateria");
            ViewBag.Periodo = new SelectList(db.PeriodoEscolars, "ID", "nombre");
            ViewBag.Grados = new SelectList(db.CentroGradoes.Where(c => c.CentroId == MasterId).Select(g => g.GradoLookup), "Id", "grado");

            ViewBag.Nivel = "";

            if (Request.Params["Modal"] != null)
            {
                return PartialView();
            }

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePersonalMateria(Docente docente)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];
            int centroId = Convert.ToInt32(ViewBag.MasterId);

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre");
            ViewBag.Areas = new SelectList(db.Materias, "Id", "nombreMateria");
            ViewBag.Periodo = new SelectList(db.PeriodoEscolars, "ID", "nombre");
            ViewBag.Grados = new SelectList(db.CentroGradoes.Where(c => c.CentroId == centroId).Select(g => g.GradoLookup), "Id", "grado");


            if (ModelState.IsValid)
            {
                bool isDocenteCentroRepeated = db.Docentes.Where(d => d.Id == docente.Id).Any(c => c.CentroId == centroId);
                if (!isDocenteCentroRepeated)
                {
                    docente.FuncionesEjerce = PersonalFuncion.Docente;
                    db.Docentes.Add(docente);
                    db.SaveChanges();

                    //Segun el nuevo modelo
                    string periodosStr = Request.Form["periodoEscolar.ID"];
                    string seccionesStr = Request.Form["seccion_Id"];
                    string areasStr = Request.Form["area.ID"];
                    string gradosStr = Request.Form["grado.ID"];

                    if (periodosStr != null && seccionesStr != null && areasStr != null && gradosStr != null)
                    {
                        List<int> areas = areasStr.Split(',').Skip(1).Select(int.Parse).ToList();
                        List<int> secciones = seccionesStr.Split(',').Select(int.Parse).ToList();
                        List<int> grados = gradosStr.Split(',').Skip(1).Select(int.Parse).ToList();

                        List<PersonalMateria> materias = new List<PersonalMateria>();
                        int materiaIndex = 0;
                        foreach (var materia in areas)
                        {
                            PersonalMateria personalMateria = new PersonalMateria();
                            personalMateria.DocenteId = docente.Id;
                            personalMateria.MateriaId = materia;
                            personalMateria.PeriodoEscolarID = Convert.ToInt32(periodosStr);


                            bool isPersonalMateriaRepeated = db.PersonalMaterias.Where(d => d.DocenteId == personalMateria.DocenteId).Where(m => m.MateriaId == personalMateria.MateriaId).Any(p => p.PeriodoEscolarID == personalMateria.PeriodoEscolarID);
                            if (!isPersonalMateriaRepeated)
                            {
                                db.PersonalMaterias.Add(personalMateria); //Agregar en la base de Datos
                                db.SaveChanges();
                            }

                            //Agrega Secciones
                            SeccionPersonalMateria personalMateriaSeccion = new SeccionPersonalMateria();
                            personalMateriaSeccion.PersonalMateriaID = personalMateria.ID;
                            personalMateriaSeccion.SeccionAulaID = secciones[materiaIndex];
                            materiaIndex++;
                            db.SeccionesPersonalMateria.Add(personalMateriaSeccion);
                            db.SaveChanges();
                        }

                    }


                }
                else
                {
                    ModelState.AddModelError("error2036", "Esta persona ya esta inscrita en este centro!");
                }

                //foreach (DocenteMateria materia in docente.Materias)
                //{
                //    db.DocenteMaterias.Add(materia);
                //}

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }

                return RedirectToAction("Index");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre", docente.CentroId);
            return View(docente);
        }









        // GET: /Docente/5/Edit
        public ActionResult EditPersonalMateria(int? id, int? centroId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }

            var Areas = new List<SelectListItem>();
            var totalAreas = db.Materias.ToList();
            foreach (var area in totalAreas)
            {
                if (docente.PersonalMaterias.Any(m => m.MateriaId == area.ID))
                {
                    Areas.Add(new SelectListItem
                    {
                        Selected = true,
                        Text = area.nombreMateria,
                        Value = Convert.ToString(area.ID)
                    });
                }
                else
                {
                    Areas.Add(new SelectListItem
                    {
                        Text = area.nombreMateria,
                        Value = Convert.ToString(area.ID)
                    });

                }
            }



            ViewBag.Periodo = new SelectList(db.PeriodoEscolars, "ID", "nombre");
            ViewBag.Areas = Areas;
            ViewBag.Centro = new SelectList(db.Centros, "Id", "Nombre", docente.CentroId);
            ViewBag.Grados = new SelectList(db.CentroGradoes.Where(c => c.CentroId == centroId).Select(g => g.GradoLookup), "Id", "grado");
            ViewBag.Seccion = new SelectList(db.Secciones, "Id", "Numero");
            return View(docente);
        }


        // GET: /Docente/5/Edit
        public ActionResult EditModal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }
            ViewBag.Centro = new SelectList(db.Centros, "Id", "Nombre", docente.CentroId);
            ViewBag.Seccion = db.Secciones.ToList();//new SelectList(db.Secciones.ToList(), "Id", "Numero");
            return PartialView(docente);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditModal(Docente docente)
        {
            string result = "OK";
            if (ModelState.IsValid)
            {

                foreach (var item in docente.Materias.ToArray())
                {
                    DocenteMateria docenteMatTemp = translateDocenteMateria(item);
                    item.Ciclo = docenteMatTemp.Ciclo;
                    item.Nivel = docenteMatTemp.Nivel;
                    item.Area = docenteMatTemp.Area;

                    if (item.Id < 0)
                    {
                        item.Id = item.Id * -1;
                        docente.Materias.Remove(item);
                        db.Entry(item).State = EntityState.Deleted;
                    }
                    else if (item.Id == 0)
                    {
                        db.Entry(item).State = EntityState.Added;
                    }
                    else
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                }
                db.Entry(docente).State = EntityState.Modified;
                await db.SaveChangesAsync();

            }

            var jsonData = new
            {
                result = result
            };
            ViewBag.Centro = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", docente.CentroId);
            ViewBag.Seccion = new SelectList(db.Secciones.Select(x => new { x.Id, x.Numero }), "Id", "Numero");
            return Json(jsonData);

        }







        // GET: /Docente/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }
            ViewBag.Centro = new SelectList(db.Centros, "Id", "Nombre", docente.CentroId);
            ViewBag.Seccion = new SelectList(db.Secciones, "Id", "Numero");

            if (Request.Params["Modal"] != null)
            {
                return PartialView(docente);
            }

            return View(docente);
        }

        // POST: /Docente/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Docente docente)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ModelState.IsValid)
            {
                foreach (var item in docente.Materias.ToArray())
                {
                    DocenteMateria docenteMatTemp = translateDocenteMateria(item);
                    item.Ciclo = docenteMatTemp.Ciclo;
                    item.Nivel = docenteMatTemp.Nivel;
                    item.Area = docenteMatTemp.Area;

                    if (item.Id < 0)
                    {
                        item.Id = item.Id * -1;
                        docente.Materias.Remove(item);
                        db.Entry(item).State = EntityState.Deleted;
                    }
                    else if (item.Id == 0)
                    {
                        db.Entry(item).State = EntityState.Added;
                    }
                    else
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                }

                db.Entry(docente).State = EntityState.Modified;

                db.SaveChanges();

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }

                return RedirectToAction("Index");
            }
            ViewBag.Centro = new SelectList(db.Centros, "Id", "Nombre", docente.CentroId);
            ViewBag.Seccion = new SelectList(db.Secciones, "Id", "Numero");
            return View(docente);
        }

        // GET: /Docente/5/Delete
        public ActionResult Delete(int? id, int? centroID)
        {
            ViewBag.MasterType = centroID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }
            return View(docente);
        }

        // POST: /Docente/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Docente docente = db.Docentes.Find(id);
            db.Docentes.Remove(docente);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error db", "Error al desinscribir docente, problablemente ya tenga datos en los Ciclos Formativos");
                return View();
            }
            string centroStr = Request.Form["centroId"];

            if (centroStr != null && centroStr != "")
            {
                int centroID = Convert.ToInt32(centroStr);
                return RedirectToAction("Details", "Centro", new { id = centroID });
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
