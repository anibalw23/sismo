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
    public class PersonalController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Personal
        [Route("Personal")]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Index()
        {
            var personals = db.Personal.Include(p => p.Centro).Include(p => p.Persona);
            return View(personals.ToList());
        }

        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult GetList(string searchTerm, int pageSize, int pageNum)
        {
            //var matches = from m in db.Personal
            //              orderby m.Persona.NombreCompleto
            //              select m;

            IQueryable<Personal> matches = db.Personal; ;

            if (!String.IsNullOrEmpty(searchTerm))
            {
                matches = matches.Where(s => s.Persona.NombreCompleto.Contains(searchTerm));
            }

            matches = matches.OrderBy(x => x.Persona.NombreCompleto).Skip(pageSize * (pageSize - 1)).Take(pageSize);

            //Return the data as a jsonp result
            return new JsonResult
            {
                Data = matches,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: /Personal/5/Details
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personal personal = db.Personal.Find(id);
            if (personal == null)
            {
                return HttpNotFound();
            }
            return View(personal);
        }

        // GET: Personal/Create
        [Route("Personal/Create")]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo");
            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "Cedula");
            return View();
        }

        // POST: Personal/Create
        [Route("Personal/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create([Bind(Include="Id,Codigo,PersonaId,CentroId,FechaContratacion,FuncionesEjerce,Tanda")] Personal personal)
        {
            if (ModelState.IsValid)
            {
                db.Personal.Add(personal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", personal.CentroId);
            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "Cedula", personal.PersonaId);
            return View(personal);
        }

        // GET: /Personal/5/Edit
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personal personal = db.Personal.Find(id);
            if (personal == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", personal.CentroId);
            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "Cedula", personal.PersonaId);
            return View(personal);
        }

        // POST: /Personal/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit([Bind(Include="Id,Codigo,PersonaId,CentroId,FechaContratacion,FuncionesEjerce,Tanda")] Personal personal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", personal.CentroId);
            ViewBag.PersonaId = new SelectList(db.Personas, "Id", "Cedula", personal.PersonaId);
            return View(personal);
        }

        // GET: /Personal/5/Delete
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personal personal = db.Personal.Find(id);
            if (personal == null)
            {
                return HttpNotFound();
            }
            return View(personal);
        }

        // POST: /Personal/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            Personal personal = db.Personal.Find(id);
            db.Personal.Remove(personal);
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
