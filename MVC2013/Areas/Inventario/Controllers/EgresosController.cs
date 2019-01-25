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
using MVC2013.Src.Seguridad.To;
using System.Transactions;
using System.Data.SqlClient;
using MVC2013.Src.Sdc.Reports; 

namespace MVC2013.Areas.Inventario.Controllers
{
    public class EgresosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Egresos
        public ActionResult Index()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            var egresos = db.Egresos.Include(e => e.Clientes).Include(e => e.Usuarios).Include(e => e.Usuarios1).Include(e => e.Usuarios2).Include(e => e.Usuarios3).Include(e => e.Bodegas).Include(e => e.Egreso_Estado).Include(e => e.Egreso_Tipo).Where(e => e.activo && !e.eliminado).OrderByDescending(i => i.fecha_creacion);
            return View(egresos.ToList());
        }

        public ActionResult Index_Autorizar()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            var egresos = db.Egresos.Include(e => e.Clientes).Include(e => e.Usuarios).Include(e => e.Usuarios1).Include(e => e.Usuarios2).Include(e => e.Usuarios3).Include(e => e.Bodegas).Include(e => e.Egreso_Estado).Include(e => e.Egreso_Tipo).Where(e => e.activo && !e.eliminado).Where(i => i.id_egreso_estado == 4);
            return View(egresos.ToList());
        }
        // GET: Inventario/Egresos/Details/5
        public ActionResult Details(int? id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id);
            if (egresos == null)
            {
                return HttpNotFound();
            }
            return View(egresos);
        }
        public ActionResult Details_Autorizar(int? id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id);
            if (egresos == null)
            {
                return HttpNotFound();
            }
            return View(egresos);
        }

        // GET: Inventario/Egresos/Create
        public ActionResult Create()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (rol == Convert.ToInt32(Catalogos.InventarioRoles.Bodeguero))
            {
                ViewBag.id_bodega_destino = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega == 4), "id_bodega", "descripcion");
                ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega==1), "id_bodega", "descripcion");
                ViewBag.id_egreso_tipo = new SelectList(db.Egreso_Tipo.Where(x => x.activo && !x.eliminado && x.id_egreso_tipo==1), "id_egreso_tipo", "descripcion");
            }
            else {
                ViewBag.id_traslado_tipo = new SelectList(db.Traslado_Tipo.Where(x => x.activo && !x.eliminado), "id_traslado_tipo", "descripcion");
                ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre");
                ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega == 4), "id_bodega", "descripcion");
                ViewBag.id_egreso_tipo = new SelectList(db.Egreso_Tipo.Where(x => x.activo && !x.eliminado && x.id_egreso_tipo==1), "id_egreso_tipo", "descripcion");
            }
            ViewBag.id_usuario_autorizacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_egreso_estado = new SelectList(db.Egreso_Estado.Where(x => x.activo && !x.eliminado), "id_egreso_estado", "descripcion");
            return View();
        }

        // POST: Inventario/Egresos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Egresos egresos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var roles = usuarioTO.usuario.Roles;
                int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
                ViewBag.rol = rol;
                egresos.id_egreso_estado = 1;
                egresos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                egresos.fecha_creacion = DateTime.Now;
                egresos.activo = true;
                egresos.eliminado = false;
                db.Egresos.Add(egresos);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = egresos.id_egreso });
            }

            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre", egresos.id_cliente);
            ViewBag.id_usuario_autorizacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_autorizacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", egresos.id_bodega);
            ViewBag.id_egreso_estado = new SelectList(db.Egreso_Estado.Where(x => x.activo && !x.eliminado), "id_egreso_estado", "descripcion", egresos.id_egreso_estado);
            ViewBag.id_egreso_tipo = new SelectList(db.Egreso_Tipo.Where(x => x.activo && !x.eliminado), "id_egreso_tipo", "descripcion", egresos.id_egreso_tipo);
            return View(egresos);
        }

        // GET: Inventario/Egresos/Edit/5
        public ActionResult Edit(int? id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id);
            if (egresos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre", egresos.id_cliente);
            ViewBag.id_usuario_autorizacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_autorizacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", egresos.id_bodega);
            ViewBag.id_egreso_estado = new SelectList(db.Egreso_Estado.Where(x => x.activo && !x.eliminado), "id_egreso_estado", "descripcion", egresos.id_egreso_estado);
            ViewBag.id_egreso_tipo = new SelectList(db.Egreso_Tipo.Where(x => x.activo && !x.eliminado), "id_egreso_tipo", "descripcion", egresos.id_egreso_tipo);
            return View(egresos);
        }

        // POST: Inventario/Egresos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_egreso,id_egreso_estado,id_bodega,id_cliente,id_usuario_autorizacion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_egreso_tipo")] Egresos egresos)
        {
            if (ModelState.IsValid)
            {
                Egresos egresosEdit = db.Egresos.Find(egresos.id_egreso);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                egresosEdit.id_egreso_tipo = egresos.id_egreso_tipo;
                egresosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                egresosEdit.fecha_modificacion = DateTime.Now;
                egresosEdit.activo = true;
                egresosEdit.eliminado = false;
                db.Entry(egresosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre", egresos.id_cliente);
            ViewBag.id_usuario_autorizacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_autorizacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", egresos.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", egresos.id_bodega);
            ViewBag.id_egreso_estado = new SelectList(db.Egreso_Estado.Where(x => x.activo && !x.eliminado), "id_egreso_estado", "descripcion", egresos.id_egreso_estado);
            ViewBag.id_egreso_tipo = new SelectList(db.Egreso_Tipo.Where(x => x.activo && !x.eliminado), "id_egreso_tipo", "descripcion", egresos.id_egreso_tipo);
            return View(egresos);
        }

        // GET: Inventario/Egresos/Delete/5
        public ActionResult Delete(int? id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id);
            if (egresos == null)
            {
                return HttpNotFound();
            }
            return View(egresos);
        }

        // POST: Inventario/Egresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Egresos egresos = db.Egresos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            egresos.id_egreso_estado = 3;
            egresos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            egresos.fecha_eliminacion = DateTime.Now;
            egresos.eliminado = true;
            egresos.activo = false;
            db.Entry(egresos).State = EntityState.Modified;
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

        // GET: Inventario/Armas
        public ActionResult sub_arma(int? id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id);
            ViewBag.id_egreso = db.Egresos.Find(id).id_egreso;
            if (egresos == null)
            {
                return HttpNotFound();
            }
            if (rol == 8) {
                var armas = db.Armas.Where(x=>x.activo && !x.eliminado && x.id_bodega==1 && !x.comprometida);//.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Arma_Estado).Include(a => a.Arma_Tipo).Include(a => a.Bodegas).Include(a => a.Calibres).Include(a => a.Marcas).Include(a => a.Modelos).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false).Where(a => a.id_bodega != null && a.id_cliente == null);
                return View(armas.ToList());
            }else
            {
                var armas = db.Armas.Where(x => x.activo && !x.eliminado && x.id_bodega == 4 && x.id_cliente == null && x.autorizada && x.trans_autorizada && x.comprometida && !x.proceso_retorno);//.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Arma_Estado).Include(a => a.Arma_Tipo).Include(a => a.Bodegas).Include(a => a.Calibres).Include(a => a.Marcas).Include(a => a.Modelos).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false).Where(a => a.id_bodega != null && a.id_cliente == null);
                return View(armas.ToList());
            }
        }

        public ActionResult enviar_arma(int id_arma, int id_egreso)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var eg_det = db.Egreso_Detalle.Where(e=> e.id_egreso==id_egreso && e.id_arma==id_arma && e.activo && !e.eliminado);
                    if (eg_det.Count() == 0)
                    {
                        Armas armas = db.Armas.Find(id_arma);
                        Egreso_Detalle egdet = new Egreso_Detalle();
                        egdet.id_egreso = id_egreso;
                        egdet.id_arma = id_arma;
                        egdet.cantidad = 1;
                        egdet.valor = armas.valor;
                        egdet.activo = true;
                        egdet.eliminado = false;
                        egdet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        egdet.fecha_creacion = DateTime.Now;
                        db.Egreso_Detalle.Add(egdet);
                        Egresos egreso = db.Egresos.Find(id_egreso);
                        egreso.id_egreso_estado = 2;
                        egreso.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egreso.fecha_modificacion = DateTime.Now;
                        db.Entry(egreso).State = EntityState.Modified;
                        if (rol == 8)
                        {
                            armas.comprometida = true;
                            armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            armas.fecha_modificacion = DateTime.Now;
                            db.Entry(armas).State = EntityState.Modified;
                        }
                        else {
                            armas.trans_cliente = true;
                            armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            armas.fecha_modificacion = DateTime.Now;
                            db.Entry(armas).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    else
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    
                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_egreso });
                }
            }

        }

        // GET: Inventario/Armas
        public ActionResult sub_articulo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id);

            if (egresos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_egreso = db.Egresos.Find(id).id_egreso;
            
            var articulos = db.Articulos.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Articulo_Tipo).Include(a => a.Bodegas).Include(a => a.Marcas).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false).Where(a => a.id_bodega != null && a.id_cliente == null);
            return View(articulos.ToList());
        }

        public ActionResult enviar_articulo(int id_articulo, int id_egreso)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var eg_det = db.Egreso_Detalle.Where(e => e.id_egreso == id_egreso && e.id_articulo == id_articulo && e.activo && !e.eliminado);
                    if (eg_det.Count() == 0)
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Articulos articulo = db.Articulos.Find(id_articulo);
                        Egreso_Detalle egdet = new Egreso_Detalle();
                        egdet.id_egreso = id_egreso;
                        egdet.id_articulo = id_articulo;
                        egdet.cantidad = 1;
                        egdet.valor = articulo.costo;
                        egdet.activo = true;
                        egdet.eliminado = false;
                        egdet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        egdet.fecha_creacion = DateTime.Now;
                        db.Egreso_Detalle.Add(egdet);
                        Egresos egreso = db.Egresos.Find(id_egreso);
                        egreso.id_egreso_estado = 2;
                        egreso.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egreso.fecha_modificacion = DateTime.Now;
                        db.Entry(egreso).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    else
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_egreso });
                }
            }

        }

        // GET: Inventario/Armas
        public ActionResult sub_municiones(int? id_egreso)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id_egreso == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);
            ViewBag.id_egreso = db.Egresos.Find(id_egreso).id_egreso;
            if (egresos == null)
            {
                return HttpNotFound();
            }
            if (rol == 8)
            {
                var bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega == 1 && x.autorizada).OrderBy(x=>x.id_municion);//.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Municiones).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == egresos.id_bodega);
                return View(bodega_Inventario_Municiones.ToList());
            }
            else {
                var bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega == 4 && !x.debitado).OrderBy(x => x.id_municion);//.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Municiones).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == egresos.id_bodega);
                return View(bodega_Inventario_Municiones.ToList());
            }
            //var bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Municiones).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == egresos.id_bodega);
            //return View(bodega_Inventario_Municiones.ToList());
        }

        // GET: Inventario/Armas
        public ActionResult enviar_municiones(int? id_municion, int? id_egreso)
        {
            if (id_egreso == null || id_municion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);
            Bodega_Inventario_Municiones bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(b=>b.activo && !b.eliminado && b.id_municion==id_municion && b.id_bodega==egresos.id_bodega && !b.debitado && b.autorizada).SingleOrDefault();
            if (bodega_Inventario_Municiones == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_egreso = db.Egresos.Find(id_egreso).id_egreso;
            ViewBag.existencia = bodega_Inventario_Municiones.existencia;
            ViewBag.comprometido = bodega_Inventario_Municiones.comprometido;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", bodega_Inventario_Municiones.id_bodega);
            ViewBag.id_municion = new SelectList(db.Municiones.Where(x => x.activo && !x.eliminado), "id_municion", "descripcion", bodega_Inventario_Municiones.id_municion);
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult enviar_municiones(int id_egreso, Bodega_Inventario_Municiones bodega_Inventario_Municiones)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            Egresos e = db.Egresos.Find(id_egreso);
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var eg_det = db.Egreso_Detalle.Where(ed => ed.id_egreso == id_egreso && ed.id_municion == bodega_Inventario_Municiones.id_municion && ed.activo && !ed.eliminado);
                    if (eg_det.Count() == 0)
                    {
                        Municiones municiones = db.Municiones.Find(bodega_Inventario_Municiones.id_municion);
                        Egreso_Detalle egdet = new Egreso_Detalle();
                        egdet.id_egreso = id_egreso;
                        egdet.id_municion = bodega_Inventario_Municiones.id_municion;
                        egdet.cantidad = bodega_Inventario_Municiones.existencia;
                        egdet.valor = municiones.costo;
                        egdet.activo = true;
                        egdet.eliminado = false;
                        egdet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        egdet.fecha_creacion = DateTime.Now;
                        db.Egreso_Detalle.Add(egdet);
                        db.SaveChanges();
                        if (rol == Convert.ToInt32(Catalogos.InventarioRoles.Bodeguero))
                        {
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_municion == egdet.id_municion && x.id_bodega == e.id_bodega && x.existencia>0 && !x.debitado).SingleOrDefault();
                            if (bim.existencia >= egdet.cantidad)
                            {
                                bim.comprometido = egdet.cantidad;
                                bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                bim.fecha_modificacion = DateTime.Now;
                                db.Entry(bim).State = EntityState.Modified;
                            }
                            Egreso_Detalle editEgresoDetalle = db.Egreso_Detalle.Find(egdet.id_egreso_detalle);
                            editEgresoDetalle.id_bodega_inventario_municion = bim.id_bodega_inventario_municiones;
                            db.Entry(editEgresoDetalle).State = EntityState.Modified;
                        //    else ModelState.AddModelError("vacio", "Ya no hay municiones en existencia");
                        }
                        else {
                            Bodega_Inventario_Municiones bimNew = new Bodega_Inventario_Municiones();
                            bimNew.id_municion = Convert.ToInt32(egdet.id_municion);
                            bimNew.id_bodega = e.id_bodega;
                            bimNew.id_egreso_detalle = egdet.id_egreso_detalle;
                            bimNew.existencia = 0;
                            bimNew.comprometido = egdet.cantidad;
                            bimNew.id_usuario_creacion=usuarioTO.usuario.id_usuario;
                            bimNew.id_cliente = e.id_cliente;
                            bimNew.fecha_creacion=DateTime.Now;
                            bimNew.activo = true;
                            bimNew.eliminado = false;
                            db.Bodega_Inventario_Municiones.Add(bimNew);
                            db.SaveChanges();
                            Egreso_Detalle editEgresoDetalle = db.Egreso_Detalle.Find(egdet.id_egreso_detalle);
                            editEgresoDetalle.id_bodega_inventario_municion = bimNew.id_bodega_inventario_municiones;
                            db.Entry(editEgresoDetalle).State = EntityState.Modified;
                        }
                        Egresos egreso = db.Egresos.Find(id_egreso);
                        egreso.id_egreso_estado = 2;
                        egreso.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egreso.fecha_modificacion = DateTime.Now;
                        db.Entry(egreso).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    else
                    {
                        if (rol == Convert.ToInt32(Catalogos.InventarioRoles.Bodeguero))
                        {
                            Egreso_Detalle egresoDetalleEdit = db.Egreso_Detalle.Where(ed => ed.id_egreso == id_egreso && ed.id_municion == bodega_Inventario_Municiones.id_municion && ed.activo && !ed.eliminado).SingleOrDefault();
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_municion == egresoDetalleEdit.id_municion && x.id_bodega == e.id_bodega).SingleOrDefault();

                            bim.comprometido = bodega_Inventario_Municiones.existencia;
                            bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            bim.fecha_modificacion = DateTime.Now;
                            db.Entry(bim).State = EntityState.Modified;

                            egresoDetalleEdit.cantidad = bodega_Inventario_Municiones.existencia;
                            db.Entry(egresoDetalleEdit).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else {
                            Egreso_Detalle egresoDetalleEdit = db.Egreso_Detalle.Where(ed => ed.id_egreso == id_egreso && ed.id_municion == bodega_Inventario_Municiones.id_municion && ed.activo && !ed.eliminado).SingleOrDefault();
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_municion == egresoDetalleEdit.id_municion && x.id_bodega == e.id_bodega && !x.autorizada && x.existencia==0 && x.comprometido>0).SingleOrDefault();

                            bim.comprometido = bodega_Inventario_Municiones.existencia;
                            bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            bim.fecha_modificacion = DateTime.Now;
                            db.Entry(bim).State = EntityState.Modified;

                            egresoDetalleEdit.cantidad = bodega_Inventario_Municiones.existencia;
                            db.Entry(egresoDetalleEdit).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_egreso });
                }
            }
        }

        // GET: Inventario/Armas
        public ActionResult sub_consumibles(int? id_egreso)
        {
            if (id_egreso == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);

            if (egresos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_egreso = db.Egresos.Find(id_egreso).id_egreso;
            var bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Consumibles).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == egresos.id_bodega);
            return View(bodega_Inventario_Consumibles.ToList());
        }

        // GET: Inventario/Armas
        public ActionResult enviar_consumibles(int? id_consumible, int? id_egreso)
        {
            if (id_egreso == null || id_consumible == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);
            Bodega_Inventario_Consumibles bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Where(b => b.id_consumible == id_consumible && b.id_bodega == egresos.id_bodega).SingleOrDefault();
            if (bodega_Inventario_Consumibles == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_egreso = db.Egresos.Find(id_egreso).id_egreso;
            ViewBag.existencia = bodega_Inventario_Consumibles.existencia;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", bodega_Inventario_Consumibles.id_bodega);
            ViewBag.id_consumible = new SelectList(db.Consumibles.Where(x => x.activo && !x.eliminado), "id_consumible", "descripcion", bodega_Inventario_Consumibles.id_consumible);
            return View(bodega_Inventario_Consumibles);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult enviar_consumibles(int id_egreso, [Bind(Include = "id_bodega_inventario_consumibles,id_consumible,id_bodega,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodega_Inventario_Consumibles bodega_Inventario_Consumibles)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var eg_det = db.Egreso_Detalle.Where(e => e.id_egreso == id_egreso && e.id_consumible == bodega_Inventario_Consumibles.id_consumible && e.activo && !e.eliminado);
                    if (eg_det.Count() == 0)
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Consumibles consumibles = db.Consumibles.Find(bodega_Inventario_Consumibles.id_consumible);
                        Egreso_Detalle egdet = new Egreso_Detalle();
                        egdet.id_egreso = id_egreso;
                        egdet.id_consumible = bodega_Inventario_Consumibles.id_consumible;
                        egdet.cantidad = bodega_Inventario_Consumibles.existencia;
                        egdet.valor = consumibles.costo;
                        egdet.activo = true;
                        egdet.eliminado = false;
                        egdet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        egdet.fecha_creacion = DateTime.Now;
                        db.Egreso_Detalle.Add(egdet);
                        Egresos egreso = db.Egresos.Find(id_egreso);
                        egreso.id_egreso_estado = 2;
                        egreso.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egreso.fecha_modificacion = DateTime.Now;
                        db.Entry(egreso).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    else
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_egreso });
                }
            }
        }

        // GET: Inventario/Armas
        public ActionResult sub_uniformes(int? id_egreso)
        {
            if (id_egreso == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);

            if (egresos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_egreso = db.Egresos.Find(id_egreso).id_egreso;
            var bodega_Inventario_Uniformes = db.Bodega_Inventario_Uniformes.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Uniformes).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == egresos.id_bodega);
            return View(bodega_Inventario_Uniformes.ToList());
        }

        // GET: Inventario/Armas
        public ActionResult enviar_uniformes(int? id_uniforme, int? id_egreso)
        {
            if (id_egreso == null || id_uniforme == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);
            Bodega_Inventario_Uniformes bodega_Inventario_Uniformes = db.Bodega_Inventario_Uniformes.Where(b => b.id_uniforme == id_uniforme && b.id_bodega == egresos.id_bodega).SingleOrDefault();
            if (bodega_Inventario_Uniformes == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_egreso = db.Egresos.Find(id_egreso).id_egreso;
            ViewBag.existencia = bodega_Inventario_Uniformes.existencia;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Uniformes.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Uniformes.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Uniformes.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", bodega_Inventario_Uniformes.id_bodega);
            ViewBag.id_uniforme = new SelectList(db.Uniformes.Where(x => x.activo && !x.eliminado), "id_uniforme", "descripcion", bodega_Inventario_Uniformes.id_uniforme);
            return View(bodega_Inventario_Uniformes);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult enviar_uniformes(int id_egreso, [Bind(Include = "id_bodega_inventario_uniformes,id_uniforme,id_bodega,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodega_Inventario_Uniformes bodega_Inventario_Uniformes)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var eg_det = db.Egreso_Detalle.Where(e => e.id_egreso == id_egreso && e.id_uniforme == bodega_Inventario_Uniformes.id_uniforme && e.activo && !e.eliminado);
                    if (eg_det.Count() == 0)
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Uniformes uniformes = db.Uniformes.Find(bodega_Inventario_Uniformes.id_uniforme);
                        Egreso_Detalle egdet = new Egreso_Detalle();
                        egdet.id_egreso = id_egreso;
                        egdet.id_uniforme = bodega_Inventario_Uniformes.id_uniforme;
                        egdet.cantidad = bodega_Inventario_Uniformes.existencia;
                        egdet.valor = uniformes.costo;
                        egdet.activo = true;
                        egdet.eliminado = false;
                        egdet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        egdet.fecha_creacion = DateTime.Now;
                        db.Egreso_Detalle.Add(egdet);
                        Egresos egreso = db.Egresos.Find(id_egreso);
                        egreso.id_egreso_estado = 2;
                        egreso.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egreso.fecha_modificacion = DateTime.Now;
                        db.Entry(egreso).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    else
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_egreso });
                }
            }
        }

        // GET: Inventario/Armas
        public ActionResult del_detalle(int id_egreso, int id_detalle)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Egreso_Detalle egdet = db.Egreso_Detalle.Where(x => x.activo && !x.eliminado && x.id_egreso_detalle == id_detalle && x.id_egreso == id_egreso).Single();
                        egdet.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                        egdet.fecha_eliminacion = DateTime.Now;
                        egdet.activo = false;
                        egdet.eliminado = true;
                        db.Entry(egdet).State = EntityState.Modified;
                        db.SaveChanges();
                        Egresos e = db.Egresos.Find(id_egreso);
                        if (rol == Convert.ToInt32(Catalogos.InventarioRoles.Bodeguero))
                        {
                            if (egdet.id_municion != null)
                            {
                                //Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega_inventario_municiones == egdet.id_bodega_inventario_municion).SingleOrDefault();
                                ////bim.comprometido -= Convert.ToInt32(egdet.cantidad);
                                //bim.activo = false;
                                //bim.eliminado = true;
                                //bim.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                                //bim.fecha_eliminacion = DateTime.Now;
                                ////bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                ////bim.fecha_modificacion = DateTime.Now;
                                Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega == e.id_bodega && x.id_municion == egdet.id_municion).SingleOrDefault();
                                bim.comprometido -= Convert.ToInt32(egdet.cantidad);
                                bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                bim.fecha_modificacion = DateTime.Now;
                                db.Entry(bim).State = EntityState.Modified;
                                db.SaveChanges();
                                //db.Entry(bim).State = EntityState.Modified;
                                //db.SaveChanges();

                            }
                            if (egdet.id_arma != null)
                            {
                                Armas armaEdit = db.Armas.Where(x => x.activo && !x.eliminado && x.id_arma == egdet.id_arma).SingleOrDefault();
                                armaEdit.comprometida = false;
                                db.Entry(armaEdit).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else {
                            if (egdet.id_municion != null)
                            {
                                Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega_inventario_municiones == egdet.id_bodega_inventario_municion).SingleOrDefault();
                                //bim.comprometido -= Convert.ToInt32(egdet.cantidad);
                                bim.activo = false;
                                bim.eliminado = true;
                                bim.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                                bim.fecha_eliminacion = DateTime.Now;
                                //bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                //bim.fecha_modificacion = DateTime.Now;
                                db.Entry(bim).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            if (egdet.id_arma != null)
                            {
                                Armas armaEdit = db.Armas.Where(x => x.activo && !x.eliminado && x.id_arma == egdet.id_arma).SingleOrDefault();
                                armaEdit.trans_cliente = false;
                                db.Entry(armaEdit).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                }

            }
            return RedirectToAction("Edit", new { id = id_egreso });

        }

        // GET: Inventario/Armas
        public ActionResult end_detalle(int? id_egreso)
        {
            if (id_egreso == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);

            if (egresos == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        //List<Egreso_Detalle> ed = db.Egreso_Detalle.Where(x => x.activo && !x.eliminado && x.id_egreso==egresos.id_egreso).ToList();
                        //foreach (var item in ed) {
                        //    Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x=>x.activo && !x.eliminado && x.id_municion==item.id_municion).SingleOrDefault();
                        //    bim.existencia -= Convert.ToInt32(item.cantidad);
                        //    bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        //    bim.fecha_modificacion = DateTime.Now;
                        //    db.Entry(bim).State = EntityState.Modified;
                        //}
                        egresos.id_egreso_estado = 4;
                        egresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egresos.fecha_modificacion = DateTime.Now;
                        egresos.activo = true;
                        egresos.eliminado = false;
                        db.Entry(egresos).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Index");
                    }
                }

            }
            return RedirectToAction("Edit", new { id = id_egreso });

        }

        // GET: Inventario/Armas
        public ActionResult Rechazar(int? id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id);

            if (egresos == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //List<Egreso_Detalle> ed = db.Egreso_Detalle.Where(x => x.activo && !x.eliminado && x.id_egreso == egresos.id_egreso).ToList();
                        //foreach (var item in ed)
                        //{
                        //    //updating armas
                        //    if (item.id_arma != null)
                        //    {
                        //        if (rol == 8)
                        //        {
                        //            Armas armas = db.Armas.Single(x => !x.eliminado && x.activo && x.id_arma == item.id_arma);
                        //            armas.comprometida = false;
                        //            armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        //            armas.fecha_modificacion = DateTime.Now;
                        //            db.Entry(armas).State = EntityState.Modified;
                        //        }
                        //        else {
                        //            Armas armas = db.Armas.Single(x => !x.eliminado && x.activo && x.id_arma == item.id_arma);
                        //            armas.trans_cliente = false;
                        //            armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        //            armas.fecha_modificacion = DateTime.Now;
                        //            db.Entry(armas).State = EntityState.Modified;
                        //        }
                        //    }

                        //    //updating municiones
                        //    if (item.id_municion != null)
                        //    {
                        //        Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_municion == item.id_municion && x.id_bodega==egresos.id_bodega).SingleOrDefault();
                        //        bim.comprometido -= Convert.ToInt32(item.cantidad);
                        //        bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        //        bim.fecha_modificacion = DateTime.Now;
                        //        db.Entry(bim).State = EntityState.Modified;
                        //    }
                        //}
                        egresos.id_egreso_estado = 7;
                        egresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egresos.fecha_modificacion = DateTime.Now;
                        egresos.activo = true;
                        egresos.eliminado = false;
                        db.Entry(egresos).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index_Autorizar");
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Index_Autorizar");
                    }
                }

            }
            return RedirectToAction("Index_Autorizar");

        }

        public ActionResult Aprobar(int? id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id);

            if (egresos == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        egresos.id_egreso_estado = 6;
                        egresos.id_usuario_autorizacion = usuarioTO.usuario.id_usuario;
                        egresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egresos.fecha_modificacion = DateTime.Now;
                        egresos.activo = true;
                        egresos.eliminado = false;
                        db.Entry(egresos).State = EntityState.Modified;

                        Transacciones_Inventario ti = new Transacciones_Inventario();
                        ti.activo = true;
                        ti.eliminado = false;
                        ti.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        ti.fecha_creacion = DateTime.Now;
                        db.Transacciones_Inventario.Add(ti);
                        //
                        foreach (Egreso_Detalle egdet in egresos.Egreso_Detalle)
                        {
                            if (egdet.activo && !egdet.eliminado)
                            {
                                if (egdet.id_arma != null)
                                {
                                    Armas armas = db.Armas.Single(x => !x.eliminado && x.activo && x.id_arma == egdet.id_arma);
                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 2;
                                    tid.cantidad = 1;
                                    tid.haber = armas.valor;
                                    tid.id_arma = armas.id_arma;
                                    tid.id_cliente = egresos.id_cliente;
                                    tid.id_bodega = egresos.id_bodega;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    if (egresos.id_egreso_tipo == 2)
                                    {
                                        armas.id_bodega = egresos.id_bodega; ;
                                        armas.id_cliente = egresos.id_cliente;
                                        armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        armas.fecha_modificacion = DateTime.Now;
                                        armas.activo = false;
                                        armas.eliminado = true;
                                        armas.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                                        armas.fecha_eliminacion = DateTime.Now;
                                        db.Entry(armas).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        if (rol == Convert.ToInt32(Catalogos.InventarioRoles.Bodeguero))
                                        {
                                            armas.trans_autorizada = true;
                                            armas.id_bodega = egresos.id_bodega_destino;
                                            armas.id_cliente = egresos.id_cliente;
                                            armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                            armas.fecha_modificacion = DateTime.Now;
                                            db.Entry(armas).State = EntityState.Modified;

                                            //Creando registro en logs para el arma
                                            Logs_Movimientos_Arma lma = new Logs_Movimientos_Arma();
                                            lma.id_arma = egdet.id_arma;
                                            lma.id_bodega_origen = egresos.id_bodega;
                                            lma.id_bodega_destino = egresos.id_bodega_destino;
                                            lma.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Egreso);
                                            lma.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.ToArmeria);
                                            lma.fecha_creacion = DateTime.Now;
                                            lma.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                            lma.activo = true;
                                            lma.eliminado = false;
                                            db.Logs_Movimientos_Arma.Add(lma);
                                            db.SaveChanges();
                                        }
                                        else {
                                            armas.autorizacion_cliente = true;
                                            armas.id_bodega = egresos.id_bodega_destino;
                                            armas.id_cliente = egresos.id_cliente;
                                            armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                            armas.fecha_modificacion = DateTime.Now;
                                            db.Entry(armas).State = EntityState.Modified;

                                            //Creando registro en logs para el arma
                                            Logs_Movimientos_Arma lma = new Logs_Movimientos_Arma();
                                            lma.id_arma = egdet.id_arma;
                                            lma.id_bodega_origen = egresos.id_bodega;
                                            lma.id_cliente = egresos.id_cliente;
                                            if (egdet.id_empleado != null)
                                            {
                                                lma.id_empleado = egdet.id_empleado;
                                                lma.fecha_asignacion = DateTime.Now;
                                            }
                                            else {
                                                lma.fecha_asignacion = null;
                                                lma.id_empleado = null;
                                            }
                                            lma.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Egreso);
                                            lma.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.ToCliente);
                                            lma.fecha_creacion = DateTime.Now;
                                            lma.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                            lma.activo = true;
                                            lma.eliminado = false;
                                            db.Logs_Movimientos_Arma.Add(lma);
                                            db.SaveChanges();
                                        }
                                    }
                                    
                                    db.SaveChanges();
                                }
                                if (egdet.id_articulo != null)
                                {
                                    Articulos articulos = db.Articulos.Single(x => !x.eliminado && x.activo && x.id_articulo == egdet.id_articulo);
                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 2;
                                    tid.cantidad = 1;
                                    tid.haber = articulos.costo;
                                    tid.id_articulo = articulos.id_articulo;
                                    tid.id_cliente = egresos.id_cliente;
                                    tid.id_bodega = egresos.id_bodega;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    if (egresos.id_egreso_tipo == 2)
                                    {
                                        articulos.id_bodega = null;
                                        articulos.id_cliente = egresos.id_cliente;
                                        articulos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        articulos.fecha_modificacion = DateTime.Now;
                                        articulos.activo = false;
                                        articulos.eliminado = true;
                                        articulos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                                        articulos.fecha_eliminacion = DateTime.Now;
                                    }
                                    else
                                    {
                                        articulos.id_bodega = null;
                                        articulos.id_cliente = egresos.id_cliente;
                                        articulos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        articulos.fecha_modificacion = DateTime.Now;
                                        db.Entry(articulos).State = EntityState.Modified;
                                    }
                                    
                                    
                                    db.SaveChanges();
                                }
                                if (egdet.id_consumible != null)
                                {
                                    Consumibles consumible = db.Consumibles.Single(x => !x.eliminado && x.activo && x.id_consumible == egdet.id_consumible);
                                    Bodega_Inventario_Consumibles bic = db.Bodega_Inventario_Consumibles.Single(x => !x.eliminado && x.activo && x.id_consumible == egdet.id_consumible && x.id_bodega == egresos.id_bodega);
                                    consumible.existencia -= Convert.ToInt32(egdet.cantidad);
                                    consumible.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    consumible.fecha_modificacion = DateTime.Now;
                                    db.Entry(consumible).State = EntityState.Modified;

                                    bic.existencia -= Convert.ToInt32(egdet.cantidad);
                                    bic.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    bic.fecha_modificacion = DateTime.Now;
                                    db.Entry(bic).State = EntityState.Modified;

                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 2;
                                    tid.cantidad = egdet.cantidad;
                                    tid.haber = consumible.costo;
                                    tid.id_consumible = consumible.id_consumible;
                                    tid.id_bodega = egresos.id_bodega;
                                    tid.id_cliente = egresos.id_cliente;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    db.SaveChanges();
                                }
                                if (egdet.id_municion != null)
                                {
                                    Municiones municion = db.Municiones.Single(x => !x.eliminado && x.activo && x.id_municion == egdet.id_municion);
                                    if (rol == Convert.ToInt32(Catalogos.InventarioRoles.Bodeguero))
                                    {
                                        Bodega_Inventario_Municiones bajas = db.Bodega_Inventario_Municiones.Single(x => !x.eliminado && x.activo && x.id_municion == egdet.id_municion && x.id_bodega == egresos.id_bodega && x.id_bodega_inventario_municiones==egdet.id_bodega_inventario_municion);
                                        municion.existencia -= Convert.ToInt32(egdet.cantidad);
                                        municion.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        municion.fecha_modificacion = DateTime.Now;
                                        db.Entry(municion).State = EntityState.Modified;

                                        //bajas(Se restand las municiones de la bodega de origen)
                                        bajas.existencia -= Convert.ToInt32(egdet.cantidad);
                                        bajas.comprometido -= Convert.ToInt32(egdet.cantidad);
                                        bajas.cantidad_debito = Convert.ToInt32(egdet.cantidad);
                                        bajas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        bajas.fecha_modificacion = DateTime.Now;
                                        db.Entry(bajas).State = EntityState.Modified;

                                        //Creando registro en logs para las municiones
                                        Logs_Movimientos_Municion lmm = new Logs_Movimientos_Municion();
                                        lmm.id_bodega_inventario_municion = bajas.id_bodega_inventario_municiones;
                                        lmm.id_bodega_origen = egresos.id_bodega;
                                        lmm.id_bodega_destino = egresos.id_bodega_destino;
                                        lmm.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Egreso);
                                        lmm.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.ToArmeria);
                                        lmm.cantidad = egdet.cantidad;
                                        lmm.fecha_creacion = DateTime.Now;
                                        lmm.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        lmm.activo = true;
                                        lmm.eliminado = false;
                                        db.Logs_Movimientos_Municion.Add(lmm);
                                        db.SaveChanges();

                                        //altas(si la bodega de destino ya tiene esa munición, se suma. De lo contrario se crea el registro con la bodega de destino y la munición)
                                        Bodega_Inventario_Municiones altas = db.Bodega_Inventario_Municiones.Where(x => !x.eliminado && x.activo && x.id_municion == egdet.id_municion && x.id_bodega == egresos.id_bodega_destino && x.existencia>0).SingleOrDefault();
                                        if (altas == null)
                                        {
                                            Bodega_Inventario_Municiones newInsert = new Bodega_Inventario_Municiones();
                                            newInsert.id_municion = Convert.ToInt32(egdet.id_municion);
                                            newInsert.id_bodega = Convert.ToInt32(egresos.id_bodega_destino);
                                            newInsert.existencia = Convert.ToInt32(egdet.cantidad);
                                            newInsert.comprometido = 0;
                                            newInsert.activo = true;
                                            newInsert.autorizada = true;
                                            newInsert.eliminado = false;
                                            newInsert.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                            newInsert.fecha_creacion = DateTime.Now;
                                            db.Bodega_Inventario_Municiones.Add(newInsert);
                                        }
                                        else {
                                            altas.existencia += Convert.ToInt32(egdet.cantidad);
                                            //altas.comprometido -= Convert.ToInt32(egdet.cantidad);
                                            altas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                            altas.fecha_modificacion = DateTime.Now;
                                            db.Entry(altas).State = EntityState.Modified;
                                        }
                                    }
                                    else {
                                        //bajas
                                        Bodega_Inventario_Municiones bajas = db.Bodega_Inventario_Municiones.Single(x => !x.eliminado && x.activo && x.id_municion == egdet.id_municion && x.id_bodega == egresos.id_bodega && x.existencia==0 && x.id_bodega_inventario_municiones==egdet.id_bodega_inventario_municion && !x.debitado);
                                        bajas.existencia -= Convert.ToInt32(egdet.cantidad);
                                        bajas.comprometido -= Convert.ToInt32(egdet.cantidad);
                                        bajas.debitado = true;
                                        bajas.cantidad_debito = Convert.ToInt32(egdet.cantidad);
                                        bajas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        bajas.fecha_modificacion = DateTime.Now;
                                        db.Entry(bajas).State = EntityState.Modified;

                                        //Creando registro en logs para las municiones
                                        Logs_Movimientos_Municion lmm = new Logs_Movimientos_Municion();
                                        lmm.id_bodega_inventario_municion = bajas.id_bodega_inventario_municiones;
                                        lmm.id_bodega_origen = egresos.id_bodega;
                                        lmm.id_cliente = egresos.id_cliente;
                                        if (egdet.id_empleado != null)
                                        {
                                            lmm.id_empleado = egdet.id_empleado;
                                            lmm.fecha_asignacion = DateTime.Now;
                                        }
                                        else {
                                            lmm.id_empleado = null;
                                            lmm.fecha_asignacion = null;
                                        }
                                        lmm.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Egreso);
                                        lmm.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.ToCliente);
                                        lmm.cantidad = egdet.cantidad;
                                        lmm.fecha_creacion = DateTime.Now;
                                        lmm.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        lmm.activo = true;
                                        lmm.eliminado = false;
                                        db.Logs_Movimientos_Municion.Add(lmm);
                                        db.SaveChanges();
                                    }

                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 2;
                                    tid.cantidad = egdet.cantidad;
                                    tid.haber = municion.costo;
                                    tid.id_municion = municion.id_municion;
                                    tid.id_bodega = egresos.id_bodega;
                                    tid.id_cliente = egresos.id_cliente;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    db.SaveChanges();
                                }
                                if (egdet.id_uniforme != null)
                                {
                                    Uniformes uniforme = db.Uniformes.Single(x => !x.eliminado && x.activo && x.id_uniforme == egdet.id_uniforme);
                                    Bodega_Inventario_Uniformes biu = db.Bodega_Inventario_Uniformes.Single(x => !x.eliminado && x.activo && x.id_uniforme == egdet.id_uniforme && x.id_bodega == egresos.id_bodega);

                                    uniforme.existencia -= Convert.ToInt32(egdet.cantidad);
                                    uniforme.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    uniforme.fecha_modificacion = DateTime.Now;
                                    db.Entry(uniforme).State = EntityState.Modified;

                                    biu.existencia -= Convert.ToInt32(egdet.cantidad);
                                    biu.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    biu.fecha_modificacion = DateTime.Now;
                                    db.Entry(biu).State = EntityState.Modified;

                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 2;
                                    tid.cantidad = egdet.cantidad;
                                    tid.haber = uniforme.costo;
                                    tid.id_uniforme = uniforme.id_uniforme;
                                    tid.id_bodega = egresos.id_bodega;
                                    tid.id_cliente = egresos.id_cliente;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    db.SaveChanges();
                                }
                            }
                        }


                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index_Autorizar");
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Index_Autorizar");
                    }
                }

            }
            return RedirectToAction("Index_Autorizar");

        }

        [GZipOrDeflate]
        public ActionResult asignar_empleado(int id_egreso, int id_detalle)
        {
            ViewBag.id_egreso = db.Egresos.Find(id_egreso).id_egreso;
            ViewBag.id_detalle = db.Egreso_Detalle.Find(id_detalle).id_egreso_detalle;
            
            //List<Empleado> empleado = db.Empleado.Where(e => e.activo && !e.eliminado).ToList();
            //ViewBag.empleadosList = empleado;
            return View();
        }

        public JsonResult Search(int? id_empleado, string primer_apellido, string primer_nombre, string segundo_apellido, string segundo_nombre, long? dpi)
        {
            List<object> resultados = new List<object>();
            var empleados = db.Empleado.Where(e => !e.eliminado && e.Contratacion.Where(c => c.id_empresa == 1 && c.id_estado_empleado == 1).Count() > 0);
            if (id_empleado.HasValue)
            {
                empleados = empleados.Where(e => e.id_empleado == id_empleado.Value && e.activo && !e.eliminado);
            }
            if (!String.IsNullOrEmpty(primer_apellido))
            {
                empleados = empleados.Where(e => e.primer_apellido.Contains(primer_apellido.Trim()));
            }
            if (!String.IsNullOrEmpty(primer_nombre))
            {
                empleados = empleados.Where(e => e.primer_nombre.Contains(primer_nombre.Trim()));
            }
            if (!String.IsNullOrEmpty(segundo_nombre))
            {
                empleados = empleados.Where(e => e.segundo_nombre.Contains(segundo_nombre.Trim()));
            }
            if (!String.IsNullOrEmpty(segundo_apellido))
            {
                empleados = empleados.Where(e => e.segundo_apellido.Contains(segundo_apellido.Trim()));
            }
            if (dpi.HasValue)
            {
                empleados = empleados.Where(e => e.dpi == dpi.Value);
            }
            foreach (var empleado in empleados)
            {
                resultados.Add(new
                {
                    empleado.id_empleado,
                    empleado.primer_nombre,
                    segundo_nombre = String.IsNullOrEmpty(empleado.segundo_nombre) ? "" : empleado.segundo_nombre,
                    empleado.primer_apellido,
                    segundo_apellido = String.IsNullOrEmpty(empleado.segundo_apellido) ? "" : empleado.segundo_apellido,
                    empleado.dpi
                });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Empleado_Asignado(int id_empleado, int id_egreso, int id_detalle)
        {

            if (id_empleado == null || id_egreso == null || id_detalle == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);
            Empleado empleado = db.Empleado.Find(id_empleado);
            Egreso_Detalle egdet = db.Egreso_Detalle.Find(id_detalle);

            if (egresos == null || empleado == null || egdet == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        egdet.id_empleado = id_empleado;
                        egdet.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egdet.fecha_modificacion = DateTime.Now;
                        db.Entry(egdet).State = EntityState.Modified;
                        if (egdet.id_municion != null)
                        {
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega_inventario_municiones == egdet.id_bodega_inventario_municion).SingleOrDefault();
                            bim.id_empleado = id_empleado;
                            db.Entry(bim).State = EntityState.Modified;
                        }
                        if (egdet.id_arma != null) {
                            Armas a = db.Armas.Where(x => x.activo && !x.eliminado && x.id_arma == egdet.id_arma).SingleOrDefault();
                            a.id_empleado = id_empleado;
                            db.Entry(a).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                }

            }
            return RedirectToAction("Edit", new { id = id_egreso });
        }

        public ActionResult desasignar_empleado(int id_egreso, int id_detalle)
        {

            if (id_egreso == null || id_detalle == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egresos egresos = db.Egresos.Find(id_egreso);
            Egreso_Detalle egdet = db.Egreso_Detalle.Find(id_detalle);

            if (egresos == null || egdet == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        egdet.id_empleado = null;
                        egdet.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        egdet.fecha_modificacion = DateTime.Now;
                        db.Entry(egdet).State = EntityState.Modified;
                        if (egdet.id_municion != null)
                        {
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega_inventario_municiones == egdet.id_bodega_inventario_municion).SingleOrDefault();
                            bim.id_empleado = null;
                            db.Entry(bim).State = EntityState.Modified;
                        }
                        if (egdet.id_arma != null) {
                            Armas a = db.Armas.Where(x => x.activo && !x.eliminado && x.id_arma == egdet.id_arma).SingleOrDefault();
                            a.id_empleado = null;
                            db.Entry(a).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_egreso });
                    }
                }

            }
            return RedirectToAction("Edit", new { id = id_egreso });
        }

        public FileStreamResult GetConstanciaEgreso(int id)
        {
            
            Egreso_Detalle egreso_detalle = db.Egreso_Detalle.SingleOrDefault(v => v.id_egreso_detalle == id && !v.eliminado);
            if (egreso_detalle == null)
            {
                return null;
            }
            string parametros = "&id_egreso_detalle=" + id;
            string reporte = "Dev_rpt_Constancia_Entrega_Inventario";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Constancia de Egreso " + egreso_detalle.id_egreso_detalle +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }
    }
}
 