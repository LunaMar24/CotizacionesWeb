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
    console.log('=== INICIALIZACIÓN EDITAR.JS ===');
    console.log('jQuery disponible:', typeof $ !== 'undefined');
    console.log('FormatConfig disponible:', typeof window.FormatConfig !== 'undefined');
    console.log('FormatUtils disponible:', typeof window.FormatUtils !== 'undefined');
    
    if (window.FormatConfig) {
        console.log('FormatConfig.estados:', window.FormatConfig.estados);
        console.log('FormatConfig.defaults:', window.FormatConfig.defaults);
    }
    
    // Esperar a que jQuery y otros componentes estén listos
    if (typeof $ === 'undefined') {
        console.error('jQuery no está disponible');
        return;
    }
    
    // Verificar que AdminLTE esté disponible
    if (typeof $.fn.CardWidget === 'undefined') {
        console.warn('AdminLTE CardWidget no está disponible');
    }
    
    inicializarVista();
    configurarEventos();
    cargarDatosTemporales();
    
    console.log('Editar.js cargado correctamente');
    console.log('=== FIN INICIALIZACIÓN EDITAR.JS ===');
});

function inicializarVista() {
// Obtener configuración desde el servidor (FormatHelper)
if (window.FormatConfig) {
    monedaActual = window.FormatConfig.moneda || 'CRC';
    estadoActual = window.FormatConfig.estado || 'B';
} else {
    // Fallback al método anterior
    monedaActual = $('#formEditarCotizacion').find('input[name="Moneda"]').val() || 'CRC';
        
    // Obtener estado actual de la cotización desde el badge en el header
    const estadoBadge = $('.header-title .badge').text().trim();
    estadoActual = detectarEstadoDeTexto(estadoBadge);
}
    
    // Verificar si es editable usando la configuración centralizada
    const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
        window.FormatUtils.isEditable(estadoActual) : 
        (estadoActual === 'B'); // Fallback
    
    // DEBUG: Log detallado para depuración
    console.log('=== DEBUG ESTADO EDITABLE ===');
    console.log('estadoActual:', estadoActual);
    console.log('window.FormatUtils disponible:', typeof window.FormatUtils !== 'undefined');
    if (window.FormatUtils) {
        console.log('FormatUtils.isEditable(estadoActual):', window.FormatUtils.isEditable(estadoActual));
    }
    console.log('esEditable (resultado final):', esEditable);
    console.log('================================');
    
    // Inicializar componentes AdminLTE solo si están disponibles
    if (typeof $.fn.CardWidget !== 'undefined') {
        try {
            $('[data-card-widget="collapse"]').CardWidget();
        } catch (e) {
            console.warn('Error inicializando CardWidget:', e);
        }
    }
    
    // Configurar cards colapsables
    $('.card-header[data-card-widget="collapse"]').on('click', function(e) {
        if (!$(e.target).closest('.btn').length) {
            $(this).find('.btn[data-card-widget="collapse"]').click();
        }
    });
    
    // Contador de caracteres para notas (solo si es editable)
    if (esEditable) {
        configurarContadorCaracteres();
    }
    
    // Mostrar aviso si no es editable
    if (!esEditable) {
        mostrarAvisoNoEditable();
    }
    
    console.log('Vista de edición inicializada correctamente', {
        estado: estadoActual,
        estadoTexto: (typeof window.FormatUtils !== 'undefined') ? 
            window.FormatUtils.getEstadoTexto(estadoActual) : 
            'Estado ' + estadoActual,
        editable: esEditable,
        moneda: monedaActual,
        configuracionServidor: !!window.FormatConfig,
        formatUtilsDisponible: typeof window.FormatUtils !== 'undefined'
    });
}

/**
 * Detecta el estado a partir del texto del badge
 */
function detectarEstadoDeTexto(texto) {
    // Si tenemos FormatConfig disponible, usarlo
    if (typeof window.FormatConfig !== 'undefined' && window.FormatConfig.estados) {
        const estados = window.FormatConfig.estados;
        for (let [codigo, config] of Object.entries(estados)) {
            if (config.texto === texto) {
                return codigo;
            }
        }
    }
    
    // Fallback: mapeo manual
    const mapeosEstado = {
        'Borrador': 'B',
        'Pendiente Aprobación': 'P',
        'Aprobada': 'A',
        'Enviada': 'E',
        'Aceptada': 'T',
        'Rechazada': 'R',
        'Archivada': 'X'
    };
    
    return mapeosEstado[texto] || 'B'; // Default a Borrador
}

function configurarEventos() {
    // Solo configurar eventos de edición si está en estado editable
    const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
        window.FormatUtils.isEditable(estadoActual) : 
        (estadoActual === 'B'); // Fallback
    
    // DEBUG: Log para configurarEventos
    console.log('=== DEBUG CONFIGURAR EVENTOS ===');
    console.log('estadoActual en configurarEventos:', estadoActual);
    console.log('esEditable en configurarEventos:', esEditable);
    console.log('=================================');
    
    if (esEditable) {
        // Guardar cambios
        $('#btnGuardar').on('click', guardarCotizacion);
        
        // Agregar nueva línea
        $('#btnAgregarLinea').on('click', function() {
            abrirModalDetalle(-1); // -1 indica nueva línea
        });
        
        // Editar línea existente
        $(document).on('click', '.btn-editar-detalle', function() {
            const index = parseInt($(this).attr('data-index'));
            abrirModalDetalle(index);
        });
        
        // Eliminar línea
        $(document).on('click', '.btn-eliminar-detalle', function() {
            const index = parseInt($(this).attr('data-index'));
            eliminarDetalle(index);
        });
        
        // Guardar cambios en modal de detalle
        $('#btnGuardarDetalle').on('click', guardarDetalle);
        
        // Cálculos automáticos en modal
        $('#modalCantidad, #modalPrecioUnitario, #modalDescuento').on('input', calcularTotalLinea);
        
        // Cambio de producto
        $('#modalProductoId').on('change', function() {
            const productoId = $(this).val();
            const producto = productosDisponibles.find(p => p.id === productoId);
            if (producto) {
                $('#modalProductoNombre').val(producto.nombre);
                $('#modalPrecioUnitario').val(producto.precio);
                calcularTotalLinea();
            }
        });
        
        // Cambio de interesado
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
        
        // También configurar evento para select2 si está inicializado
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
        
        // Limpiar modal al cerrar
        $('#modalEditarDetalle').on('hidden.bs.modal', limpiarModalDetalle);
    } else {
        // Mostrar mensaje si intenta interactuar con elementos no editables
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
    
    // Solo cargar datos en selects si es editable
    if (estadoActual === 'B') {
        // Cargar productos en select
        const $selectProducto = $('#modalProductoId');
        $selectProducto.empty().append('<option value="">Seleccione un producto...</option>');
        
        productosDisponibles.forEach(producto => {
            $selectProducto.append(
                `<option value="${producto.id}" data-precio="${producto.precio}">
                    ${producto.id} - ${producto.nombre}
                </option>`
            );
        });
        
        // Cargar interesados en select
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
        
        // Configurar Select2 para búsqueda solo si es editable y existe
        if ($('#selectInteresado').length && !$('#selectInteresado').prop('disabled')) {
            // Destruir Select2 existente si existe
            if ($('#selectInteresado').hasClass('select2-hidden-accessible')) {
                $('#selectInteresado').select2('destroy');
            }
            
            // Verificar que Select2 esté disponible
            if (typeof $.fn.select2 !== 'undefined') {
                $('#selectInteresado').select2({
                    placeholder: 'Busque por nombre o empresa...',
                    allowClear: true,
                    width: '100%'
                });
            }
        }
    }
    
    console.log('Datos temporales cargados:', {
        productos: productosDisponibles.length,
        interesados: interesadosDisponibles.length,
        editable: estadoActual === 'B'
    });
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
    
    // Recalcular totales
    recalcularTotales();
    
    // Cerrar modal
    $('#modalEditarDetalle').modal('hide');
    
    showNotification('success', 'Detalle guardado correctamente');
}

function actualizarFilaDetalle(index, datos) {
    const fila = $(`tr[data-index="${index}"]`);
    
    // Actualizar valores visibles
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
    
    // Actualizar campos ocultos - IMPORTANTE: mantener valores numéricos precisos
    fila.find('input[name$=".ProductoId"]').val(datos.productoId);
    fila.find('input[name$=".ProductoNombre"]').val(datos.productoNombre);
    fila.find('input[name$=".Cantidad"]').val(datos.cantidad.toString());
    fila.find('input[name$=".PrecioUnitario"]').val(datos.precioUnitario.toString());
    fila.find('input[name$=".Descuento"]').val(datos.descuento.toString());
    fila.find('input[name$=".TotalLinea"]').val(datos.totalLinea.toString());
    
    console.log(`Fila ${index} actualizada:`, datos);
}

function agregarNuevaFilaDetalle(datos) {
    const tbody = $('#tablaDetalles tbody');
    const nuevoIndex = tbody.find('tr').length;
    
    const nuevaFila = `
        <tr data-detalle-id="${datos.detalleVersionId}" data-index="${nuevoIndex}">
            <td>
                <strong class="text-primary">${datos.productoId}</strong>
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
                <button type="button" class="btn btn-warning btn-xs btn-editar-detalle" 
                        data-index="${nuevoIndex}" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
                <button type="button" class="btn btn-danger btn-xs ml-1 btn-eliminar-detalle" 
                        data-index="${nuevoIndex}" title="Eliminar">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `;
    
    tbody.append(nuevaFila);
    actualizarContadorLineas();
    
    console.log(`Nueva fila agregada:`, datos);
}

function eliminarDetalle(index) {
    // Verificar si tenemos la función global de modal disponible
    if (typeof window.mostrarModalConfirmacion === 'function') {
        window.mostrarModalConfirmacion(
            'Eliminar Detalle',
            '¿Está seguro de que desea eliminar esta línea de detalle?',
            'danger',
            function() {
                ejecutarEliminacionDetalle(index);
            }
        );
    } else {
        // Fallback con confirm() del navegador
        if (confirm('¿Está seguro de que desea eliminar esta línea de detalle?')) {
            ejecutarEliminacionDetalle(index);
        }
    }
}

function ejecutarEliminacionDetalle(index) {
    const fila = $(`tr[data-index="${index}"]`);
    fila.remove();
    
    // Reindexar filas
    reindexarFilasDetalle();
    
    // Recalcular totales
    recalcularTotales();
    
    showNotification('success', 'Detalle eliminado correctamente');
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
        
        // Subtotal sin descuentos (cantidad * precio)
        const subtotalLinea = cantidad * precio;
        subtotal += subtotalLinea;
        
        // Acumular descuentos
        totalDescuentos += descuento;
    });
    
    // Subtotal después de descuentos
    const subtotalConDescuentos = subtotal - totalDescuentos;
    
    // Calcular impuesto sobre el subtotal con descuentos (13%)
    const impuesto = subtotalConDescuentos * 0.13;
    const total = subtotalConDescuentos + impuesto;
    
    // Actualizar display
    $('#displaySubTotal').text(formatCurrency(subtotal)); // Subtotal sin descuentos
    $('#displayDescuento').text(formatCurrency(totalDescuentos)); // Total de descuentos
    $('#displaySubtotalDescontado').text(formatCurrency(subtotalConDescuentos)); // Subtotal después de descuentos
    $('#displayImpuesto').text(formatCurrency(impuesto));
    $('#displayTotal').text(formatCurrency(total));
    
    // Agregar efecto de actualización
    $('.financial-summary').addClass('updated');
    setTimeout(() => {
        $('.financial-summary').removeClass('updated');
    }, 500);
    
    console.log('Totales recalculados:', {
        subtotalBruto: subtotal,
        descuentosTotal: totalDescuentos,
        subtotalDescontado: subtotalConDescuentos,
        impuesto: impuesto,
        totalFinal: total
    });
}

function calcularTotalLinea() {
    const cantidad = parseFloat($('#modalCantidad').val()) || 0;
    const precio = parseFloat($('#modalPrecioUnitario').val()) || 0;
    const descuento = parseFloat($('#modalDescuento').val()) || 0;
    
    // Total de línea = (cantidad * precio) - descuento
    const total = Math.max(0, (cantidad * precio) - descuento);
    $('#modalTotalLinea').text(formatCurrency(total));
    
    // Mostrar desglose si hay descuento
    if (descuento > 0) {
        const subtotalLinea = cantidad * precio;
        console.log(`Línea: Subtotal=${formatCurrency(subtotalLinea)}, Descuento=${formatCurrency(descuento)}, Total=${formatCurrency(total)}`);
    }
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
    
    // Limpiar validaciones
    $('#formEditarDetalle .is-invalid').removeClass('is-invalid');
    $('#formEditarDetalle .invalid-feedback').remove();
}

function actualizarContadorLineas() {
    const cantidad = $('#tablaDetalles tbody tr').length;
    $('#contadorLineas').text(`${cantidad} item(s)`);
}

function configurarContadorCaracteres() {
    const textarea = $('textarea[name="Notas"]');
    const maxLength = parseInt(textarea.attr('maxlength')) || 2000;
    
    textarea.on('input', function() {
        const current = $(this).val().length;
        const remaining = maxLength - current;
        const small = $(this).siblings('small');
        
        let mensaje = `${remaining} caracteres restantes`;
        let clase = '';
        
        if (remaining < 100) {
            clase = 'warning';
            mensaje = `⚠️ ${mensaje}`;
        }
        if (remaining < 50) {
            clase = 'danger';
            mensaje = `🚨 ${mensaje}`;
        }
        
        small.removeClass('text-muted warning danger').addClass(clase || 'text-muted');
        small.text(mensaje);
    });
}

function guardarCotizacion() {
    // Solo permitir guardar si está en estado Borrador
    if (estadoActual !== 'B') {
        showNotification('error', 'No se puede guardar. Solo las cotizaciones en estado Borrador pueden ser editadas.');
        return;
    }
    
    const form = $('#formEditarCotizacion');
    const btn = $('#btnGuardar');
    
    // Validar que hay datos básicos
    const nombreInteresado = $('#NombreInteresado').val().trim();
    const emailInteresado = $('#EmailInteresado').val().trim();
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    if (!nombreInteresado) {
        showNotification('error', 'El nombre del interesado es obligatorio');
        $('#NombreInteresado').focus();
        return;
    }
    
    if (!emailInteresado) {
        showNotification('error', 'El email del interesado es obligatorio');
        $('#EmailInteresado').focus();
        return;
    }
    
    if (totalLineas === 0) {
        showNotification('error', 'Debe agregar al menos una línea de detalle');
        return;
    }
    
    // Deshabilitar botón
    btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Guardando...');
    
    try {
        // Serializar datos del formulario
        const formData = form.serialize();
        
        // TODO: Implementar guardado real - por ahora simulamos
        console.log('Datos a guardar:', formData);
        
        // Simular guardado
        setTimeout(() => {
            try {
                showNotification('success', 'Cotización guardada correctamente');
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
                
                // Opcional: Redirigir al listado
                // window.location.href = '/Cotizaciones';
            } catch (error) {
                console.error('Error en guardado simulado:', error);
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
                showNotification('error', 'Error al guardar la cotización');
            }
        }, 1500);
        
    } catch (error) {
        console.error('Error al procesar guardado:', error);
        btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
        showNotification('error', 'Error al procesar los datos de la cotización');
    }
}

function mostrarAvisoNoEditable() {
    console.log('Cotización no editable - Estado:', estadoActual);
    
    // Deshabilitar todos los controles
    $('.form-control:not([readonly]):not([disabled])').prop('readonly', true);
    $('.btn-success, .btn-warning, .btn-danger').prop('disabled', true);
    
    // Cambiar apariencia visual
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
    
    // Fallback local - Usar el mismo diccionario que el FormatHelper de C#
    const symbols = {
        'CRC': '¢',     // Colón costarricense
        'USD': '$',     // Dólar estadounidense 
        'DOL': '$',     // Dólar (alias)
        'EUR': '€',     // Euro
        'MXN': '$',     // Peso mexicano
        'CAD': '$',     // Dólar canadiense
        'GBP': '£',     // Libra esterlina
        'JPY': '¥',     // Yen japonés
        'CNY': '¥'      // Yuan chino
    };
    
    if (!currency) return '₡'; // Default a colón costarricense
    
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

function mostrarModalConfirmacion(titulo, mensaje, tipo, onConfirm) {
    // Verificar si existe función global diferente a esta
    if (typeof window.mostrarModalConfirmacion === 'function' && 
        window.mostrarModalConfirmacion !== mostrarModalConfirmacion &&
        arguments.callee !== window.mostrarModalConfirmacion) {
        try {
            window.mostrarModalConfirmacion(titulo, mensaje, tipo, onConfirm);
        } catch (error) {
            // Fallback si hay error
            console.warn('Error con modal global, usando confirm nativo');
            if (confirm(`${titulo}\n\n${mensaje}`)) {
                onConfirm();
            }
        }
    } else {
        // Fallback con confirm() del navegador
        console.warn('Usando confirm nativo - función global no disponible');
        if (confirm(`${titulo}\n\n${mensaje}`)) {
            onConfirm();
        }
    }
}

function showNotification(type, message) {
    // Verificar si existe la función global y NO es esta misma función
    if (typeof window.showNotification === 'function' && 
        window.showNotification !== showNotification && 
        arguments.callee !== window.showNotification) {
        try {
            window.showNotification(type, message);
        } catch (error) {
            // Fallback si hay error
            console.log(`[${type.toUpperCase()}] ${message}`);
        }
    } else {
        // Fallback simple - usar console o alert
        const prefix = `[${type.toUpperCase()}]`;
        console.log(`${prefix} ${message}`);
        
        // Solo mostrar alert para errores críticos
        if (type === 'error') {
            alert(`Error: ${message}`);
        }
    }
}