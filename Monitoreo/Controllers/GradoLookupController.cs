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
    public class GradoLookupController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: GradoLookups
        [Route("GradoLookups")]
        public ActionResult Index()
        {
            var gradolookups = db.GradoLookups.Include(g => g.ciclo).Include(g => g.nivel);
            return View(gradolookups.ToList());
        }

        // POST: GradoLookups
        [Route("GradoLookups/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var gradolookups = db.GradoLookups.Include(g => g.ciclo).Include(g => g.nivel);
            var recordsTotal = gradolookups.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = gradolookups.Select(x => new { DT_RowId = x.Id, nivel = x.nivel.nivel,ciclo = x.ciclo.ciclo, grado = x.grado });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.grado.Contains(searchValue)
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
                        case "grado":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.grado);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.grado);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.grado);
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

        // GET: /GradoLookup/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoLookup gradoLookup = db.GradoLookups.Find(id);
            if (gradoLookup == null)
            {
                return HttpNotFound();
            }
            return View(gradoLookup);
        }

        // GET: GradoLookups/Create
        [Route("GradoLookups/Create")]
        public ActionResult Create()
        {
            ViewBag.cicloId = new SelectList(db.CicloGradoes, "ID", "ciclo");
            ViewBag.nivelId = new SelectList(db.NivelGradoes, "ID", "nivel");
            return View();
        }

        // POST: GradoLookups/Create
        [Route("GradoLookups/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,nivelId,cicloId,grado")] GradoLookup gradoLookup)
        {
            if (ModelState.IsValid)
            {
                db.GradoLookups.Add(gradoLookup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cicloId = new SelectList(db.CicloGradoes, "ID", "ciclo", gradoLookup.cicloId);
            ViewBag.nivelId = new SelectList(db.NivelGradoes, "ID", "nivel", gradoLookup.nivelId);
            return View(gradoLookup);
        }

        // GET: /GradoLookup/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoLookup gradoLookup = db.GradoLookups.Find(id);
            if (gradoLookup == null)
            {
                return HttpNotFound();
            }
            ViewBag.cicloId = new SelectList(db.CicloGradoes, "ID", "ciclo", gradoLookup.cicloId);
            ViewBag.nivelId = new SelectList(db.NivelGradoes, "ID", "nivel", gradoLookup.nivelId);
            return View(gradoLookup);
        }

        // POST: /GradoLookup/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,nivelId,cicloId,grado")] GradoLookup gradoLookup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gradoLookup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cicloId = new SelectList(db.CicloGradoes, "ID", "ciclo", gradoLookup.cicloId);
            ViewBag.nivelId = new SelectList(db.NivelGradoes, "ID", "nivel", gradoLookup.nivelId);
            return View(gradoLookup);
        }

        // GET: /GradoLookup/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoLookup gradoLookup = db.GradoLookups.Find(id);
            if (gradoLookup == null)
            {
                return HttpNotFound();
            }
            return View(gradoLookup);
        }

        // POST: /GradoLookup/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GradoLookup gradoLookup = db.GradoLookups.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.GradoLookups.Remove(gradoLookup);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(gradoLookup);
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
