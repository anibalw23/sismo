using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models;
using Monitoreo.Models.DAL;

namespace Monitoreo.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CicloGradoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: CicloGrados
        [Route("CicloGrado")]
        public ActionResult Index()
        {
            return View(db.CicloGradoes.ToList());
        }

        // POST: CicloGrados
        [Route("CicloGrado/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var ciclogradoes = db.CicloGradoes;
            var recordsTotal = ciclogradoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = ciclogradoes.Select(x => new { DT_RowId = x.ID, x.ciclo });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.ciclo.Contains(searchValue)
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
                        case "ciclo":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.ciclo);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.ciclo);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.ciclo);
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

        // GET: /CicloGrado/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CicloGrado cicloGrado = db.CicloGradoes.Find(id);
            if (cicloGrado == null)
            {
                return HttpNotFound();
            }
            return View(cicloGrado);
        }

        // GET: CicloGrados/Create
        [Route("CicloGrado/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: CicloGrados/Create
        [Route("CicloGrado/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,ciclo")] CicloGrado cicloGrado)
        {
            if (ModelState.IsValid)
            {
                db.CicloGradoes.Add(cicloGrado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cicloGrado);
        }

        // GET: /CicloGrado/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CicloGrado cicloGrado = db.CicloGradoes.Find(id);
            if (cicloGrado == null)
            {
                return HttpNotFound();
            }
            return View(cicloGrado);
        }

        // POST: /CicloGrado/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,ciclo")] CicloGrado cicloGrado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cicloGrado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cicloGrado);
        }

        // GET: /CicloGrado/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CicloGrado cicloGrado = db.CicloGradoes.Find(id);
            if (cicloGrado == null)
            {
                return HttpNotFound();
            }
            return View(cicloGrado);
        }

        // POST: /CicloGrado/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CicloGrado cicloGrado = db.CicloGradoes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.CicloGradoes.Remove(cicloGrado);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(cicloGrado);
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
