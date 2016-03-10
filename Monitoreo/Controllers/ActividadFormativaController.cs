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
    //[Authorize(Roles = "Administrador")]
    public class ActividadFormativaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: ActividadesFormativas
        [Route("ActividadesFormativas")]
        public ActionResult Index()
        {
            var actividadesformativas = db.ActividadesFormativas.Include(a => a.ActividadFormativaBase).Include(a => a.CicloFormativo);
            return View(actividadesformativas.ToList());
        }

        public ActionResult CicloActividad(int CicloFormativoId)
        {
            var actividadesformativas = db.ActividadesFormativas.Include(a => a.ActividadFormativaBase).Include(a => a.CicloFormativo).Where(s => s.CicloFormativoId == CicloFormativoId);

            ViewBag.MasterType = "CicloFormativo";
            ViewBag.MasterId = CicloFormativoId;

            return PartialView(actividadesformativas.ToList());
        }

        // POST: ActividadFormativas
        [Route("ActividadesFormativas/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var actividadesformativas = db.ActividadesFormativas.Include(a => a.ActividadFormativaBase).Include(a => a.CicloFormativo);
            var recordsTotal = actividadesformativas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = actividadesformativas.Select(x => new { DT_RowId = x.Id, x.ActividadFormativaBaseId, x.CicloFormativoId, x.Duracion, x.FechaInicio, x.FechaFin });

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
                data = data.OrderBy(s => s.ActividadFormativaBaseId);
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

        // GET: /ActividadFormativa/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadFormativa actividadFormativa = db.ActividadesFormativas.Find(id);
            if (actividadFormativa == null)
            {
                return HttpNotFound();
            }
            return View(actividadFormativa);
        }

        // GET: ActividadesFormativas/Create
        [Route("ActividadesFormativas/Create")]
        public ActionResult Create()
        {
            ViewBag.ActividadFormativaBaseId = new SelectList(db.ActividadFormativaBases, "Id", "Organizacion");
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");
            return View();
        }

        // POST: ActividadesFormativas/Create
        [Route("ActividadesFormativas/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ActividadFormativa actividadFormativa)
        {
            if (ModelState.IsValid)
            {
                db.ActividadesFormativas.Add(actividadFormativa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActividadFormativaBaseId = new SelectList(db.ActividadFormativaBases, "Id", "Organizacion", actividadFormativa.ActividadFormativaBaseId);
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", actividadFormativa.CicloFormativoId);
            return View(actividadFormativa);
        }

        // GET: /ActividadFormativa/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadFormativa actividadFormativa = db.ActividadesFormativas.Find(id);
            if (actividadFormativa == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActividadFormativaBaseId = new SelectList(db.ActividadFormativaBases, "Id", "Organizacion", actividadFormativa.ActividadFormativaBaseId);
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", actividadFormativa.CicloFormativoId);
            return View(actividadFormativa);
        }

        // POST: /ActividadFormativa/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ActividadFormativa actividadFormativa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actividadFormativa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActividadFormativaBaseId = new SelectList(db.ActividadFormativaBases, "Id", "Organizacion", actividadFormativa.ActividadFormativaBaseId);
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", actividadFormativa.CicloFormativoId);
            return View(actividadFormativa);
        }

        // GET: /ActividadFormativa/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadFormativa actividadFormativa = db.ActividadesFormativas.Find(id);
            if (actividadFormativa == null)
            {
                return HttpNotFound();
            }
            return View(actividadFormativa);
        }

        // POST: /ActividadFormativa/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActividadFormativa actividadFormativa = db.ActividadesFormativas.Find(id);
            db.ActividadesFormativas.Remove(actividadFormativa);
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
