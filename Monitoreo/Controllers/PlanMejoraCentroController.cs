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
using Monitoreo.Models;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class PlanMejoraCentroController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: PlanMejoraCentros
        [Route("PlanMejoraCentro")]
        public ActionResult Index()
        {
            var planmejoracentroes = db.PlanMejoraCentroes.Include(p => p.Centro);
            return View(planmejoracentroes.ToList());
        }


        public ActionResult IndexByAcompanante()
        {
            Acompanante acompanante = new Acompanante();
            List<PlanMejoraCentro> planmejoracentroes = new List<PlanMejoraCentro>();
            
            try
            {
                acompanante = db.Acompanantes.Where(c => c.Persona.Cedula == User.Identity.Name).SingleOrDefault();
                 planmejoracentroes = db.PlanMejoraCentroes.Where(c => c.CentroId == acompanante.centroId).Include(p => p.Centro).ToList();
            }
            catch (Exception e) {
                var msj = e.Message;
            }

            if(acompanante != null){
                ViewBag.CentroID = acompanante.centroId;
            }            
            return View(planmejoracentroes.ToList());
        }


        [HttpPost]
        public JsonResult PlanCentroByAcompanante()
        {

            Acompanante acompanante = new Acompanante();
            List<PlanMejoraCentro> planmejoracentroes = new List<PlanMejoraCentro>();
            try
            {
                acompanante = db.Acompanantes.Where(c => c.Persona.Cedula == User.Identity.Name).SingleOrDefault();
                planmejoracentroes = db.PlanMejoraCentroes.Where(c => c.CentroId == acompanante.centroId).Include(p => p.Centro).ToList();
            }
            catch (Exception e)
            {
                var msj = e.Message;
            }
       
            var jsonData = new
            {
                data = planmejoracentroes.Select(y => new
                {
                    id = y.ID,
                    nombre = y.nombre,
                    periodo = y.periodo,
                    fechaInicio = y.fechaInicio.ToString(),
                    fechaFin = y.fechaFin.ToString()
                }),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



       

        // GET: /PlanMejoraCentro/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanMejoraCentro planMejoraCentro = db.PlanMejoraCentroes.Find(id);
            if (planMejoraCentro == null)
            {
                return HttpNotFound();
            }
            return View(planMejoraCentro);
        }


        public ActionResult CreateByAcompanante(int id)
        {
            ViewBag.CentroID = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateByAcompanante([Bind(Include = "ID,nombre,periodo,fechaInicio,fechaFin,CentroId")] PlanMejoraCentro planMejoraCentro)
        {
            if (ModelState.IsValid)
            {
                db.PlanMejoraCentroes.Add(planMejoraCentro);
                db.SaveChanges();
                return RedirectToAction("IndexByAcompanante");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", planMejoraCentro.CentroId);
            return View(planMejoraCentro);
        }




        // GET: PlanMejoraCentros/Create
        [Route("PlanMejoraCentro/Create")]
        public ActionResult Create()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo");
            return View();
        }

        // POST: PlanMejoraCentros/Create
        [Route("PlanMejoraCentro/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,nombre,periodo,fechaInicio,fechaFin,CentroId")] PlanMejoraCentro planMejoraCentro)
        {
            if (ModelState.IsValid)
            {
                db.PlanMejoraCentroes.Add(planMejoraCentro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", planMejoraCentro.CentroId);
            return View(planMejoraCentro);
        }

        // GET: /PlanMejoraCentro/5/Edit
        public ActionResult EditByAcompanante(int? id, int centroId)
        {
            ViewBag.CentroID = centroId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanMejoraCentro planMejoraCentro = db.PlanMejoraCentroes.Find(id);
            if (planMejoraCentro == null)
            {
                return HttpNotFound();
            }
            return View(planMejoraCentro);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditByAcompanante([Bind(Include = "ID,nombre,periodo,fechaInicio,fechaFin,CentroId")] PlanMejoraCentro planMejoraCentro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(planMejoraCentro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexByAcompanante");
            }
            return View(planMejoraCentro);
        }





        // GET: /PlanMejoraCentro/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanMejoraCentro planMejoraCentro = db.PlanMejoraCentroes.Find(id);
            if (planMejoraCentro == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", planMejoraCentro.CentroId);
            return View(planMejoraCentro);
        }

        // POST: /PlanMejoraCentro/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,nombre,periodo,fechaInicio,fechaFin,CentroId")] PlanMejoraCentro planMejoraCentro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(planMejoraCentro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", planMejoraCentro.CentroId);
            return View(planMejoraCentro);
        }

        public ActionResult DeleteByAcompanante(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanMejoraCentro planMejoraCentro = db.PlanMejoraCentroes.Find(id);
            if (planMejoraCentro == null)
            {
                return HttpNotFound();
            }
            return View(planMejoraCentro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteByAcompanante(int id)
        {
            PlanMejoraCentro planMejoraCentro = db.PlanMejoraCentroes.Find(id);

            try
            {

                if (ModelState.IsValid)
                {
                    db.PlanMejoraCentroes.Remove(planMejoraCentro);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("IndexByAcompanante");
            else return View(planMejoraCentro);
        }



        // GET: /PlanMejoraCentro/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanMejoraCentro planMejoraCentro = db.PlanMejoraCentroes.Find(id);
            if (planMejoraCentro == null)
            {
                return HttpNotFound();
            }
            return View(planMejoraCentro);
        }

        // POST: /PlanMejoraCentro/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlanMejoraCentro planMejoraCentro = db.PlanMejoraCentroes.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.PlanMejoraCentroes.Remove(planMejoraCentro);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(planMejoraCentro);
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
