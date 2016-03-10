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
using System.Data.Entity.SqlServer;
using System.Data.OleDb;
using System.IO;

namespace Monitoreo.Controllers
{
    [Authorize]
    public class RegionalController : BaseController
    {
        private MonitoreoContext db = new MonitoreoContext();

        // GET: Regionales
        [Route("Regionales")]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Index()
        {
            var regionales = db.Regionales.Include(r => r.Director).Include(r => r.Municipio).Include(r => r.Provincia);
            return View(regionales.ToList());
        }

        // POST: Regionales
        [Route("Regionales/GetDataJson")]
        [HttpPost]
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult GetDataJson(DatatablesParams values)
        {
            var regionales = db.Regionales.Include(r => r.Director).Include(r => r.Director.Persona).Include(r => r.Municipio).Include(r => r.Provincia);
            var recordsTotal = regionales.Count();
            var recordsFiltered = recordsTotal;
            var limit = values.length > 0 ? values.length : recordsTotal;
            var from = values.start;

            // Seleccionando
            var data = regionales.ToList().Select(x => new { DT_RowId = x.Id, x.Codigo, x.Nombre, Director = (x.Director != null ? x.Director.Persona.NombreCompleto : ""), Provincia = (x.Provincia != null ? x.Provincia.Nombre : ""), Municipio = (x.Municipio != null ? x.Municipio.Nombre : ""), x.Sector, x.Calle });

            // Filtrando
            if (values.search != null && values.search.ContainsKey("value") && values.search["value"] is string[])
            {
                string searchValue = (values.search["value"] as string[])[0];
                searchValue = searchValue.Trim();

                if (!String.IsNullOrWhiteSpace(searchValue))
                {
                    data = data.Where(x => 
                        x.Codigo.Contains(searchValue) || x.Nombre.Contains(searchValue) || x.Sector.Contains(searchValue) || x.Calle.Contains(searchValue)
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
                        case "Codigo":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Codigo);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Codigo);
                            }
                            sorting = true;
                            break;
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
                        case "Sector":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Sector);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Sector);
                            }
                            sorting = true;
                            break;
                        case "Calle":
                            if ((item["dir"] as string[])[0] == "desc")
                            {
                                data = data.OrderByDescending(s => s.Calle);
                            }
                            else
                            {
                                data = data.OrderBy(s => s.Calle);
                            }
                            sorting = true;
                            break;
                    }
                }
            }

            // Ordenando por el primer campo mostrado
            if (!sorting)
            {
                data = data.OrderBy(s => s.Codigo);
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

        // GET: /Regional/5/Details
        [Authorize(Roles = "Administrador, Acompanante")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Regional regional = db.Regionales.Find(id);
            if (regional == null)
            {
                return HttpNotFound();
            }
            return View(regional);
        }

        // GET: Regionales/Create
        [Route("Regionales/Create")]
        public ActionResult Create()
        {
            Regional regional = new Regional();
            ViewBag.DirectorId = new SelectList(db.PersonalAdministrativo.ToList().Select(x => new { x.Id, x.Persona.NombreCompleto }), "Id", "NombreCompleto", regional.DirectorId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(x => x.ProvinciaId == regional.ProvinciaId), "Id", "Nombre");

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Value = "0", Text = "Seleccionar...", Selected = true });
            items.AddRange(db.Provincias.ToList().Select(x => new SelectListItem { Value = Convert.ToString(x.Id), Text = x.Nombre }));
            ViewBag.ProvinciaId =  items;
            return View(regional);
        }

        // POST: Regionales/Create
        [Route("Regionales/Create")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Codigo,Nombre,DirectorId,ProvinciaId,MunicipioId,Sector,Calle")] Regional regional)
        {
            if (ModelState.IsValid)
            {
                db.Regionales.Add(regional);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DirectorId = new SelectList(db.PersonalAdministrativo.ToList().Select(x => new { x.Id, x.Persona.NombreCompleto }), "Id", "NombreCompleto", regional.DirectorId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(x => x.ProvinciaId == regional.ProvinciaId), "Id", "Nombre", regional.MunicipioId);
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", regional.ProvinciaId);
            return View(regional);
        }

        // GET: /Regional/5/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Regional regional = db.Regionales.Find(id);
            if (regional == null)
            {
                return HttpNotFound();
            }

            ViewBag.DirectorId = new SelectList(db.PersonalAdministrativo.ToList().Select(x => new { x.Id, x.Persona.NombreCompleto }), "Id", "NombreCompleto", regional.DirectorId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(x => x.ProvinciaId == regional.ProvinciaId), "Id", "Nombre", regional.MunicipioId);
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", regional.ProvinciaId);
            return View(regional);
        }

        // POST: /Regional/5/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Codigo,Nombre,DirectorId,ProvinciaId,MunicipioId,Sector,Calle")] Regional regional)
        {
            if (ModelState.IsValid)
            {
                db.Entry(regional).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DirectorId = new SelectList(db.PersonalAdministrativo.ToList().Select(x => new { x.Id, x.Persona.NombreCompleto }), "Id", "NombreCompleto", regional.DirectorId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(x => x.ProvinciaId == regional.ProvinciaId), "Id", "Nombre", regional.MunicipioId);
            ViewBag.ProvinciaId = new SelectList(db.Provincias, "Id", "Nombre", regional.ProvinciaId);
            return View(regional);
        }

        // GET: /Regional/5/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Regional regional = db.Regionales.Find(id);
            if (regional == null)
            {
                return HttpNotFound();
            }
            return View(regional);
        }

        // POST: /Regional/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Regional regional = db.Regionales.Find(id);

            try
            {
                if (db.Distritos.Where(x => x.RegionalId == id).Count() > 0)
                    ModelState.AddModelError("error", "Esta regional contiene distritos relacionados. Favor borrarlos y volver a intentar.");

                if (db.PersonalAdministrativo.Where(x => x.RegionalId == id).Count() > 0)
                    ModelState.AddModelError("error", "Esta regional contiene personal relacionado. Favor borrarlos y volver a intentar.");

                if (ModelState.IsValid)
                {
                    db.Regionales.Remove(regional);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
            }

            if (ModelState.IsValid) return RedirectToAction("Index");
            else return View(regional);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportFromExcelFile(HttpPostedFileBase uploadFile)
        {
            try
            {
                if (uploadFile.ContentLength > 0)
                {
                    string filePath = Path.Combine(HttpContext.Server.MapPath("../"), Path.GetFileName(uploadFile.FileName));
                    uploadFile.SaveAs(filePath);
                    DataSet ds = new DataSet();
                    string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 12.0;";

                    using (OleDbConnection conn = new System.Data.OleDb.OleDbConnection(ConnectionString))
                    {
                        conn.Open();
                        using (DataTable dtExcelSchema = conn.GetSchema("Tables"))
                        {
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            string query = "SELECT * FROM [" + sheetName + "]";
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                            //DataSet ds = new DataSet();
                            adapter.Fill(ds, "Items");
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        Regional regional = new Regional();
                                        regional.Codigo = ds.Tables[0].Rows[i][0].ToString();
                                        regional.Nombre = ds.Tables[0].Rows[i][1].ToString();
                                        String municipioId = ds.Tables[0].Rows[i][2].ToString();
                                        if (municipioId != "")
                                        {
                                            Municipio municipio= db.Municipios.Where(m => m.Codigo == municipioId).SingleOrDefault();
                                            regional.MunicipioId = municipio.Id;

                                        }
                                        bool isRepeated = db.Provincias.Any(c => c.Codigo == regional.Codigo);

                                        if (isRepeated == false && regional.Codigo != "" && regional.Codigo != null && regional.Nombre != "")
                                        {
                                            db.Regionales.Add(regional);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception exp)
            {
                var dummy = exp.Message;
            }
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
