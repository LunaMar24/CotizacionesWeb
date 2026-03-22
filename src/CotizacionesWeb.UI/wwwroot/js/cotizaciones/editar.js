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
    // Esperar a que jQuery y otros componentes estén listos
    if (typeof $ === 'undefined') {
        console.error('jQuery no está disponible');
        return;
    }
    
    // Verificar que AdminLTE esté disponible
    if (typeof $.fn.CardWidget === 'undefined') {
        console.warn('AdminLTE CardWidget no está disponible');
    }
    
    // DEBUGGING: Verificar que FormatConfig esté disponible
    console.log('=== VERIFICACIÓN INICIAL ===');
    console.log('FormatConfig disponible:', typeof window.FormatConfig !== 'undefined');
    console.log('FormatUtils disponible:', typeof window.FormatUtils !== 'undefined');
    if (window.FormatConfig) {
        console.log('Estados configurados:', window.FormatConfig.estados);
        console.log('Configuración del servidor disponible:', window.FormatConfig.moneda, window.FormatConfig.estado);
    }
    console.log('============================');
    
    inicializarVista();
    configurarEventos();
    cargarDatosTemporales();
    
    // DEBUGGING ADICIONAL: Verificar estado después de la inicialización
    setTimeout(function() {
        console.log('=== VERIFICACIÓN POST-INICIALIZACIÓN ===');
        console.log('Estado actual variable global:', estadoActual);
        console.log('¿Es editable según FormatUtils?:', window.FormatUtils?.isEditable(estadoActual));
        console.log('¿Es editable según lógica directa?:', estadoActual === 'B');
        console.log('Configuración completa del estado:', window.FormatConfig?.estados?.[estadoActual]);
        
        // Verificar si los botones están habilitados/deshabilitados
        const btnGuardar = $('#btnGuardar');
        const btnAgregarLinea = $('#btnAgregarLinea');
        console.log('Botón guardar deshabilitado:', btnGuardar.prop('disabled'));
        console.log('Botón agregar línea longitud:', btnAgregarLinea.length);
        console.log('=========================================');
    }, 1000);
    
    console.log('Editar.js cargado correctamente');
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
    
console.log('=== DETECCIÓN DE ESTADO ===');
console.log('Estado detectado:', estadoActual);
console.log('¿Es editable?:', esEditable);
console.log('FormatUtils disponible:', typeof window.FormatUtils !== 'undefined');
console.log('FormatConfig disponible:', typeof window.FormatConfig !== 'undefined');
if (window.FormatConfig && window.FormatConfig.estados) {
    console.log('Configuración de estados:', window.FormatConfig.estados[estadoActual]);
}
console.log('===========================');
    
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
    
    // Contador de caracteres para notas (siempre mostrar correctamente)
    // Ejecutar con un pequeño retraso para asegurar que el DOM esté listo
    setTimeout(function() {
        configurarContadorCaracteres();
    }, 100);
    
    // Solo habilitar edición si es editable
    if (esEditable) {
        // Ya se configuró el contador arriba
    } else {
        // Mostrar aviso si no es editable
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
// Verificar nuevamente si es editable en configurarEventos
const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
    window.FormatUtils.isEditable(estadoActual) : 
    (estadoActual === 'B'); // Fallback
    
console.log('=== CONFIGURAR EVENTOS ===');
console.log('Estado actual:', estadoActual);
console.log('Es editable:', esEditable);
console.log('==========================');
    
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
    
// Verificar si es editable usando la configuración centralizada
const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
    window.FormatUtils.isEditable(estadoActual) : 
    (estadoActual === 'B'); // Fallback
    
console.log('=== CARGAR DATOS TEMPORALES ===');
console.log('Estado actual:', estadoActual);
console.log('Es editable:', esEditable);
console.log('===============================');
    
// Solo cargar datos en selects si es editable
if (esEditable) {
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
        editable: esEditable
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
    
    // Función para actualizar el contador
    function actualizarContador() {
        const current = textarea.val().length;
        const remaining = maxLength - current;
        let small = textarea.siblings('small');
        
        // Si no existe el elemento small, crearlo
        if (small.length === 0) {
            small = $('<small class="text-muted"></small>');
            textarea.after(small);
        }
        
        let mensaje = `${remaining} caracteres restantes`;
        let clase = 'text-muted';
        
        if (remaining < 100) {
            clase = 'text-warning';
            mensaje = `⚠️ ${mensaje}`;
        }
        if (remaining < 50) {
            clase = 'text-danger';
            mensaje = `🚨 ${mensaje}`;
        }
        
        small.removeClass('text-muted text-warning text-danger warning danger').addClass(clase);
        small.text(mensaje);
        
        console.log(`Contador actualizado: ${current}/${maxLength} caracteres (${remaining} restantes)`);
    }
    
    // Actualizar contador al cargar la página (estado inicial)
    actualizarContador();
    
    // Configurar evento para actualizaciones en tiempo real
    textarea.on('input', actualizarContador);
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

// FUNCIÓN DE DIAGNÓSTICO - Llamar desde consola si hay problemas
window.diagnosticarEstado = function() {
    console.group('🔍 DIAGNÓSTICO COMPLETO DEL ESTADO');
    
    console.log('📊 Variables Globales:');
    console.log('  - estadoActual:', estadoActual);
    console.log('  - monedaActual:', monedaActual);
    
    console.log('🔧 Configuraciones Disponibles:');
    console.log('  - FormatConfig:', typeof window.FormatConfig !== 'undefined');
    console.log('  - FormatUtils:', typeof window.FormatUtils !== 'undefined');
    
    if (window.FormatConfig) {
        console.log('📋 FormatConfig.estados:');
        console.table(window.FormatConfig.estados);
        console.log('📋 Configuración del servidor:');
        console.log('  - moneda:', window.FormatConfig.moneda);
        console.log('  - estado:', window.FormatConfig.estado);
    }
    
    console.log('✅ Verificaciones de Estado:');
    const esEditableUtils = window.FormatUtils?.isEditable(estadoActual);
    const esEditableDirecto = estadoActual === 'B';
    console.log('  - Es editable (FormatUtils):', esEditableUtils);
    console.log('  - Es editable (directo):', esEditableDirecto);
    console.log('  - Configuración del estado:', window.FormatConfig?.estados?.[estadoActual]);
    
    console.log('🎛️ Estado de Botones:');
    console.log('  - #btnGuardar disabled:', $('#btnGuardar').prop('disabled'));
    console.log('  - #btnAgregarLinea exists:', $('#btnAgregarLinea').length > 0);
    console.log('  - .form-control readonly count:', $('.form-control[readonly]').length);
    
    console.log('📄 Estado desde DOM:');
    const badgeTexto = $('.header-title .badge').text().trim();
    console.log('  - Badge texto:', badgeTexto);
    console.log('  - Estado detectado desde badge:', detectarEstadoDeTexto(badgeTexto));
    
    console.groupEnd();
    
    // Sugerir soluciones
    if (!esEditableUtils && estadoActual === 'B') {
        console.warn('⚠️ PROBLEMA DETECTADO: Estado es "B" pero FormatUtils.isEditable devuelve false');
        console.log('💡 Posibles soluciones:');
        console.log('   1. Verificar que FormatConfig.estados["B"].editable sea true');
        console.log('   2. Recargar la página');
        console.log('   3. Llamar a inicializarVista() manualmente');
    }
};

// FUNCIÓN DE CORRECCIÓN RÁPIDA - Forzar estado editable si es Borrador
window.forzarEstadoEditable = function() {
    console.log('🔧 Forzando estado editable...');
    
    if (estadoActual === 'B' || window.FormatConfig?.estado === 'B') {
        // Forzar configuración
        if (window.FormatConfig && window.FormatConfig.estados && window.FormatConfig.estados['B']) {
            window.FormatConfig.estados['B'].editable = true;
        }
        
        // Habilitar botones manualmente
        $('#btnGuardar').prop('disabled', false);
        $('.form-control[readonly]').prop('readonly', false);
        $('.btn-success, .btn-warning, .btn-danger').prop('disabled', false);
        
        // Remover clases de solo lectura
        $('.form-control').removeClass('readonly-field');
        
        console.log('✅ Estado forzado a editable. Recargue eventos si es necesario.');
        console.log('💡 Si aún hay problemas, ejecute: configurarEventos();');
    } else {
        console.warn('⚠️ No se puede forzar editable. Estado actual no es Borrador:', estadoActual);
    }
};

// FUNCIÓN DE DIAGNÓSTICO DE SÍMBOLOS DE MONEDA
window.diagnosticarSimbolos = function() {
    console.group('💰 DIAGNÓSTICO DE SÍMBOLOS DE MONEDA');
    
    console.log('📋 Configuración JavaScript:');
    if (window.FormatConfig?.currencies) {
        Object.entries(window.FormatConfig.currencies).forEach(([code, symbol]) => {
            console.log(`  ${code}: "${symbol}" (Unicode: \\u${symbol.charCodeAt(0).toString(16).toUpperCase()})`);
        });
    }
    
    console.log('🧪 Pruebas de formateo:');
    const valorPrueba = 125000;
    
    if (typeof window.FormatUtils !== 'undefined') {
        const formatoCentralizado = window.FormatUtils.formatCurrency(valorPrueba, 'CRC');
        console.log(`  FormatUtils.formatCurrency(${valorPrueba}, 'CRC'):`, formatoCentralizado);
    }
    
    const formatoLocal = formatCurrency(valorPrueba);
    console.log(`  formatCurrency local (${valorPrueba}):`, formatoLocal);
    
    console.log('🔍 Símbolo del DOM actual:');
    const simboloModal = $('#modalTotalLinea').text();
    console.log(`  Modal total línea actual:`, simboloModal);
    
    console.log('💡 Corrección si es necesario:');
    console.log('  Para corregir símbolos, ejecute: corregirSimbolos()');
    
    console.groupEnd();
};

// FUNCIÓN PARA CORREGIR SÍMBOLOS EN EL DOM
window.corregirSimbolos = function() {
    console.log('🔧 Corrigiendo símbolos en el DOM...');
    
    // Corregir el modal
    const textoModal = $('#modalTotalLinea').text();
    if (textoModal.includes('¿') || textoModal.includes('?')) {
        const valorCorregido = textoModal.replace(/[¿?]/g, '¢');
        $('#modalTotalLinea').text(valorCorregido);
        console.log('✅ Modal corregido:', valorCorregido);
    }
    
    // Corregir tabla si es necesario
    $('.precio-display, .total-linea-display').each(function() {
        const texto = $(this).text();
        if (texto.includes('¿') || texto.includes('?')) {
            const valorCorregido = texto.replace(/[¿?]/g, '¢');
            $(this).text(valorCorregido);
            console.log('✅ Celda corregida:', valorCorregido);
        }
    });
    
    console.log('✅ Corrección de símbolos completada');
};

// FUNCIÓN DE DIAGNÓSTICO PARA CONTADOR DE CARACTERES
window.diagnosticarContadorNotas = function() {
    console.group('📝 DIAGNÓSTICO CONTADOR DE NOTAS');
    
    const textarea = $('textarea[name="Notas"]');
    const small = textarea.siblings('small');
    const maxLength = parseInt(textarea.attr('maxlength')) || 2000;
    const currentLength = textarea.val().length;
    const remaining = maxLength - currentLength;
    
    console.log('📋 Estado del campo Notas:');
    console.log('  - Textarea encontrado:', textarea.length > 0);
    console.log('  - Contenido actual:', `"${textarea.val()}"`);
    console.log('  - Caracteres actuales:', currentLength);
    console.log('  - Máximo permitido:', maxLength);
    console.log('  - Caracteres restantes (calculado):', remaining);
    
    console.log('📋 Estado del contador:');
    console.log('  - Elemento small encontrado:', small.length > 0);
    if (small.length > 0) {
        console.log('  - Texto del contador:', `"${small.text()}"`);
        console.log('  - Clases CSS:', small.attr('class'));
    }
    
    console.log('💡 Acciones disponibles:');
    console.log('  - Para actualizar: configurarContadorCaracteres()');
    
    console.groupEnd();
    
    // Ofrecer corrección automática
    if (small.length === 0) {
        console.warn('⚠️ PROBLEMA: No se encontró elemento contador');
        console.log('🔧 Ejecutando corrección...');
        configurarContadorCaracteres();
    }
};