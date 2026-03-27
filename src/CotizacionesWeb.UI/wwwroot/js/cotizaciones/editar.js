// ========================================
// COTIZACIONES EDITAR - JavaScript
// ========================================

// Variables globales
let productosDisponibles = [];
let interesadosDisponibles = [];
let monedaActual = 'CRC';
let detalleEditandoIndex = -1;
let estadoActual = 'B'; // Estado actual de la cotización

// Datos temporales (simulando integración con ERP y HubSpot)
const PRODUCTOS_TEMP = [
    { id: 'PROD001', nombre: 'Laptop Dell Inspiron 15', precio: 450000 },
    { id: 'PROD002', nombre: 'Monitor Samsung 24" Full HD', precio: 125000 },
    { id: 'PROD003', nombre: 'Teclado Mecánico Logitech', precio: 35000 },
    { id: 'PROD004', nombre: 'Mouse Wireless HP', precio: 15000 },
    { id: 'PROD005', nombre: 'Impresora HP LaserJet Pro', precio: 185000 },
    { id: 'PROD006', nombre: 'Router WiFi TP-Link AC1200', precio: 45000 },
    { id: 'PROD007', nombre: 'Disco Duro Externo 1TB', precio: 55000 },
    { id: 'SERV001', nombre: 'Servicio de Instalación', precio: 25000 },
    { id: 'SERV002', nombre: 'Soporte Técnico Mensual', precio: 40000 },
    { id: 'SERV003', nombre: 'Configuración de Red', precio: 75000 }
];

const INTERESADOS_TEMP = [
    { 
        id: 1, 
        nombre: 'Juan Carlos Rodríguez', 
        email: 'juan.rodriguez@email.com', 
        empresa: 'Tecnología Avanzada S.A.', 
        tipo: 'P' 
    },
    { 
        id: 2, 
        nombre: 'María José Fernández', 
        email: 'maria.fernandez@empresa.com', 
        empresa: 'Soluciones Empresariales Ltda.', 
        tipo: 'P' 
    },
    { 
        id: 3, 
        nombre: 'Carlos Alberto Méndez', 
        email: 'carlos.mendez@corporativo.co.cr', 
        empresa: 'Corporativo Internacional', 
        tipo: 'E' 
    },
    { 
        id: 4, 
        nombre: 'Ana Lucía Vargas', 
        email: 'ana.vargas@consultora.com', 
        empresa: 'Consultora Estratégica', 
        tipo: 'P' 
    },
    { 
        id: 5, 
        nombre: 'Roberto Silva', 
        email: 'roberto.silva@innovacion.cr', 
        empresa: 'Innovación y Desarrollo S.A.', 
        tipo: 'E' 
    }
];

$(document).ready(function() {
    if (typeof $ === 'undefined') {
        console.error('jQuery no está disponible');
        return;
    }
    
    if (typeof $.fn.CardWidget === 'undefined') {
        console.warn('AdminLTE CardWidget no está disponible');
    }
    
    $(window).on('beforeunload', function(e) {
        if (verificarCambiosSinGuardar()) {
            const mensaje = '¿Está seguro de que desea salir? Se perderán los cambios no guardados.';
            e.returnValue = mensaje;
            return mensaje;
        }
    });
    
    $(document).on('click', 'a[href]:not(.btn-guardar):not([data-toggle])', function(e) {
        const href = $(this).attr('href');
        
        if (href && href !== '#' && !href.startsWith('#') && href !== window.location.href) {
            if (verificarCambiosSinGuardar()) {
                e.preventDefault();
                confirmarSalidaConCambios();
                return false;
            }
        }
    });
    
    $('a[href*="/Cotizaciones"]:contains("Volver al Listado"), a[href="/Cotizaciones"], a[href$="/Cotizaciones/Index"]').on('click', function(e) {
        if (verificarCambiosSinGuardar()) {
            e.preventDefault();
            confirmarSalidaConCambios();
            return false;
        }
    });
    
    $('#NombreInteresado, #EmailInteresado, #EmpresaInteresado, textarea[name="Notas"]').on('input', function() {
        window.cotizacionGuardada = false;
    });
    
    $(document).on('click', '#btnAgregarLinea, .btn-editar-detalle, .btn-eliminar-detalle', function() {
        window.cotizacionGuardada = false;
    });
    
    inicializarVista();
    configurarEventos();
    cargarDatosTemporales();
});

function inicializarVista() {
    if (window.FormatConfig) {
        monedaActual = window.FormatConfig.moneda || 'CRC';
        estadoActual = window.FormatConfig.estado || 'B';
    } else {
        monedaActual = $('#formEditarCotizacion').find('input[name="Moneda"]').val() || 'CRC';
        const estadoBadge = $('.header-title .badge').text().trim();
        estadoActual = detectarEstadoDeTexto(estadoBadge);
    }
            
    const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
        window.FormatUtils.isEditable(estadoActual) : 
        (estadoActual === 'B');
    
    if (typeof $.fn.CardWidget !== 'undefined') {
        try {
            $('[data-card-widget="collapse"]').CardWidget();
        } catch (e) {
            console.warn('Error inicializando CardWidget:', e);
        }
    }
    
    $('.card-header[data-card-widget="collapse"]').on('click', function(e) {
        if (!$(e.target).closest('.btn').length) {
            $(this).find('.btn[data-card-widget="collapse"]').click();
        }
    });
    
    setTimeout(function() {
        configurarContadorCaracteres();
    }, 100);
    
    if (!esEditable) {
        mostrarAvisoNoEditable();
    }
    
    setTimeout(function() {
        verificarYActualizarEstadoMoneda();
    }, 200);
}

/**
 * Detecta el estado a partir del texto del badge
 */
function detectarEstadoDeTexto(texto) {
    if (typeof window.FormatConfig !== 'undefined' && window.FormatConfig.estados) {
        const estados = window.FormatConfig.estados;
        
        for (let [codigo, config] of Object.entries(estados)) {
            if (config.texto === texto) {
                return codigo;
            }
        }
    }
    
    const mapeosEstado = {
        'Borrador': 'B',
        'Pendiente Aprobación': 'P',
        'Aprobada': 'A',
        'Enviada': 'E',
        'Aceptada': 'T',
        'Rechazada': 'R',
        'Archivada': 'X'
    };
    
    if (mapeosEstado[texto]) {
        return mapeosEstado[texto];
    }
    
    const textoLower = texto.toLowerCase();
    for (let [textoEstado, codigo] of Object.entries(mapeosEstado)) {
        if (textoEstado.toLowerCase() === textoLower) {
            return codigo;
        }
    }
    
    return 'B';
}

function configurarEventos() {
    const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
        window.FormatUtils.isEditable(estadoActual) : 
        (estadoActual === 'B');
    
    if (esEditable) {
        $('#btnGuardar').on('click', guardarCotizacion);
        configurarEventoTipoCambio();
        configurarEventoVersion();
        
        $('#btnAgregarLinea').on('click', function() {
            abrirModalDetalle(-1);
        });
        
        $(document).on('click', '.btn-editar-detalle', function() {
            const index = parseInt($(this).attr('data-index'));
            abrirModalDetalle(index);
        });
        
        $(document).on('click', '.btn-eliminar-detalle', function() {
            const index = parseInt($(this).attr('data-index'));
            eliminarDetalle(index);
        });
        
        $('#btnGuardarDetalle').on('click', guardarDetalle);
        $('#modalCantidad, #modalPrecioUnitario, #modalDescuento').on('input', calcularTotalLinea);
        
        $('#modalProductoId').on('change', function() {
            const productoId = $(this).val();
            const producto = productosDisponibles.find(p => p.id === productoId);
            if (producto) {
                $('#modalProductoNombre').val(producto.nombre);
                $('#modalPrecioUnitario').val(producto.precio);
                calcularTotalLinea();
            }
        });
        
        $('#selectInteresado').on('change', function() {
            const interesadoId = parseInt($(this).val());
            const interesado = interesadosDisponibles.find(i => i.id === interesadoId);
            if (interesado) {
                $('#NombreInteresado').val(interesado.nombre);
                $('#EmailInteresado').val(interesado.email);
                $('#EmpresaInteresado').val(interesado.empresa);
                $('#TipoInteresado').val(interesado.tipo);
            }
        });
        
        $(document).on('select2:select', '#selectInteresado', function() {
            const interesadoId = parseInt($(this).val());
            const interesado = interesadosDisponibles.find(i => i.id === interesadoId);
            if (interesado) {
                $('#NombreInteresado').val(interesado.nombre);
                $('#EmailInteresado').val(interesado.email);
                $('#EmpresaInteresado').val(interesado.empresa);
                $('#TipoInteresado').val(interesado.tipo);
            }
        });
        
        $('#modalEditarDetalle').on('hidden.bs.modal', limpiarModalDetalle);
    } else {
        $('.form-control, .btn-success, .btn-warning, .btn-danger').on('click', function(e) {
            if ($(this).is(':disabled, [readonly]')) {
                const estadoTexto = (typeof window.FormatUtils !== 'undefined') ? 
                    window.FormatUtils.getEstadoTexto(estadoActual) : 
                    'Estado ' + estadoActual;
                showNotification('warning', `Esta cotización no puede ser editada. Solo las cotizaciones en estado Borrador son editables. Estado actual: ${estadoTexto}`);
                e.preventDefault();
            }
        });
    }
}

function cargarDatosTemporales() {
    productosDisponibles = PRODUCTOS_TEMP;
    interesadosDisponibles = INTERESADOS_TEMP;
    
    const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
        window.FormatUtils.isEditable(estadoActual) : 
        (estadoActual === 'B');
    
    if (esEditable) {
        const $selectProducto = $('#modalProductoId');
        $selectProducto.empty().append('<option value="">Seleccione un producto...</option>');
        
        productosDisponibles.forEach(producto => {
            $selectProducto.append(
                `<option value="${producto.id}" data-precio="${producto.precio}">
                    ${producto.id} - ${producto.nombre}
                </option>`
            );
        });
        
        const $selectInteresado = $('#selectInteresado');
        $selectInteresado.empty().append('<option value="">Seleccione un interesado...</option>');
        
        interesadosDisponibles.forEach(interesado => {
            const tipoTexto = interesado.tipo === 'P' ? 'Persona' : 
                             interesado.tipo === 'E' ? 'Empresa' : 'Otro';
            $selectInteresado.append(
                `<option value="${interesado.id}">
                    ${interesado.nombre} - ${interesado.empresa} (${tipoTexto})
                </option>`
            );
        });
        
        if ($('#selectInteresado').length && !$('#selectInteresado').prop('disabled')) {
            if ($('#selectInteresado').hasClass('select2-hidden-accessible')) {
                $('#selectInteresado').select2('destroy');
            }
            
            if (typeof $.fn.select2 !== 'undefined') {
                $('#selectInteresado').select2({
                    placeholder: 'Busque por nombre o empresa...',
                    allowClear: true,
                    width: '100%'
                });
            }
        }
    }
}

function abrirModalDetalle(index) {
    detalleEditandoIndex = index;
    limpiarModalDetalle();
    
    if (index >= 0) {
        // Modo edición
        const fila = $(`tr[data-index="${index}"]`);
        const detalleId = fila.attr('data-detalle-id');
        
        $('#modalEditarDetalleTitle').html('<i class="fas fa-edit"></i> Editar Detalle');
        $('#detalleIndex').val(index);
        $('#detalleVersionId').val(detalleId);
        
        // Cargar datos actuales
        const productoId = fila.find('input[name$=".ProductoId"]').val();
        const productoNombre = fila.find('input[name$=".ProductoNombre"]').val();
        const cantidad = parseFloat(fila.find('input[name$=".Cantidad"]').val());
        const precio = parseFloat(fila.find('input[name$=".PrecioUnitario"]').val());
        const descuento = parseFloat(fila.find('input[name$=".Descuento"]').val());
        
        // Establecer producto seleccionado ANTES de cambiar el nombre
        $('#modalProductoId').val(productoId);
        
        // Si el producto no está en la lista, agregarlo como opción
        if (productoId && $('#modalProductoId option[value="' + productoId + '"]').length === 0) {
            $('#modalProductoId').append(`<option value="${productoId}">${productoId} - ${productoNombre}</option>`);
            $('#modalProductoId').val(productoId);
        }
        
        $('#modalProductoNombre').val(productoNombre);
        $('#modalCantidad').val(cantidad);
        $('#modalPrecioUnitario').val(precio);
        $('#modalDescuento').val(descuento);
        
        calcularTotalLinea();
    } else {
        // Modo agregar
        $('#modalEditarDetalleTitle').html('<i class="fas fa-plus"></i> Agregar Detalle');
        $('#modalCantidad').val('1');
        $('#modalDescuento').val('0');
    }
    
    $('#modalEditarDetalle').modal('show');
}

function guardarDetalle() {
    // Validar campos requeridos
    const productoId = $('#modalProductoId').val();
    const productoNombre = $('#modalProductoNombre').val().trim();
    const cantidad = parseFloat($('#modalCantidad').val());
    const precio = parseFloat($('#modalPrecioUnitario').val());
    const descuento = parseFloat($('#modalDescuento').val()) || 0;
    
    if (!productoId || !productoNombre || !cantidad || cantidad <= 0 || !precio || precio < 0) {
        showNotification('error', 'Por favor complete todos los campos obligatorios correctamente');
        return;
    }
    
    const totalLinea = (cantidad * precio) - descuento;
    
    if (detalleEditandoIndex >= 0) {
        // Actualizar línea existente
        actualizarFilaDetalle(detalleEditandoIndex, {
            productoId: productoId,
            productoNombre: productoNombre,
            cantidad: cantidad,
            precioUnitario: precio,
            descuento: descuento,
            totalLinea: totalLinea
        });
    } else {
        // Agregar nueva línea
        agregarNuevaFilaDetalle({
            detalleVersionId: 0, // Nuevo registro
            productoId: productoId,
            productoNombre: productoNombre,
            cantidad: cantidad,
            precioUnitario: precio,
            descuento: descuento,
            totalLinea: totalLinea
        });
    }
    
    // Recalcular totales (esto llamará a verificarYActualizarEstadoMoneda)
    recalcularTotales();
    
    // Cerrar modal
    $('#modalEditarDetalle').modal('hide');
    
    showNotification('success', 'Detalle guardado correctamente');
}

function actualizarFilaDetalle(index, datos) {
    const fila = $(`tr[data-index="${index}"]`);
    
    fila.find('.text-primary').text(datos.productoId);
    fila.find('.producto-nombre').text(datos.productoNombre);
    fila.find('.cantidad-display').text(formatNumber(datos.cantidad, 2));
    fila.find('.precio-display').text(formatCurrency(datos.precioUnitario));
    
    if (datos.descuento > 0) {
        fila.find('.descuento-display').removeClass('text-muted').addClass('text-danger')
            .text('-' + formatCurrency(datos.descuento));
    } else {
        fila.find('.descuento-display').removeClass('text-danger').addClass('text-muted').text('-');
    }
    
    fila.find('.total-linea-display').text(formatCurrency(datos.totalLinea));
    
    fila.find('input[name$=".ProductoId"]').val(datos.productoId);
    fila.find('input[name$=".ProductoNombre"]').val(datos.productoNombre);
    fila.find('input[name$=".Cantidad"]').val(datos.cantidad.toString());
    fila.find('input[name$=".PrecioUnitario"]').val(datos.precioUnitario.toString());
    fila.find('input[name$=".Descuento"]').val(datos.descuento.toString());
    fila.find('input[name$=".TotalLinea"]').val(datos.totalLinea.toString());
}

function agregarNuevaFilaDetalle(datos) {
    const tbody = $('#tablaDetalles tbody');
    const nuevoIndex = tbody.find('tr').length;
    
    const esLineaNueva = datos.detalleVersionId === 0;
    const claseIndicador = esLineaNueva ? 'linea-nueva' : 'linea-persistente';
    const iconoEstado = esLineaNueva ? 
        '<i class="fas fa-plus-circle text-success" title="Línea nueva (no guardada)"></i>' : 
        '<i class="fas fa-database text-info" title="Línea persistente (guardada en BD)"></i>';
    
    const nuevaFila = `
        <tr data-detalle-id="${datos.detalleVersionId}" data-index="${nuevoIndex}" class="${claseIndicador}">
            <td>
                <div class="d-flex align-items-center">
                    <strong class="text-primary">${datos.productoId}</strong>
                    <span class="ml-2">${iconoEstado}</span>
                </div>
                <input type="hidden" name="Detalles[${nuevoIndex}].DetalleVersionId" value="${datos.detalleVersionId}" />
                <input type="hidden" name="Detalles[${nuevoIndex}].ProductoId" value="${datos.productoId}" />
            </td>
            <td>
                <span class="producto-nombre">${datos.productoNombre}</span>
                <input type="hidden" name="Detalles[${nuevoIndex}].ProductoNombre" value="${datos.productoNombre}" />
            </td>
            <td class="text-right">
                <span class="cantidad-display">${formatNumber(datos.cantidad, 2)}</span>
                <input type="hidden" name="Detalles[${nuevoIndex}].Cantidad" value="${datos.cantidad.toString()}" />
            </td>
            <td class="text-right">
                <span class="precio-display">${formatCurrency(datos.precioUnitario)}</span>
                <input type="hidden" name="Detalles[${nuevoIndex}].PrecioUnitario" value="${datos.precioUnitario.toString()}" />
            </td>
            <td class="text-right">
                ${datos.descuento > 0 ? 
                    `<span class="text-danger descuento-display">-${formatCurrency(datos.descuento)}</span>` :
                    `<span class="text-muted descuento-display">-</span>`
                }
                <input type="hidden" name="Detalles[${nuevoIndex}].Descuento" value="${datos.descuento.toString()}" />
            </td>
            <td class="text-right">
                <strong class="total-linea-display">${formatCurrency(datos.totalLinea)}</strong>
                <input type="hidden" name="Detalles[${nuevoIndex}].TotalLinea" value="${datos.totalLinea.toString()}" />
            </td>
            <td class="text-center">
                <div class="btn-actions-group">
                    <button type="button" class="btn btn-warning btn-xs btn-editar-detalle" 
                            data-index="${nuevoIndex}" title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button type="button" class="btn btn-danger btn-xs btn-eliminar-detalle" 
                            data-index="${nuevoIndex}" title="Eliminar">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `;
    
    tbody.append(nuevaFila);
    actualizarContadorLineas();
    verificarYActualizarEstadoMoneda();
}

function eliminarDetalle(index) {
    // Usar la nueva función global mejorada de modal
    mostrarModalConfirmacion(
        'Eliminar Detalle',
        '¿Está seguro de que desea eliminar esta línea de detalle?<br><br><small class="text-muted">Esta acción no se puede deshacer.</small>',
        'danger',
        function() {
            ejecutarEliminacionDetalle(index);
        }
    );
}

function ejecutarEliminacionDetalle(index) {
    const fila = $(`tr[data-index="${index}"]`);
    const detalleVersionId = parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) || 0;
    const productoNombre = fila.find('.producto-nombre').text();
    
    if (detalleVersionId > 0) {
        showNotification('info', `Línea persistente "${productoNombre}" marcada para eliminación. Se eliminará de BD al guardar.`);
    }
    
    fila.remove();
    reindexarFilasDetalle();
    recalcularTotales();
    
    showNotification('success', 'Detalle eliminado correctamente');
    
    if (detalleVersionId > 0) {
        let lineasPersistentesRestantes = 0;
        $('#tablaDetalles tbody tr').each(function() {
            const dvId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
            if (dvId > 0) {
                lineasPersistentesRestantes++;
            }
        });
        
        if (lineasPersistentesRestantes === 0) {
            showNotification('success', 'Ya no hay líneas persistentes. Ahora puede cambiar la moneda si lo desea.');
            setTimeout(() => {
                verificarYActualizarEstadoMoneda();
            }, 1000);
        }
    }
}

function reindexarFilasDetalle() {
    $('#tablaDetalles tbody tr').each(function(index) {
        const fila = $(this);
        
        // Actualizar atributos data-index
        fila.attr('data-index', index);
        
        // Actualizar botones
        fila.find('.btn-editar-detalle').attr('data-index', index);
        fila.find('.btn-eliminar-detalle').attr('data-index', index);
        
        // Actualizar nombres de inputs
        fila.find('input[type="hidden"]').each(function() {
            const input = $(this);
            const name = input.attr('name');
            if (name && name.includes('Detalles[')) {
                const newName = name.replace(/Detalles\[\d+\]/, `Detalles[${index}]`);
                input.attr('name', newName);
            }
        });
    });
    
    actualizarContadorLineas();
}

function recalcularTotales() {
    let subtotal = 0;
    let totalDescuentos = 0;
    
    $('#tablaDetalles tbody tr').each(function() {
        const cantidad = parseFloat($(this).find('input[name$=".Cantidad"]').val()) || 0;
        const precio = parseFloat($(this).find('input[name$=".PrecioUnitario"]').val()) || 0;
        const descuento = parseFloat($(this).find('input[name$=".Descuento"]').val()) || 0;
        
        const subtotalLinea = cantidad * precio;
        subtotal += subtotalLinea;
        totalDescuentos += descuento;
    });
    
    const subtotalConDescuentos = subtotal - totalDescuentos;
    const impuesto = subtotalConDescuentos * 0.13;
    const total = subtotalConDescuentos + impuesto;
    
    $('#displaySubTotal').text(formatCurrency(subtotal));
    $('#displayDescuento').text(formatCurrency(totalDescuentos));
    $('#displaySubtotalDescontado').text(formatCurrency(subtotalConDescuentos));
    $('#displayImpuesto').text(formatCurrency(impuesto));
    $('#displayTotal').text(formatCurrency(total));
    
    $('.financial-summary').addClass('updated');
    setTimeout(() => {
        $('.financial-summary').removeClass('updated');
    }, 500);
    
    verificarYActualizarEstadoMoneda();
}

function calcularTotalLinea() {
    const cantidad = parseFloat($('#modalCantidad').val()) || 0;
    const precio = parseFloat($('#modalPrecioUnitario').val()) || 0;
    const descuento = parseFloat($('#modalDescuento').val()) || 0;
    
    const total = Math.max(0, (cantidad * precio) - descuento);
    $('#modalTotalLinea').text(formatCurrency(total));
}

function limpiarModalDetalle() {
    $('#formEditarDetalle')[0].reset();
    $('#modalProductoId').val('');
    $('#modalProductoNombre').val('');
    $('#modalCantidad').val('1');
    $('#modalPrecioUnitario').val('');
    $('#modalDescuento').val('0');
    $('#modalTotalLinea').text(formatCurrency(0));
    $('#detalleIndex').val('');
    $('#detalleVersionId').val('');
    $('#formEditarDetalle .is-invalid').removeClass('is-invalid');
    $('#formEditarDetalle .invalid-feedback').remove();
}

function actualizarContadorLineas() {
    const totalLineas = $('#tablaDetalles tbody tr').length;
    let lineasPersistentes = 0;
    let lineasTemporales = 0;
    
    $('#tablaDetalles tbody tr').each(function() {
        const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
        if (detalleVersionId > 0) {
            lineasPersistentes++;
        } else {
            lineasTemporales++;
        }
    });
    
    let textoContador = '';
    if (totalLineas === 0) {
        textoContador = '0 líneas';
    } else {
        const partes = [];
        if (lineasPersistentes > 0) {
            partes.push(`${lineasPersistentes} guardada${lineasPersistentes > 1 ? 's' : ''}`);
        }
        if (lineasTemporales > 0) {
            partes.push(`${lineasTemporales} nueva${lineasTemporales > 1 ? 's' : ''}`);
        }
        textoContador = partes.join(', ') + ` (${totalLineas} total)`;
    }
    
    $('#contadorLineas').html(textoContador);
}

function configurarContadorCaracteres() {
    const textarea = $('textarea[name="Notas"]');
    const maxLength = parseInt(textarea.attr('maxlength')) || 2000;
    
    function actualizarContador() {
        const current = textarea.val().length;
        const remaining = maxLength - current;
        let small = textarea.siblings('small');
        
        if (small.length === 0) {
            small = $('<small class="text-muted"></small>');
            textarea.after(small);
        }
        
        let mensaje = `${remaining} caracteres restantes`;
        let clase = 'text-muted';
        
        if (remaining < 100) {
            clase = 'text-warning';
        }
        if (remaining < 50) {
            clase = 'text-danger';
        }
        
        small.removeClass('text-muted text-warning text-danger warning danger').addClass(clase);
        small.text(mensaje);
    }
    
    actualizarContador();
    textarea.on('input', actualizarContador);
}

function guardarCotizacion() {
    if (estadoActual !== 'B') {
        showNotification('error', 'No se puede guardar. Solo las cotizaciones en estado Borrador pueden ser editadas.');
        return;
    }
    
    ejecutarGuardadoCotizacion();
}

function ejecutarGuardadoCotizacion() {
    const btn = $('#btnGuardar');
    const nombreInteresado = $('#NombreInteresado').val().trim();
    const emailInteresado = $('#EmailInteresado').val().trim();
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    const erroresValidacion = [];
    
    if (!nombreInteresado) {
        erroresValidacion.push('• El nombre del interesado es obligatorio');
    }
    
    if (!emailInteresado) {
        erroresValidacion.push('• El email del interesado es obligatorio');
    }
    
    if (totalLineas === 0) {
        mostrarModalConfirmacion(
            'Cotización Sin Productos',
            '¿Está seguro de que desea guardar la cotización sin líneas de productos?<br><br>' +
            '<small class="text-muted">La cotización se guardará como borrador y podrá agregar productos más tarde.</small>',
            'warning',
            function() {
                continuarGuardadoSinValidacionLineas();
            }
        );
        return;
    }
    
    if (erroresValidacion.length > 0) {
        const mensajeError = '<strong>No se puede guardar por los siguientes errores:</strong><br><br>' +
                           erroresValidacion.join('<br>') +
                           '<br><br><small class="text-muted">Por favor corrija estos campos y vuelva a intentar.</small>';
        
        mostrarModalConfirmacion(
            'Errores de Validación',
            mensajeError,
            'danger',
            function() {
                if (erroresValidacion.some(e => e.includes('nombre'))) {
                    $('#NombreInteresado').focus();
                } else if (erroresValidacion.some(e => e.includes('email'))) {
                    $('#EmailInteresado').focus();
                }
            }
        );
        return;
    }
    
    continuarGuardadoSinValidacionLineas();
}

function continuarGuardadoSinValidacionLineas() {
    const btn = $('#btnGuardar');
    btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Guardando...');
    
    try {
        const formData = {
            CotizacionId: $('input[name="CotizacionId"]').val(),
            VersionId: parseInt($('input[name="VersionId"]').val()),
            NombreInteresado: $('#NombreInteresado').val().trim(),
            EmailInteresado: $('#EmailInteresado').val().trim(),
            EmpresaInteresado: $('#EmpresaInteresado').val().trim(),
            TipoInteresado: $('#TipoInteresado').val(),
            Notas: $('textarea[name="Notas"]').val().trim(),
            Detalles: [],
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };
        
        $('#tablaDetalles tbody tr').each(function(index) {
            const fila = $(this);
            const inputDetalleId = fila.find('input[name$=".DetalleVersionId"]');
            const inputProductoId = fila.find('input[name$=".ProductoId"]');
            const inputProductoNombre = fila.find('input[name$=".ProductoNombre"]');
            const inputCantidad = fila.find('input[name$=".Cantidad"]');
            const inputPrecio = fila.find('input[name$=".PrecioUnitario"]');
            const inputDescuento = fila.find('input[name$=".Descuento"]');
            const inputTotal = fila.find('input[name$=".TotalLinea"]');
            
            const detalle = {
                DetalleVersionId: parseInt(inputDetalleId.val() || fila.find(`input[name="Detalles[${index}].DetalleVersionId"]`).val()) || 0,
                ProductoId: inputProductoId.val() || fila.find(`input[name="Detalles[${index}].ProductoId"]`).val() || '',
                ProductoNombre: inputProductoNombre.val() || fila.find(`input[name="Detalles[${index}].ProductoNombre"]`).val() || '',
                Cantidad: parseFloat(inputCantidad.val() || fila.find(`input[name="Detalles[${index}].Cantidad"]`).val() || 0),
                PrecioUnitario: parseFloat(inputPrecio.val() || fila.find(`input[name="Detalles[${index}].PrecioUnitario"]`).val() || 0),
                Descuento: parseFloat(inputDescuento.val() || fila.find(`input[name="Detalles[${index}].Descuento"]`).val() || 0),
                TotalLinea: parseFloat(inputTotal.val() || fila.find(`input[name="Detalles[${index}].TotalLinea"]`).val() || 0)
            };
            
            if (detalle.ProductoId && detalle.ProductoId !== '') {
                formData.Detalles.push(detalle);
            }
        });
        
        if (formData.Detalles.length === 0) {
            formData.Detalles = [];
        }
        
        const numeroVersionActual = $('#NumeroVersion').val() || $('#versionValor').text().replace('v', '');
        if (numeroVersionActual) {
            formData.NumeroVersion = parseFloat(numeroVersionActual);
        }
        
        const monedaSeleccionada = $('#MonedaSelect').val() || monedaActual;
        formData.Moneda = monedaSeleccionada;
        
        const tipoCambioIngresado = $('#TipoCambio').val();
        if (tipoCambioIngresado && !isNaN(parseFloat(tipoCambioIngresado))) {
            formData.TipoCambio = parseFloat(tipoCambioIngresado);
        } else {
            formData.TipoCambio = null;
        }
        
        formData.SubTotal = calcularSubtotalActual();
        formData.TotalDescuentos = calcularTotalDescuentos();
        formData.Impuesto = calcularImpuestoActual();
        formData.Total = calcularTotalFinalActual();
        
        $.ajax({
            url: '/Cotizaciones/GuardarEdicion',
            type: 'POST',
            data: formData,
            success: function(response) {
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
                
                if (response.success) {
                    showNotification('success', response.message || 'Cotización guardada exitosamente');
                    marcarComoGuardado();
                    
                    setTimeout(function() {
                        window.location.reload(true);
                    }, 2000);
                } else {
                    showNotification('error', response.message || 'Error al guardar la cotización');
                }
            },
            error: function(xhr, status, error) {
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
                
                let errorMessage = 'Error de comunicación con el servidor';
                
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                } else if (xhr.status === 403) {
                    errorMessage = 'No tiene permisos para realizar esta operación';
                } else if (xhr.status === 404) {
                    errorMessage = 'Cotización no encontrada';
                } else if (xhr.status >= 500) {
                    errorMessage = 'Error interno del servidor';
                }
                
                showNotification('error', errorMessage);
            }
        });
        
    } catch (error) {
        btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
        showNotification('error', 'Error al procesar los datos de la cotización');
    }
}

function mostrarAvisoNoEditable() {
    $('.form-control:not([readonly]):not([disabled])').prop('readonly', true);
    $('.btn-success, .btn-warning, .btn-danger').prop('disabled', true);
    $('.form-control[readonly]').addClass('readonly-field');
}

// Funciones de utilidad
function formatCurrency(value) {
    // Usar función centralizada si está disponible
    if (typeof window.FormatUtils !== 'undefined') {
        return window.FormatUtils.formatCurrency(value, monedaActual);
    }
    
    // Fallback local
    if (isNaN(value)) {
        const symbol = getCurrencySymbol(monedaActual);
        return symbol + '0.00';
    }
    
    const symbol = getCurrencySymbol(monedaActual);
    const formattedNumber = parseFloat(value).toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
    
    // IMPORTANTE: No usar template strings para evitar problemas de encoding
    return symbol + formattedNumber;
}

function getCurrencySymbol(currency) {
    // Usar función centralizada si está disponible
    if (typeof window.FormatUtils !== 'undefined') {
        return window.FormatUtils.getCurrencySymbol(currency);
    }
    
    // Fallback local - Usar códigos Unicode para evitar problemas de encoding
    const symbols = {
        'CRC': '\u00A2',    // Colón costarricense (¢ - Unicode: U+00A2)
        'USD': '$',         // Dólar estadounidense 
        'DOL': '$',         // Dólar (alias)
        'EUR': '\u20AC',    // Euro (Unicode: U+20AC)
        'MXN': '$',         // Peso mexicano
        'CAD': '$',         // Dólar canadiense
        'GBP': '\u00A3',    // Libra esterlina (Unicode: U+00A3)
        'JPY': '\u00A5',    // Yen japonés (Unicode: U+00A5)
        'CNY': '\u00A5'     // Yuan chino (Unicode: U+00A5)
    };
    
    if (!currency) return '\u00A2'; // Default a colón costarricense (¢)
    
    const upperCurrency = currency.toUpperCase().trim();
    return symbols[upperCurrency] || upperCurrency;
}

function formatNumber(value, decimals = 2) {
    // Usar función centralizada si está disponible
    if (typeof window.FormatUtils !== 'undefined') {
        return window.FormatUtils.formatNumber(value, decimals);
    }
    
    // Fallback local
    if (isNaN(value)) return '0.00';
    return parseFloat(value).toFixed(decimals);
}

function mostrarModalConfirmacion(titulo, mensaje, tipo, onConfirm, onCancel = null) {
    if (typeof $ === 'undefined') {
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    if (typeof $.fn.modal !== 'function') {
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    const modal = $('#modalConfirmacion');
    if (modal.length === 0) {
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    const elementos = {
        header: $('#modalConfirmacionHeader'),
        titulo: $('#modalConfirmacionTitulo'),
        mensaje: $('#modalConfirmacionMensaje'),
        btnConfirmar: $('#btnConfirmarAccion')
    };
    
    const elementosFaltantes = Object.keys(elementos).filter(key => elementos[key].length === 0);
    if (elementosFaltantes.length > 0) {
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    modal.off();
    elementos.btnConfirmar.off();
    $('.modal-backdrop').remove();
    $('body').removeClass('modal-open').css({ 'overflow': '', 'padding-right': '' });
    
    let accionConfirmada = false;
    let modalCerrandose = false;
    
    const config = obtenerConfiguracionModal(tipo); // Ya no necesita parámetro adicional porque está en la función
    elementos.header.removeClass('bg-info bg-warning bg-danger bg-success text-white text-dark').addClass(config.headerClass);
    elementos.titulo.html(`<i class="fas ${config.icono}"></i> ${titulo}`);
    elementos.mensaje.html(mensaje);
    elementos.btnConfirmar.removeClass('btn-info btn-warning btn-danger btn-success btn-primary').addClass(config.btnClass);
    elementos.btnConfirmar.html(`<i class="fas fa-check"></i> ${config.btnTexto}`);
    
    elementos.btnConfirmar.on('click.modalconfirm', function(e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        
        if (modalCerrandose) {
            return;
        }
        
        accionConfirmada = true;
        modalCerrandose = true;
        modal.modal('hide');
        
        setTimeout(() => {
            if (typeof onConfirm === 'function') {
                try {
                    onConfirm();
                } catch (error) {
                    console.error('Error en callback confirmación:', error);
                }
            }
        }, 200);
    });
    
    modal.on('hidden.bs.modal.confirm', function() {
        if (!accionConfirmada && !modalCerrandose && typeof onCancel === 'function') {
            setTimeout(() => {
                try {
                    onCancel();
                } catch (error) {
                    console.error('Error en callback cancelación:', error);
                }
            }, 100);
        }
        
        $(this).off('.confirm');
        elementos.btnConfirmar.off('.modalconfirm');
    });
    
    try {
        modal.modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
        
        setTimeout(() => {
            if (!modal.hasClass('show')) {
                modal.off();
                elementos.btnConfirmar.off();
                usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
            }
        }, 500);
        
    } catch (error) {
        console.error('Error mostrando modal:', error);
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
    }
}

function usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel) {
    const mensajeTexto = mensaje.replace(/<[^>]*>/g, '').replace(/\s+/g, ' ').trim();
    const textoCompleto = `${titulo}\n\n${mensajeTexto}`;
    
    setTimeout(() => {
        const confirmacion = confirm(textoCompleto);
        
        if (confirmacion && typeof onConfirm === 'function') {
            onConfirm();
        } else if (!confirmacion && typeof onCancel === 'function') {
            onCancel();
        }
    }, 10);
}

function obtenerConfiguracionModal(tipo, btnTextoPersonalizado = null) {
    const configuraciones = {
        'info': { headerClass: 'bg-info text-white', icono: 'fa-info-circle', btnClass: 'btn-info', btnTexto: 'Aceptar' },
        'warning': { headerClass: 'bg-warning text-dark', icono: 'fa-exclamation-triangle', btnClass: 'btn-warning', btnTexto: 'Continuar' },
        'danger': { headerClass: 'bg-danger text-white', icono: 'fa-exclamation-circle', btnClass: 'btn-danger', btnTexto: 'Eliminar' },
        'success': { headerClass: 'bg-success text-white', icono: 'fa-check-circle', btnClass: 'btn-success', btnTexto: 'Aceptar' },
        'exit': { headerClass: 'bg-warning text-dark', icono: 'fa-sign-out-alt', btnClass: 'btn-warning', btnTexto: 'Regresar' }
    };
    
    const config = configuraciones[tipo] || configuraciones['info'];
    
    // Si se proporciona texto personalizado, usarlo
    if (btnTextoPersonalizado) {
        config.btnTexto = btnTextoPersonalizado;
    }
    
    return config;
}

function showNotification(type, message) {
    if (typeof window.showNotification === 'function' && 
        window.showNotification !== showNotification && 
        arguments.callee !== window.showNotification) {
        try {
            window.showNotification(type, message);
        } catch (error) {
            console.log(`[${type.toUpperCase()}] ${message}`);
        }
    } else {
        console.log(`[${type.toUpperCase()}] ${message}`);
    }
}

function revertirComboMoneda($combo, monedaAnterior) {
    $combo.off('change.moneda');
    $combo.val(monedaAnterior);
    
    if ($combo.val() !== monedaAnterior) {
        const opcionAnterior = $combo.find(`option[value="${monedaAnterior}"]`);
        if (opcionAnterior.length > 0) {
            $combo[0].selectedIndex = opcionAnterior.index();
        }
    }
}

function mostrarModalConfirmacionMoneda(titulo, mensaje, tipo, onConfirm, onCancel, $combo, monedaAnterior) {
    window._monedaCambioConfirmado = false;
    window._monedaCambioOnConfirm = onConfirm;
    window._monedaCambioOnCancel = onCancel;
    
    const modal = $('#modalConfirmacion');
    const btnConfirmar = $('#btnConfirmarAccion');
    
    modal.off('.moneda');
    btnConfirmar.off('.moneda');
    
    const elementos = {
        header: $('#modalConfirmacionHeader'),
        titulo: $('#modalConfirmacionTitulo'),
        mensaje: $('#modalConfirmacionMensaje'),
        btnConfirmar: btnConfirmar
    };
    
    const config = obtenerConfiguracionModal(tipo);
    elementos.header.removeClass('bg-info bg-warning bg-danger bg-success text-white text-dark').addClass(config.headerClass);
    elementos.titulo.html(`<i class="fas ${config.icono}"></i> ${titulo}`);
    elementos.mensaje.html(mensaje);
    elementos.btnConfirmar.removeClass('btn-info btn-warning btn-danger btn-success btn-primary').addClass(config.btnClass);
    elementos.btnConfirmar.html(`<i class="fas fa-check"></i> ${config.btnTexto}`);
    
    btnConfirmar.on('click.moneda', function(e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        window._monedaCambioConfirmado = true;
        modal.modal('hide');
    });
    
    modal.on('hidden.bs.modal.moneda', function() {
        if (window._monedaCambioConfirmado) {
            setTimeout(() => {
                if (typeof window._monedaCambioOnConfirm === 'function') {
                    try {
                        window._monedaCambioOnConfirm();
                    } catch (error) {
                        console.error('Error en callback de confirmación:', error);
                    }
                }
                
                window._monedaCambioConfirmado = false;
                window._monedaCambioOnConfirm = null;
                window._monedaCambioOnCancel = null;
            }, 100);
            
        } else {
            setTimeout(() => {
                if (typeof window._monedaCambioOnCancel === 'function') {
                    try {
                        window._monedaCambioOnCancel();
                    } catch (error) {
                        console.error('Error en callback de cancelación:', error);
                    }
                }
                
                window._monedaCambioConfirmado = false;
                window._monedaCambioOnConfirm = null;
                window._monedaCambioOnCancel = null;
            }, 100);
        }
        
        modal.off('.moneda');
        btnConfirmar.off('.moneda');
    });
    
    modal.modal({
        backdrop: 'static',
        keyboard: false,
        show: true
    });
}

function calcularSubtotalActual() {
    let subtotal = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const cantidad = parseFloat($(this).find('input[name$=".Cantidad"]').val()) || 0;
        const precio = parseFloat($(this).find('input[name$=".PrecioUnitario"]').val()) || 0;
        subtotal += (cantidad * precio);
    });
    return subtotal;
}

function calcularTotalDescuentos() {
    let totalDescuentos = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const descuento = parseFloat($(this).find('input[name$=".Descuento"]').val()) || 0;
        totalDescuentos += descuento;
    });
    return totalDescuentos;
}

function calcularImpuestoActual() {
    const subtotal = calcularSubtotalActual();
    const descuentos = calcularTotalDescuentos();
    const subtotalConDescuentos = subtotal - descuentos;
    return subtotalConDescuentos * 0.13;
}

function calcularTotalFinalActual() {
    const subtotal = calcularSubtotalActual();
    const descuentos = calcularTotalDescuentos();
    const impuesto = calcularImpuestoActual();
    return (subtotal - descuentos) + impuesto;
}

function confirmarSalidaConCambios() {
    const hayCambios = verificarCambiosSinGuardar();
    
    if (!hayCambios) {
        return true;
    }
    
    mostrarModalConfirmacion(
        'Cambios Sin Guardar',
        '¿Está seguro de que desea salir sin guardar los cambios?<br><br>' +
        '<strong class="text-danger">Se perderán todos los cambios realizados.</strong>',
        'exit', // Usar tipo 'exit' en lugar de 'danger'
        function() {
            window.location.href = '/Cotizaciones';
        }
    );
    
    return false;
}

function verificarCambiosSinGuardar() {
    if (seGuardoRecientemente()) {
        return false;
    }
    
    const nombreActual = $('#NombreInteresado').val().trim();
    const emailActual = $('#EmailInteresado').val().trim();
    const empresaActual = $('#EmpresaInteresado').val().trim();
    const notasActuales = $('textarea[name="Notas"]').val().trim();
    const monedaCombo = $('#MonedaSelect').val();
    const versionEditada = $('#NumeroVersion').val();
    const tipoCambioEditado = $('#TipoCambio').val();
    
    let lineasNuevas = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
        if (detalleVersionId === 0) {
            lineasNuevas++;
        }
    });
    
    const hayCambios = (
        nombreActual.length > 0 ||
        emailActual.length > 0 ||
        empresaActual.length > 0 ||
        notasActuales.length > 0 ||
        lineasNuevas > 0 ||
        (monedaCombo && monedaCombo !== monedaActual) ||
        (versionEditada && versionEditada !== $('#versionValor').text().replace('v', '')) ||
        (tipoCambioEditado && tipoCambioEditado.length > 0)
    );
    
    return hayCambios;
}

function eliminarTodasLasLineasPersistentes() {
    const lineasAEliminar = [];
    
    $('#tablaDetalles tbody tr').each(function() {
        const index = parseInt($(this).attr('data-index'));
        const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
        
        if (detalleVersionId > 0) {
            lineasAEliminar.push(index);
        }
    });
    
    if (lineasAEliminar.length === 0) {
        showNotification('info', 'No hay líneas persistentes que eliminar.');
        return;
    }
    
    lineasAEliminar.reverse().forEach(index => {
        const fila = $(`tr[data-index="${index}"]`);
        fila.remove();
    });
    
    reindexarFilasDetalle();
    recalcularTotales();
    
    showNotification('success', `${lineasAEliminar.length} líneas persistentes eliminadas. Ahora puede cambiar la moneda.`);
    verificarYActualizarEstadoMoneda();
}

function marcarComoGuardado() {
    window.cotizacionGuardada = true;
}

function seGuardoRecientemente() {
    return window.cotizacionGuardada === true;
}

function obtenerNombreMoneda(codigo) {
    if (window.MonedasDisponibles && Array.isArray(window.MonedasDisponibles)) {
        const moneda = window.MonedasDisponibles.find(m => 
            (m.Codigo || m.codigo) === codigo
        );
        if (moneda) {
            return moneda.Nombre || moneda.nombre;
        }
    }
    
    const nombres = {
        'CRC': 'Colón Costarricense',
        'USD': 'Dólar Estadounidense',
        'EUR': 'Euro',
        'MXN': 'Peso Mexicano',
        'CAD': 'Dólar Canadiense',
        'GBP': 'Libra Esterlina',
        'JPY': 'Yen Japonés',
        'CNY': 'Yuan Chino'
    };
    
    return nombres[codigo] || codigo;
}

function aplicarCambioMoneda(nuevaMoneda) {
    const monedaAnterior = monedaActual;
    monedaActual = nuevaMoneda;
    
    if (window.FormatConfig) {
        window.FormatConfig.moneda = nuevaMoneda;
        window.FormatConfig.simboloMoneda = getCurrencySymbol(nuevaMoneda);
    }
    
    const comboMoneda = $('#MonedaSelect');
    if (comboMoneda.length > 0) {
        comboMoneda.off('change.moneda');
        comboMoneda.val(nuevaMoneda);
        comboMoneda[0].selectedIndex = comboMoneda.find(`option[value="${nuevaMoneda}"]`).index();
        
        setTimeout(() => {
            comboMoneda.on('change.moneda', configurarEventoMoneda);
        }, 100);
    }
    
    actualizarDisplaysMoneda();
    window.cotizacionGuardada = false;
    
    showNotification('success', 
        `Moneda cambiada a ${obtenerNombreMoneda(nuevaMoneda)}. ` +
        `Debe GUARDAR la cotización para persistir el cambio.`);
}

function actualizarDisplaysMoneda() {
    const subtotal = 0;
    const descuento = 0;
    const impuesto = 0;
    const total = 0;
    
    $('#displaySubTotal').text(formatCurrency(subtotal));
    $('#displayDescuento').text(formatCurrency(descuento));
    $('#displaySubtotalDescontado').text(formatCurrency(subtotal - descuento));
    $('#displayImpuesto').text(formatCurrency(impuesto));
    $('#displayTotal').text(formatCurrency(total));
    $('#displayMonedaCodigo').text(monedaActual);
    
    $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Moneda:');
    }).html(`<strong>Moneda:</strong> ${monedaActual}`);
    
    if ($('#tablaDetalles tbody tr').length > 0) {
        recalcularTotales();
    }
}

function verificarYActualizarEstadoMoneda() {
    const estadoActual = obtenerEstadoActual();
    const totalLineas = $('#tablaDetalles tbody tr').length;
    const puedeEditarMoneda = (estadoActual === 'B' && totalLineas === 0);
    
    if (puedeEditarMoneda) {
        mostrarComboMoneda();
    } else {
        const razon = totalLineas > 0 ? 'hay-lineas' : 'estado-no-borrador';
        mostrarDisplayMoneda(razon);
    }
    
    if (estadoActual === 'B') {
        configurarEventoTipoCambio();
    }
}

function mostrarComboMoneda() {
    const seccionMoneda = $('.moneda-section');
    if (seccionMoneda.length === 0) {
        return;
    }
    
    const monedaActualReal = obtenerMonedaActual();
    let monedasDisponibles = [];
    
    if (window.MonedasDisponibles && Array.isArray(window.MonedasDisponibles)) {
        monedasDisponibles = window.MonedasDisponibles.map(moneda => {
            return {
                codigo: moneda.Codigo || moneda.codigo,
                simbolo: moneda.Simbolo || moneda.simbolo, 
                nombre: moneda.Nombre || moneda.nombre
            };
        });
    } else {
        monedasDisponibles = [
            { codigo: 'CRC', simbolo: '₡', nombre: 'Colón Costarricense' },
            { codigo: 'USD', simbolo: '$', nombre: 'Dólar Estadounidense' },
            { codigo: 'EUR', simbolo: '€', nombre: 'Euro' },
            { codigo: 'MXN', simbolo: '$', nombre: 'Peso Mexicano' },
            { codigo: 'CAD', simbolo: '$', nombre: 'Dólar Canadiense' },
            { codigo: 'GBP', simbolo: '£', nombre: 'Libra Esterlina' }
        ];
    }
    
    const optionsHTML = monedasDisponibles.map(moneda => {
        const isSelected = monedaActualReal === moneda.codigo;
        return `<option value="${moneda.codigo}" ${isSelected ? 'selected' : ''}>
            ${moneda.simbolo} ${moneda.codigo} - ${moneda.nombre}
        </option>`;
    }).join('');
    
    const comboHTML = `
        <div class="mb-3 p-3 border rounded" style="background-color: #f8f9fa;">
            <h6 class="text-success mb-2">
                <i class="fas fa-dollar-sign"></i> Configuración de Moneda
            </h6>
            <div class="form-group mb-2">
                <label for="MonedaSelect" class="form-label">Moneda de la Cotización:</label>
                <div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text"><i class="fas fa-tags"></i></span>
                    </div>
                    <select class="form-control" id="MonedaSelect" name="Moneda">
                        ${optionsHTML}
                    </select>
                </div>
            </div>
            <small class="text-success">
                <i class="fas fa-check-circle"></i>
                Puede cambiar la moneda: estado Borrador y sin líneas guardadas en BD.
            </small>
        </div>
    `;
    
    seccionMoneda.html(comboHTML);
    
    const comboCreado = $('#MonedaSelect');
    comboCreado.val(monedaActualReal);
    
    if (comboCreado.val() !== monedaActualReal) {
        const opcionCorrecta = comboCreado.find(`option[value="${monedaActualReal}"]`);
        if (opcionCorrecta.length > 0) {
            comboCreado[0].selectedIndex = opcionCorrecta.index();
        }
    }
    
    configurarEventoMoneda();
}

function mostrarDisplayMoneda(razon) {
    const seccionMoneda = $('.moneda-section');
    if (seccionMoneda.length === 0) {
        return;
    }
    
    seccionMoneda.html('');
}

function configurarEventoMoneda() {
    $('#MonedaSelect').off('change.moneda');
    
    $('#MonedaSelect').on('change.moneda', function() {
        const $combo = $(this);
        const nuevaMoneda = $combo.val();
        const monedaAnterior = monedaActual;
        
        if (nuevaMoneda !== monedaAnterior) {
            let lineasPersistentes = 0;
            $('#tablaDetalles tbody tr').each(function() {
                const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
                if (detalleVersionId > 0) {
                    lineasPersistentes++;
                }
            });
            
            if (lineasPersistentes > 0) {
                showNotification('warning', 'No se puede cambiar la moneda cuando hay líneas guardadas en la base de datos. Elimine todas las líneas persistentes primero.');
                revertirComboMoneda($combo, monedaAnterior);
                return;
            }
            
            $combo.off('change.moneda');
            
            const nombreMonedaNueva = obtenerNombreMoneda(nuevaMoneda);
            const nombreMonedaAnterior = obtenerNombreMoneda(monedaAnterior);
            
            mostrarModalConfirmacionMoneda(
                'Confirmar Cambio de Moneda',
                `¿Está seguro de que desea cambiar la moneda de <strong>${nombreMonedaAnterior}</strong> a <strong>${nombreMonedaNueva}</strong>?<br><br>
                 <small class="text-muted">Este cambio es posible porque no hay líneas guardadas en la base de datos.</small>`,
                'warning',
                function() {
                    aplicarCambioMoneda(nuevaMoneda);
                    setTimeout(() => {
                        configurarEventoMoneda();
                    }, 300);
                },
                function() {
                    revertirComboMoneda($combo, monedaAnterior);
                    setTimeout(() => {
                        configurarEventoMoneda();
                    }, 300);
                },
                $combo,
                monedaAnterior
            );
        }
    });
}

function obtenerVersionActual() {
    const versionBadge = $('.badge-version').text().trim();
    if (versionBadge) {
        const match = versionBadge.match(/v?(\d+\.\d+)/);
        if (match) {
            return parseFloat(match[1]);
        }
    }
    
    return 1.0;
}

function obtenerEstadoActual() {
    if (typeof estadoActual !== 'undefined') {
        return estadoActual;
    }
    
    const estadoBadge = $('.header-title .badge').text().trim();
    return detectarEstadoDeTexto(estadoBadge);
}

function obtenerMonedaActual() {
    if (typeof monedaActual !== 'undefined' && monedaActual && monedaActual !== 'undefined') {
        return monedaActual;
    }
    
    if (window.FormatConfig && window.FormatConfig.moneda && window.FormatConfig.moneda !== 'undefined') {
        monedaActual = window.FormatConfig.moneda;
        return monedaActual;
    }
    
    const elementoMonedaCodigo = $('#displayMonedaCodigo');
    if (elementoMonedaCodigo.length > 0) {
        const monedaDesdeCodigo = elementoMonedaCodigo.text().trim();
        if (monedaDesdeCodigo && monedaDesdeCodigo !== 'undefined') {
            monedaActual = monedaDesdeCodigo;
            return monedaActual;
        }
    }
    
    const monedaTexto = $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Moneda:');
    }).text();
    
    if (monedaTexto) {
        const match = monedaTexto.match(/Moneda:\s*[^\w]*(\w+)/);
        if (match && match[1] !== 'undefined') {
            monedaActual = match[1];
            return match[1];
        }
    }
    
    const simbolosMoneda = ['¢', '$', '€', '£', '¥'];
    const elementosConSimbolos = $('.financial-summary *').filter(function() {
        const texto = $(this).text();
        return simbolosMoneda.some(simbolo => texto.includes(simbolo));
    });
    
    if (elementosConSimbolos.length > 0) {
        const textoConSimbolo = elementosConSimbolos.first().text();
        
        if (textoConSimbolo.includes('¢')) {
            monedaActual = 'CRC';
            return 'CRC';
        } else if (textoConSimbolo.includes('$')) {
            monedaActual = 'USD';
            return 'USD';
        } else if (textoConSimbolo.includes('€')) {
            monedaActual = 'EUR';
            return 'EUR';
        }
    }
    
    monedaActual = 'CRC';
    return 'CRC';
}

function obtenerTipoCambioActual() {
    if (window.FormatConfig && window.FormatConfig.tipoCambio && window.FormatConfig.tipoCambio > 0) {
        return parseFloat(window.FormatConfig.tipoCambio);
    }
    
    const tipoCambioTexto = $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Tipo de Cambio:');
    }).text();
    
    if (tipoCambioTexto) {
        const match = tipoCambioTexto.match(/Tipo de Cambio:\s*([0-9]+\.?[0-9]*)/);
        if (match && match[1]) {
            return parseFloat(match[1]);
        }
    }
    
    const inputTipoCambio = $('input[name="TipoCambio"]');
    if (inputTipoCambio.length > 0) {
        const valor = parseFloat(inputTipoCambio.val());
        if (valor && valor > 0) {
            return valor;
        }
    }
    
    return null;
}

function configurarEventoVersion() {
    // Limpiar eventos anteriores
    $('#btnEditarVersion, #btnGuardarVersion, #btnCancelarVersion, #NumeroVersion').off('.version');
    
    let valorOriginal = '';
    
    // Evento para mostrar modo edición
    $('#btnEditarVersion').on('click.version', function() {
        // Guardar valor original (extraer el número de "v1.0")
        const versionTexto = $('#versionValor').text();
        const match = versionTexto.match(/v?(\d+\.\d+)/);
        valorOriginal = match ? match[1] : '1.0';
        
        // Establecer valor en el campo de entrada
        $('#NumeroVersion').val(valorOriginal);
        
        // Cambiar a modo edición
        $('#versionModoVista').addClass('d-none');
        $('#versionModoEdicion').removeClass('d-none');
        
        // Enfocar el campo con un pequeño delay para mejor UX
        setTimeout(function() {
            $('#NumeroVersion').focus().select();
        }, 100);
    });
    
    // Evento para guardar cambios
    $('#btnGuardarVersion').on('click.version', function() {
        guardarCambioVersion();
    });
    
    // Evento para cancelar edición
    $('#btnCancelarVersion').on('click.version', function() {
        cancelarEdicionVersion();
    });
    
    // Evento para guardar con Enter y cancelar con Escape
    $('#NumeroVersion').on('keydown.version', function(e) {
        if (e.which === 13) { // Enter
            e.preventDefault();
            guardarCambioVersion();
        } else if (e.which === 27) { // Escape
            e.preventDefault();
            cancelarEdicionVersion();
        }
    });
    
    // Validación en tiempo real
    $('#NumeroVersion').on('input.version', function() {
        const valor = $(this).val();
        const numero = parseFloat(valor);
        
        if (valor && (isNaN(numero) || numero < 1.0 || numero > 99.9)) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });
    
    function guardarCambioVersion() {
        const nuevoValor = $('#NumeroVersion').val();
        const numero = parseFloat(nuevoValor);
        
        // Validar que no tenga errores
        if ($('#NumeroVersion').hasClass('is-invalid')) {
            showNotification('warning', 'Por favor ingrese un número de versión válido (1.0 - 99.9).');
            $('#NumeroVersion').focus();
            return;
        }
        
        if (!nuevoValor || isNaN(numero) || numero < 1.0 || numero > 99.9) {
            showNotification('warning', 'El número de versión debe estar entre 1.0 y 99.9.');
            $('#NumeroVersion').focus();
            return;
        }
        
        // Formatear el valor correctamente
        const valorFormateado = numero.toFixed(1);
        const displayFormateado = `v${valorFormateado}`;
        
        // Actualizar el campo oculto con el valor correcto para el envío
        $('#NumeroVersion').val(valorFormateado);
        
        // Actualizar display del badge
        $('#versionValor').text(displayFormateado);
        $('#versionModoEdicion').addClass('d-none');
        $('#versionModoVista').removeClass('d-none');
        const valorOriginalNumerico = parseFloat(valorOriginal);
        
        if (numero !== valorOriginalNumerico) {
            showNotification('success', `Versión actualizada a ${displayFormateado}`);
        }
    }
    
    function cancelarEdicionVersion() {
        // Restaurar valor original
        $('#NumeroVersion').val(valorOriginal);
        $('#NumeroVersion').removeClass('is-invalid');
        $('#versionModoEdicion').addClass('d-none');
        $('#versionModoVista').removeClass('d-none');
    }
}
function configurarEventoTipoCambio() {
    $('#btnEditarTipoCambio, #btnGuardarTipoCambio, #btnCancelarTipoCambio, #TipoCambio').off('.tipocambio');
    
    let valorOriginal = '';
    
    $('#btnEditarTipoCambio').on('click.tipocambio', function() {
        valorOriginal = $('#tipoCambioValor').text();
        if (valorOriginal === 'No definido') {
            valorOriginal = '';
        } else {
            const match = valorOriginal.match(/[\d,]+\.?\d*/);
            valorOriginal = match ? match[0].replace(/,/g, '') : '';
        }
        
        $('#TipoCambio').val(valorOriginal);
        $('#tipoCambioModoVista').addClass('d-none');
        $('#tipoCambioModoEdicion').removeClass('d-none');
        
        setTimeout(function() {
            $('#TipoCambio').focus().select();
        }, 100);
    });
    
    $('#btnGuardarTipoCambio').on('click.tipocambio', function() {
        guardarCambioTipoCambio();
    });
    
    $('#btnCancelarTipoCambio').on('click.tipocambio', function() {
        cancelarEdicionTipoCambio();
    });
    
    $('#TipoCambio').on('keydown.tipocambio', function(e) {
        if (e.which === 13) {
            e.preventDefault();
            guardarCambioTipoCambio();
        } else if (e.which === 27) {
            e.preventDefault();
            cancelarEdicionTipoCambio();
        }
    });
    
    $('#TipoCambio').on('input.tipocambio', function() {
        const valor = $(this).val();
        
        if (valor && (isNaN(parseFloat(valor)) || parseFloat(valor) < 0)) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });
    
    function guardarCambioTipoCambio() {
        const nuevoValor = $('#TipoCambio').val();
        let valorFormateado = 'No definido';
        
        if ($('#TipoCambio').hasClass('is-invalid')) {
            showNotification('warning', 'Por favor ingrese un valor válido para el tipo de cambio.');
            $('#TipoCambio').focus();
            return;
        }
        
        if (nuevoValor && !isNaN(parseFloat(nuevoValor)) && parseFloat(nuevoValor) >= 0) {
            const valor = parseFloat(nuevoValor);
            valorFormateado = valor.toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
            $('#TipoCambio').val(valor.toFixed(2));
        } else if (nuevoValor === '' || nuevoValor === '0') {
            $('#TipoCambio').val('');
        } else {
            showNotification('warning', 'Por favor ingrese un valor numérico válido para el tipo de cambio.');
            $('#TipoCambio').focus();
            return;
        }
        
        $('#tipoCambioValor').text(valorFormateado);
        $('#tipoCambioModoEdicion').addClass('d-none');
        $('#tipoCambioModoVista').removeClass('d-none');
        
        const valorNumerico = nuevoValor ? parseFloat(nuevoValor) : 0;
        const valorOriginalNumerico = valorOriginal ? parseFloat(valorOriginal) : 0;
        
        if (valorNumerico !== valorOriginalNumerico) {
            showNotification('success', 'Tipo de cambio actualizado');
        }
    }
    
    function cancelarEdicionTipoCambio() {
        $('#TipoCambio').val(valorOriginal);
        $('#TipoCambio').removeClass('is-invalid');
        $('#tipoCambioModoEdicion').addClass('d-none');
        $('#tipoCambioModoVista').removeClass('d-none');
    }
}
