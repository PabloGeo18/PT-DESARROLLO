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
using MVC2013.Src.Comun.View;
using System.Globalization;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class ClientesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Clientes
        #region Index
        public ActionResult Index()
        {
            var clientes = db.Clientes.Where(x => !x.eliminado && x.activo).Include(c => c.Tipo_Cliente).Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2);
            return View(clientes.ToList());
        }

        public JsonResult Mostrar_RS(int id)
        {
            List<object> resultados = new List<object>();
            var razon = db.Razones_Sociales.Where(e => !e.eliminado && e.activo && e.id_cliente == id);
            foreach (var c in razon)
            {
                resultados.Add(new { c.id_razon_social, c.razon_social});
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Mostrar_U(int id_RS)
        {
            List<object> resultados = new List<object>();
            var ubi = db.Ubicaciones.Where(e => !e.eliminado && e.activo && e.id_razon_social == id_RS);
            foreach (var c in ubi)
            {
                resultados.Add(new { c.id_ubicacion, c.direccion });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Obtener_Correlativo(int id_serie_factura)
        {
            int correlativo = db.Series_Facturas.Where(x => x.id_serie_factura == id_serie_factura).Take(1).FirstOrDefault().correlativo + 1;

            return Json(correlativo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Obtener_Factura(DateTime fecha, int Razon_Social) { 

            List<object> resultados = new List<object>();
            decimal total = 0;
            var Contratos = db.Contratos_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_cat_estado_servicio_contratado == (int)Catalogos.Estado_Contrato_Servicio.Activo &&
                                                                                                   x.Cat_Tipos_Facturacion.ciclica == true && 
                                                                                                   x.Ubicaciones.Razones_Sociales.id_razon_social == Razon_Social).ToList();
            foreach (var item in Contratos)
	        {
		        var lista_agentes = db.Contratos_Agentes.Where(x => x.activo && !x.eliminado).Where(x => x.id_contrato_servicio == item.id_contrato_servicio )
                                                                                             .Where(x => x.fecha_inicio <= fecha).ToList();
                foreach (var item_agente in lista_agentes)
                {
                    if (item_agente.fecha_fin.HasValue)
                    {
                        if (item_agente.fecha_fin.Value.Year >= fecha.Year)
                        {
                            if (item_agente.fecha_fin.Value.Month <= fecha.Month)
                            {
                                total += item_agente.cantidad * item_agente.precio_venta_unitario;
                                resultados.Add(new { item_agente.Cat_Tipos_Agente.nombre, item_agente.cantidad, item_agente.precio_venta_unitario, item_agente.fecha_fin });
                            }
                        }
                    }
                    else
                    {
                        total += item_agente.cantidad * item_agente.precio_venta_unitario;
                        resultados.Add(new { item_agente.Cat_Tipos_Agente.nombre, item_agente.cantidad, item_agente.precio_venta_unitario, item_agente.fecha_fin });
                    }
                }

                var lista_servicios = db.Contratos_Otros_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_contrato_servicio == item.id_contrato_servicio)
                                                          .Where(x => x.fecha_inicio <= fecha).ToList();
                foreach (var item_servicio in lista_servicios)
                {
                    if (item_servicio.fecha_fin.HasValue)
                    {
                        if (item_servicio.fecha_fin.Value.Year >= fecha.Year)
                        {
                            if (item_servicio.fecha_fin.Value.Month <= fecha.Month)
                            {
                                total += item_servicio.cantidad * item_servicio.precio_venta_unitario;
                                resultados.Add(new { item_servicio.Cat_Otros_Servicios.nombre, item_servicio.cantidad, item_servicio.precio_venta_unitario, item_servicio.fecha_fin });
                            }
                        }
                    }
                    else
                    {
                        total += item_servicio.cantidad * item_servicio.precio_venta_unitario;
                        resultados.Add(new { item_servicio.Cat_Otros_Servicios.nombre, item_servicio.cantidad, item_servicio.precio_venta_unitario, item_servicio.fecha_fin });
                    }
                }
	        }

            return Json(new { resultados, total}, JsonRequestBehavior.AllowGet);
        }
         
        #endregion

        // GET: Administracion/Clientes/Details/5
        #region Details

        public ActionResult Details(long id)
        {
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        public ActionResult Details_RS(long id_Razon_Social)
        {
            Razones_Sociales razon_Social = db.Razones_Sociales.Where(x => x.activo && !x.eliminado).Where(x => x.id_razon_social == id_Razon_Social).FirstOrDefault();
            Clientes clientes = razon_Social.Clientes;
            if (clientes == null || razon_Social == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cliente = clientes.id_cliente;
            ViewBag.id_RS = razon_Social.id_razon_social;
            ViewBag.id_razon_social = id_Razon_Social;
            ViewBag.Nombre_Comercial = razon_Social.nombre_comercial;
            ViewBag.Nit = razon_Social.nit;
            ViewBag.Direccion_Fisica = razon_Social.direccion_fisica;
            ViewBag.RS_Contacto = db.Razon_Social_Contacto.Where(x => x.id_razon_social == id_Razon_Social).Include(x => x.Contactos).Where(x => x.activo && !x.eliminado).ToList();
            ViewBag.Municipio_Fisica = db.Municipios.Where(x => x.id_municipio == razon_Social.municipio_direccion_fisica).FirstOrDefault().nombre;
            ViewBag.Direccion_Fiscal = razon_Social.direccion_fiscal;
            ViewBag.Municipio_Fiscal = db.Municipios.Where(x => x.id_municipio == razon_Social.municipio_direccion_fiscal).FirstOrDefault().nombre;
            ViewBag.Direccion_Facturacion = razon_Social.direccion_facturacion;
            ViewBag.Municipio_Facturacion = db.Municipios.Where(x => x.id_municipio == razon_Social.municipio_direccion_facturacion).FirstOrDefault().nombre;
            ViewBag.Ubicaciones = db.Ubicaciones.Where(x => x.id_razon_social == id_Razon_Social).ToList();
            return View(clientes);
        }

        public ActionResult Details_U(long id_ubicacion)
        {

            Ubicaciones ubicacion = db.Ubicaciones.Where(x => x.id_ubicacion == id_ubicacion).FirstOrDefault();
            if (ubicacion == null)
            {
                return HttpNotFound();
            }
            Razones_Sociales razon_Social = ubicacion.Razones_Sociales;
            Clientes clientes = razon_Social.Clientes;

            /*RAZONES SOCIALES*/
            ViewBag.Nombre_Comercial = razon_Social.nombre_comercial;
            ViewBag.Nit = razon_Social.nit;
            ViewBag.Direccion_Fisica = razon_Social.direccion_fisica;
            ViewBag.Municipio_Fisica = db.Municipios.Where(x => x.id_municipio == razon_Social.municipio_direccion_fisica).FirstOrDefault().nombre;
            ViewBag.Direccion_Fiscal = razon_Social.direccion_fiscal;
            ViewBag.Municipio_Fiscal = db.Municipios.Where(x => x.id_municipio == razon_Social.municipio_direccion_fiscal).FirstOrDefault().nombre;
            ViewBag.Direccion_Facturacion = razon_Social.direccion_facturacion;
            ViewBag.Municipio_Facturacion = db.Municipios.Where(x => x.id_municipio == razon_Social.municipio_direccion_facturacion).FirstOrDefault().nombre;
            ViewBag.id_RS = razon_Social.id_razon_social;
            
            /*UBICACIONES*/
            ViewBag.id = ubicacion.id_ubicacion;
            ViewBag.direccion = ubicacion.direccion;
            ViewBag.cantidad_solicitada = ubicacion.cantidad_solicitada;
            ViewBag.cantidad_contratada = ubicacion.cantidad_contratados;
            ViewBag.encargado = ubicacion.id_contacto_encargado;
            ViewBag.responsable = ubicacion.id_contacto_responsable;
            ViewBag.ubicacion = ubicacion.id_ubicacion;

            /*Contratos_Servicios*/
            ViewBag.servicios = db.Contratos_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_ubicacion == ubicacion.id_ubicacion)
                                                        .OrderBy(x => x.id_cat_estado_servicio_contratado).ThenBy(x => x.fecha_creacion).ToList();

            /*Compromisos*/
            var compromisos_ubicacion = db.Compromisos.Where(x => x.Contratos_Agentes.Contratos_Servicios.id_ubicacion == id_ubicacion).ToList();
            ViewBag.compromisos = compromisos_ubicacion;

            return View(clientes);
        }

        public ActionResult Details_Factura(long id_factura)
        {
            var factura = db.Facturas.Where(x => x.id_factura == id_factura).FirstOrDefault();
            //var factura_agentes = db.Facturas_Agente.Where(x => x.id_factura == factura.id_factura).ToList();
            //var factura_servicios = db.Facturas_Servicio.Where(x => x.id_factura == factura.id_factura).ToList();
            //List<Contratos_Agentes> agentes = new List<Contratos_Agentes>();
            //List<Contratos_Otros_Servicios> servicios = new List<Contratos_Otros_Servicios>();
            //foreach (var item in factura_agentes)
            //{
            //    var temp_agente = db.Contratos_Agentes.Where(x => x.id_contrato_agente == item.id_contrato_agente).FirstOrDefault();
            //    agentes.Add(temp_agente);
            //}
            //foreach (var item in factura_servicios)
            //{
            //    var temp_servicio = db.Contratos_Otros_Servicios.Where(x => x.id_contrato_servicio == item.id_contrato_servicio).FirstOrDefault();
            //    servicios.Add(temp_servicio);
            //}
            //ViewBag.Contratos_Agentes = agentes;
            //ViewBag.Contratos_Servicios = servicios;

            return View(factura);
        }
        /* Details_Factura_Eventual
        public ActionResult Details_Factura_Eventual(long id_factura_eventual)
        {
            var factura = db.Facturas_Eventuales.Where(x => x.id_factura_eventual == id_factura_eventual).FirstOrDefault();
            ViewBag.Contratos_Agentes = db.Facturas_Eventuales_Agente.Where(x => x.activo && !x.eliminado).Where(x => x.id_factura_eventual == id_factura_eventual).ToList();
            ViewBag.Contratos_Servicios = db.Facturas_Eventuales_Servicio.Where(x => x.activo && !x.eliminado).Where(x => x.id_factura_eventual == id_factura_eventual).ToList();
            return View(factura);
        }
        */


        #endregion

        // GET: Administracion/Clientes/Edit/5
        #region Edit
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_tipo_cliente = new SelectList(db.Tipo_Cliente.Where(t => !t.eliminado), "id_tipo_cliente", "nombre", clientes.id_tipo_cliente);
            return View(clientes);
        }

        // POST: Administracion/Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                Clientes ClientesEdit = db.Clientes.Find(clientes.id_cliente);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                ClientesEdit.nombre = clientes.nombre;
                ClientesEdit.id_tipo_cliente = clientes.id_tipo_cliente;
                ClientesEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                ClientesEdit.fecha_modificacion = DateTime.Now;
                ClientesEdit.eliminado = false;
                ClientesEdit.activo = true;
                db.Entry(ClientesEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new {id = ClientesEdit.id_cliente});
            }
            ViewBag.id_cliente_tipo = new SelectList(db.Tipo_Cliente.Where(t => !t.eliminado), "id_cliente_tipo", "descripcion", clientes.id_tipo_cliente);
            return View(clientes);
        }

        // GET: Administracion/Clientes/Edit/5
        public ActionResult Edit_RS(long id_Razon_Social)
        {
            Razones_Sociales razon_social = db.Razones_Sociales.SingleOrDefault(rz => rz.activo && !rz.eliminado && rz.id_razon_social == id_Razon_Social);
            if (razon_social == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ubicaciones = db.Ubicaciones.Where(x => x.id_razon_social == id_Razon_Social).ToList();
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razon_social.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razon_social.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razon_social.municipio_direccion_fisica);
            return View(razon_social);
        }

        // POST: Administracion/Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_RS(Razones_Sociales razon)
        {
            if (ModelState.IsValid)
            {
                Razones_Sociales razon_social_edit = db.Razones_Sociales.Find(razon.id_razon_social);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                razon_social_edit.razon_social = razon.razon_social;
                razon_social_edit.nombre_comercial = razon_social_edit.nombre_comercial;
                razon_social_edit.direccion_facturacion = razon.direccion_facturacion;
                razon_social_edit.municipio_direccion_facturacion = razon.municipio_direccion_facturacion;
                razon_social_edit.direccion_fiscal = razon.direccion_fiscal;
                razon_social_edit.municipio_direccion_fiscal = razon.municipio_direccion_fiscal;
                razon_social_edit.direccion_fisica = razon.direccion_fisica;
                razon_social_edit.municipio_direccion_fisica = razon.municipio_direccion_fisica;

                //db.Entry(razon_social_edit).State = EntityState.Modified;
                //db.SaveChanges();

                return RedirectToAction("Details", new { id = razon_social_edit.id_cliente });
            }
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razon.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razon.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razon.municipio_direccion_fisica);
            return View(razon);
        }

        // GET: Administracion/Clientes/Edit/5
        public ActionResult Edit_U(long id_ubicacion)
        {
            Ubicaciones ubicacion = db.Ubicaciones.SingleOrDefault(u => u.activo && !u.eliminado && u.id_ubicacion == id_ubicacion);
            if (ubicacion == null)
            {
                return HttpNotFound();
            }
            return View(ubicacion);
        }

        // POST: Administracion/Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_U(Ubicaciones ubicaciones)
        {
            if (ModelState.IsValid)
            {
                Ubicaciones ubi_edit = db.Ubicaciones.Where(u => u.id_ubicacion == ubicaciones.id_ubicacion).FirstOrDefault();
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                ubi_edit.direccion = ubicaciones.direccion;
                ubi_edit.cantidad_contratados = ubicaciones.cantidad_contratados;
                ubi_edit.cantidad_solicitada = ubicaciones.cantidad_solicitada;
                ubi_edit.id_contacto_encargado = ubicaciones.id_contacto_encargado;
                ubi_edit.id_contacto_responsable = ubicaciones.id_contacto_responsable;
                ubi_edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                ubi_edit.fecha_modificacion = DateTime.Now;
                return RedirectToAction("Index");

            }
            return View(ubicaciones);
        }

        public ActionResult Edit_Agente_Contratado(int id)
        {
            Contratos_Agentes agente = db.Contratos_Agentes.Find(id);
            if (agente == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado), "id_cat_tipo_agente", "nombre", agente.id_cat_tipo_agente);
            ViewBag.id = id;
            return View(agente);
        }

        // POST: Administracion/Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Agente_Contratado(Contratos_Agentes Contratos_Agentes)
        {
            if (ModelState.IsValid)
            {
                Contratos_Agentes Agentes_Edit = db.Contratos_Agentes.Find(Contratos_Agentes.id_contrato_agente);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Agentes_Edit.id_cat_tipo_agente = Contratos_Agentes.id_cat_tipo_agente;
                Agentes_Edit.cantidad = Contratos_Agentes.cantidad;
                Agentes_Edit.costo = Contratos_Agentes.costo;
                Agentes_Edit.precio_venta_unitario = Contratos_Agentes.precio_venta_unitario;
                Agentes_Edit.fecha_inicio = Contratos_Agentes.fecha_inicio;
                Agentes_Edit.fecha_fin = Contratos_Agentes.fecha_fin;

                Agentes_Edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                Agentes_Edit.fecha_modificacion = DateTime.Now;
                db.Entry(Agentes_Edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details_U", new { id_ubicacion = Agentes_Edit.Contratos_Servicios.id_ubicacion });
            }
            ViewBag.id_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado), "id_cat_tipo_agente", "nombre", Contratos_Agentes.id_cat_tipo_agente);
            ViewBag.id = Contratos_Agentes.id_contrato_agente;
            return View(Contratos_Agentes);
        }
        // GET
        public ActionResult Edit_Servicio_Contratado(int id)
        {
            Contratos_Otros_Servicios servicio = db.Contratos_Otros_Servicios.Find(id);
            if (servicio == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado), "id_cat_otro_servicio", "nombre", servicio.id_cat_otro_servicio);
            ViewBag.id = id;
            return View(servicio);
        }

        // POST: Administracion/Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Servicio_Contratado(Contratos_Otros_Servicios servicios)
        {
            if (ModelState.IsValid)
            {
                Contratos_Otros_Servicios servicio_edit = db.Contratos_Otros_Servicios.Find(servicios.id_contrato_servicio);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                servicio_edit.id_cat_otro_servicio = servicios.id_cat_otro_servicio;
                servicio_edit.cantidad = servicios.cantidad;
                servicio_edit.costo = servicios.costo;
                servicio_edit.precio_venta_unitario = servicios.precio_venta_unitario;
                servicio_edit.fecha_inicio = servicios.fecha_inicio;
                servicio_edit.fecha_fin = servicios.fecha_fin;

                servicio_edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                servicio_edit.fecha_modificacion = DateTime.Now;
                db.Entry(servicio_edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details_U", new { id_ubicacion = servicio_edit.Contratos_Servicios.id_ubicacion });
            }
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado), "id_cat_otro_servicio", "nombre", servicios.id_cat_otro_servicio);
            ViewBag.id = servicios.id_contrato_servicio;
            return View(servicios);
        }
        #endregion 

        // GET: Administracion/Clientes/Create
        #region Create
        public ActionResult Create_Cliente()
        {
            ViewBag.id_tipo_cliente = new SelectList(db.Tipo_Cliente.Where(x => x.eliminado == false), "id_tipo_cliente", "nombre");
            return View();
        }

        // POST: Administracion/Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Cliente(Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                clientes.activo = true;
                clientes.eliminado = false;
                clientes.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                clientes.fecha_creacion = DateTime.Now;
                db.Clientes.Add(clientes);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = clientes.id_cliente });

            }
            ViewBag.id_cliente_tipo = new SelectList(db.Tipo_Cliente.Where(t => !t.eliminado), "id_cliente_tipo", "descripcion", clientes.id_tipo_cliente);
            return View(clientes);
        }


        public ActionResult Create_Compromiso(int id)
        {
            List<Contratos_Agentes> agentes = new List<Contratos_Agentes>();
            var Contratos_Agentes = db.Contratos_Agentes
                .Where(x => x.activo && !x.eliminado).Where(x => x.Contratos_Servicios.id_ubicacion == id && x.fecha_fin > DateTime.Now)
                .ToList();

            var compromisos = db.Compromisos
                .Where(x => x.activo && !x.eliminado).Where(x => x.Contratos_Agentes.Contratos_Servicios.id_ubicacion == id).ToList();
            List<int> temp_agentes = new List<int>();
            for (int i = 0; i < compromisos.Count; i++)
			{
                if (!temp_agentes.Contains(compromisos[i].id_contrato_agente))
                {
                  temp_agentes.Add(compromisos[i].id_contrato_agente);
                }
			}
            foreach (var agente_c in Contratos_Agentes)
            {
                if (!temp_agentes.Contains(agente_c.id_contrato_agente))
                {
                    if (!agentes.Contains(agente_c))
                    {
                        agentes.Add(agente_c);
                    }
                }
            }
            var temp_ubicacion = db.Ubicaciones.Where(x => x.id_ubicacion == id).FirstOrDefault();
            ViewBag.ubicacion = temp_ubicacion.direccion;
            ViewBag.razon_social = temp_ubicacion.Razones_Sociales.razon_social;
            ViewBag.cliente = temp_ubicacion.Razones_Sociales.Clientes.nombre;
            ViewBag.id_agente = new SelectList(agentes.Select(s => new { id = s.id_contrato_agente, nombre = s.Cat_Tipos_Agente.nombre + "/ cantidad contratada:" + s.cantidad }), "id", "nombre");
            ViewBag.id_ubicacion = id;
            ViewBag.compromiso = compromisos;
            return View();
        }

        // POST: Administracion/Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Compromiso(Compromisos compromiso, int id_u)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                String Mensaje = "";
                try
                {
                    int temp_agente = Convert.ToInt32(Request["id_agente"]);
                    if (!String.IsNullOrEmpty(Request["id_agente"]))
                    {
                        temp_agente = Convert.ToInt32(Request["id_agente"]);
                    }
                    var Agente_contratado = db.Contratos_Agentes.Where(x => x.id_contrato_agente == temp_agente).FirstOrDefault();
                    int Cantidad_solicitada = Agente_contratado.cantidad;
                    String nombre_agente = Agente_contratado.Cat_Tipos_Agente.nombre;
                    if (nombre_agente.Contains("24"))
                    {
                        Cantidad_solicitada = Cantidad_solicitada / 2;
                    }
                    //int id_contrato_agente = db.Contratos_Agentes.Where(x => x.id_cat_tipo_agente == temp_agente && x.Contratos_Servicios.id_ubicacion == id_u).FirstOrDefault().id_contrato_agente;
                    for (int i = 1; i <= 7; i++)
                    {
                        Compromisos compromiso_nuevo = Compromiso_Nuevo(temp_agente);
                        int cantidad = Convert.ToInt32(Request["cantidad" + i.ToString()]);
                        if (!String.IsNullOrEmpty(Request["cantidad" + i.ToString()]))
                        {
                            cantidad = Convert.ToInt32(Request["cantidad" + i.ToString()]);
                        }
                        if (nombre_agente.Contains("24"))
                        {
                            if (Cantidad_solicitada < cantidad)
                            {
                                Mensaje = "La cantidad a solocitar por dia debe ser menor o igual a la mitad de lo solicitado.";      
                                throw new Exception();
                            }
                        }
                        else if (cantidad > Cantidad_solicitada)
                        {
                            Mensaje = "La cantidad solicitada en uno de los dias excede a la cantidad de agentes disponibles para dicha ubicacion.";
                            throw new Exception();
                        }
                        if (cantidad < 0)
                        {
                            Mensaje = "La cantidad solicitada no puede ser menor a 0.";
                            throw new Exception();
                        }
                        compromiso_nuevo.cantidad = cantidad;
                        if (i < 7)
                        {
                            compromiso_nuevo.id_dia_semana = i + 2;
                        }
                        else
                        {
                            compromiso_nuevo.id_dia_semana = 1;
                        }
                        db.Compromisos.Add(compromiso_nuevo);
                    }
                    
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Create_Compromiso", new { id = id_u });
                }
                catch (Exception)
                {
                    tran.Rollback();
                    List<Contratos_Agentes> agentes = new List<Contratos_Agentes>();
                    var Contratos_Agentes = db.Contratos_Agentes
                        .Where(x => x.activo && !x.eliminado).Where(x => x.Contratos_Servicios.id_ubicacion == id_u && x.fecha_fin > DateTime.Now)
                        .ToList();

                    var compromisos = db.Compromisos
                        .Where(x => x.activo && !x.eliminado).Where(x => x.Contratos_Agentes.Contratos_Servicios.id_ubicacion == id_u).ToList();
                    List<int> temp_agentes = new List<int>();
                    for (int i = 0; i < compromisos.Count; i++)
                    {
                        if (!temp_agentes.Contains(compromisos[i].id_contrato_agente))
                        {
                            temp_agentes.Add(compromisos[i].id_contrato_agente);
                        }
                    }
                    foreach (var agente_c in Contratos_Agentes)
                    {
                        if (!temp_agentes.Contains(agente_c.id_contrato_agente))
                        {
                            if (!agentes.Contains(agente_c))
                            {
                                agentes.Add(agente_c);
                            }
                        }
                    }
                    ViewBag.compromiso = compromisos;
                    var temp_ubicacion = db.Ubicaciones.Where(x => x.id_ubicacion == id_u).FirstOrDefault();
                    ViewBag.ubicacion = temp_ubicacion.direccion;
                    ViewBag.razon_social = temp_ubicacion.Razones_Sociales.razon_social;
                    ViewBag.cliente = temp_ubicacion.Razones_Sociales.Clientes.nombre;
                    ContextMessage msg = new ContextMessage(ContextMessage.Warning, Mensaje);
                    msg.ReturnUrl = Url.Action("Create_Compromiso", id_u);
                    TempData[User.Identity.Name] = msg;
                    ViewBag.id_agente = new SelectList(agentes.Select(s => new { id = s.id_contrato_agente, nombre = s.Cat_Tipos_Agente.nombre + "/" + s.cantidad }), "id", "nombre");
                    ViewBag.id_ubicacion = id_u;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        private Compromisos Compromiso_Nuevo(int id_contrato_agente) {
            Compromisos compromiso_nuevo = new Compromisos();
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            compromiso_nuevo.activo = true;
            compromiso_nuevo.eliminado = false;
            compromiso_nuevo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            compromiso_nuevo.fecha_creacion = DateTime.Now;
            compromiso_nuevo.id_contrato_agente = id_contrato_agente;

            return compromiso_nuevo;
        }

        // GET: Administracion/Razones_Sociales/Create
        public ActionResult Create_Razon_Social(int id)
        {
            ViewBag.Cliente = db.Clientes.Where(x => x.id_cliente == id).FirstOrDefault();
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre");
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre");
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre");
            return View();
        }

        // POST: Administracion/Razones_Sociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Razon_Social(Razones_Sociales razones_Sociales, int id_cliente)
        {

            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                razones_Sociales.activo = true;
                razones_Sociales.eliminado = false;
                razones_Sociales.id_cliente = id_cliente;
                razones_Sociales.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                razones_Sociales.fecha_creacion = DateTime.Now;
                db.Razones_Sociales.Add(razones_Sociales);
                db.SaveChanges();
                return RedirectToAction("Details_RS", new { id_cliente = razones_Sociales.id_cliente, id_Razon_Social = razones_Sociales.id_razon_social });
            }

            ViewBag.id_cliente = new SelectList(db.Clientes.Where(t => !t.eliminado), "id_cliente", "nombre", razones_Sociales.id_cliente);
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios.Where(m => !m.eliminado), "id_municipio", "nombre", razones_Sociales.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios.Where(m => !m.eliminado), "id_municipio", "nombre", razones_Sociales.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios.Where(m => !m.eliminado), "id_municipio", "nombre", razones_Sociales.municipio_direccion_fisica);
            return View(razones_Sociales);
        }

        // GET: Administracion/Razones_Sociales/Create
        public ActionResult Create_Ubicacion(int id_cliente, int id_RS)
        {
            ViewBag.Cliente = db.Clientes.Where(x => x.id_cliente == id_cliente).FirstOrDefault();
            ViewBag.RS = db.Razones_Sociales.Where(x => x.id_razon_social == id_RS).FirstOrDefault();
            return View();
        }

        // POST: Administracion/Razones_Sociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Ubicacion(Ubicaciones ubicacion, int id_razon_social, int id_cliente)
        {
            if (ModelState.IsValid)
            {
                if (ubicacion.cantidad_solicitada > 0)
                {
                    if (ubicacion.cantidad_contratados > 0)
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        ubicacion.activo = true;
                        ubicacion.eliminado = false;
                        ubicacion.id_razon_social = id_razon_social;
                        ubicacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        ubicacion.fecha_creacion = DateTime.Now;
                        db.Ubicaciones.Add(ubicacion);
                        db.SaveChanges();
                        return RedirectToAction("Details_RS", new { id_cliente = id_cliente, id_Razon_Social = ubicacion.id_razon_social });
                    }
                    else
                        ModelState.AddModelError("cantidad_contratada", "Se debe ingresar una cantidad mayor a 0");
                }
                else
                    ModelState.AddModelError("cantidad_solicitada", "Se debe ingresar una cantidad mayor a 0");


            }

            ViewBag.Cliente = db.Clientes.Where(x => x.id_cliente == id_cliente).FirstOrDefault();
            ViewBag.RS = db.Razones_Sociales.Where(x => x.id_razon_social == id_razon_social).FirstOrDefault();
            return View(ubicacion);
        }

        // GET: Administracion/Razones_Sociales/Create
        public ActionResult Create_AS_U(int id)
        {
            ViewBag.id_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado), "id_cat_tipo_agente", "nombre");
            ViewBag.id_servicio = new SelectList(db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado), "id_cat_otro_servicio", "nombre");
            ViewBag.id_ubicacion = id;

            var temp_ubicacion = db.Ubicaciones.Where(x => x.id_ubicacion == id).FirstOrDefault();
            ViewBag.ubicacion = temp_ubicacion.direccion;
            ViewBag.razon_social = temp_ubicacion.Razones_Sociales.razon_social;
            ViewBag.cliente = temp_ubicacion.Razones_Sociales.Clientes.nombre;

            ViewBag.servicios = db.Contratos_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_ubicacion == id && x.Cat_Tipos_Facturacion.ciclica == true
                                                                && (x.id_cat_estado_servicio_contratado != (int)Catalogos.Estado_Contrato_Servicio.Finalizado 
                                                                || x.id_cat_estado_servicio_contratado != (int)Catalogos.Estado_Contrato_Servicio.Cancelado)).ToList();

            return View();
        }

        // POST: Administracion/Razones_Sociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create_AS_U(Contratos_Servicios Nuevo_servicio, int Ubicacion)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

            var id_factura_ciclica = db.Cat_Tipos_Facturacion.Where(x => x.activo && !x.eliminado).Where(x => x.ciclica == true).FirstOrDefault().id_cat_tipo_facturacion;
            Nuevo_servicio.id_cat_tipo_facturacion = id_factura_ciclica;
            Nuevo_servicio.id_cat_estado_servicio_contratado = (int)Catalogos.Estado_Contrato_Servicio.Activo;
            Nuevo_servicio.id_ubicacion = Ubicacion;
            Nuevo_servicio.activo = true;
            Nuevo_servicio.eliminado = false;
            Nuevo_servicio.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            Nuevo_servicio.fecha_creacion = DateTime.Now;
            db.Contratos_Servicios.Add(Nuevo_servicio);

            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    db.SaveChanges();
                    foreach (var item_agente in Request.Form.AllKeys.Where(x => x.Contains("id_agente_")).Select(x => x.Replace("id_agente_", "")))
                    {
                        
                        int temp_agente = Convert.ToInt32(Request["id_agente_" + item_agente]);
                        int temp_cantidad = Convert.ToInt32(Request["cant_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["cant_agt_" + item_agente]))
                        {
                            temp_cantidad = Convert.ToInt32(Request["cant_agt_" + item_agente]);
                        }

                        int temp_costo = Convert.ToInt32(Request["costo_unitario_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["costo_unitario_agt_" + item_agente]))
                        {
                            temp_costo = Convert.ToInt32(Request["costo_unitario_agt_" + item_agente]);
                        }

                        int temp_venta = Convert.ToInt32(Request["precio_venta_unitario_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["precio_venta_unitario_agt_" + item_agente]))
                        {
                            temp_venta = Convert.ToInt32(Request["precio_venta_unitario_agt_" + item_agente]);
                        }
                        DateTime temp_inicio = Convert.ToDateTime(Request["fecha_inicio_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["fecha_inicio_agt_" + item_agente]))
                        {
                            temp_inicio = Convert.ToDateTime(Request["fecha_inicio_agt_" + item_agente]);
                        }
                        DateTime? temp_fin = null;
                        if (!String.IsNullOrEmpty(Request["fecha_fin_agt_" + item_agente]))
                        {
                            temp_fin = Convert.ToDateTime(Request["fecha_fin_agt_" + item_agente]);
                        }
                            Contratos_Agentes agente_con = Nuevo_Agente_Contratado();
                            agente_con.id_cat_tipo_agente = temp_agente;
                            agente_con.cantidad = temp_cantidad;
                            agente_con.costo = temp_costo;
                            agente_con.precio_venta_unitario = temp_venta;
                            agente_con.fecha_inicio = temp_inicio;
                            agente_con.fecha_fin = temp_fin;
                            agente_con.id_contrato_servicio = Nuevo_servicio.id_contrato_servicio;
                            db.Contratos_Agentes.Add(agente_con);

                    }
                    foreach (var item_servicio in Request.Form.AllKeys.Where(x => x.Contains("id_cat_otro_servicio_")).Select(x => x.Replace("id_cat_otro_servicio_", "")))
                    {
                        int temp_servicio = Convert.ToInt32(Request["id_cat_otro_servicio_" + item_servicio]);
                        int temp_cantidad = Convert.ToInt32(Request["cant_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["cant_serv_" + item_servicio]))
                        {
                            temp_cantidad = Convert.ToInt32(Request["cant_serv_" + item_servicio]);
                        }

                        int temp_costo = Convert.ToInt32(Request["costo_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["costo_serv_" + item_servicio]))
                        {
                            temp_costo = Convert.ToInt32(Request["costo_serv_" + item_servicio]);
                        }

                        int temp_venta = Convert.ToInt32(Request["venta_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["venta_serv_" + item_servicio]))
                        {
                            temp_venta = Convert.ToInt32(Request["venta_serv_" + item_servicio]);
                        }
                        DateTime temp_inicio = Convert.ToDateTime(Request["fecha_inicio_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["fecha_inicio_serv_" + item_servicio]))
                        {
                            temp_inicio = Convert.ToDateTime(Request["fecha_inicio_serv_" + item_servicio]);
                        }
                        DateTime? temp_fin = null;
                        if (!String.IsNullOrEmpty(Request["fecha_fin_serv_" + item_servicio]))
                        {
                            temp_fin = Convert.ToDateTime(Request["fecha_fin_serv_" + item_servicio]);
                        }
                            Contratos_Otros_Servicios servicio_con = Nuevo_Servicio_Contratado();
                            servicio_con.id_cat_otro_servicio = temp_servicio;
                            servicio_con.cantidad = temp_cantidad;
                            servicio_con.fecha_inicio = temp_inicio;
                            servicio_con.fecha_fin = temp_fin;
                            servicio_con.costo = temp_costo;
                            servicio_con.precio_venta_unitario = temp_venta;
                            servicio_con.id_contrato_servicio = Nuevo_servicio.id_contrato_servicio;
                            db.Contratos_Otros_Servicios.Add(servicio_con);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Details_U", new { id_ubicacion = Ubicacion });
                }
                catch (Exception)
                {
                    ViewBag.servicios = db.Contratos_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_ubicacion == Ubicacion && x.Cat_Tipos_Facturacion.ciclica == true
                                                                && (x.id_cat_estado_servicio_contratado != (int)Catalogos.Estado_Contrato_Servicio.Finalizado
                                                                || x.id_cat_estado_servicio_contratado != (int)Catalogos.Estado_Contrato_Servicio.Cancelado)).ToList(); 
                    var temp_ubicacion = db.Ubicaciones.Where(x => x.id_ubicacion == Ubicacion).FirstOrDefault();
                    ViewBag.ubicacion = temp_ubicacion.direccion;
                    ViewBag.razon_social = temp_ubicacion.Razones_Sociales.razon_social;
                    ViewBag.cliente = temp_ubicacion.Razones_Sociales.Clientes.nombre;
                    ContextMessage msg = new ContextMessage(ContextMessage.Warning, "Error con la conexión al servidor. Datos no guardados.");
                    tran.Rollback();
                    msg.ReturnUrl = Url.Action("Create_AS_U");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }

        }
        /* Create_Factura_Eventual
        // GET: Administracion/Razones_Sociales/Create
        public ActionResult Create_Factura_Eventual(int id)
        {
            ViewBag.id_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado), "id_cat_tipo_agente", "nombre");
            ViewBag.id_servicio = new SelectList(db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado), "id_cat_otro_servicio", "nombre");
            var temp_razon_social = db.Razones_Sociales.Where(x => x.activo && !x.eliminado).Where(x => x.id_razon_social == id).FirstOrDefault();
            ViewBag.id = id;
            ViewBag.razon_social = temp_razon_social.razon_social;
            ViewBag.cliente = temp_razon_social.Clientes.nombre;
            ViewBag.factura = db.Facturas_Eventuales.Where(x => x.id_razon_social == id).ToList();
            return View();
        }

        // POST: Administracion/Razones_Sociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create_Factura_Eventual(Contratos_Servicios Nuevo_servicio, int razon_social)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    int Total = 0;
                    List<Facturas_Eventuales_Agente> id_agentes = new List<Facturas_Eventuales_Agente>();
                    List<Facturas_Eventuales_Servicio> id_servicios = new List<Facturas_Eventuales_Servicio>();
                    foreach (var item_agente in Request.Form.AllKeys.Where(x => x.Contains("id_agente_")).Select(x => x.Replace("id_agente_", "")))
                    {

                        int temp_agente = Convert.ToInt32(Request["id_agente_" + item_agente]);
                        int temp_cantidad = Convert.ToInt32(Request["cant_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["cant_agt_" + item_agente]))
                        {
                            temp_cantidad = Convert.ToInt32(Request["cant_agt_" + item_agente]);
                        }

                        int temp_costo = Convert.ToInt32(Request["costo_unitario_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["costo_unitario_agt_" + item_agente]))
                        {
                            temp_costo = Convert.ToInt32(Request["costo_unitario_agt_" + item_agente]);
                        }

                        int temp_venta = Convert.ToInt32(Request["precio_venta_unitario_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["precio_venta_unitario_agt_" + item_agente]))
                        {
                            temp_venta = Convert.ToInt32(Request["precio_venta_unitario_agt_" + item_agente]);
                        }
                        DateTime temp_inicio = Convert.ToDateTime(Request["fecha_inicio_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["fecha_inicio_agt_" + item_agente]))
                        {
                            temp_inicio = Convert.ToDateTime(Request["fecha_inicio_agt_" + item_agente]);
                        }
                        DateTime temp_fin = Convert.ToDateTime(Request["fecha_fin_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["fecha_fin_agt_" + item_agente]))
                        {
                            temp_fin = Convert.ToDateTime(Request["fecha_fin_agt_" + item_agente]);
                        }
                        Facturas_Eventuales_Agente agente_con = new Facturas_Eventuales_Agente();
                        Total += temp_cantidad * temp_venta;
                        agente_con.id_cat_tipo_agente = temp_agente;
                        agente_con.cantidad = temp_cantidad;
                        agente_con.costo_de_venta = temp_costo;
                        agente_con.precio_de_venta = temp_venta;
                        agente_con.activo = true;
                        agente_con.eliminado = false;
                        agente_con.fecha_creacion = DateTime.Now;
                        agente_con.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        id_agentes.Add(agente_con);
                    }
                    foreach (var item_servicio in Request.Form.AllKeys.Where(x => x.Contains("id_cat_otro_servicio_")).Select(x => x.Replace("id_cat_otro_servicio_", "")))
                    {
                        int temp_servicio = Convert.ToInt32(Request["id_cat_otro_servicio_" + item_servicio]);
                        int temp_cantidad = Convert.ToInt32(Request["cant_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["cant_serv_" + item_servicio]))
                        {
                            temp_cantidad = Convert.ToInt32(Request["cant_serv_" + item_servicio]);
                        }

                        int temp_costo = Convert.ToInt32(Request["costo_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["costo_serv_" + item_servicio]))
                        {
                            temp_costo = Convert.ToInt32(Request["costo_serv_" + item_servicio]);
                        }

                        int temp_venta = Convert.ToInt32(Request["venta_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["venta_serv_" + item_servicio]))
                        {
                            temp_venta = Convert.ToInt32(Request["venta_serv_" + item_servicio]);
                        }
                        DateTime temp_inicio = Convert.ToDateTime(Request["fecha_inicio_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["fecha_inicio_serv_" + item_servicio]))
                        {
                            temp_inicio = Convert.ToDateTime(Request["fecha_inicio_serv_" + item_servicio]);
                        }
                        DateTime temp_fin = Convert.ToDateTime(Request["fecha_fin_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["fecha_fin_serv_" + item_servicio]))
                        {
                            temp_fin = Convert.ToDateTime(Request["fecha_fin_serv_" + item_servicio]);
                        }
                        Facturas_Eventuales_Servicio servicio_con = new Facturas_Eventuales_Servicio();
                        Total += temp_cantidad * temp_venta;
                        servicio_con.id_cat_otro_servicio = temp_servicio;
                        servicio_con.cantidad = temp_cantidad;
                        servicio_con.costo_de_venta = temp_costo;
                        servicio_con.precio_de_venta = temp_venta;
                        servicio_con.activo = true;
                        servicio_con.eliminado = false;
                        servicio_con.fecha_creacion = DateTime.Now;
                        servicio_con.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        id_servicios.Add(servicio_con);
                    }

                    Facturas_Eventuales factura_rapida = new Facturas_Eventuales();
                    factura_rapida.activo = true;
                    factura_rapida.eliminado = false;
                    int numero = 0;
                    var temp_numero_factura = db.Facturas.Where(x => x.activo && !x.eliminado).ToList();
                    var temp2 = db.Facturas_Eventuales.Where(x => x.activo && !x.eliminado).ToList();

                    if (temp_numero_factura.Count == 0 && temp2.Count == 0)
                    {
                        numero = 8500;
                    }
                    else if (temp_numero_factura.Count > 0 && temp2.Count > 0)
                    {
                        if (temp2.LastOrDefault().numero_factura > temp_numero_factura.LastOrDefault().numero_factura)
                        {
                            numero = temp2.LastOrDefault().numero_factura + 1;
                        }
                        else
                        {
                            numero = temp_numero_factura.LastOrDefault().numero_factura + 1;
                        }

                    }
                    else
                    {
                        if (temp2.Count() == 0)
                        {
                            numero = temp_numero_factura.LastOrDefault().numero_factura + 1;
                        }
                        else
                        {
                            numero = temp2.LastOrDefault().numero_factura + 1;
                        }
                    }
                    factura_rapida.numero_factura = numero;
                    factura_rapida.id_tipo_factura = 1;
                    factura_rapida.id_razon_social = razon_social;
                    factura_rapida.fecha_factura = DateTime.Now;
                    factura_rapida.descripcion = "Factura hecha el " + DateTime.Now.ToString("dd/MMM/yyyy");
                    factura_rapida.total = Total;
                    factura_rapida.id_estado_factura = 1;
                    factura_rapida.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    factura_rapida.fecha_creacion = DateTime.Now;
                    db.Facturas_Eventuales.Add(factura_rapida);

                    for (int i = 0; i < id_agentes.Count; i++)
                    {
                        id_agentes[i].id_factura_eventual = factura_rapida.id_factura_eventual;
                        db.Facturas_Eventuales_Agente.Add(id_agentes[i]);
                    }
                    for (int i = 0; i < id_servicios.Count; i++)
                    {
                        id_servicios[i].id_factura_eventual = factura_rapida.id_factura_eventual;
                        db.Facturas_Eventuales_Servicio.Add(id_servicios[i]);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Details_Factura_Eventual", new { id_factura_eventual = factura_rapida.id_factura_eventual });

                }
                catch (Exception)
                {
                    var temp_razon_social = db.Razones_Sociales.Where(x => x.activo && !x.eliminado).Where(x => x.id_razon_social == razon_social).FirstOrDefault();
                    ViewBag.razon_social = temp_razon_social.razon_social;
                    ViewBag.cliente = temp_razon_social.Clientes.nombre;
                    ViewBag.factura = db.Facturas_Eventuales.Where(x => x.id_razon_social == razon_social).ToList();
                    ContextMessage msg = new ContextMessage(ContextMessage.Warning, "Error con la conexión al servidor. Datos no guardados.");
                    tran.Rollback();
                    msg.ReturnUrl = Url.Action("Create_Factura_Eventual");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }

        }

        */
        private Contratos_Agentes Nuevo_Agente_Contratado()
        {
            Contratos_Agentes nuevo_contrato = new Contratos_Agentes();
            nuevo_contrato.activo = true;
            nuevo_contrato.eliminado = false;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            nuevo_contrato.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            nuevo_contrato.fecha_creacion = DateTime.Now;
            return nuevo_contrato;
        }

        private Contratos_Otros_Servicios Nuevo_Servicio_Contratado()
        {
            Contratos_Otros_Servicios nuevo_contrato = new Contratos_Otros_Servicios();
            nuevo_contrato.activo = true;
            nuevo_contrato.eliminado = false;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            nuevo_contrato.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            nuevo_contrato.fecha_creacion = DateTime.Now;
            return nuevo_contrato;
        }
        
        
        //Create Factura
        public ActionResult Create_Factura(long id_Razon_Social)
        {
            Razones_Sociales razon_Social = db.Razones_Sociales.Where(x => x.activo && !x.eliminado).Where(x => x.id_razon_social == id_Razon_Social).FirstOrDefault();
            ViewBag.Clientes = razon_Social.Clientes;
            ViewBag.Razon_Social = razon_Social;

            ViewBag.id_serie_factura = new SelectList(db.Series_Facturas.Where(x => x.activo && !x.eliminado).Select(s => new { id_tipo_factura = s.id_serie_factura, nombre = s.serie }), "id_tipo_factura", "nombre");

            ViewBag.Nit = razon_Social.nit;
            ViewBag.direccion = razon_Social.direccion_facturacion;
            ViewBag.id_razon_social = id_Razon_Social;
            ViewBag.facturas = db.Facturas.Where(x => x.activo && !x.eliminado).Where(x => x.id_razon_social == razon_Social.id_razon_social).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Factura(Facturas factura)
        {
            UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var razone_social = db.Razones_Sociales.Where(x => x.id_razon_social == factura.id_razon_social).FirstOrDefault();
            List<Contratos_Agentes> Contratos_Agentes = new List<Contratos_Agentes>();
            List<Contratos_Otros_Servicios> Contratos_Otros_Servicios = new List<Contratos_Otros_Servicios>();
            decimal Total = 0;
            var Contratos = db.Contratos_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_cat_estado_servicio_contratado == (int)Catalogos.Estado_Contrato_Servicio.Activo &&
                                                                                                   x.Cat_Tipos_Facturacion.ciclica == true &&
                                                                                                   x.Ubicaciones.Razones_Sociales.id_razon_social == factura.id_razon_social).ToList();
            bool Servicio_Finalizado = true;

            foreach (var item in Contratos)
            {
                var lista_agentes = db.Contratos_Agentes.Where(x => x.activo && !x.eliminado).Where(x => x.id_contrato_servicio == item.id_contrato_servicio &&
                                                                                                         x.fecha_inicio <= factura.fecha_factura).ToList();
                foreach (var item_agente in lista_agentes)
                {
                    if (item_agente.fecha_fin.HasValue)
                    {
                        if (item_agente.fecha_fin.Value.Year >= factura.fecha_factura.Year)
                        {
                            if (item_agente.fecha_fin.Value.Month <= factura.fecha_factura.Month)
                            {
                                Total += item_agente.cantidad * item_agente.precio_venta_unitario;
                                Contratos_Agentes.Add(item_agente);
                                if (item_agente.fecha_fin.Value.Month != factura.fecha_factura.Month || item_agente.fecha_fin.Value.Year != factura.fecha_factura.Year)
                                {
                                    Servicio_Finalizado = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        Total += item_agente.cantidad * item_agente.precio_venta_unitario;
                        Contratos_Agentes.Add(item_agente);
                        Servicio_Finalizado = false;
                    }
                }

                var lista_servicios = db.Contratos_Otros_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_contrato_servicio == item.id_contrato_servicio)
                                                          .Where(x => x.fecha_inicio <= factura.fecha_factura).ToList();
                foreach (var item_servicio in lista_servicios)
                {
                    if (item_servicio.fecha_fin.HasValue)
                    {
                        if (item_servicio.fecha_fin.Value.Year >= factura.fecha_factura.Year)
                        {
                            if (item_servicio.fecha_fin.Value.Month <= factura.fecha_factura.Month)
                            {
                                Total += item_servicio.cantidad * item_servicio.precio_venta_unitario;
                                Contratos_Otros_Servicios.Add(item_servicio);
                                if (item_servicio.fecha_fin.Value.Month != factura.fecha_factura.Month || item_servicio.fecha_fin.Value.Year != factura.fecha_factura.Year)
                                {
                                    Servicio_Finalizado = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        Total += item_servicio.cantidad * item_servicio.precio_venta_unitario;
                        Contratos_Otros_Servicios.Add(item_servicio);
                        Servicio_Finalizado = false;
                    }
                }
                if (Servicio_Finalizado)
                {
                    var Contratos_Finalizado = db.Contratos_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_contrato_servicio == item.id_contrato_servicio).ToList().FirstOrDefault();
                    Contratos_Finalizado.id_usuario_modificacion = usuario.usuario.id_usuario;
                    Contratos_Finalizado.fecha_modificacion = DateTime.Now;
                    Contratos_Finalizado.id_cat_estado_servicio_contratado = (int)Catalogos.Estado_Contrato_Servicio.Finalizado;
                    db.Entry(Contratos_Finalizado).State = EntityState.Modified;
                }
            }
            String msg = "";
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    //var razon_social = db.Razones_Sociales.Where(x => x.id_razon_social == factura.Razones_Sociales.
                    var temp = db.Facturas.Where(x => x.id_razon_social == factura.id_razon_social && x.fecha_factura.Month == factura.fecha_factura.Month && x.fecha_factura.Year == factura.fecha_factura.Year).ToList();
                    if (temp.Count == 0)
                    {
                        var temp_serie = db.Series_Facturas.Where(x => x.id_serie_factura == factura.id_serie_factura).FirstOrDefault();
                        temp_serie.id_usuario_modificacion = usuario.usuario.id_usuario;
                        temp_serie.fecha_modificacion = DateTime.Now;
                        temp_serie.correlativo = factura.numero_factura;
                        db.Entry(temp_serie).State = EntityState.Modified;
                        factura.id_usuario_creacion = usuario.usuario.id_usuario;
                        factura.serie = temp_serie.serie;
                        factura.id_cat_tipo_facturacion = db.Cat_Tipos_Facturacion.Where(x => x.ciclica == true).ToList().FirstOrDefault().id_cat_tipo_facturacion;
                        factura.nombre_factura = razone_social.razon_social;
                        factura.nit = razone_social.nit;
                        factura.direccion = razone_social.direccion_facturacion;
                        factura.fecha_creacion = DateTime.Now;
                        factura.id_cat_estado_factura = (int)Catalogos.Estado_Factura.creada;
                        factura.total = Total;
                        factura.activo = true;
                        factura.eliminado = false;
                        db.Facturas.Add(factura);

                        foreach (var item_factura_agente in Contratos_Agentes)
                        {
                            Facturas_Detalle fac_detalle_agente = new Facturas_Detalle();
                            fac_detalle_agente.id_factura = factura.id_factura;
                            fac_detalle_agente.id_contrato_agente = item_factura_agente.id_contrato_agente;
                            fac_detalle_agente.costo = item_factura_agente.costo;
                            fac_detalle_agente.precio_venta_unitario = item_factura_agente.precio_venta_unitario;
                            fac_detalle_agente.cantidad = item_factura_agente.cantidad;
                            fac_detalle_agente.descripcion = item_factura_agente.Cat_Tipos_Agente.nombre;
                            fac_detalle_agente.id_usuario_creacion = usuario.usuario.id_usuario;
                            fac_detalle_agente.fecha_creacion = DateTime.Now;
                            fac_detalle_agente.activo = true;
                            fac_detalle_agente.eliminado = false;
                            db.Facturas_Detalle.Add(fac_detalle_agente);
                        }
                        foreach (var item_factura_servicio in Contratos_Otros_Servicios)
                        {
                            Facturas_Detalle fac_detalle_otro_servicio = new Facturas_Detalle();
                            fac_detalle_otro_servicio.id_factura = factura.id_factura;
                            fac_detalle_otro_servicio.id_contrato_otro_servicio = item_factura_servicio.id_contrato_otro_servicio;
                            fac_detalle_otro_servicio.costo = item_factura_servicio.costo;
                            fac_detalle_otro_servicio.precio_venta_unitario = item_factura_servicio.precio_venta_unitario;
                            fac_detalle_otro_servicio.cantidad = item_factura_servicio.cantidad;
                            fac_detalle_otro_servicio.descripcion = item_factura_servicio.Cat_Otros_Servicios.nombre;
                            fac_detalle_otro_servicio.id_usuario_creacion = usuario.usuario.id_usuario;
                            fac_detalle_otro_servicio.activo = true;
                            fac_detalle_otro_servicio.eliminado = false;
                            fac_detalle_otro_servicio.fecha_creacion = DateTime.Now;
                            db.Facturas_Detalle.Add(fac_detalle_otro_servicio);
                        }
                        db.SaveChanges();
                        ViewBag.Razon_Social = razone_social;
                        ViewBag.Clientes = razone_social.Clientes;
                        ViewBag.id_razon_social = factura.id_razon_social;
                        //return RedirectToAction("Create_Factura", new { id_Razon_Social = factura.id_razon_social });
                        ContextMessage mens = new ContextMessage(ContextMessage.Success, "Servicios agregados exitosamente.");
                        tran.Commit();
                        mens.ReturnUrl = Url.Action("Details_Factura", new { id_factura = factura.id_factura });
                        TempData[User.Identity.Name] = mens;
                        return RedirectToAction("Create_Factura", new { id_Razon_Social = factura.id_razon_social });
                    }
                    else
                    {
                        msg = "La factura del mes seleccionado ya existe"; 
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    if (msg == "")
                    {
                        msg = "Error en la conexion a la base de datos";
                    }
                    ViewBag.Razon_Social = razone_social;
                    ViewBag.Clientes = razone_social.Clientes;
                    ViewBag.id_razon_social = factura.id_razon_social;
                    ContextMessage mens = new ContextMessage(ContextMessage.Warning, msg);
                    tran.Rollback();
                    mens.ReturnUrl = Url.Action("Create_Factura", new { id_Razon_Social = factura.id_razon_social });
                    TempData[User.Identity.Name] = mens;
                    return RedirectToAction("Mensaje");
                }
            }
            
        }
       
        #endregion

        // GET: Administracion/Clientes/Delete/5
        #region Delete
        public ActionResult Delete_C(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Razon_Social_Contacto rsc = db.Razon_Social_Contacto.Find(id);
            if (rsc == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            return View(rsc);
        }

        // POST: Administracion/Clientes/Delete/5
        [HttpPost, ActionName("Delete_C")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Razon_Social_Contacto rsc = db.Razon_Social_Contacto.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            rsc.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            rsc.fecha_eliminacion = DateTime.Now;
            rsc.eliminado = true;
            rsc.activo = false;
            db.Entry(rsc).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details_RS", new { id_Razon_Social = rsc.id_razon_social});
        }

        public ActionResult Delete_Agente_Contratado(int id)
        {
            Contratos_Agentes agente_contratado = db.Contratos_Agentes.Find(id);
            if (agente_contratado == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            return View(agente_contratado);
        }

        // POST: Administracion/Clientes/Delete/5
        [HttpPost, ActionName("Delete_Agente_Contratado")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Agente_Contratado_Confirmed(int id)
        {
            Contratos_Agentes agente = db.Contratos_Agentes.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            agente.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            agente.fecha_eliminacion = DateTime.Now;
            agente.eliminado = true;
            agente.activo = false;
            db.Entry(agente).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details_U", new { id_ubicacion = agente.Contratos_Servicios.id_ubicacion });
        }

        public ActionResult Delete_Servicio_Contratado(int id)
        {
            Contratos_Otros_Servicios tipo_servicio_contratado = db.Contratos_Otros_Servicios.Find(id);
            if (tipo_servicio_contratado == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            return View(tipo_servicio_contratado);
        }

        // POST: Administracion/Clientes/Delete/5
        [HttpPost, ActionName("Delete_Servicio_Contratado")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Servicio_Contratado_Confirmed(int id)
        {
            Contratos_Otros_Servicios tipo_servicio_contratado = db.Contratos_Otros_Servicios.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            tipo_servicio_contratado.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            tipo_servicio_contratado.fecha_eliminacion = DateTime.Now;
            tipo_servicio_contratado.eliminado = true;
            tipo_servicio_contratado.activo = false;
            db.Entry(tipo_servicio_contratado).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details_U", new { id_ubicacion = tipo_servicio_contratado.Contratos_Servicios.id_ubicacion });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        // GET: Administracion/Contactos/Create
        #region otros
        public ActionResult Create_contacto(int id)
        {
            ViewBag.id_contacto_puesto = new SelectList(db.Contacto_Puesto.Where(x => x.eliminado == false), "id_contacto_puesto", "descripcion");
            ViewBag.id = id;
            return View();
        }

        // POST: Administracion/Contactos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_contacto(int id, Contactos contactos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                contactos.activo = true;
                contactos.eliminado = false;
                contactos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                contactos.fecha_creacion = DateTime.Now;
                db.Contactos.Add(contactos);

                Razon_Social_Contacto rsc = new Razon_Social_Contacto();
                rsc.id_contacto = contactos.id_contacto;
                rsc.id_razon_social = id;
                rsc.activo = true;
                rsc.eliminado = false;
                rsc.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                rsc.fecha_creacion = DateTime.Now;
                db.Razon_Social_Contacto.Add(rsc);
                db.SaveChanges();
                return RedirectToAction("Details_RS", new { id_Razon_Social  = id});
            }

            ViewBag.id_contacto_puesto = new SelectList(db.Contacto_Puesto.Where(x => x.eliminado == false), "id_contacto_puesto", "descripcion", contactos.id_contacto_puesto);
            return View(contactos);
        }

        public ActionResult Asignar_contacto(int id)
        {
            ViewBag.id_contacto = new SelectList(db.Contactos.Where(x => x.eliminado == false), "id_contacto", "nombre");
            ViewBag.id = id;
            return View();
        }

        // POST: Administracion/Contactos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar_contacto(int id, Contactos contactos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Razon_Social_Contacto rsc = new Razon_Social_Contacto();
                rsc.id_contacto = contactos.id_contacto;
                rsc.id_razon_social = id;
                rsc.activo = true;
                rsc.eliminado = false;
                rsc.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                rsc.fecha_creacion = DateTime.Now;
                db.Razon_Social_Contacto.Add(rsc);
                db.SaveChanges();
                return RedirectToAction("Details_RS", new { id_Razon_Social = id });
            }

            ViewBag.id_contacto = new SelectList(db.Contactos.Where(x => x.eliminado == false), "id_contacto", "nombre");
            return View(contactos);
        }
        public ViewResult Mensaje()
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
        }
        #endregion

    }
}
