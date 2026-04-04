using Microsoft.AspNetCore.Razor.TagHelpers;
using CotizacionesWeb.UI.Services;

namespace CotizacionesWeb.UI.TagHelpers;

[HtmlTargetElement(Attributes = "requiere-permiso")]
public class RequierePermisoTagHelper : TagHelper
{
    private readonly IPermisoChecker _permisoChecker;

    [HtmlAttributeName("requiere-permiso")]
    public string CodigoPermiso { get; set; } = string.Empty;

    [HtmlAttributeName("tipo-restriccion")]
    public string TipoRestriccion { get; set; } = "deshabilitar"; // "deshabilitar" u "ocultar"

    public RequierePermisoTagHelper(IPermisoChecker permisoChecker)
    {
        _permisoChecker = permisoChecker;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrEmpty(CodigoPermiso))
            return;

        var tienePermiso = await _permisoChecker.TienePermisoAsync(CodigoPermiso);

        if (!tienePermiso)
        {
            if (TipoRestriccion.ToLower() == "ocultar")
            {
                output.SuppressOutput();
            }
            else // deshabilitar
            {
                output.Attributes.SetAttribute("disabled", "disabled");
                output.Attributes.SetAttribute("title", "No tiene permisos para esta acción");
                
                // Si es un enlace, remover el href
                if (output.TagName == "a")
                {
                    output.Attributes.RemoveAll("href");
                    output.Attributes.SetAttribute("style", "cursor: not-allowed; opacity: 0.5; pointer-events: none;");
                }
                else if (output.TagName == "button")
                {
                    // CAMBIO: No modificar el estilo de botones, solo deshabilitarlos
                    // Para que mantengan su apariencia visual original con colores
                    output.Attributes.SetAttribute("style", "cursor: not-allowed; opacity: 0.7;");
                }
            }
        }
    }
}
