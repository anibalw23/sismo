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
    public class RespuestaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Respuestas
        [Route("Respuestas")]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Index()
        {
            var respuestas = db.Respuestas.Include(r => r.Evaluacion).Include(r => r.Participante).Include(r => r.Pregunta);
            return View(respuestas.ToList());
        }

        // POST: Respuestas
        [Route("Respuestas/GetDataJson")]
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante")]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var respuestas = db.Respuestas.Include(r => r.Evaluacion).Include(r => r.Participante).Include(r => r.Pregunta);
            var recordsTotal = respuestas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = respuestas.Select(x => new { DT_RowId = x.Id, x.Fecha, x.Digitador, x.EvaluacionId, x.PreguntaId, x.ParticipanteId, x.Valor });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Digitador.Contains(searchValue) || x.Valor.Contains(searchValue)
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
                        case "Digitador":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Digitador);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Digitador);
                            }
                            sorting = true;
                            break;
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
                data = data.OrderBy(s => s.Fecha);
            }

            // Preparando respuesta y ejecutando consulta
            var jsonData = new {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.Skip(from).Take(limit).ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: /Respuesta/5/Details
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Respuesta respuesta = db.Respuestas.Find(id);
            if (respuesta == null)
            {
                return HttpNotFound();
            }
            return View(respuesta);
        }

        // GET: Respuestas/Create
        [Route("Respuestas/Create")]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Create()
        {
            ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Id");
            ViewBag.ParticipanteId = new SelectList(db.Personas, "Id", "Cedula");
            ViewBag.PreguntaId = new SelectList(db.Preguntas, "Id", "Descripcion");
            return View();
        }

        // POST: Respuestas/Create
        [Route("Respuestas/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Create([Bind(Include="Id,Fecha,Digitador,EvaluacionId,PreguntaId,ParticipanteId,Valor")] Respuesta respuesta)
        {
            if (ModelState.IsValid)
            {
                db.Respuestas.Add(respuesta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Id", respuesta.EvaluacionId);
            ViewBag.ParticipanteId = new SelectList(db.Personas, "Id", "Cedula", respuesta.ParticipanteId);
            ViewBag.PreguntaId = new SelectList(db.Preguntas, "Id", "Descripcion", respuesta.PreguntaId);
            return View(respuesta);
        }

        // GET: /Respuesta/5/Edit
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Respuesta respuesta = db.Respuestas.Find(id);
            if (respuesta == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Id", respuesta.EvaluacionId);
            ViewBag.ParticipanteId = new SelectList(db.Personas, "Id", "Cedula", respuesta.ParticipanteId);
            ViewBag.PreguntaId = new SelectList(db.Preguntas, "Id", "Descripcion", respuesta.PreguntaId);
            return View(respuesta);
        }

        // POST: /Respuesta/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Edit([Bind(Include="Id,Fecha,Digitador,EvaluacionId,PreguntaId,ParticipanteId,Valor")] Respuesta respuesta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(respuesta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Id", respuesta.EvaluacionId);
            ViewBag.ParticipanteId = new SelectList(db.Personas, "Id", "Cedula", respuesta.ParticipanteId);
            ViewBag.PreguntaId = new SelectList(db.Preguntas, "Id", "Descripcion", respuesta.PreguntaId);
            return View(respuesta);
        }

        // GET: /Respuesta/5/Delete
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Respuesta respuesta = db.Respuestas.Find(id);
            if (respuesta == null)
            {
                return HttpNotFound();
            }
            return View(respuesta);
        }

        // POST: /Respuesta/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult DeleteConfirmed(int id)
        {
            Respuesta respuesta = db.Respuestas.Find(id);
            db.Respuestas.Remove(respuesta);
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
