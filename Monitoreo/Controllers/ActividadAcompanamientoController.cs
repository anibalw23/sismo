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
    [Authorize(Roles = "Administrador,EspecialistaCurricular")]
    public class ActividadAcompanamientoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: ActividadAcompanamientos
        [Route("ActividadAcompanamientos")]
        public ActionResult Index()
        {
            var actividadacompanamientoes = db.ActividadAcompanamientoes.Include(a => a.SuperCicloFormativo);
            return View(actividadacompanamientoes.ToList());
        }

        // POST: ActividadAcompanamientos
        [Route("ActividadAcompanamientos/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var actividadacompanamientoes = db.ActividadAcompanamientoes.Include(a => a.SuperCicloFormativo);
            var recordsTotal = actividadacompanamientoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = actividadacompanamientoes.Select(x => new { DT_RowId = x.ID, x.SuperCicloFormativoId, x.TipoAcompanamiento });

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
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.SuperCicloFormativoId);
            }

            // Preparando respuesta y ejecutando consulta
            var jsonData = new
            {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.Skip(from).Take(limit).ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: /ActividadAcompanamiento/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadAcompanamiento actividadAcompanamiento = db.ActividadAcompanamientoes.Find(id);
            if (actividadAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(actividadAcompanamiento);
        }

        // GET: ActividadAcompanamientos/Create
        [Route("ActividadAcompanamientos/Create")]
        public ActionResult Create(int superCicloId)
        {
            ViewBag.SuperCicloFormativoId = superCicloId;
            return View();
        }

        // POST: ActividadAcompanamientos/Create
        [Route("ActividadAcompanamientos/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SuperCicloFormativoId,TipoAcompanamiento")] ActividadAcompanamiento actividadAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                bool isRepeated = db.ActividadAcompanamientoes.Where(a => a.SuperCicloFormativoId == actividadAcompanamiento.SuperCicloFormativoId).Any(b => b.TipoAcompanamiento == actividadAcompanamiento.TipoAcompanamiento);
                if (!isRepeated)
                {
                    db.ActividadAcompanamientoes.Add(actividadAcompanamiento);
                    db.SaveChanges();
                    return RedirectToAction("Details", "SuperCicloFormativo", new { id = actividadAcompanamiento.SuperCicloFormativoId });
                }
                else
                {
                    ModelState.AddModelError("Error 3659", "Este tipo de actividad ya está registrada para este Ciclo Formativo!");
                    return View();
                }
            }

            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes, "Id", "nombre", actividadAcompanamiento.SuperCicloFormativoId);
            return View(actividadAcompanamiento);
        }

        // GET: /ActividadAcompanamiento/5/Edit
        public ActionResult Edit(int? id, int superCicloID)
        {

            ViewBag.SuperCicloFormativoId = superCicloID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadAcompanamiento actividadAcompanamiento = db.ActividadAcompanamientoes.Find(id);
            if (actividadAcompanamiento == null)
            {
                return HttpNotFound();
            }
            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes, "Id", "nombre", actividadAcompanamiento.SuperCicloFormativoId);
            return View(actividadAcompanamiento);
        }

        // POST: /ActividadAcompanamiento/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SuperCicloFormativoId,TipoAcompanamiento")] ActividadAcompanamiento actividadAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actividadAcompanamiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SuperCicloFormativoId = new SelectList(db.SuperCicloFormativoes, "Id", "nombre", actividadAcompanamiento.SuperCicloFormativoId);
            return View(actividadAcompanamiento);
        }

        // GET: /ActividadAcompanamiento/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadAcompanamiento actividadAcompanamiento = db.ActividadAcompanamientoes.Find(id);
            if (actividadAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(actividadAcompanamiento);
        }

        // POST: /ActividadAcompanamiento/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActividadAcompanamiento actividadAcompanamiento = db.ActividadAcompanamientoes.Find(id);
            if (ModelState.IsValid)
            {
                db.ActividadAcompanamientoes.Remove(actividadAcompanamiento);
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("error", "Error al borrar actividad!");
            }
            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(actividadAcompanamiento);
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
