using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Comun.Util;
using System.IO;
using System.Reflection;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class ControladoresController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Controladores
        public ActionResult Index()
        {
            var controladores = db.Controladores.Include(c => c.Aplicaciones);
            return View(controladores.ToList());
        }

        public ActionResult IndexR() {
            string s = "";
            var controllers = Assembly.GetExecutingAssembly().GetExportedTypes().Where(t => typeof(ControllerBase).IsAssignableFrom(t)).Select(t => t);
            List<MVC2013.Src.Comun.Helper.AreaControladorMetodo> listado = new List<MVC2013.Src.Comun.Helper.AreaControladorMetodo>();

            Aplicaciones app = new Aplicaciones();
            Controladores cont = new Controladores();
            Acciones acc = new Acciones();
            foreach (Type controller in controllers)
            {
                app = db.Aplicaciones.Where(a => a.ruta == controller.Namespace.Replace("MVC2013.Areas.", "").Replace(".Controllers", "").Replace("MVC2013", "").ToString()).Take(1).FirstOrDefault();
                bool ExisteAplicacion = false;
                bool ExisteControlador = false;
                bool ExisteMetodo = false;


                if (app == null)
                {
                    ExisteAplicacion = false;
                    ExisteControlador = false;
                }
                else {
                    ExisteAplicacion = true;
                    cont = db.Controladores.Where(c => c.nombre == controller.Name.Replace("Controller", "") && c.Aplicaciones.id_aplicacion == app.id_aplicacion).Take(1).FirstOrDefault();
                    if (cont == null)
                    {
                        ExisteControlador = false;
                        ExisteMetodo = false;

                    }
                    else {
                        ExisteControlador = true;
                    }
                }

                var actions = controller.GetMethods().Where(t => t.Name != "Dispose" && !t.IsSpecialName && t.DeclaringType.IsSubclassOf(typeof(ControllerBase)) && t.IsPublic && !t.IsStatic).ToList();
                foreach (var action in actions)
                {
                    //var myAttributes = action.GetCustomAttributes(false);
                    //for (int j = 0; j < myAttributes.Length; j++){
                        
                    //}
                    //    s += string.Format(" Area: {0} Controlador: {1} ActionName: {2}, Attribute: {3}<br>", controller.Module, controller.Name, action.Name, myAttributes[j]);
                    if (ExisteControlador) {
                        acc = db.Acciones.Where(c => c.nombre == action.Name && c.Controladores.id_controlador == cont.id_controlador ).Take(1).FirstOrDefault();
                        if (acc == null)
                        {
                            ExisteMetodo = false;
                        }
                        else
                        {
                            ExisteMetodo = true;
                        }
                    }
                    MVC2013.Src.Comun.Helper.AreaControladorMetodo busca = new Src.Comun.Helper.AreaControladorMetodo();
                    busca = listado.Where(l => l.Area == controller.Namespace.Replace("MVC2013.Areas.", "").Replace(".Controllers", "").Replace("MVC2013", "") && l.Contolador == controller.Name.Replace("Controller", "") && l.Metodo == action.Name).Take(1).FirstOrDefault();
                    //listado.Contains()
                    if (busca == null) {
                        listado.Add(new MVC2013.Src.Comun.Helper.AreaControladorMetodo
                        {
                            Area = controller.Namespace.Replace("MVC2013.Areas.", "").Replace(".Controllers", "").Replace("MVC2013", ""),
                            Contolador = controller.Name.Replace("Controller", ""),
                            Metodo = action.Name,
                            existeArea = ExisteAplicacion,
                            existeControlador = ExisteControlador,
                            existeMetodo = ExisteMetodo
                        });
                    }
                    


                }
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("Usuario: admin" + " Normal: 2311" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("2311"));
            sb.AppendLine("Usuario: daniel.lemus" + " Normal: 1" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("1"));
            sb.AppendLine("Usuario: Iris" + " Normal: 31010612" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("31010612"));
            sb.AppendLine("Usuario: raul.garcia" + " Normal: kalua" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("kalua"));
            sb.AppendLine("Usuario: edwin" + " Normal: 123." + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123."));
            sb.AppendLine("Usuario: angel.vasquez" + " Normal: 090694" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("090694"));
            sb.AppendLine("Usuario: Jvillegas" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Liz" + " Normal: 1976lizz" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("1976lizz"));
            sb.AppendLine("Usuario: Elver" + " Normal: 251093" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("251093"));
            sb.AppendLine("Usuario: ICordova" + " Normal: 04121931" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("04121931"));
            sb.AppendLine("Usuario: c" + " Normal: sadesa18" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("sadesa18"));
            sb.AppendLine("Usuario: Hjuarez" + " Normal: pronesis15" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("pronesis15"));
            sb.AppendLine("Usuario: Angel" + " Normal: 43237567" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("43237567"));
            sb.AppendLine("Usuario: Luis.Fernando" + " Normal: sambari" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("sambari"));
            sb.AppendLine("Usuario: Juan.Lemus" + " Normal: jlemusg" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("jlemusg"));
            sb.AppendLine("Usuario: Elver.Lopez" + " Normal: 251093" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("251093"));
            sb.AppendLine("Usuario: kmejia" + " Normal: 2552" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("2552"));
            sb.AppendLine("Usuario: Anita" + " Normal: Anitav1990" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Anitav1990"));
            sb.AppendLine("Usuario: Rcrespin" + " Normal: R2013" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("R2013"));
            sb.AppendLine("Usuario: Sanabria" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Hsalazar" + " Normal: Hsalazar" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Hsalazar"));
            sb.AppendLine("Usuario: Mlucas" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Jmejia" + " Normal: danisha" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("danisha"));
            sb.AppendLine("Usuario: Lgaitan" + " Normal: Lgaitan" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Lgaitan"));
            sb.AppendLine("Usuario: Usuario1" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: hborrayo" + " Normal: hborrayo" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("hborrayo"));
            sb.AppendLine("Usuario: FEscobar" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Frank" + " Normal: roberth" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("roberth"));
            sb.AppendLine("Usuario: celis" + " Normal: celis" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("celis"));
            sb.AppendLine("Usuario: Cristopher" + " Normal: 1285" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("1285"));
            sb.AppendLine("Usuario: recepcionista" + " Normal: recepcion" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("recepcion"));
            sb.AppendLine("Usuario: KFernandez" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: pollo" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: pulte" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: JGarcia" + " Normal: 56125943" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("56125943"));
            sb.AppendLine("Usuario: KarenHerrera" + " Normal: trienio" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("trienio"));
            sb.AppendLine("Usuario: JDavila" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: jsolis" + " Normal: Jsolis16*" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Jsolis16*"));
            sb.AppendLine("Usuario: Mroma" + " Normal: roma" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("roma"));
            sb.AppendLine("Usuario: WLopez" + " Normal: 1988" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("1988"));
            sb.AppendLine("Usuario: Damaris" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: SecretarioPortales" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: SecretarioCayala" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: SecretarioOakland" + " Normal: Octubre1993" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Octubre1993"));
            sb.AppendLine("Usuario: SecretarioPradera" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Yperez" + " Normal: protal" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("protal"));
            sb.AppendLine("Usuario: admin1" + " Normal: admin" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("admin"));
            sb.AppendLine("Usuario: ROlivares" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: SecretarioMiraflores" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Ddiaz" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: CSabala" + " Normal: angel2015" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("angel2015"));
            sb.AppendLine("Usuario: SRivas" + " Normal: 1364" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("1364"));
            sb.AppendLine("Usuario: WCamposeco" + " Normal: alex" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("alex"));
            sb.AppendLine("Usuario: Agodoy" + " Normal: Agodoy*" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Agodoy*"));
            sb.AppendLine("Usuario: ELima" + " Normal: papitolindo" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("papitolindo"));
            sb.AppendLine("Usuario: EEstupe" + " Normal: 4218" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("4218"));
            sb.AppendLine("Usuario: ESoto" + " Normal: inesa1015" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("inesa1015"));
            sb.AppendLine("Usuario: JDieguez" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Jefedeservicioops1" + " Normal: 12345" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("12345"));
            sb.AppendLine("Usuario: Jefedeservicioops2" + " Normal: gaitan16956" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("gaitan16956"));
            sb.AppendLine("Usuario: Jefedeservicioops3" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Jefedeservicioops4" + " Normal: 123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("123"));
            sb.AppendLine("Usuario: Jefeinstalaciones" + " Normal: diosmirey" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("diosmirey"));
            sb.AppendLine("Usuario: jcgarcia" + " Normal: 56125943" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("56125943"));
            sb.AppendLine("Usuario: gromero" + " Normal: rgloria11" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("rgloria11"));
            sb.AppendLine("Usuario: Dgil" + " Normal: Dgil2016*" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Dgil2016*"));
            sb.AppendLine("Usuario: efranco" + " Normal: efranco16" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("efranco16"));
            sb.AppendLine("Usuario: ralbeno" + " Normal: Albeno123" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Albeno123"));
            sb.AppendLine("Usuario: jlopez" + " Normal: Jlopez123*" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("Jlopez123*"));
            sb.AppendLine("Usuario: mgonzalez" + " Normal: 0" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("0"));
            sb.AppendLine("Usuario: pchicas" + " Normal: 17062303" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("17062303"));
            sb.AppendLine("Usuario: Jcordova" + " Normal: 33405441" + " Encriptado:" + MVC2013.Src.Seguridad.Util.CipherUtil.Encrypt("33405441")); ;

            ViewBag.pass = sb.ToString();
            ViewBag.listado = listado;

            return View();

        }
        
        // GET: Administracion/Controladores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Controladores controladores = db.Controladores.Find(id);
            if (controladores == null)
            {
                return HttpNotFound();
            }
            return View(controladores);
        }

        // GET: Administracion/Controladores/Create
        public ActionResult Create()
        {
            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre");
            return View();
        }

        // POST: Administracion/Controladores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_controlador,nombre,id_aplicacion")] Controladores controladores)
        {
            if (ModelState.IsValid)
            {
                db.Controladores.Add(controladores);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre", controladores.id_aplicacion);
            return View(controladores);
        }

        // GET: Administracion/Controladores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Controladores controladores = db.Controladores.Find(id);
            if (controladores == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre", controladores.id_aplicacion);
            return View(controladores);
        }

        // POST: Administracion/Controladores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_controlador,nombre,id_aplicacion")] Controladores controladores)
        {
            if (ModelState.IsValid)
            {
                db.Entry(controladores).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre", controladores.id_aplicacion);
            return View(controladores);
        }

        // GET: Administracion/Controladores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Controladores controladores = db.Controladores.Find(id);
            if (controladores == null)
            {
                return HttpNotFound();
            }
            return View(controladores);
        }

        // POST: Administracion/Controladores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Controladores controladores = db.Controladores.Find(id);
            db.Controladores.Remove(controladores);
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

        //////
        public ActionResult CreateControllerAccion(int idControlador, string accion)
        {
            Acciones accionVerification =
                db.Acciones.Where(x =>
                    x.id_controlador == idControlador &&
                    x.nombre == accion
                ).DefaultIfEmpty(null).Single();

            if (accionVerification == null)
            {
                Controladores controlador = db.Controladores.Find(idControlador);
                Acciones accionNew = new Acciones();
                accionNew.id_controlador = idControlador;
                accionNew.nombre = accion;
                db.Acciones.Add(accionNew);
                db.SaveChanges();
            }

            return new EmptyResult();
        }

        public ActionResult RemoveControllerAccion(int idAccion, int idControlador)
        {
            Acciones accion =
                db.Acciones.Where(x =>
                    x.id_accion == idAccion &&
                    x.id_controlador == idControlador
                ).DefaultIfEmpty(null).Single();

            if (accion != null)
            {
                db.Acciones.Remove(accion);
                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = idControlador }); ;
        }








    }
}
namespace MVC2013.Src.Comun.Helper
{
    public class AreaControladorMetodo {
            public string Area { get; set; }
            public string Contolador { get; set; }
            public string Metodo { get; set; }
            public bool existeArea { get; set; }
            public bool existeControlador { get; set; }
            public bool existeMetodo { get; set; }

        }
}