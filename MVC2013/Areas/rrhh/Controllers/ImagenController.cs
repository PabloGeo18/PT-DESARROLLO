using MVC2013.Models;
using MVC2013.Src.Comun.Util;
using MVC2013.Src.Comun.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class ImagenController : Controller
    {

        private AppEntities db = new AppEntities();

        // GET: rrhh/Imagen
        public ActionResult TomarImagen()
        {
            string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg";
            try
            {
                if (System.IO.File.Exists(ruta_imagen_webcam))
                {
                    System.IO.File.Delete(ruta_imagen_webcam);
                }
            }
            catch
            {

            }
            return View();
        }

        public void TomarFoto()
        {
            string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg";
            var stream = Request.InputStream;
            string dumb;
            using (var reader = new StreamReader(stream))
            {
                dumb = reader.ReadToEnd();
            }
            var path = Server.MapPath(ruta_imagen_webcam);
            System.IO.File.WriteAllBytes(path, String_To_Bytes2(dumb));
        }

        public ActionResult OtenerImagenCamara()
        {
            try
            {
                string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg";
                var image_byte = System.IO.File.ReadAllBytes(Server.MapPath(ruta_imagen_webcam));
                return File(image_byte, "image/.jpg");
            }
            catch { return HttpNotFound(); }
        }

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];
            for (int x = 0; x < numBytes; x++)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }
            return bytes;
        }

        public ActionResult Camara()
        {
            ViewBag.id = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return View();
        }

        public ActionResult WebCam()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GuardarImagen(int id_empleado) 
        {
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.id_empleado == id_empleado && e.activo);
            if(empleado == null)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "Empleado no encontrado. Imagen no fue guardada.");
                msg.ReturnUrl = Url.Action("TomarImagen");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            var imagenes = empleado.Archivo_Empleado.Where(a => a.activo && a.Archivo.id_tipo_archivo == (int)Catalogos.Tipo_Archivo.Foto_Perfil);
            string error = "";
            string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg";
            if(imagenes.Count() > 0) //Hay foto de perfil
            {
                var imagen = imagenes.First();
                error += "1";
                System.IO.File.Copy(Server.MapPath(ruta_imagen_webcam), Server.MapPath(imagen.Archivo.ubicacion), true);
                ContextMessage msg = new ContextMessage(ContextMessage.Success, "Imagen guardada con éxito.");
                msg.ReturnUrl = Url.Action("TomarImagen");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            else //No hay foto de perfil
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Archivo imagenPerfil = new Archivo();
                        string name = "foto_" + empleado.id_empleado;
                        string extension = ".jpg";
                        imagenPerfil.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        imagenPerfil.fecha_creacion = DateTime.Now;
                        imagenPerfil.activo = true;
                        imagenPerfil.eliminado = false;
                        imagenPerfil.nombre = name + extension;
                        imagenPerfil.id_tipo_archivo = (int)Catalogos.Tipo_Archivo.Foto_Perfil;
                        imagenPerfil.ubicacion = "~/Archivos/Foto Perfil/" + name + extension;
                        error += "2";
                        db.Archivo.Add(imagenPerfil);
                        db.SaveChanges();
                        if (System.IO.File.Exists(Server.MapPath(imagenPerfil.ubicacion)))
                        {
                            System.IO.File.Delete(Server.MapPath(imagenPerfil.ubicacion));
                        }
                        System.IO.File.Move(Server.MapPath(ruta_imagen_webcam), Server.MapPath(imagenPerfil.ubicacion));
                        error += "3";
                        //Relacionar la imagen con el usuario
                        Archivo_Empleado nuevo_archivo_empleado = new Archivo_Empleado();
                        nuevo_archivo_empleado.id_archivo = imagenPerfil.id_archivo;
                        nuevo_archivo_empleado.id_empleado = empleado.id_empleado;
                        nuevo_archivo_empleado.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        nuevo_archivo_empleado.fecha_creacion = DateTime.Now;
                        nuevo_archivo_empleado.activo = true;
                        nuevo_archivo_empleado.eliminado = false;
                        db.Archivo_Empleado.Add(nuevo_archivo_empleado);
                        db.SaveChanges();
                        tran.Commit();
                        ContextMessage msg = new ContextMessage(ContextMessage.Success, "Imagen guardada con éxito.");
                        msg.ReturnUrl = Url.Action("TomarImagen");
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "Ocurrió un errror innesperado. Imagen no guardada." + error + ex.Message);
                        msg.ReturnUrl = Url.Action("TomarImagen");
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
                
            }

        }



        public ViewResult Mensaje()
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
        }

    }
}