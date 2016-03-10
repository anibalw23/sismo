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
    [Authorize(Roles = "Administrador")]
    public class PreguntaOpcionAcompanamientoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: PreguntaOpcionAcompanamientos
        [Route("PreguntaOpcionAcompanamientos")]
        public ActionResult Index()
        {
            return View(db.PreguntaOpcionAcompanamientoes.ToList());
        }

        // POST: PreguntaOpcionAcompanamientos
        [Route("PreguntaOpcionAcompanamientos/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var preguntaopcionacompanamientoes = db.PreguntaOpcionAcompanamientoes;
            var recordsTotal = preguntaopcionacompanamientoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = preguntaopcionacompanamientoes.Select(x => new { DT_RowId = x.Id, x.EvalAcompPreguntaId, x.Titulo, x.Valor, x.Correcta });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Titulo.Contains(searchValue) || x.Valor.Contains(searchValue)
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
                        case "Titulo":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Titulo);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Titulo);
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
                data = data.OrderBy(s => s.EvalAcompPreguntaId);
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

        // GET: /PreguntaOpcionAcompanamiento/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreguntaOpcionAcompanamiento preguntaOpcionAcompanamiento = db.PreguntaOpcionAcompanamientoes.Find(id);
            if (preguntaOpcionAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(preguntaOpcionAcompanamiento);
        }

        // GET: PreguntaOpcionAcompanamientos/Create
        [Route("PreguntaOpcionAcompanamientos/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PreguntaOpcionAcompanamientos/Create
        [Route("PreguntaOpcionAcompanamientos/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,EvalAcompPreguntaId,Titulo,Valor,Correcta")] PreguntaOpcionAcompanamiento preguntaOpcionAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                db.PreguntaOpcionAcompanamientoes.Add(preguntaOpcionAcompanamiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(preguntaOpcionAcompanamiento);
        }

        // GET: /PreguntaOpcionAcompanamiento/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreguntaOpcionAcompanamiento preguntaOpcionAcompanamiento = db.PreguntaOpcionAcompanamientoes.Find(id);
            if (preguntaOpcionAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(preguntaOpcionAcompanamiento);
        }

        // POST: /PreguntaOpcionAcompanamiento/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,EvalAcompPreguntaId,Titulo,Valor,Correcta")] PreguntaOpcionAcompanamiento preguntaOpcionAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(preguntaOpcionAcompanamiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(preguntaOpcionAcompanamiento);
        }

        // GET: /PreguntaOpcionAcompanamiento/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PreguntaOpcionAcompanamiento preguntaOpcionAcompanamiento = db.PreguntaOpcionAcompanamientoes.Find(id);
            if (preguntaOpcionAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(preguntaOpcionAcompanamiento);
        }

        // POST: /PreguntaOpcionAcompanamiento/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PreguntaOpcionAcompanamiento preguntaOpcionAcompanamiento = db.PreguntaOpcionAcompanamientoes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.PreguntaOpcionAcompanamientoes.Remove(preguntaOpcionAcompanamiento);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(preguntaOpcionAcompanamiento);
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
