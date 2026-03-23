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
    console.log('=== INICIO INICIALIZACIÓN VISTA ===');
    
    // Obtener configuración desde el servidor (FormatHelper)
    if (window.FormatConfig) {
        monedaActual = window.FormatConfig.moneda || 'CRC';
        estadoActual = window.FormatConfig.estado || 'B';
        console.log('Configuración desde FormatConfig:', {
            moneda: monedaActual,
            estado: estadoActual
        });
    } else {
        // Fallback al método anterior
        monedaActual = $('#formEditarCotizacion').find('input[name="Moneda"]').val() || 'CRC';
                
        // Obtener estado actual de la cotización desde el badge en el header
        const estadoBadge = $('.header-title .badge').text().trim();
        estadoActual = detectarEstadoDeTexto(estadoBadge);
        
        console.log('Configuración desde DOM fallback:', {
            moneda: monedaActual,
            estadoBadge: estadoBadge,
            estadoDetectado: estadoActual
        });
    }
            
    // Verificar si es editable usando la configuración centralizada
    const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
        window.FormatUtils.isEditable(estadoActual) : 
        (estadoActual === 'B'); // Fallback
        
    console.log('=== DETECCIÓN DE ESTADO DETALLADA ===');
    console.log('Estado detectado:', estadoActual);
    console.log('Tipo del estado:', typeof estadoActual);
    console.log('Estado === "B":', estadoActual === 'B');
    console.log('Estado == "B":', estadoActual == 'B');
    console.log('¿Es editable (lógica directa)?:', estadoActual === 'B');
    console.log('¿Es editable (FormatUtils)?:', window.FormatUtils?.isEditable(estadoActual));
    console.log('¿Es editable (resultado final)?:', esEditable);
    console.log('FormatUtils disponible:', typeof window.FormatUtils !== 'undefined');
    console.log('FormatConfig disponible:', typeof window.FormatConfig !== 'undefined');
    if (window.FormatConfig && window.FormatConfig.estados) {
        console.log('Configuración de estados completa:', window.FormatConfig.estados);
        console.log('Configuración del estado actual:', window.FormatConfig.estados[estadoActual]);
    }
    
    // DEBUGGING ADICIONAL: Verificar el DOM directamente
    const badgeElement = $('.header-title .badge');
    console.log('=== DEBUGGING DOM ===');
    console.log('Badge element found:', badgeElement.length > 0);
    console.log('Badge text raw:', `"${badgeElement.text()}"`);
    console.log('Badge text trimmed:', `"${badgeElement.text().trim()}"`);
    console.log('Badge HTML:', badgeElement.html());
    console.log('Badge classes:', badgeElement.attr('class'));
    console.log('===================');
    
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
        console.log('✅ Cotización ES EDITABLE - Habilitando funcionalidad');
        // Ya se configuró el contador arriba
    } else {
        console.log('❌ Cotización NO ES EDITABLE - Mostrando aviso');
        // Mostrar aviso si no es editable
        mostrarAvisoNoEditable();
    }
    
    // NUEVA FUNCIONALIDAD: Verificar estado inicial de moneda
    // Ejecutar con un pequeño retraso para asegurar que el DOM esté completamente listo
    setTimeout(function() {
        verificarYActualizarEstadoMoneda();
    }, 200);
    
    console.log('=== RESUMEN INICIALIZACIÓN ===');
    console.log('Vista de edición inicializada:', {
        estado: estadoActual,
        estadoTexto: (typeof window.FormatUtils !== 'undefined') ? 
            window.FormatUtils.getEstadoTexto(estadoActual) : 
            'Estado ' + estadoActual,
        editable: esEditable,
        moneda: monedaActual,
        configuracionServidor: !!window.FormatConfig,
        formatUtilsDisponible: typeof window.FormatUtils !== 'undefined'
    });
    console.log('===============================');
}

/**
 * Detecta el estado a partir del texto del badge
 */
function detectarEstadoDeTexto(texto) {
    console.log('=== DETECTAR ESTADO DE TEXTO ===');
    console.log('Texto de entrada:', `"${texto}"`);
    
    // Si tenemos FormatConfig disponible, usarlo
    if (typeof window.FormatConfig !== 'undefined' && window.FormatConfig.estados) {
        const estados = window.FormatConfig.estados;
        console.log('Usando FormatConfig.estados:', estados);
        
        for (let [codigo, config] of Object.entries(estados)) {
            console.log(`Comparando "${texto}" con "${config.texto}" (código: ${codigo})`);
            if (config.texto === texto) {
                console.log(`✅ Match encontrado: ${codigo}`);
                return codigo;
            }
        }
        console.log('❌ No se encontró match en FormatConfig');
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
    
    console.log('Usando mapeo manual fallback:', mapeosEstado);
    
    // Verificar mapeo exacto
    if (mapeosEstado[texto]) {
        console.log(`✅ Match exacto encontrado: ${mapeosEstado[texto]}`);
        return mapeosEstado[texto];
    }
    
    // Verificar mapeo case-insensitive
    const textoLower = texto.toLowerCase();
    for (let [textoEstado, codigo] of Object.entries(mapeosEstado)) {
        if (textoEstado.toLowerCase() === textoLower) {
            console.log(`✅ Match case-insensitive encontrado: ${codigo}`);
            return codigo;
        }
    }
    
    console.log('❌ No se encontró ningún match, usando default "B"');
    console.log('===============================');
    
    return 'B'; // Default a Borrador
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
        
        // Configurar tipo de cambio (siempre editable en estado Borrador)
        configurarEventoTipoCambio();
        
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
    
    // Recalcular totales (esto llamará a verificarYActualizarEstadoMoneda)
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
    fila.remove();
    
    // Reindexar filas
    reindexarFilasDetalle();
    
    // Recalcular totales (esto llamará a verificarYActualizarEstadoMoneda)
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
    
    // NUEVA FUNCIONALIDAD: Verificar si se puede cambiar moneda después del recálculo
    verificarYActualizarEstadoMoneda();
    
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
        // Preparar datos para envío
        const formData = {
            CotizacionId: $('input[name="CotizacionId"]').val(),
            VersionId: parseInt($('input[name="VersionId"]').val()),
            NombreInteresado: nombreInteresado,
            EmailInteresado: emailInteresado,
            EmpresaInteresado: $('#EmpresaInteresado').val().trim(),
            TipoInteresado: $('#TipoInteresado').val(),
            Notas: $('textarea[name="Notas"]').val().trim(),
            Detalles: [],
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };
        
        // Recopilar detalles de la tabla
        $('#tablaDetalles tbody tr').each(function() {
            const fila = $(this);
            const detalle = {
                DetalleVersionId: parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) || 0,
                ProductoId: fila.find('input[name$=".ProductoId"]').val(),
                ProductoNombre: fila.find('input[name$=".ProductoNombre"]').val(),
                Cantidad: parseFloat(fila.find('input[name$=".Cantidad"]').val()),
                PrecioUnitario: parseFloat(fila.find('input[name$=".PrecioUnitario"]').val()),
                Descuento: parseFloat(fila.find('input[name$=".Descuento"]').val()) || 0,
                TotalLinea: parseFloat(fila.find('input[name$=".TotalLinea"]').val())
            };
            
            
            formData.Detalles.push(detalle);
        });
        
        // Incluir moneda y tipo de cambio si se pueden cambiar
        const monedaSeleccionada = $('#MonedaSelect').val();
        if (monedaSeleccionada) {
            formData.Moneda = monedaSeleccionada;
        }
        
        const tipoCambioIngresado = $('#TipoCambio').val();
        if (tipoCambioIngresado && !isNaN(parseFloat(tipoCambioIngresado))) {
            formData.TipoCambio = parseFloat(tipoCambioIngresado);
        }
        
        console.log('Datos a enviar:', formData);
        
        // Enviar al servidor
        $.ajax({
            url: '/Cotizaciones/GuardarEdicion',
            type: 'POST',
            data: formData,
            success: function(response) {
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
                
                if (response.success) {
                    showNotification('success', response.message || 'Cotización guardada exitosamente');
                    
                    // Opcional: Redirigir al listado después de un momento
                    setTimeout(function() {
                        // window.location.href = '/Cotizaciones';
                        console.log('Guardado exitoso - permaneciendo en la vista de edición');
                    }, 1500);
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
                
                console.error('Error al guardar:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    response: xhr.responseJSON,
                    error: error
                });
                
                showNotification('error', errorMessage);
            }
        });
        
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

function mostrarModalConfirmacion(titulo, mensaje, tipo, onConfirm, onCancel = null) {
    console.log('Mostrando modal de confirmación:', { titulo, tipo });
    
    // Definir clases y colores para cada tipo
    const configuracionesTipo = {
        'info': {
            headerClass: 'bg-info text-white',
            icono: 'fa-info-circle',
            btnClass: 'btn-info',
            btnTexto: 'Aceptar'
        },
        'warning': {
            headerClass: 'bg-warning text-dark',
            icono: 'fa-exclamation-triangle',
            btnClass: 'btn-warning',
            btnTexto: 'Continuar'
        },
        'danger': {
            headerClass: 'bg-danger text-white',
            icono: 'fa-exclamation-circle',
            btnClass: 'btn-danger',
            btnTexto: 'Eliminar'
        },
        'success': {
            headerClass: 'bg-success text-white',
            icono: 'fa-check-circle',
            btnClass: 'btn-success',
            btnTexto: 'Aceptar'
        }
    };
    
    const config = configuracionesTipo[tipo] || configuracionesTipo['info'];
    
    // Configurar modal
    const $modalHeader = $('#modalConfirmacionHeader');
    const $modalTitulo = $('#modalConfirmacionTitulo');
    const $modalMensaje = $('#modalConfirmacionMensaje');
    const $btnConfirmar = $('#btnConfirmarAccion');
    
    // Limpiar clases anteriores del header
    $modalHeader.removeClass('bg-info bg-warning bg-danger bg-success text-white text-dark');
    $modalHeader.addClass(config.headerClass);
    
    // Configurar título con icono
    $modalTitulo.html(`<i class="fas ${config.icono}"></i> ${titulo}`);
    
    // Configurar mensaje (permitir HTML)
    $modalMensaje.html(mensaje);
    
    // Configurar botón de confirmar
    $btnConfirmar.removeClass('btn-info btn-warning btn-danger btn-success btn-primary');
    $btnConfirmar.addClass(config.btnClass);
    $btnConfirmar.html(`<i class="fas fa-check"></i> ${config.btnTexto}`);
    
    // Limpiar eventos anteriores y configurar nuevos
    $btnConfirmar.off('click.confirmacion');
    $btnConfirmar.on('click.confirmacion', function() {
        $('#modalConfirmacion').modal('hide');
        if (typeof onConfirm === 'function') {
            console.log('Ejecutando callback de confirmación');
            onConfirm();
        }
    });
    
    // Configurar callback de cancelación si existe
    if (typeof onCancel === 'function') {
        $('#modalConfirmacion').off('hidden.bs.modal.cancelacion');
        $('#modalConfirmacion').on('hidden.bs.modal.cancelacion', function(e) {
            // Solo ejecutar si se cerró sin confirmar
            if (!e.confirmedAction) {
                console.log('Modal cerrada sin confirmar, ejecutando callback de cancelación');
                onCancel();
            }
            // Limpiar el evento
            $(this).off('hidden.bs.modal.cancelacion');
        });
    }
    
    // Marcar cuando se confirma la acción
    $btnConfirmar.off('click.marcarConfirmacion');
    $btnConfirmar.on('click.marcarConfirmacion', function() {
        $('#modalConfirmacion')[0].confirmedAction = true;
    });
    
    // Limpiar la marca al mostrar el modal
    $('#modalConfirmacion')[0].confirmedAction = false;
    
    // Mostrar modal
    $('#modalConfirmacion').modal('show');
    
    console.log('Modal de confirmación configurada y mostrada');
}

function showNotification(type, message) {
    // Usar la función global de site.js en lugar del fallback
    if (typeof window.showNotification === 'function' && 
        window.showNotification !== showNotification && 
        arguments.callee !== window.showNotification) {
        try {
            window.showNotification(type, message);
        } catch (error) {
            // Solo usar console como último fallback
            console.log(`[${type.toUpperCase()}] ${message}`);
        }
    } else {
        // Si llegamos aquí, mostrar en consola (no alert)
        console.log(`[${type.toUpperCase()}] ${message}`);
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
        console.log('📋 FormatConfig completa:');
        console.log('  - moneda:', window.FormatConfig.moneda);
        console.log('  - estado:', window.FormatConfig.estado);
        console.log('  - simboloMoneda:', window.FormatConfig.simboloMoneda);
        if (window.FormatConfig.estados) {
            console.log('📋 FormatConfig.estados:');
            console.table(window.FormatConfig.estados);
        } else {
            console.log('❌ FormatConfig.estados no está definido');
        }
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
    console.log('  - Badge texto:', `"${badgeTexto}"`);
    console.log('  - Estado detectado desde badge:', detectarEstadoDeTexto(badgeTexto));
    
    console.log('🔄 RE-TEST: Forzar detección nueva');
    const nuevoEstado = detectarEstadoDeTexto(badgeTexto);
    console.log('  - Nuevo estado detectado:', nuevoEstado);
    console.log('  - ¿Es "B"?:', nuevoEstado === 'B');
    console.log('  - ¿Es editable con nuevo estado?:', nuevoEstado === 'B');
    
    console.groupEnd();
    
    // Sugerir soluciones
    if (!esEditableUtils && estadoActual === 'B') {
        console.warn('⚠️ PROBLEMA DETECTADO: Estado es "B" pero FormatUtils.isEditable devuelve false');
        console.log('💡 Posibles soluciones:');
        console.log('   1. Verificar que FormatConfig.estados["B"].editable sea true');
        console.log('   2. Recargar la página');
        console.log('   3. Llamar a inicializarVista() manualmente');
        console.log('   4. Ejecutar: window.forzarEstadoEditable()');
    }
    
    if (badgeTexto !== 'Borrador' && estadoActual === 'B') {
        console.warn('⚠️ INCONSISTENCIA: DOM muestra "' + badgeTexto + '" pero estado detectado es "B"');
    }
    
    if (badgeTexto === 'Borrador' && estadoActual !== 'B') {
        console.warn('⚠️ PROBLEMA DE DETECCIÓN: DOM muestra "Borrador" pero estado detectado es "' + estadoActual + '"');
        console.log('💡 Solución rápida: window.forzarEstadoBorrador()');
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

// FUNCIÓN ESPECÍFICA - Forzar estado a Borrador cuando DOM lo indica
window.forzarEstadoBorrador = function() {
    console.log('🔧 Forzando estado a Borrador basado en DOM...');
    
    const badgeTexto = $('.header-title .badge').text().trim();
    console.log('Badge del DOM dice:', `"${badgeTexto}"`);
    
    if (badgeTexto === 'Borrador') {
        // Forzar variable global
        estadoActual = 'B';
        
        // Forzar configuración
        if (window.FormatConfig) {
            window.FormatConfig.estado = 'B';
            if (window.FormatConfig.estados && window.FormatConfig.estados['B']) {
                window.FormatConfig.estados['B'].editable = true;
            }
        }
        
        console.log('✅ Estado forzado a "B" (Borrador)');
        console.log('💡 Ahora ejecute: configurarEventos(); para habilitar funcionalidad');
        
        // Re-ejecutar configuración automáticamente
        try {
            configurarEventos();
            console.log('✅ Eventos reconfigurados automáticamente');
            
            // Habilitar botones
            $('#btnGuardar').prop('disabled', false);
            $('.form-control[readonly]').prop('readonly', false);
            $('.btn-success, .btn-warning, .btn-danger').prop('disabled', false);
            $('.form-control').removeClass('readonly-field');
            
            console.log('✅ Interfaz habilitada para edición');
            
        } catch (error) {
            console.error('❌ Error al reconfigurar eventos:', error);
        }
        
    } else {
        console.warn('⚠️ El DOM no indica que sea Borrador. Texto actual:', badgeTexto);
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

// FUNCIÓN DE DIAGNÓSTICO ESPECÍFICA PARA MONEDAS
window.diagnosticarMonedas = function() {
    console.group('💰 DIAGNÓSTICO ESPECÍFICO DE MONEDAS');
    
    console.log('🔍 Variables globales:');
    console.log('  - monedaActual:', monedaActual);
    console.log('  - typeof monedaActual:', typeof monedaActual);
    
    console.log('🔍 Configuración del servidor:');
    console.log('  - window.MonedasDisponibles existe:', typeof window.MonedasDisponibles !== 'undefined');
    console.log('  - window.MonedasDisponibles es array:', Array.isArray(window.MonedasDisponibles));
    console.log('  - window.MonedasDisponibles contenido:', window.MonedasDisponibles);
    
    console.log('🔍 FormatConfig:');
    console.log('  - window.FormatConfig existe:', typeof window.FormatConfig !== 'undefined');
    if (window.FormatConfig) {
        console.log('  - FormatConfig.moneda:', window.FormatConfig.moneda);
        console.log('  - FormatConfig.simboloMoneda:', window.FormatConfig.simboloMoneda);
    }
    
    console.log('🔍 Estado actual de la sección moneda:');
    const seccionMoneda = $('.moneda-section');
    console.log('  - Sección encontrada:', seccionMoneda.length > 0);
    console.log('  - Contenido actual:', seccionMoneda.html());
    
    console.log('🔍 Condiciones para edición:');
    const versionActual = obtenerVersionActual();
    const estadoActual = obtenerEstadoActual();
    const totalLineas = $('#tablaDetalles tbody tr').length;
    const puedeEditarMoneda = (versionActual === 1.0 && estadoActual === 'B' && totalLineas === 0);
    
    console.log('  - Versión actual:', versionActual);
    console.log('  - Estado actual:', estadoActual);
    console.log('  - Total líneas:', totalLineas);
    console.log('  - ¿Puede editar moneda?:', puedeEditarMoneda);
    
    console.log('💡 Acciones de corrección:');
    console.log('  - Para forzar actualización: verificarYActualizarEstadoMoneda()');
    console.log('  - Para obtener moneda actual: obtenerMonedaActual()');
    
    console.groupEnd();
    
    // Auto-corrección si es posible
    if (typeof window.MonedasDisponibles === 'undefined') {
        console.warn('⚠️ MonedasDisponibles no está definido. Esto puede causar problemas.');
        console.log('💡 Verifique que el ViewBag.MonedasDisponibles se esté pasando correctamente desde el controlador.');
    }
};

// FUNCIÓN PARA FORZAR ACTUALIZACIÓN DE MONEDAS
window.forzarActualizacionMonedas = function() {
    console.log('🔧 Forzando actualización de monedas...');
    
    // Limpiar sección actual
    $('.moneda-section').html('');
    
    // Forzar re-verificación
    verificarYActualizarEstadoMoneda();
    
    console.log('✅ Actualización forzada completada');
};

// FUNCIÓN PARA PROBAR MONEDAS INMEDIATAMENTE
window.probarMonedas = function() {
    console.group('🧪 PRUEBA RÁPIDA DE MONEDAS');
    
    console.log('1. MonedasDisponibles:', window.MonedasDisponibles);
    console.log('2. Tipo:', typeof window.MonedasDisponibles);
    console.log('3. Es array:', Array.isArray(window.MonedasDisponibles));
    
    if (Array.isArray(window.MonedasDisponibles) && window.MonedasDisponibles.length > 0) {
        const primera = window.MonedasDisponibles[0];
        console.log('4. Primera moneda:', primera);
        console.log('5. Propiedades de primera moneda:', Object.keys(primera));
        console.log('6. Estructura esperada:', {
            codigo: primera.Codigo || primera.codigo,
            simbolo: primera.Simbolo || primera.simbolo,
            nombre: primera.Nombre || primera.nombre
        });
        
        // Probar función obtenerNombreMoneda
        const nombreUSD = obtenerNombreMoneda('USD');
        console.log('7. obtenerNombreMoneda("USD"):', nombreUSD);
        
        // Probar generar HTML
        console.log('8. Probando mostrarComboMoneda...');
        mostrarComboMoneda();
    } else {
        console.error('❌ MonedasDisponibles no es un array válido');
    }
    
    console.groupEnd();
};

// ========================================
// FUNCIONES PARA MANEJO DE MONEDA
// ========================================

function obtenerNombreMoneda(codigo) {
    // Intentar obtener desde configuración del servidor primero
    if (window.MonedasDisponibles && Array.isArray(window.MonedasDisponibles)) {
        const moneda = window.MonedasDisponibles.find(m => 
            (m.Codigo || m.codigo) === codigo
        );
        if (moneda) {
            return moneda.Nombre || moneda.nombre;
        }
    }
    
    // Fallback: mapeo manual para las monedas más comunes
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
    console.log(`Aplicando cambio de moneda a: ${nuevaMoneda}`);
    
    // Actualizar variable global
    const monedaAnterior = monedaActual;
    monedaActual = nuevaMoneda;
    
    // Actualizar configuración global si existe
    if (window.FormatConfig) {
        window.FormatConfig.moneda = nuevaMoneda;
        window.FormatConfig.simboloMoneda = getCurrencySymbol(nuevaMoneda);
    }
    
    // Actualizar displays financieros (aunque estén en 0 porque no hay detalles)
    actualizarDisplaysMoneda();
    
    // Mostrar notificación de éxito
    showNotification('success', `Moneda cambiada a ${obtenerNombreMoneda(nuevaMoneda)}`);
    
    console.log(`Cambio de moneda completado: ${monedaAnterior} -> ${nuevaMoneda}`);
}

function actualizarDisplaysMoneda() {
    // Actualizar los displays del resumen financiero con la nueva moneda
    const subtotal = 0; // Sin detalles, todos son 0
    const descuento = 0;
    const impuesto = 0;
    const total = 0;
    
    $('#displaySubTotal').text(formatCurrency(subtotal));
    $('#displayDescuento').text(formatCurrency(descuento));
    $('#displaySubtotalDescontado').text(formatCurrency(subtotal - descuento));
    $('#displayImpuesto').text(formatCurrency(impuesto));
    $('#displayTotal').text(formatCurrency(total));
    
    // Actualizar el texto de la moneda en el resumen
    const monedaTexto = $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Moneda:');
    });
    
    if (monedaTexto.length > 0) {
        const simbolo = getCurrencySymbol(monedaActual);
        const nombre = obtenerNombreMoneda(monedaActual);
        monedaTexto.html(`<strong>Moneda:</strong> ${simbolo} ${monedaActual} - ${nombre}`);
    }
    
    console.log('Displays de moneda actualizados');
}

// ========================================
// FUNCIONES PARA MANEJO DINÁMICO DE MONEDA
// ========================================

function verificarYActualizarEstadoMoneda() {
    console.log('Verificando estado de moneda...');
    
    // Obtener información actual
    const versionActual = obtenerVersionActual();
    const estadoActual = obtenerEstadoActual();
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    // Verificar condiciones para cambio de moneda
    const puedeEditarMoneda = (versionActual === 1.0 && estadoActual === 'B' && totalLineas === 0);
    
    console.log('Condiciones para cambio de moneda:', {
        versionActual,
        estadoActual,
        totalLineas,
        puedeEditarMoneda
    });
    
    if (puedeEditarMoneda) {
        mostrarComboMoneda();
    } else {
        mostrarDisplayMoneda(totalLineas > 0 ? 'detalles' : 'estado');
    }
    
    // Configurar tipo de cambio independientemente (siempre editable si está en Borrador)
    if (estadoActual === 'B') {
        configurarEventoTipoCambio();
    }
}

function mostrarComboMoneda() {
console.log('Cambiando a modo edición de moneda');
    
const seccionMoneda = $('.moneda-section');
if (seccionMoneda.length === 0) {
    console.warn('No se encontró la sección de moneda');
    return;
}
    
// Obtener moneda actual
const monedaActual = obtenerMonedaActual();
    
    // Obtener monedas disponibles
    let monedasDisponibles = [];
    
    console.log('Estado de MonedasDisponibles:', {
        existe: typeof window.MonedasDisponibles !== 'undefined',
        esArray: Array.isArray(window.MonedasDisponibles),
        contenido: window.MonedasDisponibles
    });
    
    // Intentar obtener desde configuración del servidor
    if (window.MonedasDisponibles && Array.isArray(window.MonedasDisponibles)) {
        monedasDisponibles = window.MonedasDisponibles.map(moneda => {
            // Manejar tanto el formato nuevo (Codigo, Simbolo, Nombre) como el viejo (codigo, simbolo, nombre)
            return {
                codigo: moneda.Codigo || moneda.codigo,
                simbolo: moneda.Simbolo || moneda.simbolo, 
                nombre: moneda.Nombre || moneda.nombre
            };
        });
        console.log('Usando monedas desde configuración del servidor:', monedasDisponibles);
    } else {
        console.warn('MonedasDisponibles no está disponible, usando fallback');
        // Fallback con datos básicos
        monedasDisponibles = [
            { codigo: 'CRC', simbolo: '₡', nombre: 'Colón Costarricense' },
            { codigo: 'USD', simbolo: '$', nombre: 'Dólar Estadounidense' },
            { codigo: 'EUR', simbolo: '€', nombre: 'Euro' },
            { codigo: 'MXN', simbolo: '$', nombre: 'Peso Mexicano' },
            { codigo: 'CAD', simbolo: '$', nombre: 'Dólar Canadiense' },
            { codigo: 'GBP', simbolo: '£', nombre: 'Libra Esterlina' }
        ];
    }
    
    // Generar options dinámicamente
    const optionsHTML = monedasDisponibles.map(moneda => 
        `<option value="${moneda.codigo}" ${monedaActual === moneda.codigo ? 'selected' : ''}>
            ${moneda.simbolo} ${moneda.codigo} - ${moneda.nombre}
        </option>`
    ).join('');
    
    // HTML para combo de monedas (sin tipo de cambio)
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
                Puede cambiar la moneda: versión 1.0, estado Borrador y sin líneas de detalle.
            </small>
        </div>
    `;
    
    seccionMoneda.html(comboHTML);
    
    // Re-configurar event listener para el combo de moneda
    configurarEventoMoneda();
}

function mostrarDisplayMoneda(razon) {
    console.log('Ocultando configuración de moneda, razón:', razon);
    
    const seccionMoneda = $('.moneda-section');
    if (seccionMoneda.length === 0) {
        console.warn('No se encontró la sección de moneda');
        return;
    }
    
    // No mostrar nada - la información de moneda ya se muestra abajo en el resumen financiero
    seccionMoneda.html('');
}

function configurarEventoMoneda() {
    // Limpiar eventos anteriores
    $('#MonedaSelect').off('change.moneda');
    
    // Configurar nuevo evento
    $('#MonedaSelect').on('change.moneda', function() {
        const nuevaMoneda = $(this).val();
        const monedaAnterior = monedaActual;
        
        console.log(`Cambio de moneda solicitado: ${monedaAnterior} -> ${nuevaMoneda}`);
        
        if (nuevaMoneda !== monedaAnterior) {
            // Verificar que no hay detalles (doble verificación)
            const totalLineas = $('#tablaDetalles tbody tr').length;
            
            if (totalLineas > 0) {
                showNotification('warning', 'No se puede cambiar la moneda cuando hay líneas de detalle agregadas.');
                $(this).val(monedaAnterior); // Revertir selección
                return;
            }
            
            // Confirmar cambio
            const nombreMonedaNueva = obtenerNombreMoneda(nuevaMoneda);
            const nombreMonedaAnterior = obtenerNombreMoneda(monedaAnterior);
            
            mostrarModalConfirmacion(
                'Confirmar Cambio de Moneda',
                `¿Está seguro de que desea cambiar la moneda de <strong>${nombreMonedaAnterior}</strong> a <strong>${nombreMonedaNueva}</strong>?<br><br>
                 <small class="text-muted">Este cambio solo es posible en versión 1.0, estado Borrador y sin líneas de detalle.</small>`,
                'warning',
                function() {
                    aplicarCambioMoneda(nuevaMoneda);
                },
                function() {
                    // Callback de cancelación - revertir selección
                    $('#MonedaSelect').val(monedaAnterior);
                    console.log('Cambio de moneda cancelado, revertido a:', monedaAnterior);
                }
            );
        }
    });
}

function obtenerVersionActual() {
    // Intentar obtener desde el badge de versión
    const versionBadge = $('.badge-version').text().trim();
    if (versionBadge) {
        // Extraer el número (ej: "v1.0" -> 1.0)
        const match = versionBadge.match(/v?(\d+\.\d+)/);
        if (match) {
            return parseFloat(match[1]);
        }
    }
    
    // Fallback: buscar en inputs ocultos o datos del DOM
    return 1.0; // Default
}

function obtenerEstadoActual() {
    // Intentar obtener desde el badge de estado o variable global
    if (typeof estadoActual !== 'undefined') {
        return estadoActual;
    }
    
    // Fallback: buscar en el DOM
    const estadoBadge = $('.header-title .badge').text().trim();
    return detectarEstadoDeTexto(estadoBadge);
}

function obtenerMonedaActual() {
    // 1. Usar variable global si está disponible
    if (typeof monedaActual !== 'undefined' && monedaActual && monedaActual !== 'undefined') {
        console.log('Moneda desde variable global:', monedaActual);
        return monedaActual;
    }
    
    // 2. Intentar obtener desde FormatConfig
    if (window.FormatConfig && window.FormatConfig.moneda && window.FormatConfig.moneda !== 'undefined') {
        console.log('Moneda desde FormatConfig:', window.FormatConfig.moneda);
        monedaActual = window.FormatConfig.moneda; // Actualizar variable global
        return monedaActual;
    }
    
    // 3. Fallback: buscar en el texto de moneda del display financiero
    const monedaTexto = $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Moneda:');
    }).text();
    
    if (monedaTexto) {
        const match = monedaTexto.match(/Moneda:\s*[^\w]*(\w+)/);
        if (match && match[1] !== 'undefined') {
            console.log('Moneda desde DOM financiero:', match[1]);
            monedaActual = match[1]; // Actualizar variable global
            return match[1];
        }
    }
    
    // 4. Default final
    console.log('Usando moneda default: CRC');
    monedaActual = 'CRC';
    return 'CRC';
}

function obtenerTipoCambioActual() {
    // 1. Intentar obtener desde FormatConfig si está disponible
    if (window.FormatConfig && window.FormatConfig.tipoCambio && window.FormatConfig.tipoCambio > 0) {
        console.log('Tipo cambio desde FormatConfig:', window.FormatConfig.tipoCambio);
        return parseFloat(window.FormatConfig.tipoCambio);
    }
    
    // 2. Buscar en el texto del resumen financiero
    const tipoCambioTexto = $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Tipo de Cambio:');
    }).text();
    
    if (tipoCambioTexto) {
        const match = tipoCambioTexto.match(/Tipo de Cambio:\s*([0-9]+\.?[0-9]*)/);
        if (match && match[1]) {
            const valor = parseFloat(match[1]);
            console.log('Tipo cambio desde DOM financiero:', valor);
            return valor;
        }
    }
    
    // 3. Buscar en input hidden si existe
    const inputTipoCambio = $('input[name="TipoCambio"]');
    if (inputTipoCambio.length > 0) {
        const valor = parseFloat(inputTipoCambio.val());
        if (valor && valor > 0) {
            console.log('Tipo cambio desde input hidden:', valor);
            return valor;
        }
    }
    
    console.log('No se encontró tipo de cambio, usando null');
    return null;
}

function configurarEventoTipoCambio() {
    // Limpiar eventos anteriores
    $('#TipoCambio').off('input.tipocambio change.tipocambio');
    
    // Validación en tiempo real
    $('#TipoCambio').on('input.tipocambio', function() {
        const valor = parseFloat($(this).val());
        
        if (isNaN(valor) || valor < 0) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });
    
    // Formateo al perder foco
    $('#TipoCambio').on('change.tipocambio', function() {
        const valor = parseFloat($(this).val());
        
        if (!isNaN(valor) && valor >= 0) {
            $(this).val(valor.toFixed(2));
            $(this).removeClass('is-invalid');
            console.log('Tipo de cambio actualizado:', valor.toFixed(2));
        }
    });
}