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
    //[Authorize(Roles = "Administrador")]
    public class ActividadFormativaBaseController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: ActividadesFormativasBases
        [Route("ActividadesFormativasBases")]
        public ActionResult Index()
        {
            return View(db.ActividadFormativaBases.ToList());
        }

        // GET: /ActividadFormativaBase/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadFormativaBase actividadFormativaBase = db.ActividadFormativaBases.Find(id);
            if (actividadFormativaBase == null)
            {
                return HttpNotFound();
            }
            return View(actividadFormativaBase);
        }

        // GET: ActividadesFormativasBases/Create
        [Route("ActividadesFormativasBases/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ActividadesFormativasBases/Create
        [Route("ActividadesFormativasBases/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Tipo,Duracion,Organizacion,Actores,ModoEvaluacion")] ActividadFormativaBase actividadFormativaBase)
        {
            if (ModelState.IsValid)
            {
                db.ActividadFormativaBases.Add(actividadFormativaBase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(actividadFormativaBase);
        }

        // GET: /ActividadFormativaBase/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadFormativaBase actividadFormativaBase = db.ActividadFormativaBases.Find(id);
            if (actividadFormativaBase == null)
            {
                return HttpNotFound();
            }
            return View(actividadFormativaBase);
        }

        // POST: /ActividadFormativaBase/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Tipo,Duracion,Organizacion,Actores,ModoEvaluacion")] ActividadFormativaBase actividadFormativaBase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actividadFormativaBase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(actividadFormativaBase);
        }

        // GET: /ActividadFormativaBase/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActividadFormativaBase actividadFormativaBase = db.ActividadFormativaBases.Find(id);
            if (actividadFormativaBase == null)
            {
                return HttpNotFound();
            }
            return View(actividadFormativaBase);
        }

        // POST: /ActividadFormativaBase/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActividadFormativaBase actividadFormativaBase = db.ActividadFormativaBases.Find(id);
            db.ActividadFormativaBases.Remove(actividadFormativaBase);
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
