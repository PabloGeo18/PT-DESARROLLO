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
    public class TrasladosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Traslados
        public ActionResult Index()
        {
            var traslados = db.Traslados.Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2).Include(t => t.Bodegas).Include(t => t.Clientes).Where(t => t.activo && !t.eliminado).OrderByDescending(i => i.fecha_creacion);
            return View(traslados.ToList());
        }
        public ActionResult Index_Autorizar()
        {
            var traslados = db.Traslados.Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2).Include(t => t.Bodegas).Include(t => t.Clientes).Where(t => t.activo && !t.eliminado).Where(i => i.id_traslado_estado == 4);
            return View(traslados.ToList());
        }

        // GET: Inventario/Traslados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id);
            if (traslados == null)
            {
                return HttpNotFound();
            }
            return View(traslados);
        }
        public ActionResult Details_Autorizar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id);
            if (traslados == null)
            {
                return HttpNotFound();
            }
            return View(traslados);
        }

        // GET: Inventario/Traslados/Create
        public ActionResult Create()
        {
            ViewBag.id_traslado_tipo = new SelectList(db.Traslado_Tipo.Where(x => x.activo && !x.eliminado), "id_traslado_tipo", "descripcion");
            ViewBag.bodega_central = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega == 1), "id_bodega", "descripcion");
            ViewBag.armeria = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega == 4), "id_bodega", "descripcion");
            ViewBag.cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre");
            ViewBag.id_traslado_estado = new SelectList(db.Traslado_Estado.Where(x => x.activo && !x.eliminado), "id_traslado_estado", "descripcion");
            
            return View();
        }

        // POST: Inventario/Traslados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Traslados traslados)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                traslados.id_traslado_estado = 1;
                traslados.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                traslados.fecha_creacion = DateTime.Now;
                traslados.activo = true;
                traslados.eliminado = false;
                db.Traslados.Add(traslados);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = traslados.id_traslado });
            }
            ViewBag.id_traslado_estado = new SelectList(db.Traslado_Estado, "id_traslado_estado", "descripcion", traslados.id_traslado_estado);
            ViewBag.bodega_central = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega == 1), "id_bodega", "descripcion");
            ViewBag.armeria = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega == 4), "id_bodega", "descripcion");
            ViewBag.cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre");
            
            return View(traslados);
        }

        // GET: Inventario/Traslados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id);
            if (traslados == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_traslado_estado = new SelectList(db.Traslado_Estado, "id_traslado_estado", "descripcion", traslados.id_traslado_estado);
            ViewBag.bodega_central = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega == 1), "id_bodega", "descripcion");
            ViewBag.armeria = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega == 4), "id_bodega", "descripcion");
            ViewBag.cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre");
            ViewBag.test = traslados.Traslado_Detalle.Where(x => x.activo && !x.eliminado).ToList();
            return View(traslados);
        }

        // POST: Inventario/Traslados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_traslado,id_bodega_origen,id_bodega_destino,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Traslados traslados)
        {
            if (ModelState.IsValid)
            {
                Traslados trasladosEdit = db.Traslados.Find(traslados.id_traslado);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                trasladosEdit.id_bodega_destino = traslados.id_bodega_destino;
                trasladosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                trasladosEdit.fecha_modificacion = DateTime.Now;
                trasladosEdit.activo = true;
                trasladosEdit.eliminado = false;
                db.Entry(trasladosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", traslados.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", traslados.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", traslados.id_usuario_modificacion);
            ViewBag.id_bodega_destino = new SelectList(db.Bodegas, "id_bodega", "descripcion", traslados.id_bodega_destino);
            ViewBag.id_bodega_origen = new SelectList(db.Bodegas, "id_bodega", "descripcion", traslados.id_bodega_origen);
            return View(traslados);
        }

        // GET: Inventario/Traslados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id);
            if (traslados == null)
            {
                return HttpNotFound();
            }
            return View(traslados);
        }

        // POST: Inventario/Traslados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Traslados traslados = db.Traslados.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            traslados.id_traslado_estado = 3;
            traslados.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            traslados.fecha_eliminacion = DateTime.Now;
            traslados.eliminado = true;
            traslados.activo = false;
            db.Entry(traslados).State = EntityState.Modified;
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
        public ActionResult move_arma(int? id, int? cliente)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id);

            if (traslados == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_traslado = db.Traslados.Find(id).id_traslado;
            ViewBag.traslado_tipo = db.Traslados.Find(id).Traslado_Tipo.id_traslado_tipo;
            if (cliente != null)
            {
                ViewBag.cliente = cliente;
            }
            else {
                ViewBag.cliente = 0;
            }
            if (traslados.id_traslado_tipo == 1)
            {
                var armas = db.Armas.Where(x => x.activo && !x.eliminado && x.id_cliente == null && x.autorizada && x.trans_autorizada && x.comprometida && !x.trans_cliente && !x.autorizacion_cliente);//.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Arma_Estado).Include(a => a.Arma_Tipo).Include(a => a.Bodegas).Include(a => a.Calibres).Include(a => a.Marcas).Include(a => a.Modelos).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false).Where(a => a.id_bodega == traslados.id_bodega_origen && a.id_cliente == null);
                return View(armas.ToList());
            }
            else {
                var armas = db.Armas.Where(x => x.activo && !x.eliminado && x.id_cliente == cliente && x.trans_cliente && x.autorizacion_cliente);//.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Arma_Estado).Include(a => a.Arma_Tipo).Include(a => a.Bodegas).Include(a => a.Calibres).Include(a => a.Marcas).Include(a => a.Modelos).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false).Where(a => a.id_bodega == traslados.id_bodega_origen && a.id_cliente == null);
                return View(armas.ToList());
            }
        }

        public ActionResult enviar_arma(int id_arma, int id_traslado)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var tra_det = db.Traslado_Detalle.Where(t => t.id_traslado == id_traslado && t.id_arma == id_arma && t.activo && !t.eliminado);
                    if (tra_det.Count() == 0)
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Armas armas = db.Armas.Find(id_arma);
                        //se crea el detalle del traslado
                        Traslado_Detalle tradet = new Traslado_Detalle();
                        tradet.id_traslado = id_traslado;
                        tradet.id_arma = id_arma;
                        tradet.cantidad = 1;
                        tradet.valor = armas.valor;
                        tradet.activo = true;
                        tradet.eliminado = false;
                        tradet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        tradet.fecha_creacion = DateTime.Now;
                        db.Traslado_Detalle.Add(tradet);
                        //actualizando estado del traslado
                        Traslados traslado = db.Traslados.Find(id_traslado);
                        traslado.id_traslado_estado = 2;
                        traslado.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        traslado.fecha_modificacion = DateTime.Now;
                        db.Entry(traslado).State = EntityState.Modified;
                        //actualizando armas
                        if (traslado.id_traslado_tipo == Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral))
                        {
                            armas.proceso_retorno = true;
                        }
                        else {
                            armas.trans_cliente = false;
                            armas.autorizacion_cliente = false;
                            //armas.id_cliente = null;
                            //armas.id_bodega = traslado.id_bodega_destino;
                        }
                        armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        armas.fecha_modificacion = DateTime.Now;
                        db.Entry(armas).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }
                    else
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_traslado });
                }
            }

        }

        // GET: Inventario/Armas
        public ActionResult move_articulo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id);

            if (traslados == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_traslado = db.Traslados.Find(id).id_traslado;
            var articulos = db.Articulos.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Articulo_Tipo).Include(a => a.Bodegas).Include(a => a.Marcas).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false).Where(a => a.id_bodega == traslados.id_bodega_origen && a.id_cliente == null);
            return View(articulos.ToList());
        }

        public ActionResult enviar_articulo(int id_articulo, int id_traslado)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var tra_det = db.Traslado_Detalle.Where(t => t.id_traslado == id_traslado && t.id_articulo == id_articulo && t.activo && !t.eliminado);
                    if (tra_det.Count() == 0)
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Articulos articulo = db.Articulos.Find(id_articulo);
                        Traslado_Detalle tradet = new Traslado_Detalle();
                        tradet.id_traslado = id_traslado;
                        tradet.id_articulo = id_articulo;
                        tradet.cantidad = 1;
                        tradet.valor = articulo.costo;
                        tradet.activo = true;
                        tradet.eliminado = false;
                        tradet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        tradet.fecha_creacion = DateTime.Now;
                        db.Traslado_Detalle.Add(tradet);
                        Traslados traslado = db.Traslados.Find(id_traslado);
                        traslado.id_traslado_estado = 2;
                        traslado.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        traslado.fecha_modificacion = DateTime.Now;
                        db.Entry(traslado).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }
                    else
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_traslado });
                }
            }

        }

        // GET: Inventario/Armas
        public ActionResult move_municiones(int? id_traslado, int? id_cliente)
        {
            if (id_traslado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id_traslado);

            if (traslados == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_traslado = db.Traslados.Find(id_traslado).id_traslado;
            if (id_cliente != null)
            {
                if (id_cliente > 0) {
                    ViewBag.id_cliente = db.Traslados.Find(id_traslado).id_cliente_origen;
                }
                else
                {
                    ViewBag.id_cliente = 0;
                }
            }
            else
            {
                ViewBag.id_cliente = 0;
            }
            ViewBag.traslado_tipo = db.Traslados.Find(id_traslado).id_traslado_tipo;
            int Armeria = Convert.ToInt32(Catalogos.InventarioBodegas.Armeria);
            if (traslados.id_traslado_tipo == Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral))
            {
                var bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega == Armeria).ToList();
                return View(bodega_Inventario_Municiones);
            }
            else {
                var bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.debitado && !x.autorizada && x.id_bodega == Armeria && x.id_cliente == id_cliente).ToList();
                return View(bodega_Inventario_Municiones);
            }
        }

        // GET: Inventario/Armas
        public ActionResult enviar_municiones(int? id_municion, int? id_traslado, int? id_bodega_inventario_municiones, int? id_cliente)
        {
            if (id_traslado == null || id_municion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id_traslado);
            if (traslados.id_traslado_tipo == Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral))
            {
                List<Bodega_Inventario_Municiones> bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(b => b.activo && !b.eliminado && b.id_municion == id_municion && b.id_bodega == traslados.id_bodega_origen).ToList();
                if (bodega_Inventario_Municiones == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id_cliente = id_cliente;
                ViewBag.existencia = bodega_Inventario_Municiones.Sum(x=>x.existencia);
                ViewBag.comprometido = (bodega_Inventario_Municiones.Sum(x=>x.cantidad_debito)+bodega_Inventario_Municiones.Sum(x=>x.retornando));
                ViewBag.id_traslado = db.Traslados.Find(id_traslado).id_traslado;
                ViewBag.bodega_inventario = 0;
            }
            else {
                //Bodega_Inventario_Municiones bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(b => b.activo && !b.eliminado && b.id_municion == id_municion && b.id_bodega == traslados.id_bodega_destino && b.id_cliente==traslados.id_cliente_origen).SingleOrDefault();
                Bodega_Inventario_Municiones bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(b => b.activo && !b.eliminado && b.id_bodega_inventario_municiones == id_bodega_inventario_municiones).SingleOrDefault();
                if (bodega_Inventario_Municiones == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id_cliente = id_cliente;
                ViewBag.existencia = bodega_Inventario_Municiones.cantidad_debito;
                ViewBag.comprometido = bodega_Inventario_Municiones.retornando;
                ViewBag.id_traslado = db.Traslados.Find(id_traslado).id_traslado;
                ViewBag.bodega_inventario = bodega_Inventario_Municiones.id_bodega_inventario_municiones;
            }
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult enviar_municiones(int id_traslado, int? id_bodega_inventario_municiones, Bodega_Inventario_Municiones bodega_Inventario_Municiones)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Traslados traslado = db.Traslados.Find(id_traslado);
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var tra_det = db.Traslado_Detalle.Where(t => t.id_traslado == id_traslado && t.id_municion == bodega_Inventario_Municiones.id_municion && t.activo && !t.eliminado);
                    if (tra_det.Count() == 0)
                    {
                        Municiones municiones = db.Municiones.Find(bodega_Inventario_Municiones.id_municion);
                        Traslado_Detalle tradet = new Traslado_Detalle();
                        tradet.id_traslado = id_traslado;
                        tradet.id_municion = bodega_Inventario_Municiones.id_municion;
                        tradet.cantidad = bodega_Inventario_Municiones.existencia;
                        tradet.valor = municiones.costo;
                        tradet.activo = true;
                        tradet.eliminado = false;
                        tradet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        tradet.fecha_creacion = DateTime.Now;
                        db.Traslado_Detalle.Add(tradet);
                        traslado.id_traslado_estado = 2;
                        traslado.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        traslado.fecha_modificacion = DateTime.Now;
                        db.Entry(traslado).State = EntityState.Modified;
                        db.SaveChanges();
                        if (traslado.id_traslado_tipo == (Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral)))
                        {
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.existencia > 0 && x.id_municion == tradet.id_municion && x.id_bodega == traslado.id_bodega_origen).SingleOrDefault();
                            if (bim == null)
                            {
                                return HttpNotFound();
                            }
                            //if (bim.existencia >= tradet.cantidad)
                            //{
                            bim.retornando = tradet.cantidad;
                            bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            bim.fecha_modificacion = DateTime.Now;
                            db.Entry(bim).State = EntityState.Modified;
                            db.SaveChanges();
                            tradet.id_bodega_inventario_municion = bim.id_bodega_inventario_municiones;
                            db.Entry(tradet).State = EntityState.Modified;
                            db.SaveChanges();
                            //}
                        }
                        else {
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega_inventario_municiones == id_bodega_inventario_municiones ).SingleOrDefault();
                            //Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_municion == tradet.id_municion && x.id_cliente==traslado.id_cliente_origen).SingleOrDefault();
                            if (bim == null)
                            {
                                return HttpNotFound();
                            }
                            bim.retornando = tradet.cantidad;
                            bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            bim.fecha_modificacion = DateTime.Now;
                            db.Entry(bim).State = EntityState.Modified;

                            tradet.id_bodega_inventario_municion = bim.id_bodega_inventario_municiones;
                            db.Entry(tradet).State = EntityState.Modified;
                            //}
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }
                    else
                    {
                        if (traslado.id_traslado_tipo == (Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral)))
                        {
                            Traslado_Detalle trasladoDetalleEdit = db.Traslado_Detalle.Where(td => td.id_traslado == id_traslado && td.id_municion == bodega_Inventario_Municiones.id_municion && td.activo && !td.eliminado).SingleOrDefault();
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_municion == trasladoDetalleEdit.id_municion && x.id_bodega == traslado.id_bodega_origen && x.autorizada && !x.debitado).SingleOrDefault();

                            bim.retornando = bodega_Inventario_Municiones.existencia;
                            bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            bim.fecha_modificacion = DateTime.Now;
                            db.Entry(bim).State = EntityState.Modified;

                            trasladoDetalleEdit.cantidad = bodega_Inventario_Municiones.existencia;
                            db.Entry(trasladoDetalleEdit).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            Traslado_Detalle trasladoDetalleEdit = db.Traslado_Detalle.Where(td => td.id_traslado == id_traslado && td.id_municion == bodega_Inventario_Municiones.id_municion && td.activo && !td.eliminado).SingleOrDefault();
                            Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_municion == trasladoDetalleEdit.id_municion && x.id_bodega == traslado.id_bodega_destino && !x.autorizada && x.debitado && x.retornando > 0).SingleOrDefault();

                            bim.retornando = bodega_Inventario_Municiones.existencia;
                            bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            bim.fecha_modificacion = DateTime.Now;
                            db.Entry(bim).State = EntityState.Modified;

                            trasladoDetalleEdit.cantidad = bodega_Inventario_Municiones.existencia;
                            db.Entry(trasladoDetalleEdit).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_traslado });
                }
            }
        }
        
        // GET: Inventario/Armas
        public ActionResult move_consumibles(int? id_traslado)
        {
            if (id_traslado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id_traslado);

            if (traslados == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_traslado = db.Traslados.Find(id_traslado).id_traslado;
            var bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Consumibles).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == traslados.id_bodega_origen);
            return View(bodega_Inventario_Consumibles.ToList());
        }

        // GET: Inventario/Armas
        public ActionResult enviar_consumibles(int? id_consumible, int? id_traslado)
        {
            if (id_traslado == null || id_consumible == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id_traslado);
            Bodega_Inventario_Consumibles bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Where(b => b.id_consumible == id_consumible && b.id_bodega == traslados.id_bodega_origen).SingleOrDefault();
            if (bodega_Inventario_Consumibles == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_traslado = db.Traslados.Find(id_traslado).id_traslado;
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
        public ActionResult enviar_consumibles(int id_traslado, [Bind(Include = "id_bodega_inventario_consumibles,id_consumible,id_bodega,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodega_Inventario_Consumibles bodega_Inventario_Consumibles)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var tra_det = db.Traslado_Detalle.Where(t => t.id_traslado == id_traslado && t.id_consumible == bodega_Inventario_Consumibles.id_consumible && t.activo && !t.eliminado);
                    if (tra_det.Count() == 0)
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Consumibles consumibles = db.Consumibles.Find(bodega_Inventario_Consumibles.id_consumible);
                        Traslado_Detalle tradet = new Traslado_Detalle();
                        tradet.id_traslado = id_traslado;
                        tradet.id_consumible = bodega_Inventario_Consumibles.id_consumible;
                        tradet.cantidad = bodega_Inventario_Consumibles.existencia;
                        tradet.valor = consumibles.costo;
                        tradet.activo = true;
                        tradet.eliminado = false;
                        tradet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        tradet.fecha_creacion = DateTime.Now;
                        db.Traslado_Detalle.Add(tradet);
                        Traslados traslado = db.Traslados.Find(id_traslado);
                        traslado.id_traslado_estado = 2;
                        traslado.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        traslado.fecha_modificacion = DateTime.Now;
                        db.Entry(traslado).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }
                    else
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_traslado });
                }
            }
        }

        // GET: Inventario/Armas
        public ActionResult move_uniformes(int? id_traslado)
        {
            if (id_traslado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id_traslado);

            if (traslados == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_traslado = db.Traslados.Find(id_traslado).id_traslado;
            var bodega_Inventario_Uniformes = db.Bodega_Inventario_Uniformes.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Uniformes).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == traslados.id_bodega_origen);
            return View(bodega_Inventario_Uniformes.ToList());
        }

        // GET: Inventario/Armas
        public ActionResult enviar_uniformes(int? id_uniforme, int? id_traslado)
        {
            if (id_traslado == null || id_uniforme == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id_traslado);
            Bodega_Inventario_Uniformes bodega_Inventario_Uniformes = db.Bodega_Inventario_Uniformes.Where(b => b.id_uniforme == id_uniforme && b.id_bodega == traslados.id_bodega_origen).SingleOrDefault();
            if (bodega_Inventario_Uniformes == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_traslado = db.Traslados.Find(id_traslado).id_traslado;
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
        public ActionResult enviar_uniformes(int id_traslado, [Bind(Include = "id_bodega_inventario_uniformes,id_uniforme,id_bodega,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodega_Inventario_Uniformes bodega_Inventario_Uniformes)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var tra_det = db.Traslado_Detalle.Where(t => t.id_traslado == id_traslado && t.id_uniforme == bodega_Inventario_Uniformes.id_uniforme && t.activo && !t.eliminado);
                    if (tra_det.Count() == 0)
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Uniformes uniformes = db.Uniformes.Find(bodega_Inventario_Uniformes.id_uniforme);
                        Traslado_Detalle tradet = new Traslado_Detalle();
                        tradet.id_traslado = id_traslado;
                        tradet.id_uniforme = bodega_Inventario_Uniformes.id_uniforme;
                        tradet.cantidad = bodega_Inventario_Uniformes.existencia;
                        tradet.valor = uniformes.costo;
                        tradet.activo = true;
                        tradet.eliminado = false;
                        tradet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        tradet.fecha_creacion = DateTime.Now;
                        db.Traslado_Detalle.Add(tradet);
                        Traslados traslado = db.Traslados.Find(id_traslado);
                        traslado.id_traslado_estado = 2;
                        traslado.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        traslado.fecha_modificacion = DateTime.Now;
                        db.Entry(traslado).State = EntityState.Modified;
                        db.SaveChanges(); 
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }
                    else
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }

                }
                catch
                {
                    tran.Rollback();
                    return RedirectToAction("Edit", new { id = id_traslado });
                }
            }
        }

        // GET: Inventario/Armas
        public ActionResult del_detalle(int id_traslado, int id_detalle)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Traslados traslado = db.Traslados.Find(id_traslado);
                        Traslado_Detalle tradet = db.Traslado_Detalle.Where(x => x.activo && !x.eliminado && x.id_traslado_detalle == id_detalle && x.id_traslado == id_traslado).Single();
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        tradet.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                        tradet.fecha_eliminacion = DateTime.Now;
                        tradet.activo = false;
                        tradet.eliminado = true;
                        db.Entry(tradet).State = EntityState.Modified;
                        if (tradet.id_arma != null) {
                            Armas arma = db.Armas.Find(tradet.id_arma);
                            if (traslado.id_traslado_tipo == Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral))
                            {
                                arma.proceso_retorno = false;
                            }
                            else {
                                arma.trans_cliente = true;
                                arma.autorizacion_cliente = true;
                                arma.id_cliente = traslado.id_cliente_origen;
                                arma.id_bodega = null;
                            }
                            arma.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                            arma.fecha_modificacion = DateTime.Now;
                            db.Entry(arma).State = EntityState.Modified;
                        }
                        if (tradet.id_municion != null) {
                            if (traslado.id_traslado_tipo == Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral))
                            {
                                Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_municion == tradet.id_municion && x.id_bodega == traslado.id_bodega_origen && x.autorizada && !x.debitado).SingleOrDefault();
                                bim.retornando -= Convert.ToInt32(tradet.cantidad);
                                bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                bim.fecha_modificacion = DateTime.Now;
                                db.Entry(bim).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else {
                                Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega_inventario_municiones==tradet.id_bodega_inventario_municion).SingleOrDefault();
                                bim.retornando -= Convert.ToInt32(tradet.cantidad);
                                bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                bim.fecha_modificacion = DateTime.Now;
                                db.Entry(bim).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_traslado });
                    }
                }

            }
            return RedirectToAction("Edit", new { id = id_traslado });

        }

        // GET: Inventario/Armas
        public ActionResult end_detalle(int? id_traslado)
        {
            if (id_traslado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id_traslado);

            if (traslados == null)
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
                        traslados.id_traslado_estado = 4;
                        traslados.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        traslados.fecha_modificacion = DateTime.Now;
                        traslados.activo = true;
                        traslados.eliminado = false;
                        db.Entry(traslados).State = EntityState.Modified;
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
            return RedirectToAction("Index");

        }

        // GET: Inventario/Armas
        public ActionResult Rechazar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id);

            if (traslados == null)
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
                        traslados.id_traslado_estado = 7;
                        traslados.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        traslados.fecha_modificacion = DateTime.Now;
                        traslados.activo = true;
                        traslados.eliminado = false;
                        db.Entry(traslados).State = EntityState.Modified;
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

        //not working yet...
        public ActionResult Aprobar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslados traslados = db.Traslados.Find(id);

            if (traslados == null)
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
                        traslados.id_traslado_estado = 6;
                        traslados.id_usuario_autorizacion = usuarioTO.usuario.id_usuario;
                        traslados.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        traslados.fecha_modificacion = DateTime.Now;
                        traslados.activo = true;
                        traslados.eliminado = false;
                        db.Entry(traslados).State = EntityState.Modified;

                        Transacciones_Inventario ti = new Transacciones_Inventario();
                        ti.activo = true;
                        ti.eliminado = false;
                        ti.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        ti.fecha_creacion = DateTime.Now;
                        db.Transacciones_Inventario.Add(ti);
                        //
                        foreach (Traslado_Detalle tradet in traslados.Traslado_Detalle)
                        {
                            if (tradet.activo && !tradet.eliminado)
                            {
                                if (tradet.id_arma != null)
                                {
                                    Armas armas = db.Armas.Single(x => !x.eliminado && x.activo && x.id_arma == tradet.id_arma);
                                    //egreso
                                    Transaccion_Inventario_Detalle tid1 = new Transaccion_Inventario_Detalle();
                                    tid1.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid1.id_transaccion_inventario_tipo = 3;
                                    tid1.cantidad = 1;
                                    tid1.haber = armas.valor;
                                    tid1.id_arma = armas.id_arma;
                                    tid1.id_bodega = traslados.id_bodega_origen;
                                    tid1.activo = true;
                                    tid1.eliminado = false;
                                    tid1.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid1.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid1);
                                    //ingreso
                                    Transaccion_Inventario_Detalle tid2 = new Transaccion_Inventario_Detalle();
                                    tid2.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid2.id_transaccion_inventario_tipo = 3;
                                    tid2.cantidad = 1;
                                    tid2.debe = armas.valor;
                                    tid2.id_arma = armas.id_arma;
                                    tid2.id_bodega = traslados.id_bodega_destino;
                                    tid2.activo = true;
                                    tid2.eliminado = false;
                                    tid2.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid2.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid2);

                                    if (traslados.id_traslado_tipo == Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral))
                                    {
                                        armas.id_bodega = traslados.id_bodega_destino;
                                        armas.proceso_retorno = false;
                                        armas.comprometida = false;

                                        //Creando registro en logs para el arma
                                        Logs_Movimientos_Arma lma = new Logs_Movimientos_Arma();
                                        lma.id_arma = tradet.id_arma;
                                        lma.id_bodega_origen = traslados.id_bodega_origen;
                                        lma.id_bodega_destino = traslados.id_bodega_destino;
                                        lma.id_cliente = null;
                                        lma.id_empleado = null;
                                        lma.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Retorno);
                                        lma.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.ToBodegaCentral);
                                        lma.fecha_creacion = DateTime.Now;
                                        lma.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        lma.activo = true;
                                        lma.eliminado = false;
                                        db.Logs_Movimientos_Arma.Add(lma);
                                        db.SaveChanges();
                                    }
                                    else {
                                        armas.id_bodega = traslados.id_bodega_destino;
                                        armas.id_cliente = null;
                                        armas.id_empleado = null;

                                        //Creando registro en logs para el arma
                                        Logs_Movimientos_Arma lma = new Logs_Movimientos_Arma();
                                        lma.id_arma = tradet.id_arma;
                                        lma.id_cliente = traslados.id_cliente_origen;
                                        lma.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Retorno);
                                        lma.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.ToArmeria);
                                        lma.fecha_creacion = DateTime.Now;
                                        lma.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        lma.activo = true;
                                        lma.eliminado = false;
                                        db.Logs_Movimientos_Arma.Add(lma);
                                        db.SaveChanges();
                                    }
                                    armas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    armas.fecha_modificacion = DateTime.Now;
                                    db.Entry(armas).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                if (tradet.id_articulo != null)
                                {
                                    Articulos articulos = db.Articulos.Single(x => !x.eliminado && x.activo && x.id_articulo == tradet.id_articulo);
                                    //egreso
                                    Transaccion_Inventario_Detalle tid1 = new Transaccion_Inventario_Detalle();
                                    tid1.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid1.id_transaccion_inventario_tipo = 3;
                                    tid1.cantidad = 1;
                                    tid1.haber = articulos.costo;
                                    tid1.id_articulo = articulos.id_articulo;
                                    tid1.id_bodega = traslados.id_bodega_origen;
                                    tid1.activo = true;
                                    tid1.eliminado = false;
                                    tid1.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid1.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid1);
                                    //ingreso
                                    Transaccion_Inventario_Detalle tid2 = new Transaccion_Inventario_Detalle();
                                    tid2.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid2.id_transaccion_inventario_tipo = 3;
                                    tid2.cantidad = 1;
                                    tid2.haber = articulos.costo;
                                    tid2.id_articulo = articulos.id_articulo;
                                    tid2.id_bodega = traslados.id_bodega_destino;
                                    tid2.activo = true;
                                    tid2.eliminado = false;
                                    tid2.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid2.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid2);

                                    articulos.id_bodega = traslados.id_bodega_destino;
                                    articulos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    articulos.fecha_modificacion = DateTime.Now;
                                    db.Entry(articulos).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                if (tradet.id_consumible != null)
                                {
                                    Consumibles consumible = db.Consumibles.Single(x => !x.eliminado && x.activo && x.id_consumible == tradet.id_consumible);
                                    //egreso
                                    Bodega_Inventario_Consumibles bic1 = db.Bodega_Inventario_Consumibles.Single(x => !x.eliminado && x.activo && x.id_consumible == tradet.id_consumible && x.id_bodega == traslados.id_bodega_origen);
                                    
                                    bic1.existencia -= Convert.ToInt32(tradet.cantidad);
                                    bic1.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    bic1.fecha_modificacion = DateTime.Now;
                                    db.Entry(bic1).State = EntityState.Modified;
                                    
                                    //Ingreso
                                    var bic = db.Bodega_Inventario_Consumibles.Where(x => !x.eliminado && x.activo && x.id_consumible == tradet.id_consumible && x.id_bodega == traslados.id_bodega_destino);

                                    if (bic.Count() == 1)
                                    {
                                        Bodega_Inventario_Consumibles bic2 = db.Bodega_Inventario_Consumibles.Single(x => !x.eliminado && x.activo && x.id_consumible == tradet.id_consumible && x.id_bodega == traslados.id_bodega_destino);
                                        bic2.existencia += Convert.ToInt32(tradet.cantidad);
                                        bic2.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        bic2.fecha_modificacion = DateTime.Now;
                                        db.Entry(bic2).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        Bodega_Inventario_Consumibles bic2 = new Bodega_Inventario_Consumibles();
                                        bic2.id_consumible = tradet.Consumibles.id_consumible;
                                        bic2.id_bodega = Convert.ToInt32(traslados.id_bodega_destino);
                                        bic2.existencia = Convert.ToInt32(tradet.cantidad);
                                        bic2.activo = true;
                                        bic2.eliminado = false;
                                        bic2.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        bic2.fecha_creacion = DateTime.Now;
                                        db.Bodega_Inventario_Consumibles.Add(bic2);
                                    }


                                    Transaccion_Inventario_Detalle tid1 = new Transaccion_Inventario_Detalle();
                                    tid1.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid1.id_transaccion_inventario_tipo = 3;
                                    tid1.cantidad = tradet.cantidad;
                                    tid1.haber = consumible.costo;
                                    tid1.id_consumible = consumible.id_consumible;
                                    tid1.id_bodega = traslados.id_bodega_origen;
                                    tid1.activo = true;
                                    tid1.eliminado = false;
                                    tid1.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid1.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid1);
                                    Transaccion_Inventario_Detalle tid2 = new Transaccion_Inventario_Detalle();
                                    tid2.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid2.id_transaccion_inventario_tipo = 3;
                                    tid2.cantidad = tradet.cantidad;
                                    tid2.debe = consumible.costo;
                                    tid2.id_consumible = consumible.id_consumible;
                                    tid2.id_bodega = traslados.id_bodega_destino;
                                    tid2.activo = true;
                                    tid2.eliminado = false;
                                    tid2.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid2.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid2);
                                    db.SaveChanges();
                                }
                                if (tradet.id_municion != null)
                                {
                                    Municiones municion = db.Municiones.Single(x => !x.eliminado && x.activo && x.id_municion == tradet.id_municion);
                                    //egreso
                                    //Bodega_Inventario_Municiones bim1 = db.Bodega_Inventario_Municiones.Single(x => !x.eliminado && x.activo && x.id_municion == tradet.id_municion && x.id_bodega == traslados.id_bodega_origen);
                                    if (traslados.id_traslado_tipo == Convert.ToInt32(Catalogos.InventarioTrasladosTipo.Armeria_BodCentral))
                                    {
                                        Bodega_Inventario_Municiones bajas = db.Bodega_Inventario_Municiones.Where(x => !x.eliminado && x.activo && x.id_bodega_inventario_municiones == tradet.id_bodega_inventario_municion).SingleOrDefault();
                                        Bodega_Inventario_Municiones altas = db.Bodega_Inventario_Municiones.Where(x => !x.eliminado && x.activo && x.id_bodega == traslados.id_bodega_destino && x.id_municion == tradet.id_municion && !x.debitado).SingleOrDefault();

                                        bajas.existencia -= Convert.ToInt32(tradet.cantidad);
                                        bajas.retornando -= Convert.ToInt32(tradet.cantidad);
                                        bajas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        bajas.fecha_modificacion = DateTime.Now;
                                        db.Entry(bajas).State = EntityState.Modified;
                                        db.SaveChanges();

                                        altas.existencia += Convert.ToInt32(tradet.cantidad);
                                        altas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        altas.fecha_modificacion = DateTime.Now;
                                        db.Entry(altas).State = EntityState.Modified;
                                        db.SaveChanges();

                                        //Creando registro en logs para las municiones
                                        Logs_Movimientos_Municion lmm = new Logs_Movimientos_Municion();
                                        lmm.id_bodega_inventario_municion = bajas.id_bodega_inventario_municiones;
                                        lmm.id_bodega_origen = traslados.id_bodega_origen;
                                        lmm.id_bodega_destino = traslados.id_bodega_destino;
                                        lmm.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Retorno);
                                        lmm.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.ToBodegaCentral);
                                        lmm.cantidad = tradet.cantidad;
                                        lmm.fecha_creacion = DateTime.Now;
                                        lmm.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        lmm.activo = true;
                                        lmm.eliminado = false;
                                        db.Logs_Movimientos_Municion.Add(lmm);
                                        db.SaveChanges();

                                    }
                                    else {
                                        Bodega_Inventario_Municiones bajas = db.Bodega_Inventario_Municiones.Where(x => !x.eliminado && x.activo && x.id_bodega_inventario_municiones == tradet.id_bodega_inventario_municion).SingleOrDefault();
                                        Bodega_Inventario_Municiones altas = db.Bodega_Inventario_Municiones.Where(x => !x.eliminado && x.activo && x.id_bodega == traslados.id_bodega_destino && x.id_municion == tradet.id_municion && !x.debitado && x.autorizada).SingleOrDefault();

                                        bajas.retornando -= Convert.ToInt32(tradet.cantidad);
                                        bajas.cantidad_debito -= Convert.ToInt32(tradet.cantidad);
                                        //bajas.retornado = true;
                                        bajas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        bajas.fecha_modificacion = DateTime.Now;
                                        db.Entry(bajas).State = EntityState.Modified;
                                        db.SaveChanges();

                                        //altas.existencia += Convert.ToInt32(tradet.cantidad);
                                        altas.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        altas.fecha_modificacion = DateTime.Now;
                                        db.Entry(altas).State = EntityState.Modified;
                                        db.SaveChanges();

                                        //Creando registro en logs para las municiones
                                        Logs_Movimientos_Municion lmm = new Logs_Movimientos_Municion();
                                        lmm.id_bodega_inventario_municion = bajas.id_bodega_inventario_municiones;
                                        lmm.id_bodega_destino = traslados.id_bodega_destino;
                                        lmm.id_cliente = traslados.id_cliente_origen;
                                        lmm.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Retorno);
                                        lmm.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.ToArmeria);
                                        lmm.cantidad = tradet.cantidad;
                                        lmm.fecha_creacion = DateTime.Now;
                                        lmm.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        lmm.activo = true;
                                        lmm.eliminado = false;
                                        db.Logs_Movimientos_Municion.Add(lmm);
                                        db.SaveChanges();

                                    }
                                    

                                    ////ingreso
                                    //var bim = db.Bodega_Inventario_Municiones.Where(x => !x.eliminado && x.activo && x.id_municion == tradet.id_municion && x.id_bodega == traslados.id_bodega_destino);

                                    //if (bim.Count() == 1)
                                    //{
                                    //    Bodega_Inventario_Municiones bim2 = db.Bodega_Inventario_Municiones.Single(x => !x.eliminado && x.activo && x.id_municion == tradet.id_municion && x.id_bodega == traslados.id_bodega_destino);
                                    //    bim2.existencia += Convert.ToInt32(tradet.cantidad);
                                    //    bim2.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    //    bim2.fecha_modificacion = DateTime.Now;
                                    //    //db.Entry(bim2).State = EntityState.Modified;
                                    //}
                                    //else
                                    //{
                                    //    Bodega_Inventario_Municiones bim2 = new Bodega_Inventario_Municiones();
                                    //    bim2.id_municion = tradet.Municiones.id_municion;
                                    //    bim2.id_bodega = Convert.ToInt32(traslados.id_bodega_destino);
                                    //    bim2.existencia = Convert.ToInt32(tradet.cantidad);
                                    //    bim2.activo = true;
                                    //    bim2.eliminado = false;
                                    //    bim2.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    //    bim2.fecha_creacion = DateTime.Now;
                                    //    //db.Bodega_Inventario_Municiones.Add(bim2);
                                    //}
                                    
                                    Transaccion_Inventario_Detalle tid1 = new Transaccion_Inventario_Detalle();
                                    tid1.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid1.id_transaccion_inventario_tipo = 3;
                                    tid1.cantidad = tradet.cantidad;
                                    tid1.haber = municion.costo;
                                    tid1.id_municion = municion.id_municion;
                                    tid1.id_bodega = traslados.id_bodega_origen;
                                    tid1.activo = true;
                                    tid1.eliminado = false;
                                    tid1.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid1.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid1);
                                    db.SaveChanges();

                                    Transaccion_Inventario_Detalle tid2 = new Transaccion_Inventario_Detalle();
                                    tid2.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid2.id_transaccion_inventario_tipo = 3;
                                    tid2.cantidad = tradet.cantidad;
                                    tid2.debe = municion.costo;
                                    tid2.id_municion = municion.id_municion;
                                    tid2.id_bodega = traslados.id_bodega_origen;
                                    tid2.activo = true;
                                    tid2.eliminado = false;
                                    tid2.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid2.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid2);
                                    db.SaveChanges();
                                }
                                if (tradet.id_uniforme != null)
                                {
                                    Uniformes uniforme = db.Uniformes.Single(x => !x.eliminado && x.activo && x.id_uniforme == tradet.id_uniforme);
                                    //egreso
                                    Bodega_Inventario_Uniformes biu1 = db.Bodega_Inventario_Uniformes.Single(x => !x.eliminado && x.activo && x.id_uniforme == tradet.id_uniforme && x.id_bodega == traslados.id_bodega_origen);


                                    biu1.existencia -= Convert.ToInt32(tradet.cantidad);
                                    biu1.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    biu1.fecha_modificacion = DateTime.Now;
                                    db.Entry(biu1).State = EntityState.Modified;

                                    //ingreso
                                    var biu = db.Bodega_Inventario_Uniformes.Where(x => !x.eliminado && x.activo && x.id_uniforme == tradet.id_uniforme && x.id_bodega == traslados.id_bodega_destino);

                                    if (biu.Count() == 1)
                                    {
                                        Bodega_Inventario_Uniformes biu2 = db.Bodega_Inventario_Uniformes.Single(x => !x.eliminado && x.activo && x.id_uniforme == tradet.id_uniforme && x.id_bodega == traslados.id_bodega_destino);
                                        biu2.existencia += Convert.ToInt32(tradet.cantidad);
                                        biu2.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                        biu2.fecha_modificacion = DateTime.Now;
                                        db.Entry(biu2).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        Bodega_Inventario_Uniformes biu2 = new Bodega_Inventario_Uniformes();
                                        biu2.id_uniforme = tradet.Uniformes.id_uniforme;
                                        biu2.id_bodega = Convert.ToInt32(traslados.id_bodega_destino);
                                        biu2.existencia = Convert.ToInt32(tradet.cantidad);
                                        biu2.activo = true;
                                        biu2.eliminado = false;
                                        biu2.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        biu2.fecha_creacion = DateTime.Now;
                                        db.Bodega_Inventario_Uniformes.Add(biu2);
                                    }

                                    Transaccion_Inventario_Detalle tid1 = new Transaccion_Inventario_Detalle();
                                    tid1.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid1.id_transaccion_inventario_tipo = 3;
                                    tid1.cantidad = tradet.cantidad;
                                    tid1.haber = uniforme.costo;
                                    tid1.id_uniforme = uniforme.id_uniforme;
                                    tid1.id_bodega = traslados.id_bodega_origen;
                                    tid1.activo = true;
                                    tid1.eliminado = false;
                                    tid1.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid1.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid1);

                                    Transaccion_Inventario_Detalle tid2 = new Transaccion_Inventario_Detalle();
                                    tid2.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid2.id_transaccion_inventario_tipo = 3;
                                    tid2.cantidad = tradet.cantidad;
                                    tid2.debe = uniforme.costo;
                                    tid2.id_uniforme = uniforme.id_uniforme;
                                    tid2.id_bodega = traslados.id_bodega_origen;
                                    tid2.activo = true;
                                    tid2.eliminado = false;
                                    tid2.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid2.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid2);
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
    }
}
