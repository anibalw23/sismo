using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models.BO.EvaluacionAcompanamiento;
using Monitoreo.Models.DAL;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class EvaluacionAcompanamientoRespuestaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: EvaluacionAcompanamientoRespuestas
        [Route("EvaluacionAcompanamientoRespuestas")]
        public ActionResult Index()
        {
            var evaluacionacompanamientorespuestas = db.EvaluacionAcompanamientoRespuestas.Include(e => e.EvaluacionAcomp).Include(e => e.InscripcionActividadAcompanamiento).Include(e => e.PreguntaAcomp);
            return View(evaluacionacompanamientorespuestas.ToList());
        }

        // POST: EvaluacionAcompanamientoRespuestas
        [Route("EvaluacionAcompanamientoRespuestas/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var evaluacionacompanamientorespuestas = db.EvaluacionAcompanamientoRespuestas.Include(e => e.EvaluacionAcomp).Include(e => e.InscripcionActividadAcompanamiento).Include(e => e.PreguntaAcomp);
            var recordsTotal = evaluacionacompanamientorespuestas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = evaluacionacompanamientorespuestas.Select(x => new { DT_RowId = x.Id, x.EvaluacionAcompId, x.InscripcionActividadAcompanamientoId, x.PreguntaAcompId, x.Valor });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Valor.Contains(searchValue)
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
                        case "Valor":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Valor);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Valor);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.EvaluacionAcompId);
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

        // GET: /EvaluacionAcompanamientoRespuesta/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamientoRespuesta evaluacionAcompanamientoRespuesta = db.EvaluacionAcompanamientoRespuestas.Find(id);
            if (evaluacionAcompanamientoRespuesta == null)
            {
                return HttpNotFound();
            }
            return View(evaluacionAcompanamientoRespuesta);
        }

        // GET: EvaluacionAcompanamientoRespuestas/Create
        [Route("EvaluacionAcompanamientoRespuestas/Create")]
        public ActionResult Create()
        {
            ViewBag.EvaluacionAcompId = new SelectList(db.EvaluacionAcompanamientoes, "Id", "Titulo");
            ViewBag.InscripcionActividadAcompanamientoId = new SelectList(db.InscripcionesActividadesAcompanamiento, "ID", "ID");
            ViewBag.ParticipanteId = new SelectList(db.Personas, "Id", "Cedula");
            ViewBag.PreguntaAcompId = new SelectList(db.EvaluacionAcompanamientoPreguntas, "Id", "Descripcion");
            return View();
        }

        // POST: EvaluacionAcompanamientoRespuestas/Create
        [Route("EvaluacionAcompanamientoRespuestas/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EvaluacionAcompId,InscripcionActividadAcompanamientoId,ParticipanteId,PreguntaAcompId,Valor,Fecha")] EvaluacionAcompanamientoRespuesta evaluacionAcompanamientoRespuesta)
        {
            if (ModelState.IsValid)
            {
                db.EvaluacionAcompanamientoRespuestas.Add(evaluacionAcompanamientoRespuesta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EvaluacionAcompId = new SelectList(db.EvaluacionAcompanamientoes, "Id", "Titulo", evaluacionAcompanamientoRespuesta.EvaluacionAcompId);
            ViewBag.InscripcionActividadAcompanamientoId = new SelectList(db.InscripcionesActividadesAcompanamiento, "ID", "ID", evaluacionAcompanamientoRespuesta.InscripcionActividadAcompanamientoId);
            ViewBag.PreguntaAcompId = new SelectList(db.EvaluacionAcompanamientoPreguntas, "Id", "Descripcion", evaluacionAcompanamientoRespuesta.PreguntaAcompId);
            return View(evaluacionAcompanamientoRespuesta);
        }

        // GET: /EvaluacionAcompanamientoRespuesta/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamientoRespuesta evaluacionAcompanamientoRespuesta = db.EvaluacionAcompanamientoRespuestas.Find(id);
            if (evaluacionAcompanamientoRespuesta == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvaluacionAcompId = new SelectList(db.EvaluacionAcompanamientoes, "Id", "Titulo", evaluacionAcompanamientoRespuesta.EvaluacionAcompId);
            ViewBag.InscripcionActividadAcompanamientoId = new SelectList(db.InscripcionesActividadesAcompanamiento, "ID", "ID", evaluacionAcompanamientoRespuesta.InscripcionActividadAcompanamientoId);
            ViewBag.PreguntaAcompId = new SelectList(db.EvaluacionAcompanamientoPreguntas, "Id", "Descripcion", evaluacionAcompanamientoRespuesta.PreguntaAcompId);
            return View(evaluacionAcompanamientoRespuesta);
        }

        // POST: /EvaluacionAcompanamientoRespuesta/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EvaluacionAcompId,InscripcionActividadAcompanamientoId,ParticipanteId,PreguntaAcompId,Valor,Fecha")] EvaluacionAcompanamientoRespuesta evaluacionAcompanamientoRespuesta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluacionAcompanamientoRespuesta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EvaluacionAcompId = new SelectList(db.EvaluacionAcompanamientoes, "Id", "Titulo", evaluacionAcompanamientoRespuesta.EvaluacionAcompId);
            ViewBag.InscripcionActividadAcompanamientoId = new SelectList(db.InscripcionesActividadesAcompanamiento, "ID", "ID", evaluacionAcompanamientoRespuesta.InscripcionActividadAcompanamientoId);
            ViewBag.PreguntaAcompId = new SelectList(db.EvaluacionAcompanamientoPreguntas, "Id", "Descripcion", evaluacionAcompanamientoRespuesta.PreguntaAcompId);
            return View(evaluacionAcompanamientoRespuesta);
        }

        // GET: /EvaluacionAcompanamientoRespuesta/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamientoRespuesta evaluacionAcompanamientoRespuesta = db.EvaluacionAcompanamientoRespuestas.Find(id);
            if (evaluacionAcompanamientoRespuesta == null)
            {
                return HttpNotFound();
            }
            return View(evaluacionAcompanamientoRespuesta);
        }

        // POST: /EvaluacionAcompanamientoRespuesta/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvaluacionAcompanamientoRespuesta evaluacionAcompanamientoRespuesta = db.EvaluacionAcompanamientoRespuestas.Find(id);


            //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
            //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

            if (ModelState.IsValid)
            {
                db.EvaluacionAcompanamientoRespuestas.Remove(evaluacionAcompanamientoRespuesta);
                db.SaveChanges();
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(evaluacionAcompanamientoRespuesta);
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
