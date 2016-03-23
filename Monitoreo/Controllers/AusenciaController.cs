using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models.BO;
using Monitoreo.Models.DAL;
using Monitoreo.Models;
using Monitoreo.Helpers;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class AusenciaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();
        Logger Logger = new Logger();

        // GET: Ausencias
        [Route("Ausencias")]
        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Index()
        {
            List<Ausencia> ausencias = new List<Ausencia>();
            ausencias = db.Ausencias.Include(a => a.Persona).ToList();
            return View(ausencias);
        }

        // POST: Ausencias
        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [Route("Ausencias/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {

            List<Ausencia> ausencias = new List<Ausencia>();

            if (User.IsInRole("Administrador"))
            {
                ausencias = db.Ausencias.Include(a => a.Persona).Include(c => c.CalendarioCicloFormativo).ToList();
            }
            if (User.IsInRole("Acompanante") || User.IsInRole("Formador"))
            {
                List<Inscripcion> inscripciones = new List<Inscripcion>();
                Persona participante = new Persona();
                try
                {
                    participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();
                    if (participante != null)
                    {
                        inscripciones = db.Inscripciones.Where(p => p.Participante.Id == participante.Id).Include(p => p.Participante).ToList();
                        List<CicloFormativo> ciclosFormativos = inscripciones.Select(c => c.CicloFormativo).ToList();
                        foreach (CicloFormativo c in ciclosFormativos)
                        {
                            List<Ausencia> tempAusencias = db.Ausencias.Where(d => d.CalendarioCicloFormativo.CicloFormativoID == c.Id).ToList();
                            ausencias.AddRange(tempAusencias);
                        }
                    }

                }
                catch (Exception e)
                {
                    var msj = e.Message;
                }

            }

            var recordsTotal = ausencias.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = ausencias.Select(x => new { DT_RowId = x.Id, x.PersonaId, x.Persona.Cedula, Nombre = x.Persona.NombreCompleto, Fecha = x.CalendarioCicloFormativo.Fecha.ToString(), x.Tipo, x.Comentario, CicloFormativoTema = x.CalendarioCicloFormativo.CicloFormativo.Tema });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Comentario.Contains(searchValue)
                    );

                    recordsFiltered = data.Count();
                }
            }
            // Ordenando
            var sorting = false;
            if (values.order != null && values.order.Count() > 0)
            {
                foreach (var item in values.order)
                {
                    string sortById = (item["column"] as string[])[0];
                    string sortBy = (values.columns[int.Parse(sortById)]["data"] as string[])[0];

                    switch (sortBy)
                    {
                        case "Comentario":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Comentario);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Comentario);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.PersonaId);
            }

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

        // GET: /Ausencia/5/Details
        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ausencia ausencia = db.Ausencias.Find(id);
            if (ausencia == null)
            {
                return HttpNotFound();
            }
            return View(ausencia);
        }

        // GET: Ausencias/Create
        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [Route("Ausencias/Create")]
        public ActionResult Create()
        {
            List<SuperCicloFormativo> superCiclos = new List<SuperCicloFormativo>();
            if (User.IsInRole("Administrador") || User.IsInRole("Coordinador"))
            {
                ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes, "Id", "nombre");
                ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");
                ViewBag.GrupoCicloFormativo = new SelectList(db.GruposCiclosFormativos.Select(c => new { Id = c.ID, Nombre = c.Centro.Nombre }), "Id", "Nombre");

                //ViewBag.GrupoCicloFormativoId = new SelectList(db.GruposCiclosFormativos, "Id", "codigo"); 
            }
            if (User.IsInRole("Acompanante") || User.IsInRole("Formador"))
            {
                List<Inscripcion> inscripciones = new List<Inscripcion>();
                List<CicloFormativo> ciclosFormativos = new List<CicloFormativo>();

                Persona participante = new Persona();
                participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();
                if (participante != null)
                {
                    inscripciones = db.Inscripciones.Where(p => p.Participante.Id == participante.Id).Include(p => p.Participante).ToList();
                    ciclosFormativos = inscripciones.Select(c => c.CicloFormativo).ToList();
                    foreach (CicloFormativo c in ciclosFormativos)
                    {
                        List<SuperCicloFormativo> tempSuperCiclos = db.SuperCicloFormativoes.Where(d => d.CiclosFormativos.Any(s => s.Id == c.Id)).ToList();
                        superCiclos.AddRange(tempSuperCiclos);
                    }
                }
                ViewBag.SuperCicloFormativoId = new SelectList(superCiclos.Distinct(), "Id", "nombre");
                ViewBag.CicloFormativoId = new SelectList(ciclosFormativos, "Id", "Tema");
                ViewBag.GrupoCicloFormativo = new SelectList(db.GruposCiclosFormativos.Select(c => new { Id = c.ID, Nombre = c.Centro.Nombre }), "Id", "Nombre");
            }

            return View();
        }

        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [Route("Ausencias/GetPersonalBySeccion")]
        [HttpPost]
        public JsonResult GetPersonalBySeccion(int seccionId, String cicloId)
        {
            var participantes = new List<Persona>();
            var calendarios = new List<CalendarioCicloFormativo>();

            try
            {
                CicloFormativo ciclo = db.CiclosFormativos.Where(c => c.Tema == cicloId).SingleOrDefault();
                //GrupoCicloFormativo seccion = db.GruposCiclosFormativos.Where(s => s.ID == seccionId).Where(c => c.CicloFormativoId == ciclo.Id).SingleOrDefault();
                calendarios = db.CalendarioCicloFormativoes.Where(c => c.CicloFormativoID == ciclo.Id).ToList();

                IEnumerable<Inscripcion> inscripcion = ciclo.Participantes.Where(s => s.GrupoCicloFormativoId == seccionId);
                IEnumerable<Inscripcion> inscripciones = inscripcion.Where(s => s.GrupoCicloFormativoId == seccionId)
                                                                    .Where(c => c.CicloFormativoId == ciclo.Id);

                //*** Participantes Inscritos en el Ciclo Formativo ***//
                foreach (Inscripcion i in inscripciones)
                {
                    Persona participante = new Persona();
                    participante = i.Participante;
                    participantes.Add(participante);
                }

            }
            catch (Exception exp)
            {
                var dummy = exp.Message;
            }
            var jsonData = new
            {
                data = participantes,
                calendarios = calendarios.Select(y => new
                {
                    id = y.Id,
                    fecha = y.Fecha.ToString()
                })
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [Route("Ausencias/GetGruposByCicloId")]
        [HttpPost]
        public JsonResult GetGruposByCicloId(int cicloId)
        {
            List<GrupoCicloFormativo> grupos = new List<GrupoCicloFormativo>();
            if (User.IsInRole("Administrador") || User.IsInRole("Coordinador"))
            {
                grupos = db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == cicloId).ToList();
            }
            if (User.IsInRole("Acompanante") || User.IsInRole("Formador"))
            {
                List<Inscripcion> inscripciones = new List<Inscripcion>();
                Persona participante = new Persona();
                participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();
                grupos = db.GruposCiclosFormativos.Where(c => c.CicloFormativoId == cicloId).Where(p => p.inscripciones.Any(u => u.ParticipanteId == participante.Id)).ToList();
            }

            var jsonData = new
            {
                data = grupos.Select(y => new
                {
                    id = y.ID,
                    nombre = y.Centro.Nombre
                })

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [Route("Ausencias/GetCiclosBySuperCicloId")]
        [HttpPost]
        public JsonResult GetCiclosBySuperCicloId(int superCicloId)
        {
            var ciclos = db.CiclosFormativos.Select(x => new {x.Id, x.Tema, x.SuperCicloFormativoId }).Where(c => c.SuperCicloFormativoId == superCicloId).ToList();

            var jsonData = new
            {
                data = ciclos.Select(y => new
                {
                    id = y.Id,
                    tema = y.Tema,
                })

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        public JsonResult GetAsistenciaPersonasActividadFormativa(int CicloFormativoId, int calendarioCicloId, DatatablesParams values)
        {

            List<Monitoreo.Models.BO.ViewModels.AssitenciaVM> Asistencias = new List<Models.BO.ViewModels.AssitenciaVM>();

            var recordsTotal = 0;
            var recordsFiltered = 0;
            var limit = 0;
            var from = 0;

            var ausencias = new List<Ausencia>();
            List<Inscripcion> inscripciones = new List<Inscripcion>();
            try
            {
                int countInscripciones = db.Inscripciones.Select(x => new { x.Id }).Count();
                recordsTotal = countInscripciones;
                recordsFiltered = recordsTotal;
                limit = values.length > 0 ? values.length : recordsTotal;
                from = values.start;

                inscripciones = db.Inscripciones.Include(i => i.CicloFormativo).Include(i => i.Participante).Include(i => i.GrupoCicloFormativo).Where(s => s.CicloFormativoId == CicloFormativoId).OrderBy(n => n.Participante.Nombres).Skip(from).Take(limit).ToList();

                foreach (var inscripcion in inscripciones)
                {
                    var asisteciaVmTemp = new Monitoreo.Models.BO.ViewModels.AssitenciaVM();
                    asisteciaVmTemp.participanteID = inscripcion.Participante.Id;
                    asisteciaVmTemp.participanteCedula = inscripcion.Participante.Cedula;
                    asisteciaVmTemp.participanteNombre = inscripcion.Participante.NombreCompleto;
                    bool noAsistio = db.Ausencias.Select(x => new { x.CalendarioCicloFormativoId, x.PersonaId }).Where(c => c.CalendarioCicloFormativoId == calendarioCicloId).Any(p => p.PersonaId == asisteciaVmTemp.participanteID);
                    if (!noAsistio)
                    {
                        asisteciaVmTemp.asistio = true;
                    }
                    else
                    {
                        asisteciaVmTemp.asistio = false;
                    }
                    Asistencias.Add(asisteciaVmTemp);
                }

            }
            catch (Exception e)
            {
                var dummy = e.Message;
            }
            var jsonData = new
            {
                data = Asistencias.Select(y => new
                {
                    partcicipanteId = y.participanteID,
                    Nombre = y.participanteNombre,
                    Cedula = y.participanteCedula,
                    Asistio = y.asistio
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }




        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [HttpPost]
        public JsonResult DeleteAusenciaPersonal(int calendarioCicloId, int personalId)
        {

            var response = "OK";
            try
            {
                Personal personal = db.Personal.Find(personalId);
                Persona persona = personal.Persona;

                Ausencia ausencia = db.Ausencias.Where(c => c.CalendarioCicloFormativoId == calendarioCicloId).Where(p => p.PersonaId == persona.Id).SingleOrDefault();
                db.Ausencias.Remove(ausencia);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                response = "ERROR";
            }

            var jsonData = new
            {
                response
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }








        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [HttpPost]
        public JsonResult CreateAusenciaPersonal(int calendarioCicloId, int personalId)
        {

            var response = "OK";
            Personal personal = db.Personal.Find(personalId);
            Persona persona = personal.Persona;
            bool isRepeated = db.Ausencias.Select(x => new { x.CalendarioCicloFormativoId, x.PersonaId}).Where(c => c.CalendarioCicloFormativoId == calendarioCicloId).Any(p => p.PersonaId == persona.Id);
            if (!isRepeated)
            {
                try
                {
                    Ausencia ausencia = new Ausencia();
                    ausencia.CalendarioCicloFormativoId = calendarioCicloId;
                    ausencia.PersonaId = persona.Id;
                    ausencia.Tipo = TipoDeAusencia.Justificada;
                    ausencia.Comentario = "";
                    db.Ausencias.Add(ausencia);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    response = "ERROR";
                }
            }


            var jsonData = new
            {
                response
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }








        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        // POST: Ausencias/Create
        [Route("Ausencias/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ausencia ausencia)
        {

            String personaCedula = Request.Form["personaId"].ToString();
            String calendariosCicloFormativosIds = Request.Form["fechasCalendarioID"].ToString();
            string[] calendariosIds;
            calendariosIds = calendariosCicloFormativosIds.Split(',');

            try
            {
                Persona persona = db.Personas.Where(p => p.Cedula == personaCedula).SingleOrDefault();
                ausencia.PersonaId = persona.Id;

                foreach (string c in calendariosIds)
                {
                    ausencia.CalendarioCicloFormativoId = Convert.ToInt32(c);
                    bool isRepeated = db.Ausencias.Any(cal => cal.Id == ausencia.CalendarioCicloFormativoId);

                    if (db.Ausencias.Any(x => x.CalendarioCicloFormativoId == ausencia.CalendarioCicloFormativoId && x.PersonaId == ausencia.PersonaId) == false)
                    {
                        db.Ausencias.Add(ausencia);
                        db.SaveChanges();
                    }
                }

            }
            catch (Exception exp)
            {
                var dummy = exp.Message;
            }

            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");
            ViewBag.GrupoCicloFormativoId = new SelectList(db.GruposCiclosFormativos, "Id", "codigo");
            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes, "Id", "nombre");
            return RedirectToAction("Index");
            //return View(ausencia);
        }

        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        // GET: /Ausencia/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ausencia ausencia = db.Ausencias.Find(id);
            if (ausencia == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "Cedula", ausencia.PersonaId);
            ViewBag.CalendarioId = new SelectList(db.CalendarioCicloFormativoes, "Id", "Fecha", ausencia.CalendarioCicloFormativoId);
            return View(ausencia);
        }

        // POST: /Ausencia/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ausencia ausencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ausencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "Cedula", ausencia.PersonaId);
            ViewBag.CalendarioId = new SelectList(db.CalendarioCicloFormativoes, "Id", "Fecha", ausencia.CalendarioCicloFormativoId);
            return View(ausencia);
        }

        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        // GET: /Ausencia/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ausencia ausencia = db.Ausencias.Find(id);
            if (ausencia == null)
            {
                return HttpNotFound();
            }
            return View(ausencia);
        }

        // POST: /Ausencia/5/Delete
        [Authorize(Roles = "Administrador, Acompanante, Coordinador, AdministradorTransversal,EspecialistaCurricular")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ausencia ausencia = db.Ausencias.Find(id);
            db.Ausencias.Remove(ausencia);
            db.SaveChanges();
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
