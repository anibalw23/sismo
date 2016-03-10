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

using Monitoreo.Models;
namespace Monitoreo.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AusenciaCicloFormativoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: AusenciaCicloFormativos
        [Route("AusenciaCicloFormativos")]
        public ActionResult Index()
        {
            var ausenciacicloformativoes = db.AusenciaCicloFormativoes.Include(a => a.CalendarioCicloFormativo).Include(a => a.Participante);
            return View(ausenciacicloformativoes.ToList());
        }

        // POST: AusenciaCicloFormativos
        [Route("AusenciaCicloFormativos/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var ausenciacicloformativoes = db.AusenciaCicloFormativoes.Include(a => a.CalendarioCicloFormativo).Include(a => a.Participante);
            var recordsTotal = ausenciacicloformativoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = ausenciacicloformativoes.Select(x => new { DT_RowId = x.ID, x.CalendarioCicloFormativoId, Fecha = x.CalendarioCicloFormativo.Fecha.ToString(), x.ParticipanteId, Tipo = x.Tipo.ToString(), x.Comentario, ParticipanteCedula = x.Participante.Codigo  });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Comentario.Contains(searchValue)
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
                        case "Comentario":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Comentario);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Comentario);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.CalendarioCicloFormativoId);
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

        // GET: /AusenciaCicloFormativo/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AusenciaCicloFormativo ausenciaCicloFormativo = db.AusenciaCicloFormativoes.Find(id);
            if (ausenciaCicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(ausenciaCicloFormativo);
        }

        // GET: AusenciaCicloFormativos/Create
        [Route("AusenciaCicloFormativos/Create")]
        public ActionResult Create()
        {
               //ViewBag.CicloFormativoCalendario = new SelectList(CicloFormativosCalendario, "Id", "CicloFormativoCalendarioId");

            //CicloFormativo cicloFormativo = new CicloFormativo();
            //ViewBag.CiclosFormativoId = new SelectList(db.CiclosFormativos, "Id", "Id");

            //ViewBag.ActividadFormativaId = new SelectList(db.ActividadesFormativas.Where(x => x.CicloFormativo == cicloFormativo.Id), "Id", "Nombre", regional.MunicipioId);



            ViewBag.CalendarioCicloFormativoId = new SelectList(db.CalendarioCicloFormativoes, "Id", "Id");
            ViewBag.ParticipanteId = new SelectList(db.Personal, "Id", "Codigo");
            return View();
        }

        // POST: AusenciaCicloFormativos/Create
        [Route("AusenciaCicloFormativos/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,CalendarioCicloFormativoId,ParticipanteId,Tipo,Comentario")] AusenciaCicloFormativo ausenciaCicloFormativo)
        {
            if (ModelState.IsValid)
            {
                db.AusenciaCicloFormativoes.Add(ausenciaCicloFormativo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CalendarioCicloFormativoId = new SelectList(db.CalendarioCicloFormativoes, "Id", "Id", ausenciaCicloFormativo.CalendarioCicloFormativoId);
            ViewBag.ParticipanteId = new SelectList(db.Personal, "Id", "Codigo", ausenciaCicloFormativo.ParticipanteId);
            return View(ausenciaCicloFormativo);
        }

        // GET: /AusenciaCicloFormativo/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AusenciaCicloFormativo ausenciaCicloFormativo = db.AusenciaCicloFormativoes.Find(id);
            if (ausenciaCicloFormativo == null)
            {
                return HttpNotFound();
            }
            ViewBag.CalendarioCicloFormativoId = new SelectList(db.CalendarioCicloFormativoes, "Id", "Id", ausenciaCicloFormativo.CalendarioCicloFormativoId);
            ViewBag.ParticipanteId = new SelectList(db.Personal, "Id", "Codigo", ausenciaCicloFormativo.ParticipanteId);
            return View(ausenciaCicloFormativo);
        }

        // POST: /AusenciaCicloFormativo/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,CalendarioCicloFormativoId,ParticipanteId,Tipo,Comentario")] AusenciaCicloFormativo ausenciaCicloFormativo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ausenciaCicloFormativo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CalendarioCicloFormativoId = new SelectList(db.CalendarioCicloFormativoes, "Id", "Id", ausenciaCicloFormativo.CalendarioCicloFormativoId);
            ViewBag.ParticipanteId = new SelectList(db.Personal, "Id", "Codigo", ausenciaCicloFormativo.ParticipanteId);
            return View(ausenciaCicloFormativo);
        }

        // GET: /AusenciaCicloFormativo/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AusenciaCicloFormativo ausenciaCicloFormativo = db.AusenciaCicloFormativoes.Find(id);
            if (ausenciaCicloFormativo == null)
            {
                return HttpNotFound();
            }
            return View(ausenciaCicloFormativo);
        }

        // POST: /AusenciaCicloFormativo/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AusenciaCicloFormativo ausenciaCicloFormativo = db.AusenciaCicloFormativoes.Find(id);
            db.AusenciaCicloFormativoes.Remove(ausenciaCicloFormativo);
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
