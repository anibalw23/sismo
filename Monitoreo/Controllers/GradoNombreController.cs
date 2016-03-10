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
    public class GradoCentroController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: GradoCentros
        [Route("GradoCentros")]
        public ActionResult Index()
        {
            var gradocentroes = db.GradoCentroes.Include(g => g.ciclo).Include(g => g.nivel);
            return View(gradocentroes.ToList());
        }

        // POST: GradoCentros
        [Route("GradoCentros/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var gradocentroes = db.GradoCentroes.Include(g => g.ciclo).Include(g => g.nivel);
            var recordsTotal = gradocentroes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = gradocentroes.Select(x => new { DT_RowId = x.ID, nivel = x.nivel.nivel, ciclo = x.ciclo.ciclo, x.grado });

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
                data = data.OrderBy(s => s.nivel);
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

        // GET: /GradoCentro/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoCentro gradoCentro = db.GradoCentroes.Find(id);
            if (gradoCentro == null)
            {
                return HttpNotFound();
            }
            return View(gradoCentro);
        }

        // GET: GradoCentros/Create
        [Route("GradoCentros/Create")]
        public ActionResult Create()
        {
            ViewBag.cicloId = new SelectList(db.CicloGradoes, "ID", "ciclo");
            ViewBag.nivelId = new SelectList(db.NivelGradoes, "ID", "nivel");
            return View();
        }

        // POST: GradoCentros/Create
        [Route("GradoCentros/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,nivelId,cicloId,grado")] GradoCentro gradoCentro)
        {
            if (ModelState.IsValid)
            {
                db.GradoCentroes.Add(gradoCentro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cicloId = new SelectList(db.CicloGradoes, "ID", "ciclo", gradoCentro.cicloId);
            ViewBag.nivelId = new SelectList(db.NivelGradoes, "ID", "nivel", gradoCentro.nivelId);
            return View(gradoCentro);
        }

        // GET: /GradoCentro/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoCentro gradoCentro = db.GradoCentroes.Find(id);
            if (gradoCentro == null)
            {
                return HttpNotFound();
            }
            ViewBag.cicloId = new SelectList(db.CicloGradoes, "ID", "ciclo", gradoCentro.cicloId);
            ViewBag.nivelId = new SelectList(db.NivelGradoes, "ID", "nivel", gradoCentro.nivelId);
            return View(gradoCentro);
        }

        // POST: /GradoCentro/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,nivelId,cicloId,grado")] GradoCentro gradoCentro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gradoCentro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cicloId = new SelectList(db.CicloGradoes, "ID", "ciclo", gradoCentro.cicloId);
            ViewBag.nivelId = new SelectList(db.NivelGradoes, "ID", "nivel", gradoCentro.nivelId);
            return View(gradoCentro);
        }

        // GET: /GradoCentro/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoCentro gradoCentro = db.GradoCentroes.Find(id);
            if (gradoCentro == null)
            {
                return HttpNotFound();
            }
            return View(gradoCentro);
        }

        // POST: /GradoCentro/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GradoCentro gradoCentro = db.GradoCentroes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.GradoCentroes.Remove(gradoCentro);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(gradoCentro);
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
