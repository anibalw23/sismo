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

namespace Monitoreo.Controllers
{
    [Authorize]
    public class PreguntaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Preguntas
        [Route("Preguntas")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Index()
        {
            return View(db.Preguntas.ToList());
        }

        // GET: /Pregunta/5/Details
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta pregunta = db.Preguntas.Find(id);
            if (pregunta == null)
            {
                return HttpNotFound();
            }
            return View(pregunta);
        }

        // GET: Preguntas/Create
        [Route("Preguntas/Create")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Create()
        {
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");
            ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Titulo");

            return View();
        }

        // POST: Preguntas/Create
        [Route("Preguntas/Create")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pregunta pregunta)
        {
            if (ModelState.IsValid)
            {
                db.Preguntas.Add(pregunta);

                foreach (PreguntaOpcion opcion in pregunta.Opciones)
                {
                    db.PreguntasOpciones.Add(opcion);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", pregunta.CicloFormativoId);
            ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Titulo");
            return View(pregunta);
        }





        // GET: Preguntas/Create
        [Route("Preguntas/CreateByEvaluacion")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult CreateByEvaluacion()
        {
            //ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");
            //ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Titulo");

            return View();
        }

        // POST: Preguntas/Create
        [Route("Preguntas/CreateByEvaluacion")]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateByEvaluacion(Pregunta pregunta, int evaluacionId)
        {
            Evaluacion evaluacion = new Evaluacion();
            if (ModelState.IsValid)
            {
                pregunta.NivelDominio = NivelDominio.Contenido;
                pregunta.EvaluacionId = evaluacionId;
                evaluacion = db.Evaluaciones.Find(evaluacionId);
               // pregunta.TipoEvaluacion = evaluacion.TipoEvaluacion;
                pregunta.CicloFormativoId = evaluacion.CicloFormativoId;
                db.Preguntas.Add(pregunta);
                foreach (PreguntaOpcion opcion in pregunta.Opciones)
                {
                    db.PreguntasOpciones.Add(opcion);
                }
                db.SaveChanges();

                return RedirectToAction("Details", "Evaluacion", new { id = evaluacion.Id });
            }

            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", pregunta.CicloFormativoId);
            ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Titulo");
            return View(pregunta);
        }




        // GET: /Pregunta/5/Edit
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta pregunta = db.Preguntas.Find(id);
            if (pregunta == null)
            {
                return HttpNotFound();
            }

            ViewBag.CicloFormativo = new SelectList(db.CiclosFormativos, "Id", "Tema", pregunta.CicloFormativoId);
            return View(pregunta);
        }

        // POST: /Pregunta/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult Edit(Pregunta pregunta)
        {
            if (ModelState.IsValid)
            {
                pregunta.NivelDominio = NivelDominio.Contenido;
                foreach (var item in pregunta.Opciones.ToArray())
                {
                    if (item.Id < 0)
                    {
                        item.Id = item.Id * -1;
                        pregunta.Opciones.Remove(item);
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
                db.Entry(pregunta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CicloFormativo = new SelectList(db.CiclosFormativos, "Id", "Tema", pregunta.CicloFormativoId);
            return View(pregunta);
        }








        // GET: /Pregunta/5/Edit
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult EditByEvaluacion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta pregunta = db.Preguntas.Find(id);
            if (pregunta == null)
            {
                return HttpNotFound();
            }
            return View(pregunta);
        }

        // POST: /Pregunta/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult EditByEvaluacion(Pregunta pregunta)
        {
            if (ModelState.IsValid)
            {
                pregunta.NivelDominio = NivelDominio.Contenido;
                foreach (var item in pregunta.Opciones.ToArray())
                {
                    if (item.Id < 0)
                    {
                        item.Id = item.Id * -1;
                        pregunta.Opciones.Remove(item);
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

                db.Entry(pregunta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Evaluacion", new { id = pregunta.EvaluacionId });
            }

            return View(pregunta);
        }





        // GET: /Pregunta/5/Delete
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta pregunta = db.Preguntas.Find(id);
            if (pregunta == null)
            {
                return HttpNotFound();
            }

            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", pregunta.CicloFormativoId);
            return View(pregunta);
        }

        // POST: /Pregunta/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult DeleteConfirmed(int id)
        {
            Pregunta pregunta = db.Preguntas.Find(id);
            var evaluacionId = pregunta.EvaluacionId;

            db.Preguntas.Remove(pregunta);
            db.SaveChanges();
            return RedirectToAction("Details", "Evaluacion", new { id = evaluacionId });
          //  return RedirectToAction("Index");
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
