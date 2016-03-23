using Monitoreo.Models.BO.ViewModels.EvaluacionAcompanamientoVm;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class EmailsController : Controller
    {
        // GET: Emails
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmailActividadAcompanamiento()
        {
            dynamic email = new Email("EmailEvaluacionAcompanamiento");
            // set up the email ...
            email.IdInscripcion = "5969";
            List<EvaluacionAcompanamientoRespuestasVM> evaluacionAcompanamientoRespuestas = new List<EvaluacionAcompanamientoRespuestasVM>();
            evaluacionAcompanamientoRespuestas.Add(new EvaluacionAcompanamientoRespuestasVM { pregunta ="Pregunta 1", respuesta = "No se" });
            email.respuestas = evaluacionAcompanamientoRespuestas;
            return new EmailViewResult(email);
        }
    }
}