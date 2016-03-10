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
    public class NivelGradoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: NivelGrados
        [Route("NivelGrado")]
        public ActionResult Index()
        {
            return View(db.NivelGradoes.ToList());
        }

        // POST: NivelGrados
        [Route("NivelGrado/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var nivelgradoes = db.NivelGradoes;
            var recordsTotal = nivelgradoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = nivelgradoes.Select(x => new { DT_RowId = x.ID, x.nivel });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.nivel.Contains(searchValue)
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
                        case "nivel":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.nivel);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.nivel);
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

        // GET: /NivelGrado/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NivelGrado nivelGrado = db.NivelGradoes.Find(id);
            if (nivelGrado == null)
            {
                return HttpNotFound();
            }
            return View(nivelGrado);
        }

        // GET: NivelGrados/Create
        [Route("NivelGrado/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: NivelGrados/Create
        [Route("NivelGrado/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,nivel")] NivelGrado nivelGrado)
        {
            if (ModelState.IsValid)
            {
                db.NivelGradoes.Add(nivelGrado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nivelGrado);
        }

        // GET: /NivelGrado/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NivelGrado nivelGrado = db.NivelGradoes.Find(id);
            if (nivelGrado == null)
            {
                return HttpNotFound();
            }
            return View(nivelGrado);
        }

        // POST: /NivelGrado/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,nivel")] NivelGrado nivelGrado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nivelGrado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nivelGrado);
        }

        // GET: /NivelGrado/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NivelGrado nivelGrado = db.NivelGradoes.Find(id);
            if (nivelGrado == null)
            {
                return HttpNotFound();
            }
            return View(nivelGrado);
        }

        // POST: /NivelGrado/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NivelGrado nivelGrado = db.NivelGradoes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.NivelGradoes.Remove(nivelGrado);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(nivelGrado);
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
