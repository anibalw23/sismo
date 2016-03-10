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
    [Authorize(Roles = "Administrador")]
    public class PeriodosEscolaresController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: PeriodosEscolaress
        [Route("PeriodoEscolar")]
        public ActionResult Index()
        {
            return View(db.PeriodoEscolars.ToList());
        }

        // POST: PeriodosEscolaress
        [Route("PeriodoEscolar/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var periodoescolars = db.PeriodoEscolars;
            var recordsTotal = periodoescolars.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = periodoescolars.Select(x => new { DT_RowId = x.ID, x.nombre, fechaInicio = x.fechaInicio.ToString(), fechaFin = x.fechaFin.ToString() });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.nombre.Contains(searchValue)
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
                        case "nombre":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.nombre);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.nombre);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.nombre);
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

        // GET: /PeriodosEscolares/5/Details
        [Route("PeriodoEscolar/Delete")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodoEscolar periodoEscolar = db.PeriodoEscolars.Find(id);
            if (periodoEscolar == null)
            {
                return HttpNotFound();
            }
            return View(periodoEscolar);
        }

        // GET: PeriodosEscolaress/Create
        [Route("PeriodoEscolar/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PeriodosEscolaress/Create
        [Route("PeriodoEscolar/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,nombre,fechaInicio,fechaFin")] PeriodoEscolar periodoEscolar)
        {
            if (ModelState.IsValid)
            {
                db.PeriodoEscolars.Add(periodoEscolar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(periodoEscolar);
        }

        // GET: /PeriodosEscolares/5/Edit
        [Route("PeriodoEscolar/Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodoEscolar periodoEscolar = db.PeriodoEscolars.Find(id);
            if (periodoEscolar == null)
            {
                return HttpNotFound();
            }
            return View(periodoEscolar);
        }

        // POST: /PeriodosEscolares/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("PeriodoEscolar/Edit")]
        public ActionResult Edit([Bind(Include="ID,nombre,fechaInicio,fechaFin")] PeriodoEscolar periodoEscolar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(periodoEscolar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(periodoEscolar);
        }

        // GET: /PeriodosEscolares/5/Delete
        [Route("PeriodoEscolar/Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodoEscolar periodoEscolar = db.PeriodoEscolars.Find(id);
            if (periodoEscolar == null)
            {
                return HttpNotFound();
            }
            return View(periodoEscolar);
        }

        // POST: /PeriodosEscolares/5/Delete
        [Route("PeriodoEscolar/Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PeriodoEscolar periodoEscolar = db.PeriodoEscolars.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.PeriodoEscolars.Remove(periodoEscolar);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(periodoEscolar);
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
