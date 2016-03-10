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
    public class IndicadorDesagregacionController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: IndicadorDesagregacions
        [Route("IndicadorDesagregacion")]
        public ActionResult Index()
        {
            return View(db.IndicadorDesagregacions.ToList());
        }

        // POST: IndicadorDesagregacions
        [Route("IndicadorDesagregacion/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var indicadordesagregacions = db.IndicadorDesagregacions;
            var recordsTotal = indicadordesagregacions.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = indicadordesagregacions.Select(x => new { DT_RowId = x.Id, x.nombre });

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

        // GET: /IndicadorDesagregacion/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorDesagregacion indicadorDesagregacion = db.IndicadorDesagregacions.Find(id);
            if (indicadorDesagregacion == null)
            {
                return HttpNotFound();
            }
            return View(indicadorDesagregacion);
        }

        // GET: IndicadorDesagregacions/Create
        [Route("IndicadorDesagregacion/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: IndicadorDesagregacions/Create
        [Route("IndicadorDesagregacion/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,nombre")] IndicadorDesagregacion indicadorDesagregacion)
        {
            if (ModelState.IsValid)
            {
                db.IndicadorDesagregacions.Add(indicadorDesagregacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(indicadorDesagregacion);
        }

        // GET: /IndicadorDesagregacion/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorDesagregacion indicadorDesagregacion = db.IndicadorDesagregacions.Find(id);
            if (indicadorDesagregacion == null)
            {
                return HttpNotFound();
            }
            return View(indicadorDesagregacion);
        }

        // POST: /IndicadorDesagregacion/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,nombre")] IndicadorDesagregacion indicadorDesagregacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicadorDesagregacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(indicadorDesagregacion);
        }

        // GET: /IndicadorDesagregacion/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorDesagregacion indicadorDesagregacion = db.IndicadorDesagregacions.Find(id);
            if (indicadorDesagregacion == null)
            {
                return HttpNotFound();
            }
            return View(indicadorDesagregacion);
        }

        // POST: /IndicadorDesagregacion/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IndicadorDesagregacion indicadorDesagregacion = db.IndicadorDesagregacions.Find(id);
            db.IndicadorDesagregacions.Remove(indicadorDesagregacion);
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
