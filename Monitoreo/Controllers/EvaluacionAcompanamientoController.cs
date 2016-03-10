using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Monitoreo.Models.BO.EvaluacionAcompanamiento;
using Monitoreo.Models.DAL;
using Monitoreo.Models;
using Monitoreo.Models.BO;
using System.Text;
using Monitoreo.Helpers;
using System.Threading.Tasks;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class EvaluacionAcompanamientoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: EvaluacionAcompanamientos
        [Route("EvaluacionAcompanamientos")]
        public ActionResult Index()
        {
            var evaluacionacompanamientoes = db.EvaluacionAcompanamientoes.ToList();
            return View(evaluacionacompanamientoes.ToList());
        }

        // POST: EvaluacionAcompanamientos
        [Route("EvaluacionAcompanamientos/GetDataJson")]
        [HttpPost]
        public JsonResult GetDataJson(DatatablesParams values)
        {
            var evaluacionacompanamientoes = db.EvaluacionAcompanamientoes.Select(x => new { DT_RowId = x.Id, x.Titulo, TipoEvaluacionAcomp = x.TipoEvaluacionAcomp.ToString(), x.creadoPor });
            var recordsTotal = evaluacionacompanamientoes.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = evaluacionacompanamientoes;

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Titulo.Contains(searchValue) || x.creadoPor.Contains(searchValue)
                    );

                    recordsFiltered = data.Count();
                }
            }
          


            // Preparando respuesta y ejecutando consulta
            var jsonData = new
            {
                draw = values.raw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data.OrderBy(n => n.Titulo).Skip(from).Take(limit).ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: /EvaluacionAcompanamiento/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamiento evaluacionAcompanamiento = db.EvaluacionAcompanamientoes.Find(id);
            if (evaluacionAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(evaluacionAcompanamiento);
        }

        // GET: EvaluacionAcompanamientos/Create
        [Route("EvaluacionAcompanamientos/Create")]
        public ActionResult Create()
        {
            ViewBag.ActividadAcompanamientoId = new SelectList(db.ActividadAcompanamientoes, "ID", "TipoAcompanamiento");
            return View();
        }

        // POST: EvaluacionAcompanamientos/Create
        [Route("EvaluacionAcompanamientos/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Titulo,TipoEvaluacionAcomp,ActividadAcompanamientoId,creadoPor")] EvaluacionAcompanamiento evaluacionAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                db.EvaluacionAcompanamientoes.Add(evaluacionAcompanamiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //   ViewBag.ActividadAcompanamientoId = new SelectList(db.ActividadAcompanamientoes, "ID", "TipoAcompanamiento", evaluacionAcompanamiento.ActividadAcompanamientoId);
            return View(evaluacionAcompanamiento);
        }

        // GET: /EvaluacionAcompanamiento/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamiento evaluacionAcompanamiento = db.EvaluacionAcompanamientoes.Find(id);
            if (evaluacionAcompanamiento == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ActividadAcompanamientoId = new SelectList(db.ActividadAcompanamientoes, "ID", "ID", evaluacionAcompanamiento.ActividadAcompanamientoId);
            return View(evaluacionAcompanamiento);
        }

        // POST: /EvaluacionAcompanamiento/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Titulo,TipoEvaluacionAcomp,ActividadAcompanamientoId,creadoPor")] EvaluacionAcompanamiento evaluacionAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluacionAcompanamiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.ActividadAcompanamientoId = new SelectList(db.ActividadAcompanamientoes, "ID", "ID", evaluacionAcompanamiento.ActividadAcompanamientoId);
            return View(evaluacionAcompanamiento);
        }

        // GET: /EvaluacionAcompanamiento/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamiento evaluacionAcompanamiento = db.EvaluacionAcompanamientoes.Find(id);
            if (evaluacionAcompanamiento == null)
            {
                return HttpNotFound();
            }
            return View(evaluacionAcompanamiento);
        }

        // POST: /EvaluacionAcompanamiento/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvaluacionAcompanamiento evaluacionAcompanamiento = db.EvaluacionAcompanamientoes.Find(id);
            if (ModelState.IsValid)
            {
                db.EvaluacionAcompanamientoes.Remove(evaluacionAcompanamiento);
                db.SaveChanges();
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(evaluacionAcompanamiento);
        }



        public ActionResult EvaluacionCiclos(int id) {
            
            List<SuperCicloFormativo> superCiclosFormativos = db.SuperCicloFormativoes.ToList(); // Lista de SuperCiclos Formativos
            var dropDownCiclos = new List<SelectListItem>();

            EvaluacionAcompanamiento evaluacion =  db.EvaluacionAcompanamientoes.Find(id);
            foreach (var superCiclo in superCiclosFormativos)
            {
                   dropDownCiclos.Add(
                    new SelectListItem
                    {
                        Selected = evaluacion.SuperCicloFormativo.Any(i => i.Id == superCiclo.Id),
                        Text = superCiclo.nombre,
                        Value = Convert.ToString(superCiclo.Id)
                    }

                   );
            }

            ViewBag.SuperCiclos = dropDownCiclos;
            ViewBag.evaluacionId = id;
            return View(evaluacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvaluacionCiclos(FormCollection formCol)
        {

            string selectedValues = formCol["SelectedCiclos"];
            string evaluacionId = formCol["evaluacionId"];
            string[] selectedIds;
            int intEvaluacionId = 0;


            if (evaluacionId != "")
            {
                intEvaluacionId = Convert.ToInt32(evaluacionId);
                EvaluacionAcompanamiento evaluacion = db.EvaluacionAcompanamientoes.Find(intEvaluacionId);

                List<int> cicloIdsBefore = new List<int>();
                foreach (var superCiclo in evaluacion.SuperCicloFormativo)
                {
                    cicloIdsBefore.Add(superCiclo.Id);
                }
                
                //******** Si todos los checkboxes estan desabilitados borra todos los Ciclos Formativos asignados ********
                if (selectedValues == null)
                {
                    List<SuperCicloFormativo> superciclosEvaluacion = new List<SuperCicloFormativo>();
                    superciclosEvaluacion = evaluacion.SuperCicloFormativo;
                    foreach (var cicloId in cicloIdsBefore)
                    {
                        SuperCicloFormativo superciclo = db.SuperCicloFormativoes.Find(cicloId);
                        evaluacion.SuperCicloFormativo.Remove(superciclo);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index", "EvaluacionAcompanamiento");
                }
                //*************************************************************************************************************
                selectedIds = selectedValues.Split(',');                

                //Verifica si ha sido borrado 
              
                List<int> idsAfter = new List<int>();               
                foreach (var idCiclos in selectedIds)
                {
                    int id = Convert.ToInt32(idCiclos);
                    idsAfter.Add(id);
                }

                IEnumerable<int> idsExcluded = cicloIdsBefore.Except(idsAfter);
                //Remueve los Ciclos que han sido deschekeados
                foreach(var exId in idsExcluded){
                    int a = exId;
                    //Remover de la lista
                    SuperCicloFormativo superciclo = db.SuperCicloFormativoes.Find(exId);
                    evaluacion.SuperCicloFormativo.Remove(superciclo);
                    db.SaveChanges();
                }               

                foreach (var idCiclos in selectedIds)
                {
                    int id = Convert.ToInt32(idCiclos);                    
                    bool isRepeated = evaluacion.SuperCicloFormativo.Any(i => i.Id == id);
                    if (!isRepeated)
                    {
                        //Agregar a la lista
                        SuperCicloFormativo superciclo = db.SuperCicloFormativoes.Find(id);
                        evaluacion.SuperCicloFormativo.Add(superciclo);
                        db.SaveChanges();
                    }
                }
            }
            else {
               
                   
            }
            

            return RedirectToAction("Index","EvaluacionAcompanamiento");
        }





        [AllowAnonymous]
        public async Task<ActionResult> ParticipanteRespuestas(int evaluacionId, int participanteId, int inscripcionAcompid)
        {

            var evaluacion = db.EvaluacionAcompanamientoes.AsNoTracking().Select(x => new { x.Id, x.Titulo }).Where(e => e.Id == evaluacionId).SingleOrDefault();

            List<EvaluacionAcompanamientoRespuesta> respuestasTemp = new List<EvaluacionAcompanamientoRespuesta>();
            List<EvaluacionAcompanamientoPregunta> preguntas = new List<EvaluacionAcompanamientoPregunta>();

            if (evaluacion == null)
            {
                return HttpNotFound();
            }

            Persona participante = db.Personal.Find(participanteId).Persona;
            if (participante == null)
            {
                return HttpNotFound();
            }

            else
            {
                preguntas = await db.EvaluacionAcompanamientoPreguntas.AsNoTracking().Where(e => e.EvaluacionAcompId == evaluacionId).ToListAsync();
                respuestasTemp = await db.EvaluacionAcompanamientoRespuestas.AsNoTracking().Where(e => e.EvaluacionAcompId == evaluacionId).Where(i => i.InscripcionActividadAcompanamientoId == inscripcionAcompid).ToListAsync();
                if (respuestasTemp.Count() == 0)
                {
                    foreach (var pregunta in preguntas)
                    {
                        respuestasTemp.Add(
                            new EvaluacionAcompanamientoRespuesta
                            {
                                EvaluacionAcompId = evaluacion.Id,
                                InscripcionActividadAcompanamientoId = inscripcionAcompid,
                                PreguntaAcompId = pregunta.Id,
                                PreguntaAcomp = pregunta,
                            });
                    }
                }


                ViewBag.Evaluacion = evaluacion;
                ViewBag.ParticipanteNombre = participante.NombreCompleto;
                ViewBag.ParticipanteId = participante.Id;
                ViewBag.preguntasViewBag = preguntas.ToList();

                return PartialView(
                    new EvaluacionAcompanamiento
                    {
                        Id = evaluacion.Id,
                        RespuestasAcomp = respuestasTemp,
                    });
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Acompanante,Coordinador")]
        public async Task<ActionResult> ParticipanteRespuestas(EvaluacionAcompanamiento evaluacion)
        {
            var respuestaTemp = new EvaluacionAcompanamientoRespuesta();
            string resultado = "OK";
            StringBuilder textoEmail = new StringBuilder();
            if (evaluacion.RespuestasAcomp != null)
            {
                string respuestasEvalRequest = Request.Form["respuestas"];
                textoEmail.AppendLine("<ul>");
                foreach (var respuesta in evaluacion.RespuestasAcomp)
                {
                    var resp = new EvaluacionAcompanamientoRespuesta
                    {
                        InscripcionActividadAcompanamientoId = respuesta.InscripcionActividadAcompanamientoId,
                        EvaluacionAcompId = evaluacion.Id,
                        PreguntaAcompId = respuesta.PreguntaAcompId,
                        Id = respuesta.Id,
                        Valor = respuesta.Valor
                    };
                    respuestaTemp = resp;
                    if (respuesta.Id == 0)
                    {
                        bool isRepeated = await db.EvaluacionAcompanamientoRespuestas.Where(e => e.EvaluacionAcompId == resp.EvaluacionAcompId).AnyAsync(i => i.InscripcionActividadAcompanamientoId == resp.InscripcionActividadAcompanamientoId);
                        if (!isRepeated) // Para asegurarse que no se repitan por inscripcionId ni evaluacionID
                        {
                            db.EvaluacionAcompanamientoRespuestas.Add(resp);
                        }

                    }
                    else
                    {
                        db.Entry(resp).State = EntityState.Modified;
                    }
                    textoEmail.AppendLine("<li> Pregunta: " + db.EvaluacionAcompanamientoPreguntas.Find(respuesta.PreguntaAcompId).Descripcion + " Respuesta: " + respuesta.Valor + "</li>");

                }
                textoEmail.AppendLine("</ul>");

                await db.SaveChangesAsync();
            }
            InscripcionActividadAcompanamiento inscripcionActividad = await db.InscripcionesActividadesAcompanamiento.FindAsync(respuestaTemp.InscripcionActividadAcompanamientoId);

            //Enviar por email
            Personal personal = db.Personal.Find(inscripcionActividad.personalID);
            string tituloEmail = inscripcionActividad.ActividadAcompanamiento.TipoAcompanamiento.ToString();
            textoEmail.AppendLine("<h1>Ciclo Formativo: " + inscripcionActividad.ActividadAcompanamiento.SuperCicloFormativo.nombre + " " + inscripcionActividad.ActividadAcompanamiento.TipoAcompanamiento.ToString() + "</h1>");
            textoEmail.AppendLine("<p>" + "Nombre: " + personal.Persona.Nombres + " fecha:" + inscripcionActividad.fecha + " horas: " + inscripcionActividad.horas + " Area: " + inscripcionActividad.Area + "</p>");
            textoEmail.AppendLine("<h1>Creado Por: " + User.Identity.Name + "</h1>");
            await Logger.LogEvent(User.Identity.Name, "Evaluacion de " + tituloEmail + " Creada", textoEmail.ToString(), "", DateTime.Now);
            //End Enviar por email

            return RedirectToAction("DocenteDetailsAcompanante", "Docente", new { id = inscripcionActividad.personalID });

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
