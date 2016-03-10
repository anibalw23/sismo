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
    public class CursoTallerController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: CursosTalleres
        [Route("CursosTalleres")]
        public ActionResult Index()
        {
            var cursostalleres = db.CursosTalleres.Include(c => c.Centro).Include(c => c.CicloFormativo);
            return View(cursostalleres.ToList());
        }

        // GET: /CursoTaller/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CursoTaller cursoTaller = db.CursosTalleres.Find(id);
            if (cursoTaller == null)
            {
                return HttpNotFound();
            }
            return View(cursoTaller);
        }

        // GET: CursoTallers/Create
        [Route("CursosTalleres/Create")]
        public ActionResult Create()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo");
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Id");
            return View();
        }

        // POST: CursoTallers/Create
        [Route("CursosTalleres/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Nombre,Grado,Tipo,CicloFormativoId,CentroId,FechaInicio,FechaFin,Estado")] CursoTaller cursoTaller)
        {
            if (ModelState.IsValid)
            {
                db.CursosTalleres.Add(cursoTaller);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", cursoTaller.CentroId);
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Id", cursoTaller.CicloFormativoId);
            return View(cursoTaller);
        }

        // GET: /CursoTaller/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CursoTaller cursoTaller = db.CursosTalleres.Find(id);
            if (cursoTaller == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", cursoTaller.CentroId);
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Id", cursoTaller.CicloFormativoId);
            return View(cursoTaller);
        }

        // POST: /CursoTaller/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Nombre,Grado,Tipo,CicloFormativoId,CentroId,FechaInicio,FechaFin,Estado")] CursoTaller cursoTaller)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cursoTaller).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", cursoTaller.CentroId);
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Id", cursoTaller.CicloFormativoId);
            return View(cursoTaller);
        }

        // GET: /CursoTaller/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CursoTaller cursoTaller = db.CursosTalleres.Find(id);
            if (cursoTaller == null)
            {
                return HttpNotFound();
            }
            return View(cursoTaller);
        }

        // POST: /CursoTaller/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CursoTaller cursoTaller = db.CursosTalleres.Find(id);
            db.CursosTalleres.Remove(cursoTaller);
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
