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
    [Authorize]
    public class SeccionController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Secciones
        [Route("Secciones")]
        public ActionResult Index()
        {
            return View(db.Secciones.ToList());
        }

        // POST: Secciones
        [Route("Secciones/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var secciones = db.Secciones;
            var recordsTotal = secciones.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = secciones.Select(x => new { DT_RowId = x.Id, x.Numero });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Numero.Contains(searchValue)
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
                        case "Numero":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Numero);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Numero);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Numero);
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

        // GET: /Seccion/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seccion seccion = db.Secciones.Find(id);
            if (seccion == null)
            {
                return HttpNotFound();
            }
            return View(seccion);
        }

        // GET: Secciones/Create
        [Route("Secciones/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Secciones/Create
        [Route("Secciones/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Numero")] Seccion seccion)
        {
            if (ModelState.IsValid)
            {
                db.Secciones.Add(seccion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(seccion);
        }

        // GET: /Seccion/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seccion seccion = db.Secciones.Find(id);
            if (seccion == null)
            {
                return HttpNotFound();
            }
            return View(seccion);
        }

        // POST: /Seccion/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Numero")] Seccion seccion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seccion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(seccion);
        }

        // GET: /Seccion/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seccion seccion = db.Secciones.Find(id);
            if (seccion == null)
            {
                return HttpNotFound();
            }
            return View(seccion);
        }

        // POST: /Seccion/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Seccion seccion = db.Secciones.Find(id);

            // Seteando en null lo relacionada
            var materias = db.DocenteMaterias.Where(x => x.SeccionId == id);
            foreach (var item in materias)
            {
                item.SeccionId = null;
                db.Entry(item).State = EntityState.Modified;
            }

            var estudiantes = db.Estudiantes.Where(x => x.SeccionId == id);
            foreach (var item in estudiantes)
            {
                item.SeccionId = null;
                db.Entry(item).State = EntityState.Modified;
            }

            db.Secciones.Remove(seccion);
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
