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
    public class IndicadorFechaController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: IndicadorFechas
        [Route("IndicadorFecha")]
        public ActionResult Index()
        {
            return View(db.IndicadorFechas.ToList());
        }

        // POST: IndicadorFechas
        [Route("IndicadorFecha/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var indicadorfechas = db.IndicadorFechas;
            var recordsTotal = indicadorfechas.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = indicadorfechas.Select(x => new { DT_RowId = x.Id, x.mes, x.cuarto, x.anio });

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
                data = data.OrderBy(s => s.anio);
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

        // GET: /IndicadorFecha/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorFecha indicadorFecha = db.IndicadorFechas.Find(id);
            if (indicadorFecha == null)
            {
                return HttpNotFound();
            }
            return View(indicadorFecha);
        }

        // GET: IndicadorFechas/Create
        [Route("IndicadorFecha/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: IndicadorFechas/Create
        [Route("IndicadorFecha/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,mes,cuarto,anio")] IndicadorFecha indicadorFecha)
        {
            if (ModelState.IsValid)
            {
                db.IndicadorFechas.Add(indicadorFecha);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(indicadorFecha);
        }



        public ActionResult CrearRapido()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult CrearRapido(IndicadorFecha indicadorFecha)
        {
            int Inicio = Convert.ToInt32(Request.Form["Inicio"]);
            int Fin = Convert.ToInt32(Request.Form["Fin"]);

            int rows = (Fin - Inicio) * 12;
            int mesCount = 1;
            int añoCount = Inicio;
            int quarterCount = 1;
            int i = 0;
            for (i = 1; i <= rows; i++)
            {
                IndicadorFecha indicadorFechaVar = new IndicadorFecha();

                if (mesCount % 12 == 0)
                {
                    añoCount++;
                    quarterCount = 1;
                    mesCount = 1;
                }
                if (mesCount % 4 == 0)
                {
                    quarterCount++;
                }

                indicadorFechaVar.anio = añoCount;
                indicadorFechaVar.mes = mesCount;
                indicadorFechaVar.cuarto = quarterCount;

                int fechaExistCount= db.IndicadorFechas.Where(a => a.anio == añoCount).Where(m => m.mes == mesCount).Where(c => c.cuarto == quarterCount).Count();
                if (fechaExistCount == 0) {
                    db.IndicadorFechas.Add(indicadorFechaVar);
                    db.SaveChanges();
                }                
                mesCount++;
               
            }

                return RedirectToAction("Index");
                //return View(indicadorFecha);
        }




        // GET: /IndicadorFecha/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorFecha indicadorFecha = db.IndicadorFechas.Find(id);
            if (indicadorFecha == null)
            {
                return HttpNotFound();
            }
            return View(indicadorFecha);
        }

        // POST: /IndicadorFecha/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,mes,cuarto,anio")] IndicadorFecha indicadorFecha)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicadorFecha).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(indicadorFecha);
        }

        // GET: /IndicadorFecha/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicadorFecha indicadorFecha = db.IndicadorFechas.Find(id);
            if (indicadorFecha == null)
            {
                return HttpNotFound();
            }
            return View(indicadorFecha);
        }

        // POST: /IndicadorFecha/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IndicadorFecha indicadorFecha = db.IndicadorFechas.Find(id);
            db.IndicadorFechas.Remove(indicadorFecha);
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
