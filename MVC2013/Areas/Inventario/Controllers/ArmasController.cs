using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Areas.Inventario.Controllers;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Inventario.Controllers
{
    public class ArmasController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Armas
        public ActionResult Index()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == 3).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            ViewBag.id_bodega = db.Bodegas.Where(x => x.activo && !x.eliminado);
            if (rol == 8)
            {
                var armas = db.Armas.Where(x => x.activo && !x.eliminado && x.id_bodega == 1 && x.id_cliente == null && x.autorizada);//.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Arma_Estado).Include(a => a.Arma_Tipo).Include(a => a.Bodegas).Include(a => a.Calibres).Include(a => a.Marcas).Include(a => a.Modelos).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false);
                var countArmas = db.Armas.Where(x => x.activo && !x.eliminado && x.id_bodega == 1 && x.id_cliente==null && x.autorizada).Count();
                ViewBag.coutA = countArmas;
                return View(armas.ToList());
            }
            else {
                var armas = db.Armas.Where(x => x.activo && !x.eliminado && x.id_bodega == 4 && x.id_cliente == null && x.autorizada && x.trans_autorizada && x.comprometida);//.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Arma_Estado).Include(a => a.Arma_Tipo).Include(a => a.Bodegas).Include(a => a.Calibres).Include(a => a.Marcas).Include(a => a.Modelos).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false);
                var countArmas = db.Armas.Where(x => x.activo && !x.eliminado && x.id_bodega == 4 && x.id_cliente == null && x.autorizada && x.trans_autorizada && x.comprometida).Count();
                ViewBag.coutA = countArmas;
                return View(armas.ToList());
            }
        }

        // GET: Inventario/Armas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Armas armas = db.Armas.Find(id);
            if (armas == null)
            {
                return HttpNotFound();
            }
            return View(armas);
        }

        // GET: Inventario/Armas/Create
        public ActionResult Create()
        {
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x=> x.activo && !x.eliminado), "id_cliente", "nombre");
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado), "id_ubicacion", "direccion");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_arma_estado = new SelectList(db.Arma_Estado.Where(x => x.activo && !x.eliminado), "id_arma_estado", "descripcion");
            ViewBag.id_estado_tipo = new SelectList(db.Estado_Tipo.Where(x => x.activo && !x.eliminado), "id_estado_tipo", "descripcion");
            ViewBag.id_arma_estado_policia = new SelectList(db.Arma_Estado_Policia.Where(x => x.activo && !x.eliminado), "id_arma_estado_policia", "descripcion");
            ViewBag.id_arma_tipo = new SelectList(db.Arma_Tipo.Where(x => x.activo && !x.eliminado), "id_arma_tipo", "descripcion");
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion");
            ViewBag.id_calibre = new SelectList(db.Calibres.Where(x => x.activo && !x.eliminado), "id_calibre", "descripcion");
            ViewBag.id_marca = new SelectList(db.Marcas.Where(x => x.activo && !x.eliminado), "id_marca", "descripcion");
            ViewBag.id_modelo = new SelectList(db.Modelos.Where(x => x.activo && !x.eliminado), "id_modelo", "descripcion");
            ViewBag.id_proveedor = new SelectList(db.Proveedores.Where(x => x.activo && !x.eliminado), "id_proveedor", "descripcion");
            return View();
        }

        // POST: Inventario/Armas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_arma,id_arma_estado,id_arma_estado_policia,id_arma_tipo,id_proveedor,id_marca,id_calibre,cantidad_municion,registro,tenencia,portacion,id_cliente,id_ubicacion,observaciones,id_bodega,valor,marcage,id_modelo,huella,largo,fecha_vencimiento,fecha_robo,expediente,responsable,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Armas armas)
        {
            if (ModelState.IsValid)
            {
                db.Armas.Add(armas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre", armas.id_cliente);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado), "id_ubicacion", "direccion");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_modificacion);
            ViewBag.id_arma_estado = new SelectList(db.Arma_Estado.Where(x => x.activo && !x.eliminado), "id_arma_estado", "descripcion", armas.id_arma_estado);
            ViewBag.id_arma_estado_policia = new SelectList(db.Arma_Estado_Policia.Where(x => x.activo && !x.eliminado), "id_arma_estado_policia", "descripcion");
            ViewBag.id_arma_tipo = new SelectList(db.Arma_Tipo.Where(x => x.activo && !x.eliminado), "id_arma_tipo", "descripcion", armas.id_arma_tipo);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", armas.id_bodega);
            ViewBag.id_calibre = new SelectList(db.Calibres.Where(x => x.activo && !x.eliminado), "id_calibre", "descripcion", armas.id_calibre);
            ViewBag.id_marca = new SelectList(db.Marcas.Where(x => x.activo && !x.eliminado), "id_marca", "descripcion", armas.id_marca);
            ViewBag.id_modelo = new SelectList(db.Modelos.Where(x => x.activo && !x.eliminado), "id_modelo", "descripcion", armas.id_modelo);
            ViewBag.id_proveedor = new SelectList(db.Proveedores.Where(x => x.activo && !x.eliminado), "id_proveedor", "descripcion", armas.id_proveedor);
            return View(armas);
        }

        // GET: Inventario/Armas/Edit/5
        public ActionResult Edit(int? id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == 3).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Armas armas = db.Armas.Find(id);
            if (armas == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_estado_tipo = new SelectList(db.Estado_Tipo.Where(x => x.activo && !x.eliminado), "id_estado_tipo", "descripcion");
            ViewBag.id_arma_estado = new SelectList(db.Arma_Estado.Where(x => x.activo && !x.eliminado && x.id_arma_estado==armas.id_arma_estado), "id_arma_estado", "descripcion", armas.id_arma_estado);
            ViewBag.id_arma_estado_policia = new SelectList(db.Arma_Estado_Policia.Where(x => x.activo && !x.eliminado && x.id_arma_estado_policia==armas.id_arma_estado_policia), "id_arma_estado_policia", "descripcion");
            ViewBag.id_arma_tipo = new SelectList(db.Arma_Tipo.Where(x => x.activo && !x.eliminado && x.id_arma_tipo==armas.id_arma_tipo), "id_arma_tipo", "descripcion", armas.id_arma_tipo);
            ViewBag.id_proveedor = new SelectList(db.Proveedores.Where(x => x.activo && !x.eliminado && x.id_proveedor==armas.id_proveedor), "id_proveedor", "descripcion", armas.id_proveedor);
            ViewBag.id_marca = new SelectList(db.Marcas.Where(x => x.activo && !x.eliminado && x.id_marca==armas.id_marca), "id_marca", "descripcion", armas.id_marca);
            ViewBag.id_calibre = new SelectList(db.Calibres.Where(x => x.activo && !x.eliminado && x.id_calibre==armas.id_calibre), "id_calibre", "descripcion", armas.id_calibre);
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado && x.id_cliente==armas.id_cliente), "id_cliente", "nombre", armas.id_cliente);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado && x.id_ubicacion==armas.id_ubicacion), "id_ubicacion", "direccion");
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega==armas.id_bodega), "id_bodega", "descripcion", armas.id_bodega);
            ViewBag.id_modelo = new SelectList(db.Modelos.Where(x => x.activo && !x.eliminado && x.id_modelo==armas.id_modelo), "id_modelo", "descripcion", armas.id_modelo);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_modificacion);
            ViewBag.fechaVencimiento = Convert.ToDateTime(armas.fecha_vencimiento).ToString("yyyy-MM-dd");
            return View(armas);
        }

        // POST: Inventario/Armas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Armas armas)
        {
            if (ModelState.IsValid)
            {
                Armas armasEdit = db.Armas.Find(armas.id_arma);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                armasEdit.id_arma_estado = armas.id_arma_estado;
                armasEdit.id_arma_estado_policia = armas.id_arma_estado_policia;
                armasEdit.id_arma_tipo = armas.id_arma_tipo;
                armasEdit.id_proveedor = armas.id_proveedor;
                armasEdit.id_marca = armas.id_marca;
                armasEdit.id_calibre = armas.id_calibre;
                armasEdit.cantidad_municion = armas.cantidad_municion;
                armasEdit.registro = armas.registro;
                armasEdit.tenencia = armas.tenencia;
                armasEdit.portacion = armas.portacion;
                armasEdit.id_cliente = armas.id_cliente;
                armasEdit.id_ubicacion = armas.id_ubicacion;
                armasEdit.id_bodega = armas.id_bodega;
                armasEdit.valor = armas.valor;
                armasEdit.marcage = armas.marcage;
                armasEdit.id_modelo = armas.id_modelo;
                armasEdit.huella = armas.huella;
                armasEdit.largo = armas.largo;
                armasEdit.fecha_vencimiento = armas.fecha_vencimiento;
                armasEdit.fecha_robo = armas.fecha_robo;
                armasEdit.expediente = armas.expediente;
                armasEdit.responsable = armas.responsable;
                armasEdit.observaciones = armas.observaciones;
                armasEdit.activo = true;
                armasEdit.eliminado = false;
                armasEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                armasEdit.fecha_modificacion = DateTime.Now;

                db.Entry(armasEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre", armas.id_cliente);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado), "id_ubicacion", "direccion");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", armas.id_usuario_modificacion);
            ViewBag.id_arma_estado = new SelectList(db.Arma_Estado.Where(x => x.activo && !x.eliminado), "id_arma_estado", "descripcion", armas.id_arma_estado);
            ViewBag.id_arma_estado_policia = new SelectList(db.Arma_Estado_Policia.Where(x => x.activo && !x.eliminado), "id_arma_estado_policia", "descripcion");
            ViewBag.id_arma_tipo = new SelectList(db.Arma_Tipo.Where(x => x.activo && !x.eliminado), "id_arma_tipo", "descripcion", armas.id_arma_tipo);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", armas.id_bodega);
            ViewBag.id_calibre = new SelectList(db.Calibres.Where(x => x.activo && !x.eliminado), "id_calibre", "descripcion", armas.id_calibre);
            ViewBag.id_marca = new SelectList(db.Marcas.Where(x => x.activo && !x.eliminado), "id_marca", "descripcion", armas.id_marca);
            ViewBag.id_modelo = new SelectList(db.Modelos.Where(x => x.activo && !x.eliminado), "id_modelo", "descripcion", armas.id_modelo);
            ViewBag.id_proveedor = new SelectList(db.Proveedores, "id_proveedor", "descripcion", armas.id_proveedor);
            return View(armas);
        }

        // GET: Inventario/Armas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Armas armas = db.Armas.Find(id);
            if (armas == null)
            {
                return HttpNotFound();
            }
            return View(armas);
        }

        // POST: Inventario/Armas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Armas armas = db.Armas.Find(id);
            db.Armas.Remove(armas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Inventario/Armas
        public ActionResult Multiple()
        {
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre");
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado), "id_ubicacion", "direccion");
            ViewBag.id_arma_estado = new SelectList(db.Arma_Estado.Where(x => x.activo && !x.eliminado), "id_arma_estado", "descripcion");
            ViewBag.id_arma_tipo = new SelectList(db.Arma_Tipo.Where(x => x.activo && !x.eliminado), "id_arma_tipo", "descripcion");
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion");
            ViewBag.id_calibre = new SelectList(db.Calibres.Where(x => x.activo && !x.eliminado), "id_calibre", "descripcion");
            ViewBag.id_marca = new SelectList(db.Marcas.Where(x => x.activo && !x.eliminado), "id_marca", "descripcion");
            ViewBag.id_modelo = new SelectList(db.Modelos.Where(x => x.activo && !x.eliminado), "id_modelo", "descripcion");
            ViewBag.id_proveedor = new SelectList(db.Proveedores.Where(x => x.activo && !x.eliminado), "id_proveedor", "descripcion");
            ViewBag.id_uniforme = new SelectList(db.Bodega_Inventario_Uniformes.Where(x => x.activo && !x.eliminado), "id_uniforme", "id_uniforme");
            ViewBag.id_uniforme_tipo = new SelectList(db.Uniforme_Tipo.Where(x => x.activo && !x.eliminado), "id_uniforme_tipo", "descripcion");
            ViewBag.id_arma_estado_policia = new SelectList(db.Arma_Estado_Policia.Where(x => x.activo && !x.eliminado), "id_arma_estado_policia", "descripcion");
            ViewBag.id_articulo_tipo = new SelectList(db.Articulo_Tipo.Where(x => x.activo && !x.eliminado), "id_articulo_tipo", "descripcion");
            
            return View();
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
