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
    public class ActividadEspecificaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();


        public ActionResult PlanActividades(int PlanId)
        {
            var actividads = db.ActividadEspecificas.Include(a => a.Actividad).Where(p => p.Actividad.Objetivo.PlanMejoraCentroId == PlanId);
            return PartialView(actividads.ToList());
        }


        // GET: ActividadEspecificas
        [Route("ActividadEspecifica")]
        public ActionResult Index()
        {
            var actividadespecificas = db.ActividadEspecificas.Include(a => a.Actividad);
            return View(actividadespecificas.ToList());
        }

        // POST: ActividadEspecificas
        [Route("ActividadEspecifica/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var actividadespecificas = db.ActividadEspecificas.Include(a => a.Actividad);
            var recordsTotal = actividadespecificas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = actividadespecificas.Select(x => new { DT_RowId = x.ID, x.fechaInicio, x.fechaFin, x.observaciones, x.ActividadId });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.observaciones.Contains(searchValue)
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
                        case "observaciones":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.observaciones);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.observaciones);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.fechaInicio);
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

        // GET: /ActividadEspecifica/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadEspecifica actividadEspecifica = db.ActividadEspecificas.Find(id);
            if (actividadEspecifica == null)
            {
                return HttpNotFound();
            }
            return View(actividadEspecifica);
        }

        // GET: ActividadEspecificas/Create
        [Route("ActividadEspecifica/Create")]
        public ActionResult Create()
        {
            ViewBag.ActividadId = new SelectList(db.Actividads, "ID", "nombre");
            return View();
        }

        // POST: ActividadEspecificas/Create
        [Route("ActividadEspecifica/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,nombre,fechaInicio,fechaFin,observaciones,ActividadId")] ActividadEspecifica actividadEspecifica)
        {
            if (ModelState.IsValid)
            {
                db.ActividadEspecificas.Add(actividadEspecifica);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActividadId = new SelectList(db.Actividads, "ID", "nombre", actividadEspecifica.ActividadId);
            return View(actividadEspecifica);
        }

        public ActionResult EditFechaFin(int? pk, string value)
        {
            var result = "OK";
            if (ModelState.IsValid)
            {
                try {
                    ActividadEspecifica actividadEspecifica = db.ActividadEspecificas.AsNoTracking().Where(i => i.ID == pk).SingleOrDefault();
                    actividadEspecifica.fechaFin = DateTime.Parse(value);
                    db.Entry(actividadEspecifica).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch(Exception e){
                    result = "ERROR";
                }

            }
           
            var jsonData = new
            {
                result = result
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        public ActionResult EditFechaInicio(int? pk, string value)
        {
            var result = "OK";
            if (ModelState.IsValid)
            {
                try
                {
                    ActividadEspecifica actividadEspecifica = db.ActividadEspecificas.AsNoTracking().Where(i => i.ID == pk).SingleOrDefault();
                    actividadEspecifica.fechaInicio = DateTime.Parse(value);
                    db.Entry(actividadEspecifica).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    result = "ERROR";
                }

            }

            var jsonData = new
            {
                result = result
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        public ActionResult EditObservaciones(int? pk, string value)
        {
            var result = "OK";
            if (ModelState.IsValid)
            {
                try
                {
                    ActividadEspecifica actividadEspecifica = db.ActividadEspecificas.AsNoTracking().Where(i => i.ID == pk).SingleOrDefault();
                    actividadEspecifica.observaciones = value;
                    db.Entry(actividadEspecifica).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    result = "ERROR";
                }

            }

            var jsonData = new
            {
                result = result
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }







        // GET: /ActividadEspecifica/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadEspecifica actividadEspecifica = db.ActividadEspecificas.Find(id);
            if (actividadEspecifica == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActividadId = new SelectList(db.Actividads, "ID", "nombre", actividadEspecifica.ActividadId);
            return View(actividadEspecifica);
        }

        // POST: /ActividadEspecifica/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,nombre,fechaInicio,fechaFin,observaciones,ActividadId")] ActividadEspecifica actividadEspecifica)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actividadEspecifica).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActividadId = new SelectList(db.Actividads, "ID", "nombre", actividadEspecifica.ActividadId);
            return View(actividadEspecifica);
        }

        // GET: /ActividadEspecifica/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadEspecifica actividadEspecifica = db.ActividadEspecificas.Find(id);
            if (actividadEspecifica == null)
            {
                return HttpNotFound();
            }
            return View(actividadEspecifica);
        }

        // POST: /ActividadEspecifica/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActividadEspecifica actividadEspecifica = db.ActividadEspecificas.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.ActividadEspecificas.Remove(actividadEspecifica);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(actividadEspecifica);
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
