using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models.BO.PlanMejora;
using Monitoreo.Models.DAL;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class ActividadController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        [HttpPost]
        public JsonResult GetGrandesActividadesByObjetivo(int objetivoId)
        {

            List<Actividad> actividades = new List<Actividad>();
            ViewBag.ObjetivoID = objetivoId;

            try
            {
                actividades = db.Actividads.Where(o => o.ObjetivoId == objetivoId).ToList();

            }
            catch (Exception e)
            {
                var msj = e.Message;
            }

            var jsonData = new
            {
                data = actividades.Select(y => new
                {
                    id = y.ID,
                    nombre = y.nombre,
                    fechaInicio = y.fechaInicio.ToString(),
                    fechaFin = y.fechaFin.ToString()
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }










        // GET: Actividads
        [Route("Actividad")]
        public ActionResult Index()
        {
            var actividads = db.Actividads.Include(a => a.Objetivo);
            return View(actividads.ToList());
        }

        // POST: Actividads
        [Route("Actividad/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var actividads = db.Actividads.Include(a => a.Objetivo);
            var recordsTotal = actividads.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = actividads.Select(x => new { DT_RowId = x.ID, x.nombre, fechaInicio = x.fechaInicio.ToString(), fechaFin = x.fechaFin.ToString(), x.ObjetivoId });

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

        // GET: /Actividad/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actividad actividad = db.Actividads.Find(id);
            if (actividad == null)
            {
                return HttpNotFound();
            }
            return View(actividad);
        }

        // GET: Actividads/Create
        [Route("Actividad/Create")]
        public ActionResult Create()
        {
            ViewBag.ObjetivoId = new SelectList(db.Objetivoes, "ID", "nombre");
            return View();
        }

        // POST: Actividads/Create
        [Route("Actividad/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,nombre,fechaInicio,fechaFin,ObjetivoId")] Actividad actividad)
        {
            if (ModelState.IsValid)
            {
                db.Actividads.Add(actividad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ObjetivoId = new SelectList(db.Objetivoes, "ID", "nombre", actividad.ObjetivoId);
            return View(actividad);
        }

        // GET: /Actividad/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actividad actividad = db.Actividads.Find(id);
            if (actividad == null)
            {
                return HttpNotFound();
            }
            ViewBag.ObjetivoId = new SelectList(db.Objetivoes, "ID", "nombre", actividad.ObjetivoId);
            return View(actividad);
        }

        // POST: /Actividad/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,nombre,fechaInicio,fechaFin,ObjetivoId")] Actividad actividad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actividad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ObjetivoId = new SelectList(db.Objetivoes, "ID", "nombre", actividad.ObjetivoId);
            return View(actividad);
        }

        // GET: /Actividad/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actividad actividad = db.Actividads.Find(id);
            if (actividad == null)
            {
                return HttpNotFound();
            }
            return View(actividad);
        }

        // POST: /Actividad/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Actividad actividad = db.Actividads.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.Actividads.Remove(actividad);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(actividad);
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
