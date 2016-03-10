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
    public class GradoCentroController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();


        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult GradoCentro(int CentroId)
        {
            var gradoscentros = db.GradoCentros.Where(d => d.CentroId == CentroId).Include(g => g.).ToList();
            ViewBag.MasterType = "Centro";
            ViewBag.MasterId = CentroId;
            return PartialView(gradoscentros.ToList());
        }




        // GET: GradoCentros
        [Route("GradoCentro")]
        public ActionResult Index()
        {
            var gradocentroes = db.GradoCentros.Include(g => g.Centro).Include(f => f.GradoLookup);
            return View(gradocentroes.ToList());
        }

        // POST: GradoCentros
        [Route("GradoCentro/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var gradocentroes1 = db.GradoCentros.Include(g => g.Centro).Include(g => g.GradoLookup);
            var recordsTotal = gradocentroes1.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = gradocentroes1.Select(x => new { DT_RowId = x.ID, x.CentroId, x.GradoNombreId });

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
                data = data.OrderBy(s => s.CentroId);
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

        // GET: /GradoCentro/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoCentro gradoCentro = db.GradoCentros.Find(id);
            if (gradoCentro == null)
            {
                return HttpNotFound();
            }
            return View(gradoCentro);
        }

        // GET: GradoCentros/Create
        [Route("GradoCentro/Create")]
        public ActionResult Create()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo");
            ViewBag.GradoNombreId = new SelectList(db.GradoNombre, "ID", "grado");
            ViewBag.Secciones = new SelectList(db.Secciones, "Id", "Numero");

            if (Request.Params["Modal"] != null)
            {
                return PartialView();
            }


            return View();
        }

        // POST: GradoCentros/Create
        [Route("GradoCentro/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,CentroId,GradoNombreId")] GradoCentro gradoCentro)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];
            string Secciones = Request.Form["seccion.Id"];
            string[] SeccionesArr = Secciones.Split(',');
            List<string> seccionesStrTemp = SeccionesArr.Skip(1).ToList();

            int centroID = Convert.ToInt32(ViewBag.MasterId);

            if (ModelState.IsValid)
            {
                bool isGradoRepeated = db.GradoCentros.Where(c => c.CentroId == centroID).Any(g => g.GradoNombreId == gradoCentro.GradoNombreId);

                if(!isGradoRepeated){
                    gradoCentro.CentroId = centroID;
                    db.GradoCentros.Add(gradoCentro);
                    db.SaveChanges();
                }

               
                //Añadir Secciones
                List<SeccionAula> seccionesAula = new List<SeccionAula>();
                foreach (var seccion in SeccionesArr.Skip(1))
                {
                    SeccionAula s = new SeccionAula();
                    s.SeccionId = Convert.ToInt32(seccion);
                    s.gradoId = gradoCentro.ID;
                    bool isSeccionAulaRepeated = db.SeccionesAula.Where(a => a.gradoId == s.gradoId).Any(g => g.SeccionId == s.SeccionId);
                    if (!isSeccionAulaRepeated)
                    {
                        seccionesAula.Add(s);
                    }
                }
                db.SeccionesAula.AddRange(seccionesAula);
                db.SaveChanges();

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }


                return RedirectToAction("Index");
            }

            ViewBag.Secciones = new SelectList(db.Secciones, "Id", "Numero");
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", gradoCentro.CentroId);
            ViewBag.GradoNombre = new SelectList(db.GradoNombre, "ID", "grado", gradoCentro.GradoNombreId);
            return View(gradoCentro);
        }

        // GET: /GradoCentro/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoCentro gradoCentro = db.GradoCentros.Find(id);
            if (gradoCentro == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", gradoCentro.CentroId);
            ViewBag.GradoNombreId = new SelectList(db.GradoNombre, "ID", "grado", gradoCentro.GradoNombreId);
            return View(gradoCentro);
        }

        // POST: /GradoCentro/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,CentroId,GradoNombreId")] GradoCentro gradoCentro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gradoCentro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", gradoCentro.CentroId);
            ViewBag.GradoNombreId = new SelectList(db.GradoNombre, "ID", "grado", gradoCentro.GradoNombreId);
            return View(gradoCentro);
        }

        // GET: /GradoCentro/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradoCentro gradoCentro = db.GradoCentros.Find(id);
            if (gradoCentro == null)
            {
                return HttpNotFound();
            }
            return View(gradoCentro);
        }

        // POST: /GradoCentro/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GradoCentro gradoCentro = db.GradoCentros.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.GradoCentros.Remove(gradoCentro);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(gradoCentro);
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
