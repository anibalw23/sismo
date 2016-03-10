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
    public class MunicipioController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Municipios
        [Route("Municipios")]
        public ActionResult Index()
        {
            //var municipios = db.Municipios.Include(m => m.Provincia);
            //return View(municipios.ToList());
            return View();
        }

        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var recordsTotal = db.Municipios.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = db.Municipios.Select(x => new { DT_RowId = x.Id, Nombre = x.Nombre, Codigo = x.Codigo, Provincia = x.Provincia.Nombre });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];

                //".Contains(searchValue) || x."

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Nombre.Contains(searchValue.Trim())
                        );

                    recordsFiltered = data.Count();
                }
            }

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
                        case "Nombre":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Nombre);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Nombre);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Nombre);
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
		
		






        // GET: /Municipio/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipio municipio = db.Municipios.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            return View(municipio);
        }

        // GET: Municipios/Create
        [Route("Municipios/Create")]
        public ActionResult Create()
        {
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre");
            return View();
        }

        // POST: Municipios/Create
        [Route("Municipios/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Codigo,Nombre,ProvinciaId")] Municipio municipio)
        {
            if (ModelState.IsValid)
            {
                db.Municipios.Add(municipio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", municipio.ProvinciaId);
            return View(municipio);
        }

        // GET: /Municipio/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipio municipio = db.Municipios.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", municipio.ProvinciaId);
            return View(municipio);
        }

        // POST: /Municipio/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Codigo,Nombre,ProvinciaId")] Municipio municipio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(municipio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", municipio.ProvinciaId);
            return View(municipio);
        }

        // GET: /Municipio/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipio municipio = db.Municipios.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            return View(municipio);
        }

        // POST: /Municipio/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Municipio municipio = db.Municipios.Find(id);

            // Seteando en null lo relacionada
            var personas = db.Personas.Where(x => x.MunicipioId == id);
            foreach (var item in personas)
            {
                item.MunicipioId = null;
                db.Entry(item).State = EntityState.Modified;
            }

            db.Municipios.Remove(municipio);
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
