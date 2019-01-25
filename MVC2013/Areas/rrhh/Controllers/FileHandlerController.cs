using MVC2013.Models;
using MVC2013.Src.Comun.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class FileHandlerController : Controller
    {
        AppEntities db = new AppEntities();
        private Usuarios usuario {
            get { return Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario; }
        }

        // GET: rrhh/FileHandler
        public ActionResult GetFileCurso(int id_curso_empleado)
        {
            Curso_Empleado curso_empleado = db.Curso_Empleado.Find(id_curso_empleado);
            Archivo archivo = db.Archivo.Find(curso_empleado.id_archivo);
            if (archivo == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("GetFile", new { id_archivo = archivo.id_archivo });
        }

        public ActionResult GetFile(int id_archivo)
        {
            Archivo dataArchivo = db.Archivo.Find(id_archivo);
            if (dataArchivo == null)
            {
                return HttpNotFound();
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(dataArchivo.ubicacion));
            Response.Clear();
            string extension = dataArchivo.nombre.Substring(dataArchivo.nombre.LastIndexOf('.'));
            Response.ContentType = "application/" + extension;
            Response.AddHeader("content-disposition", "attachment; filename=\"" + dataArchivo.nombre + "\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            var x = new FileStreamResult(Response.OutputStream, Response.ContentType);
            return new FileStreamResult(Response.OutputStream, Response.ContentType);
        }
    }
}