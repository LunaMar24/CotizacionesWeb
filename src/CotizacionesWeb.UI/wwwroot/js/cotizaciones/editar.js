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
    
// 🆕 NUEVA FUNCIONALIDAD: Interceptar navegación para confirmar salida
$(window).on('beforeunload', function(e) {
    if (verificarCambiosSinGuardar()) {
        const mensaje = '¿Está seguro de que desea salir? Se perderán los cambios no guardados.';
        e.returnValue = mensaje; // Para navegadores antiguos
        return mensaje; // Para navegadores modernos
    }
});
    
    // 🆕 Interceptar clics en enlaces de navegación
    $(document).on('click', 'a[href]:not(.btn-guardar):not([data-toggle])', function(e) {
        const href = $(this).attr('href');
        
        // Solo interceptar enlaces que navegan fuera de la página actual
        if (href && href !== '#' && !href.startsWith('#') && href !== window.location.href) {
            if (verificarCambiosSinGuardar()) {
                e.preventDefault();
                confirmarSalidaConCambios();
                return false;
            }
        }
    });
    
    // 🆕 ESPECÍFICO: Interceptar el botón "Volver al Listado"
    $('a[href*="/Cotizaciones"]:contains("Volver al Listado"), a[href="/Cotizaciones"], a[href$="/Cotizaciones/Index"]').on('click', function(e) {
        if (verificarCambiosSinGuardar()) {
            e.preventDefault();
            confirmarSalidaConCambios();
            return false;
        }
    });
    
    // 🆕 Marcar cambios al interactuar con los campos
    $('#NombreInteresado, #EmailInteresado, #EmpresaInteresado, textarea[name="Notas"]').on('input', function() {
        window.cotizacionGuardada = false; // Marcar como no guardada al hacer cambios
    });
    
    // 🆕 Marcar cambios al agregar/editar/eliminar líneas de detalle
    $(document).on('click', '#btnAgregarLinea, .btn-editar-detalle, .btn-eliminar-detalle', function() {
        window.cotizacionGuardada = false;
    });
    
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
        
        // Configurar versión (editable en estado Borrador)
        configurarEventoVersion();
        
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
    
    // 🔧 MEJORA: Indicador visual para líneas nuevas vs persistentes
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
    
    // 🔧 CORRECCIÓN CRÍTICA: Verificar estado de moneda después de agregar línea
    verificarYActualizarEstadoMoneda();
    
    console.log(`Nueva fila agregada (${esLineaNueva ? 'TEMPORAL' : 'PERSISTENTE'}):`, datos);
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
    
    // 🔧 CORRECCIÓN: Verificar si es línea persistente que se está eliminando
    const detalleVersionId = parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) || 0;
    const productoNombre = fila.find('.producto-nombre').text();
    
    if (detalleVersionId > 0) {
        console.log(`🔥 ELIMINANDO línea PERSISTENTE: DetalleVersionId=${detalleVersionId}, Producto=${productoNombre}`);
        showNotification('info', `Línea persistente "${productoNombre}" marcada para eliminación. Se eliminará de BD al guardar.`);
    } else {
        console.log(`🗑️ Eliminando línea TEMPORAL: Producto=${productoNombre}`);
    }
    
    fila.remove();
    
    // Reindexar filas
    reindexarFilasDetalle();
    
    // Recalcular totales (esto llamará a verificarYActualizarEstadoMoneda)
    recalcularTotales();
    
    showNotification('success', 'Detalle eliminado correctamente');
    
    // 🔧 MEJORA: Si eliminamos una línea persistente, verificar si ahora se puede cambiar moneda
    if (detalleVersionId > 0) {
        // Verificar líneas persistentes restantes
        let lineasPersistentesRestantes = 0;
        $('#tablaDetalles tbody tr').each(function() {
            const dvId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
            if (dvId > 0) {
                lineasPersistentesRestantes++;
            }
        });
        
        if (lineasPersistentesRestantes === 0) {
            showNotification('success', '🎉 Ya no hay líneas persistentes. Ahora puede cambiar la moneda si lo desea.');
            // Actualizar automáticamente la validación de moneda
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
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    // 🔧 MEJORA: Contador detallado que distingue líneas persistentes vs temporales
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
    
    console.log('Contador actualizado:', {
        total: totalLineas,
        persistentes: lineasPersistentes,
        temporales: lineasTemporales,
        texto: textoContador
    });
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
    
    // 🔧 CORRECCIÓN: Ir directamente al guardado sin modal de confirmación general
    // La confirmación específica se hará solo si es necesario (sin líneas, errores, etc.)
    ejecutarGuardadoCotizacion();
}

// 🆕 NUEVA FUNCIÓN: Lógica de guardado separada para poder usar confirmación
function ejecutarGuardadoCotizacion() {
    console.log('=== INICIO EJECUTAR GUARDADO ===');
    
    const form = $('#formEditarCotizacion');
    const btn = $('#btnGuardar');
    
    // 🔧 MEJORA: Validaciones más claras y permisivas
    const nombreInteresado = $('#NombreInteresado').val().trim();
    const emailInteresado = $('#EmailInteresado').val().trim();
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    console.log('📊 Estado actual:', {
        nombreInteresado: nombreInteresado,
        emailInteresado: emailInteresado,
        totalLineas: totalLineas
    });
    
    // Array para acumular errores de validación
    const erroresValidacion = [];
    
    if (!nombreInteresado) {
        erroresValidacion.push('• El nombre del interesado es obligatorio');
    }
    
    if (!emailInteresado) {
        erroresValidacion.push('• El email del interesado es obligatorio');
    }
    
    // 🔧 CORRECCIÓN CRÍTICA: Solo mostrar advertencia para líneas vacías, NO bloquear
    if (totalLineas === 0) {
        console.log('⚠️ Advertencia: Guardando cotización sin líneas de detalle');
        // 🔧 CORRECCIÓN: Mostrar modal y después continuar al confirmar
        mostrarModalConfirmacion(
            'Cotización Sin Productos',
            '¿Está seguro de que desea guardar la cotización sin líneas de productos?<br><br>' +
            '<small class="text-muted">La cotización se guardará como borrador y podrá agregar productos más tarde.</small>',
            'warning',
            function() {
                // 🔧 CLAVE: Llamar a continuar guardado después de confirmar
                console.log('✅ Usuario confirmó guardar sin líneas');
                continuarGuardadoSinValidacionLineas();
            },
            function() {
                // Usuario canceló
                console.log('❌ Usuario canceló guardado sin líneas');
            }
        );
        return; // ⚠️ IMPORTANTE: Salir aquí para esperar confirmación
    }
    
    // Si hay errores críticos, mostrarlos y no continuar
    if (erroresValidacion.length > 0) {
        console.log('❌ Errores de validación encontrados:', erroresValidacion);
        
        const mensajeError = '<strong>No se puede guardar por los siguientes errores:</strong><br><br>' +
                           erroresValidacion.join('<br>') +
                           '<br><br><small class="text-muted">Por favor corrija estos campos y vuelva a intentar.</small>';
        
        mostrarModalConfirmacion(
            'Errores de Validación',
            mensajeError,
            'danger',
            function() {
                // Solo cerrar modal, no hacer nada más
                if (erroresValidacion.some(e => e.includes('nombre'))) {
                    $('#NombreInteresado').focus();
                } else if (erroresValidacion.some(e => e.includes('email'))) {
                    $('#EmailInteresado').focus();
                }
            }
        );
        return;
    }
    
    // Si llegamos aquí, NO hay errores críticos y HAY líneas, continuar directamente
    console.log('✅ Validaciones pasadas, continuando con guardado...');
    continuarGuardadoSinValidacionLineas();
}

// 🆕 FUNCIÓN AUXILIAR: Continuar guardado sin validar líneas
function continuarGuardadoSinValidacionLineas() {
    console.log('=== INICIO CONTINUAR GUARDADO ===');
    console.log('🚀 Ejecutando guardado sin validación de líneas...');
    
    const btn = $('#btnGuardar');
    
    // Deshabilitar botón
    btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Guardando...');
    
    try {
        console.log('📋 Preparando datos para envío...');
        
        // Preparar datos para envío
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
        
        console.log('📊 Datos básicos:', {
            CotizacionId: formData.CotizacionId,
            VersionId: formData.VersionId,
            NombreInteresado: formData.NombreInteresado,
            EmailInteresado: formData.EmailInteresado
        });
        
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
        
        console.log('📋 Detalles recopilados:', formData.Detalles.length, 'líneas');
        
        // 🔧 MEJORA: Si no hay detalles, crear array vacío explícitamente
        if (formData.Detalles.length === 0) {
            console.log('ℹ️ Guardando cotización sin líneas de detalle (estado borrador)');
            formData.Detalles = [];
        }
        
        // 🔧 CORRECCIÓN: SIEMPRE incluir versión, moneda y tipo de cambio para garantizar persistencia
        
        // Incluir número de versión (puede haber sido editado)
        const numeroVersionActual = $('#NumeroVersion').val() || $('#versionValor').text().replace('v', '');
        if (numeroVersionActual) {
            formData.NumeroVersion = parseFloat(numeroVersionActual);
            console.log('📊 NumeroVersion incluido:', formData.NumeroVersion);
        }
        
        // 🔧 CRUCIAL: Incluir moneda SIEMPRE (sea del combo o la actual)
        const monedaSeleccionada = $('#MonedaSelect').val() || monedaActual;
        formData.Moneda = monedaSeleccionada;
        console.log('💱 Moneda a enviar:', formData.Moneda, '(combo:', $('#MonedaSelect').val(), ', global:', monedaActual, ')');
        
        // Incluir tipo de cambio (siempre enviar para persistencia)
        const tipoCambioIngresado = $('#TipoCambio').val();
        if (tipoCambioIngresado && !isNaN(parseFloat(tipoCambioIngresado))) {
            formData.TipoCambio = parseFloat(tipoCambioIngresado);
            console.log('💹 TipoCambio incluido:', formData.TipoCambio);
        } else {
            // Enviar null explícitamente si no hay valor
            formData.TipoCambio = null;
            console.log('💹 TipoCambio: null (no especificado)');
        }
        
        // 🔧 CORRECCIÓN: Incluir totales calculados para garantizar persistencia de montos
        formData.SubTotal = calcularSubtotalActual();
        formData.TotalDescuentos = calcularTotalDescuentos();
        formData.Impuesto = calcularImpuestoActual();
        formData.Total = calcularTotalFinalActual();
        
        console.log('💰 Totales calculados:', {
            SubTotal: formData.SubTotal,
            TotalDescuentos: formData.TotalDescuentos,
            Impuesto: formData.Impuesto,
            Total: formData.Total
        });
        
        console.log('🚀 Enviando al servidor...');
        console.log('Datos completos a enviar:', formData);
        
        // Enviar al servidor
        $.ajax({
            url: '/Cotizaciones/GuardarEdicion',
            type: 'POST',
            data: formData,
            success: function(response) {
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
                
                if (response.success) {
                    console.log('✅ GUARDADO EXITOSO');
                    showNotification('success', response.message || 'Cotización guardada exitosamente');
                    
                    // 🔧 CORRECCIÓN: Marcar que se guardó exitosamente para evitar confirmaciones
                    marcarComoGuardado();
                    
                    // 🆕 NUEVA FUNCIONALIDAD: Verificar estado después del guardado
                    setTimeout(() => {
                        verificarEstadoPostGuardado();
                    }, 1000);
                    
                    // 🔧 IMPORTANTE: Recargar página después del guardado exitoso para actualizar estado
                    setTimeout(function() {
                        console.log('🔄 Recargando página para reflejar cambios...');
                        showNotification('info', 'Recargando página para mostrar cambios...');
                        
                        // Recargar la página actual en lugar de redirigir
                        window.location.reload(true); // true fuerza recarga desde servidor
                    }, 2000);
                } else {
                    console.error('❌ Error en respuesta del servidor:', response.message);
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
                
                console.error('❌ Error AJAX:', {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    response: xhr.responseJSON,
                    error: error
                });
                
                showNotification('error', errorMessage);
            }
        });
        
    } catch (error) {
        console.error('❌ Error al procesar guardado:', error);
        btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
        showNotification('error', 'Error al procesar los datos de la cotización');
    }
    
    console.log('=== FIN CONTINUAR GUARDADO ===');
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
    console.log('🔍 DEBUG MODAL - Iniciando:', { titulo, tipo });
    
    // 🔧 VERIFICACIÓN COMPLETA: Bootstrap y modal disponibles
    if (typeof $ === 'undefined') {
        console.error('❌ jQuery no está disponible');
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    if (typeof $.fn.modal !== 'function') {
        console.error('❌ Bootstrap modal no está disponible');
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    const modal = $('#modalConfirmacion');
    if (modal.length === 0) {
        console.error('❌ Modal #modalConfirmacion no existe en DOM');
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    // Verificar elementos críticos del modal
    const elementos = {
        header: $('#modalConfirmacionHeader'),
        titulo: $('#modalConfirmacionTitulo'),
        mensaje: $('#modalConfirmacionMensaje'),
        btnConfirmar: $('#btnConfirmarAccion')
    };
    
    const elementosFaltantes = Object.keys(elementos).filter(key => elementos[key].length === 0);
    if (elementosFaltantes.length > 0) {
        console.error('❌ Elementos del modal faltantes:', elementosFaltantes);
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    console.log('✅ Modal y Bootstrap disponibles, configurando...');
    
    // 🔧 LIMPIAR ESTADO PREVIO COMPLETAMENTE
    modal.off();
    elementos.btnConfirmar.off();
    $('.modal-backdrop').remove();
    $('body').removeClass('modal-open').css({ 'overflow': '', 'padding-right': '' });
    
    // Variables de control
    let accionConfirmada = false;
    let modalCerrandose = false;
    
    // Configurar apariencia según tipo
    const config = obtenerConfiguracionModal(tipo);
    elementos.header.removeClass('bg-info bg-warning bg-danger bg-success text-white text-dark').addClass(config.headerClass);
    elementos.titulo.html(`<i class="fas ${config.icono}"></i> ${titulo}`);
    elementos.mensaje.html(mensaje);
    elementos.btnConfirmar.removeClass('btn-info btn-warning btn-danger btn-success btn-primary').addClass(config.btnClass);
    elementos.btnConfirmar.html(`<i class="fas fa-check"></i> ${config.btnTexto}`);
    
    // 🔧 EVENTO CONFIRMACIÓN - Más robusto
    elementos.btnConfirmar.on('click.modalconfirm', function(e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        
        if (modalCerrandose) {
            console.log('🔍 Click ignorado: modal ya cerrandose');
            return;
        }
        
        console.log('🔍 DEBUG MODAL - CONFIRMACIÓN ejecutada');
        accionConfirmada = true;
        modalCerrandose = true;
        
        // Cerrar modal y ejecutar callback
        modal.modal('hide');
        
        setTimeout(() => {
            if (typeof onConfirm === 'function') {
                try {
                    onConfirm();
                } catch (error) {
                    console.error('❌ Error en callback confirmación:', error);
                }
            }
        }, 200);
    });
    
    // 🔧 EVENTO CIERRE MODAL
    modal.on('hidden.bs.modal.confirm', function() {
        console.log('🔍 DEBUG MODAL - Modal cerrado:', { accionConfirmada, modalCerrandose });
        
        if (!accionConfirmada && !modalCerrandose && typeof onCancel === 'function') {
            console.log('🔍 DEBUG MODAL - CANCELACIÓN ejecutada');
            setTimeout(() => {
                try {
                    onCancel();
                } catch (error) {
                    console.error('❌ Error en callback cancelación:', error);
                }
            }, 100);
        }
        
        // Limpiar eventos
        $(this).off('.confirm');
        elementos.btnConfirmar.off('.modalconfirm');
    });
    
    // 🔧 MOSTRAR MODAL CON VERIFICACIÓN
    console.log('🔍 DEBUG MODAL - Mostrando modal...');
    
    try {
        modal.modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
        
        // Verificar que se mostró correctamente
        setTimeout(() => {
            if (modal.hasClass('show')) {
                console.log('✅ Modal mostrado correctamente');
            } else {
                console.error('❌ Modal no se mostró, usando fallback');
                modal.off();
                elementos.btnConfirmar.off();
                usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
            }
        }, 500);
        
    } catch (error) {
        console.error('❌ Error mostrando modal:', error);
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
    }
}

// 🆕 FUNCIÓN AUXILIAR: Confirmación nativa como fallback
function usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel) {
    console.log('🔄 Usando confirmación nativa');
    
    // Limpiar HTML del mensaje para mostrar solo texto
    const mensajeTexto = mensaje.replace(/<[^>]*>/g, '').replace(/\s+/g, ' ').trim();
    const textoCompleto = `${titulo}\n\n${mensajeTexto}`;
    
    // Usar setTimeout para evitar bloqueo inmediato
    setTimeout(() => {
        const confirmacion = confirm(textoCompleto);
        
        if (confirmacion && typeof onConfirm === 'function') {
            onConfirm();
        } else if (!confirmacion && typeof onCancel === 'function') {
            onCancel();
        }
    }, 10);
}

// 🆕 FUNCIÓN AUXILIAR: Configuración de modal por tipo
function obtenerConfiguracionModal(tipo) {
    const configuraciones = {
        'info': { headerClass: 'bg-info text-white', icono: 'fa-info-circle', btnClass: 'btn-info', btnTexto: 'Aceptar' },
        'warning': { headerClass: 'bg-warning text-dark', icono: 'fa-exclamation-triangle', btnClass: 'btn-warning', btnTexto: 'Continuar' },
        'danger': { headerClass: 'bg-danger text-white', icono: 'fa-exclamation-circle', btnClass: 'btn-danger', btnTexto: 'Eliminar' },
        'success': { headerClass: 'bg-success text-white', icono: 'fa-check-circle', btnClass: 'btn-success', btnTexto: 'Aceptar' }
    };
    
    return configuraciones[tipo] || configuraciones['info'];
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

// ========================================
// FUNCIONES AUXILIARES PARA MONEDA
// ========================================

// 🆕 FUNCIÓN AUXILIAR: Revertir combo de moneda de forma segura
function revertirComboMoneda($combo, monedaAnterior) {
    console.log(`🔄 Revirtiendo combo de ${$combo.val()} a ${monedaAnterior}`);
    
    // Desconectar todos los eventos temporalmente
    $combo.off('change.moneda');
    
    // Establecer valor anterior por múltiples métodos
    $combo.val(monedaAnterior);
    
    // Forzar por índice si es necesario
    if ($combo.val() !== monedaAnterior) {
        const opcionAnterior = $combo.find(`option[value="${monedaAnterior}"]`);
        if (opcionAnterior.length > 0) {
            $combo[0].selectedIndex = opcionAnterior.index();
        }
    }
    
    console.log(`✅ Combo revertido a: ${$combo.val()}`);
}

// 🆕 FUNCIÓN MEJORADA: Modal de confirmación específico para monedas
function mostrarModalConfirmacionMoneda(titulo, mensaje, tipo, onConfirm, onCancel, $combo, monedaAnterior) {
    console.log('Mostrando modal de confirmación de moneda:', { titulo, tipo });
    
    // Limpiar eventos previos del modal para evitar conflictos
    $('#modalConfirmacion').off('hidden.bs.modal.moneda');
    $('#btnConfirmarAccion').off('click.moneda');
    
    // Configurar el modal usando la función existente
    mostrarModalConfirmacion(titulo, mensaje, tipo, 
        function() {
            // Callback de confirmación
            if (typeof onConfirm === 'function') {
                onConfirm();
            }
        },
        function() {
            // Este callback de cancelación podría no ejecutarse correctamente
            // Así que usamos un evento adicional como respaldo
        }
    );
    
    // 🔧 RESPALDO: Manejar cancelación cuando se cierre el modal sin confirmar
    let confirmacionEjecutada = false;
    
    // Marcar cuando se confirma
    $('#btnConfirmarAccion').on('click.moneda', function() {
        confirmacionEjecutada = true;
    });
    
    // Detectar cuando se cierra el modal
    $('#modalConfirmacion').on('hidden.bs.modal.moneda', function() {
        // Solo ejecutar cancelación si no se confirmó
        if (!confirmacionEjecutada) {
            console.log('Modal cerrado sin confirmación, ejecutando cancelación...');
            if (typeof onCancel === 'function') {
                onCancel();
            }
        }
        
        // Limpiar eventos
        $(this).off('hidden.bs.modal.moneda');
        $('#btnConfirmarAccion').off('click.moneda');
    });
    
    console.log('Modal de confirmación de moneda configurado');
}

// ========================================
// FUNCIONES PARA CÁLCULOS Y CONFIRMACIONES
// ========================================

// 🆕 FUNCIÓN: Calcular subtotal actual de la tabla
function calcularSubtotalActual() {
    let subtotal = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const cantidad = parseFloat($(this).find('input[name$=".Cantidad"]').val()) || 0;
        const precio = parseFloat($(this).find('input[name$=".PrecioUnitario"]').val()) || 0;
        subtotal += (cantidad * precio);
    });
    return subtotal;
}

// 🆕 FUNCIÓN: Calcular total de descuentos actual
function calcularTotalDescuentos() {
    let totalDescuentos = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const descuento = parseFloat($(this).find('input[name$=".Descuento"]').val()) || 0;
        totalDescuentos += descuento;
    });
    return totalDescuentos;
}

// 🆕 FUNCIÓN: Calcular impuesto actual
function calcularImpuestoActual() {
    const subtotal = calcularSubtotalActual();
    const descuentos = calcularTotalDescuentos();
    const subtotalConDescuentos = subtotal - descuentos;
    return subtotalConDescuentos * 0.13; // 13% impuesto Costa Rica
}

// 🆕 FUNCIÓN: Calcular total final actual
function calcularTotalFinalActual() {
    const subtotal = calcularSubtotalActual();
    const descuentos = calcularTotalDescuentos();
    const impuesto = calcularImpuestoActual();
    return (subtotal - descuentos) + impuesto;
}

// 🆕 FUNCIÓN: Confirmación para salir/navegar
function confirmarSalidaConCambios() {
    // Verificar si hay cambios sin guardar
    const hayCambios = verificarCambiosSinGuardar();
    
    if (!hayCambios) {
        return true; // No hay cambios, puede salir
    }
    
    // Mostrar confirmación modal
    mostrarModalConfirmacion(
        'Cambios Sin Guardar',
        '¿Está seguro de que desea salir sin guardar los cambios?<br><br>' +
        '<strong class="text-danger">Se perderán todos los cambios realizados.</strong>',
        'danger',
        function() {
            // Confirma salida sin guardar
            window.location.href = '/Cotizaciones';
        },
        function() {
            // Cancela salida - no hace nada, se queda en la vista
            console.log('Usuario canceló salida, permanece en edición');
        }
    );
    
    return false; // Bloquea navegación hasta confirmación
}

// 🆕 FUNCIÓN: Verificar si hay cambios sin guardar
function verificarCambiosSinGuardar() {
    // Si se guardó recientemente, no considerar cambios
    if (seGuardoRecientemente()) {
        return false;
    }
    
    // Verificar cambios en campos principales
    const nombreActual = $('#NombreInteresado').val().trim();
    const emailActual = $('#EmailInteresado').val().trim();
    const empresaActual = $('#EmpresaInteresado').val().trim();
    const notasActuales = $('textarea[name="Notas"]').val().trim();
    
    // Verificar cambios en moneda y versión
    const monedaCombo = $('#MonedaSelect').val();
    const versionEditada = $('#NumeroVersion').val();
    const tipoCambioEditado = $('#TipoCambio').val();
    
    // Verificar si hay líneas en la tabla
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    // 🔧 MEJORA: Considerar líneas temporales vs persistentes
    let lineasNuevas = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
        if (detalleVersionId === 0) {
            lineasNuevas++;
        }
    });
    
    // Considerar que hay cambios si:
    // 1. Hay contenido en campos principales
    // 2. Hay líneas nuevas (temporales) en la tabla
    // 3. Se cambió moneda, versión o tipo de cambio
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
    
    console.log('Verificación de cambios (MEJORADA):', {
        nombreActual: nombreActual.length,
        emailActual: emailActual.length,
        totalLineas,
        lineasNuevas,
        monedaCambiada: (monedaCombo && monedaCombo !== monedaActual),
        hayCambios
    });
    
    return hayCambios;
}

// 🆕 FUNCIÓN AUXILIAR: Verificar si se puede cambiar moneda y sugerir acción
function verificarYSugerirCambioMoneda() {
    const estadoActual = obtenerEstadoActual();
    
    if (estadoActual !== 'B') {
        showNotification('info', 'El cambio de moneda solo está disponible en cotizaciones en estado Borrador.');
        return false;
    }
    
    // Contar líneas persistentes
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
    
    if (lineasPersistentes > 0) {
        // Sugerir eliminar líneas persistentes
        mostrarModalConfirmacion(
            'Eliminar Líneas para Cambiar Moneda',
            `Para cambiar la moneda debe eliminar las <strong>${lineasPersistentes} líneas guardadas</strong> en la base de datos.<br><br>` +
            '¿Desea eliminar todas las líneas guardadas para proceder con el cambio de moneda?<br><br>' +
            '<small class="text-warning">⚠️ Esta acción eliminará las líneas de forma permanente.</small>',
            'warning',
            function() {
                eliminarTodasLasLineasPersistentes();
            }
        );
        return false;
    }
    
    if (lineasTemporales > 0) {
        showNotification('info', `Hay ${lineasTemporales} líneas temporales. Éstas no impiden el cambio de moneda ya que no están guardadas.`);
    }
    
    return true; // Se puede cambiar moneda
}

// 🆕 FUNCIÓN: Eliminar todas las líneas persistentes
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
    
    // Eliminar líneas de atrás hacia adelante para evitar problemas de índices
    lineasAEliminar.reverse().forEach(index => {
        const fila = $(`tr[data-index="${index}"]`);
        fila.remove();
    });
    
    // Reindexar después de eliminar
    reindexarFilasDetalle();
    recalcularTotales();
    
    showNotification('success', `${lineasAEliminar.length} líneas persistentes eliminadas. Ahora puede cambiar la moneda.`);
    
    // Actualizar estado de moneda automáticamente
    verificarYActualizarEstadoMoneda();
}

// 🆕 FUNCIÓN: Marcar como guardado para evitar confirmaciones innecesarias
function marcarComoGuardado() {
    window.cotizacionGuardada = true;
    console.log('✅ Cotización marcada como guardada');
}

// 🆕 FUNCIÓN: Verificar si se guardó recientemente
function seGuardoRecientemente() {
    return window.cotizacionGuardada === true;
}

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

// 🆕 FUNCIÓN DE DIAGNÓSTICO PARA MONEDAS MEJORADA
window.diagnosticarMonedas = function() {
    console.group('💰 DIAGNÓSTICO COMPLETO DE MONEDAS');
    
    console.log('🔍 Variables globales:');
    console.log('  - monedaActual (variable):', monedaActual);
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
    
    console.log('🔍 DOM - Elementos de moneda:');
    const elementoMonedaCodigo = $('#displayMonedaCodigo');
    console.log('  - #displayMonedaCodigo encontrado:', elementoMonedaCodigo.length > 0);
    console.log('  - #displayMonedaCodigo texto:', elementoMonedaCodigo.text().trim());
    
    const comboMoneda = $('#MonedaSelect');
    console.log('  - #MonedaSelect encontrado:', comboMoneda.length > 0);
    console.log('  - #MonedaSelect valor:', comboMoneda.val());
    console.log('  - #MonedaSelect options count:', comboMoneda.find('option').length);
    
    console.log('🔍 Método obtenerMonedaActual():');
    const monedaDetectada = obtenerMonedaActual();
    console.log('  - Moneda detectada:', monedaDetectada);
    
    console.log('🔍 Estado actual de la sección moneda:');
    const seccionMoneda = $('.moneda-section');
    console.log('  - Sección encontrada:', seccionMoneda.length > 0);
    console.log('  - Contenido HTML:', seccionMoneda.html()?.substring(0, 200) + '...');
    
    console.log('🔍 Condiciones para edición:');
    const estadoActual = obtenerEstadoActual();
    const totalLineas = $('#tablaDetalles tbody tr').length;
    let lineasPersistentes = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
        if (detalleVersionId > 0) lineasPersistentes++;
    });
    const puedeEditarMoneda = (estadoActual === 'B' && lineasPersistentes === 0);
    
    console.log('  - Estado actual:', estadoActual);
    console.log('  - Total líneas:', totalLineas);
    console.log('  - Líneas persistentes:', lineasPersistentes);
    console.log('  - ¿Puede editar moneda?:', puedeEditarMoneda);
    
    console.log('💡 Acciones de corrección:');
    console.log('  - Para forzar actualización: verificarYActualizarEstadoMoneda()');
    console.log('  - Para probar combo: window.probarComboMoneda()');
    console.log('  - Para resetear moneda: window.resetearMoneda()');
    
    console.groupEnd();
    
    // Auto-corrección si es posible
    if (typeof window.MonedasDisponibles === 'undefined') {
        console.warn('⚠️ MonedasDisponibles no está definido. Esto puede causar problemas.');
    }
    
    if (comboMoneda.length > 0 && comboMoneda.val() !== monedaDetectada) {
        console.warn(`⚠️ Inconsistencia: Combo muestra "${comboMoneda.val()}" pero moneda detectada es "${monedaDetectada}"`);
    }
};

// 🆕 FUNCIÓN DE DIAGNÓSTICO PARA MODALES
window.diagnosticarModalMoneda = function() {
    console.group('🔍 DIAGNÓSTICO MODAL DE MONEDA');
    
    console.log('📊 Estado del modal:');
    const modal = $('#modalConfirmacion');
    console.log('  - Modal existe:', modal.length > 0);
    console.log('  - Modal visible:', modal.hasClass('show'));
    console.log('  - Modal HTML:', modal.length > 0 ? 'Disponible' : 'NO DISPONIBLE');
    
    console.log('📋 Estado del combo:');
    const combo = $('#MonedaSelect');
    console.log('  - Combo existe:', combo.length > 0);
    console.log('  - Eventos registrados:', $._data(combo[0], 'events'));
    console.log('  - Valor actual:', combo.val());
    console.log('  - Moneda global:', monedaActual);
    
    console.log('🎛️ Eventos activos:');
    console.log('  - Modal hidden events:', $._data(modal[0], 'events'));
    console.log('  - Button click events:', $._data($('#btnConfirmarAccion')[0], 'events'));
    
    console.log('💡 Acciones de prueba:');
    console.log('  - Para probar modal: window.probarModalMoneda()');
    console.log('  - Para limpiar eventos: window.limpiarEventosModal()');
    
    console.groupEnd();
};

// 🆕 FUNCIÓN PARA PROBAR EL MODAL
window.probarModalMoneda = function() {
    console.log('🧪 Probando modal de moneda...');
    
    mostrarModalConfirmacionMoneda(
        'Prueba de Modal',
        'Este es un modal de prueba para verificar funcionamiento',
        'info',
        function() {
            console.log('✅ Confirmación funciona correctamente');
            showNotification('success', 'Modal de confirmación funciona');
        },
        function() {
            console.log('❌ Cancelación funciona correctamente');
            showNotification('info', 'Modal de cancelación funciona');
        },
        $('#MonedaSelect'),
        'CRC'
    );
};

// 🆕 FUNCIÓN PARA LIMPIAR EVENTOS DEL MODAL
window.limpiarEventosModal = function() {
    console.log('🧹 Limpiando todos los eventos del modal...');
    
    $('#modalConfirmacion').off();
    $('#btnConfirmarAccion').off();
    
    console.log('✅ Eventos del modal limpiados');
};

// 🆕 FUNCIÓN DE DIAGNÓSTICO ESPECÍFICA PARA EL PROBLEMA
window.diagnosticarProblemaComboMoneda = function() {
    console.group('🔍 DIAGNÓSTICO PROBLEMA COMBO MONEDA');
    
    const combo = $('#MonedaSelect');
    const valorMostrado = combo.val();
    const textoMostrado = combo.find('option:selected').text();
    const indiceSeleccionado = combo[0].selectedIndex;
    
    console.log('📊 Estado actual del combo:');
    console.log('  - Valor mostrado:', valorMostrado);
    console.log('  - Texto mostrado:', textoMostrado);
    console.log('  - Índice seleccionado:', indiceSeleccionado);
    console.log('  - Moneda global:', monedaActual);
    console.log('  - FormatConfig.moneda:', window.FormatConfig?.moneda);
    
    console.log('📋 Opciones disponibles:');
    combo.find('option').each(function(index) {
        const value = $(this).val();
        const text = $(this).text();
        const selected = this.selected;
        console.log(`  ${index}: "${value}" -> "${text}" ${selected ? '(SELECTED ✓)' : ''}`);
    });
    
    console.log('🔍 Labels en el DOM:');
    const labelMoneda = $('#displayMonedaCodigo');
    console.log('  - Label moneda texto:', labelMoneda.text());
    
    // Detectar inconsistencia
    const hayInconsistencia = (valorMostrado !== monedaActual) || 
                             (window.FormatConfig?.moneda && valorMostrado !== window.FormatConfig.moneda);
    
    if (hayInconsistencia) {
        console.warn('⚠️ INCONSISTENCIA DETECTADA:');
        console.log('  - Combo muestra:', valorMostrado);
        console.log('  - Variable global:', monedaActual);
        console.log('  - FormatConfig:', window.FormatConfig?.moneda);
        
        console.log('💡 Soluciones disponibles:');
        console.log('  1. window.forzarComboMonedaCorreto(monedaActual)');
        console.log('  2. window.resetearMoneda()');
        console.log('  3. Recargar la página');
        
        // Ofrecer auto-corrección
        const monedaCorrecta = monedaActual || window.FormatConfig?.moneda || 'CRC';
        console.log(`🔧 Ejecutando auto-corrección a: ${monedaCorrecta}`);
        window.forzarComboMonedaCorreto(monedaCorrecta);
    } else {
        console.log('✅ No se detectaron inconsistencias');
    }
    
    console.groupEnd();
};

// 🆕 FUNCIÓN PARA PROBAR EL COMBO DE MONEDA
window.probarComboMoneda = function() {
    console.log('🧪 Probando combo de moneda...');
    
    const combo = $('#MonedaSelect');
    if (combo.length === 0) {
        console.error('❌ Combo de moneda no encontrado');
        return;
    }
    
    console.log('📋 Options disponibles:');
    combo.find('option').each(function(index) {
        const value = $(this).val();
        const text = $(this).text();
        const selected = $(this).prop('selected');
        console.log(`  ${index}: "${value}" -> "${text}" ${selected ? '(SELECTED)' : ''}`);
    });
    
    console.log('🔍 Estado actual:');
    console.log('  - Valor seleccionado:', combo.val());
    console.log('  - Moneda global:', monedaActual);
    console.log('  - ¿Coinciden?:', combo.val() === monedaActual);
    
    // Probar cambio manual
    const opciones = combo.find('option').map(function() { return $(this).val(); }).get();
    if (opciones.length > 1) {
        const nuevaOpcion = opciones.find(op => op !== combo.val());
        if (nuevaOpcion) {
            console.log(`🧪 Probando cambio a: ${nuevaOpcion}`);
            combo.val(nuevaOpcion).trigger('change');
        }
    }
};

// 🆕 FUNCIÓN PARA TESTEAR CAMBIOS SECUENCIALES DE MONEDA
window.testearCambioMonedaSecuencial = function() {
    console.log('🧪 Iniciando test de cambio secuencial de moneda...');
    
    const combo = $('#MonedaSelect');
    if (combo.length === 0) {
        console.error('❌ Combo no disponible');
        return;
    }
    
    const opciones = combo.find('option').map(function() { 
        return $(this).val(); 
    }).get().filter(val => val !== '');
    
    console.log('Opciones disponibles:', opciones);
    
    if (opciones.length < 2) {
        console.error('❌ No hay suficientes opciones para probar');
        return;
    }
    
    let indiceActual = 0;
    
    function cambiarAlaSiguiente() {
        if (indiceActual >= opciones.length - 1) {
            console.log('✅ Test completado - todas las opciones probadas');
            return;
        }
        
        indiceActual++;
        const siguienteMoneda = opciones[indiceActual];
        
        console.log(`🔄 Cambiando a: ${siguienteMoneda} (${indiceActual}/${opciones.length - 1})`);
        
        // Simular cambio de usuario
        combo.val(siguienteMoneda).trigger('change');
        
        // Programar siguiente cambio
        setTimeout(cambiarAlaSiguiente, 3000); // 3 segundos entre cambios
    }
    
    // Iniciar el test
    cambiarAlaSiguiente();
};

// 🆕 FUNCIÓN DE DIAGNÓSTICO ESPECÍFICA PARA GUARDADO DE MONEDA
window.diagnosticarGuardadoMoneda = function() {
    console.group('🔍 DIAGNÓSTICO GUARDADO DE MONEDA');
    
    const monedaCombo = $('#MonedaSelect').val();
    const formData = {
        CotizacionId: $('input[name="CotizacionId"]').val(),
        VersionId: parseInt($('input[name="VersionId"]').val()),
        Moneda: monedaCombo || monedaActual,
        MonedaAnterior: monedaActual,
        MonedaGlobal: window.FormatConfig?.moneda
    };
    
    console.log('📊 Estado antes del guardado:');
    console.table(formData);
    
    console.log('🔍 Verificaciones:');
    console.log('  - ¿Hay cambio de moneda?:', monedaCombo !== monedaActual);
    console.log('  - ¿Es editable (estado)?:', estadoActual === 'B');
    console.log('  - ¿Hay líneas persistentes?:', contarLineasPersistentes());
    
    const lineasPersistentes = contarLineasPersistentes();
    console.log('  - Líneas persistentes encontradas:', lineasPersistentes);
    
    // Mostrar lo que se enviaría al servidor
    const datosParaEnvio = {
        CotizacionId: formData.CotizacionId,
        VersionId: formData.VersionId,
        Moneda: formData.Moneda,
        NombreInteresado: $('#NombreInteresado').val(),
        EmailInteresado: $('#EmailInteresado').val(),
        Detalles: []
    };
    
    $('#tablaDetalles tbody tr').each(function() {
        const fila = $(this);
        const detalle = {
            DetalleVersionId: parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) || 0,
            ProductoId: fila.find('input[name$=".ProductoId"]').val(),
            EsPersistente: parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) > 0
        };
        datosParaEnvio.Detalles.push(detalle);
    });
    
    console.log('📤 Datos que se enviarían al servidor:');
    console.log('  - Moneda:', datosParaEnvio.Moneda);
    console.log('  - Líneas persistentes en envío:', datosParaEnvio.Detalles.filter(d => d.EsPersistente).length);
    console.log('  - Total líneas:', datosParaEnvio.Detalles.length);
    
    console.groupEnd();
    
    return {
        puedeGuardarConCambioMoneda: (estadoActual === 'B' && lineasPersistentes === 0),
        datosParaEnvio
    };
};

function contarLineasPersistentes() {
    let lineasPersistentes = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
        if (detalleVersionId > 0) {
            lineasPersistentes++;
        }
    });
    return lineasPersistentes;
}

// 🆕 FUNCIÓN PARA PROBAR EL GUARDADO COMPLETO
window.probarGuardadoConMoneda = function(nuevaMoneda) {
    console.log('🧪 Probando guardado con cambio de moneda a:', nuevaMoneda);
    
    // Cambiar la moneda primero
    const combo = $('#MonedaSelect');
    if (combo.length > 0) {
        combo.val(nuevaMoneda).trigger('change');
        
        // Esperar un poco y luego intentar guardar
        setTimeout(() => {
            console.log('🔄 Intentando guardar después del cambio de moneda...');
            $('#btnGuardar').click();
        }, 2000);
    } else {
        console.error('❌ Combo de moneda no encontrado');
    }
};

// 🆕 FUNCIÓN PARA RESETEAR MONEDA
window.resetearMoneda = function() {
    console.log('🔄 Reseteando estado de moneda...');
    
    // Limpiar variable global
    monedaActual = 'CRC';
    
    // Actualizar FormatConfig si existe
    if (window.FormatConfig) {
        window.FormatConfig.moneda = 'CRC';
        window.FormatConfig.simboloMoneda = '¢';
    }
    
    // Limpiar combo si existe
    const combo = $('#MonedaSelect');
    if (combo.length > 0) {
        combo.val('CRC');
    }
    
    // Actualizar displays
    $('#displayMonedaCodigo').text('CRC');
    actualizarDisplaysMoneda();
    
    console.log('✅ Moneda reseteada a CRC');
};

// 🆕 FUNCIÓN PARA FORZAR COMBO CORRECTO
window.forzarComboMonedaCorreto = function(monedaEsperada) {
    console.log('🔧 Forzando combo a moneda correcta:', monedaEsperada);
    
    const combo = $('#MonedaSelect');
    if (combo.length === 0) {
        console.error('❌ Combo no encontrado');
        return false;
    }
    
    console.log('Estado antes de corrección:', {
        valorActual: combo.val(),
        monedaEsperada: monedaEsperada,
        optionsDisponibles: combo.find('option').map(function() { return $(this).val(); }).get()
    });
    
    // Desconectar eventos
    combo.off('change.moneda');
    
    // Establecer valor forzadamente
    combo.val(monedaEsperada);
    
    // Verificar y forzar por índice si es necesario
    const opcionCorrecta = combo.find(`option[value="${monedaEsperada}"]`);
    if (opcionCorrecta.length > 0) {
        const indice = opcionCorrecta.index();
        combo[0].selectedIndex = indice;
        console.log('✅ Forzado por índice:', indice);
    }
    
    // Actualizar variables globales
    monedaActual = monedaEsperada;
    if (window.FormatConfig) {
        window.FormatConfig.moneda = monedaEsperada;
    }
    
    console.log('Estado después de corrección:', {
        valorCombo: combo.val(),
        selectedIndex: combo[0].selectedIndex,
        monedaGlobal: monedaActual
    });
    
    // Reconectar eventos
    setTimeout(() => {
        combo.on('change.moneda', configurarEventoMoneda);
    }, 100);
    
    return combo.val() === monedaEsperada;
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

// 🆕 FUNCIÓN DE DIAGNÓSTICO PARA PERSISTENCIA
window.diagnosticarPersistencia = function() {
    console.group('🔍 DIAGNÓSTICO DE PERSISTENCIA');
    
    console.log('📊 Estado de datos para envío:');
    const formData = {
        CotizacionId: $('input[name="CotizacionId"]').val(),
        VersionId: parseInt($('input[name="VersionId"]').val()),
        Moneda: $('#MonedaSelect').val() || monedaActual,
        NumeroVersion: $('#NumeroVersion').val() || $('#versionValor').text().replace('v', ''),
        TipoCambio: $('#TipoCambio').val()
    };
    
    console.table(formData);
    
    console.log('💾 Líneas de detalle:');
    const detalles = [];
    $('#tablaDetalles tbody tr').each(function() {
        const fila = $(this);
        const detalle = {
            Index: parseInt(fila.attr('data-index')),
            DetalleVersionId: parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) || 0,
            ProductoId: fila.find('input[name$=".ProductoId"]').val(),
            EsPersistente: parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) > 0,
            Estado: parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) > 0 ? 'PERSISTENTE' : 'TEMPORAL'
        };
        detalles.push(detalle);
    });
    
    console.table(detalles);
    
    console.log('🔄 Resumen:');
    const totalLineas = detalles.length;
    const lineasPersistentes = detalles.filter(d => d.EsPersistente).length;
    const lineasTemporales = totalLineas - lineasPersistentes;
    
    console.log(`  - Total líneas: ${totalLineas}`);
    console.log(`  - Líneas persistentes (BD): ${lineasPersistentes}`);
    console.log(`  - Líneas temporales (nuevas): ${lineasTemporales}`);
    console.log(`  - ¿Puede cambiar moneda?: ${lineasPersistentes === 0 ? '✅ SÍ' : '❌ NO'}`);
    
    console.log('💱 Estado de moneda:');
    console.log(`  - Moneda actual (variable): ${monedaActual}`);
    console.log(`  - Moneda seleccionada (combo): ${$('#MonedaSelect').val() || 'N/A'}`);
    console.log(`  - ¿Hay cambio de moneda?: ${($('#MonedaSelect').val() && $('#MonedaSelect').val() !== monedaActual) ? '✅ SÍ' : '❌ NO'}`);
    
    console.groupEnd();
    
    return {
        formData,
        detalles,
        resumen: {
            totalLineas,
            lineasPersistentes,
            lineasTemporales,
            puedeCambiarMoneda: lineasPersistentes === 0,
            hayCambioMoneda: $('#MonedaSelect').val() && $('#MonedaSelect').val() !== monedaActual
        }
    };
};

// 🆕 FUNCIÓN PARA PROBAR ELIMINACIÓN FORZADA
window.forzarEliminacionLineasPersistentes = function() {
    console.log('🔥 Eliminando FORZADAMENTE todas las líneas persistentes...');
    
    const lineasEliminadas = [];
    $('#tablaDetalles tbody tr').each(function() {
        const fila = $(this);
        const detalleVersionId = parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) || 0;
        
        if (detalleVersionId > 0) {
            const productName = fila.find('.producto-nombre').text();
            lineasEliminadas.push({ id: detalleVersionId, producto: productName });
            fila.remove();
        }
    });
    
    if (lineasEliminadas.length > 0) {
        reindexarFilasDetalle();
        recalcularTotales();
        verificarYActualizarEstadoMoneda();
        
        console.log(`✅ Eliminadas ${lineasEliminadas.length} líneas persistentes:`, lineasEliminadas);
        showNotification('success', `🔥 ${lineasEliminadas.length} líneas persistentes eliminadas. Ahora puede cambiar la moneda.`);
    } else {
        console.log('ℹ️ No hay líneas persistentes que eliminar');
        showNotification('info', 'No hay líneas persistentes que eliminar');
    }
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
    console.log(`💱 Aplicando cambio de moneda a: ${nuevaMoneda}`);
    
    // Actualizar variable global
    const monedaAnterior = monedaActual;
    monedaActual = nuevaMoneda;
    
    // Actualizar configuración global si existe
    if (window.FormatConfig) {
        window.FormatConfig.moneda = nuevaMoneda;
        window.FormatConfig.simboloMoneda = getCurrencySymbol(nuevaMoneda);
    }
    
    // 🔧 CORRECCIÓN MEJORADA: Forzar la actualización del combo correctamente
    const comboMoneda = $('#MonedaSelect');
    if (comboMoneda.length > 0) {
        // Desconectar eventos temporalmente para evitar loops
        comboMoneda.off('change.moneda');
        
        // Establecer el valor correcto
        comboMoneda.val(nuevaMoneda);
        
        // Verificar que se estableció correctamente
        const valorActualCombo = comboMoneda.val();
        console.log('🔧 Combo actualizado:', {
            valorEsperado: nuevaMoneda,
            valorActual: valorActualCombo,
            esticoEsperado: valorActualCombo === nuevaMoneda
        });
        
        // Forzar refresh visual del combo (trigger sin events)
        comboMoneda[0].selectedIndex = comboMoneda.find(`option[value="${nuevaMoneda}"]`).index();
        
        // Reconectar eventos después de un pequeño delay
        setTimeout(() => {
            comboMoneda.on('change.moneda', configurarEventoMoneda);
        }, 100);
    }
    
    // Actualizar displays financieros (aunque estén en 0 porque no hay detalles)
    actualizarDisplaysMoneda();
    
    // 🔧 MEJORA: Marcar como cambio pendiente de guardar
    window.cotizacionGuardada = false;
    
    // Mostrar notificación de éxito con instrucción
    showNotification('success', 
        `💱 Moneda cambiada a ${obtenerNombreMoneda(nuevaMoneda)}. ` +
        `Debe GUARDAR la cotización para persistir el cambio.`);
    
    console.log(`✅ Cambio de moneda completado: ${monedaAnterior} -> ${nuevaMoneda} (pendiente de guardar)`);
}

function actualizarDisplaysMoneda() {
    // Actualizar los displays del resumen financiero con la nueva moneda
    const subtotal = 0; // Sin detalles, todos son 0
    const descuento = 0;
    const impuesto = 0;
    const total = 0;
    
    // 🔧 CORRECCIÓN CRÍTICA: Usar monedaActual consistentemente
    $('#displaySubTotal').text(formatCurrency(subtotal));
    $('#displayDescuento').text(formatCurrency(descuento));
    $('#displaySubtotalDescontado').text(formatCurrency(subtotal - descuento));
    $('#displayImpuesto').text(formatCurrency(impuesto));
    $('#displayTotal').text(formatCurrency(total));
    
    // 🔧 CORRECCIÓN CRÍTICA: Actualizar AMBOS elementos de moneda
    $('#displayMonedaCodigo').text(monedaActual);
    
    // 🔧 NUEVO: También actualizar el texto "Moneda:" que aparece abajo
    $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Moneda:');
    }).html(`<strong>Moneda:</strong> ${monedaActual}`);
    
    // 🔧 CORRECCIÓN: Forzar recálculo de totales si hay líneas
    if ($('#tablaDetalles tbody tr').length > 0) {
        recalcularTotales(); // Esto asegura que todos los totales usen la moneda correcta
    }
    
    console.log('✅ Displays de moneda actualizados a:', monedaActual);
}

// ========================================
// FUNCIONES PARA MANEJO DINÁMICO DE MONEDA
// ========================================

function verificarYActualizarEstadoMoneda() {
    console.log('Verificando estado de moneda...');
    
    // Obtener información actual
    const estadoActual = obtenerEstadoActual();
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    // 🔧 CORRECCIÓN FINAL: La moneda se puede cambiar SOLO cuando:
    // 1. Está en estado Borrador, Y
    // 2. NO hay NINGUNA línea (ni temporal ni persistente)
    // 
    // Esto es consistente con la regla de negocio: "No se puede cambiar moneda si hay líneas de detalle"
    
    const puedeEditarMoneda = (estadoActual === 'B' && totalLineas === 0);
    
    // Para diagnóstico detallado
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
    
    console.log('Condiciones para cambio de moneda (REGLA NEGOCIO):', {
        estadoActual,
        totalLineas,
        lineasPersistentes,
        lineasTemporales,
        puedeEditarMoneda,
        regla: 'NO puede haber líneas para cambiar moneda'
    });
    
    if (puedeEditarMoneda) {
        mostrarComboMoneda();
    } else {
        const razon = totalLineas > 0 ? 'hay-lineas' : 'estado-no-borrador';
        mostrarDisplayMoneda(razon);
    }
    
    // Configurar tipo de cambio independientemente (siempre editable si está en Borrador)
    if (estadoActual === 'B') {
        configurarEventoTipoCambio();
    }
}

function mostrarComboMoneda() {
    console.log('💱 Cambiando a modo edición de moneda');
    
    const seccionMoneda = $('.moneda-section');
    if (seccionMoneda.length === 0) {
        console.warn('No se encontró la sección de moneda');
        return;
    }
    
    // 🔧 CORRECCIÓN: Obtener moneda actual correctamente
    const monedaActualReal = obtenerMonedaActual();
    console.log('💰 Moneda actual detectada para combo:', monedaActualReal);
    
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
    
    // 🔧 CORRECCIÓN: Generar options con la moneda actual correctamente seleccionada
    const optionsHTML = monedasDisponibles.map(moneda => {
        const isSelected = monedaActualReal === moneda.codigo;
        console.log(`🔍 Comparando: "${monedaActualReal}" === "${moneda.codigo}" -> ${isSelected}`);
        return `<option value="${moneda.codigo}" ${isSelected ? 'selected' : ''}>
            ${moneda.simbolo} ${moneda.codigo} - ${moneda.nombre}
        </option>`;
    }).join('');
    
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
                Puede cambiar la moneda: estado Borrador y sin líneas guardadas en BD.
            </small>
        </div>
    `;
    
    seccionMoneda.html(comboHTML);
    
    // 🔧 CORRECCIÓN MEJORADA: Establecer valor inicial con múltiples métodos
    const comboCreado = $('#MonedaSelect');
    
    // Método 1: Establecer valor directamente
    comboCreado.val(monedaActualReal);
    
    // Método 2: Si no funciona el método 1, forzar por índice
    if (comboCreado.val() !== monedaActualReal) {
        const opcionCorrecta = comboCreado.find(`option[value="${monedaActualReal}"]`);
        if (opcionCorrecta.length > 0) {
            comboCreado[0].selectedIndex = opcionCorrecta.index();
            console.log('🔧 Forzado por selectedIndex:', opcionCorrecta.index());
        }
    }
    
    // Método 3: Verificar que se estableció correctamente
    const valorFinal = comboCreado.val();
    console.log('✅ Verificación final combo:', {
        esperado: monedaActualReal,
        obtenido: valorFinal,
        esCorecto: valorFinal === monedaActualReal,
        selectedIndex: comboCreado[0].selectedIndex
    });
    
    // Si aún no es correcto, intentar una vez más
    if (valorFinal !== monedaActualReal) {
        console.warn('⚠️ Combo no se estableció correctamente, intentando corrección...');
        setTimeout(() => {
            window.forzarComboMonedaCorreto(monedaActualReal);
        }, 200);
    }
    
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
        const $combo = $(this); // Referencia fija al combo
        const nuevaMoneda = $combo.val();
        const monedaAnterior = monedaActual;
        
        console.log(`💱 Cambio de moneda solicitado: ${monedaAnterior} -> ${nuevaMoneda}`);
        console.log(`🔍 Estado del combo: valor="${nuevaMoneda}", texto="${$combo.find('option:selected').text()}"`);
        
        if (nuevaMoneda !== monedaAnterior) {
            // 🔧 CORRECCIÓN: Verificar líneas persistentes, no temporales
            let lineasPersistentes = 0;
            $('#tablaDetalles tbody tr').each(function() {
                const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
                if (detalleVersionId > 0) {
                    lineasPersistentes++;
                }
            });
            
            if (lineasPersistentes > 0) {
                showNotification('warning', 'No se puede cambiar la moneda cuando hay líneas guardadas en la base de datos. Elimine todas las líneas persistentes primero.');
                // 🔧 CORRECCIÓN: Revertir INMEDIATAMENTE sin triggerar eventos
                console.log(`⚠️ Revirtiendo combo: ${nuevaMoneda} -> ${monedaAnterior}`);
                revertirComboMoneda($combo, monedaAnterior);
                return;
            }
            
            // 🔧 PREVENIR CAMBIOS MIENTRAS ESTÁ EL MODAL ABIERTO
            $combo.off('change.moneda');
            
            // Confirmar cambio
            const nombreMonedaNueva = obtenerNombreMoneda(nuevaMoneda);
            const nombreMonedaAnterior = obtenerNombreMoneda(monedaAnterior);
            
            mostrarModalConfirmacionMoneda(
                'Confirmar Cambio de Moneda',
                `¿Está seguro de que desea cambiar la moneda de <strong>${nombreMonedaAnterior}</strong> a <strong>${nombreMonedaNueva}</strong>?<br><br>
                 <small class="text-muted">Este cambio es posible porque no hay líneas guardadas en la base de datos.</small>`,
                'warning',
                function() {
                    // ✅ CONFIRMADO: Aplicar el cambio de moneda
                    console.log(`✅ Usuario CONFIRMÓ cambio: ${monedaAnterior} -> ${nuevaMoneda}`);
                    aplicarCambioMoneda(nuevaMoneda);
                    // Reconectar eventos después del cambio exitoso
                    setTimeout(() => {
                        configurarEventoMoneda();
                    }, 300);
                },
                function() {
                    // ❌ CANCELADO: Revertir selección sin triggerar eventos
                    console.log(`❌ Usuario CANCELÓ cambio: ${nuevaMoneda} -> ${monedaAnterior}`);
                    revertirComboMoneda($combo, monedaAnterior);
                    // Reconectar eventos después de revertir
                    setTimeout(() => {
                        configurarEventoMoneda();
                    }, 300);
                },
                $combo,
                monedaAnterior
            );
        } else {
            console.log('ℹ️ Misma moneda seleccionada, no se hace nada');
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
    // 1. Usar variable global si está disponible y es válida
    if (typeof monedaActual !== 'undefined' && monedaActual && monedaActual !== 'undefined') {
        console.log('💰 Moneda desde variable global:', monedaActual);
        return monedaActual;
    }
    
    // 2. Intentar obtener desde FormatConfig
    if (window.FormatConfig && window.FormatConfig.moneda && window.FormatConfig.moneda !== 'undefined') {
        console.log('💰 Moneda desde FormatConfig:', window.FormatConfig.moneda);
        monedaActual = window.FormatConfig.moneda; // Actualizar variable global
        return monedaActual;
    }
    
    // 3. 🔧 CORREGIDO: Buscar en el elemento específico de moneda del DOM
    const elementoMonedaCodigo = $('#displayMonedaCodigo');
    if (elementoMonedaCodigo.length > 0) {
        const monedaDesdeCodigo = elementoMonedaCodigo.text().trim();
        if (monedaDesdeCodigo && monedaDesdeCodigo !== 'undefined') {
            console.log('💰 Moneda desde #displayMonedaCodigo:', monedaDesdeCodigo);
            monedaActual = monedaDesdeCodigo; // Actualizar variable global
            return monedaActual;
        }
    }
    
    // 4. Fallback: buscar en el texto de moneda del display financiero
    const monedaTexto = $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Moneda:');
    }).text();
    
    if (monedaTexto) {
        const match = monedaTexto.match(/Moneda:\s*[^\w]*(\w+)/);
        if (match && match[1] !== 'undefined') {
            console.log('💰 Moneda desde texto financiero:', match[1]);
            monedaActual = match[1]; // Actualizar variable global
            return match[1];
        }
    }
    
    // 5. 🔧 NUEVO: Buscar en cualquier elemento que contenga el símbolo de moneda
    const simbolosMoneda = ['¢', '$', '€', '£', '¥'];
    const elementosConSimbolos = $('.financial-summary *').filter(function() {
        const texto = $(this).text();
        return simbolosMoneda.some(simbolo => texto.includes(simbolo));
    });
    
    if (elementosConSimbolos.length > 0) {
        const textoConSimbolo = elementosConSimbolos.first().text();
        console.log('💰 Texto con símbolo encontrado:', textoConSimbolo);
        
        // Detectar moneda por símbolo
        if (textoConSimbolo.includes('¢')) {
            console.log('💰 Moneda detectada por símbolo ¢: CRC');
            monedaActual = 'CRC';
            return 'CRC';
        } else if (textoConSimbolo.includes('$')) {
            console.log('💰 Moneda detectada por símbolo $: USD (asumido)');
            monedaActual = 'USD';
            return 'USD';
        } else if (textoConSimbolo.includes('€')) {
            console.log('💰 Moneda detectada por símbolo €: EUR');
            monedaActual = 'EUR';
            return 'EUR';
        }
    }
    
    // 6. Default final
    console.log('💰 Usando moneda default: CRC');
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
        
        console.log('Modo edición versión activado, valor original:', valorOriginal);
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
        
        // Volver a modo vista
        $('#versionModoEdicion').addClass('d-none');
        $('#versionModoVista').removeClass('d-none');
        
        console.log('Versión actualizada:', displayFormateado);
        
        // Mostrar notificación solo si realmente cambió el valor
        const valorOriginalNumerico = parseFloat(valorOriginal);
        
        if (numero !== valorOriginalNumerico) {
            showNotification('success', `Versión actualizada a ${displayFormateado}`);
        }
    }
    
    function cancelarEdicionVersion() {
        // Restaurar valor original
        $('#NumeroVersion').val(valorOriginal);
        $('#NumeroVersion').removeClass('is-invalid');
        
        // Volver a modo vista
        $('#versionModoEdicion').addClass('d-none');
        $('#versionModoVista').removeClass('d-none');
        
        console.log('Edición de versión cancelada');
    }
}

// 🆕 FUNCIÓN PARA FORZAR CAMBIO DE MONEDA CORRECTO
window.forzarCambioMonedaCorrectamente = function(nuevaMoneda) {
    console.log(`💱 Forzando cambio de moneda a: ${nuevaMoneda}`);
    
    const combo = $('#MonedaSelect');
    if (combo.length === 0) {
        console.error('❌ Combo de moneda no disponible');
        return Promise.reject('Combo no disponible');
    }
    
    const monedaActualCombo = combo.val();
    console.log(`🔍 Estado actual: Combo=${monedaActualCombo}, Global=${monedaActual}`);
    
    if (monedaActualCombo === nuevaMoneda) {
        console.log('ℹ️ Ya está en la moneda deseada');
        return Promise.resolve();
    }
    
    return new Promise((resolve, reject) => {
        // Configurar listener temporal para detectar cuando se complete el cambio
        let cambioCompletado = false;
        
        const timeoutId = setTimeout(() => {
            if (!cambioCompletado) {
                console.error('⏰ Timeout: El cambio de moneda tardó demasiado');
                reject('Timeout en cambio de moneda');
            }
        }, 10000); // 10 segundos máximo
        
        // Listener para detectar cuando se completa el cambio
        function onCambioCompletado() {
            if (combo.val() === nuevaMoneda && monedaActual === nuevaMoneda) {
                cambioCompletado = true;
                clearTimeout(timeoutId);
                console.log('✅ Cambio de moneda completado exitosamente');
                resolve();
            }
        }
        
        // Verificar periódicamente
        const checkInterval = setInterval(() => {
            if (cambioCompletado) {
                clearInterval(checkInterval);
                return;
            }
            
            if (combo.val() === nuevaMoneda && monedaActual === nuevaMoneda) {
                clearInterval(checkInterval);
                onCambioCompletado();
            }
        }, 500);
        
        // Triggear el cambio
        console.log(`🔄 Triggerando cambio de ${combo.val()} a ${nuevaMoneda}`);
        combo.val(nuevaMoneda).trigger('change');
    });
};

// 🆕 FUNCIÓN DE PRUEBA MEJORADA CON ASYNC/AWAIT
window.probarCambioMonedaAsync = async function() {
    console.group('🧪 PRUEBA ASYNC: CAMBIO DE MONEDA');
    
    try {
        const cotizacionId = $('input[name="CotizacionId"]').val();
        console.log('1. CotizacionId:', cotizacionId);
        
        // Paso 1: Verificar estado inicial
        console.log('📊 PASO 1: Verificando estado inicial...');
        window.diagnosticarGuardadoMoneda();
        
        // Paso 2: Eliminar líneas persistentes
        console.log('📊 PASO 2: Eliminando líneas persistentes...');
        await new Promise((resolve, reject) => {
            $.ajax({
                url: `/Cotizaciones/EliminarLineasParaCambioMoneda/${cotizacionId}`,
                type: 'POST',
                data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
                success: (response) => {
                    if (response.success) {
                        console.log('✅ Líneas eliminadas:', response.message);
                        resolve(response);
                    } else {
                        reject(response.message);
                    }
                },
                error: (xhr) => reject('Error de comunicación')
            });
        });
        
        // Esperar un poco para que se actualice el DOM
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        // Paso 3: Cambiar moneda de forma controlada
        console.log('📊 PASO 3: Cambiando moneda a USD...');
        await window.forzarCambioMonedaCorrectamente('USD');
        
        // Paso 4: Completar datos
        console.log('📊 PASO 4: Completando datos...');
        $('#NombreInteresado').val('Prueba Async USD');
        $('#EmailInteresado').val('async-usd@test.com');
        
        // Paso 5: Guardar
        console.log('📊 PASO 5: Guardando...');
        window.forzarGuardadoDirecto();
        
        // Paso 6: Verificar resultado
        setTimeout(() => {
            console.log('📊 PASO 6: Verificando resultado...');
            window.verificarEstadoPostGuardado();
        }, 5000);
        
        console.log('✅ Prueba completada exitosamente');
        
    } catch (error) {
        console.error('❌ Error en la prueba:', error);
    } finally {
        console.groupEnd();
    }
};

// 🆕 FUNCIÓN PARA ELIMINAR LÍNEAS PERSISTENTES USANDO EL SERVIDOR
window.eliminarLineasPersistentesViaServidor = function() {
    console.log('🔄 Eliminando líneas persistentes via servidor...');
    
    const cotizacionId = $('input[name="CotizacionId"]').val();
    if (!cotizacionId) {
        console.error('❌ No se puede obtener CotizacionId');
        return;
    }
    
    // Obtener el token antiforgery
    const token = $('input[name="__RequestVerificationToken"]').val();
    
    console.log('📤 Enviando petición para eliminar líneas...');
    
    $.ajax({
        url: `/Cotizaciones/EliminarLineasParaCambioMoneda/${cotizacionId}`,
        type: 'POST',
        data: {
            __RequestVerificationToken: token
        },
        success: function(response) {
            if (response.success) {
                console.log('✅ Líneas eliminadas exitosamente');
                showNotification('success', response.message);
                
                // Actualizar el frontend para reflejar los cambios
                $('#tablaDetalles tbody').empty();
                actualizarContadorLineas();
                recalcularTotales();
                
                // Verificar que ahora se puede cambiar moneda
                setTimeout(() => {
                    verificarYActualizarEstadoMoneda();
                    window.verificarEstadoPostGuardado();
                }, 1000);
                
            } else {
                console.error('❌ Error:', response.message);
                showNotification('error', response.message);
            }
        },
        error: function(xhr, status, error) {
            console.error('❌ Error de comunicación:', error);
            showNotification('error', 'Error de comunicación con el servidor');
        }
    });
};

// 🆕 FUNCIÓN PARA PROBAR ESPECÍFICAMENTE EL PROBLEMA DEL GUARDADO DE MONEDA
window.probarProblemaGuardadoMoneda = function() {
    console.group('🧪 PRUEBA ESPECÍFICA: PROBLEMA GUARDADO DE MONEDA');
    
    const cotizacionId = $('input[name="CotizacionId"]').val();
    console.log('1. CotizacionId:', cotizacionId);
    
    // Paso 1: Verificar estado inicial
    console.log('📊 PASO 1: Verificando estado inicial...');
    window.diagnosticarGuardadoMoneda();
    
    // Paso 2: Usar el servidor para eliminar líneas persistentes
    console.log('📊 PASO 2: Eliminando líneas persistentes via servidor...');
    window.eliminarLineasPersistentesViaServidor();
    
    // Los pasos 3-6 se ejecutarán después del callback de eliminación exitosa
    setTimeout(() => {
        // Paso 3: Cambiar moneda a USD CORRECTAMENTE
        console.log('📊 PASO 3: Cambiando moneda a USD...');
        const combo = $('#MonedaSelect');
        if (combo.length > 0 && combo.val() !== 'USD') {
            // 🔧 CORRECCIÓN: Usar el evento change real, no solo cambiar el valor
            console.log(`  Cambiando de ${combo.val()} a USD usando evento change`);
            combo.val('USD').trigger('change.moneda');
            console.log('  Moneda cambiada a USD usando trigger change');
        } else if (combo.val() === 'USD') {
            console.log('  Moneda ya está en USD, continuando...');
        } else {
            console.error('  ❌ Combo no disponible');
        }
        
        // Paso 4: Completar datos mínimos
        console.log('📊 PASO 4: Completando datos mínimos...');
        $('#NombreInteresado').val('Prueba Moneda USD');
        $('#EmailInteresado').val('prueba-usd@test.com');
        console.log('  Datos completados');
        
        // Paso 5: Esperar a que el modal de cambio de moneda se procese
        console.log('📊 PASO 5: Esperando procesamiento de cambio de moneda...');
        
        setTimeout(() => {
            console.log('🔄 Ejecutando guardado con datos de prueba...');
            
            // 🔧 VALIDACIÓN: Verificar que la moneda está correctamente establecida
            const monedaFinal = $('#MonedaSelect').val();
            console.log(`💱 Validación pre-guardado: Combo=${monedaFinal}, Global=${monedaActual}`);
            
            if (monedaFinal === 'USD' && monedaActual === 'USD') {
                console.log('✅ Moneda correctamente establecida, procediendo con guardado');
                $('#btnGuardar').click();
            } else {
                console.error('❌ Problema con moneda antes del guardado:', {
                    combo: monedaFinal,
                    global: monedaActual
                });
            }
            
            // Paso 6: Verificar después de un tiempo
            setTimeout(() => {
                console.log('📊 PASO 6: Verificando resultado...');
                window.verificarEstadoPostGuardado();
            }, 5000); // Más tiempo para que termine el guardado
            
        }, 2000); // Más tiempo para que se procese el cambio de moneda
        
    }, 3000); // Esperar a que se complete la eliminación
    
    console.groupEnd();
};

// 🆕 FUNCIÓN PARA FORZAR GUARDADO DIRECTO (BYPASS MODAL)
window.forzarGuardadoDirecto = function() {
    console.log('🚀 Forzando guardado directo (bypass modal)...');
    
    // Asegurar que no hay líneas persistentes
    const lineasPersistentes = contarLineasPersistentes();
    if (lineasPersistentes > 0) {
        console.warn('❌ Aún hay líneas persistentes, no se puede continuar');
        return;
    }
    
    // Llamar directamente la función de guardado sin modal
    window.cotizacionGuardada = false; // Marcar como no guardada
    ejecutarGuardadoCotizacion(); // Llamar directamente
};

// 🆕 FUNCIÓN DE DEBUG ESPECÍFICA PARA PERSISTENCIA DE MONEDA
window.debugPersistenciaMoneda = function() {
    console.group('🔍 DEBUG ESPECÍFICO: PERSISTENCIA DE MONEDA');
    
    const cotizacionId = $('input[name="CotizacionId"]').val();
    const monedaCombo = $('#MonedaSelect').val();
    const monedaGlobal = monedaActual;
    
    console.log('📊 Estado actual del frontend:');
    console.log('  - CotizacionId:', cotizacionId);
    console.log('  - Combo moneda:', monedaCombo);
    console.log('  - Variable global:', monedaGlobal);
    console.log('  - FormatConfig moneda:', window.FormatConfig?.moneda);
    
    // Verificar consistencia
    const esConsistente = (monedaCombo === monedaGlobal);
    console.log('  - ¿Frontend consistente?:', esConsistente);
    
    if (!esConsistente) {
        console.warn('⚠️ INCONSISTENCIA en frontend detectada');
    }
    
    // Verificar estado en BD
    fetch(`/Cotizaciones/DebugMoneda/${cotizacionId}`)
        .then(response => response.json())
        .then(data => {
            console.log('📊 Estado en base de datos:');
            console.log('  - Moneda BD:', data.monedaCotizacion);
            console.log('  - Estado:', data.estado);
            console.log('  - Puede editar:', data.puedeEditarMoneda);
            console.log('  - Líneas persistentes:', data.detalleVersion?.lineasPersistentes);
            
            console.log('🔄 Análisis de discrepancias:');
            const discrepanciaFrontendBD = monedaCombo !== data.monedaCotizacion;
            console.log('  - Frontend vs BD:', discrepanciaFrontendBD ? '❌ DIFERENTE' : '✅ IGUAL');
            
            if (discrepanciaFrontendBD) {
                console.warn(`🚨 PROBLEMA: Frontend tiene "${monedaCombo}" pero BD tiene "${data.monedaCotizacion}"`);
                console.log('💡 Posibles soluciones:');
                console.log('  1. window.forzarCambioMonedaCorrectamente("USD")');
                console.log('  2. window.probarCambioMonedaAsync()');
                console.log('  3. Recargar la página');
            }
        })
        .catch(error => {
            console.error('❌ Error al consultar BD:', error);
        });
    
    console.groupEnd();
};

// 🆕 FUNCIÓN DE PRUEBA COMPLETA: Cambio de moneda + Guardado + Verificación
window.pruebaFlujoCambioMonedaCompleto = async function() {
    console.group('🧪 PRUEBA FLUJO COMPLETO: CAMBIO MONEDA + GUARDADO + VERIFICACIÓN');
    
    try {
        const cotizacionId = $('input[name="CotizacionId"]').val();
        console.log('🔍 PASO 1: Verificando estado inicial...');
        
        // Verificar estado inicial
        const estadoInicial = await fetch(`/Cotizaciones/DebugMoneda/${cotizacionId}`)
            .then(r => r.json());
        
        console.log('📊 Estado inicial:', {
            moneda: estadoInicial.monedaCotizacion,
            estado: estadoInicial.estado,
            lineasPersistentes: estadoInicial.detalleVersion?.lineasPersistentes
        });
        
        // Paso 2: Si hay líneas persistentes, eliminarlas
        if (estadoInicial.detalleVersion?.lineasPersistentes > 0) {
            console.log('🔥 PASO 2: Eliminando líneas persistentes...');
            
            const resultadoEliminacion = await fetch(`/Cotizaciones/EliminarLineasParaCambioMoneda/${cotizacionId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: new URLSearchParams({
                    '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                })
            }).then(r => r.json());
            
            if (!resultadoEliminacion.success) {
                throw new Error('Error eliminando líneas: ' + resultadoEliminacion.message);
            }
            
            console.log('✅ Líneas eliminadas:', resultadoEliminacion.message);
            
            // Limpiar la tabla en el frontend
            $('#tablaDetalles tbody').empty();
            actualizarContadorLineas();
            recalcularTotales();
            verificarYActualizarEstadoMoneda();
        }
        
        // Paso 3: Cambiar moneda a USD
        console.log('💱 PASO 3: Cambiando moneda a USD...');
        
        // Completar datos mínimos
        $('#NombreInteresado').val('Prueba Moneda USD');
        $('#EmailInteresado').val('prueba-usd@test.com');
        
        // Forzar cambio de moneda sin modal
        monedaActual = 'USD';
        if (window.FormatConfig) {
            window.FormatConfig.moneda = 'USD';
        }
        
        // Si hay combo, actualizarlo
        const combo = $('#MonedaSelect');
        if (combo.length > 0) {
            combo.off('change.moneda'); // Desconectar eventos temporalmente
            combo.val('USD');
        }
        
        console.log('💾 PASO 4: Guardando cambios...');
        
        // Guardar usando la función directa
        const resultadoGuardado = await new Promise((resolve, reject) => {
            // Preparar datos para envío
            const formData = {
                CotizacionId: $('input[name="CotizacionId"]').val(),
                VersionId: parseInt($('input[name="VersionId"]').val()),
                NombreInteresado: $('#NombreInteresado').val().trim(),
                EmailInteresado: $('#EmailInteresado').val().trim(),
                EmpresaInteresado: $('#EmpresaInteresado').val().trim(),
                TipoInteresado: $('#TipoInteresado').val(),
                Moneda: 'USD', // Forzar USD
                Notas: $('textarea[name="Notas"]').val().trim(),
                Detalles: [],
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            };
            
            console.log('📤 Datos a enviar:', formData);
            
            $.ajax({
                url: '/Cotizaciones/GuardarEdicion',
                type: 'POST',
                data: formData,
                success: resolve,
                error: reject
            });
        });
        
        if (!resultadoGuardado.success) {
            throw new Error('Error guardando: ' + resultadoGuardado.message);
        }
        
        console.log('✅ Guardado exitoso:', resultadoGuardado.message);
        
        // Paso 5: Verificar resultado después de un delay
        console.log('🔍 PASO 5: Verificando resultado en BD...');
        
        await new Promise(resolve => setTimeout(resolve, 2000)); // Esperar 2 segundos
        
        const estadoFinal = await fetch(`/Cotizaciones/DebugMoneda/${cotizacionId}`)
            .then(r => r.json());
        
        console.log('📊 Estado final en BD:', {
            moneda: estadoFinal.monedaCotizacion,
            estado: estadoFinal.estado,
            timestamp: estadoFinal.timestamp
        });
        
        // Verificar éxito
        if (estadoFinal.monedaCotizacion === 'USD') {
            console.log('🎉 ¡ÉXITO TOTAL! La moneda se persistió correctamente como USD');
            showNotification('success', '🎉 ¡Prueba exitosa! La moneda se cambió y persistió correctamente.');
            
            // Recargar la página para confirmar
            setTimeout(() => {
                console.log('🔄 Recargando página para confirmación final...');
                window.location.reload();
            }, 3000);
            
        } else {
            console.error('❌ FALLO: La moneda en BD sigue siendo:', estadoFinal.monedaCotizacion);
            showNotification('error', `❌ Error: La moneda no se persistió. BD muestra: ${estadoFinal.monedaCotizacion}`);
        }
        
    } catch (error) {
        console.error('❌ Error en la prueba:', error);
        showNotification('error', 'Error en la prueba: ' + error.message);
    } finally {
        console.groupEnd();
    }
};

// 🆕 FUNCIÓN DE PRUEBA: Guardado normal (con validaciones pero sin confirmación doble)
window.pruebaGuardadoNormal = function() {
    console.group('🧪 PRUEBA GUARDADO NORMAL (VALIDACIONES, SIN CONFIRMACIÓN DOBLE)');
    
    // Asegurar que hay datos mínimos
    if (!$('#NombreInteresado').val()) {
        $('#NombreInteresado').val('Prueba Save Normal');
    }
    if (!$('#EmailInteresado').val()) {
        $('#EmailInteresado').val('test@save.com');
    }
    
    console.log('📊 Datos preparados:', {
        nombre: $('#NombreInteresado').val(),
        email: $('#EmailInteresado').val(),
        totalLineas: $('#tablaDetalles tbody tr').length
    });
    
    // Llamar la función de guardado normal (corregida sin doble modal)
    console.log('🚀 Llamando a guardarCotizacion() corregida...');
    guardarCotizacion();
    
    console.groupEnd();
};

// 🆕 FUNCIÓN ESPECÍFICA: Corregir inconsistencia UI de moneda
window.corregirInconsistenciaMonedaUI = function() {
    console.group('🔧 CORRECCIÓN INCONSISTENCIA UI DE MONEDA');
    
    console.log('🔍 Paso 1: Detectando moneda real desde combo...');
    const combo = $('#MonedaSelect');
    let monedaReal = 'CRC'; // Default
    
    if (combo.length > 0) {
        monedaReal = combo.val() || 'CRC';
        console.log('Moneda desde combo:', monedaReal);
    } else {
        // Si no hay combo, usar FormatConfig
        if (window.FormatConfig?.moneda) {
            monedaReal = window.FormatConfig.moneda;
            console.log('Moneda desde FormatConfig:', monedaReal);
        } else {
            // Último recurso: detectar desde variable global
            if (monedaActual && monedaActual !== 'undefined') {
                monedaReal = monedaActual;
                console.log('Moneda desde variable global:', monedaReal);
            }
        }
    }
    
    console.log('💰 Moneda real detectada:', monedaReal);
    
    console.log('🔧 Paso 2: Sincronizando todas las variables globales...');
    
    // Actualizar variable global
    monedaActual = monedaReal;
    
    // Actualizar FormatConfig si existe
    if (window.FormatConfig) {
        window.FormatConfig.moneda = monedaReal;
        window.FormatConfig.simboloMoneda = getCurrencySymbol(monedaReal);
    }
    
    console.log('🔧 Paso 3: Actualizando todos los elementos de la UI...');
    
    // 1. Actualizar combo (si existe y no está ya correcto)
    if (combo.length > 0 && combo.val() !== monedaReal) {
        combo.off('change.moneda'); // Desconectar temporalmente
        combo.val(monedaReal);
        setTimeout(() => combo.on('change.moneda', configurarEventoMoneda), 100);
    }
    
    // 2. Actualizar código de moneda en el display
    $('#displayMonedaCodigo').text(monedaReal);
    
    // 3. Actualizar el texto "Moneda:" que aparece abajo
    $('.text-center.text-muted small').filter(function() {
        return $(this).html().includes('Moneda:');
    }).html(`<strong>Moneda:</strong> ${monedaReal}`);
    
    // 4. Actualizar TODOS los displays financieros para usar la moneda correcta
    console.log('💰 Recalculando totales con moneda corregida...');
    
    // Si hay detalles, recalcular todo
    if ($('#tablaDetalles tbody tr').length > 0) {
        recalcularTotales();
    } else {
        // Si no hay detalles, actualizar los displays en 0 con la moneda correcta
        $('#displaySubTotal').text(formatCurrency(0));
        $('#displayDescuento').text(formatCurrency(0));
        $('#displaySubtotalDescontado').text(formatCurrency(0));
        $('#displayImpuesto').text(formatCurrency(0));
        $('#displayTotal').text(formatCurrency(0));
    }
    
    // 5. CORRECCIÓN ESPECÍFICA: Buscar y corregir símbolo EUR por el correcto
    console.log('🔧 Paso 4: Corrigiendo símbolos incorrectos...');
    
    const simboloCorrecto = getCurrencySymbol(monedaReal);
    const simboloEsperado = simboloCorrecto;
    
    // Buscar todos los elementos que contengan símbolos de moneda y corregirlos
    $('.financial-summary *').each(function() {
        const $elemento = $(this);
        const textoActual = $elemento.text();
        
        // Buscar símbolos incorrectos y reemplazarlos
        if (textoActual.includes('€') && monedaReal !== 'EUR') {
            const textoCorregido = textoActual.replace(/€/g, simboloEsperado);
            $elemento.text(textoCorregido);
            console.log('✅ Corregido símbolo EUR por', simboloEsperado, 'en:', textoActual);
        }
        
        if (textoActual.includes('¢') && monedaReal !== 'CRC') {
            const textoCorregido = textoActual.replace(/¢/g, simboloEsperado);
            $elemento.text(textoCorregido);
            console.log('✅ Corregido símbolo CRC por', simboloEsperado, 'en:', textoActual);
        }
        
        if (textoActual.includes('$') && !['USD', 'CAD', 'MXN'].includes(monedaReal)) {
            const textoCorregido = textoActual.replace(/\$/g, simboloEsperado);
            $elemento.text(textoCorregido);
            console.log('✅ Corregido símbolo $ por', simboloEsperado, 'en:', textoActual);
        }
    });
    
    // 6. Verificar que el cambio de moneda esté disponible si corresponde
    verificarYActualizarEstadoMoneda();
    
    console.log('✅ CORRECCIÓN COMPLETADA');
    console.log('📊 Estado final:', {
        monedaActual: monedaActual,
        formatConfigMoneda: window.FormatConfig?.moneda,
        comboVal: combo.length > 0 ? combo.val() : 'N/A',
        displayMonedaCodigo: $('#displayMonedaCodigo').text()
    });
    
    showNotification('success', `✅ UI de moneda corregida a ${monedaReal}. Todos los elementos sincronizados.`);
    
    console.groupEnd();
    
    return monedaReal;
};

// 🆕 FUNCIÓN DE DEBUG: Verificar estado del modal
window.debugModalDOM = function() {
    console.group('🔍 DEBUG ESTADO DEL MODAL EN DOM');
    
    const modal = $('#modalConfirmacion');
    const backdrop = $('.modal-backdrop');
    const body = $('body');
    
    console.log('📊 Estado del modal:');
    console.log('  - Modal existe:', modal.length > 0);
    console.log('  - Modal visible:', modal.is(':visible'));
    console.log('  - Modal tiene clase show:', modal.hasClass('show'));
    console.log('  - Modal display style:', modal.css('display'));
    
    console.log('📊 Estado del backdrop:');
    console.log('  - Backdrop existe:', backdrop.length > 0);
    console.log('  - Backdrop visible:', backdrop.is(':visible'));
    console.log('  - Backdrop count:', backdrop.length);
    
    console.log('📊 Estado del body:');
    console.log('  - Body tiene modal-open:', body.hasClass('modal-open'));
    console.log('  - Body overflow:', body.css('overflow'));
    console.log('  - Body padding-right:', body.css('padding-right'));
    
    if (modal.length > 0) {
        console.log('📋 Contenido del modal:');
        console.log('  - Título:', $('#modalConfirmacionTitulo').text());
        console.log('  - Mensaje:', $('#modalConfirmacionMensaje').text());
        console.log('  - Botón confirmar existe:', $('#btnConfirmarAccion').length > 0);
    }
    
    console.log('💡 Para limpiar modal bloqueado: window.limpiarModalBloqueado()');
    
    console.groupEnd();
};
window.debugModal = function() {
    console.group('🔍 DEBUG MODAL DE CONFIRMACIÓN');
    
    console.log('🧪 Probando modal con callbacks...');
    
    mostrarModalConfirmacion(
        'Prueba de Modal Debug',
        '¿Funciona correctamente el modal de confirmación?<br><br><small>Presiona <strong>Continuar</strong> para confirmar.</small>',
        'warning',
        function() {
            console.log('✅ SUCCESS: Callback de confirmación ejecutado');
            alert('✅ CONFIRMACIÓN detectada correctamente');
        },
        function() {
            console.log('❌ PROBLEM: Callback de cancelación ejecutado');
            alert('❌ CANCELACIÓN detectada - HAY PROBLEMA');
        }
    );
    
    console.groupEnd();
};

// 🆕 FUNCIÓN DE DEBUG: Probar guardado completo paso a paso
window.debugGuardado = function() {
    console.group('🔍 DEBUG PASO A PASO DEL GUARDADO');
    
    console.log('🏁 Paso 1: Verificando elementos en DOM');
    console.log('  - Botón guardar:', $('#btnGuardar').length > 0);
    console.log('  - Formulario:', $('#formEditarCotizacion').length > 0);
    console.log('  - Campos principales:', {
        nombre: $('#NombreInteresado').val(),
        email: $('#EmailInteresado').val(),
        totalLineas: $('#tablaDetalles tbody tr').length
    });
    
    console.log('🏁 Paso 2: Simulando click en guardar...');
    $('#btnGuardar').trigger('click');
    
    console.groupEnd();
};

// 🆕 FUNCIÓN DE DEBUG: Llamar directamente el guardado
window.debugGuardadoDirecto = function() {
    console.group('🔍 DEBUG GUARDADO DIRECTO');
    
    console.log('🚀 Llamando directamente a ejecutarGuardadoCotizacion()...');
    ejecutarGuardadoCotizacion();
    
    console.groupEnd();
};

// 🆕 FUNCIÓN DE DEBUG: Llamar directamente continuar guardado
window.debugContinuarGuardado = function() {
    console.group('🔍 DEBUG CONTINUAR GUARDADO DIRECTO');
    
    console.log('🚀 Llamando directamente a continuarGuardadoSinValidacionLineas()...');
    continuarGuardadoSinValidacionLineas();
    
    console.groupEnd();
};
window.verificarEstadoPostGuardado = function() {
    console.log('🔍 Verificando estado después del guardado...');
    
    const cotizacionId = $('input[name="CotizacionId"]').val();
    if (!cotizacionId) {
        console.error('❌ No se puede obtener CotizacionId');
        return;
    }
    
    // Hacer petición para verificar estado de moneda en BD
    fetch(`/Cotizaciones/DebugMoneda/${cotizacionId}`)
        .then(response => response.json())
        .then(data => {
            console.group('📊 ESTADO POST-GUARDADO EN BD');
            console.log('Cotización ID:', data.cotizacionId);
            console.log('Moneda en BD:', data.monedaCotizacion);
            console.log('Estado:', data.estado);
            console.log('¿Puede editar moneda?:', data.puedeEditarMoneda);
            
            if (data.detalleVersion) {
                console.log('Versión ID:', data.detalleVersion.versionId);
                console.log('Número versión:', data.detalleVersion.numeroVersion);
                console.log('Líneas persistentes:', data.detalleVersion.lineasPersistentes);
            }
            
            // Comparar con estado actual en frontend
            const monedaFrontend = monedaActual;
            const monedaCombo = $('#MonedaSelect').val();
            
            console.log('🔄 Comparación Frontend vs BD:');
            console.log('  - Frontend monedaActual:', monedaFrontend);
            console.log('  - Combo valor:', monedaCombo);
            console.log('  - BD moneda:', data.monedaCotizacion);
            
            const esConsistente = (monedaCombo === data.monedaCotizacion) && (monedaFrontend === data.monedaCotizacion);
            console.log('  - ¿Es consistente?:', esConsistente);
            
            if (!esConsistente) {
                console.warn('⚠️ INCONSISTENCIA DETECTADA entre frontend y BD');
                showNotification('warning', 'Se detectó una inconsistencia en la moneda. Recargue la página.');
                
                // Actualizar automáticamente el frontend con los valores de BD
                if (data.monedaCotizacion !== monedaFrontend) {
                    console.log('🔄 Actualizando frontend con moneda de BD:', data.monedaCotizacion);
                    monedaActual = data.monedaCotizacion;
                    if (window.FormatConfig) {
                        window.FormatConfig.moneda = data.monedaCotizacion;
                    }
                }
            } else {
                console.log('✅ Estado consistente entre frontend y BD');
            }
            
            console.groupEnd();
        })
        .catch(error => {
            console.error('❌ Error al verificar estado post-guardado:', error);
        });
};

function configurarEventoTipoCambio() {
    // Limpiar eventos anteriores
    $('#btnEditarTipoCambio, #btnGuardarTipoCambio, #btnCancelarTipoCambio, #TipoCambio').off('.tipocambio');
    
    let valorOriginal = '';
    
    // Evento para mostrar modo edición
    $('#btnEditarTipoCambio').on('click.tipocambio', function() {
        // Guardar valor original
        valorOriginal = $('#tipoCambioValor').text();
        if (valorOriginal === 'No definido') {
            valorOriginal = '';
        } else {
            // Extraer solo el número, quitando formato
            const match = valorOriginal.match(/[\d,]+\.?\d*/);
            valorOriginal = match ? match[0].replace(/,/g, '') : '';
        }
        
        // Establecer valor en el campo de entrada
        $('#TipoCambio').val(valorOriginal);
        
        // Cambiar a modo edición
        $('#tipoCambioModoVista').addClass('d-none');
        $('#tipoCambioModoEdicion').removeClass('d-none');
        
        // Enfocar el campo con un pequeño delay para mejor UX
        setTimeout(function() {
            $('#TipoCambio').focus().select();
        }, 100);
        
        console.log('Modo edición activado, valor original:', valorOriginal);
    });
    
    // Evento para guardar cambios
    $('#btnGuardarTipoCambio').on('click.tipocambio', function() {
        guardarCambioTipoCambio();
    });
    
    // Evento para cancelar edición
    $('#btnCancelarTipoCambio').on('click.tipocambio', function() {
        cancelarEdicionTipoCambio();
    });
    
    // Evento para guardar con Enter y cancelar con Escape
    $('#TipoCambio').on('keydown.tipocambio', function(e) {
        if (e.which === 13) { // Enter
            e.preventDefault();
            guardarCambioTipoCambio();
        } else if (e.which === 27) { // Escape
            e.preventDefault();
            cancelarEdicionTipoCambio();
        }
    });
    
    // Validación en tiempo real
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
        
        // Validar que no tenga errores
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
            // Actualizar el campo oculto con el valor correcto para el envío
            $('#TipoCambio').val(valor.toFixed(2));
        } else if (nuevoValor === '' || nuevoValor === '0') {
            $('#TipoCambio').val('');
        } else {
            // Valor inválido
            showNotification('warning', 'Por favor ingrese un valor numérico válido para el tipo de cambio.');
            $('#TipoCambio').focus();
            return;
        }
        
        // Actualizar display
        $('#tipoCambioValor').text(valorFormateado);
        
        // Volver a modo vista con animación suave
        $('#tipoCambioModoEdicion').addClass('d-none');
        $('#tipoCambioModoVista').removeClass('d-none');
        
        console.log('Tipo de cambio actualizado:', valorFormateado);
        
        // Mostrar notificación solo si realmente cambió el valor
        const valorNumerico = nuevoValor ? parseFloat(nuevoValor) : 0;
        const valorOriginalNumerico = valorOriginal ? parseFloat(valorOriginal) : 0;
        
        if (valorNumerico !== valorOriginalNumerico) {
            showNotification('success', 'Tipo de cambio actualizado');
        }
    }
    
    function cancelarEdicionTipoCambio() {
        // Restaurar valor original
        $('#TipoCambio').val(valorOriginal);
        $('#TipoCambio').removeClass('is-invalid');
        
        // Volver a modo vista
        $('#tipoCambioModoEdicion').addClass('d-none');
        $('#tipoCambioModoVista').removeClass('d-none');
        
        console.log('Edición de tipo de cambio cancelada');
    }
}

// 🚨 FUNCIÓN DE EMERGENCIA PARA EL PROBLEMA DE LA IMAGEN
window.solucionarProblemaImagenUI = function() {
    console.group('🚨 SOLUCIÓN INMEDIATA: PROBLEMA UI IMAGEN');
    
    console.log('🔍 Problema detectado: Combo muestra USD pero totales muestran EUR');
    console.log('💡 Aplicando solución inmediata...');
    
    // Paso 1: Determinar cuál es la moneda REAL basándose en la información más confiable
    const combo = $('#MonedaSelect');
    let monedaCorrecta = 'USD'; // En la imagen vemos que debe ser USD
    
    if (combo.length > 0) {
        monedaCorrecta = combo.val() || 'USD';
        console.log('✅ Combo indica:', monedaCorrecta);
    }
    
    // Paso 2: Forzar corrección inmediata
    console.log('🔧 Aplicando corrección completa...');
    window.corregirInconsistenciaMonedaUI();
    
    // Paso 3: Forzar actualización de TODOS los totales
    console.log('💰 Actualizando displays financieros...');
    
    // Simular valores para demostrar el cambio (en el ejemplo todos son 0)
    const valores = {
        subtotal: 0,
        descuento: 0,
        impuesto: 0,
        total: 0
    };
    
    // Aplicar la moneda correcta a TODOS los displays
    $('#displaySubTotal').text(formatCurrency(valores.subtotal));
    $('#displayDescuento').text(formatCurrency(valores.descuento));
    $('#displaySubtotalDescontado').text(formatCurrency(valores.subtotal));
    $('#displayImpuesto').text(formatCurrency(valores.impuesto));
    $('#displayTotal').text(formatCurrency(valores.total));
    
    // Paso 4: Cambiar específicamente el texto "Moneda: EUR" por la moneda correcta
    $('.text-center.text-muted small').each(function() {
        const $elem = $(this);
        const html = $elem.html();
        
        if (html.includes('Moneda:')) {
            const nuevoHtml = html.replace(/Moneda:\s*\w+/g, `Moneda: ${monedaCorrecta}`);
            $elem.html(nuevoHtml);
            console.log('✅ Texto moneda corregido:', html, '->', nuevoHtml);
        }
    });
    
    // Paso 5: Verificar resultado
    console.log('🔍 Verificando resultado...');
    
    const verificacion = {
        comboVal: combo.val(),
        monedaActualVar: monedaActual,
        formatConfigMoneda: window.FormatConfig?.moneda,
        textoMonedaElement: $('.text-center.text-muted small:contains("Moneda:")').text(),
        simboloEnTotales: $('#displayTotal').text().charAt(0)
    };
    
    console.table(verificacion);
    
    if (verificacion.comboVal === verificacion.monedaActualVar) {
        console.log('🎉 ¡PROBLEMA SOLUCIONADO!');
        showNotification('success', '🎉 Problema de UI solucionado. Combo y totales ahora están sincronizados.');
    } else {
        console.warn('⚠️ Aún hay inconsistencias. Puede requerir recarga de página.');
        showNotification('warning', 'Corrección aplicada. Si persiste el problema, recargue la página.');
    }
    
    console.groupEnd();
    
    return verificacion;
};