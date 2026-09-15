using SECADPLUS.Models.Extendidas;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SECADPLUS.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Base de datos")]
        public string Conexion { get; set; }

        [Display(Name = "Lista de Conexiones")]
        public List<System.Web.Mvc.SelectListItem> ListaConexiones { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contrase&ntilde;a")]
        public string Password { get; set; }

        [Display(Name = "¿Recordarme?")]
        public bool RememberMe { get; set; }

        public string ConexionSeleccionada { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        public string Empleado { get; set; }
        public bool Estado { get; set; }
        public decimal Sitio { get; set; }
        public List<Empleado> Empleados { get; set; }

        [Display(Name = "Sitios de grabación")]
        public System.Web.Mvc.SelectList ListaSitios { get; set; }

        public decimal CanalFuerza { get; set; }
        public decimal CanalCodigo { get; set; }

        [Display(Name = "Sitios de grabación")]
        public List<System.Web.Mvc.SelectListItem> ListaCanales { get; set; }

        public string Condicion { get; set; }
        public System.Web.Mvc.SelectList Departamentos { get; set; }
        public System.Web.Mvc.SelectList Ciudades { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class MenuModel
    {
        public string Nombre { get; set; }
        public string Ruta { get; set; }
    }

    public class UsuarioModel
    {
        public List<ExtUsuarioSecad> Usuarios { get; set; }
    }

    public class LoginUsuarioDTO
    {
        [Required]
        [StringLength(30, ErrorMessage = "Debe tener entre 5 y 30 caracteres.", MinimumLength = 5)]
        public string Usuario { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Debe tener entre 5 y 50 caracteres.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}