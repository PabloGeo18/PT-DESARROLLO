using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC2013.Models;
using System.Web.Security;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;
using System.Collections;
using System.Configuration;
using MVC2013.Src.Comun.View;
using System.Data.Entity;
using System.IO;

namespace MVC2013.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private AppEntities db = new AppEntities();
        private string ruta_imagen_default = "~/Archivos/Templates/foto_perfil_default.png";

        public AccountController(){}

        // GET: /Account/MasterLogin
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult MasterLogin()
        {
            // Cada vez que se ingresa a esta pantalla ocurre deslogueo automatico
            if (!String.IsNullOrEmpty(User.Identity.Name) && Cache.DiccionarioUsuariosLogueados.ContainsKey(User.Identity.Name))
            {
                Cache.DiccionarioUsuariosLogueados.Remove(User.Identity.Name);
            }
            FormsAuthentication.SignOut();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            var empresas = (from config in ((Hashtable)ConfigurationManager.GetSection("Empresas")).Cast<DictionaryEntry>()
                            select new
                            {
                                key = config.Key,
                                value = config.Value
                            }).ToList();
            ViewBag.Empresa = new SelectList(empresas, "value", "key");
            if (TempData.ContainsKey("SessionExpired"))
            {
                ViewBag.session_expired = "Por seguridad su sesion ha sido expirada ya que se ingreso con el mismo usuario en otro ordenador.";
                TempData.Remove("SessionExpired");
            }
            
            ViewBag.Usuario_Login = new SelectList(db.Usuarios.Where(u => !u.bloqueo_habilitado), "id_usuario", "usuario");
            return View();
        }

        // POST: /Account/MasterLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult MasterLogin(MasterLoginViewModel model, string returnUrl)
        {
            UsuarioTO usuarioTO;

            if (ModelState.IsValid)
            {
                usuarioTO = model.IsValid(model.Usuario, model.Password, model.Empresa, this.Request.UserHostAddress, model.Usuario_Login);
                if (usuarioTO.Valid)
                {
                    FormsAuthentication.SetAuthCookie(usuarioTO.usuario.usuario, false);
                    usuarioTO.SessionId = System.Web.HttpContext.Current.Session.SessionID;
                    if (!Cache.DiccionarioUsuariosLogueados.ContainsKey(usuarioTO.usuario.usuario))
                    {
                        Cache.DiccionarioUsuariosLogueados.Add(usuarioTO.usuario.usuario, usuarioTO);
                    }
                    else
                    {
                        TempData["HasExpiredSession"] = true;
                        Cache.DiccionarioUsuariosLogueados[usuarioTO.usuario.usuario].SessionId = System.Web.HttpContext.Current.Session.SessionID;
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Credenciales incorrectas o no esta autorizado.");
                }
                var empresas = (from config in ((Hashtable)ConfigurationManager.GetSection("Empresas")).Cast<DictionaryEntry>()
                                select new
                                {
                                    key = config.Key,
                                    value = config.Value
                                }).ToList();
                ViewBag.Empresa = new SelectList(empresas, "value", "key");
                ViewBag.Usuario_Login = new SelectList(db.Usuarios.Where(u => !u.bloqueo_habilitado), "id_usuario", "usuario");
                return View(model);
            }


            return View(model);

        }

        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    if (usuarioTO != null)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        FormsAuthentication.SignOut();
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    }
                }
                else
                {
                    FormsAuthentication.SignOut();
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                }
            }
            catch
            {
                //ContextMessage msg = new ContextMessage(ContextMessage.Error, "Error de ingreso. Consulte con soporte.");
                //return View(ContextMessage.ViewLocation(this), msg);
            }

            ViewBag.ReturnUrl = returnUrl;
            var empresas = (from config in ((Hashtable)ConfigurationManager.GetSection("Empresas")).Cast<DictionaryEntry>()
                            select new
                            {
                                key = config.Key,
                                value = config.Value
                            }).ToList();
            ViewBag.Empresa = new SelectList(empresas, "value", "key");
            if (TempData.ContainsKey("SessionExpired"))
            {
                ViewBag.session_expired = "Por seguridad su sesion ha sido expirada ya que se ingreso con el mismo usuario en otro ordenador.";
                TempData.Remove("SessionExpired");
            }
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            UsuarioTO usuarioTO;

            try
            {
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    if (usuarioTO != null)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        FormsAuthentication.SignOut();
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    }
                }
                else
                {
                    FormsAuthentication.SignOut();
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                }
            }
            catch
            {
                //ModelState.AddModelError(String.Empty, "Error inesperado");
            }

            if (ModelState.IsValid)
            {
                usuarioTO = model.IsValid(model.Usuario, model.Password, model.Empresa, this.Request.UserHostAddress);
                if (usuarioTO.Valid)
                {
                    FormsAuthentication.SetAuthCookie(usuarioTO.usuario.usuario, false);
                    usuarioTO.SessionId = System.Web.HttpContext.Current.Session.SessionID;
                    if (!Cache.DiccionarioUsuariosLogueados.ContainsKey(usuarioTO.usuario.usuario))
                    {
                        Cache.DiccionarioUsuariosLogueados.Add(usuarioTO.usuario.usuario, usuarioTO);
                    }
                    else
                    {
                        TempData["HasExpiredSession"] = true;
                        Cache.DiccionarioUsuariosLogueados[usuarioTO.usuario.usuario].SessionId = System.Web.HttpContext.Current.Session.SessionID;
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "El usuario o contraseña son incorrectos.");
                }
                var empresas = (from config in ((Hashtable)ConfigurationManager.GetSection("Empresas")).Cast<DictionaryEntry>()
                                select new
                                {
                                    key = config.Key,
                                    value = config.Value
                                }).ToList();
                ViewBag.Empresa = new SelectList(empresas, "value", "key");
                return View(model);
            }


            return View(model);

        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            //    var result = await UserManager.CreateAsync(user, model.Password);
            //    if (result.Succeeded)
            //    {
            //        await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
            //        // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
            //        // Enviar correo electrónico con este vínculo
            //        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //        // await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");

            //        return RedirectToAction("Index", "Home");
            //    }
            //    AddErrors(result);
            //}

            //// Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            //return View(model);
            return null;
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Cache.DiccionarioUsuariosLogueados.Remove(User.Identity.GetUserName());
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult GetImage(int id)
        {
            var archivo_empleado = db.Archivo_Empleado.Where(e => !e.eliminado && e.id_empleado == id && e.Archivo.id_tipo_archivo == (int)Catalogos.Tipo_Archivo.Foto_Perfil);
            foreach (var file in archivo_empleado)
            {
                string nombre = file.Archivo.nombre;
                nombre = nombre.Substring(0, nombre.LastIndexOf('.'));
                string[] imagen = nombre.Split('_');
                if (imagen.Length == 2)
                {
                    int id_imagen;
                    bool conversion = int.TryParse(imagen[1], out id_imagen);
                    if (conversion)
                    {
                        if (id_imagen == id)
                        {
                            var image_byte = System.IO.File.ReadAllBytes(Server.MapPath(file.Archivo.ubicacion));
                            string extension = file.Archivo.ubicacion.Substring(file.Archivo.ubicacion.LastIndexOf('.') + 1);

                            return File(image_byte, "image/" + extension);
                        }
                    }
                }
            }
            //Retornar imagen default si el empleado no tiene ninguna en el sistema
            var image_default = System.IO.File.ReadAllBytes(Server.MapPath(ruta_imagen_default));
            string ext = ruta_imagen_default.Substring(ruta_imagen_default.LastIndexOf('.') + 1);
            //return File(image_default, "image/" + ext);
            
            /////////////////
            string path = ruta_imagen_default; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "myimage.png");
            FileStream stream = new FileStream(path, FileMode.Open);
            FileStreamResult result = new FileStreamResult(stream, "image/jpg");
            result.FileDownloadName = "image.jpg";
            return result;
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}