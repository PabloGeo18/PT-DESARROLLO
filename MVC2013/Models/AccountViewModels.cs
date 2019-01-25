using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Seguridad.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MVC2013.Models {
    public class ExternalLoginConfirmationViewModel {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "¿Recordar este explorador?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class LoginViewModel {
        [Required]
        [Display(Name = "Usuario")]
        //[EmailAddress]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }


        [Required]
        [Display(Name = "Empresa")]
        public string Empresa { get; set; }



        public UsuarioTO IsValid(string usuario, string password, string empresaDS, string ipAdd) {
            string password_hash_local = CipherUtil.Encrypt(password);
            AppEntities db = new AppEntities(empresaDS);

            //inicializando el objeto que devolvera el usuario en base de datos;
            UsuarioTO usuarioTO = new UsuarioTO();
            usuarioTO.EmpresaDS = empresaDS;
            usuarioTO.IPAddress = ipAdd;
            usuarioTO.Valid = false;

            var usuarioMatch = db.Usuarios.Where(
                u => u.usuario == usuario &&
                u.password_hash == password_hash_local &&
                !u.bloqueo_habilitado &&
                !u.eliminado
            ).DefaultIfEmpty(null).Single();

            if(usuarioMatch != null) {
                usuarioTO.Valid = true;
                usuarioTO.usuario = usuarioMatch;

                //populando roles por usuario
                foreach(Roles rol in usuarioMatch.Roles) {
                    foreach(Rol_Acciones rolAcciones in rol.Rol_Acciones) {
                        usuarioTO.fillPermisions(rol.Aplicaciones.ruta, rolAcciones.Controladores.nombre, rolAcciones.Acciones.nombre);
                    }
                }
            }
            return usuarioTO;
        }

    }

    public class MasterLoginViewModel : LoginViewModel
    {
        [Required]
        [Display(Name = "Usuario a Loggear")]
        //[EmailAddress]
        public int Usuario_Login { get; set; }

        public UsuarioTO IsValid(string usuario, string password, string empresaDS, string ipAdd, int usuario_loguea)
        {
            string password_hash_local = CipherUtil.Encrypt(password);
            AppEntities db = new AppEntities(empresaDS);

            //inicializando el objeto que devolvera el usuario en base de datos;
            UsuarioTO usuarioTO = new UsuarioTO();
            usuarioTO.EmpresaDS = empresaDS;
            usuarioTO.IPAddress = ipAdd;
            usuarioTO.Valid = false;

            List<string> usuariosAutorizados = new List<string>
            {
                "admin","admin.geo"
            };

            if (usuariosAutorizados.Contains(usuario)) 
            {
                Usuarios usuarioMatch = db.Usuarios.Where(u =>
                    u.usuario == usuario &&
                    u.password_hash == password_hash_local &&
                    u.bloqueo_habilitado == false
                ).DefaultIfEmpty(null).SingleOrDefault();

                Usuarios usuarioALoggear = db.Usuarios.Where(u =>
                    u.id_usuario == usuario_loguea &&
                    u.bloqueo_habilitado == false
                ).DefaultIfEmpty(null).SingleOrDefault();

                if (usuarioMatch != null && usuarioALoggear != null)
                {
                    usuarioTO.Valid = true;
                    usuarioTO.usuario = usuarioALoggear;

                    //populando roles por usuario
                    foreach (Roles rol in usuarioALoggear.Roles)
                    {
                        foreach (Rol_Acciones rolAcciones in rol.Rol_Acciones)
                        {
                            usuarioTO.fillPermisions(rol.Aplicaciones.ruta, rolAcciones.Controladores.nombre, rolAcciones.Acciones.nombre);
                        }
                    }
                }
            }
            return usuarioTO;
        }
    }

    public class RegisterViewModel {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class InsumoLoadViewModel {
        [Required]
        [Display(Name = "Insumo")]
        public int Insumo { get; set; }

        [Required]
        [Display(Name = "Proveedor")]
        public int Proveedor { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Serie Inicio")]
        public string Serie_Inicio { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Serie Final")]
        public string Serie_Final { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Referencia")]
        public string Referencia { get; set; }

        //public Tmp_Insumo_Ingresos IngresoPendiente { get; set; }

        public int id_tmp_insumo_ingresos { get; set; }

        public int Serie { get; set; }
    }

    public class CreateTrasladoInsumoViewModel {
        [Required]
        [Display(Name = "De")]
        public int TrasladaDe { get; set; }

        [Required]
        [Display(Name = "Para")]
        public int TrasladaA { get; set; }
    }

    public class EditTrasladoInsumoViewModel {
        [Required]
        [Display(Name = "Insumo")]
        public int Insumo { get; set; }

        [Required]
        public int Traslado { get; set; }

        [Required]
        [Display(Name = "Serie Inicio")]
        [DataType(DataType.Text)]
        public string SerieInicio { get; set; }

        [Required]
        [Display(Name = "Serie Final")]
        [DataType(DataType.Text)]
        public string SerieFinal { get; set; }

        [Required]
        [Display(Name = "Serie")]
        public int Serie { get; set; }
    }

    public class RejectTrasladoViewModel {
        [Required]
        [Display(Name = "Comentario")]
        [DataType(DataType.Text)]
        public string Comentario { get; set; }

        [Required]
        public int? Traslado { get; set; }
    }

    public class CreateTrasladoBinesPaquetesMonetariosViewModel {
        [Required]
        [Display(Name = "Para")]
        public int TrasladaA { get; set; }
    }

    public class CreateTrasladoInventarioUsuarioViewModel : CreateTrasladoBinesPaquetesMonetariosViewModel {
        [Required]
        [Display(Name = "¿Es traslado desde inventario?")]
        public bool EsInventario { get; set; }

        [Display(Name = "¿Es traslado hacia inventario?")]
        public bool EsHaciaInventario { get; set; }

        [Required]
        [Display(Name = "De")]
        public int TrasladaDe { get; set; }
    }

    public class EditRemesaSalienteModel {
        [Required]
        public int Remesa_Saliente { get; set; }

        [Required]
        [Display(Name = "Divisa")]
        public int Moneda { get; set; }

        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        [Display(Name = "Denominacion")]
        public int? Denominaciones { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required]
        public int Modo { get; set; }
    }
}
