using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CotizacionesWeb.Application.Roles;
using CotizacionesWeb.UI.Models;

namespace CotizacionesWeb.UI.Controllers;

[Authorize(Roles = "Admin,Administrador")]
public class RolesController : Controller
{
    private readonly IRolService _rolService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRolService rolService, ILogger<RolesController> logger)
    {
        _rolService = rolService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _rolService.GetAllAsync();
        return View(roles);
    }

    public IActionResult Create()
    {
        return View(new RolCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RolCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new CreateRolRequest(
                model.Nombre,
                model.Descripcion,
                model.Activo
            );

            await _rolService.CreateAsync(request);
            TempData["Success"] = "Rol creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear rol");
            ModelState.AddModelError("", "Error al crear el rol: " + ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var rol = await _rolService.GetByIdAsync(id);
        if (rol == null)
        {
            TempData["Error"] = "Rol no encontrado";
            return RedirectToAction(nameof(Index));
        }

        var model = new RolEditViewModel
        {
            Id = rol.Id,
            Nombre = rol.Nombre,
            Descripcion = rol.Descripcion,
            Activo = rol.Activo
        };

        return PartialView("_EditModal", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RolEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Datos inválidos" });
        }

        try
        {
            var request = new UpdateRolRequest(
                model.Nombre,
                model.Descripcion,
                model.Activo
            );

            await _rolService.UpdateAsync(model.Id, request);
            return Json(new { success = true, message = "Rol actualizado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar rol");
            return Json(new { success = false, message = "Error al actualizar el rol: " + ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _rolService.DeleteAsync(id);
            return Json(new { success = true, message = "Rol eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar rol");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPermisos(int id)
    {
        var rol = await _rolService.GetByIdAsync(id);
        if (rol == null)
        {
            return NotFound();
        }

        var permisosAsignados = await _rolService.GetRolPermisosAsync(id);
        var todosPermisos = await _rolService.GetAllPermisosAsync();

        var model = new RolPermisosViewModel
        {
            RolId = id,
            RolNombre = rol.Nombre,
            PermisosDisponibles = todosPermisos.Select(p => new PermisoItemViewModel
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Categoria = p.Categoria,
                Descripcion = p.Descripcion
            }).ToList(),
            PermisosAsignados = permisosAsignados.Select(p => p.Id).ToList()
        };

        return PartialView("_PermisosModal", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePermisos(int rolId, List<int> permisosIds)
    {
        try
        {
            permisosIds ??= new List<int>();
            await _rolService.UpdateRolPermisosAsync(rolId, permisosIds);
            return Json(new { success = true, message = "Permisos actualizados exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar permisos");
            return Json(new { success = false, message = "Error al actualizar los permisos: " + ex.Message });
        }
    }
}
