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
using System.Data.Entity.Infrastructure;
using Monitoreo.Models.BO.ViewModels;

namespace Monitoreo.Controllers
{
    //[Authorize(Roles = "Administrador")]
    [Authorize]
    public class EvaluacionController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Evaluaciones
        [Route("Evaluaciones")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            List<Evaluacion> evaluaciones = new List<Evaluacion>();

            if (User.IsInRole("Administrador") || User.IsInRole("Coordinador"))
            {
                evaluaciones = db.Evaluaciones.Include(e => e.CicloFormativo).ToList();
            }
            if (User.IsInRole("Acompanante") || User.IsInRole("Formador"))
            {
                Persona participante = new Persona();
                List<Inscripcion> inscripciones = new List<Inscripcion>();
                try
                {
                    participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();
                    if (participante != null)
                    {
                        inscripciones = db.Inscripciones.Where(p => p.Participante.Id == participante.Id).Include(p => p.Participante).ToList();
                        List<CicloFormativo> ciclosFormativos = inscripciones.Select(c => c.CicloFormativo).ToList();
                        foreach (CicloFormativo c in ciclosFormativos)
                        {
                            List<Evaluacion> tempEvaluaciones = db.Evaluaciones.Where(d => d.CicloFormativoId == c.Id).ToList();
                            evaluaciones.AddRange(tempEvaluaciones);
                        }
                    }
                }
                catch (Exception e)
                {
                    var msj = e.Message;
                }


            }

            //var evaluaciones = db.Evaluaciones.Include(e => e.CicloFormativo);
            return View(evaluaciones.ToList());
        }




        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public JsonResult GetEvaluacionesBySuperCicloByPersona(int superCicloFormativoId, int? docenteId)
        {
            List<EvaluacionPersonalVM> evaluacionesPersonal = new List<EvaluacionPersonalVM>();
            List<Evaluacion> evaluaciones = new List<Evaluacion>();

            List<Inscripcion> inscripciones = new List<Inscripcion>();

            try
            {
                Persona persona = db.Personas.Find(docenteId);
                inscripciones = persona.inscripciones.Where(c => c.CicloFormativo.SuperCicloFormativoId == superCicloFormativoId).ToList();
                foreach(var i in inscripciones){
                    if(i.ParticipanteId == docenteId){
                        List<Evaluacion> evaluacionesTemp = new List<Evaluacion>();
                        evaluacionesTemp = db.Evaluaciones.Where(c => c.CicloFormativo.SuperCicloFormativoId == superCicloFormativoId).ToList();
                        foreach(var eval in evaluacionesTemp){
                            evaluaciones.Add(eval);
                        }
                    }
                }
               
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error 5063", "Error favor contacte a su admin");
            }


            var jsonData = new
            {
                data = evaluaciones.Select(y => new
                {
                    id = y.Id,
                    nombre = y.Titulo,
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }










        // POST: Evaluaciones
        [Route("Evaluaciones/GetDataJson")]
        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
             
            List<Evaluacion> evaluaciones = new List<Evaluacion>();

            if (User.IsInRole("Administrador") || User.IsInRole("Coordinador"))
            {
               evaluaciones =  db.Evaluaciones.Include(e => e.CicloFormativo).ToList();
            
            }
            if (User.IsInRole("Acompanante") || User.IsInRole("Formador"))
            {
                Persona participante = new Persona();
                List<Inscripcion> inscripciones = new List<Inscripcion>();
                try {
                    participante = db.Personas.Where(p => p.Cedula == User.Identity.Name).SingleOrDefault();                    
                    if (participante != null) {
                        inscripciones = db.Inscripciones.Where(p => p.Participante.Id == participante.Id).Include(p => p.Participante).ToList();
                        List<CicloFormativo> ciclosFormativos = inscripciones.Select(c => c.CicloFormativo).ToList();
                        foreach (CicloFormativo c in ciclosFormativos)
                        {
                            List<Evaluacion> tempEvaluaciones = db.Evaluaciones.Where(d => d.CicloFormativoId == c.Id).ToList();
                            evaluaciones.AddRange(tempEvaluaciones);
                        }
                        //evaluaciones = db.Evaluaciones.Include(e => e.CicloFormativo).Where(c => c.).ToList();
                    }
                }
                catch (Exception e)
                {
                    var msj = e.Message;
                }
               

            }
            var recordsTotal = evaluaciones.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = evaluaciones.Select(x => new { DT_RowId = x.Id, x.Titulo, x.Fecha, x.TipoEvaluacion, x.FormaEvaluacion, CicloFormativo = x.CicloFormativo.Tema, x.TipoDeObjeto, creadoPor = x.creadoPor });

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
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Fecha);
            }

            // Preparando respuesta y ejecutando consulta
            var jsonData = new
            {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.Skip(from).Take(limit).ToList().Select(x => new
                {
                    x.DT_RowId,
                    x.Titulo,
                    x.Fecha,
                    TipoEvaluacion = Enum.GetName(typeof(TipoEvaluacion), x.TipoEvaluacion),
                    FormaEvaluacion = Enum.GetName(typeof(FormaEvaluacion), x.FormaEvaluacion),
                    x.CicloFormativo,
                    TipoDeObjeto = Enum.GetName(typeof(TipoDeObjeto), x.TipoDeObjeto),
                    creadoPor = x.creadoPor,
                    userName = User.Identity.Name
                })
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: /Evaluacion/5/Details
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluacion evaluacion = db.Evaluaciones.Find(id);
            if (evaluacion == null)
            {
                return HttpNotFound();
            }
            return View(evaluacion);
        }
        [AllowAnonymous]
        public ActionResult ParticipanteRespuestas(int evaluacionId, int participanteId)
        {

            Evaluacion evaluacion = new Evaluacion();
            evaluacion = db.Evaluaciones.Find(evaluacionId);

            List<Respuesta> respuestasTemp = new List<Respuesta>();
            List<Pregunta> preguntas = new List<Pregunta>();

            if (evaluacion == null)
            {
                return HttpNotFound();
            }

            Persona participante = db.Personas.Find(participanteId);
            if (participante == null || !evaluacion.Participantes.Contains(participante))
            {
                return HttpNotFound();
            }



            if (evaluacion.ModoEntradaEvaluacion == ModoEntradaEvaluacion.Nota_Final)
            {
                evaluacion.EvaluacionNotaRaw.Add(
                    new EvaluacionNotaRaw
                    {
                        Evaluacion = evaluacion,
                        EvaluacionId = evaluacion.Id,
                        Participante = participante,
                        ParticipanteId = participante.Id
                    }

                );

                ViewBag.Evaluacion = evaluacion;
                ViewBag.Participante = participante;
                return PartialView(
                   new Evaluacion
                   {
                       Id = evaluacion.Id,
                       ModoEntradaEvaluacion = Models.BO.ModoEntradaEvaluacion.Nota_Final,
                       EvaluacionNotaRaw = evaluacion.EvaluacionNotaRaw.Where(x => x.Participante.Id == participanteId).ToList()
                   });
            }
            else
            {
                preguntas = evaluacion.Preguntas.Where(e => e.EvaluacionId == evaluacionId).ToList();
                foreach (var pregunta in preguntas)
                {
                    int exist = db.Respuestas.Where(p => p.PreguntaId == pregunta.Id).Where(u => u.ParticipanteId == participante.Id).Count();
                    if (exist == 0)
                    {
                        evaluacion.Respuestas.Add(
                            new Respuesta
                            {
                                Evaluacion = evaluacion,
                                Fecha = DateTime.Now,
                                Participante = participante,
                                Pregunta = pregunta
                            });
                    }
                }
                
                ViewBag.Evaluacion = evaluacion;
                ViewBag.Participante = participante;
                ViewBag.preguntasViewBag = preguntas.ToList();
                //preguntas[0].Opciones.ToList();
                respuestasTemp = evaluacion.Respuestas.Where(x => x.Participante.Id == participanteId).ToList();

                return PartialView(
                    new Evaluacion
                    {
                        Id = evaluacion.Id,
                        ModoEntradaEvaluacion = Models.BO.ModoEntradaEvaluacion.Preguntas_Respuesta,                        
                        Respuestas = evaluacion.Respuestas.Where(x => x.Participante.Id == participanteId).ToList()                        
                    });
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult ParticipanteRespuestas(Evaluacion evaluacion)
        {
            string strNotaRawId = Request.Form["evalNotaRaw.Id"];
            string strParticipante = Request.Form["evalNotaRaw.ParticipanteId"];
            string strEvaluacionID = Request.Form["evalNotaRaw.EvaluacionId"];
            string strNotaEvaluacion = Request.Form["evalNotaRaw.nota"];


            if (evaluacion.ModoEntradaEvaluacion == ModoEntradaEvaluacion.Nota_Final)
            {
                int participanteID = Convert.ToInt32(strParticipante);
                int evaluacionID = Convert.ToInt32(strEvaluacionID);
                int notaEvaluacion = Convert.ToInt32(strNotaEvaluacion);
                int notaId = Convert.ToInt32(strNotaRawId);

                var nota = new EvaluacionNotaRaw
                {
                    Id = notaId,
                    EvaluacionId = evaluacion.Id,
                    ParticipanteId = participanteID,                    
                    nota = notaEvaluacion
                };
                int count = db.EvaluacionNotasRaw.Where(e => e.EvaluacionId == evaluacionID).Where(p => p.ParticipanteId == participanteID).Count();
                if (count == 0)
                {
                    db.EvaluacionNotasRaw.Add(nota);
                }
                else
                {
                    db.Entry(nota).State = EntityState.Modified;

                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    var dummy = e.Message;
                }

            }
            else
            {
                if (evaluacion.Respuestas != null)
                {
                    string respuestasEvalRequest = Request.Form["respuestas"];



                    foreach (var respuesta in evaluacion.Respuestas)
                    {
                        var resp = new Respuesta
                        {
                            EvaluacionId = respuesta.Evaluacion.Id,
                            ParticipanteId = respuesta.Participante.Id,
                            PreguntaId = respuesta.Pregunta.Id,
                            Id = respuesta.Id,
                            Digitador = User.Identity.Name,
                            Fecha = respuesta.Fecha,
                            Valor = respuesta.Valor
                        };
                        if (respuesta.Id == 0)
                        {
                            db.Respuestas.Add(resp);
                        }
                        else
                        {
                            db.Entry(resp).State = EntityState.Modified;
                        }
                    }

                    db.SaveChanges();
                }


            }

            return RedirectToAction("Details", new { id = evaluacion.Id });
        }

        // GET: Evaluaciones/Create
        [Route("Evaluaciones/Create")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Create()
        {
            if (User.IsInRole("Administrador") || User.IsInRole("Coordinador"))
            {
                ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");
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
                        ViewBag.CicloFormativoId = new SelectList(ciclosFormativos, "Id", "Tema");
                    }
                }
                catch (Exception exp)
                {
                    var msj = exp.Message;
                }
            }
            return View();
        }

        // POST: Evaluaciones/Create
        [Route("Evaluaciones/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Create(Evaluacion evaluacion)
        {
            if (ModelState.IsValid)
            {
                evaluacion.FormaEvaluacion = FormaEvaluacion.Presencial;
                evaluacion.TipoDeObjeto = TipoDeObjeto.Ciclo;
                evaluacion.creadoPor = User.Identity.Name;
                db.Evaluaciones.Add(evaluacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CicloFormativoList = new SelectList(db.CiclosFormativos, "Id", "Tema", evaluacion.CicloFormativoId);
            return View(evaluacion);
        }

        // GET: /Evaluacion/5/Edit
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluacion evaluacion = db.Evaluaciones.Find(id);
            if (evaluacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.CicloFormativoList = new SelectList(db.CiclosFormativos, "Id", "Tema", evaluacion.CicloFormativoId);
            return View(evaluacion);
        }

        // POST: /Evaluacion/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Edit(Evaluacion evaluacion)
        {
            if (ModelState.IsValid)
            {
                evaluacion.creadoPor = User.Identity.Name;
                evaluacion.FormaEvaluacion = FormaEvaluacion.Presencial;
                evaluacion.TipoDeObjeto = TipoDeObjeto.Ciclo;
                db.Entry(evaluacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CicloFormativoList = new SelectList(db.CiclosFormativos, "Id", "Tema", evaluacion.CicloFormativoId);
            return View(evaluacion);
        }

        // GET: /Evaluacion/5/Delete
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluacion evaluacion = db.Evaluaciones.Find(id);
            if (evaluacion == null)
            {
                return HttpNotFound();
            }
            return View(evaluacion);
        }

        // POST: /Evaluacion/5/Delete
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evaluacion evaluacion = db.Evaluaciones.Find(id);

            try {
                db.Evaluaciones.Remove(evaluacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception exp)
            {
                var msj = exp.Message;
                ModelState.AddModelError("568", "Favor borrar preguntas y respuestas antes de borrar la evaluacion!!");
                return View(evaluacion);
            }

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
