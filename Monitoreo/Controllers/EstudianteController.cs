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
    [Authorize]
    public class EstudianteController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Estudiantes
        [Route("Estudiantes")]
        public ActionResult Index()
        {
            var estudiantes = db.Estudiantes.Include(e => e.Centro).Include(e => e.Persona).Include(e => e.Seccion);
            return View(estudiantes.ToList());
        }

        public ActionResult CentroEstudiantes(int CentroId)
        {
            var Estudiantes = db.Estudiantes.Include(e => e.Centro).Include(e => e.Persona).Include(e => e.Seccion).Where(s => s.CentroId == CentroId); ;

            ViewBag.MasterType = "Centro";
            ViewBag.MasterId = CentroId;

            return PartialView(Estudiantes.ToList());
        }

        // GET: /Estudiante/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = db.Estudiantes.Find(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }
            return View(estudiante);
        }

        // GET: Estudiantes/Create
        [Route("Estudiantes/Create")]
        public ActionResult Create()
        {
            ViewBag.Modal = Request.Params["Modal"];

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre");
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero");

            if (ViewBag.Modal != null)
            {
                return PartialView();
            }

            return View();
        }

        // POST: Estudiantes/Create
        [Route("Estudiantes/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Estudiante estudiante)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ModelState.IsValid)
            {
                db.Estudiantes.Add(estudiante);
                db.SaveChanges();

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }

                return RedirectToAction("Index");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre", estudiante.CentroId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", estudiante.SeccionId);
            return View(estudiante);
        }

        // GET: /Estudiante/5/Edit
        public ActionResult Edit(int? id)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = db.Estudiantes.Find(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre", estudiante.CentroId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", estudiante.SeccionId);
            return View(estudiante);
        }

        // POST: /Estudiante/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Estudiante estudiante)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ModelState.IsValid)
            {
                db.Entry(estudiante).State = EntityState.Modified;
                db.SaveChanges();

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }

                return RedirectToAction("Index");
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Nombre", estudiante.CentroId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", estudiante.SeccionId);
            return View(estudiante);
        }

        // GET: /Estudiante/5/Delete
        public ActionResult Delete(int? id)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estudiante estudiante = db.Estudiantes.Find(id);
            if (estudiante == null)
            {
                return HttpNotFound();
            }
            return View(estudiante);
        }

        // POST: /Estudiante/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            Estudiante estudiante = db.Estudiantes.Find(id);
            db.Estudiantes.Remove(estudiante);
            db.SaveChanges();

            if (ViewBag.MasterType != null)
            {
                return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
            }

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
