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
using System.Web.UI;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class DistritoController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Distritos
        [Route("Distritos")]
        [OutputCache(Duration = 300, Location = OutputCacheLocation.Server)] // 5 minutos
        public ActionResult Index()
        {
            return View();
        }

        // POST: Distritos
        [Route("Distritos/GetDataJson")]
        [HttpPost]
        public ActionResult GetDataJson(DatatablesParams values)
        {
            var distritos = db.Distritos.Select(x => new { DT_RowId = x.Id, x.Codigo, CentroSede = x.CentroSede.Nombre, x.Nombre, Regional = x.Regional.Nombre, Provincia = x.Provincia.Nombre, Municipio = x.Municipio.Nombre, x.Sector, x.Calle, x.Telefono, x.CorreoElectronico, x.SitioWeb });
            var recordsTotal = distritos.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = distritos;

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x =>
                        x.Codigo.Contains(searchValue) || x.Nombre.Contains(searchValue) || x.Sector.Contains(searchValue) || x.Calle.Contains(searchValue) || x.CorreoElectronico.Contains(searchValue) || x.SitioWeb.Contains(searchValue)
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
                data = data.OrderBy(n => n.Nombre).Skip(from).Take(limit).ToList()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: /Distrito/5/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distrito distrito = db.Distritos.Find(id);
            if (distrito == null)
            {
                return HttpNotFound();
            }
            return View(distrito);
        }

        // GET: Distritos/Create
        [Route("Distritos/Create")]
        public ActionResult Create()
        {
            ViewBag.CentroSedeId = new SelectList(db.Centros.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
            ViewBag.MunicipioId = new SelectList(db.Municipios.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre", 0);

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Value = "0", Text = "Seleccionar...", Selected = true });
            items.AddRange(db.Provincias.ToList().Select(x => new SelectListItem { Value = Convert.ToString(x.Id), Text = x.Nombre }));
            ViewBag.ProvinciaId = items;

            ViewBag.RegionalId = new SelectList(db.Regionales.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre");
            return View();
        }

        // POST: Distritos/Create
        [Route("Distritos/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Codigo,CentroSedeId,Nombre,RegionalId,ProvinciaId,MunicipioId,Sector,Calle,Telefono,CorreoElectronico,SitioWeb")] Distrito distrito)
        {
            if (ModelState.IsValid)
            {
                db.Distritos.Add(distrito);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CentroSedeId = new SelectList(db.Centros, "Id", "Nombre", distrito.CentroSedeId);
            ViewBag.MunicipioId = new SelectList(db.Municipios, "Id", "Nombre", distrito.MunicipioId);

            List<SelectListItem> items = new List<SelectListItem>();
            items.AddRange(db.Provincias.ToList().Select(x => new SelectListItem { Value = Convert.ToString(x.Id), Text = x.Nombre, Selected = x.Id == distrito.ProvinciaId }));
            ViewBag.ProvinciaId = items;

            ViewBag.RegionalId = new SelectList(db.Regionales, "Id", "Nombre", distrito.RegionalId);
            return View(distrito);
        }

        // GET: /Distrito/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distrito distrito = db.Distritos.Find(id);
            if (distrito == null)
            {
                return HttpNotFound();
            }
            ViewBag.CentroSede = new SelectList(db.Centros, "Id", "Nombre", distrito.CentroSedeId);
            ViewBag.Municipio = new SelectList(db.Municipios, "Id", "Nombre", distrito.MunicipioId);

            List<SelectListItem> items = new List<SelectListItem>();
            items.AddRange(db.Provincias.ToList().Select(x => new SelectListItem { Value = Convert.ToString(x.Id), Text = x.Nombre, Selected = x.Id == distrito.ProvinciaId }));
            ViewBag.Provincia = items;

            ViewBag.Regional = new SelectList(db.Regionales, "Id", "Nombre", distrito.RegionalId);
            return View(distrito);
        }

        // POST: /Distrito/5/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Codigo,CentroSedeId,Nombre,RegionalId,ProvinciaId,MunicipioId,Sector,Calle,Telefono,CorreoElectronico,SitioWeb")] Distrito distrito)
        {
            if (ModelState.IsValid)
            {
                db.Entry(distrito).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CentroSede = new SelectList(db.Centros.Select(x => new {x.Id, x.Nombre }), "Id", "Nombre", distrito.CentroSedeId);
            ViewBag.Municipio = new SelectList(db.Municipios.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre", distrito.MunicipioId);

            List<SelectListItem> items = new List<SelectListItem>();
            items.AddRange(db.Provincias.ToList().Select(x => new SelectListItem { Value = Convert.ToString(x.Id), Text = x.Nombre, Selected = x.Id == distrito.ProvinciaId }));
            ViewBag.Provincia = items;

            ViewBag.Regional = new SelectList(db.Regionales.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre", distrito.RegionalId);
            return View(distrito);
        }

        // GET: /Distrito/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distrito distrito = db.Distritos.Find(id);
            if (distrito == null)
            {
                return HttpNotFound();
            }
            return View(distrito);
        }

        // POST: /Distrito/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Distrito distrito = db.Distritos.Find(id);

            if (db.Redes.Where(x => x.DistritoId == id).Count() > 0)
                ModelState.AddModelError("error", "Este distrito contiene redes relacionadas. Favor borrarlas y volver a intentar.");

            if (db.PersonalAdministrativo.Where(x => x.DistritoId == id).Count() > 0)
                ModelState.AddModelError("error", "Este distrito contiene personal relacionados. Favor borrarlos y volver a intentar.");

            if (ModelState.IsValid)
            {
                db.Distritos.Remove(distrito);
                db.SaveChanges();
            }


            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(distrito);
        }



        [Route("Distrito/getPersonalAdmin")]
        [HttpGet]
        [OutputCache(Duration = 300, Location = OutputCacheLocation.Server)] //5 minutos
        public JsonResult getPersonalAdmin()
        {

            List<PersonalCentroViewModel> pcs = new List<PersonalCentroViewModel>();

            var personal = db.Personal.Include(c => c.Persona).Where(p => p.FuncionesEjerce != PersonalFuncion.Docente).ToList();
           
            foreach (var d in personal)
            {
                PersonalCentroViewModel pc = new PersonalCentroViewModel();
                pc.centro = d.Centro.Nombre;
                pc.cedula = d.Persona.Cedula;
                pc.nombres = d.Persona.NombreCompleto;
                pc.sexo = d.Persona.Sexo.ToString();
                pc.funcionEjerce = d.FuncionesEjerce.ToString();
                pc.tanda = d.Tanda.ToString();
                pcs.Add(pc);
            }

            var jsonData = new
            {
                Records = pcs
            };

            return Json(new { Result = "OK", Records = pcs }, JsonRequestBehavior.AllowGet);

        }




        [Route("Distrito/getPersonalAdminByCentro")]
        [HttpGet]
        [OutputCache(Duration = 300, Location = OutputCacheLocation.Server, VaryByParam = "centroID")] //5 minutos
        public JsonResult getPersonalAdminByCentro(int centroID)
        {

            List<PersonalCentroViewModel> pcs = new List<PersonalCentroViewModel>();

            if (centroID == 0) {
                return Json(new { Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest), Records = pcs }, JsonRequestBehavior.AllowGet);
            }

            var personal = db.Personal.Include(c => c.Persona).Where(p => p.FuncionesEjerce != PersonalFuncion.Docente).Where(c => c.CentroId == centroID).ToList();

            foreach (var d in personal)
            {
                PersonalCentroViewModel pc = new PersonalCentroViewModel();
                pc.centro = d.Centro.Nombre;
                pc.cedula = d.Persona.Cedula;
                pc.nombres = d.Persona.NombreCompleto;
                pc.sexo = d.Persona.Sexo.ToString();
                pc.funcionEjerce = d.FuncionesEjerce.ToString();
                pc.tanda = d.Tanda.ToString();
                pcs.Add(pc);
            }

            var jsonData = new
            {
                Records = pcs
            };

            return Json(new { Result = "OK", Records = pcs }, JsonRequestBehavior.AllowGet);

        }






        [Route("Distrito/getPersonalAdminByRed")]
        [HttpGet]
        [OutputCache(Duration = 300, Location = OutputCacheLocation.Server, VaryByParam = "redID")] //5 minutos
        public JsonResult getPersonalAdminByRed(int redID)
        {

            List<PersonalCentroViewModel> pcs = new List<PersonalCentroViewModel>();

            var personal = db.Personal.Include(c => c.Persona).Where(p => p.FuncionesEjerce != PersonalFuncion.Docente).Where(c => c.Centro.RedId == redID).ToList();

            foreach (var d in personal)
            {
                PersonalCentroViewModel pc = new PersonalCentroViewModel();
                //pc.ID = d.Id;
                //pc.red = d.Centro.Red.Nombre;
                //pc.distrito = d.Centro.Red.Distrito.Nombre;
                pc.centro = d.Centro.Nombre;
                pc.cedula = d.Persona.Cedula;
                pc.nombres = d.Persona.NombreCompleto;
                pc.sexo = d.Persona.Sexo.ToString();
                pc.funcionEjerce = d.FuncionesEjerce.ToString();
                pc.tanda = d.Tanda.ToString();
                pc.telefono = d.Persona.Telefono.Celular;
                pcs.Add(pc);
            }

            var jsonData = new
            {
                Records = pcs
            };

            return Json(new { Result = "OK", Records = pcs }, JsonRequestBehavior.AllowGet);

        }


        [Route("Distrito/getPersonalDocente")]
        [HttpGet]
         public JsonResult getPersonalDocente()
        {

            List<PersonalCentroViewModel> pcs = new List<PersonalCentroViewModel>();

            var docentes = db.Docentes.Include(p => p.Persona).Where(c => c.CentroId == 1).ToList();

            foreach (var d in docentes)
            {
                PersonalCentroViewModel pc = new PersonalCentroViewModel();
                //pc.ID = d.Id;
                //pc.red = d.Centro.Red.Nombre;
                //pc.distrito = d.Centro.Red.Distrito.Nombre;
                pc.centro = d.Centro.Nombre;
                pc.cedula = d.Persona.Cedula;
                pc.nombres = d.Persona.NombreCompleto;
                pc.sexo = d.Persona.Sexo.ToString();
                pc.tipo = d.FuncionesEjerce.ToString();
                pc.tanda = d.Tanda.ToString();
                foreach (var materia in d.Materias)
                {
                    pc.area = materia.Area.ToString();
                    pc.nivel = materia.Nivel.ToString();
                    pc.ciclo = materia.Ciclo.ToString();
                    pc.grado = materia.Grados.ToString();
                    pc.seccion = materia.Seccion.Numero;
                    pcs.Add(pc);
                }
            }

            var jsonData = new
            {
                Records = pcs
            };

            return Json(new { Result = "OK", Records = pcs }, JsonRequestBehavior.AllowGet);

        }




        [Route("Distrito/getPersonalDocenteByCentro")]
        [HttpGet]
        [OutputCache(Duration = 300, Location = OutputCacheLocation.Server, VaryByParam = "centroID")] //5 minutos
        public JsonResult getPersonalDocenteByCentro(int centroID)
        {
            List<PersonalCentroViewModel> pcs = new List<PersonalCentroViewModel>();
            var docentes = db.Docentes.Include(p => p.Persona).Where(c => c.CentroId == centroID).ToList();
            foreach (var d in docentes)
            {

                //pc.red = d.Centro.Red.Nombre.ToString().Trim(); 
                //pc.distrito = d.Centro.Red.Distrito.Nombre.ToString().Trim(); 

                foreach (var materia in d.Materias)
                {
                    PersonalCentroViewModel pc = new PersonalCentroViewModel();
                    //pc.ID = 0;//d.Id;
                    pc.centro = d.Centro.Nombre.ToString().Trim();
                    pc.cedula = d.Persona.Cedula.ToString().Trim();
                    pc.nombres = d.Persona.NombreCompleto.ToString().Trim();
                    pc.sexo = d.Persona.Sexo.ToString().Trim();
                    pc.tipo = d.FuncionesEjerce.ToString().Trim();
                    pc.tanda = d.Tanda.ToString().Trim();
                    pc.formacionAcademica = d.FormacionAcademica.ToString();
                    pc.area = materia.Area.ToString().Trim();
                    pc.nivel = materia.Nivel.ToString().Trim();
                    pc.ciclo = materia.Ciclo.ToString().Trim();
                    pc.grado = materia.Grados.ToString();
                    pc.seccion = materia.Seccion.Numero.ToString().Trim(); ;
                    pcs.Add(pc);
                }
            }

            var jsonData = new
            {
                Records = pcs
            };

            return Json(new { Result = "OK", Records = pcs }, JsonRequestBehavior.AllowGet);

        }



        [Route("Distrito/getPersonalDocenteByRed")]
        [HttpGet]
        [OutputCache(Duration = 300, Location = OutputCacheLocation.Server, VaryByParam = "redID")] //5 minutos
        public JsonResult getPersonalDocenteByRed(int redID)
        {
            List<PersonalCentroViewModel> pcs = new List<PersonalCentroViewModel>();
            var docentes = db.Docentes.Include(p => p.Persona).Where(c => c.Centro.RedId == redID).ToList();
            foreach (var d in docentes)
            {

                //pc.red = d.Centro.Red.Nombre.ToString().Trim(); 
                //pc.distrito = d.Centro.Red.Distrito.Nombre.ToString().Trim(); 

                foreach (var materia in d.Materias)
                {
                    PersonalCentroViewModel pc = new PersonalCentroViewModel();
                    //pc.ID = 0;//d.Id;
                    pc.centro = d.Centro.Nombre.ToString().Trim();
                    pc.cedula = d.Persona.Cedula.ToString().Trim();
                    pc.nombres = d.Persona.NombreCompleto.ToString().Trim();
                    pc.sexo = d.Persona.Sexo != null ? d.Persona.Sexo.ToString().Trim() : "";
                    pc.tipo = d.FuncionesEjerce.ToString().Trim();
                    pc.tanda = materia.Tanda.ToString().Trim();
                    pc.formacionAcademica = d.FormacionAcademica.ToString();

                    pc.area = materia.Area.ToString().Trim();
                    pc.nivel = materia.Nivel.ToString().Trim();
                    pc.ciclo = materia.Ciclo.ToString().Trim();
                    pc.grado = materia.Grados.ToString();
                    pc.seccion = materia.Seccion != null ? materia.Seccion.Numero.ToString().Trim() : "";
                    pcs.Add(pc);
                }
            }

            var jsonData = new
            {
                Records = pcs
            };

            return Json(new { Result = "OK", Records = pcs }, JsonRequestBehavior.AllowGet);

        }




        [HttpGet]
        [OutputCache(Duration = 300, Location = OutputCacheLocation.Server, VaryByParam = "redID")] //5 minutos
        public JsonResult getDocentesPorGradoByRed(int redID)
        {
            List<PersonalCentroViewModel> pcs = new List<PersonalCentroViewModel>();
            List<PersonalCentroViewModel> pcsFinal = new List<PersonalCentroViewModel>();
            List<PersonalCentroViewModel> pcsFinalRemoved = new List<PersonalCentroViewModel>();
            if (redID == null) {
                redID = 0;
            }


            var docentes = db.Docentes.Include(p => p.Persona).Where(c => c.Centro.RedId == redID).ToList();
            foreach (var d in docentes)
            {

                //pc.red = d.Centro.Red.Nombre.ToString().Trim(); 
                //pc.distrito = d.Centro.Red.Distrito.Nombre.ToString().Trim(); 

                foreach (var materia in d.Materias)
                {
                    PersonalCentroViewModel pc = new PersonalCentroViewModel();
                    //pc.ID = 0;//d.Id;
                    pc.centro = d.Centro.Nombre.ToString().Trim();
                    pc.cedula = d.Persona.Cedula.ToString().Trim();
                    pc.nombres = d.Persona.NombreCompleto.ToString().Trim();
                    pc.sexo = d.Persona.Sexo != null ? d.Persona.Sexo.ToString().Trim() : "";
                    pc.tipo = d.FuncionesEjerce.ToString().Trim();
                    pc.tanda = materia.Tanda.ToString().Trim();

                    pc.area = materia.Area.ToString().Trim();
                    pc.nivel = materia.Nivel.ToString().Trim();
                    pc.ciclo = materia.Ciclo.ToString().Trim();
                    pc.grado = materia.Grados.ToString();
                    pc.seccion = materia.Seccion.Numero.ToString().Trim();

                    //Para obviar las secciones, no contar si se repite el grado por profesor
                    bool isRepeatedGrado = pcs.Where(c => c.cedula == pc.cedula).Where(t => t.tanda == pc.tanda).Any(g => g.grado == pc.grado);
                    if (!isRepeatedGrado)
                    {
                        pcs.Add(pc);
                    }
                }
            }

            //Algoritmo de eliminar si tiene mas de un grado, haciendo el conteo de grados lo mas uniforme posible
            int samePersonCont = 1;
            int primeroCount = 0;// pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Primero.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Primero.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Primero.ToString()).Count();
            int segundoCount = 0;//pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Segundo.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Segundo.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Segundo.ToString()).Count();
            int terceroCount = 0;//pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Tercero.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Tercero.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Tercero.ToString()).Count();
            int cuartoCount = 0;//pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Cuarto.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Cuarto.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Cuarto.ToString()).Count();
            int quintoCount = 0;//pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Quinto.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Quinto.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Quinto.ToString()).Count();
            int sextoCount = 0;//pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Sexto.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Sexto.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Sexto.ToString()).Count();
            int septimoCount = 0;//pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Septimo.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Septimo.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Septimo.ToString()).Count();
            int octavoCount = 0;//pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Octavo.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.Octavo.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Octavo.ToString()).Count();
            int inicialCount = 0;//pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.NivelInicial.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == mat.tanda).Where(g => g.grado == Grado.NivelInicial.ToString()).Count() + pcsFinal.Where(h => h.tanda == mat.tanda).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.NivelInicial.ToString()).Count();

            foreach (var mat in pcs)
            {

                Dictionary<string, int> gradoArr = new Dictionary<string, int>();

                int personaCount = pcs.Where(p => p.cedula == mat.cedula).Count();

                List<string> tandas = new List<string>();
                tandas = pcs.Where(p => p.cedula == mat.cedula).Select(t => t.tanda).Distinct().ToList();
                int tandaPersonaCount = tandas.Count();
                //Si tiene mas de 2 tandas debe contarse como otro docente

                if (personaCount > 1)
                {
                    if (samePersonCont == personaCount)
                    {
                        //algoritmo de decisión

                        if (tandaPersonaCount > 0)
                        {
                            var i = 0;

                            foreach (var t in tandas)
                            {
                                samePersonCont = 0;
                                primeroCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Primero.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Primero.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Primero.ToString()).Count();
                                segundoCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Segundo.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Segundo.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Segundo.ToString()).Count();
                                terceroCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Tercero.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Tercero.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Tercero.ToString()).Count();
                                cuartoCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Cuarto.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Cuarto.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Cuarto.ToString()).Count();
                                quintoCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Quinto.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Quinto.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Quinto.ToString()).Count();
                                sextoCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Sexto.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Sexto.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Sexto.ToString()).Count();
                                septimoCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Septimo.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Septimo.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Septimo.ToString()).Count();
                                octavoCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Octavo.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.Octavo.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.Octavo.ToString()).Count();
                                inicialCount = pcs.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.NivelInicial.ToString()).Count() - pcsFinalRemoved.Where(c => c.centro == mat.centro).Where(h => h.tanda == t).Where(g => g.grado == Grado.NivelInicial.ToString()).Count() + pcsFinal.Where(h => h.tanda == t).Where(c => c.centro == mat.centro).Where(g => g.grado == Grado.NivelInicial.ToString()).Count();

                                List<string> gradosPersona = new List<string>();
                                gradosPersona = pcs.Where(c => c.cedula == mat.cedula).Where(h => h.tanda == t).Select(g => g.grado).ToList();
                                foreach (var grade in gradosPersona)
                                {
                                    if (grade == Grado.Primero.ToString())
                                    {
                                        gradoArr.Add(Grado.Primero.ToString(), primeroCount);
                                    }
                                    if (grade == Grado.Segundo.ToString())
                                    {
                                        gradoArr.Add(Grado.Segundo.ToString(), segundoCount);
                                    }
                                    if (grade == Grado.Tercero.ToString())
                                    {
                                        gradoArr.Add(Grado.Tercero.ToString(), terceroCount);
                                    }
                                    if (grade == Grado.Cuarto.ToString())
                                    {
                                        gradoArr.Add(Grado.Cuarto.ToString(), cuartoCount);
                                    }
                                    if (grade == Grado.Quinto.ToString())
                                    {
                                        gradoArr.Add(Grado.Quinto.ToString(), quintoCount);
                                    }
                                    if (grade == Grado.Sexto.ToString())
                                    {
                                        gradoArr.Add(Grado.Sexto.ToString(), sextoCount);
                                    }
                                    if (grade == Grado.Septimo.ToString())
                                    {
                                        gradoArr.Add(Grado.Septimo.ToString(), septimoCount);
                                    }
                                    if (grade == Grado.Octavo.ToString())
                                    {
                                        gradoArr.Add(Grado.Octavo.ToString(), octavoCount);
                                    }
                                    if (grade == Grado.NivelInicial.ToString())
                                    {
                                        gradoArr.Add(Grado.NivelInicial.ToString(), inicialCount);
                                    }
                                }

                                if (gradoArr.Count > 0)
                                {
                                    int minValue = gradoArr.OrderBy(kvp => kvp.Value).First().Value;
                                    List<string> vals = gradoArr.OrderBy(kvp => kvp.Value).Select(v => v.Key).ToList();
                                    string choosenGrado = gradoArr.FirstOrDefault(x => x.Value == minValue).Key;

                                    foreach (var b in vals)
                                    {
                                        if (choosenGrado != b)
                                        {
                                            PersonalCentroViewModel pcsTempRemoved = new PersonalCentroViewModel();
                                            pcsTempRemoved = pcs.Where(c => c.cedula == mat.cedula).Where(g => g.grado == b).Where(x => x.tanda == t).SingleOrDefault();
                                            pcsFinalRemoved.Add(pcsTempRemoved);
                                        }
                                    }

                                    PersonalCentroViewModel pcsTemp = pcs.Where(c => c.cedula == mat.cedula).Where(g => g.grado == choosenGrado).Where(x => x.tanda == t).SingleOrDefault();
                                    pcsFinal.Add(pcsTemp);
                                    gradoArr.Clear();
                                }
                            }
                        }

                    }
                    samePersonCont++;
                }
                else
                {
                    pcsFinal.Add(mat);
                }
            }




            // pcsFinal = pcs.

            var jsonData = new
            {
                Records = pcsFinal.Select(u => new
                {
                    centro = u.centro,
                    cedula = u.cedula,
                    nombres = u.nombres,
                    grado = u.grado

                })
            };

            return Json(new { Result = "OK", Records = pcsFinal }, JsonRequestBehavior.AllowGet);

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
