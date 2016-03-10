using Microsoft.AspNet.Identity.EntityFramework;
using Monitoreo.Models.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.DAL
{
    public class MonitoreoInitializer : System.Data.Entity.CreateDatabaseIfNotExists<MonitoreoContext> 
    {
        protected override void Seed(MonitoreoContext context)
        {
            //ApplicationDbContext appContext = new ApplicationDbContext();
            
            //var grupos = new List<IdentityRole>
            //{
            //    new IdentityRole{Name="Administrador"},
            //    new IdentityRole{Name="Formador"},
            //    new IdentityRole{Name="Docente"},
            //    new IdentityRole{Name="Otro"}
            //};
            //grupos.ForEach(s => appContext.Roles.Add(s));
            //appContext.SaveChanges();

            //var superadmin = new ApplicationUser { UserName = "superadmin" };

            //var userManager = new Microsoft.AspNet.Identity.UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //var result = userManager.CreateAsync(superadmin, "123");
            //userManager.AddToRoleAsync(superadmin.Id, "Administrador");
            //appContext.SaveChanges();


            //#region Parametricos
            //var gruposEtnicos = new List<GrupoEtnico>
            //{
            //    new GrupoEtnico{Nombre="No Definido"},
            //    new GrupoEtnico{Nombre="Mulato / Mestizo"},
            //    new GrupoEtnico{Nombre="Blanco / Europeo-Mediterráneo"},
            //    new GrupoEtnico{Nombre="Negro / Afrodescendiente"},
            //    new GrupoEtnico{Nombre="Asiático / Otra Raza"},
            //};

            //var actividadFormativaBases = new List<ActividadFormativaBase>
            //{
            //    new ActividadFormativaBase{
            //        Tipo=TipoActividadFormativa.Presencial, 
            //        Duracion=8, 
            //        Organizacion="Grupal por Área y  Ámbito de Acción. Se realiza un sábado al mes organizados por Nivel/Ciclo/Área por Distritos/Centros Sedes de las Redes.",
            //        Actores="Formadores, Docentes o Equipo de Gestión del Centro Educativo.",
            //        ModoEvaluacion="Lista de Asistencia del Taller, Reporte de notas por parte de los (as) formadores."
            //    },
            //    new ActividadFormativaBase{
            //        Tipo=TipoActividadFormativa.Acompanamiento, 
            //        Duracion=8, 
            //        Organizacion="Grupos Pedagógicos: Una vez al mes, grupal por Nivel/Ciclo/Área/Centro Educativo.",
            //        Actores="Acompañantes Pedagógicos, Docentes o Equipo de Gestión del Centro Educativo.",
            //        ModoEvaluacion=""
            //    },
            //    new ActividadFormativaBase{
            //        Tipo=TipoActividadFormativa.Acompanamiento, 
            //        Duracion=8, 
            //        Organizacion="Clases Modelo: Una vez al mes, individual por grado/área -> grado/sección",
            //        Actores="Acompañantes Pedagógicos, Docentes o Equipo de Gestión del Centro Educativo.",
            //        ModoEvaluacion=""
            //    },
            //    new ActividadFormativaBase{
            //        Tipo=TipoActividadFormativa.Acompanamiento, 
            //        Duracion=8, 
            //        Organizacion="Acompañamiento Tutorial.",
            //        Actores="Acompañantes Pedagógicos, Docentes o Equipo de Gestión del Centro Educativo.",
            //        ModoEvaluacion=""
            //    },
            //    new ActividadFormativaBase{
            //        Tipo=TipoActividadFormativa.Acompanamiento, 
            //        Duracion=8, 
            //        Organizacion="Acompañamiento en el diseño de acciones de mejora.",
            //        Actores="",
            //        ModoEvaluacion=""
            //    },
            //    new ActividadFormativaBase{
            //        Tipo=TipoActividadFormativa.Virtual, 
            //        Duracion=8, 
            //        Organizacion="",
            //        Actores="Tutor Virtual, Docentes o Equipo de Gestión del Centro Educativo.",
            //        ModoEvaluacion=""
            //    },
            //};

            //var provincias = new List<Provincia>
            //{
            //    new Provincia{Codigo = "001", Nombre="Distrito Nacional"},
            //    new Provincia{Codigo = "002", Nombre="Santiago"},
            //    new Provincia{Codigo = "003", Nombre="Santo Domingo"},
            //    new Provincia{Codigo = "004", Nombre="San Cristóbal"},
            //    new Provincia{Codigo = "005", Nombre="La Vega"},
            //    new Provincia{Codigo = "006", Nombre="San Pedro de Macorís"},
            //    new Provincia{Codigo = "007", Nombre="La Romana"},
            //    new Provincia{Codigo = "008", Nombre="Duarte"},
            //    new Provincia{Codigo = "009", Nombre="Puerto Plata"},
            //    new Provincia{Codigo = "010", Nombre="La Altagracia"},
            //    new Provincia{Codigo = "011", Nombre="Espaillat"},
            //    new Provincia{Codigo = "012", Nombre="San Juan de la Maguana"},
            //    new Provincia{Codigo = "013", Nombre="Monseñor Nouel"},
            //    new Provincia{Codigo = "014", Nombre="Peravia"},
            //    new Provincia{Codigo = "015", Nombre="Azua"},
            //    new Provincia{Codigo = "016", Nombre="Barahona"},
            //    new Provincia{Codigo = "017", Nombre="Sánchez Ramírez"},
            //    new Provincia{Codigo = "018", Nombre="El Seibo"},
            //    new Provincia{Codigo = "019", Nombre="María Trinidad Sánchez"},
            //    new Provincia{Codigo = "020", Nombre="Samaná"},
            //    new Provincia{Codigo = "021", Nombre="Valverde"},
            //    new Provincia{Codigo = "022", Nombre="Hato Mayor"},
            //    new Provincia{Codigo = "023", Nombre="Hermanas Mirabal"},
            //    new Provincia{Codigo = "024", Nombre="Monte Plata"},
            //    new Provincia{Codigo = "025", Nombre="Santiago Rodríguez"},
            //    new Provincia{Codigo = "026", Nombre="San José de Ocoa"},
            //    new Provincia{Codigo = "027", Nombre="Monte Cristi"},
            //    new Provincia{Codigo = "028", Nombre="Dajabón"},
            //    new Provincia{Codigo = "029", Nombre="Elías Piña"},
            //    new Provincia{Codigo = "030", Nombre="Bahoruco"},
            //    new Provincia{Codigo = "031", Nombre="Independencia"},
            //    new Provincia{Codigo = "032", Nombre="Pedernales"}
            //};

            //var municipios = new List<Municipio>
            //{
            //    new Municipio{Nombre="Santo Domingo de Guzmán",Provincia=provincias.Single(s => s.Nombre == "Distrito Nacional")},
            //    new Municipio{Nombre="Santiago de los Caballeros",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Santo Domingo Este",Provincia=provincias.Single(s => s.Nombre == "Santo Domingo")},
            //    new Municipio{Nombre="Santo Domingo Norte",Provincia=provincias.Single(s => s.Nombre == "Santo Domingo")},
            //    new Municipio{Nombre="Santo Domingo Oeste",Provincia=provincias.Single(s => s.Nombre == "Santo Domingo")},
            //    new Municipio{Nombre="San Cristóbal",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="Concepción de la Vega",Provincia=provincias.Single(s => s.Nombre == "La Vega")},
            //    new Municipio{Nombre="San Pedro de Macorís",Provincia=provincias.Single(s => s.Nombre == "San Pedro de Macorís")},
            //    new Municipio{Nombre="La Romana",Provincia=provincias.Single(s => s.Nombre == "La Romana")},
            //    new Municipio{Nombre="Los Alcarrizos",Provincia=provincias.Single(s => s.Nombre == "Santo Domingo")},
            //    new Municipio{Nombre="San Francisco de Macorís",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="San Felipe de Puerto Plata",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Salvaleón de Higüey",Provincia=provincias.Single(s => s.Nombre == "La Altagracia")},
            //    new Municipio{Nombre="Moca",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="San Juan de la Maguana",Provincia=provincias.Single(s => s.Nombre == "San Juan de la Maguana")},
            //    new Municipio{Nombre="Bonao",Provincia=provincias.Single(s => s.Nombre == "Monseñor Nouel")},
            //    new Municipio{Nombre="Baní",Provincia=provincias.Single(s => s.Nombre == "Peravia")},
            //    new Municipio{Nombre="Boca Chica",Provincia=provincias.Single(s => s.Nombre == "Santo Domingo")},
            //    new Municipio{Nombre="Azua de Compostela",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Bajos de Haina",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="Villa Altagracia",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="Santa Cruz de Barahona",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Cotuí",Provincia=provincias.Single(s => s.Nombre == "Sánchez Ramírez")},
            //    new Municipio{Nombre="Santa Cruz del Seibo",Provincia=provincias.Single(s => s.Nombre == "El Seibo")},
            //    new Municipio{Nombre="Jarabacoa",Provincia=provincias.Single(s => s.Nombre == "La Vega")},
            //    new Municipio{Nombre="Nagua",Provincia=provincias.Single(s => s.Nombre == "María Trinidad Sánchez")},
            //    new Municipio{Nombre="Samana",Provincia=provincias.Single(s => s.Nombre == "Samaná")},
            //    new Municipio{Nombre="Tamboril",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Mao",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="La Victoria (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santo Domingo")},
            //    new Municipio{Nombre="Esperanza",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="Pedro Brand",Provincia=provincias.Single(s => s.Nombre == "Santo Domingo")},
            //    new Municipio{Nombre="Sosua",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Hato Mayor del Rey",Provincia=provincias.Single(s => s.Nombre == "Hato Mayor")},
            //    new Municipio{Nombre="Constanza",Provincia=provincias.Single(s => s.Nombre == "La Vega")},
            //    new Municipio{Nombre="Navarrete",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Salcedo",Provincia=provincias.Single(s => s.Nombre == "Hermanas Mirabal")},
            //    new Municipio{Nombre="Yaguate",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="La Mata",Provincia=provincias.Single(s => s.Nombre == "Sánchez Ramírez")},
            //    new Municipio{Nombre="Las Matas de Farfán",Provincia=provincias.Single(s => s.Nombre == "San Juan de la Maguana")},
            //    new Municipio{Nombre="Monte Plata",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="Yamasá",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="San Ignacio de Sabaneta",Provincia=provincias.Single(s => s.Nombre == "Santiago Rodríguez")},
            //    new Municipio{Nombre="San José de las Matas",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="San Antonio de Guerra",Provincia=provincias.Single(s => s.Nombre == "Santo Domingo")},
            //    new Municipio{Nombre="San José de Ocoa",Provincia=provincias.Single(s => s.Nombre == "San José de Ocoa")},
            //    new Municipio{Nombre="Bayaguana",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="El Carril (D.M.)",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="Consuelo",Provincia=provincias.Single(s => s.Nombre == "San Pedro de Macorís")},
            //    new Municipio{Nombre="Cambita Garabitos",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="Villa Gonzalez",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="San Gregorio de Nigua",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="Licey al Medio",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Sánchez",Provincia=provincias.Single(s => s.Nombre == "Samaná")},
            //    new Municipio{Nombre="Hato del Yaque (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="San Fernando de Monte Cristi",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="Dajabón",Provincia=provincias.Single(s => s.Nombre == "Dajabón")},
            //    new Municipio{Nombre="Comendador",Provincia=provincias.Single(s => s.Nombre == "Elías Piña")},
            //    new Municipio{Nombre="Villa Tapia",Provincia=provincias.Single(s => s.Nombre == "Hermanas Mirabal")},
            //    new Municipio{Nombre="Neyba",Provincia=provincias.Single(s => s.Nombre == "Bahoruco")},
            //    new Municipio{Nombre="Tenares",Provincia=provincias.Single(s => s.Nombre == "Hermanas Mirabal")},
            //    new Municipio{Nombre="Sabana Grande de Boyá",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="El Cercado",Provincia=provincias.Single(s => s.Nombre == "San Juan de la Maguana")},
            //    new Municipio{Nombre="Imbert",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Sabana Yegua",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Altamira",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Fantino",Provincia=provincias.Single(s => s.Nombre == "Sánchez Ramírez")},
            //    new Municipio{Nombre="Río Verde Arriba (D.M.)",Provincia=provincias.Single(s => s.Nombre == "La Vega")},
            //    new Municipio{Nombre="Padre Las Casas",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="San Víctor (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="Miches",Provincia=provincias.Single(s => s.Nombre == "El Seibo")},
            //    new Municipio{Nombre="San José de Los Llanos",Provincia=provincias.Single(s => s.Nombre == "San Pedro de Macorís")},
            //    new Municipio{Nombre="Gaspar Hernández",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="Quisqueya",Provincia=provincias.Single(s => s.Nombre == "San Pedro de Macorís")},
            //    new Municipio{Nombre="Villa Riva",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Pimentel",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Villa Montellano (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Luperón",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Maimón",Provincia=provincias.Single(s => s.Nombre == "Monseñor Nouel")},
            //    new Municipio{Nombre="Guaymate",Provincia=provincias.Single(s => s.Nombre == "La Romana")},
            //    new Municipio{Nombre="Duvergé",Provincia=provincias.Single(s => s.Nombre == "Independencia")},
            //    new Municipio{Nombre="Peralvillo",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="Las Matas de Santa Cruz",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="La Canela (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Vicente Noble",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Castillo",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Tireo (D.M.)",Provincia=provincias.Single(s => s.Nombre == "La Vega")},
            //    new Municipio{Nombre="San Rafael del Yuma",Provincia=provincias.Single(s => s.Nombre == "La Altagracia")},
            //    new Municipio{Nombre="Cenovi (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Sabana Grande de Palenque",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="Loma de Cabrera",Provincia=provincias.Single(s => s.Nombre == "Dajabón")},
            //    new Municipio{Nombre="Río San Juan",Provincia=provincias.Single(s => s.Nombre == "María Trinidad Sánchez")},
            //    new Municipio{Nombre="Jánico",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Villa Vásquez",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="Matanzas",Provincia=provincias.Single(s => s.Nombre == "Peravia")},
            //    new Municipio{Nombre="Laguna Salada",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="Sabana de la Mar",Provincia=provincias.Single(s => s.Nombre == "Hato Mayor")},
            //    new Municipio{Nombre="Jima Abajo",Provincia=provincias.Single(s => s.Nombre == "La Vega")},
            //    new Municipio{Nombre="Galván",Provincia=provincias.Single(s => s.Nombre == "Bahoruco")},
            //    new Municipio{Nombre="Veragua (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="El Factor",Provincia=provincias.Single(s => s.Nombre == "María Trinidad Sánchez")},
            //    new Municipio{Nombre="Los Botados (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="Cabral",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Las Terrenas",Provincia=provincias.Single(s => s.Nombre == "Samaná")},
            //    new Municipio{Nombre="Villa Isabela",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Pedernales",Provincia=provincias.Single(s => s.Nombre == "Pedernales")},
            //    new Municipio{Nombre="Castañuela",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="Arenoso",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Los Hidalgos",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Paya (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Peravia")},
            //    new Municipio{Nombre="Las Guaranas",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Uvilla (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Bahoruco")},
            //    new Municipio{Nombre="Paraíso",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="La Otra Banda (D.M.)",Provincia=provincias.Single(s => s.Nombre == "La Altagracia")},
            //    new Municipio{Nombre="Juan López (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="La Peña (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Enriquillo",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Cabrera",Provincia=provincias.Single(s => s.Nombre == "María Trinidad Sánchez")},
            //    new Municipio{Nombre="Juan de Herrera",Provincia=provincias.Single(s => s.Nombre == "San Juan de la Maguana")},
            //    new Municipio{Nombre="Tabara Arriba",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Villa Los Almácigos",Provincia=provincias.Single(s => s.Nombre == "Santiago Rodríguez")},
            //    new Municipio{Nombre="Sabana Iglesia (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Piedra Blanca",Provincia=provincias.Single(s => s.Nombre == "Monseñor Nouel")},
            //    new Municipio{Nombre="Rincon (D.M.)",Provincia=provincias.Single(s => s.Nombre == "La Vega")},
            //    new Municipio{Nombre="El Pozo (D.M.)",Provincia=provincias.Single(s => s.Nombre == "María Trinidad Sánchez")},
            //    new Municipio{Nombre="Vallejuelo",Provincia=provincias.Single(s => s.Nombre == "San Juan de la Maguana")},
            //    new Municipio{Nombre="San José de Matanzas (D.M.)",Provincia=provincias.Single(s => s.Nombre == "María Trinidad Sánchez")},
            //    new Municipio{Nombre="Nizao",Provincia=provincias.Single(s => s.Nombre == "Peravia")},
            //    new Municipio{Nombre="Monción",Provincia=provincias.Single(s => s.Nombre == "Santiago Rodríguez")},
            //    new Municipio{Nombre="Rancho Arriba",Provincia=provincias.Single(s => s.Nombre == "San José de Ocoa")},
            //    new Municipio{Nombre="Peralta",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Sabana Larga",Provincia=provincias.Single(s => s.Nombre == "San José de Ocoa")},
            //    new Municipio{Nombre="Villa Jaragua",Provincia=provincias.Single(s => s.Nombre == "Bahoruco")},
            //    new Municipio{Nombre="Jimaní",Provincia=provincias.Single(s => s.Nombre == "Independencia")},
            //    new Municipio{Nombre="Sabana del Puerto (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monseñor Nouel")},
            //    new Municipio{Nombre="Las Yayas de Viajama",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Pueblo Viejo",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Hondo Valle",Provincia=provincias.Single(s => s.Nombre == "Elías Piña")},
            //    new Municipio{Nombre="Tamayo",Provincia=provincias.Single(s => s.Nombre == "Bahoruco")},
            //    new Municipio{Nombre="Cevicos",Provincia=provincias.Single(s => s.Nombre == "Sánchez Ramírez")},
            //    new Municipio{Nombre="Amina (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="Polo",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Maizal (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="Matayaya (D.M.)",Provincia=provincias.Single(s => s.Nombre == "San Juan de la Maguana")},
            //    new Municipio{Nombre="Ramon Santana",Provincia=provincias.Single(s => s.Nombre == "San Pedro de Macorís")},
            //    new Municipio{Nombre="Pepillo Salcedo",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="El Palmar (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Bahoruco")},
            //    new Municipio{Nombre="El Rubio (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Bohechío",Provincia=provincias.Single(s => s.Nombre == "San Juan de la Maguana")},
            //    new Municipio{Nombre="Baitoa (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Las Lagunas de Nisibon",Provincia=provincias.Single(s => s.Nombre == "La Altagracia")},
            //    new Municipio{Nombre="Villa Fundación (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Peravia")},
            //    new Municipio{Nombre="Cana Chapetón (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="Don Juan (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="Jaibón de Pueblo Nuevo (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="El Llano",Provincia=provincias.Single(s => s.Nombre == "Elías Piña")},
            //    new Municipio{Nombre="Jamao al Norte",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="Hatillo Palma (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="El Valle",Provincia=provincias.Single(s => s.Nombre == "Hato Mayor")},
            //    new Municipio{Nombre="Los Ríos",Provincia=provincias.Single(s => s.Nombre == "Bahoruco")},
            //    new Municipio{Nombre="Villa Sonador (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monseñor Nouel")},
            //    new Municipio{Nombre="Estebania",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Guatapanal (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="La Ciénaga",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Guayabo Dulce (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Hato Mayor")},
            //    new Municipio{Nombre="Cristo Rey de Guaraguao (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Partido",Provincia=provincias.Single(s => s.Nombre == "Dajabón")},
            //    new Municipio{Nombre="Los Cacaos",Provincia=provincias.Single(s => s.Nombre == "San Cristóbal")},
            //    new Municipio{Nombre="Guayubín",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="Banica",Provincia=provincias.Single(s => s.Nombre == "Elías Piña")},
            //    new Municipio{Nombre="Arroyo Salado (D.M.)",Provincia=provincias.Single(s => s.Nombre == "María Trinidad Sánchez")},
            //    new Municipio{Nombre="Mata Palacio (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Hato Mayor")},
            //    new Municipio{Nombre="Villarpando (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Cayetano Germosen",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="La Descubierta",Provincia=provincias.Single(s => s.Nombre == "Independencia")},
            //    new Municipio{Nombre="Restauración",Provincia=provincias.Single(s => s.Nombre == "Dajabón")},
            //    new Municipio{Nombre="Las Charcas",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Pedro Corto (D.M.)",Provincia=provincias.Single(s => s.Nombre == "San Juan de la Maguana")},
            //    new Municipio{Nombre="El Pino",Provincia=provincias.Single(s => s.Nombre == "Dajabón")},
            //    new Municipio{Nombre="Villa Elisa (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monte Cristi")},
            //    new Municipio{Nombre="Santana (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Peravia")},
            //    new Municipio{Nombre="La Entrada (D.M.)",Provincia=provincias.Single(s => s.Nombre == "María Trinidad Sánchez")},
            //    new Municipio{Nombre="Guananico",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Juncalito (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Cristóbal",Provincia=provincias.Single(s => s.Nombre == "Independencia")},
            //    new Municipio{Nombre="Eugenio Maria de Hostos",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="José Contreras (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="Gonzalo (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="Jaibón (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="La Cueva (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Sánchez Ramírez")},
            //    new Municipio{Nombre="Blanco (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Hermanas Mirabal")},
            //    new Municipio{Nombre="Las Salinas",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Jicomé (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="La Ciénaga (D.M.)",Provincia=provincias.Single(s => s.Nombre == "San José de Ocoa")},
            //    new Municipio{Nombre="Guayabal",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Pizarrete (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Peravia")},
            //    new Municipio{Nombre="Pedro García (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Agua Santa del Yuma (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Duarte")},
            //    new Municipio{Nombre="Juan Santiago",Provincia=provincias.Single(s => s.Nombre == "Elías Piña")},
            //    new Municipio{Nombre="El Puerto (D.M.)",Provincia=provincias.Single(s => s.Nombre == "San Pedro de Macorís")},
            //    new Municipio{Nombre="Palmar Arriba (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Santiago")},
            //    new Municipio{Nombre="Pedro Sánchez (D.M.)",Provincia=provincias.Single(s => s.Nombre == "El Seibo")},
            //    new Municipio{Nombre="Joba Arriba (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Espaillat")},
            //    new Municipio{Nombre="Pescadería (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Pedro Santana",Provincia=provincias.Single(s => s.Nombre == "Elías Piña")},
            //    new Municipio{Nombre="El Peñón",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Jaquimeyes",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Oviedo",Provincia=provincias.Single(s => s.Nombre == "Pedernales")},
            //    new Municipio{Nombre="Fundación",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Río Limpio (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Elías Piña")},
            //    new Municipio{Nombre="Postrer Río",Provincia=provincias.Single(s => s.Nombre == "Independencia")},
            //    new Municipio{Nombre="Canoa (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Juan Adrián (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monseñor Nouel")},
            //    new Municipio{Nombre="Juancho (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Pedernales")},
            //    new Municipio{Nombre="Yerba Buena (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Hato Mayor")},
            //    new Municipio{Nombre="Palmar de Ocoa (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Azua")},
            //    new Municipio{Nombre="Elupina Cordero de las Cañitas(D.M.)",Provincia=provincias.Single(s => s.Nombre == "Hato Mayor")},
            //    new Municipio{Nombre="Estero Hondo (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")},
            //    new Municipio{Nombre="Guayabal (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Independencia")},
            //    new Municipio{Nombre="Majagual (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Monte Plata")},
            //    new Municipio{Nombre="Mella",Provincia=provincias.Single(s => s.Nombre == "Independencia")},
            //    new Municipio{Nombre="Sabana Buey (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Peravia")},
            //    new Municipio{Nombre="El Cachón (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Barahona")},
            //    new Municipio{Nombre="Boca de Yuma (D.M.)",Provincia=provincias.Single(s => s.Nombre == "La Altagracia")},
            //    new Municipio{Nombre="La Caya (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Valverde")},
            //    new Municipio{Nombre="La Isabela (D.M.)",Provincia=provincias.Single(s => s.Nombre == "Puerto Plata")}
            //};

            //var secciones = new List<Seccion>
            //{
            //    new Seccion{ Numero="A" },
            //    new Seccion{ Numero="B" },
            //    new Seccion{ Numero="C" },
            //    new Seccion{ Numero="D" },
            //    new Seccion{ Numero="E" },
            //    new Seccion{ Numero="F" },
            //    new Seccion{ Numero="G" },
            //    new Seccion{ Numero="H" },
            //    new Seccion{ Numero="I" },
            //    new Seccion{ Numero="J" },
            //    new Seccion{ Numero="NA"}
            //};

            //provincias.ForEach(s => context.Provincias.Add(s));
            //municipios.ForEach(s => context.Municipios.Add(s));
            //actividadFormativaBases.ForEach(s => context.ActividadFormativaBases.Add(s));
            //gruposEtnicos.ForEach(s => context.GruposEtnicos.Add(s));
            //secciones.ForEach(s => context.Secciones.Add(s));
           // #endregion

            #region De Pruebas
            var preguntas = new List<Pregunta>
            {
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "1- Es el documento que edita el Ministerio de Educación para cada maestro/a con la intención de que lo utilice para orientar y organizar los procesos pedagógicos que desarrolla.", 
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "A", Valor = "A", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "B", Valor = "B", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "C", Valor = "C", Correcta = true}
                //    }
                //},
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "2- En el se expresan las ideas y aspiraciones que tiene una nación para formar sus ciudadanos y ciudadanas.",
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "A", Valor = "A", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "B", Valor = "B", Correcta = true},
                //        new PreguntaOpcion{ Titulo = "C", Valor = "C", Correcta = false}
                //    }
                //},
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "3- Son fundamentos legales del currículo dominicano.", 
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "A", Valor = "A", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "B", Valor = "B", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "C", Valor = "C", Correcta = true}
                //    }
                //},
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "4- Es el proceso de organización del trabajo docente que guiado por lo establecido en el currículo permite tomar las decisiones sobre las estrategias de enseñanza que seleccionaran para promover aprendizajes en los niños y las niñas.", 
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "A", Valor = "A", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "B", Valor = "B", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "C", Valor = "C", Correcta = true}
                //    }
                //},
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "5- En el diseño curricular expresan las intenciones educativas de mayor relevancia y significatividad.", 
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "A", Valor = "A", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "B", Valor = "B", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "C", Valor = "C", Correcta = true}
                //    }
                //},
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "6- Expresan la capacidad de utilizar los conocimientos, procedimientos, habilidades y destrezas, valores y actitudes para resolver situaciones diversas de la vida cotidiana y en contextos diversos.", 
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "A", Valor = "A", Correcta = true},
                //        new PreguntaOpcion{ Titulo = "B", Valor = "B", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "C", Valor = "C", Correcta = false}
                //    }
                //},
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "7- En nuestro currículo se asume los enfoques.", 
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "A", Valor = "A", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "B", Valor = "B", Correcta = false},
                //        new PreguntaOpcion{ Titulo = "C", Valor = "C", Correcta = true}
                //    }
                //},
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "1- Los indicadores de logros y las competencias específicas deben guardar estricta diferencia con las competencias fundamentales para promover su desarrollo.", 
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "Si", Valor = "Si", Correcta = true},
                //        new PreguntaOpcion{ Titulo = "No", Valor = "No", Correcta = false}
                //    }
                //},
                //new Pregunta{ TipoEvaluacion = TipoEvaluacion.PreTest, NivelDominio = NivelDominio.Actitudinal,
                //    Descripcion = "2- El grado de desarrollo que alcanzan los niños y las niñas de las competencias fundamentales en cada nivel del sistema educativo se considera el nivel de dominio.", 
                //    Opciones = new List<PreguntaOpcion> {
                //        new PreguntaOpcion{ Titulo = "Si", Valor = "Si", Correcta = true},
                //        new PreguntaOpcion{ Titulo = "No", Valor = "No", Correcta = false}
                //    }
                //},
            };

            //var regionales = new List<Regional>
            //{
            //    new Regional{Codigo = "01", Nombre= "Regional 1 - Barahona"},
            //    new Regional{Codigo = "02", Nombre= "Regional 2 - San Juan de la Maguana"},
            //    new Regional{Codigo = "03", Nombre= "Regional 3 - Azua"},
            //    new Regional{Codigo = "04", Nombre= "Regional 4 - San Cristobal"},
            //    new Regional{Codigo = "05", Nombre= "Regional 5 - San Pedro de Macoris"},
            //    new Regional{Codigo = "06", Nombre= "Regional 6 - La Vega"},
            //    new Regional{Codigo = "07", Nombre= "Regional 7 - San Francisco de Macoris"},
            //    new Regional{Codigo = "08", Nombre= "Regional 8 - Santiago"},
            //    new Regional{Codigo = "09", Nombre= "Regional 9 - Mao"},
            //    new Regional{Codigo = "10", Nombre= "Regional 10 - Santo Domingo II"},
            //    new Regional{Codigo = "11", Nombre= "Regional 11 - Puerto Plata"},
            //    new Regional{Codigo = "12", Nombre= "Regional 12 - Higuey"},
            //    new Regional{Codigo = "13", Nombre= "Regional 13 - Monte Cristi"},
            //    new Regional{Codigo = "14", Nombre= "Regional 14 - Nagua"},
            //    new Regional{Codigo = "15", Nombre= "Regional 15 - Santo Domingo III"},
            //    new Regional{Codigo = "16", Nombre= "Regional 16 - Cotui"},
            //    new Regional{Codigo = "17", Nombre= "Regional 17 - Monte Plata"},
            //    new Regional{Codigo = "18", Nombre= "Regional 18 - Neyba"},

            //};

            //var distritos = new List<Distrito>
            //{
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 1- 01", Regional = regionales[0]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 1- 02", Regional = regionales[0]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 1- 03", Regional = regionales[0]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 1- 04", Regional = regionales[0]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 1- 05", Regional = regionales[0]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 2- 01", Regional = regionales[1]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 2- 02", Regional = regionales[1]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 2- 03", Regional = regionales[1]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 2- 04", Regional = regionales[1]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 2- 05", Regional = regionales[1]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 2- 06", Regional = regionales[1]},
            //    new Distrito{Codigo = "7" , Nombre="Distrito Educativo 2- 07", Regional = regionales[1]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 3- 01", Regional = regionales[2]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 3- 02", Regional = regionales[2]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 3- 03", Regional = regionales[2]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 3- 04", Regional = regionales[2]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 4- 01", Regional = regionales[3]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 4- 02", Regional = regionales[3]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 4- 03", Regional = regionales[3]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 4- 04", Regional = regionales[3]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 4- 05", Regional = regionales[3]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 4- 06", Regional = regionales[3]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 5- 01", Regional = regionales[4]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 5- 02", Regional = regionales[4]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 5- 03", Regional = regionales[4]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 5- 04", Regional = regionales[4]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 5- 05", Regional = regionales[4]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 5- 06", Regional = regionales[4]},
            //    new Distrito{Codigo = "7" , Nombre="Distrito Educativo 5- 07", Regional = regionales[4]},
            //    new Distrito{Codigo = "8" , Nombre="Distrito Educativo 5- 08", Regional = regionales[4]},
            //    new Distrito{Codigo = "9" , Nombre="Distrito Educativo 5- 09", Regional = regionales[4]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 6- 01", Regional = regionales[5]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 6- 02", Regional = regionales[5]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 6- 03", Regional = regionales[5]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 6- 04", Regional = regionales[5]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 6- 05", Regional = regionales[5]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 6- 06", Regional = regionales[5]},
            //    new Distrito{Codigo = "7" , Nombre="Distrito Educativo 6- 07", Regional = regionales[5]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 7- 01", Regional = regionales[6]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 7- 02", Regional = regionales[6]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 7- 03", Regional = regionales[6]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 7- 04", Regional = regionales[6]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 7- 05", Regional = regionales[6]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 7- 06", Regional = regionales[6]},
            //    new Distrito{Codigo = "7" , Nombre="Distrito Educativo 7- 07", Regional = regionales[6]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 8- 01", Regional = regionales[7]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 8- 02", Regional = regionales[7]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 8- 03", Regional = regionales[7]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 8- 04", Regional = regionales[7]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 8- 05", Regional = regionales[7]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 8- 06", Regional = regionales[7]},
            //    new Distrito{Codigo = "7" , Nombre="Distrito Educativo 8- 07", Regional = regionales[7]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 9- 01", Regional = regionales[8]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 9- 02", Regional = regionales[8]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 9- 03", Regional = regionales[8]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 9- 04", Regional = regionales[8]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 10- 01", Regional = regionales[9]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 10- 02", Regional = regionales[9]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 10- 03", Regional = regionales[9]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 10- 04", Regional = regionales[9]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 10- 05", Regional = regionales[9]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 10- 06", Regional = regionales[9]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 11- 01", Regional = regionales[10]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 11- 02", Regional = regionales[10]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 11- 03", Regional = regionales[10]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 11- 04", Regional = regionales[10]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 11- 05", Regional = regionales[10]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 11- 06", Regional = regionales[10]},
            //    new Distrito{Codigo = "7" , Nombre="Distrito Educativo 11- 07", Regional = regionales[10]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 12- 01", Regional = regionales[11]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 12- 02", Regional = regionales[11]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 12- 03", Regional = regionales[11]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 12- 04", Regional = regionales[11]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 13- 01", Regional = regionales[12]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 13- 02", Regional = regionales[12]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 13- 03", Regional = regionales[12]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 13- 04", Regional = regionales[12]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 13- 05", Regional = regionales[12]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 13- 06", Regional = regionales[12]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 14- 01", Regional = regionales[13]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 14- 02", Regional = regionales[13]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 14- 03", Regional = regionales[13]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 14- 04", Regional = regionales[13]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 14- 05", Regional = regionales[13]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 15- 01", Regional = regionales[14]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 15- 02", Regional = regionales[14]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 15- 03", Regional = regionales[14]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 15- 04", Regional = regionales[14]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 15- 05", Regional = regionales[14]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 16- 01", Regional = regionales[15]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 16- 02", Regional = regionales[15]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 16- 03", Regional = regionales[15]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 16- 04", Regional = regionales[15]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 16- 05", Regional = regionales[15]},
            //    new Distrito{Codigo = "6" , Nombre="Distrito Educativo 16- 06", Regional = regionales[15]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 17- 01", Regional = regionales[16]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 17- 02", Regional = regionales[16]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 17- 03", Regional = regionales[16]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 17- 04", Regional = regionales[16]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 17- 05", Regional = regionales[16]},
            //    new Distrito{Codigo = "1" , Nombre="Distrito Educativo 18- 01", Regional = regionales[17]},
            //    new Distrito{Codigo = "2" , Nombre="Distrito Educativo 18- 02", Regional = regionales[17]},
            //    new Distrito{Codigo = "3" , Nombre="Distrito Educativo 18- 03", Regional = regionales[17]},
            //    new Distrito{Codigo = "4" , Nombre="Distrito Educativo 18- 04", Regional = regionales[17]},
            //    new Distrito{Codigo = "5" , Nombre="Distrito Educativo 18- 05", Regional = regionales[17]},
            //};

            //var redes = new List<Red>
            //{
            //    new Red{Codigo = "1", Nombre="Aruba Primaria", Distrito = distritos[87]},
            //    new Red{Codigo = "2", Nombre="Benito Juarez Primaria", Distrito = distritos[87]},
            //    new Red{Codigo = "3", Nombre="Benito Juarez Secundaria", Distrito = distritos[87]},
            //    new Red{Codigo = "4", Nombre="Fidel Ferrer Primaria", Distrito = distritos[87]},
            //    new Red{Codigo = "5", Nombre="Fidel Ferrer Secundaria", Distrito = distritos[87]},
            //    new Red{Codigo = "6", Nombre="Francisco Ulises Dominguez Primaria", Distrito = distritos[87]},
            //    new Red{Codigo = "7", Nombre="Fray Ramon Pane Primaria", Distrito = distritos[87]},
            //    new Red{Codigo = "8", Nombre="Fray Ramon Pane Secundaria", Distrito = distritos[87]},
            //    new Red{Codigo = "9", Nombre="Jesus Maestro Primaria", Distrito = distritos[87]},
            //    new Red{Codigo = "10", Nombre="Jose Francisco Peña Gomez Secundaria", Distrito = distritos[87]},

            //};

            //var centros = new List<Centro>
            //{ 
            //    new Centro{Codigo="0", Nombre="Aruba", Red =redes[0]},
            //    new Centro{Codigo="1", Nombre="Manzano", Red =redes[0]},
            //    new Centro{Codigo="2", Nombre="La Milagrosa", Red =redes[0]},
            //    new Centro{Codigo="3", Nombre="Isidro Pérez Bello", Red =redes[0]},
            //    new Centro{Codigo="4", Nombre="Prof. Juan Bosch", Red =redes[0]},
            //    new Centro{Codigo="5", Nombre="Puerto Isabela", Red =redes[0]},
            //    new Centro{Codigo="6", Nombre="Benito Juárez", Red =redes[1]},
            //    new Centro{Codigo="7", Nombre="Santa Martha", Red =redes[1]},
            //    new Centro{Codigo="8", Nombre="Padre Arias (Pax)", Red =redes[1]},
            //    new Centro{Codigo="9", Nombre="Luz del Calvario (SODECA)", Red =redes[1]},
            //    new Centro{Codigo="10", Nombre="Luis Manuel Caraballo", Red =redes[1]},
            //    new Centro{Codigo="11", Nombre="Ángel Manuel Belliard", Red =redes[1]},
            //    new Centro{Codigo="12", Nombre="Liceo Matutino Benito Juárez", Red =redes[2]},
            //    new Centro{Codigo="13", Nombre="Liceo Vespertino Benito Juárez", Red =redes[2]},
            //    new Centro{Codigo="14", Nombre="Liceo Nocturno Francisco Ulises Domínguez", Red =redes[2]},
            //    new Centro{Codigo="15", Nombre="Liceo Nocturno Nuestra Señora de la Altagracia", Red =redes[2]},
            //    new Centro{Codigo="16", Nombre="Instituto Politécnico San Pablo Apóstol", Red =redes[2]},
            //    new Centro{Codigo="17", Nombre="Instituto Politécnico Los Ángeles Custodios", Red =redes[2]},
            //    new Centro{Codigo="18", Nombre="Fidel Ferrer", Red =redes[3]},
            //    new Centro{Codigo="19", Nombre="Francisco del Rosario Sánchez", Red =redes[3]},
            //    new Centro{Codigo="20", Nombre="Escuela Especial Santo Domingo", Red =redes[3]},
            //    new Centro{Codigo="21", Nombre="Marillac", Red =redes[3]},
            //    new Centro{Codigo="22", Nombre="María Fania Encarnación", Red =redes[3]},
            //    new Centro{Codigo="23", Nombre="Liceo Matutino Fidel Ferrer", Red =redes[4]},
            //    new Centro{Codigo="24", Nombre="Liceo Vespertino Fidel Ferrer", Red =redes[4]},
            //    new Centro{Codigo="25", Nombre="Liceo Nocturno La Fe", Red =redes[4]},
            //    new Centro{Codigo="26", Nombre="Escuela Nacional Artes y Oficios (ENAO)", Red =redes[4]},
            //    new Centro{Codigo="27", Nombre="Liceo Marillac", Red =redes[4]},
            //    new Centro{Codigo="28", Nombre="Instituto Politécnico Víctor Estrella Liz (La Perito)", Red =redes[4]},
            //    new Centro{Codigo="29", Nombre="Francisco Ulises Domínguez", Red =redes[5]},
            //    new Centro{Codigo="30", Nombre="San Pablo Apóstol", Red =redes[5]},
            //    new Centro{Codigo="31", Nombre="Los Ramírez", Red =redes[5]},
            //    new Centro{Codigo="32", Nombre="Casa de la Providencia", Red =redes[5]},
            //    new Centro{Codigo="33", Nombre="Juventud en Desarrollo", Red =redes[5]},
            //    new Centro{Codigo="34", Nombre="Parroquial Cristo Rey", Red =redes[5]},
            //    new Centro{Codigo="35", Nombre="Fray Ramón Pané", Red =redes[6]},
            //    new Centro{Codigo="36", Nombre="Proyecto Educación para Pensar", Red =redes[6]},
            //    new Centro{Codigo="37", Nombre="Malaquías Gil", Red =redes[6]},
            //    new Centro{Codigo="38", Nombre="Costa Rica", Red =redes[6]},
            //    new Centro{Codigo="39", Nombre="Perantuén", Red =redes[6]},
            //    new Centro{Codigo="40", Nombre="Renovación", Red =redes[6]},
            //    new Centro{Codigo="41", Nombre="Liceo Nocturno Fray Ramón Pané", Red =redes[7]},
            //    new Centro{Codigo="42", Nombre="Liceo Nocturno Malaquías Gil", Red =redes[7]},
            //    new Centro{Codigo="43", Nombre="Liceo Nocturno República de Costa Rica", Red =redes[7]},
            //    new Centro{Codigo="44", Nombre="Liceo Matutino Los Jardines", Red =redes[7]},
            //    new Centro{Codigo="45", Nombre="Jesús Maestro", Red =redes[8]},
            //    new Centro{Codigo="46", Nombre="José Bordas Valdez", Red =redes[8]},
            //    new Centro{Codigo="47", Nombre="José Francisco Peña Gómez", Red =redes[8]},
            //    new Centro{Codigo="48", Nombre="Mi Segundo Hogar", Red =redes[8]},
            //    new Centro{Codigo="49", Nombre="Julián Amparo", Red =redes[8]},
            //    new Centro{Codigo="50", Nombre="Liceo Matutino José Francisco Peña Gómez", Red =redes[9]},
            //    new Centro{Codigo="51", Nombre="Liceo Nocturno La Yuca", Red =redes[9]},
            //    new Centro{Codigo="52", Nombre="Liceo Nocturno Jesús Maestro", Red =redes[9]},
            //    new Centro{Codigo="53", Nombre="Liceo Vespertino José Francisco Peña Gómez", Red =redes[9]},

            //};

            //preguntas.ForEach(s => context.Preguntas.Add(s));
            //regionales.ForEach(s => context.Regionales.Add(s));
            //distritos.ForEach(s => context.Distritos.Add(s));
            //redes.ForEach(s => context.Redes.Add(s));
            //centros.ForEach(s => context.Centros.Add(s));
            //context.SaveChanges();

            var estudiantes = new List<Estudiante>
            {
               
            };

            var docentes = new List<Docente>
            {
                  



            };

            var docentesMaterias = new List<DocenteMateria>
            {
             
            };

            var personal = new List<PersonalAdministrativo>
            {
                //new PersonalAdministrativo{Codigo="00001", Persona = new Persona{Cedula = "001-1525181-1", Nombres="Laura",PrimerApellido="Norman",Sexo=PersonaSexo.Femenino }, FechaContratacion = DateTime.Now, Centro = centros[3]},
                //new PersonalAdministrativo{Codigo="00002", Persona = new Persona{Cedula = "001-1525181-2", Nombres="Nino",PrimerApellido="Olivetto",Sexo=PersonaSexo.Masculino }, FechaContratacion = DateTime.Now, Centro = centros[4]},
                //new PersonalAdministrativo{Codigo="00003", Persona = new Persona{Cedula = "001-1525181-3", Nombres="Astrid",PrimerApellido="Ferrara",Sexo=PersonaSexo.Femenino }, FechaContratacion = DateTime.Now, Centro = centros[5]}
            };

            //var ciclosFormativos = new List<CicloFormativo>
            //{
            //    new CicloFormativo{ FechaFinalizacion = DateTime.Now, FechaInicio = DateTime.Now, Nivel = NivelEducativo.Inicial, Tema = "Ofimática", 
            //        Inscripciones = new List<Inscripcion>
            //        {
            //            new Inscripcion{ Fecha = DateTime.Now, Participante = docentes[540].Persona, GrupoCicloFormativo = secciones[0], Rol = InscripcionRol.Participante},
            //            new Inscripcion{ Fecha = DateTime.Now, Participante = docentes[541].Persona, GrupoCicloFormativo = secciones[1], Rol = InscripcionRol.Participante}
            //        },
            //        ActividadesFormativas = new List<ActividadFormativa>()
            //    }
            //};

            //ciclosFormativos.ForEach(
            //    s => context.ActividadFormativaBases.ToList().ForEach(
            //        x => s.ActividadesFormativas.Add(new ActividadFormativa { ActividadFormativaBase = x, Duracion = x.Duracion, FechaInicio = s.FechaInicio, FechaFin = s.FechaFinalizacion }))
            //    );

            //ciclosFormativos.ForEach(s => context.CiclosFormativos.Add(s));
            //estudiantes.ForEach(s => context.Estudiantes.Add(s));
            //docentes.ForEach(s => context.Docentes.Add(s));
            //docentesMaterias.ForEach(s => context.DocenteMaterias.Add(s));
            //personal.ForEach(s => context.PersonalAdministrativo.Add(s));
            #endregion

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //foreach (var item in context.GetValidationErrors())
            //{
            //    foreach (var item2 in item.ValidationErrors)
            //    {
            //        sb.AppendLine(String.Format("{0} - {1}: {2}", item.Entry.Entity.GetType().Name, item2.PropertyName, item2.ErrorMessage));
            //    }
            //}
           // context.SaveChanges();
        }
    }
}