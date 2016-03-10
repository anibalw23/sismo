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
using System.Threading.Tasks;

namespace Monitoreo.Controllers
{
    //[Authorize(Roles = "Administrador, Acompanante")]
    public class AusenciaReportesController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: AusenciaReportess
        [Route("AusenciaReportes")]
        public ActionResult Index()
        {
            var ausenciareportes = db.AusenciaReportes.Include(a => a.CicloFormativo).Include(a => a.Seccion);
            return View(ausenciareportes.ToList());
        }

        // POST: AusenciaReportess
        [Route("AusenciaReportes/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var ausenciareportes = db.AusenciaReportes.Include(a => a.CicloFormativo).Include(a => a.Seccion);
            var recordsTotal = ausenciareportes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = ausenciareportes.Select(x => new { DT_RowId = x.ID, x.titulo, x.CicloFormativoId, x.SeccionId, x.NumeroPersonasObjetivo, x.AsistenciaObjetivo, x.NumeroHorasObjetivo, CicloFormativo = x.CicloFormativo.Tema, Seccion = x.Seccion.Numero });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.titulo.Contains(searchValue)
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
                        case "titulo":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.titulo);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.titulo);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.titulo);
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

        // GET: /AusenciaReportes/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AusenciaReporte ausenciaReporte = db.AusenciaReportes.Find(id);
            if (ausenciaReporte == null)
            {
                return HttpNotFound();
            }
            return View(ausenciaReporte);
        }


        public ActionResult ReporteAsistenciaTotal() {
            return View();
        }

        [Route("AusenciaReportes/getReporteAsistenciaTotal")]
        [HttpGet]
        public  async Task<JsonResult> getReporteAsistenciaTotal() {

            List<AsistenciaReporteViewModel> asistenciaVMs = new List<AsistenciaReporteViewModel>();
            var personasInscritas = await db.Inscripciones.Include(p => p.Participante).Include(s => s.GrupoCicloFormativo).Include(c => c.CicloFormativo).ToListAsync();

            foreach (var a in personasInscritas)
            {
                AsistenciaReporteViewModel asistenciaVM = new AsistenciaReporteViewModel();
                asistenciaVM.ID = a.Id;
                asistenciaVM.cedula = a.Participante.Cedula;
                asistenciaVM.nombre = a.Participante.NombreCompleto;
                asistenciaVM.cicloSeccion = a.GrupoCicloFormativo.Centro.Nombre;
                asistenciaVM.cicloTema = a.CicloFormativo.Tema;
                var ausencia = db.Ausencias.Where(p => p.Persona.Cedula == asistenciaVM.cedula);
                int diasFaltados = ausencia.Count();

                var calCiclo = db.CalendarioCicloFormativoes.Where(c => c.CicloFormativoID == a.CicloFormativo.Id);
                int totalDias = await calCiclo.CountAsync();
                int totalHoras = calCiclo.AsEnumerable().Sum(h => h.horas);
                int totalHorasFaltadas = 0;

                if (totalDias != 0) {
                    asistenciaVM.totalDias = totalDias;
                    asistenciaVM.totalHoras = totalHoras;

                    // Calculo de Total de horas faltadas
                    foreach (Ausencia au in ausencia)
                    {
                        int horaAcc = calCiclo.Where(i => i.Id == au.CalendarioCicloFormativoId).SingleOrDefault().horas;
                        totalHorasFaltadas = horaAcc + totalHorasFaltadas;
                    }
                    asistenciaVM.porcentajeAsistencia = 100 * (totalDias - diasFaltados) / (totalDias);
                    asistenciaVM.horasAsistidas = totalHoras - totalHorasFaltadas;
                    asistenciaVMs.Add(asistenciaVM);
                
                }

              

            }
            var jsonData = new
            {
                Records = asistenciaVMs
            };

            return Json(new { Result = "OK", Records = asistenciaVMs }, JsonRequestBehavior.AllowGet);
        
        }


        // POST: AusenciaReportess
        [Route("AusenciaReportes/GetReporteAsistencia")]
        [HttpPost]
        public JsonResult GetReporteAsistencia(DatatablesParams values, int CicloFormativoId, int seccionId)
        {

            List<AsistenciaViewModel> asistenciaVMs = new List<AsistenciaViewModel>();

            var personasInscritas = db.Inscripciones.Where(c => c.CicloFormativoId == CicloFormativoId).Where(s => s.GrupoCicloFormativoId == seccionId).Include( p => p.Participante);
            

            var calCiclo = db.CalendarioCicloFormativoes.Where(c => c.CicloFormativoID == CicloFormativoId);
            int totalDias = calCiclo.Count();
            int totalHoras = calCiclo.AsEnumerable().Sum(h => h.horas);
            int totalHorasFaltadas = 0;
         
 
            foreach (var a in personasInscritas)
            {
                AsistenciaViewModel asistenciaVM = new AsistenciaViewModel();
                asistenciaVM.ID = a.Id;
                asistenciaVM.cedula = a.Participante.Cedula;
                asistenciaVM.cicloID = CicloFormativoId;
                var ausencia = db.Ausencias.Where(p => p.Persona.Cedula == asistenciaVM.cedula);
                int diasFaltados = ausencia.Count();

                if (totalDias != 0) {
                    // Calculo de Total de horas faltadas
                    foreach (Ausencia au in ausencia)
                    {
                        int horaAcc = calCiclo.Where(i => i.Id == au.CalendarioCicloFormativoId).SingleOrDefault().horas;
                        totalHorasFaltadas = horaAcc + totalHorasFaltadas;
                    }
                    asistenciaVM.porcentajeAsistencia = 100 * (totalDias - diasFaltados) / (totalDias);
                    asistenciaVM.horasAsistidad = totalHoras - totalHorasFaltadas;
                    asistenciaVMs.Add(asistenciaVM);
                }
            }

            var recordsTotal = asistenciaVMs.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = asistenciaVMs.Select(x => new { DT_RowId = x.ID, Cedula = x.cedula, Asistencia = x.porcentajeAsistencia, Horas = x.horasAsistidad });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Cedula.Contains(searchValue)
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
                        case "titulo":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Cedula);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Cedula);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Cedula);
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









        // GET: AusenciaReportess/Create
        [Route("AusenciaReportes/Create")]
        public ActionResult Create()
        {
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema");
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero");
            return View();
        }


        // POST: AusenciaReportess/Create
        [Route("AusenciaReportes/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,titulo,CicloFormativoId,SeccionId,NumeroPersonasObjetivo,AsistenciaObjetivo,NumeroHorasObjetivo")] AusenciaReporte ausenciaReporte)
        {
            if (ModelState.IsValid)
            {
                db.AusenciaReportes.Add(ausenciaReporte);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", ausenciaReporte.CicloFormativoId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", ausenciaReporte.SeccionId);
            return View(ausenciaReporte);
        }

        // GET: /AusenciaReportes/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AusenciaReporte ausenciaReporte = db.AusenciaReportes.Find(id);
            if (ausenciaReporte == null)
            {
                return HttpNotFound();
            }
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", ausenciaReporte.CicloFormativoId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", ausenciaReporte.SeccionId);
            return View(ausenciaReporte);
        }

        // POST: /AusenciaReportes/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,titulo,CicloFormativoId,SeccionId,NumeroPersonasObjetivo,AsistenciaObjetivo,NumeroHorasObjetivo")] AusenciaReporte ausenciaReporte)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ausenciaReporte).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CicloFormativoId = new SelectList(db.CiclosFormativos, "Id", "Tema", ausenciaReporte.CicloFormativoId);
            ViewBag.SeccionId = new SelectList(db.Secciones, "Id", "Numero", ausenciaReporte.SeccionId);
            return View(ausenciaReporte);
        }

        // GET: /AusenciaReportes/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AusenciaReporte ausenciaReporte = db.AusenciaReportes.Find(id);
            if (ausenciaReporte == null)
            {
                return HttpNotFound();
            }
            return View(ausenciaReporte);
        }

        // POST: /AusenciaReportes/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AusenciaReporte ausenciaReporte = db.AusenciaReportes.Find(id);
            db.AusenciaReportes.Remove(ausenciaReporte);
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
