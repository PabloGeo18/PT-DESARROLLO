using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Globalization;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Customers.Controllers
{
    public class Procesos_Facturacion_DetalleController : Controller
    {
        private AppEntities db = new AppEntities();
        CultureInfo cultura_Chapina = new CultureInfo("es-GT");

        // GET: Customers/Procesos_Facturacion_Detalle
        public ActionResult Facturacion_Detalle(int id)
        {
            //var series = new SelectList(db.Series_Facturas.Where(x => x.activo && !x.eliminado).Select(x => new {id = x.id_serie_factura, nombre = x.serie}), "id", "nombre");
            var Fecha_Proceso_factura = db.Procesos_Facturacion.Find(id).fecha_proceso;
            var procesos_Facturacion_Detalle = db.Procesos_Facturacion_Detalle.Where(x => x.id_proceso_facturacion == id && x.activo && !x.eliminado)
                .OrderBy(x => x.id_cat_estado_proceso_facturacion_detalle).ThenBy(x => x.id_cliente).ThenBy(x => x.id_razon_social).ThenBy(x => x.id_ubicacion);
            ViewBag.Mes = Fecha_Proceso_factura.ToString("MMMM", cultura_Chapina).ToUpper();
            ViewBag.Anio = Fecha_Proceso_factura.Year;
            ViewBag.id = id;

            return View(procesos_Facturacion_Detalle.ToList());
        }

        public ActionResult Contrato_Agente(int id)
        {
            var Fecha_Proceso_factura = db.Procesos_Facturacion.Find(id).fecha_proceso;
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion.Where(x => x.activo && !x.eliminado).Select(x => new { id = x.id_cat_tipo_facturacion, nombre = x.nombre }), "id", "nombre");
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(x => new { id = x.id_cliente, nombre = x.nombre }), "id", "nombre");
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(x => new { id = x.id_cat_tipo_agente, nombre = x.nombre }), "id", "nombre");
            ViewBag.id = id;
            ViewBag.Mes = Fecha_Proceso_factura.ToString("MMMM", cultura_Chapina).ToUpper();
            ViewBag.Anio = Fecha_Proceso_factura.Year;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contrato_Agente(Contratos_Servicios contrato, Contratos_Agentes agentes, int id_facturacion)
        {
            var Fecha_Proceso_factura = db.Procesos_Facturacion.Find(id_facturacion).fecha_proceso;
            try
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                var razon_social = db.Razones_Sociales.Find(contrato.id_razon_social);
                Procesos_Facturacion_Detalle facturacion_detalle = new Procesos_Facturacion_Detalle();
                facturacion_detalle.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                facturacion_detalle.fecha_creacion = DateTime.Now;
                facturacion_detalle.activo = true;
                facturacion_detalle.eliminado = false;
                facturacion_detalle.id_cat_tipo_facturacion = contrato.id_cat_tipo_facturacion.Value;
                facturacion_detalle.id_cliente = db.Razones_Sociales.Find(contrato.id_razon_social).id_cliente;
                facturacion_detalle.id_razon_social = agentes.id_razon_social;
                facturacion_detalle.id_ubicacion = agentes.id_ubicacion;
                facturacion_detalle.id_proceso_facturacion = id_facturacion;
                facturacion_detalle.id_cat_estado_proceso_facturacion_detalle = (int)Catalogos.Estado_Proceso_Facturacion_Detalle.Activo;
                facturacion_detalle.id_razon_social_grupo_factura = agentes.id_razon_social_grupo_factura;
                facturacion_detalle.id_cat_tipo_agente = agentes.id_cat_tipo_agente;
                facturacion_detalle.descripcion = db.Cat_Tipos_Agente.Find(agentes.id_cat_tipo_agente).nombre;
                facturacion_detalle.cantidad = agentes.cantidad;
                facturacion_detalle.costo = agentes.costo;
                facturacion_detalle.precio_venta_unitario = agentes.precio_venta_unitario;
                facturacion_detalle.fecha_inicio = agentes.fecha_inicio;
                facturacion_detalle.fecha_fin = agentes.fecha_fin;
                db.Procesos_Facturacion_Detalle.Add(facturacion_detalle);
                db.SaveChanges();

                return RedirectToAction("Facturacion_Detalle", new { id = id_facturacion });
            }
            catch (Exception)
            {
                ViewBag.Mes = Fecha_Proceso_factura.ToString("MMMM", cultura_Chapina).ToUpper();
                ViewBag.Anio = Fecha_Proceso_factura.Year;
                ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(x => new { id = x.id_cliente, nombre = x.nombre }), "id", "nombre");
                ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(x => new { id = x.id_cat_tipo_agente, nombre = x.nombre }), "id", "nombre");
                ViewBag.id = id_facturacion;
                return RedirectToAction("Contrato_Agente", new { id = id_facturacion });
            }
            
            
        }

        public ActionResult Contrato_Servicio(int id)
        {
            var Fecha_Proceso_factura = db.Procesos_Facturacion.Find(id).fecha_proceso;
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion.Where(x => x.activo && !x.eliminado).Select(x => new { id = x.id_cat_tipo_facturacion, nombre = x.nombre }), "id", "nombre");
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(x => new { id = x.id_cliente, nombre = x.nombre }), "id", "nombre");
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(x => new { id = x.id_cat_otro_servicio, nombre = x.nombre }), "id", "nombre");
            ViewBag.id = id;
            ViewBag.Mes = Fecha_Proceso_factura.ToString("MMMM", cultura_Chapina).ToUpper();
            ViewBag.Anio = Fecha_Proceso_factura.Year;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contrato_Servicio(Contratos_Servicios contrato, Contratos_Otros_Servicios servicio, int id_facturacion)
        {
            var Fecha_Proceso_factura = db.Procesos_Facturacion.Find(id_facturacion).fecha_proceso;
            try
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                servicio.id_contrato_servicio = contrato.id_contrato_servicio;

                Procesos_Facturacion_Detalle facturacion_detalle = new Procesos_Facturacion_Detalle();
                facturacion_detalle.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                facturacion_detalle.fecha_creacion = DateTime.Now;
                facturacion_detalle.activo = true;
                facturacion_detalle.eliminado = false;
                facturacion_detalle.id_cliente = db.Razones_Sociales.Find(contrato.id_razon_social).id_cliente;
                facturacion_detalle.id_razon_social = servicio.id_razon_social;
                facturacion_detalle.id_ubicacion = servicio.id_ubicacion;
                facturacion_detalle.id_cat_tipo_facturacion = contrato.id_cat_tipo_facturacion.Value;
                facturacion_detalle.id_proceso_facturacion = id_facturacion;
                facturacion_detalle.id_cat_tipo_facturacion = contrato.id_cat_tipo_facturacion.Value;
                facturacion_detalle.id_cat_estado_proceso_facturacion_detalle = (int)Catalogos.Estado_Proceso_Facturacion_Detalle.Activo;
                facturacion_detalle.id_razon_social_grupo_factura = servicio.id_razon_social_grupo_factura;
                facturacion_detalle.id_cat_otro_servicio = servicio.id_cat_otro_servicio;
                facturacion_detalle.descripcion = db.Cat_Otros_Servicios.Find(servicio.id_cat_otro_servicio).nombre;
                facturacion_detalle.cantidad = servicio.cantidad;
                facturacion_detalle.costo = servicio.costo;
                facturacion_detalle.precio_venta_unitario = servicio.precio_venta_unitario;
                facturacion_detalle.fecha_inicio = servicio.fecha_inicio;
                facturacion_detalle.fecha_fin = servicio.fecha_fin;
                db.Procesos_Facturacion_Detalle.Add(facturacion_detalle);
                db.SaveChanges();

                return RedirectToAction("Facturacion_Detalle", new { id = id_facturacion });
            }
            catch (Exception)
            {
                ViewBag.Mes = Fecha_Proceso_factura.ToString("MMMM", cultura_Chapina).ToUpper();
                ViewBag.Anio = Fecha_Proceso_factura.Year;
                ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(x => new { id = x.id_cliente, nombre = x.nombre }), "id", "nombre");
                ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(x => new { id = x.id_cat_tipo_agente, nombre = x.nombre }), "id", "nombre");
                ViewBag.id = id_facturacion;
                return RedirectToAction("Contrato_Agente", new { id = id_facturacion });
            }


        }

        public JsonResult Obtener_Ubicacion(int id)
        {
            List<object> resultados = new List<object>();
            var razon_social = db.Razones_Sociales.Find(id);
            foreach (var item_u in razon_social.Ubicaciones)
            {
                resultados.Add(new { id = item_u.id_ubicacion, nombre = item_u.nombre });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        #region Procesar_Facturacion
        public ActionResult Procesar_Facturacion()
        {
            ViewBag.id_clientes = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado).OrderBy(x => x.nombre).Select(s => new { id_cliente = s.id_cliente, nombre = s.nombre }), "id_cliente", "nombre");
            ViewBag.id_serie_factura = new SelectList(db.Series_Facturas.Where(x => x.activo && !x.eliminado).OrderBy(x => x.serie).Select(s => new { id = s.id_serie_factura, nombre = s.serie }), "id", "nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Procesar_Facturacion(List<int> check_todos, int id_serie_factura, DateTime fecha_factura, int tipo_factura)
        {
            if (tipo_factura == 1)
            {
                return Simple(check_todos, id_serie_factura, fecha_factura);
            }
            else
            {
                return Multiple(check_todos, id_serie_factura, fecha_factura);
            }
        }

        public ActionResult Simple(List<int> check_todos, int id_serie_factura, DateTime fecha_factura)
        {
            if (check_todos != null)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var serie = db.Series_Facturas.Find(id_serie_factura);
                int correlativo = serie.correlativo;
                //obtener contratos
                List<Procesos_Facturacion_Detalle> listado_proceso_facturacion_detalle = new List<Procesos_Facturacion_Detalle>();
                db.SaveChanges();
                foreach (var item in check_todos)
                {
                    var contrato = db.Procesos_Facturacion_Detalle.Find(item);
                    listado_proceso_facturacion_detalle.Add(contrato);
                    contrato.id_cat_estado_proceso_facturacion_detalle = (int)Catalogos.Estado_Proceso_Facturacion_Detalle.Facturado;
                    contrato.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                    contrato.fecha_modificacion = DateTime.Now;
                    db.Entry(contrato).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var razon_social = db.Razones_Sociales.Find(listado_proceso_facturacion_detalle.First().id_razon_social);
                Facturas factura = Informacion_Factura();
                factura.id_serie_factura = id_serie_factura;
                factura.serie = serie.serie;
                factura.numero_factura = correlativo;
                correlativo = correlativo + 1;
                factura.id_razon_social = razon_social.id_razon_social;
                factura.nit = razon_social.nit;
                factura.direccion = razon_social.direccion_facturacion;
                List<string> listado_fechas = fechas(listado_proceso_facturacion_detalle.ToList());
                String fecha = (listado_fechas.Count() > 1) ? " EN LOS MESES DE " : " EN EL MES DE ";
                for (int i = 0; i < listado_fechas.Count; i++)
                {
                    if (listado_fechas.Count() - (i + 1) == 0)
                    {
                        fecha += listado_fechas[i];
                    }
                    else if (listado_fechas.Count() - (i + 1) == 1)
                    {
                        fecha += listado_fechas[i] + " Y ";
                    }
                    else
                    {
                        fecha += listado_fechas[i] + ", ";
                    }

                }
                factura.descripcion = razon_social.descripcion_factura.Replace("@Meses", fecha);
                factura.fecha_factura = fecha_factura;
                factura.nombre_factura = razon_social.nombre_comercial;
                factura.id_cat_estado_factura = (int)Catalogos.Estado_Factura.creada;
                factura.total = listado_proceso_facturacion_detalle.Select(x => x.cantidad * x.precio_venta_unitario).Sum();
                factura.fecha_cobro = factura.fecha_factura.AddDays(razon_social.dias_credito);
                db.Facturas.Add(factura);
                db.SaveChanges();

                foreach (var item_factura_actual in listado_proceso_facturacion_detalle)
                {
                    Facturas_Detalle facturas_detalle = Informacion_Detalle_Factura();
                    facturas_detalle.id_factura = factura.id_factura;
                    facturas_detalle.id_proceso_facturacion_detalle = item_factura_actual.id_proceso_facturacion_detalle;
                    if (item_factura_actual.id_contrato_agente.HasValue)
                    {
                        facturas_detalle.id_contrato_agente = item_factura_actual.id_contrato_agente;
                        facturas_detalle.id_cat_tipo_agente = item_factura_actual.id_cat_tipo_agente;

                    }
                    else if (item_factura_actual.id_contrato_otro_servicio.HasValue)
                    {
                        facturas_detalle.id_cat_otro_servicio = item_factura_actual.id_cat_otro_servicio;
                        facturas_detalle.id_contrato_otro_servicio = item_factura_actual.id_contrato_otro_servicio;
                    }
                    facturas_detalle.descripcion = item_factura_actual.descripcion;
                    facturas_detalle.cantidad = item_factura_actual.cantidad;
                    facturas_detalle.precio_venta_unitario = item_factura_actual.precio_venta_unitario;
                    facturas_detalle.costo = item_factura_actual.costo;
                    db.Facturas_Detalle.Add(facturas_detalle);
                    db.SaveChanges();
                }

                serie.correlativo = correlativo;
                serie.fecha_modificacion = DateTime.Now;
                serie.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                db.Entry(serie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index_rs", "Facturas", new { id = razon_social.id_razon_social });
            }
            return RedirectToAction("Procesar_Facturacion");
        }

        public ActionResult Multiple(List<int> check_todos, int id_serie_factura, DateTime fecha_factura)
        {
            if (check_todos != null)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var serie = db.Series_Facturas.Find(id_serie_factura);
                int correlativo = serie.correlativo;
                //obtener contratos
                List<Procesos_Facturacion_Detalle> listado_proceso_facturacion_detalle = new List<Procesos_Facturacion_Detalle>();
                db.SaveChanges();
                foreach (var item in check_todos)
                {
                    var contrato = db.Procesos_Facturacion_Detalle.Find(item);
                    listado_proceso_facturacion_detalle.Add(contrato);
                    contrato.id_cat_estado_proceso_facturacion_detalle = (int)Catalogos.Estado_Proceso_Facturacion_Detalle.Facturado;
                    contrato.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                    contrato.fecha_modificacion = DateTime.Now;
                    db.Entry(contrato).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var tabla_proceso_facturacion = listado_proceso_facturacion_detalle;
                var razon_social = db.Razones_Sociales.Find(listado_proceso_facturacion_detalle.First().id_razon_social);
                tabla_proceso_facturacion = tabla_proceso_facturacion.OrderBy(x => (x.id_razon_social_grupo_factura.HasValue) ? x.id_razon_social_grupo_factura.Value : 0).ToList();
                var Grupos_facturas = listado_proceso_facturacion_detalle.Select(x => x.id_razon_social_grupo_factura).GroupBy(x => (x.HasValue) ? x.Value : 0).ToList();
                foreach (var item in Grupos_facturas)
                {

                    var tabla_agrupada = tabla_proceso_facturacion.Where(x => ((x.id_razon_social_grupo_factura.HasValue) ? x.id_razon_social_grupo_factura.Value : 0) == item.Key).ToList();

                    Facturas factura = Informacion_Factura();
                    factura.id_serie_factura = id_serie_factura;
                    factura.serie = serie.serie;
                    factura.numero_factura = correlativo;
                    correlativo = correlativo + 1;
                    factura.id_razon_social = razon_social.id_razon_social;
                    factura.nit = razon_social.nit;
                    factura.direccion = razon_social.direccion_facturacion;
                    List<string> listado_fechas = fechas(tabla_agrupada.ToList());
                    String fecha = (listado_fechas.Count() > 1) ? " EN LOS MESES DE " : " EN EL MES DE ";
                    for (int i = 0; i < listado_fechas.Count; i++)
                    {
                        if (listado_fechas.Count() - (i + 1) == 0)
                        {
                            fecha += listado_fechas[i];
                        }
                        else if (listado_fechas.Count() - (i + 1) == 1)
                        {
                            fecha += listado_fechas[i] + " Y ";
                        }
                        else
                        {
                            fecha += listado_fechas[i] + ", ";
                        }

                    }
                    if (item.Key == 0)
                    {
                        factura.descripcion = razon_social.descripcion_factura.Replace("@Meses", fecha);
                    }
                    else
                    {
                        var desc_grupo = db.Razon_Social_Grupos_Factura.Find(item.Key).descripcion_factura;
                        factura.descripcion = desc_grupo.Replace("@Meses", fecha);
                    }
                    factura.fecha_factura = fecha_factura;
                    factura.nombre_factura = razon_social.nombre_comercial;
                    factura.id_cat_estado_factura = (int)Catalogos.Estado_Factura.creada;
                    factura.total = tabla_agrupada.Select(x => x.cantidad * x.precio_venta_unitario).Sum();
                    factura.fecha_cobro = factura.fecha_factura.AddDays(razon_social.dias_credito);
                    db.Facturas.Add(factura);
                    db.SaveChanges();
                    foreach (var item_factura_actual in tabla_agrupada)
                    {
                        Facturas_Detalle facturas_detalle = Informacion_Detalle_Factura();
                        facturas_detalle.id_factura = factura.id_factura;
                        facturas_detalle.id_proceso_facturacion_detalle = item_factura_actual.id_proceso_facturacion_detalle;
                        if (item_factura_actual.id_contrato_agente.HasValue)
                        {
                            facturas_detalle.id_contrato_agente = item_factura_actual.id_contrato_agente;
                            facturas_detalle.id_cat_tipo_agente = item_factura_actual.id_cat_tipo_agente;

                        }
                        else if (item_factura_actual.id_contrato_otro_servicio.HasValue)
                        {
                            facturas_detalle.id_cat_otro_servicio = item_factura_actual.id_cat_otro_servicio;
                            facturas_detalle.id_contrato_otro_servicio = item_factura_actual.id_contrato_otro_servicio;
                        }
                        facturas_detalle.descripcion = item_factura_actual.descripcion;
                        facturas_detalle.cantidad = item_factura_actual.cantidad;
                        facturas_detalle.precio_venta_unitario = item_factura_actual.precio_venta_unitario;
                        facturas_detalle.costo = item_factura_actual.costo;
                        db.Facturas_Detalle.Add(facturas_detalle);
                        db.SaveChanges();
                    }
                }
                serie.correlativo = correlativo;
                serie.fecha_modificacion = DateTime.Now;
                serie.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                db.Entry(serie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index_rs", "Facturas", new { id = razon_social.id_razon_social });
            }
            return RedirectToAction("Procesar_Facturacion");
        }

        public List<string> fechas(List<Procesos_Facturacion_Detalle> tabla)
        {
            List<string> listado_fechas = new List<string>();
            for (int i = 0; i < tabla.Count(); i++)
            {
                string fecha_escrita = tabla[i].Procesos_Facturacion.fecha_proceso.ToString("MMMM", cultura_Chapina).ToUpper() + " " + tabla[i].Procesos_Facturacion.fecha_proceso.Year;
                if (!listado_fechas.Contains(fecha_escrita))
                {
                    listado_fechas.Add(fecha_escrita);
                }
            }

            return listado_fechas;
        }

        public Facturas Informacion_Factura()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Facturas factura = new Facturas();
            factura.activo = true;
            factura.eliminado = false;
            factura.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            factura.fecha_creacion = DateTime.Now;
            return factura;
        }

        public Facturas_Detalle Informacion_Detalle_Factura()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Facturas_Detalle facturas_detalle = new Facturas_Detalle();
            facturas_detalle.activo = true;
            facturas_detalle.eliminado = false;
            facturas_detalle.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            facturas_detalle.fecha_creacion = DateTime.Now;
            return facturas_detalle;
        }

        public JsonResult Obtener_Razones_Sociales(int id)
        {
            List<object> resultados = new List<object>();
            var clientes = db.Clientes.Find(id);
            foreach (var item_RS in clientes.Razones_Sociales)
            {
                resultados.Add(new { item_RS.id_razon_social, nombre = item_RS.razon_social });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Obtener_Grupos_Factura(int id)
        {
            List<object> resultados = new List<object>();
            var rz = db.Razones_Sociales.Find(id);
            foreach (var item_gf in rz.Razon_Social_Grupos_Factura)
            {
                resultados.Add(new { id = item_gf.id_razon_social_grupo_factura, nombre = item_gf.correlativo + ". " + item_gf.nombre });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Obtener_Serie_Correlativo(int id)
        {
            var serie = db.Series_Facturas.Find(id);
            int correlativo = serie.correlativo;
            return Json(correlativo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Obtener_Contratos(int id_cliente, int id_rs, int? id_rzgf)
        {
            List<object> resultados = new List<object>();
            var contratos = db.Procesos_Facturacion_Detalle.Where(x => x.activo && !x.eliminado && x.id_cat_estado_proceso_facturacion_detalle == (int)Catalogos.Estado_Proceso_Facturacion_Detalle.Activo && x.id_cliente == id_cliente && x.id_razon_social == id_rs);
            if (id_rzgf.HasValue)
            {
                contratos = contratos.Where(x => x.id_razon_social_grupo_factura == id_rzgf.Value);
            }
            int temp = contratos.ToList().Count();
            contratos = contratos.OrderBy(x => x.Procesos_Facturacion.fecha_proceso).ThenBy(x => x.Contratos_Servicios.Cat_Tipos_Facturacion.nombre).ThenBy(x => x.descripcion);
            decimal total_cliente = 0;
            foreach (var item in contratos.ToList())
            {
                resultados.Add(new
                {
                    id = item.id_proceso_facturacion_detalle,
                    ubicacion = (item.id_ubicacion.HasValue) ? item.Ubicaciones.nombre : "----",
                    tipo_facturacion = item.Cat_Tipos_Facturacion.nombre,
                    nombre = item.descripcion,
                    cantidad = item.cantidad,
                    precio = item.precio_venta_unitario.ToString("C", cultura_Chapina),
                    total = (item.precio_venta_unitario * item.cantidad).ToString("C", cultura_Chapina),
                    fecha = item.Procesos_Facturacion.fecha_proceso.ToString("MMMM", cultura_Chapina).ToUpper() + " " + item.Procesos_Facturacion.fecha_proceso.Year,
                    grupo_factura = (item.id_razon_social_grupo_factura.HasValue) ? item.Razon_Social_Grupos_Factura.nombre : "----"
                });

                total_cliente += item.precio_venta_unitario * item.cantidad;
            }
            string retornar_total = total_cliente.ToString("C", cultura_Chapina);
            return Json(new { resultados, retornar_total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult Procesar_Facturacion_Cliente(int id)
        {
            var procesos_Facturacion_Detalle = db.Procesos_Facturacion_Detalle.Where(x => x.id_cliente == id).Include(x => x.Procesos_Facturacion).OrderBy(x => x.Procesos_Facturacion.fecha_proceso).ThenBy(x => x.Razones_Sociales.razon_social);

            return View(procesos_Facturacion_Detalle.ToList());
        }

        public ActionResult Procesar_Facturacion_RS(int id)
        {
            var procesos_Facturacion_Detalle = db.Procesos_Facturacion_Detalle.Where(x => x.id_razon_social == id).Include(x => x.Procesos_Facturacion).OrderBy(x => x.Procesos_Facturacion.fecha_proceso).ThenBy(x => x.Razones_Sociales.razon_social);

            return View(procesos_Facturacion_Detalle.ToList());
        }

        public JsonResult Mostrar_Razones_Sociales(int id)
        {
            List<object> resultados = new List<object>();
            var razon = db.Razones_Sociales.Where(e => !e.eliminado && e.activo && e.id_cliente == id);
            foreach (var c in razon)
            {
                resultados.Add(new { c.id_razon_social, c.razon_social });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        // GET: Customers/Procesos_Facturacion_Detalle/Details/5
        public ActionResult Details(int? id)
        {
            Procesos_Facturacion_Detalle procesos_Facturacion_Detalle = db.Procesos_Facturacion_Detalle.Find(id);
            if (procesos_Facturacion_Detalle == null)
            {
                return HttpNotFound();
            }
            return View(procesos_Facturacion_Detalle);
        }

        // GET: Customers/Procesos_Facturacion_Detalle/Create
        public ActionResult Create()
        {
            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre");
            ViewBag.id_razon_social = new SelectList(db.Razones_Sociales, "id_razon_social", "razon_social");
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones, "id_ubicacion", "direccion");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios, "id_cat_otro_servicio", "nombre");
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente, "id_cat_tipo_agente", "nombre");
            ViewBag.id_contrato_agente = new SelectList(db.Contratos_Agentes, "id_contrato_agente", "descripcion_eliminacion");
            ViewBag.id_contrato_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion");
            ViewBag.id_contrato_servicio = new SelectList(db.Contratos_Servicios, "id_contrato_servicio", "id_contrato_servicio");
            ViewBag.id_proceso_facturacion = new SelectList(db.Procesos_Facturacion, "id_proceso_facturacion", "id_proceso_facturacion");
            return View();
        }

        // POST: Customers/Procesos_Facturacion_Detalle/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Procesos_Facturacion_Detalle procesos_Facturacion_Detalle)
        {
            if (ModelState.IsValid)
            {
                db.Procesos_Facturacion_Detalle.Add(procesos_Facturacion_Detalle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre", procesos_Facturacion_Detalle.id_cliente);
            ViewBag.id_razon_social = new SelectList(db.Razones_Sociales, "id_razon_social", "razon_social", procesos_Facturacion_Detalle.id_razon_social);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones, "id_ubicacion", "direccion", procesos_Facturacion_Detalle.id_ubicacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion_Detalle.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion_Detalle.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion_Detalle.id_usuario_modificacion);
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios, "id_cat_otro_servicio", "nombre", procesos_Facturacion_Detalle.id_cat_otro_servicio);
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente, "id_cat_tipo_agente", "nombre", procesos_Facturacion_Detalle.id_cat_tipo_agente);
            ViewBag.id_contrato_agente = new SelectList(db.Contratos_Agentes, "id_contrato_agente", "descripcion_eliminacion", procesos_Facturacion_Detalle.id_contrato_agente);
            ViewBag.id_contrato_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion", procesos_Facturacion_Detalle.id_contrato_otro_servicio);
            ViewBag.id_contrato_servicio = new SelectList(db.Contratos_Servicios, "id_contrato_servicio", "id_contrato_servicio", procesos_Facturacion_Detalle.id_contrato_servicio);
            ViewBag.id_proceso_facturacion = new SelectList(db.Procesos_Facturacion, "id_proceso_facturacion", "id_proceso_facturacion", procesos_Facturacion_Detalle.id_proceso_facturacion);
            return View(procesos_Facturacion_Detalle);
        }

        // GET: Customers/Procesos_Facturacion_Detalle/Edit/5
        public ActionResult Edit(int? id)
        {
            Procesos_Facturacion_Detalle procesos_Facturacion_Detalle = db.Procesos_Facturacion_Detalle.Find(id);
            
            if (procesos_Facturacion_Detalle == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_razon_social_grupo_factura = new SelectList(db.Razon_Social_Grupos_Factura.Where(x => x.activo && !x.eliminado && x.id_razon_social == procesos_Facturacion_Detalle.id_razon_social)
                                                                       .Select(x => new { id = x.id_razon_social_grupo_factura, nombre = x.correlativo + ". " + x.nombre }), "id", "nombre", procesos_Facturacion_Detalle.id_razon_social_grupo_factura);
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion.Where(x => x.activo && !x.eliminado).Select(x => new { id = x.id_cat_tipo_facturacion, nombre = x.nombre }), "id", "nombre", procesos_Facturacion_Detalle.id_cat_tipo_facturacion);

            return View(procesos_Facturacion_Detalle);
        }

        // POST: Customers/Procesos_Facturacion_Detalle/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Procesos_Facturacion_Detalle procesos_Facturacion_Detalle)
        {
            Procesos_Facturacion_Detalle Temp_procesos_Facturacion_Detalle = db.Procesos_Facturacion_Detalle.Find(procesos_Facturacion_Detalle.id_proceso_facturacion_detalle);
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Temp_procesos_Facturacion_Detalle.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                Temp_procesos_Facturacion_Detalle.fecha_modificacion = DateTime.Now;
                Temp_procesos_Facturacion_Detalle.id_cat_tipo_facturacion = procesos_Facturacion_Detalle.id_cat_tipo_facturacion;
                Temp_procesos_Facturacion_Detalle.cantidad = procesos_Facturacion_Detalle.cantidad;
                Temp_procesos_Facturacion_Detalle.costo = procesos_Facturacion_Detalle.costo;
                Temp_procesos_Facturacion_Detalle.precio_venta_unitario = procesos_Facturacion_Detalle.precio_venta_unitario;
                Temp_procesos_Facturacion_Detalle.fecha_inicio = procesos_Facturacion_Detalle.fecha_inicio;
                Temp_procesos_Facturacion_Detalle.fecha_fin = procesos_Facturacion_Detalle.fecha_fin;
                db.Entry(Temp_procesos_Facturacion_Detalle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Facturacion_Detalle", new { id = Temp_procesos_Facturacion_Detalle.id_proceso_facturacion });
            }
            ViewBag.id_razon_social_grupo_factura = new SelectList(db.Razon_Social_Grupos_Factura.Where(x => x.activo && !x.eliminado && x.id_razon_social == procesos_Facturacion_Detalle.id_razon_social)
                                                                       .Select(x => new { id = x.id_razon_social_grupo_factura, nombre = x.correlativo + ". " + x.nombre }), "id", "nombre", procesos_Facturacion_Detalle.id_razon_social_grupo_factura);
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion.Where(x => x.activo && !x.eliminado).Select(x => new { id = x.id_cat_tipo_facturacion, nombre = x.nombre }), "id", "nombre", procesos_Facturacion_Detalle.id_cat_tipo_facturacion);

            return View(Temp_procesos_Facturacion_Detalle);
        }

        // GET: Customers/Procesos_Facturacion_Detalle/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procesos_Facturacion_Detalle procesos_Facturacion_Detalle = db.Procesos_Facturacion_Detalle.Find(id);
            if (procesos_Facturacion_Detalle == null)
            {
                return HttpNotFound();
            }
            return View(procesos_Facturacion_Detalle);
        }

        // POST: Customers/Procesos_Facturacion_Detalle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Procesos_Facturacion_Detalle procesos_Facturacion_Detalle = db.Procesos_Facturacion_Detalle.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            procesos_Facturacion_Detalle.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            procesos_Facturacion_Detalle.fecha_eliminacion = DateTime.Now;
            procesos_Facturacion_Detalle.eliminado = true;
            procesos_Facturacion_Detalle.activo = false;
            db.Entry(procesos_Facturacion_Detalle).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Facturacion_Detalle", new { id = procesos_Facturacion_Detalle.id_proceso_facturacion });
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
