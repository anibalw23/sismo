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
    //[Authorize(Roles = "Administrador")]
    public class ComponenteController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Componentes
        [Route("Componentes")]
        public ActionResult Index()
        {
            return View(db.Componentes.ToList());
        }

        // POST: Componentes
        [Route("Componentes/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var componentes = db.Componentes;
            var recordsTotal = componentes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = componentes.Select(x => new { DT_RowId = x.Id, x.Descripcion });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Descripcion.Contains(searchValue)
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

        // GET: /Componente/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Componente componente = db.Componentes.Find(id);
            if (componente == null)
            {
                return HttpNotFound();
            }
            return View(componente);
        }

        // GET: Componentes/Create
        [Route("Componentes/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Componentes/Create
        [Route("Componentes/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Descripcion")] Componente componente)
        {
            if (ModelState.IsValid)
            {
                db.Componentes.Add(componente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(componente);
        }

        // GET: /Componente/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Componente componente = db.Componentes.Find(id);
            if (componente == null)
            {
                return HttpNotFound();
            }
            return View(componente);
        }

        // POST: /Componente/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Descripcion")] Componente componente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(componente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(componente);
        }

        // GET: /Componente/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Componente componente = db.Componentes.Find(id);
            if (componente == null)
            {
                return HttpNotFound();
            }
            return View(componente);
        }

        // POST: /Componente/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Componente componente = db.Componentes.Find(id);
            db.Componentes.Remove(componente);
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
