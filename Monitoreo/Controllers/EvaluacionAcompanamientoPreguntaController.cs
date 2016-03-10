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
    public class EvaluacionAcompanamientoPreguntaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: EvaluacionAcompanamientoPreguntas
        [Route("EvaluacionAcompanamientoPregunta")]
        public ActionResult Index()
        {
            var evaluacionacompanamientopreguntas = db.EvaluacionAcompanamientoPreguntas.Include(e => e.EvaluacionAcomp);
            return View(evaluacionacompanamientopreguntas.ToList());
        }

        // POST: EvaluacionAcompanamientoPreguntas
        [Route("EvaluacionAcompanamientoPregunta/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var evaluacionacompanamientopreguntas = db.EvaluacionAcompanamientoPreguntas.Include(e => e.EvaluacionAcomp);
            var recordsTotal = evaluacionacompanamientopreguntas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = evaluacionacompanamientopreguntas.Select(x => new { DT_RowId = x.Id, x.Descripcion, x.codigo, x.EvaluacionAcompId });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Descripcion.Contains(searchValue) || x.codigo.Contains(searchValue)
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
                        case "Descripcion":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Descripcion);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Descripcion);
                            }
                            sorting = true;
                            break;
                        case "codigo":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.codigo);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.codigo);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Descripcion);
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

        // GET: /EvaluacionAcompanamientoPregunta/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamientoPregunta evaluacionAcompanamientoPregunta = db.EvaluacionAcompanamientoPreguntas.Find(id);
            if (evaluacionAcompanamientoPregunta == null)
            {
                return HttpNotFound();
            }
            return View(evaluacionAcompanamientoPregunta);
        }

        // GET: EvaluacionAcompanamientoPreguntas/Create
        [Route("EvaluacionAcompanamientoPregunta/Create")]
        public ActionResult Create()
        {
            ViewBag.EvaluacionAcompId = new SelectList(db.EvaluacionAcompanamientoes, "Id", "Titulo");
            return View();
        }

        // POST: EvaluacionAcompanamientoPreguntas/Create
        [Route("EvaluacionAcompanamientoPregunta/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Descripcion,codigo,EvaluacionAcompId")] EvaluacionAcompanamientoPregunta evaluacionAcompanamientoPregunta)
        {
            if (ModelState.IsValid)
            {
                db.EvaluacionAcompanamientoPreguntas.Add(evaluacionAcompanamientoPregunta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EvaluacionAcompId = new SelectList(db.EvaluacionAcompanamientoes, "Id", "Titulo", evaluacionAcompanamientoPregunta.EvaluacionAcompId);
            return View(evaluacionAcompanamientoPregunta);
        }



        public ActionResult CreateByEvaluacion(int evaluacionId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateByEvaluacion(EvaluacionAcompanamientoPregunta pregunta, int evaluacionId)
        {
            EvaluacionAcompanamiento evaluacion = new EvaluacionAcompanamiento();
            if (ModelState.IsValid)
            {
                pregunta.EvaluacionAcompId = evaluacionId;
                evaluacion = db.EvaluacionAcompanamientoes.Find(evaluacionId);
                db.EvaluacionAcompanamientoPreguntas.Add(pregunta);
                foreach (PreguntaOpcionAcompanamiento opcion in pregunta.Opciones)
                {
                    db.PreguntaOpcionAcompanamientoes.Add(opcion);
                }
                db.SaveChanges();

                return RedirectToAction("Details", "EvaluacionAcompanamiento", new { id = evaluacion.Id });
            }

            //ViewBag.EvaluacionId = new SelectList(db.Evaluaciones, "Id", "Titulo");
            return View(pregunta);
        }







        // GET: /EvaluacionAcompanamientoPregunta/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamientoPregunta evaluacionAcompanamientoPregunta = db.EvaluacionAcompanamientoPreguntas.Find(id);
            if (evaluacionAcompanamientoPregunta == null)
            {
                return HttpNotFound();
            }
            ViewBag.EvaluacionAcompId = new SelectList(db.EvaluacionAcompanamientoes, "Id", "Titulo", evaluacionAcompanamientoPregunta.EvaluacionAcompId);
            return View(evaluacionAcompanamientoPregunta);
        }

        // POST: /EvaluacionAcompanamientoPregunta/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Descripcion,codigo,EvaluacionAcompId")] EvaluacionAcompanamientoPregunta evaluacionAcompanamientoPregunta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluacionAcompanamientoPregunta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EvaluacionAcompId = new SelectList(db.EvaluacionAcompanamientoes, "Id", "Titulo", evaluacionAcompanamientoPregunta.EvaluacionAcompId);
            return View(evaluacionAcompanamientoPregunta);
        }





        // GET: /Pregunta/5/Edit
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public ActionResult EditByEvaluacion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamientoPregunta pregunta = db.EvaluacionAcompanamientoPreguntas.Find(id);
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
        public ActionResult EditByEvaluacion(EvaluacionAcompanamientoPregunta pregunta)
        {
            
            if (ModelState.IsValid)
            {
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
                return RedirectToAction("Details", "EvaluacionAcompanamiento", new { id = pregunta.EvaluacionAcompId });
            }

            return View(pregunta);
        }
















        // GET: /EvaluacionAcompanamientoPregunta/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamientoPregunta evaluacionAcompanamientoPregunta = db.EvaluacionAcompanamientoPreguntas.Find(id);
            if (evaluacionAcompanamientoPregunta == null)
            {
                return HttpNotFound();
            }
            return View(evaluacionAcompanamientoPregunta);
        }

        // POST: /EvaluacionAcompanamientoPregunta/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvaluacionAcompanamientoPregunta evaluacionAcompanamientoPregunta = db.EvaluacionAcompanamientoPreguntas.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.EvaluacionAcompanamientoPreguntas.Remove(evaluacionAcompanamientoPregunta);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(evaluacionAcompanamientoPregunta);
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
