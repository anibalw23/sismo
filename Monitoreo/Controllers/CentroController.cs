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
using Monitoreo.Models.BO.ViewModels;
using System.Threading.Tasks;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class CentroController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Centros
        [Authorize(Roles = "Administrador, Acompanante,Coordinador")]
        [Route("Centros")]
        public ActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Administrador, Acompanante")]
        [HttpPost]
        public async Task<JsonResult> GetDataJson(DatatablesParams values)
        {
            // Seleccionando
            var centros = await db.Centros.AsNoTracking().Include(i => i.Director.Persona.Nombres).Include(r => r.Red.Nombre).Select(x => new {DT_RowId = x.Id, x.Codigo, x.Nombre, Director = (x.Director != null ? x.Director.Persona.Nombres +" "+ x.Director.Persona.PrimerApellido + " " + x.Director.Persona.SegundoApellido  : ""), Red = x.Red.Nombre }).ToListAsync();

            var data = centros; // db.Centros.ToList().Select(x => new { DT_RowId = x.Id, Nombre = x.Nombre, Codigo = x.Codigo, Director = (x.Director != null ? x.Director.Persona.NombreCompleto : ""), Red = x.Red.Nombre }).OrderBy(n => n.Nombre);
            var recordsTotal = data.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

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

        // GET: RedCentros
        //[Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult RedCentros(int RedId)
        {
            var centros = db.Centros.AsNoTracking().Include(c => c.Director).Include(c => c.Red).Where(s => s.RedId == RedId);

            ViewBag.MasterType = "Red";
            ViewBag.MasterId = RedId;

            return PartialView(centros.ToList());
        }

        //[Authorize(Roles = "Administrador, Acompanante")]
        public  ActionResult DistritoCentros(int DistritoId)
        {
            var centros =  db.Centros.AsNoTracking().Include(c => c.Director).Include(c => c.Red).Where(s => s.Red.DistritoId == DistritoId);

            ViewBag.MasterType = "Distrito";
            ViewBag.MasterId = DistritoId;

            return PartialView(centros.ToList());
        }


        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante,Coordinador, AdministradorTransversal")]
        public JsonResult GetCentrosByRedesIds(int[] ints)
        {

            if (ints == null)
            {
                var jsonNoRecords = new
                {
                    data = ""
                };
                return Json(jsonNoRecords, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var centros = db.Centros.Select(x => new { x.RedId, x.Id, x.Nombre }).Where(d => ints.Contains(d.RedId)).ToList();
                var centrosOutList = new List<Centro>();

                foreach (var centro in centros)
                {
                    centrosOutList.Add(new Centro { Id = centro.Id, RedId = centro.RedId, Nombre = centro.Nombre });
                    //centrosOutList.Add(centro);
                }
                var jsonData = new
                {
                    data = centrosOutList.Select(y => new
                    {
                        y.Id,
                        y.Nombre

                    }),
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }


        }







        // GET: /Centro/5/Details
        //[Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Centro centro = db.Centros.Find(id);
            if (centro == null)
            {
                return HttpNotFound();
            }
            return View(centro);
        }

        // GET: Centros/Create
        [Authorize(Roles = "Administrador")]
        [Route("Centros/Create")]       
        public ActionResult Create()
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            switch ((string)ViewBag.MasterType)
            {
                case "Red":
                    ViewBag.RedId = new SelectList(db.Redes.Select(x => new { x.Id, x.Nombre}), "Id", "Nombre", int.Parse(ViewBag.MasterId));
                    break;
                default:
                    ViewBag.RedId = new SelectList(db.Redes.Select(x => new { x.Id, x.Nombre }), "Id", "Nombre");
                    break;
            }

            ViewBag.DirectorId = new SelectList(db.PersonalAdministrativo.ToList().Select(x => new { x.Id, x.Persona.NombreCompleto }), "Id", "NombreCompleto");

            List<SelectListItem> items = new List<SelectListItem>();
            //items.Add(new SelectListItem { Text = "Seleccionar...", Selected = true });
            items.AddRange(db.Provincias.ToList().Select(x => new SelectListItem { Value = Convert.ToString(x.Id), Text = x.Nombre }));
            ViewBag.ProvinciaId = items;
            ViewBag.MunicipioId = new SelectList(db.Municipios, "Id", "Nombre");
            ViewBag.ComponenteId = new SelectList(db.Componentes, "Id", "Descripcion");
            
            return View();
        }

        // POST: Centros/Create
        [Route("Centros/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create(Centro centro)
        {
            ViewBag.MasterType = Request.Params["MasterType"];
            ViewBag.MasterId = Request.Params["MasterId"];

            if (ModelState.IsValid)
            {
                db.Centros.Add(centro);
                db.SaveChanges();

                if (ViewBag.MasterType != null)
                {
                    return RedirectToAction("Details", ViewBag.MasterType, new { id = ViewBag.MasterId });
                }

                return RedirectToAction("Index");
            }

            ViewBag.DirectorId = new SelectList(db.PersonalAdministrativo.ToList().Select(x => new { x.Id, x.Persona.NombreCompleto }).OrderBy(x => x.NombreCompleto), "Id", "NombreCompleto", centro.DirectorId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(x => x.ProvinciaId == centro.ProvinciaId).OrderBy(x => x.Nombre), "Id", "Nombre", centro.MunicipioId);
            ViewBag.ProvinciaId = new SelectList(db.Provincias.OrderBy(x => x.Nombre), "Id", "Nombre", centro.ProvinciaId);
            ViewBag.RedId = new SelectList(db.Redes, "Id", "Nombre", centro.RedId);
            ViewBag.ComponenteId = new SelectList(db.Componentes, "Id", "Descripcion", centro.Componente);

            return View(centro);
        }

        // GET: /Centro/5/Edit
        [Authorize(Roles = "Administrador")]       
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Centro centro = db.Centros.Find(id);
            if (centro == null)
            {
                return HttpNotFound();
            }
            ViewBag.Director = new SelectList(db.PersonalAdministrativo.ToList().Select(x => new { x.Id, NombreCompleto = x.Persona.Nombres }), "Id", "NombreCompleto", centro.DirectorId);
            ViewBag.Municipio = new SelectList(db.Municipios.Where(x => x.ProvinciaId == centro.ProvinciaId).OrderBy(x => x.Nombre), "Id", "Nombre", centro.MunicipioId);
            ViewBag.Provincia = new SelectList(db.Provincias.OrderBy(x => x.Nombre), "Id", "Nombre", centro.ProvinciaId);
            ViewBag.Red = new SelectList(db.Redes.Select(r => new { r.Id, r.Nombre}).OrderBy(x => x.Nombre), "Id", "Nombre", centro.RedId);
            ViewBag.Componente = new SelectList(db.Componentes, "Id", "Descripcion", centro.Componente);

            return View(centro);
        }

        // POST: /Centro/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Centro centro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(centro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Director = new SelectList(db.PersonalAdministrativo.ToList().Select(x => new { x.Id, x.Persona.NombreCompleto }).OrderBy(x => x.NombreCompleto), "Id", "NombreCompleto", centro.DirectorId);
            ViewBag.Municipio = new SelectList(db.Municipios.Where(x => x.ProvinciaId == centro.ProvinciaId).OrderBy(x => x.Nombre), "Id", "Nombre", centro.MunicipioId);
            ViewBag.Provincia = new SelectList(db.Provincias.OrderBy(x => x.Nombre), "Id", "Nombre", centro.ProvinciaId);
            ViewBag.Red = new SelectList(db.Redes.OrderBy(x => x.Nombre), "Id", "Nombre", centro.RedId);
            ViewBag.Componente = new SelectList(db.Componentes, "Id", "Descripcion", centro.Componente);

            return View(centro);
        }
        [Authorize(Roles = "Administrador")]
        // GET: /Centro/5/Delete
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Centro centro = db.Centros.Find(id);
            if (centro == null)
            {
                return HttpNotFound();
            }
            return View(centro);
        }

        // POST: /Centro/5/Delete
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Centro centro = db.Centros.Find(id);

            try
            {
                if (db.Docentes.Where(x => x.CentroId == id).Count() > 0)
                    ModelState.AddModelError("error", "Este centro contiene docentes relacionados. Favor borrarlos y volver a intentar.");

                if (db.Estudiantes.Where(x => x.CentroId == id).Count() > 0)
                    ModelState.AddModelError("error", "Este centro contiene estudiantes relacionados. Favor borrarlos y volver a intentar.");

                if (db.PersonalAdministrativo.Where(x => x.CentroId == id).Count() > 0)
                    ModelState.AddModelError("error", "Este centro contiene personal administrativo relacionado. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.Centros.Remove(centro);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(centro);
        }

        public ActionResult DashboardCentro() {

            return View();
        }

        //[OutputCache(Duration = 86400, VaryByParam = "id")]
        public ActionResult EstadisticasCentro(int id) //centroID
        {
            List<EstadisticaCentro> estadisticascentro = new List<EstadisticaCentro>();
            List<Docente> docentes = db.Docentes.Where(c => c.CentroId == id).ToList();
            List<DocenteMateria> docentesMaterias = new List<DocenteMateria>();
            docentesMaterias = db.DocenteMaterias.Where(c => c.Docente.CentroId == id).ToList();

            Centro centro = db.Centros.Find(id);
            
            //Cantidad Docentes del Centro
            int cantidadDocentesCentro = docentes.Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Cantidad Docentes", contador = cantidadDocentesCentro, colorhex = "#FFA500" });
            //Cantidad Docentes por Nivel Inicial
            int docenteNivelInicial = docentesMaterias.Select(m => new { m.DocenteId, m.Nivel }).Where(g => g.Nivel.HasFlag(NivelEducativo.Inicial)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes Nivel Inicial", contador = docenteNivelInicial, colorhex = "#157DEC" });

            //Cantidad Docentes por Nivel Primario
            int docenteNivelPrimario = docentesMaterias.Select(m => new { m.DocenteId, m.Nivel }).Where(g => g.Nivel.HasFlag(NivelEducativo.Primaria)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes Nivel Primario", contador = docenteNivelPrimario, colorhex = "#48CCCD" });
            //Cantidad Docentes 1er Grado
            int docentesPrimerGrado = docentesMaterias.Select(m => new { m.DocenteId, m.Grados }).Where(g => g.Grados.HasFlag(Grado.Primero)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes 1er Grado", contador = docentesPrimerGrado, colorhex = "#CC4747" });
            //Cantidad Docentes 2do Grado
            int docentesSegundoGrado = docentesMaterias.Select(m => new { m.DocenteId, m.Grados }).Where(g => g.Grados.HasFlag(Grado.Segundo)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes 2do Grado", contador = docentesSegundoGrado, colorhex = "#A41F1F" });
            //Cantidad Docentes 3er Grado
            int docentesTercerGrado = docentesMaterias.Select(m => new { m.DocenteId, m.Grados }).Where(g => g.Grados.HasFlag(Grado.Tercero)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes 3er Grado", contador = docentesTercerGrado, colorhex = "#4747CC" });
            //Cantidad Docentes 4to Grado
            int docentesCuartoGrado = docentesMaterias.Select(m => new { m.DocenteId, m.Grados }).Where(g => g.Grados.HasFlag(Grado.Cuarto)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes 4to Grado", contador = docentesCuartoGrado, colorhex = "#CCCC47" });
            //Cantidad Docentes 5to Grado
            int docentesQuintoGrado = docentesMaterias.Select(m => new { m.DocenteId, m.Grados }).Where(g => g.Grados.HasFlag(Grado.Quinto)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes 5to Grado", contador = docentesQuintoGrado, colorhex = "#555" });
            //Cantidad Docentes 6to Grado
            int docentesSextoGrado = docentesMaterias.Select(m => new { m.DocenteId, m.Grados }).Where(g => g.Grados.HasFlag(Grado.Sexto)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes 6to Grado", contador = docentesSextoGrado, colorhex = "#555" });
            //Cantidad Docentes 7mo Grado
            int docentesSeptimoGrado = docentesMaterias.Select(m => new { m.DocenteId, m.Grados }).Where(g => g.Grados.HasFlag(Grado.Septimo)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes 7mo Grado", contador = docentesSeptimoGrado, colorhex = "#555" });
            //Cantidad Docentes 8vo Grado
            int docentesOctavoGrado = docentesMaterias.Select(m => new { m.DocenteId, m.Grados }).Where(g => g.Grados.HasFlag(Grado.Octavo)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes 8vo Grado", contador = docentesOctavoGrado, colorhex = "#555" });

            //Cantidad Docentes Matematica
            int docentesMatematica = docentesMaterias.Select(m => new { m.DocenteId, m.Area }).Where(g => g.Area.HasFlag(DocenteArea.Matemática)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes Matemática", contador = docentesMatematica, colorhex = "#555" });
            //Cantidad Docentes Lengua Española
            int docentesLenguajes = docentesMaterias.Select(m => new { m.DocenteId, m.Area }).Where(g => g.Area.HasFlag(DocenteArea.LenguaEspañola)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes Lengua Española", contador = docentesLenguajes, colorhex = "#555" });
            //Cantidad Docentes Sociales
            int docentesSociales = docentesMaterias.Select(m => new { m.DocenteId, m.Area }).Where(g => g.Area.HasFlag(DocenteArea.Sociales)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes Sociales", contador = docentesSociales, colorhex = "#555" });
            //Cantidad Docentes Naturales
            int docentesNaturales = docentesMaterias.Select(m => new { m.DocenteId, m.Area }).Where(g => g.Area.HasFlag(DocenteArea.CienciasNaturales)).Distinct().Count();
            estadisticascentro.Add(new EstadisticaCentro { label = "Docentes Ciencias Naturales", contador = docentesNaturales, colorhex = "#555" });


            ViewBag.Centro = centro;
            return View(estadisticascentro);
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
