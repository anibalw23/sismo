using Monitoreo.Models;
using Monitoreo.Models.BO;
using Monitoreo.Models.BO.EvaluacionAcompanamiento;
using Monitoreo.Models.BO.ViewModels;
using Monitoreo.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private MonitoreoContext db = new MonitoreoContext();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        public ActionResult ReporteEvaluacionesByCiclo()
        {
            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes.Select(x => new { x.Id, x.nombre }), "Id", "nombre");
            ViewBag.CentroId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            ViewBag.RedId = new SelectList(db.Redes.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            ViewBag.EvaluacionId = new SelectList(db.EvaluacionAcompanamientoes.Select(x => new { x.Id, x.Titulo }), "Id", "Titulo");
            return View();
        }

        public ActionResult ResumenEvalsAcompanamiento()
        {
            ViewBag.DistritoId = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes.Select(x => new { x.Id, x.nombre }), "Id", "nombre");
            ViewBag.EvaluacionId = new SelectList(db.EvaluacionAcompanamientoes.Select(x => new { x.Id, x.Titulo }), "Id", "Titulo");
            return View();
        }


        [HttpPost]
        public async Task <JsonResult> GetResumenEvalsByCicloDistrito(string cicloId, int distritoId, int evaluacionId)
        {
            var evaluacionesAcompanamientoRespuestas = new List<EvaluacionAcompanamientoRespuesta>();
            Distrito distrito = await db.Distritos.FindAsync(distritoId);
            List<Centro> centros = db.Centros.Where(d => d.Red.DistritoId == distritoId).ToList();
            List<ResumenEvalsByCiclo> resumenEvals = new List<ResumenEvalsByCiclo>();
            var preguntas = db.EvaluacionAcompanamientoes.Where(i => i.Id == evaluacionId).SingleOrDefault().PreguntasAcomp;

            String[] ciclos = cicloId.Split(',');
            foreach (var cicloIdTemp in ciclos)
            {
                
                int cicloTempInt = Convert.ToInt32(cicloIdTemp);
                string cicloNombre = db.SuperCicloFormativoes.Find(cicloTempInt).nombre; 
                //List<EvaluacionAcompanamientoRespuesta> evaluacionesAcompanamientoRespuestasTemp = db.EvaluacionAcompanamientoRespuestas.AsNoTracking().Where(c => c.EvaluacionAcompId == evaluacionId).Where(f => f.InscripcionActividadAcompanamiento.Personal.Centro.Red.DistritoId == distritoId).Where(k => k.InscripcionActividadAcompanamiento.ActividadAcompanamiento.SuperCicloFormativoId == cicloTempInt).ToList();
                foreach (var centro in centros)
                {
                    List<EvaluacionAcompanamientoRespuesta> respuestasCentro = new List<EvaluacionAcompanamientoRespuesta>();
                    respuestasCentro = db.EvaluacionAcompanamientoRespuestas.AsNoTracking().Where(c => c.EvaluacionAcompId == evaluacionId).Where(f => f.InscripcionActividadAcompanamiento.Personal.CentroId == centro.Id).Where(k => k.InscripcionActividadAcompanamiento.ActividadAcompanamiento.SuperCicloFormativoId == cicloTempInt).ToList();

                    foreach (var p in preguntas)
                    {
                        ResumenEvalsByCiclo resumen = new ResumenEvalsByCiclo();
                        int count = respuestasCentro.Where(x => x.PreguntaAcompId == p.Id).Count();
                        List<int> valores = new List<int>();
                        foreach (var resp in respuestasCentro.Where(x => x.PreguntaAcompId == p.Id))
                        {
                            int valor = Convert.ToInt32(resp.Valor);
                            valores.Add(valor);
                        }
                        double average = valores.Count() > 0? valores.Average(): 0;
                        average = Math.Round(average, 2);
                        resumen.pregunta = p.Descripcion;
                        resumen.respuestaAverage = average;
                        resumen.centro = centro.Nombre;
                        resumen.ciclo = cicloNombre;
                        resumenEvals.Add(resumen);
                    }
                }
                

            }

            var jsonData = new
            {
                data = resumenEvals.Select(y => new
                {
                    y.ciclo,
                    y.centro,
                    y.pregunta,
                    y.respuestaAverage
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }








        [HttpPost]
        public JsonResult GetEvaluacionesByCicloCentro(string cicloId, int centroId, int? redId, int evaluacionId)
        {
            var evaluacionesAcompanamientoRespuestas = new List<EvaluacionAcompanamientoRespuesta>();

            String[] ciclos = cicloId.Split(',');
            foreach (var cicloIdTemp in ciclos)
            {
                int cicloTempInt = Convert.ToInt32(cicloIdTemp);
                //var evaluacionesAcompanamientoRespuestasTemp = db.EvaluacionAcompanamientoRespuestas.AsNoTracking().Where(c => c.EvaluacionAcompId == evaluacionId).Where(f => f.InscripcionActividadAcompanamiento.Personal.CentroId == centroId).Where(k => k.InscripcionActividadAcompanamiento.ActividadAcompanamiento.SuperCicloFormativoId == cicloTempInt).Select(x => new { x.Id, x.InscripcionActividadAcompanamiento, x.PreguntaAcomp, x.Valor });
                List<EvaluacionAcompanamientoRespuesta> evaluacionesAcompanamientoRespuestasTemp = db.EvaluacionAcompanamientoRespuestas.AsNoTracking().Where(c => c.EvaluacionAcompId == evaluacionId).Where(f => f.InscripcionActividadAcompanamiento.Personal.CentroId == centroId).Where(k => k.InscripcionActividadAcompanamiento.ActividadAcompanamiento.SuperCicloFormativoId == cicloTempInt).ToList();
                if (evaluacionesAcompanamientoRespuestasTemp != null)
                {
                    evaluacionesAcompanamientoRespuestas.AddRange(evaluacionesAcompanamientoRespuestasTemp);
                }
            }

            var jsonData = new
            {
                data = evaluacionesAcompanamientoRespuestas.Select(y => new
                {
                    Id = y.Id.ToString(),
                    cedula = y.InscripcionActividadAcompanamiento.Personal.Persona.Cedula,
                    nombre = y.InscripcionActividadAcompanamiento.Personal.Persona.Nombres,
                    valor = y.Valor == null ? "0" : y.Valor.ToString(),
                    pregunta = y.PreguntaAcomp.Descripcion,
                    tipoActividad = y.InscripcionActividadAcompanamiento.ActividadAcompanamiento.TipoAcompanamiento.ToString(),
                    cicloFormativo = y.InscripcionActividadAcompanamiento.ActividadAcompanamiento.SuperCicloFormativo.nombre,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        public ActionResult ReporteAsistenciaByCiclo()
        {
            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes.Select(x => new { x.Id, x.nombre}), "Id", "nombre");
            ViewBag.CentroId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre");
            ViewBag.RedId = new SelectList(db.Redes.Select(x => new {x.Id, x.Nombre }), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        //[OutputCache(Duration = 43200, VaryByParam = "cicloId;centroId;redId")]
        public JsonResult GetAsistenciaByCicloCentro(string cicloId, int? centroId, int? redId, bool? horasPresenciales, bool includeActive)
        {
            List<InscripcionActividadAcompanamiento> inscripcionesAcompanamiento = new List<InscripcionActividadAcompanamiento>();
            List<Inscripcion> inscripcionesPresenciales = new List<Inscripcion>();
            List<AsistenciaViewModel> asistenciasPresenciales = new List<AsistenciaViewModel>();
            List<Docente> docentesCentro = new List<Docente>();
            List<InscripcionActividadAcompanamiento> inscripcionesAcompanamientoTemp = new List<InscripcionActividadAcompanamiento>();

            bool searchByRed = false;

            List<Inscripcion> inscripcionesPresencialesTemp = new List<Inscripcion>();
            if (centroId == 0 && redId != 0 && cicloId != "") //Busca los docentes por Red
            {
                if (includeActive)
                {
                    docentesCentro = db.Docentes.Include("Centro").AsNoTracking().Where(c => c.Centro.RedId == redId).Where(a => a.isActive == true).ToList();
                }
                else {
                    docentesCentro = db.Docentes.Include("Centro").AsNoTracking().Where(c => c.Centro.RedId == redId).ToList();
                }
                
                searchByRed = true;
            }
            else
            {
                if (centroId != 0 && redId != 0 && cicloId != "") // Busca los docentes por centro
                {

                    if (includeActive)
                    {
                        docentesCentro = db.Docentes.AsNoTracking().Where(c => c.CentroId == centroId).Where(a => a.isActive == true).ToList();
                    }
                    else
                    {
                        docentesCentro = db.Docentes.AsNoTracking().Where(c => c.CentroId == centroId).ToList();
                    }
                    
                    searchByRed = false;
                }
                else
                {
                    //Error
                    var jsonErrorData = new
                    {
                        data = "ERROR"
                    };
                    return Json(jsonErrorData, JsonRequestBehavior.AllowGet);

                }


            }


            String[] ciclos = cicloId.Split(',');

            //Esto es para que salgan ordenadas las columnas de Acompanamiento
            inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.AcompanamientoAula }, Area = DocenteArea.NA, fecha = DateTime.Now });
            inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.AcompanamientoTutorial }, Area = DocenteArea.NA, fecha = DateTime.Now });
            inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.ClaseModelo }, Area = DocenteArea.NA, fecha = DateTime.Now });
            inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.GrupoPedagogico }, Area = DocenteArea.NA, fecha = DateTime.Now });
            if (horasPresenciales == true)
            {
                inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = -1, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0 }, Area = DocenteArea.NA, fecha = DateTime.Now });
                asistenciasPresenciales.Add(new AsistenciaViewModel { personaId = 0, cedula = "", horasAsistidad = 0, cicloID = 0 });
            }

            //docentesCentro = db.Docentes.AsNoTracking().Where(c => c.CentroId == centroId).ToList();

            foreach (var cicloIdTemp in ciclos)
            {
                int cicloTempInt = Convert.ToInt32(cicloIdTemp);
                var result = (List<InscripcionActividadAcompanamiento>)null;
                var resultPresencial = (List<Inscripcion>)null;
                if (searchByRed == true)
                {
                    result = db.InscripcionesActividadesAcompanamiento.AsNoTracking().Where(c => c.ActividadAcompanamiento.SuperCicloFormativoId == cicloTempInt).Where(e => e.Personal.Centro.RedId == redId).ToList();
                    if (horasPresenciales == true)
                    {
                        resultPresencial = db.Inscripciones.AsNoTracking().Where(s => s.CicloFormativo.SuperCicloFormativoId == cicloTempInt).Where(e => e.GrupoCicloFormativo.Centro.RedId == redId).ToList();
                    }
                }
                else
                {
                    result = db.InscripcionesActividadesAcompanamiento.AsNoTracking().Where(c => c.ActividadAcompanamiento.SuperCicloFormativoId == cicloTempInt).Where(e => e.Personal.CentroId == centroId).ToList();
                    if (horasPresenciales == true)
                    {
                        resultPresencial = db.Inscripciones.AsNoTracking().Where(s => s.CicloFormativo.SuperCicloFormativoId == cicloTempInt).Where(e => e.GrupoCicloFormativo.Centro.Id == centroId).ToList();
                    }
                }

                if (horasPresenciales == true)
                {
                    if (resultPresencial != null)
                    {
                        inscripcionesPresencialesTemp.AddRange(resultPresencial);
                    }
                }


                if (result != null)
                {
                    inscripcionesAcompanamientoTemp.AddRange(result);
                    //Esto es para que salgan ordenadas las columnas de Acompanamiento
                    var superCiclo = db.SuperCicloFormativoes.Find(cicloTempInt);
                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.AcompanamientoAula, SuperCicloFormativo = superCiclo }, Area = DocenteArea.NA, fecha = DateTime.Now });
                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.AcompanamientoTutorial, SuperCicloFormativo = superCiclo }, Area = DocenteArea.NA, fecha = DateTime.Now });
                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.ClaseModelo, SuperCicloFormativo = superCiclo }, Area = DocenteArea.NA, fecha = DateTime.Now });
                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = new Personal { Persona = new Persona { Cedula = "", Nombres = "" } }, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.GrupoPedagogico, SuperCicloFormativo = superCiclo }, Area = DocenteArea.NA, fecha = DateTime.Now });
                }
            }


            foreach (var doc in docentesCentro)
            {
                bool hasAcompanamiento = inscripcionesAcompanamientoTemp.Any(p => p.Personal.Persona.Cedula == doc.Persona.Cedula);
                if (hasAcompanamiento)
                {
                    foreach (var i in inscripcionesAcompanamientoTemp.Where(p => p.Personal.Persona.Cedula == doc.Persona.Cedula))
                    {
                        inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = i.ID, horas = i.horas, Personal = doc, ActividadAcompanamiento = i.ActividadAcompanamiento, Area = i.Area, fecha = i.fecha });
                    }
                }
                else
                {
                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = doc, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.AcompanamientoAula }, Area = DocenteArea.NA, fecha = DateTime.Now });
                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = doc, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.AcompanamientoTutorial }, Area = DocenteArea.NA, fecha = DateTime.Now });
                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = doc, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.ClaseModelo }, Area = DocenteArea.NA, fecha = DateTime.Now });
                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = 0, Personal = doc, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0, TipoAcompanamiento = TipoAcompanamiento.GrupoPedagogico }, Area = DocenteArea.NA, fecha = DateTime.Now });

                }

                if (horasPresenciales == true)
                {
                    bool hasPresenciales = inscripcionesPresencialesTemp.Any(p => p.Participante.Cedula == doc.Persona.Cedula);
                    if (hasPresenciales)
                    {
                        int totalHorasFaltadas = 0;
                        foreach (var i in inscripcionesPresencialesTemp.Where(p => p.Participante.Cedula == doc.Persona.Cedula))
                        {
                            var calcs = db.CalendarioCicloFormativoes.AsNoTracking().Select(x => new { x.Id, x.CicloFormativo, x.horas }).Where(a => a.CicloFormativo.Id == i.CicloFormativoId).ToList();
                            int totalDias = calcs.Count();
                            int totalHoras = calcs.AsEnumerable().Sum(h => h.horas);
                            var ausencia = db.Ausencias.AsNoTracking().Where(p => p.Persona.Cedula == i.Participante.Cedula).Where(c => c.CalendarioCicloFormativo.CicloFormativo.SuperCicloFormativoId == i.CicloFormativo.SuperCicloFormativoId);
                            int diasFaltados = ausencia.Count();
                            if (totalDias != 0)
                            {
                                // Calculo de Total de horas faltadas
                                foreach (Ausencia au in ausencia)
                                {
                                    var calTemp = calcs.Where(l => l.Id == au.CalendarioCicloFormativoId).SingleOrDefault();
                                    int horaAcc = 0;
                                    if (calTemp != null)
                                    {
                                        horaAcc = calTemp.horas;
                                    }
                                    totalHorasFaltadas = horaAcc + totalHorasFaltadas;
                                }
                                int horasAsistidas = totalHoras - totalHorasFaltadas;
                                if (!asistenciasPresenciales.Any(c => c.cedula == i.Participante.Cedula))
                                {
                                    asistenciasPresenciales.Add(new AsistenciaViewModel { personaId = i.ParticipanteId, cedula = i.Participante.Cedula, horasAsistidad = horasAsistidas, cicloID = i.CicloFormativoId });
                                    inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = -1, Personal = doc, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0 }, Area = DocenteArea.NA, fecha = DateTime.Now });
                                }
                            }


                        }
                    }
                    else
                    {
                        asistenciasPresenciales.Add(new AsistenciaViewModel { personaId = doc.PersonaId, cedula = doc.Persona.Cedula, horasAsistidad = 0, cicloID = 0 });
                        inscripcionesAcompanamiento.Add(new InscripcionActividadAcompanamiento { ID = 0, horas = -1, Personal = doc, ActividadAcompanamiento = new ActividadAcompanamiento { ID = 0 }, Area = DocenteArea.NA, fecha = DateTime.Now });

                    }
                }

            }

            var jsonData = new
            {
                data = inscripcionesAcompanamiento.Select(y => new
                {
                    cicloFormativo = y.ActividadAcompanamiento.SuperCicloFormativo != null ? y.ActividadAcompanamiento.SuperCicloFormativo.nombre : "NA",
                    cedula = y.Personal.Persona.Cedula,
                    nombre = y.Personal.Persona.Nombres,
                    fecha = y.fecha.ToShortDateString(),
                    area = y.Area.ToString(),
                    horas = y.horas != -1 ? y.horas : asistenciasPresenciales.Where(x => x.cedula == y.Personal.Persona.Cedula).FirstOrDefault() != null ? asistenciasPresenciales.Where(x => x.cedula == y.Personal.Persona.Cedula).FirstOrDefault().horasAsistidad : 0,
                    tipoActividad = y.ActividadAcompanamiento != null ? y.ActividadAcompanamiento.TipoAcompanamiento != 0 ? y.ActividadAcompanamiento.TipoAcompanamiento.ToString() : "Actividad Presencial" : "Actividad Presencial",
                    Centro = y.Personal.Centro != null ? y.Personal.Centro.Nombre : ""
                }).OrderBy(n => n.nombre),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }


        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        public ActionResult BusquedaPersonal()
        {
            return View();
        }

        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        public ActionResult ReportePivotePersonal()
        {
            ViewBag.DistritoId = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            return View();
        }


        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        public ActionResult DocentesPorGrado()
        {
            ViewBag.DistritoId = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            return View();
        }



        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        public ActionResult ReportePivotePersonalAdministrativo()
        {
            ViewBag.DistritoId = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            return View();
        }

        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        public ActionResult ReporteFiltradoDocentes()
        {
            ViewBag.DistritoId = new SelectList(db.Distritos.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");

            var Arealist = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(DocenteArea)))
            {
                int val = (int)item;
                string b = Enum.GetName(typeof(DocenteArea), item);
                Arealist.Add((int)val, b);
            }
            ViewBag.Arealist = new SelectList(Arealist, "Key", "Value");

            var Nivellist = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(NivelEducativo)))
            {
                int val = (int)item;
                string b = Enum.GetName(typeof(NivelEducativo), item);
                Nivellist.Add((int)val, b);
            }
            ViewBag.Nivellist = new SelectList(Nivellist, "Key", "Value");


            var Ciclolist = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(DocenteCiclo)))
            {
                int val = (int)item;
                string b = Enum.GetName(typeof(DocenteCiclo), item);
                Ciclolist.Add((int)val, b);
            }
            ViewBag.Ciclolist = new SelectList(Ciclolist, "Key", "Value");

            //ViewBag.Tanda = new SelectList(TandasCentro, "Id", "Nombre");


            return View();
        }


        [Route("Reporte/GetRedesByDistritoId")]
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "distritoId")]
        public JsonResult GetRedesByDistritoId(int distritoId)
        {

            var redes = db.Redes.Select(x => new { x.Id, x.Nombre, x.DistritoId}).Where(d => d.DistritoId == distritoId).ToList();
            var redesOutList = new List<Red>();

            foreach (var red in redes)
            {
                redesOutList.Add(new Red { Id = red.Id, Nombre = red.Nombre, DistritoId = red.DistritoId });
            }
            var jsonData = new
            {
                data = redesOutList.Select(y => new
                {
                    y.Id,
                    y.Nombre

                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        [Route("Reporte/GetCentrosByRedId")]
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "redID")]
        public JsonResult GetCentrosByRedId(int redID)
        {
            var centros = db.Centros.Select(x => new { x.Id, x.RedId, x.Nombre}).Where(r => r.RedId == redID).ToList();
            var jsonData = new
            {
                data = centros.Select(y => new
                {
                    y.Id,
                    y.Nombre
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Route("Reporte/GetDocentesByCentro")]
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricularvvvv")]
        public JsonResult GetDocentesByCentro(int centroID)
        {
            var personal = db.Docentes.Where(c => c.CentroId == centroID).ToList();
            var jsonData = new
            {
                data = personal.Select(y => new
                {
                    y.Id,
                    y.Persona.NombreCompleto,
                    y.Persona.Cedula,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        public JsonResult GetDocentesByCentroAreaNivelCiclo(int centroID, int? areaId, int? nivelId, int? cicloId)
        {

            var personal = new List<Docente>();
            try
            {
                string Area = Enum.GetName(typeof(DocenteArea), areaId);
                string Nivel = Enum.GetName(typeof(NivelEducativo), nivelId);
                string Ciclo = Enum.GetName(typeof(DocenteCiclo), cicloId);

                personal = db.Docentes.Where(c => c.CentroId == centroID).ToList();

                if (areaId != 0 && areaId != null)
                {
                    personal = personal.Where(m => m.Materias.Any(a => a.Area.ToString() == Area)).ToList();
                }

                if (nivelId != 0 && nivelId != null)
                {
                    personal = personal.Where(m => m.Materias.Any(a => a.Nivel.ToString() == Nivel)).ToList();
                }

                if (cicloId != 0 && cicloId != null)
                {
                    personal = personal.Where(m => m.Materias.Any(a => a.Ciclo.ToString() == Ciclo)).ToList();
                }
            }
            catch (Exception e)
            {
                var dummy = e.Message;
            }
            var jsonData = new
            {
                data = personal.Select(y => new
                {
                    y.Id,
                    y.Persona.NombreCompleto,
                    y.Persona.Cedula,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Route("Reporte/GetPersonal")]
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "cedula;nombre")]
        public JsonResult GetPersonal(string cedula, string nombre)
        {
            var docente = db.Docentes.Where(d => d.Persona.Cedula == cedula).ToList();
            var jsonData = new
            {
                data = docente.Select(y => new
                {
                    y.Id,
                    y.Persona.Cedula,
                    y.Persona.NombreCompleto,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        [Route("Reporte/GetCedulasPersonas")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador, AdministradorTransversal,Visualizador,EspecialistaCurricular")]
        //[OutputCache(Duration = 86400, VaryByParam = "searchTerm")]
        public JsonResult GetCedulasPersonas(string searchTerm)
        {
            List<Docente> docentes = new List<Docente>();

            if (searchTerm != "")
            {
                docentes = db.Docentes.Where(d => d.Persona.Cedula.ToUpper().Contains(searchTerm.ToUpper()) || d.Persona.Nombres.Contains(searchTerm)).Take(20).ToList();
            }

            var jsonData = new
            {
                data = docentes.Select(y => new
                {
                    y.Id,
                    Cedula = y.Persona.Cedula,
                    NombreCompleto = y.Persona.NombreCompleto
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }




    }
}