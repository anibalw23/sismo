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
    public class SeccionesAulasController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();


        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult CentroSeccionAula(int CentroId)
        {
            var aulas = db.SeccionesAula.Include(g => g.Grado).Where(s => s.Grado.CentroId == CentroId);
            ViewBag.MasterType = "Centro";
            ViewBag.MasterId = CentroId;
            return PartialView(aulas.ToList());
        }



        [HttpPost]
        public JsonResult GetSeccionesAulaByCentroByGrado(int centroId, int gradoId)
        {
            List<SeccionAula> seccionesAula = new List<SeccionAula>();

            try {
                CentroGrado CentroGrado = db.CentroGradoes.Where(g => g.GradoLookupId == gradoId).Where(c => c.CentroId == centroId).SingleOrDefault();
                seccionesAula = CentroGrado.SeccionesAulas.ToList();
            }
            catch(Exception e){
                ModelState.AddModelError("error2021","error single or default");
            }

            var jsonData = new
            {
                data = seccionesAula.Select(y => new
                {
                    id = y.Id,
                    Numero = y.Seccion.Numero,
                })

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }






        // GET: SeccionesAulass
        [Route("SeccionesAulas")]
        public ActionResult Index()
        {
            var seccionesaula = db.SeccionesAula.Include(s => s.Grado).Include(s => s.Seccion);
            return View(seccionesaula.ToList());
        }

        // POST: SeccionesAulass
        [Route("SeccionesAulas/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var seccionesaula = db.SeccionesAula.Include(s => s.Grado).Include(s => s.Seccion);
            var recordsTotal = seccionesaula.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = seccionesaula.Select(x => new { DT_RowId = x.Id, x.gradoId, x.SeccionId });

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
                data = data.OrderBy(s => s.SeccionId);
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

        // GET: /SeccionesAulas/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeccionAula seccionAula = db.SeccionesAula.Find(id);
            if (seccionAula == null)
            {
                return HttpNotFound();
            }
            return View(seccionAula);
        }

        // GET: SeccionesAulass/Create
        [Route("SeccionesAulas/Create")]
        public ActionResult Create()
        {
            ViewBag.centroId = new SelectList(db.Centros, "Id", "Codigo");
            ViewBag.Secciones = new SelectList(db.Secciones, "Id", "Numero");

            if (Request.Params["Modal"] != null)
            {
                return PartialView();
            }
            return View();
        }

        // POST: SeccionesAulass/Create
        [Route("SeccionesAulas/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,centroId,gradoId,SeccionId")] SeccionAula seccionAula)
        {

            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];
            string Secciones = Request.Form["seccion.Id"];
            string[] SeccionesArr = Secciones.Split(',');
            List<string> seccionesStrTemp = SeccionesArr.Skip(1).ToList();

            //if (ModelState.IsValid)
            //{

            //    //Inserta las secciones de estas aulas
            //    List<SeccionAula> seccionesAula = new List<SeccionAula>();
            //    foreach (var seccion in SeccionesArr.Skip(1))
            //    {
            //        SeccionAula s = new SeccionAula();
            //        s.SeccionId = Convert.ToInt32(seccion);
            //        s.centroId = seccionAula.centroId;
            //        s.gradoId = seccionAula.gradoId;
            //        s.tanda = seccionAula.tanda;

            //        bool isSeccionAulaRepeated = db.SeccionesAula.Where(a => a.centroId == seccionAula.centroId).Where(f => f.SeccionId == seccionAula.SeccionId).Any(g => g.gradoId == seccionAula.gradoId);
            //        if (!isSeccionAulaRepeated)
            //        {
            //            seccionesAula.Add(s);
            //        }
            //    }
            //    db.SeccionesAula.AddRange(seccionesAula);
            //    db.SaveChanges();

            //    if (ViewBag.MasterType != null)
            //    {
            //        return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
            //    }
            //    return RedirectToAction("Index");
            //}

            //ViewBag.centroId = new SelectList(db.Centros, "Id", "Codigo", seccionAula.centroId);
            //ViewBag.GradoId = new SelectList(db.GradoCentroes, "ID", "grado", seccionAula.gradoId);
            //ViewBag.Secciones = new SelectList(db.Secciones, "Id", "Numero", seccionAula.SeccionId);
            return View(seccionAula);
        }

        // GET: /SeccionesAulas/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeccionAula seccionAula = db.SeccionesAula.Find(id);
            if (seccionAula == null)
            {
                return HttpNotFound();
            }
            //ViewBag.centroId = new SelectList(db.Centros, "Id", "Codigo", seccionAula.centroId);
           // ViewBag.gradoId = new SelectList(db.GradoCentroes, "ID", "grado", seccionAula.gradoId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", seccionAula.SeccionId);
            return View(seccionAula);
        }

        // POST: /SeccionesAulas/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,centroId,gradoId,SeccionId")] SeccionAula seccionAula)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seccionAula).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.centroId = new SelectList(db.Centros, "Id", "Codigo", seccionAula.centroId);
           // ViewBag.gradoId = new SelectList(db.GradoCentroes, "ID", "grado", seccionAula.gradoId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", seccionAula.SeccionId);
            return View(seccionAula);
        }

        // GET: /SeccionesAulas/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeccionAula seccionAula = db.SeccionesAula.Find(id);
            if (seccionAula == null)
            {
                return HttpNotFound();
            }
            return View(seccionAula);
        }

        // POST: /SeccionesAulas/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SeccionAula seccionAula = db.SeccionesAula.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.SeccionesAula.Remove(seccionAula);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(seccionAula);
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
