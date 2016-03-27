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
using Monitoreo.Models.BO.ViewModels.EvaluacionAcompanamientoVm;
using Postal;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            ViewBag.ActividadAcompanamientoId = new SelectList(db.ActividadAcompanamientoes.Select(x => new { x.ID, x.TipoAcompanamiento}), "ID", "TipoAcompanamiento");
            var personalFunciones = from PersonalFuncion d in Enum.GetValues(typeof(PersonalFuncion))
            select new { ID = (int)d, Name = d.ToString() };
            ViewBag.PersonalFunciones = new SelectList(personalFunciones, "ID", "Name");


            return View();
        }

        // POST: EvaluacionAcompanamientos/Create
        [Route("EvaluacionAcompanamientos/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Titulo,TipoEvaluacionAcomp,ActividadAcompanamientoId,creadoPor,PersonalFuncion")] EvaluacionAcompanamiento evaluacionAcompanamiento)
        {
            if (ModelState.IsValid)
            {
                evaluacionAcompanamiento.creadoPor = User.Identity.Name;
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
            var personalFunciones = from PersonalFuncion d in Enum.GetValues(typeof(PersonalFuncion))
                                    select new { Id = (int)d, Name = d.ToString() };

            Array values = Enum.GetValues(typeof(PersonalFuncion));
            List<ListItem> items = new List<ListItem>(values.Length);
            foreach (var i in values)
            {
                items.Add(new ListItem
                {
                    Text = Enum.GetName(typeof(PersonalFuncion), i),
                    Value = i.ToString(),
                    Selected = evaluacionAcompanamiento.PersonalFuncion.Value.ToString() == Enum.GetName(typeof(PersonalFuncion), i).ToString()
                });
            }


            
            ViewBag.PersonalFunciones = new SelectList(items);

            return View(evaluacionAcompanamiento);
        }

        // POST: /EvaluacionAcompanamiento/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Titulo,TipoEvaluacionAcomp,ActividadAcompanamientoId,creadoPor,PersonalFuncion")] EvaluacionAcompanamiento evaluacionAcompanamiento)
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


        //id = id de la evaluacion, seeAll=1 ver todos los superCiclos
        public ActionResult EvaluacionCiclos(int id, int ? seeAll) {
            List<SuperCicloFormativo> superCiclosFormativos = new List<SuperCicloFormativo>();

            if (id == 0 || id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluacionAcompanamiento evaluacion = db.EvaluacionAcompanamientoes.Find(id);

            if (seeAll == 1)
            {
                superCiclosFormativos = db.SuperCicloFormativoes.ToList();
            }
            else {
                superCiclosFormativos = db.SuperCicloFormativoes.Where(f => f.FechaInicio < DateTime.Now)
                                                                   .Where(f => f.FechaFinalizacion > DateTime.Now)
                                                                   .OrderBy(f => f.FechaInicio.Year)
                                                                   .ThenBy(y => y.FechaInicio.Month)
                                                                   .ThenBy(y => y.FechaInicio.Day).ToList();
            }

           
           
            var dropDownCiclos = new List<SelectListItem>();            
            foreach (var superCiclo in superCiclosFormativos)
            {
                   dropDownCiclos.Add(
                    new SelectListItem
                    {
                        Selected = evaluacion.SuperCicloFormativo.Any(i => i.Id == superCiclo.Id),
                        Text = superCiclo.nombre + " - " + superCiclo.CategoriaSuperCiclo.ToString(),
                        Value = Convert.ToString(superCiclo.Id),                       
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
        public ActionResult ParticipanteRespuestas(EvaluacionAcompanamiento evaluacion)
        {
            var respuestaTemp = new EvaluacionAcompanamientoRespuesta();

            List<EvaluacionAcompanamientoRespuestasVM> evaluacionAcompanamientoRespuestas = new List<EvaluacionAcompanamientoRespuestasVM>();

            if (evaluacion.RespuestasAcomp != null)
            {
                string respuestasEvalRequest = Request.Form["respuestas"];
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
                        bool isRepeated = db.EvaluacionAcompanamientoRespuestas.Where(e => e.EvaluacionAcompId == resp.EvaluacionAcompId).Any(i => i.InscripcionActividadAcompanamientoId == resp.InscripcionActividadAcompanamientoId);
                        if (!isRepeated) // Para asegurarse que no se repitan por inscripcionId ni evaluacionID
                        {
                            db.EvaluacionAcompanamientoRespuestas.Add(resp);
                        }

                    }
                    else
                    {
                        db.Entry(resp).State = EntityState.Modified;
                    }

                    evaluacionAcompanamientoRespuestas.Add(new EvaluacionAcompanamientoRespuestasVM { pregunta = db.EvaluacionAcompanamientoPreguntas.Find(respuesta.PreguntaAcompId).Descripcion, respuesta = respuesta.Valor });
                }

                db.SaveChanges();
            }
            InscripcionActividadAcompanamiento inscripcionActividad = db.InscripcionesActividadesAcompanamiento.Find(respuestaTemp.InscripcionActividadAcompanamientoId);


            /*Enviar por Email de Verificacion*/
            //Envia por Email Verificacion
            dynamic email = new Email("EmailEvaluacionAcompanamiento");
            email.IdInscripcion = inscripcionActividad.ID;
            email.IdEvaluacion = evaluacion.Id;
            email.TipoAcompanamiento = inscripcionActividad.ActividadAcompanamiento.TipoAcompanamiento;
            email.cicloFormativo = inscripcionActividad.ActividadAcompanamiento.SuperCicloFormativo.nombre;
            email.respuestas = evaluacionAcompanamientoRespuestas;

            var persona = db.Personas.AsNoTracking().Where(p => p.Cedula == User.Identity.Name).Select(x => new { x.Cedula, x.Nombres, x.PrimerApellido, x.mail }).FirstOrDefault();
            email.acompanante = persona != null ? persona.Nombres + " " + persona.PrimerApellido : "";
            Personal personal = db.Personal.Find(inscripcionActividad.personalID);
            email.docente = personal.Persona.NombreCompleto.ToString();
            email.escuela = personal.Centro.Nombre;
            email.fecha = inscripcionActividad.fecha;
            email.horas = inscripcionActividad.horas;
            email.area = inscripcionActividad.Area.ToString();
            email.Subject = "Evaluación de Acompañamiento " + inscripcionActividad.ActividadAcompanamiento.TipoAcompanamiento + " Creada" + " Centro " + personal.Centro.Nombre;

            if (verifyEmail(persona.mail))
            {
                email.To = "sismo@stat5.com" + "," + persona.mail;
            }
            else
            {
                email.To = "sismo@stat5.com";
            }
            email.Send();
            /*End Enviar por email*/

            return RedirectToAction("DocenteDetailsAcompanante", "Docente", new { id = inscripcionActividad.personalID });

        }


        private bool verifyEmail(string email)
        {
            bool result = true;
            string emailRegex = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$";
            Regex re = new Regex(emailRegex);
            if (email == null)
            {
                return false;
            }
            if (!re.IsMatch(email))
            {
                return false;
            }
            return result;
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
