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
using Monitoreo.Models.BO;

namespace Monitoreo.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AulaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Aulas
        [Route("Aulas")]
        public ActionResult Index()
        {
            var aulas = db.Aulas.Include(a => a.Centro).Include(a => a.Grado);
            return View(aulas.ToList());
        }

        // POST: Aulas
        [Route("Aulas/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var aulas = db.Aulas.Include(a => a.Centro).Include(a => a.Grado);
            var recordsTotal = aulas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = aulas.Select(x => new { DT_RowId = x.Id, x.CentroId, x.GradoId });

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


        //[HttpPost]
        //public JsonResult GetSeccionesAulaByCentroByGrado(int centroId, int gradoId)
        //{
        //    List<SeccionAula> aulas = db.SeccionesAula.Where(g => g.gradoId == gradoId).Where(c => c.centroId == centroId);
        //    List<SeccionAula> seccionesAula = new List<SeccionAula>();
        //    seccionesAula =  aulas.SeccionesAula.ToList();

        //    List<Seccion> secciones = new List<Seccion>();
        //    secciones = secciones.Distinct().ToList();
        //    var jsonData = new
        //    {
        //        data = seccionesAula.Select(y => new
        //        {
        //            id = y.Id,
        //            Numero = y.Seccion.Numero,
        //        })

        //    };
        //    return Json(jsonData, JsonRequestBehavior.AllowGet);
        //}








        [HttpPost]
        public JsonResult GetSecciones()
        {
            var secciones = db.Secciones.ToList();
            // Preparando respuesta y ejecutando consulta
            var jsonData = new
            {
                data = secciones.Select(y => new
                {
                    id = y.Id,
                    numero = y.Numero
                })
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        // GET: /Aula/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aula aula = db.Aulas.Find(id);
            if (aula == null)
            {
                return HttpNotFound();
            }
            return View(aula);
        }

        // GET: Aulas/Create
        [Route("Aulas/Create")]
        public ActionResult Create()
        {
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo");
            //ViewBag.GradoId = new SelectList(db.GradoCentroes, "ID", "grado");
            ViewBag.Secciones = new SelectList(db.Secciones, "Id", "Numero");
            if (Request.Params["Modal"] != null)
            {
                return PartialView();
            }
            return View();
        }

        // POST: Aulas/Create
        [Route("Aulas/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Aula aula)
        {

            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];
            string Secciones = Request.Form["seccion.Id"];
            string[] SeccionesArr = Secciones.Split(',');
            List<string> seccionesStrTemp = SeccionesArr.Skip(1).ToList();
            

            //if (ModelState.IsValid)
            //{
            //    bool isRepeated = db.Aulas.Where(a => a.CentroId == aula.CentroId).Any(g => g.GradoId == aula.GradoId);
            //    if (!isRepeated)
            //    {
            //        try
            //        {
            //            db.Aulas.Add(aula);
            //            db.SaveChanges();


            //            //Inserta las secciones de estas aulas
            //            List<SeccionAula> seccionesAula = new List<SeccionAula>();
            //            foreach (var seccion in SeccionesArr.Skip(1))
            //            {
            //                SeccionAula s = new SeccionAula();
            //                //s.AulaId = aula.Id;
            //                s.SeccionId = Convert.ToInt32(seccion);
            //                bool isSeccionAulaRepeated = db.SeccionesAula.Any(a => a.AulaId == aula.Id);
            //                if (!isSeccionAulaRepeated)
            //                {
            //                    seccionesAula.Add(s);
            //                }
            //            }
            //            db.SeccionesAula.AddRange(seccionesAula);
            //            db.SaveChanges();
            //            // End Insertar
            //        }
            //        catch (Exception e)
            //        {
            //            var msj = e.Message;
            //        }
            //    }
            //    else {

            //        //Inserta las secciones de estas aulas
            //        List<SeccionAula> seccionesAula = new List<SeccionAula>();
            //        foreach (var seccion in SeccionesArr.Skip(1))
            //        {
            //            Aula aulaRepetida =  db.Aulas.Where(a => a.CentroId == aula.CentroId).Where(g => g.GradoId == aula.GradoId).SingleOrDefault();
            //            SeccionAula s = new SeccionAula();
            //          //  s.AulaId = aulaRepetida.Id;
            //            s.SeccionId = Convert.ToInt32(seccion);
            //            bool isSeccionAulaRepeated = db.SeccionesAula.Any(a => a.AulaId == aula.Id);
            //            if (!isSeccionAulaRepeated)
            //            {
            //                seccionesAula.Add(s);
            //            }                        
            //        }
            //        db.SeccionesAula.AddRange(seccionesAula);
            //        db.SaveChanges();
                    // End Insertar
            //    }


            //    if (ViewBag.MasterType != null)
            //    {
            //        return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
            //    }
            //    return RedirectToAction("Index");
            //}

            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo");
            //ViewBag.GradoId = new SelectList(db.GradoCentroes, "ID", "grado");
            ViewBag.Secciones = new SelectList(db.Secciones, "Id", "Numero");
            return View(aula);
        }



        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult CentroAula(int CentroId)
        {
            var aulas = db.Aulas.Include(d => d.Centro).Include(g => g.Grado).Where(s => s.CentroId == CentroId);
            ViewBag.MasterType = "Centro";
            ViewBag.MasterId = CentroId;
            return PartialView(aulas.ToList());
        }




        // GET: /Aula/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aula aula = db.Aulas.Find(id);
            if (aula == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", aula.CentroId);
           // ViewBag.GradoId = new SelectList(db.GradoCentroes, "ID", "grado", aula.GradoId);          
            return View(aula);
        }

        // POST: /Aula/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,CentroId,GradoId,SeccionID")] Aula aula)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aula).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CentroId = new SelectList(db.Centros, "Id", "Codigo", aula.CentroId);
            //ViewBag.GradoId = new SelectList(db.GradoCentroes, "ID", "grado", aula.GradoId);
            return View(aula);
        }

        // GET: /Aula/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aula aula = db.Aulas.Find(id);
            if (aula == null)
            {
                return HttpNotFound();
            }
            return View(aula);
        }

        // POST: /Aula/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aula aula = db.Aulas.Find(id);

            try
            {
                //if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                //    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.Aulas.Remove(aula);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(aula);
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
