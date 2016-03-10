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
    public class IndicadorResultadoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: IndicadorResultados
        [Route("IndicadorResultados")]
        public ActionResult Index()
        {
            var indicadorresultadoes = db.indicadorResultadoes.Include(i => i.Indicador).Include(i => i.IndicadorDesagregacion).Include(i => i.IndicadorFecha);
            return View(indicadorresultadoes.ToList());
        }

        // POST: IndicadorResultados
        [Route("IndicadorResultados/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var indicadorresultadoes = db.indicadorResultadoes.Include(i => i.Indicador).Include(i => i.IndicadorDesagregacion).Include(i => i.IndicadorFecha);
            var recordsTotal = indicadorresultadoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = indicadorresultadoes.Select(x => new { DT_RowId = x.ID, x.valor, x.valorEsperado, x.IndicadorID, x.IndicadorFechaID, x.IndicadorDesagregacionID });

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
                data = data.OrderBy(s => s.valor);
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

        // GET: /IndicadorResultado/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            indicadorResultado indicadorResultado = db.indicadorResultadoes.Find(id);
            if (indicadorResultado == null)
            {
                return HttpNotFound();
            }
            return View(indicadorResultado);
        }

        // GET: IndicadorResultados/Create
        [Route("IndicadorResultados/Create")]
        public ActionResult Create()
        {
            ViewBag.IndicadorID = new SelectList(db.Indicadors, "ID", "nombre");
            ViewBag.IndicadorDesagregacionID = new SelectList(db.IndicadorDesagregacions, "Id", "nombre");
            ViewBag.IndicadorFechaID = new SelectList(db.IndicadorFechas, "anio", "Id");
            return View();
        }

        // POST: IndicadorResultados/Create
        [Route("IndicadorResultados/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,valor,valorEsperado,IndicadorID,IndicadorFechaID,IndicadorDesagregacionID")] indicadorResultado indicadorResultado)
        {
            if (ModelState.IsValid)
            {
                db.indicadorResultadoes.Add(indicadorResultado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IndicadorID = new SelectList(db.Indicadors, "ID", "nombre", indicadorResultado.IndicadorID);
            ViewBag.IndicadorDesagregacionID = new SelectList(db.IndicadorDesagregacions, "Id", "nombre", indicadorResultado.IndicadorDesagregacionID);
            ViewBag.IndicadorFechaID = new SelectList(db.IndicadorFechas, "Id", "Id", indicadorResultado.IndicadorFechaID);
            return View(indicadorResultado);
        }

        // GET: /IndicadorResultado/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            indicadorResultado indicadorResultado = db.indicadorResultadoes.Find(id);
            if (indicadorResultado == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndicadorID = new SelectList(db.Indicadors, "ID", "nombre", indicadorResultado.IndicadorID);
            ViewBag.IndicadorDesagregacionID = new SelectList(db.IndicadorDesagregacions, "Id", "nombre", indicadorResultado.IndicadorDesagregacionID);
            ViewBag.IndicadorFechaID = new SelectList(db.IndicadorFechas, "Id", "Id", indicadorResultado.IndicadorFechaID);
            return View(indicadorResultado);
        }

        // POST: /IndicadorResultado/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,valor,valorEsperado,IndicadorID,IndicadorFechaID,IndicadorDesagregacionID")] indicadorResultado indicadorResultado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicadorResultado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IndicadorID = new SelectList(db.Indicadors, "ID", "nombre", indicadorResultado.IndicadorID);
            ViewBag.IndicadorDesagregacionID = new SelectList(db.IndicadorDesagregacions, "Id", "nombre", indicadorResultado.IndicadorDesagregacionID);
            ViewBag.IndicadorFechaID = new SelectList(db.IndicadorFechas, "Id", "Id", indicadorResultado.IndicadorFechaID);
            return View(indicadorResultado);
        }

        // GET: /IndicadorResultado/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            indicadorResultado indicadorResultado = db.indicadorResultadoes.Find(id);
            if (indicadorResultado == null)
            {
                return HttpNotFound();
            }
            return View(indicadorResultado);
        }

        // POST: /IndicadorResultado/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            indicadorResultado indicadorResultado = db.indicadorResultadoes.Find(id);
            db.indicadorResultadoes.Remove(indicadorResultado);
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
