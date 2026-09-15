using Oracle.ManagedDataAccess.Client;
using SECADPLUS.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class RolController : Controller
    {
        //private ApplicationDbContext context = new ApplicationDbContext(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
        //OracleConnection conexion = new OracleConnection(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
        private ApplicationDbContext context = new ApplicationDbContext(new OracleConnection(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString)));
        private GestorAuditoria auditoria = new GestorAuditoria();


        // GET: /Roles/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"].ToUpper()
                });
                context.SaveChanges();
                return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", collection["RoleName"]) });
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2146233087)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                }
                else
                {
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                }
                return RedirectToAction("Index", "Rol");
            }
        }

        [SecadPlusAutorizador("/Rol/Index")]
        public ActionResult Index(string strMensaje = null)
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }
                var roles = context.Roles.ToList();
                return View(roles);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Roles/Edit/5
        [Authorize]
        public ActionResult Edit(string roleName)
        {
            try
            {
                var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                return View(thisRole);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                role.Name = role.Name.ToUpper();
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                auditoria.RegistrarAuditoria<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(role, "IdentityRole", "Id=" + role.Id.ToString() , TipoAuditoria.Modificar);

                return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", role.Name) });
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2146233087)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                }
                else
                {
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                }
                return RedirectToAction("Index", "Rol");
            }
        }

        // GET: /Roles/Edit/5
        [Authorize]
        public ActionResult Eliminar(string roleName)
        {
            try
            {
                var rol = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                return View("Delete", null, rol);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string RoleName)
        {
            try
            {
                var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                context.Roles.Remove(thisRole);
                context.SaveChanges();


                Microsoft.AspNet.Identity.EntityFramework.IdentityRole thisRole_copy = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                thisRole_copy = (Microsoft.AspNet.Identity.EntityFramework.IdentityRole)auditoria.GenerarCopiaAuditoria(thisRole);

                auditoria.RegistrarAuditoria<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(thisRole_copy, "IdentityRole", "Id=" + thisRole.Id.ToString(), TipoAuditoria.Eliminar);



                return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", RoleName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}