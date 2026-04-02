// ========================================
// COTIZACIONES EDITAR - JavaScript
// ========================================

// Variables globales
let productosDisponibles = []; // Se cargará dinámicamente desde ERP vía Select2
let monedaActual = 'CRC';
let detalleEditandoIndex = -1;
let estadoActual = 'B'; // Estado actual de la cotización
let esNuevaCotizacion = false; // Indica si es una nueva cotización

$(document).ready(function() {
if (typeof $ === 'undefined') {
    console.error('jQuery no está disponible');
    return;
}
    
if (typeof $.fn.CardWidget === 'undefined') {
    console.warn('AdminLTE CardWidget no está disponible');
}
    
// ✅ La inicialización de window.cotizacionGuardada se hace en inicializarVista()
    
// ⚠️ LIMITACIÓN DEL NAVEGADOR: beforeunload requiere mensaje nativo
// Los navegadores modernos NO permiten usar modales personalizados en beforeunload
// Este evento solo se dispara para: cerrar tab, cerrar ventana, refresh (F5)
// Para navegación interna (links, botones), usamos modal Bootstrap más abajo
$(window).on('beforeunload', function(e) {
    if (verificarCambiosSinGuardar()) {
        // Mensaje genérico (navegadores modernos muestran su propio texto)
        const mensaje = 'Tiene cambios sin guardar que se perderán';
        e.returnValue = mensaje;
        return mensaje;
    }
});
    
    // ✅ NAVEGACIÓN INTERNA: Usar modal Bootstrap (mejor UX)
    // Interceptar clicks en enlaces para mostrar confirmación con modal
    $(document).on('click', 'a[href]:not(.btn-guardar):not([data-toggle])', function(e) {
        const href = $(this).attr('href');
        
        if (href && href !== '#' && !href.startsWith('#') && href !== window.location.href) {
            const hayCambios = verificarCambiosSinGuardar();
            console.log('🔍 Click en enlace detectado:', {
                href: href,
                hayCambios: hayCambios,
                cotizacionGuardada: window.cotizacionGuardada
            });
            
            if (hayCambios) {
                e.preventDefault();
                confirmarSalidaConCambios(href);
                return false;
            }
        }
    });
    
    // Botón específico de "Volver al Listado" (mayor prioridad)
    $('a[href*="/Cotizaciones"]:contains("Volver al Listado"), a[href="/Cotizaciones"], a[href$="/Cotizaciones/Index"]').on('click', function(e) {
        const href = $(this).attr('href');
        const hayCambios = verificarCambiosSinGuardar();
        console.log('🔍 Click en "Volver al Listado":', {
            href: href,
            hayCambios: hayCambios,
            cotizacionGuardada: window.cotizacionGuardada
        });
        
        if (hayCambios) {
            e.preventDefault();
            confirmarSalidaConCambios(href);
            return false;
        }
    });
    
    // Detectar cambios en campos editables
    $('textarea[name="Notas"]').on('input', function() {
        window.cotizacionGuardada = false;
    });
    
    // Marcar cambios al interactuar con líneas de detalle
    $(document).on('click', '#btnAgregarLinea, .btn-editar-detalle, .btn-eliminar-detalle, #btnGuardarDetalle', function() {
        window.cotizacionGuardada = false;
        console.log('🔄 Interacción con líneas de detalle: marcando como cambios sin guardar');
    });
    
    // Detectar cambios en versión y tipo de cambio
    $(document).on('change', '#NumeroVersion, #TipoCambio', function() {
        window.cotizacionGuardada = false;
    });
    
    // ✅ REMOVER el listener global de moneda - se maneja dinámicamente
    // $(document).on('change', '#MonedaSelect', function() {
    //     window.cotizacionGuardada = false;
    // });
    
    inicializarVista();
    configurarEventos();
    cargarDatosTemporales();
});

// ========================================
// INICIALIZACIÓN Y CONFIGURACIÓN
// ========================================

function inicializarVista() {
if (window.FormatConfig) {
    monedaActual = window.FormatConfig.moneda || 'CRC';
    estadoActual = window.FormatConfig.estado || 'B';
    esNuevaCotizacion = window.FormatConfig.esNuevaCotizacion || false;
} else {
    monedaActual = $('#formEditarCotizacion').find('input[name="Moneda"]').val() || 'CRC';
    const estadoBadge = $('.header-title .badge').text().trim();
    estadoActual = detectarEstadoDeTexto(estadoBadge);
    esNuevaCotizacion = false;
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
    
    // ✅ INICIALIZAR ESTADO DE GUARDADO según el contexto
    if (esNuevaCotizacion) {
        // Para nuevas cotizaciones, comenzar sin cambios pendientes
        window.cotizacionGuardada = true;
    } else {
        // Para ediciones, comenzar sin cambios pendientes (estado inicial)
        window.cotizacionGuardada = true;
    }
    
    console.log('💾 Estado inicial de guardado:', window.cotizacionGuardada);
    
    // Almacenar valores originales para detección de cambios
    const $notas = $('textarea[name="Notas"]');
    if ($notas.length > 0) {
        $notas.data('original-value', $notas.val().trim());
    }
    
    // ✅ ALMACENAR MONEDA ORIGINAL para detección de cambios
    window._monedaOriginal = monedaActual;
    console.log('💾 Moneda original almacenada:', window._monedaOriginal);
    
    // Almacenar valor original de la versión
    const versionTexto = $('#versionValor').text().trim();
    const matchVersion = versionTexto.match(/v?(\d+\.\d+)/);
    window._versionOriginal = matchVersion ? matchVersion[1] : '1.0';
    
    // ✅ MEJORADO: Inicialización del tipo de cambio original
    const $tipoCambio = $('#TipoCambio');
    if ($tipoCambio.length > 0) {
        $tipoCambio.data('original-value', $tipoCambio.val());
        window._tipoCambioOriginal = $tipoCambio.val();
    } else {
        // Si no existe el input, leer del display
        const tipoCambioTexto = $('#tipoCambioValor').text().trim();
        if (tipoCambioTexto && tipoCambioTexto !== 'No definido') {
            const match = tipoCambioTexto.match(/[\d,]+\.?\d*/);
            const valor = match ? match[0].replace(/,/g, '') : '';
            window._tipoCambioOriginal = valor;
        } else {
            // ✅ NUEVO: Si es nueva cotización y hay tipo de cambio en FormatConfig, usarlo
            if (esNuevaCotizacion && window.FormatConfig && window.FormatConfig.tipoCambio) {
                window._tipoCambioOriginal = window.FormatConfig.tipoCambio.toString();
                // Actualizar el display también
                const tipoCambioFormateado = parseFloat(window.FormatConfig.tipoCambio).toLocaleString('en-US', {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });
                $('#tipoCambioValor').text(tipoCambioFormateado);
                console.log('💱 Inicializando tipo de cambio para nueva cotización:', window.FormatConfig.tipoCambio);
            } else {
                window._tipoCambioOriginal = '';
            }
        }
    }
    
    console.log('💱 Tipo de cambio original almacenado:', window._tipoCambioOriginal);
    
    setTimeout(function() {
        verificarYActualizarEstadoMoneda();
    }, 500); // Aumentar el delay para asegurar que el DOM esté listo
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
        
        // Nota: El evento de cambio de producto ahora lo maneja Select2 en inicializarSelect2Productos()
        
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
    // Los productos se cargarán dinámicamente desde el ERP mediante Select2 con AJAX
    const esEditable = (typeof window.FormatUtils !== 'undefined') ? 
        window.FormatUtils.isEditable(estadoActual) : 
        (estadoActual === 'B');
    
    if (esEditable) {
        // Inicializar Select2 con búsqueda remota al ERP
        inicializarSelect2Productos();
    }
}

/**
 * Inicializa Select2 en el campo de productos con búsqueda remota al ERP
 */
function inicializarSelect2Productos() {
    const $select = $('#modalProductoId');
    
    // Verificar que el elemento existe
    if ($select.length === 0) {
        console.error('❌ El elemento #modalProductoId no existe en el DOM');
        return;
    }
    
    // Verificar si Select2 ya está inicializado y destruirlo si es necesario
    if ($select.hasClass('select2-hidden-accessible')) {
        console.log('🔄 Destruyendo Select2 existente...');
        $select.select2('destroy');
    }
    
    console.log('✅ Inicializando Select2 en #modalProductoId...');
    
    // Configurar Select2 con AJAX
    $select.select2({
        theme: 'bootstrap4',
        placeholder: 'Busque un producto por código o descripción...',
        allowClear: true,
        dropdownParent: $('#modalEditarDetalle'), // IMPORTANTE: Asociar al modal
        language: {
            inputTooShort: function() {
                return 'Ingrese al menos 2 caracteres para buscar';
            },
            noResults: function() {
                return 'No se encontraron productos';
            },
            searching: function() {
                return 'Buscando productos en ERP...';
            },
            errorLoading: function() {
                return 'Error al cargar productos';
            }
        },
        minimumInputLength: 2,
        ajax: {
            url: '/Cotizaciones/BuscarProductosErp',
            dataType: 'json',
            delay: 400, // Debounce de 400ms
            data: function(params) {
                // Obtener moneda actual de la cotización
                const moneda = obtenerMonedaActual();
                
                console.log('🔍 Buscando productos:', {
                    moneda: moneda,
                    textoBusqueda: params.term
                });
                
                return {
                    moneda: moneda,
                    textoBusqueda: params.term
                };
            },
            processResults: function(response) {
                console.log('📦 Respuesta del servidor:', response);
                
                if (!response.success) {
                    showNotification('error', response.message || 'Error al buscar productos');
                    return { results: [] };
                }
                
                // Mapear respuesta al formato de Select2
                const productos = response.data.map(function(producto) {
                    return {
                        id: producto.value,
                        text: producto.text,
                        precio: producto.precio,
                        porcentajeImpuesto: producto.porcentajeImpuesto,
                        impuesto: producto.impuesto
                    };
                });
                
                console.log('✅ Productos mapeados:', productos.length, 'items');
                
                return { results: productos };
            },
            cache: true
        },
        escapeMarkup: function(markup) {
            return markup;
        },
        templateResult: formatProductoResult,
        templateSelection: formatProductoSelection
    });
    
    console.log('✅ Select2 inicializado correctamente');
    
    // Evento al seleccionar un producto
    $select.on('select2:select', function(e) {
        const data = e.params.data;
        console.log('✅ Producto seleccionado:', data);
        
        if (data) {
            // Auto-llenar descripción
            if (data.text) {
                // Extraer solo la descripción (después del guión)
                const partes = data.text.split(' - ');
                const descripcion = partes.length > 1 ? partes.slice(1).join(' - ') : data.text;
                $('#modalProductoNombre').val(descripcion.trim());
            }
            
            // Auto-llenar precio
            if (data.precio !== null && data.precio !== undefined) {
                $('#modalPrecioUnitario').val(data.precio);
            } else {
                $('#modalPrecioUnitario').val('0');
            }
            
            // Obtener porcentaje de impuesto según configuración del sistema
            const porcentajeImpuesto = obtenerPorcentajeImpuesto(data.porcentajeImpuesto);
            $('#modalPorcentajeImpuesto').val(porcentajeImpuesto);
            
            console.log(`💰 Impuesto configurado: ${porcentajeImpuesto}% (ERP: ${data.porcentajeImpuesto || 'N/A'})`);
            
            // Recalcular total de línea
            calcularTotalLinea();
        }
    });
    
    // Evento al limpiar selección
    $select.on('select2:clear', function() {
        console.log('🧹 Limpiando selección de producto');
        $('#modalProductoNombre').val('');
        $('#modalPrecioUnitario').val('');
        calcularTotalLinea();
    });
    
    // Evento al abrir el dropdown
    $select.on('select2:open', function() {
        console.log('📂 Dropdown de Select2 abierto');
    });
}

/**
 * Formatea el resultado del producto en el dropdown de Select2
 */
function formatProductoResult(producto) {
    if (producto.loading) {
        return producto.text;
    }
    
    // Separar código y descripción
    const partes = producto.text.split(' - ');
    const codigo = partes[0] || '';
    const descripcion = partes.length > 1 ? partes.slice(1).join(' - ') : '';
    
    const precioFormateado = producto.precio ? formatCurrency(producto.precio) : 'N/A';
    
    const $resultado = $(
        '<div class="select2-result-producto">' +
            '<div class="select2-result-producto__codigo">' + codigo + '</div>' +
            '<div class="select2-result-producto__descripcion">' + descripcion + '</div>' +
            '<div class="select2-result-producto__precio">Precio: ' + precioFormateado + '</div>' +
        '</div>'
    );
    
    return $resultado;
}

/**
 * Formatea la selección del producto en el campo de Select2
 */
function formatProductoSelection(producto) {
    if (!producto.id) {
        return producto.text;
    }
    
    // Mostrar solo código - descripción en el campo seleccionado
    return producto.text;
}

// ========================================
// GESTIÓN DE LÍNEAS DE DETALLE
// ========================================

/**
 * Obtiene el porcentaje de impuesto a usar según configuración del sistema
 * 
 * @param {number|null|undefined} porcentajeErp - Porcentaje de impuesto proveniente del ERP
 * @returns {number} Porcentaje de impuesto a aplicar
 * 
 * Lógica:
 * - Si ERP_USAR_IMPUESTOS = "S" y porcentajeErp tiene valor → usar porcentajeErp
 * - Si ERP_USAR_IMPUESTOS = "N" → usar TASA_IMPUESTO del sistema
 * - Si porcentajeErp es null/undefined → fallback a TASA_IMPUESTO del sistema
 */
function obtenerPorcentajeImpuesto(porcentajeErp) {
    // Obtener configuración desde window.ConfigImpuestos (configurado en la vista Razor)
    const usarImpuestosErp = window.ConfigImpuestos?.usarImpuestosErp || 'S';
    const tasaImpuesto = parseFloat(window.ConfigImpuestos?.tasaImpuesto) || 13.0;
    
    // Si se debe usar impuestos del ERP y viene un valor válido del ERP
    if (usarImpuestosErp === 'S' && porcentajeErp !== null && porcentajeErp !== undefined) {
        const porcentaje = parseFloat(porcentajeErp);
        if (!isNaN(porcentaje) && porcentaje >= 0) {
            console.log(`✅ Usando porcentaje del ERP: ${porcentaje}%`);
            return porcentaje;
        }
    }
    
    // Fallback: usar tasa de impuesto del sistema
    console.log(`📋 Usando tasa de impuesto del sistema: ${tasaImpuesto}%`);
    return tasaImpuesto;
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
        const porcentajeImpuesto = parseFloat(fila.find('input[name$=".PorcentajeImpuesto"]').val()) || 0;
        
        // Almacenar valores originales para detectar cambios (solo si no existen)
        const detalleVersionId = parseInt(detalleId) || 0;
        if (detalleVersionId > 0) {
            // Intentar leer desde el atributo data-valores-originales
            let valoresOriginales = fila.data('valores-originales');
            
            // Si no existe en jQuery data, intentar desde el atributo HTML
            if (!valoresOriginales) {
                const atributoJson = fila.attr('data-valores-originales');
                if (atributoJson) {
                    try {
                        valoresOriginales = JSON.parse(atributoJson);
                        fila.data('valores-originales', valoresOriginales);
                    } catch (e) {
                        console.warn('Error parseando valores originales:', e);
                    }
                }
            }
            
            // Si aún no existen, usar los valores actuales como originales
            if (!valoresOriginales) {
                valoresOriginales = {
                    productoId: productoId,
                    productoNombre: productoNombre,
                    cantidad: cantidad,
                    precio: precio,
                    descuento: descuento
                };
                fila.data('valores-originales', valoresOriginales);
            }
        }
        
        // Configurar Select2 con el producto actual
        const $selectProducto = $('#modalProductoId');
        if ($selectProducto.hasClass('select2-hidden-accessible')) {
            // Agregar opción si no existe y establecer valor
            if (productoId && $selectProducto.find(`option[value="${productoId}"]`).length === 0) {
                const textoCompleto = `${productoId} - ${productoNombre}`;
                const newOption = new Option(textoCompleto, productoId, true, true);
                $selectProducto.append(newOption).trigger('change');
            } else {
                $selectProducto.val(productoId).trigger('change');
            }
        } else {
            // Fallback si Select2 no está inicializado
            if (productoId && $selectProducto.find(`option[value="${productoId}"]`).length === 0) {
                $selectProducto.append(`<option value="${productoId}" selected>${productoId} - ${productoNombre}</option>`);
            }
            $selectProducto.val(productoId);
        }
        
        $('#modalProductoNombre').val(productoNombre);
        $('#modalCantidad').val(cantidad);
        $('#modalPrecioUnitario').val(precio);
        $('#modalDescuento').val(descuento);
        $('#modalPorcentajeImpuesto').val(porcentajeImpuesto);
        
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
    const porcentajeImpuesto = parseFloat($('#modalPorcentajeImpuesto').val()) || 0;
    
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
            porcentajeImpuesto: porcentajeImpuesto,
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
            porcentajeImpuesto: porcentajeImpuesto,
            totalLinea: totalLinea
        });
    }
    
    // Recalcular totales (esto llamará a verificarYActualizarEstadoMoneda)
    recalcularTotales();
    
    // Marcar como cambios sin guardar
    window.cotizacionGuardada = false;
    
    // Cerrar modal
    $('#modalEditarDetalle').modal('hide');
    
    showNotification('success', 'Detalle guardado correctamente');
}

function actualizarFilaDetalle(index, datos) {
    const fila = $(`tr[data-index="${index}"]`);
    const detalleVersionId = parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) || 0;
    
    // Actualizar displays visuales
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
    
    // Actualizar inputs ocultos
    fila.find('input[name$=".ProductoId"]').val(datos.productoId);
    fila.find('input[name$=".ProductoNombre"]').val(datos.productoNombre);
    fila.find('input[name$=".Cantidad"]').val(datos.cantidad.toString());
    fila.find('input[name$=".PrecioUnitario"]').val(datos.precioUnitario.toString());
    fila.find('input[name$=".Descuento"]').val(datos.descuento.toString());
    fila.find('input[name$=".PorcentajeImpuesto"]').val(datos.porcentajeImpuesto.toString());
    fila.find('input[name$=".TotalLinea"]').val(datos.totalLinea.toString());
    
    // Detectar si la línea fue modificada (solo para líneas persistentes)
    if (detalleVersionId > 0) {
        const valoresOriginales = fila.data('valores-originales');
        
        // Si no hay valores originales almacenados, almacenarlos ahora
        if (!valoresOriginales) {
            fila.data('valores-originales', {
                productoId: datos.productoId,
                productoNombre: datos.productoNombre,
                cantidad: datos.cantidad,
                precio: datos.precioUnitario,
                descuento: datos.descuento
            });
        } else {
            // Comparar valores actuales con originales
            const fueModificada = (
                valoresOriginales.productoId !== datos.productoId ||
                valoresOriginales.productoNombre !== datos.productoNombre ||
                valoresOriginales.cantidad !== datos.cantidad ||
                valoresOriginales.precio !== datos.precioUnitario ||
                valoresOriginales.descuento !== datos.descuento
            );
            
            if (fueModificada) {
                // Marcar línea como modificada
                fila.removeClass('linea-nueva linea-persistente').addClass('linea-modificada');
                
                // Actualizar o agregar indicador visual
                let indicadorExistente = fila.find('.indicador-estado');
                
                if (indicadorExistente.length > 0) {
                    indicadorExistente.replaceWith('<i class="fas fa-edit text-warning indicador-estado" title="Línea modificada"></i>');
                } else {
                    // Agregar indicador si no existe
                    const contenedorCodigo = fila.find('td:first-child .d-flex');
                    if (contenedorCodigo.length > 0) {
                        contenedorCodigo.append('<span class="ml-2"><i class="fas fa-edit text-warning indicador-estado" title="Línea modificada"></i></span>');
                    }
                }
            } else {
                // No fue modificada, mantener como persistente sin indicador
                fila.removeClass('linea-nueva linea-modificada').addClass('linea-persistente');
                fila.find('.indicador-estado').closest('span').remove();
            }
        }
    }
}

function agregarNuevaFilaDetalle(datos) {
    const tbody = $('#tablaDetalles tbody');
    const nuevoIndex = tbody.find('tr').length;
    
    const esLineaNueva = datos.detalleVersionId === 0;
    const claseIndicador = esLineaNueva ? 'linea-nueva' : 'linea-persistente';
    
    // Determinar indicador visual
    let iconoEstado = '';
    if (esLineaNueva) {
        // Línea nueva (no guardada en BD)
        iconoEstado = '<i class="fas fa-plus-circle text-success indicador-estado" title="Línea nueva"></i>';
    }
    // Si es línea persistente, no mostrar indicador inicialmente (se mostrará al editar)
    
    const nuevaFila = `
        <tr data-detalle-id="${datos.detalleVersionId}" data-index="${nuevoIndex}" class="${claseIndicador}">
            <td>
                <div class="d-flex align-items-center">
                    <strong class="text-primary">${datos.productoId}</strong>
                    ${iconoEstado ? `<span class="ml-2">${iconoEstado}</span>` : ''}
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
                <input type="hidden" name="Detalles[${nuevoIndex}].PorcentajeImpuesto" value="${datos.porcentajeImpuesto.toString()}" />
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
    
    // ✅ IMPORTANTE: Verificar estado de moneda después de agregar línea
    setTimeout(() => {
        verificarYActualizarEstadoMoneda();
    }, 100);
}

function eliminarDetalle(index) {
    // ✅ USAR MODAL BOOTSTRAP en lugar de confirm() nativo
    mostrarModalConfirmacion(
        'Eliminar Detalle',
        '¿Está seguro de que desea eliminar esta línea de detalle?<br><br>' +
        '<small class="text-muted">Esta acción no se puede deshacer al guardar.</small>',
        'danger',
        function() {
            ejecutarEliminacionDetalle(index);
        },
        null, // No necesita callback de cancelación
        {
            btnTextoConfirmar: 'Eliminar',
            btnTextoCancelar: 'Cancelar'
        }
    );
}

function ejecutarEliminacionDetalle(index) {
    const fila = $(`tr[data-index="${index}"]`);
    const detalleVersionId = parseInt(fila.find('input[name$=".DetalleVersionId"]').val()) || 0;
    const productoNombre = fila.find('.producto-nombre').text();
    
    // ✅ IMPORTANTE: Marcar como cambios sin guardar ANTES de eliminar
    window.cotizacionGuardada = false;
    console.log('🗑️ ANTES de eliminar: marcando como cambios sin guardar');
    
    if (detalleVersionId > 0) {
        showNotification('info', `Línea persistente "${productoNombre}" marcada para eliminación. Se eliminará de BD al guardar.`);
    }
    
    fila.remove();
    reindexarFilasDetalle();
    recalcularTotales();
    
    // ✅ REFORZAR: Asegurar que se mantenga como cambios sin guardar
    window.cotizacionGuardada = false;
    console.log('🗑️ DESPUÉS de eliminar: confirmando cambios sin guardar');
    
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
    
    // ✅ VERIFICAR ESTADO DESPUÉS DE ELIMINAR para actualizar detección de cambios
    setTimeout(() => {
        verificarYActualizarEstadoMoneda();
        // Asegurar que el estado se mantiene como sin guardar
        if (window.cotizacionGuardada !== false) {
            window.cotizacionGuardada = false;
            console.log('🗑️ FORZANDO estado sin guardar después de eliminar');
        }
    }, 100);
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
    let totalImpuestos = 0;
    
    $('#tablaDetalles tbody tr').each(function() {
        const cantidad = parseFloat($(this).find('input[name$=".Cantidad"]').val()) || 0;
        const precio = parseFloat($(this).find('input[name$=".PrecioUnitario"]').val()) || 0;
        const descuento = parseFloat($(this).find('input[name$=".Descuento"]').val()) || 0;
        const porcentajeImpuesto = parseFloat($(this).find('input[name$=".PorcentajeImpuesto"]').val()) || 0;
        
        const subtotalLinea = cantidad * precio;
        subtotal += subtotalLinea;
        totalDescuentos += descuento;
        
        // Calcular impuesto de esta línea
        const baseImponible = subtotalLinea - descuento;
        const impuestoLinea = baseImponible * (porcentajeImpuesto / 100);
        totalImpuestos += impuestoLinea;
    });
    
    const subtotalConDescuentos = subtotal - totalDescuentos;
    const total = subtotalConDescuentos + totalImpuestos;
    
    $('#displaySubTotal').text(formatCurrency(subtotal));
    $('#displayDescuento').text(formatCurrency(totalDescuentos));
    $('#displaySubtotalDescontado').text(formatCurrency(subtotalConDescuentos));
    $('#displayImpuesto').text(formatCurrency(totalImpuestos));
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
    
    // Limpiar Select2
    const $selectProducto = $('#modalProductoId');
    if ($selectProducto.hasClass('select2-hidden-accessible')) {
        $selectProducto.val(null).trigger('change');
    } else {
        $selectProducto.val('');
    }
    
    $('#modalProductoNombre').val('');
    $('#modalCantidad').val('1');
    $('#modalPrecioUnitario').val('');
    $('#modalDescuento').val('0');
    $('#modalPorcentajeImpuesto').val('0');
    $('#modalTotalLinea').text(formatCurrency(0));
    $('#detalleIndex').val('');
    $('#detalleVersionId').val('');
    $('#formEditarDetalle .is-invalid').removeClass('is-invalid');
    $('#formEditarDetalle .invalid-feedback').remove();
}

function actualizarContadorLineas() {
    const totalLineas = $('#tablaDetalles tbody tr').length;
    
    // Texto simplificado: solo mostrar total de líneas
    const textoContador = totalLineas === 0 ? '0 líneas' : 
                         totalLineas === 1 ? '1 línea' : 
                         `${totalLineas} líneas`;
    
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

// ========================================
// GUARDADO DE COTIZACIÓN
// ========================================

function guardarCotizacion() {
    if (estadoActual !== 'B') {
        showNotification('error', 'No se puede guardar. Solo las cotizaciones en estado Borrador pueden ser editadas.');
        return;
    }
    
    // Verificar si es creación o edición
    if (esNuevaCotizacion) {
        ejecutarGuardadoCreacion();
    } else {
        ejecutarGuardadoCotizacion();
    }
}

function ejecutarGuardadoCreacion() {
    const btn = $('#btnGuardar');
    
    // Leer valores desde los spans de solo lectura
    let nombreInteresado = $('#NombreInteresado').text().trim();
    if (nombreInteresado === 'No asignado') {
        nombreInteresado = '';
    }
    
    const totalLineas = $('#tablaDetalles tbody tr').length;
    const erroresValidacion = [];
    
    // Validación ESTRICTA para creación: interesado obligatorio
    if (!nombreInteresado) {
        erroresValidacion.push('• El nombre del interesado es obligatorio');
    }
    
    // Validación ESTRICTA para creación: al menos una línea obligatoria
    if (totalLineas === 0) {
        erroresValidacion.push('• Debe agregar al menos una línea de producto');
    }
    
    if (erroresValidacion.length > 0) {
        const mensajeError = '<strong>No se puede crear la cotización por los siguientes errores:</strong><br><br>' +
                           erroresValidacion.join('<br>') +
                           '<br><br><small class="text-muted">Por favor corrija estos campos y vuelva a intentar.</small>';
        
        // ✅ MODAL BOOTSTRAP para mostrar errores de validación
        mostrarModalConfirmacion(
            'Errores de Validación',
            mensajeError,
            'warning',
            function() {
                // Al cerrar, enfocar el primer campo con error
                if (!nombreInteresado) {
                    $('#btnBuscarHubSpot').focus();
                } else if (totalLineas === 0) {
                    $('#btnAgregarLinea').focus();
                }
            },
            null,
            {
                btnTextoConfirmar: 'Entendido',
                btnTextoCancelar: 'Cerrar'
            }
        );
        return;
    }
    
    continuarGuardadoCreacion();
}

function continuarGuardadoCreacion() {
    const btn = $('#btnGuardar');
    btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Creando...');
    
    try {
        // Leer valores desde los spans de solo lectura
        let nombreInteresado = $('#NombreInteresado').text().trim();
        if (nombreInteresado === 'No asignado') {
            nombreInteresado = '';
        }
            
        let emailInteresado = $('#EmailInteresado').text().trim();
        if (emailInteresado === 'No asignado') {
            emailInteresado = '';
        }
            
        let empresaInteresado = $('#EmpresaInteresado').text().trim();
        if (empresaInteresado === 'No asignado') {
            empresaInteresado = '';
        }
            
        const formData = {
            NombreInteresado: nombreInteresado,
            EmailInteresado: emailInteresado,
            EmpresaInteresado: empresaInteresado,
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
            const inputPorcentajeImpuesto = fila.find('input[name$=".PorcentajeImpuesto"]');
            const inputTotal = fila.find('input[name$=".TotalLinea"]');
            
            const detalle = {
                ProductoId: inputProductoId.val() || fila.find(`input[name="Detalles[${index}].ProductoId"]`).val() || '',
                ProductoNombre: inputProductoNombre.val() || fila.find(`input[name="Detalles[${index}].ProductoNombre"]`).val() || '',
                Cantidad: parseFloat(inputCantidad.val() || fila.find(`input[name="Detalles[${index}].Cantidad"]`).val() || 0),
                PrecioUnitario: parseFloat(inputPrecio.val() || fila.find(`input[name="Detalles[${index}].PrecioUnitario"]`).val() || 0),
                Descuento: parseFloat(inputDescuento.val() || fila.find(`input[name="Detalles[${index}].Descuento"]`).val() || 0),
                PorcentajeImpuesto: parseFloat(inputPorcentajeImpuesto.val() || fila.find(`input[name="Detalles[${index}].PorcentajeImpuesto"]`).val() || 0),
                TotalLinea: parseFloat(inputTotal.val() || fila.find(`input[name="Detalles[${index}].TotalLinea"]`).val() || 0)
            };
            
            if (detalle.ProductoId && detalle.ProductoId !== '') {
                formData.Detalles.push(detalle);
            }
        });
            
        const monedaSeleccionada = obtenerMonedaActual();
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
            url: '/Cotizaciones/GuardarCreacion',
            type: 'POST',
            data: formData,
            success: function(response) {
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar');
                
                if (response.success) {
                    showNotification('success', response.message || 'Cotización creada exitosamente');
                    marcarComoGuardado();
                    
                    // Redirigir a la vista de edición de la nueva cotización
                    setTimeout(function() {
                        if (response.cotizacionId) {
                            window.location.href = `/Cotizaciones/Editor/${response.cotizacionId}`;
                        } else {
                            window.location.href = '/Cotizaciones';
                        }
                    }, 2000);
                } else {
                    showNotification('error', response.message || 'Error al crear la cotización');
                }
            },
            error: function(xhr, status, error) {
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar');
                
                let errorMessage = 'Error de comunicación con el servidor';
                
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                } else if (xhr.status === 403) {
                    errorMessage = 'No tiene permisos para realizar esta operación';
                } else if (xhr.status === 404) {
                    errorMessage = 'Servicio no encontrado';
                } else if (xhr.status >= 500) {
                    errorMessage = 'Error interno del servidor';
                }
                
                showNotification('error', errorMessage);
            }
        });
        
    } catch (error) {
        btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar');
        showNotification('error', 'Error al procesar los datos de la cotización');
    }
}

function ejecutarGuardadoCotizacion() {
    const btn = $('#btnGuardar');
    
    // Leer valores desde los spans de solo lectura
    let nombreInteresado = $('#NombreInteresado').text().trim();
    if (nombreInteresado === 'No asignado') {
        nombreInteresado = '';
    }
    
    const totalLineas = $('#tablaDetalles tbody tr').length;
    const erroresValidacion = [];
    
    // Validar solo el nombre del interesado (el email no es obligatorio)
    if (!nombreInteresado) {
        erroresValidacion.push('• El nombre del interesado es obligatorio');
    }
    
    if (totalLineas === 0) {
        // ✅ MODAL BOOTSTRAP para confirmación de guardado sin productos
        mostrarModalConfirmacion(
            'Cotización Sin Productos',
            '¿Está seguro de que desea guardar la cotización sin líneas de productos?<br><br>' +
            '<small class="text-muted">La cotización se guardará como borrador y podrá agregar productos más tarde.</small>',
            'warning',
            function() {
                continuarGuardadoSinValidacionLineas();
            },
            function() {
                // Usuario canceló, restaurar botón
                btn.prop('disabled', false).html('<i class="fas fa-save"></i> Guardar Cambios');
            },
            {
                btnTextoConfirmar: 'Guardar Sin Productos',
                btnTextoCancelar: 'Cancelar'
            }
        );
        return;
    }
    
    if (erroresValidacion.length > 0) {
        const mensajeError = '<strong>No se puede guardar por los siguientes errores:</strong><br><br>' +
                           erroresValidacion.join('<br>') +
                           '<br><br><small class="text-muted">Por favor corrija estos campos y vuelva a intentar.</small>';
        
        // ✅ MODAL BOOTSTRAP para mostrar errores de validación (no es confirmación, es informativo)
        mostrarModalConfirmacion(
            'Errores de Validación',
            mensajeError,
            'warning',
            function() {
                // Al cerrar, enfocar el primer campo con error
                $('#NombreInteresado').focus();
            },
            null,
            {
                btnTextoConfirmar: 'Entendido',
                btnTextoCancelar: 'Cerrar'
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
        // Leer valores desde los spans de solo lectura
        let nombreInteresado = $('#NombreInteresado').text().trim();
        if (nombreInteresado === 'No asignado') {
            nombreInteresado = '';
        }
        
        let emailInteresado = $('#EmailInteresado').text().trim();
        if (emailInteresado === 'No asignado') {
            emailInteresado = '';
        }
        
        let empresaInteresado = $('#EmpresaInteresado').text().trim();
        if (empresaInteresado === 'No asignado') {
            empresaInteresado = '';
        }
        
        const formData = {
            CotizacionId: $('input[name="CotizacionId"]').val(),
            VersionId: parseInt($('input[name="VersionId"]').val()),
            NombreInteresado: nombreInteresado,
            EmailInteresado: emailInteresado,
            EmpresaInteresado: empresaInteresado,
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
            const inputPorcentajeImpuesto = fila.find('input[name$=".PorcentajeImpuesto"]');
            const inputTotal = fila.find('input[name$=".TotalLinea"]');
            
            const detalle = {
                DetalleVersionId: parseInt(inputDetalleId.val() || fila.find(`input[name="Detalles[${index}].DetalleVersionId"]`).val()) || 0,
                ProductoId: inputProductoId.val() || fila.find(`input[name="Detalles[${index}].ProductoId"]`).val() || '',
                ProductoNombre: inputProductoNombre.val() || fila.find(`input[name="Detalles[${index}].ProductoNombre"]`).val() || '',
                Cantidad: parseFloat(inputCantidad.val() || fila.find(`input[name="Detalles[${index}].Cantidad"]`).val() || 0),
                PrecioUnitario: parseFloat(inputPrecio.val() || fila.find(`input[name="Detalles[${index}].PrecioUnitario"]`).val() || 0),
                Descuento: parseFloat(inputDescuento.val() || fila.find(`input[name="Detalles[${index}].Descuento"]`).val() || 0),
                PorcentajeImpuesto: parseFloat(inputPorcentajeImpuesto.val() || fila.find(`input[name="Detalles[${index}].PorcentajeImpuesto"]`).val() || 0),
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

// ========================================
// UTILIDADES Y FORMATEO
// ========================================

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

/**
 * Muestra modal de confirmación usando la función global (si está disponible)
 * o implementación local como fallback
 */
function mostrarModalConfirmacion(titulo, mensaje, tipo, onConfirm, onCancel, opciones) {
    // Usar función global si está disponible (recomendado)
    if (typeof window.mostrarModalConfirmacionGlobal === 'function') {
        window.mostrarModalConfirmacionGlobal(titulo, mensaje, tipo, onConfirm, onCancel, opciones);
        return;
    }
    
    // Fallback a implementación local si la global no está disponible
    const modal = $('#modalConfirmacion');
    if (modal.length === 0) {
        usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    // Limpiar eventos anteriores
    modal.off('.localconfirm');
    $('#btnConfirmarAccion').off('.localconfirm');
    
    // Configuración visual
    const config = obtenerConfiguracionModal(tipo);
    const btnTextoConfirmar = (opciones && opciones.btnTextoConfirmar) || config.btnTexto;
    
    $('#modalConfirmacionHeader')
        .removeClass('bg-info bg-warning bg-danger bg-success bg-primary text-white text-dark')
        .addClass(config.headerClass);
    $('#modalConfirmacionTitulo').html(`<i class="fas ${config.icono}"></i> ${titulo}`);
    $('#modalConfirmacionMensaje').html(mensaje);
    
    const btnConfirmar = $('#btnConfirmarAccion');
    btnConfirmar.removeClass('btn-info btn-warning btn-danger btn-success btn-primary')
                .addClass(config.btnClass)
                .html(`<i class="fas fa-check"></i> ${btnTextoConfirmar}`);
    
    let accionConfirmada = false;
    
    // Evento de confirmación
    btnConfirmar.on('click.localconfirm', function(e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        accionConfirmada = true;
        modal.modal('hide');
    });
    
    // Evento al cerrar
    modal.one('hidden.bs.modal.localconfirm', function() {
        if (accionConfirmada && typeof onConfirm === 'function') {
            setTimeout(onConfirm, 150);
        } else if (!accionConfirmada && typeof onCancel === 'function') {
            setTimeout(onCancel, 150);
        }
        
        modal.off('.localconfirm');
        btnConfirmar.off('.localconfirm');
    });
    
    // Mostrar modal
    modal.modal({ backdrop: 'static', keyboard: false, show: true });
}

/**
 * Fallback a confirmación nativa del navegador
 * ⚠️ SOLO SE USA SI: jQuery, Bootstrap o el modal no están disponibles
 * En condiciones normales, SIEMPRE se debe usar el modal Bootstrap
 */
function usarConfirmacionNativa(titulo, mensaje, onConfirm, onCancel) {
    console.warn('⚠️ Usando confirmación nativa como fallback (modal Bootstrap no disponible)');
    
    // Convertir HTML a texto plano más inteligentemente
    const mensajeTexto = mensaje
        .replace(/<br\s*\/?>/gi, '\n')           // <br> a salto de línea
        .replace(/<\/p>\s*<p>/gi, '\n\n')        // Párrafos separados
        .replace(/<strong>(.*?)<\/strong>/gi, '$1') // Quitar tags strong
        .replace(/<[^>]*>/g, '')                   // Quitar resto de HTML
        .replace(/\s+/g, ' ')                      // Normalizar espacios
        .trim();
    
    const textoCompleto = `${titulo}\n\n${mensajeTexto}`;
    
    // setTimeout para evitar bloqueo del navegador
    setTimeout(function() {
        const confirmacion = confirm(textoCompleto);
        
        if (confirmacion && typeof onConfirm === 'function') {
            onConfirm();
        } else if (!confirmacion && typeof onCancel === 'function') {
            onCancel();
        }
    }, 10);
}

/**
 * Obtiene la configuración visual del modal según el tipo
 * Mantiene consistencia visual en toda la aplicación
 */
function obtenerConfiguracionModal(tipo) {
    const configuraciones = {
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
        },
        'exit': {
            headerClass: 'bg-warning text-dark',
            icono: 'fa-sign-out-alt',
            btnClass: 'btn-warning',
            btnTexto: 'Salir Sin Guardar'
        }
    };
    
    return configuraciones[tipo] || configuraciones['info'];
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

/**
 * Muestra modal de confirmación para cambio de moneda
 * Usa la función global unificada en lugar de implementación específica
 */
function mostrarModalConfirmacionMoneda(titulo, mensaje, tipo, onConfirm, onCancel) {
    // ✅ USAR FUNCIÓN GLOBAL UNIFICADA - parámetros simplificados
    mostrarModalConfirmacion(
        titulo,
        mensaje,
        tipo,
        onConfirm,
        onCancel,
        {
            btnTextoConfirmar: 'Cambiar Moneda',
            btnTextoCancelar: 'Cancelar'
        }
    );
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
    let totalImpuestos = 0;
    
    $('#tablaDetalles tbody tr').each(function() {
        const cantidad = parseFloat($(this).find('input[name$=".Cantidad"]').val()) || 0;
        const precio = parseFloat($(this).find('input[name$=".PrecioUnitario"]').val()) || 0;
        const descuento = parseFloat($(this).find('input[name$=".Descuento"]').val()) || 0;
        const porcentajeImpuesto = parseFloat($(this).find('input[name$=".PorcentajeImpuesto"]').val()) || 0;
        
        const subtotalLinea = cantidad * precio;
        const baseImponible = subtotalLinea - descuento;
        const impuestoLinea = baseImponible * (porcentajeImpuesto / 100);
        totalImpuestos += impuestoLinea;
    });
    
    return totalImpuestos;
}

function calcularTotalFinalActual() {
    const subtotal = calcularSubtotalActual();
    const descuentos = calcularTotalDescuentos();
    const impuesto = calcularImpuestoActual();
    return (subtotal - descuentos) + impuesto;
}

/**
 * Muestra modal de confirmación cuando el usuario intenta salir con cambios sin guardar
 * @param {string} urlDestino - URL a la que se dirigirá si confirma (opcional)
 */
function confirmarSalidaConCambios(urlDestino) {
    const hayCambios = verificarCambiosSinGuardar();
    
    if (!hayCambios) {
        // No hay cambios, permitir navegación
        if (urlDestino) {
            window.location.href = urlDestino;
        }
        return true;
    }
    
    // Determinar URL de destino
    const urlFinal = urlDestino || '/Cotizaciones';
    
    // Mensajes diferentes según si es creación o edición
    let titulo, mensaje;
    if (esNuevaCotizacion) {
        titulo = 'Cancelar Creación';
        mensaje = '¿Está seguro de que desea cancelar la creación de la cotización?<br><br>' +
                 '<strong class="text-danger">Se perderá toda la información ingresada.</strong><br><br>' +
                 '<small class="text-muted">Puede hacer clic en "Guardar" para crear la cotización antes de salir.</small>';
    } else {
        titulo = 'Cambios Sin Guardar';
        mensaje = '¿Está seguro de que desea salir sin guardar los cambios?<br><br>' +
                 '<strong class="text-danger">Se perderán todos los cambios realizados.</strong><br><br>' +
                 '<small class="text-muted">Puede hacer clic en "Guardar Cambios" antes de salir para no perder su trabajo.</small>';
    }
    
    // ✅ USAR MODAL BOOTSTRAP en lugar de confirm() nativo
    mostrarModalConfirmacion(
        titulo,
        mensaje,
        'exit',
        function() {
            // Usuario confirmó: permitir salida
            marcarComoGuardado(); // Evitar que se dispare beforeunload
            window.location.href = urlFinal;
        },
        function() {
            // Usuario canceló: quedarse en la página
            // No hacer nada, el modal se cierra automáticamente
        },
        {
            btnTextoConfirmar: esNuevaCotizacion ? 'Cancelar Creación' : 'Salir Sin Guardar',
            btnTextoCancelar: 'Quedarme Aquí'
        }
    );
    
    return false;
}

// ========================================
// GESTIÓN DE CAMBIOS SIN GUARDAR
// ========================================

function verificarCambiosSinGuardar() {
    if (seGuardoRecientemente()) {
        return false;
    }
    
    // Verificar solo cambios reales que requieren guardado
    const notasActuales = $('textarea[name="Notas"]').val().trim();
    const notasOriginales = $('textarea[name="Notas"]').data('original-value') || '';
    
    // ✅ MEJORAR DETECCIÓN DE CAMBIO DE MONEDA
    const monedaComboActual = $('#MonedaSelect').val();
    const monedaOriginalGuardada = window._monedaOriginal || monedaActual;
    
    // Obtener valor actual y original de la versión
    const versionEditada = $('#NumeroVersion').val();
    const versionOriginal = window._versionOriginal || $('#versionValor').text().replace('v', '');
    
    // Obtener valor actual y original del tipo de cambio
    const tipoCambioActual = $('#TipoCambio').val();
    const tipoCambioOriginal = $('#TipoCambio').data('original-value') || window._tipoCambioOriginal || '';
    
    // Verificar líneas nuevas
    let lineasNuevas = 0;
    $('#tablaDetalles tbody tr').each(function() {
        const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
        if (detalleVersionId === 0) {
            lineasNuevas++;
        }
    });
    
    // Verificar líneas modificadas (que tienen clase linea-modificada)
    const lineasModificadas = $('#tablaDetalles tbody tr.linea-modificada').length;
    
    // ✅ VERIFICACIÓN MEJORADA DE CAMBIO DE MONEDA
    let cambioMoneda = false;
    if (monedaComboActual && monedaComboActual !== monedaOriginalGuardada) {
        cambioMoneda = true;
        console.log('🔍 Detectado cambio de moneda:', {
            actual: monedaComboActual,
            original: monedaOriginalGuardada,
            monedaActualVariable: monedaActual
        });
    }
    
    // ✅ VERIFICAR SI HAY LÍNEAS ELIMINADAS PENDIENTES DE GUARDAR
    // Si window.cotizacionGuardada es false, significa que hubo cambios
    const hayLineasEliminadas = (window.cotizacionGuardada === false);
    
    const hayCambios = (
        notasActuales !== notasOriginales ||
        lineasNuevas > 0 ||
        lineasModificadas > 0 ||
        cambioMoneda ||
        (versionEditada && versionEditada !== versionOriginal) ||
        (tipoCambioActual !== tipoCambioOriginal) ||
        hayLineasEliminadas // ✅ INCLUIR LÍNEAS ELIMINADAS
    );
    
    console.log('🔍 Verificar cambios:', {
        notasCambiaron: notasActuales !== notasOriginales,
        lineasNuevas: lineasNuevas,
        lineasModificadas: lineasModificadas,
        cambioMoneda: cambioMoneda,
        versionCambio: versionEditada && versionEditada !== versionOriginal,
        tipoCambioCambio: tipoCambioActual !== tipoCambioOriginal,
        hayLineasEliminadas: hayLineasEliminadas,
        cotizacionGuardada: window.cotizacionGuardada,
        hayCambios: hayCambios
    });
    
    return hayCambios;
}

function marcarComoGuardado() {
    window.cotizacionGuardada = true;
    
    // Actualizar valores originales después de guardar exitosamente
    const $notas = $('textarea[name="Notas"]');
    if ($notas.length > 0) {
        $notas.data('original-value', $notas.val().trim());
    }
    
    // ✅ ACTUALIZAR MONEDA ORIGINAL después de guardar
    window._monedaOriginal = monedaActual;
    console.log('💾 Moneda original actualizada después de guardar:', window._monedaOriginal);
    
    // Actualizar valor original de la versión
    const versionTexto = $('#versionValor').text().trim();
    const matchVersion = versionTexto.match(/v?(\d+\.\d+)/);
    window._versionOriginal = matchVersion ? matchVersion[1] : '1.0';
    
    // ✅ MEJORADO: Actualizar valor original del tipo de cambio
    const tipoCambioActual = $('#TipoCambio').val();
    if (tipoCambioActual) {
        $('#TipoCambio').data('original-value', tipoCambioActual);
        window._tipoCambioOriginal = tipoCambioActual;
    } else {
        // Si no hay input, leer del display
        const tipoCambioTexto = $('#tipoCambioValor').text().trim();
        if (tipoCambioTexto && tipoCambioTexto !== 'No definido') {
            const match = tipoCambioTexto.match(/[\d,]+\.?\d*/);
            const valor = match ? match[0].replace(/,/g, '') : '';
            window._tipoCambioOriginal = valor;
        }
    }
    
    console.log('💱 Tipo de cambio original actualizado después de guardar:', window._tipoCambioOriginal);
}

function seGuardoRecientemente() {
    // ✅ SOLO considerar "guardado recientemente" si explícitamente se marcó como guardado
    // AND no se han hecho cambios posteriores
    const guardadoExplicitamente = (window.cotizacionGuardada === true);
    console.log('🔍 ¿Se guardó recientemente?:', {
        cotizacionGuardada: window.cotizacionGuardada,
        guardadoExplicitamente: guardadoExplicitamente
    });
    return guardadoExplicitamente;
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

// ========================================
// GESTIÓN DE MONEDA
// ========================================

/**
 * ✅ FUNCIÓN AUXILIAR: Procesar cambio de moneda
 */
function procesarCambioMoneda(nuevaMoneda) {
    monedaActual = nuevaMoneda;
    
    if (window.FormatConfig) {
        window.FormatConfig.moneda = nuevaMoneda;
        window.FormatConfig.simboloMoneda = getCurrencySymbol(nuevaMoneda);
    }
    
    actualizarDisplaysMoneda();
    window.cotizacionGuardada = false;
    
    showNotification('success', 
        `Moneda cambiada a ${obtenerNombreMoneda(nuevaMoneda)}. ` +
        `Debe Guardar la cotización para aplicar el cambio.`);
    
    // ✅ Reconfigurar eventos después del cambio
    setTimeout(() => {
        configurarEventoMoneda();
    }, 100);
}

/**
 * ✅ FUNCIÓN AUXILIAR: Revertir selección de moneda
 */
function revertirSeleccionMoneda($combo, monedaAnterior) {
    $combo.val(monedaAnterior);
    $combo[0].selectedIndex = $combo.find(`option[value="${monedaAnterior}"]`).index();
    
    // ✅ Reconfigurar eventos después de revertir
    setTimeout(() => {
        configurarEventoMoneda();
    }, 100);
}

function aplicarCambioMoneda(nuevaMoneda) {
    // ✅ USAR LA FUNCIÓN AUXILIAR
    procesarCambioMoneda(nuevaMoneda);
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
    
    // ✅ NUEVA LÓGICA: Para creaciones nuevas, mostrar combo siempre (sin líneas persistentes)
    // Para ediciones, aplicar la lógica existente
    let puedeEditarMoneda = false;
    
    if (esNuevaCotizacion) {
        // En creación: siempre puede cambiar moneda (no hay líneas persistentes)
        puedeEditarMoneda = (estadoActual === 'B');
    } else {
        // En edición: solo si está en borrador Y no tiene líneas persistentes
        let lineasPersistentes = 0;
        $('#tablaDetalles tbody tr').each(function() {
            const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
            if (detalleVersionId > 0) {
                lineasPersistentes++;
            }
        });
        puedeEditarMoneda = (estadoActual === 'B' && lineasPersistentes === 0);
    }
    
    if (puedeEditarMoneda) {
        // ✅ SOLO mostrar combo si no existe o necesita actualizarse
        mostrarComboMoneda();
    } else {
        // ✅ SOLO ocultar combo si actualmente es visible
        const seccionMoneda = $('.moneda-section');
        if (seccionMoneda.length > 0 && seccionMoneda.children().length > 0) {
            mostrarDisplayMoneda('restricciones');
        }
    }
    
    // ✅ SOLO configurar tipo de cambio si está en borrador
    if (estadoActual === 'B') {
        // Verificar si el evento ya está configurado para evitar duplicación
        if ($('#btnEditarTipoCambio').data('eventos-configurados') !== true) {
            configurarEventoTipoCambio();
        }
    }
}

function mostrarComboMoneda() {
    const seccionMoneda = $('.moneda-section');
    if (seccionMoneda.length === 0) {
        return;
    }
    
    const monedaActualReal = obtenerMonedaActual();
    
    // ✅ VERIFICAR si ya existe un combo funcional
    const comboExistente = $('#MonedaSelect');
    if (comboExistente.length > 0 && comboExistente.val() === monedaActualReal) {
        // Ya existe y tiene el valor correcto, solo configurar eventos si es necesario
        if (!comboExistente.data('eventos-configurados')) {
            configurarEventoMoneda();
        }
        return;
    }
    
    // ✅ GENERAR combo solo si es necesario
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
                Puede cambiar la moneda.
            </small>
        </div>
    `;
    
    seccionMoneda.html(comboHTML);
    
    // ✅ CONFIGURAR eventos solo UNA VEZ después de crear el combo
    setTimeout(() => {
        configurarEventoMoneda();
    }, 50);
}

function mostrarDisplayMoneda(razon) {
    const seccionMoneda = $('.moneda-section');
    if (seccionMoneda.length === 0) {
        return;
    }
    
    seccionMoneda.html('');
}

function configurarEventoMoneda() {
    const $combo = $('#MonedaSelect');
    
    // ✅ Si no existe el combo, no configurar eventos
    if ($combo.length === 0) {
        return;
    }
    
    // ✅ LIMPIAR completamente todos los eventos del combo
    $combo.off();
    
    // ✅ Agregar una marca para evitar configuración múltiple
    if ($combo.data('eventos-configurados') === true) {
        return;
    }
    
    // ✅ CONFIGURAR UN SOLO LISTENER simple
    $combo.on('change.moneda', function(e) {
        const nuevaMoneda = $(this).val();
        const monedaAnterior = monedaActual;
        
        // ✅ Evitar procesamiento si no hay cambio real
        if (nuevaMoneda === monedaAnterior) {
            return;
        }
        
        // ✅ Marcar como cambios sin guardar inmediatamente
        window.cotizacionGuardada = false;
        
        // ✅ DETENER el evento inmediatamente para evitar bucles
        $(this).off('change.moneda');
        $(this).data('eventos-configurados', false);
        
        // ✅ Para nuevas cotizaciones: cambio directo
        if (esNuevaCotizacion) {
            procesarCambioMoneda(nuevaMoneda);
            return;
        }
        
        // ✅ Para ediciones: verificar líneas persistentes
        let lineasPersistentes = 0;
        $('#tablaDetalles tbody tr').each(function() {
            const detalleVersionId = parseInt($(this).find('input[name$=".DetalleVersionId"]').val()) || 0;
            if (detalleVersionId > 0) {
                lineasPersistentes++;
            }
        });
        
        if (lineasPersistentes > 0) {
            showNotification('warning', 'No se puede cambiar la moneda cuando hay líneas guardadas en la base de datos. Elimine todas las líneas persistentes primero.');
            revertirSeleccionMoneda($(this), monedaAnterior);
            return;
        }
        
        // ✅ Mostrar modal de confirmación
        const nombreMonedaNueva = obtenerNombreMoneda(nuevaMoneda);
        const nombreMonedaAnterior = obtenerNombreMoneda(monedaAnterior);
        
        mostrarModalConfirmacionMoneda(
            'Confirmar Cambio de Moneda',
            `¿Está seguro de que desea cambiar la moneda de <strong>${nombreMonedaAnterior}</strong> a <strong>${nombreMonedaNueva}</strong>?<br><br>
             <small class="text-muted">Este cambio es posible porque no hay líneas guardadas en la base de datos.</small>`,
            'warning',
            function() {
                // Usuario confirmó
                procesarCambioMoneda(nuevaMoneda);
            },
            function() {
                // Usuario canceló
                revertirSeleccionMoneda($combo, monedaAnterior);
            }
        );
    });
    
    // ✅ Marcar como configurado
    $combo.data('eventos-configurados', true);
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
            showNotification('success', `Versión actualizada a ${displayFormateado}. Recuerde Guardar la cotización para aplicar el cambio.`);
            // Marcar que hay cambios sin guardar
            window.cotizacionGuardada = false;
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

// ========================================
// CONFIGURACIÓN DE VERSIÓN Y TIPO DE CAMBIO
// ========================================

function configurarEventoTipoCambio() {
    // ✅ Verificar si ya están configurados los eventos
    if ($('#btnEditarTipoCambio').data('eventos-configurados') === true) {
        return;
    }
    
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
    
    // ✅ Marcar como configurado
    $('#btnEditarTipoCambio').data('eventos-configurados', true);
    
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
            showNotification('success', 'Tipo de cambio actualizado. Recuerde Guardar la cotización para aplicar el cambio.');
            // Marcar que hay cambios sin guardar
            window.cotizacionGuardada = false;
        }
    }
    
    function cancelarEdicionTipoCambio() {
        $('#TipoCambio').val(valorOriginal);
        $('#TipoCambio').removeClass('is-invalid');
        $('#tipoCambioModoEdicion').addClass('d-none');
        $('#tipoCambioModoVista').removeClass('d-none');
    }
}

// ========================================
// BÚSQUEDA DE INTERESADOS EN HUBSPOT
// ========================================

/**
 * Configurar eventos del modal de búsqueda de HubSpot
 */
function configurarEventosHubSpot() {
    // Botón para abrir el modal (en el header del card)
    $('#btnBuscarHubSpot').on('click', function(e) {
        e.preventDefault();
        e.stopPropagation(); // Prevenir que el evento llegue al card-header y lo colapse
        abrirModalBuscarHubSpot();
    });
    
    // Botón de búsqueda dentro del modal
    $('#btnBuscarHubSpotModal').on('click', function() {
        ejecutarBusquedaHubSpot();
    });
    
    // Búsqueda al presionar Enter en el input
    $('#textoBusquedaHubSpot').on('keypress', function(e) {
        if (e.which === 13) {
            e.preventDefault();
            ejecutarBusquedaHubSpot();
        }
    });
    
    // Cambio de tipo de interesado - limpiar resultados
    $('input[name="tipoInteresadoHubSpot"]').on('change', function() {
        limpiarResultadosHubSpot();
    });
    
    // Limpiar al cerrar el modal
    $('#modalBuscarInteresadoHubSpot').on('hidden.bs.modal', function() {
        limpiarModalBusquedaHubSpot();
    });
}

/**
 * Abre el modal de búsqueda de HubSpot
 */
function abrirModalBuscarHubSpot() {
    limpiarModalBusquedaHubSpot();
    
    // Pre-seleccionar el tipo de interesado según lo que está en el formulario
    const tipoActual = $('#TipoInteresado').val();
    if (tipoActual === 'E') {
        $('#radioEmpresa').prop('checked', true);
    } else {
        $('#radioContacto').prop('checked', true);
    }
    
    $('#modalBuscarInteresadoHubSpot').modal('show');
}

/**
 * Ejecuta la búsqueda de interesados en HubSpot
 */
function ejecutarBusquedaHubSpot() {
    const textoBusqueda = $('#textoBusquedaHubSpot').val().trim();
    
    // Validar mínimo de caracteres
    if (textoBusqueda.length < 3) {
        mostrarAlertaModal('warning', 'Por favor ingrese al menos 3 caracteres para buscar.');
        $('#textoBusquedaHubSpot').focus();
        return;
    }
    
    // Obtener tipo seleccionado
    const tipoInteresado = $('input[name="tipoInteresadoHubSpot"]:checked').val();
    
    // Mostrar spinner y ocultar resultados anteriores
    $('#spinnerBusquedaHubSpot').removeClass('d-none');
    $('#resultadosBusquedaHubSpot').addClass('d-none');
    $('#sinResultadosHubSpot').addClass('d-none');
    ocultarAlertaModal();
    
    // Obtener token antiforgery
    const token = $('input[name="__RequestVerificationToken"]').val();
    
    // Realizar búsqueda vía AJAX
    $.ajax({
        url: '/Cotizaciones/BuscarInteresadosHubSpot',
        type: 'POST',
        data: {
            tipoInteresado: tipoInteresado,
            textoBusqueda: textoBusqueda,
            __RequestVerificationToken: token
        },
        success: function(response) {
            $('#spinnerBusquedaHubSpot').addClass('d-none');
            
            if (response.success) {
                mostrarResultadosHubSpot(response.data);
            } else {
                mostrarAlertaModal('danger', response.message || 'Error al buscar interesados');
            }
        },
        error: function(xhr, status, error) {
            $('#spinnerBusquedaHubSpot').addClass('d-none');
            
            let errorMessage = 'Error de comunicación con el servidor';
            
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            } else if (xhr.status === 403) {
                errorMessage = 'No tiene permisos para realizar esta operación';
            } else if (xhr.status === 404) {
                errorMessage = 'Servicio no disponible';
            } else if (xhr.status >= 500) {
                errorMessage = 'Error interno del servidor';
            }
            
            mostrarAlertaModal('danger', errorMessage);
        }
    });
}

/**
 * Muestra los resultados de búsqueda en la tabla
 */
function mostrarResultadosHubSpot(resultados) {
    const tbody = $('#tablaResultadosHubSpot tbody');
    tbody.empty();
    
    if (!resultados || resultados.length === 0) {
        $('#sinResultadosHubSpot').removeClass('d-none');
        $('#resultadosBusquedaHubSpot').addClass('d-none');
        $('#contadorResultados').text('0');
        return;
    }
    
    // Mostrar resultados
    $('#sinResultadosHubSpot').addClass('d-none');
    $('#resultadosBusquedaHubSpot').removeClass('d-none');
    $('#contadorResultados').text(resultados.length);
    
    resultados.forEach(function(interesado) {
        const fila = `
            <tr>
                <td>
                    <strong>${escapeHtml(interesado.nombreInteresado || interesado.NombreInteresado || '')}</strong>
                </td>
                <td>
                    <span class="text-muted">
                        <i class="fas fa-envelope"></i> ${escapeHtml(interesado.emailInteresado || interesado.EmailInteresado || 'N/A')}
                    </span>
                </td>
                <td>
                    <span class="text-info">
                        <i class="fas fa-building"></i> ${escapeHtml(interesado.empresaInteresado || interesado.EmpresaInteresado || 'N/A')}
                    </span>
                </td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-success btn-seleccionar-interesado" 
                            data-hubspot-id="${escapeHtml(interesado.hubSpotObjectId || interesado.HubSpotObjectId || '')}"
                            data-hubspot-type="${escapeHtml(interesado.hubSpotObjectType || interesado.HubSpotObjectType || '')}"
                            data-tipo="${escapeHtml(interesado.tipoInteresado || interesado.TipoInteresado || 'P')}"
                            data-nombre="${escapeHtml(interesado.nombreInteresado || interesado.NombreInteresado || '')}"
                            data-email="${escapeHtml(interesado.emailInteresado || interesado.EmailInteresado || '')}"
                            data-empresa="${escapeHtml(interesado.empresaInteresado || interesado.EmpresaInteresado || '')}"
                            title="Seleccionar este interesado">
                        <i class="fas fa-check"></i> Seleccionar
                    </button>
                </td>
            </tr>
        `;
        tbody.append(fila);
    });
    
    // Configurar evento de selección
    $('.btn-seleccionar-interesado').on('click', function() {
        seleccionarInteresadoHubSpot($(this));
    });
}

/**
 * Selecciona un interesado y lo asigna a la cotización
 */
function seleccionarInteresadoHubSpot($btn) {
    const cotizacionId = $('input[name="CotizacionId"]').val();
    
    const datosInteresado = {
        cotizacionId: cotizacionId,
        hubSpotObjectId: $btn.attr('data-hubspot-id'),
        hubSpotObjectType: $btn.attr('data-hubspot-type'),
        tipoInteresado: $btn.attr('data-tipo'),
        nombreInteresado: $btn.attr('data-nombre'),
        emailInteresado: $btn.attr('data-email'),
        empresaInteresado: $btn.attr('data-empresa'),
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    };
    
    // Validaciones básicas
    if (!datosInteresado.hubSpotObjectId) {
        mostrarAlertaModal('danger', 'ID de HubSpot no disponible');
        return;
    }
    
    if (!datosInteresado.nombreInteresado) {
        mostrarAlertaModal('danger', 'Nombre del interesado no disponible');
        return;
    }
    
    // Deshabilitar botón mientras procesa
    $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Asignando...');
    
    // Realizar asignación vía AJAX
    $.ajax({
        url: '/Cotizaciones/AsignarInteresadoHubSpot',
        type: 'POST',
        data: datosInteresado,
        success: function(response) {
            if (response.success) {
                // Extraer datos de la respuesta
                const nombre = response.data.nombreInteresado || '';
                const email = response.data.emailInteresado || '';
                const empresa = response.data.empresaInteresado || '';
                const tipo = datosInteresado.tipoInteresado || 'P';
                
                // Actualizar SPAN de nombre (solo lectura visual)
                $('#NombreInteresado').text(nombre || 'No asignado');
                
                // Actualizar SPAN de email con formato HTML (SIN link mailto)
                const $emailSpan = $('#EmailInteresado');
                if (email && email !== 'N/A') {
                    $emailSpan.html(`<span class="text-info"><i class="fas fa-envelope"></i> ${escapeHtml(email)}</span>`);
                } else {
                    $emailSpan.html('<span class="text-muted">No asignado</span>');
                }
                
                // Actualizar SPAN de empresa con formato HTML
                const $empresaSpan = $('#EmpresaInteresado');
                if (empresa) {
                    $empresaSpan.html(`<span><i class="fas fa-building text-info"></i> ${escapeHtml(empresa)}</span>`);
                } else {
                    $empresaSpan.html('<span class="text-muted">No asignado</span>');
                }
                
                // Actualizar SPAN de tipo con badge
                const $tipoDisplay = $('#TipoInteresadoDisplay');
                let tipoBadge = '';
                switch(tipo) {
                    case 'P':
                        tipoBadge = '<span class="badge badge-info"><i class="fas fa-user"></i> Persona</span>';
                        break;
                    case 'E':
                        tipoBadge = '<span class="badge badge-primary"><i class="fas fa-building"></i> Empresa</span>';
                        break;
                    default:
                        tipoBadge = '<span class="badge badge-secondary"><i class="fas fa-question"></i> No definido</span>';
                }
                $tipoDisplay.html(tipoBadge);
                
                // Actualizar INPUT HIDDEN del tipo (para envío del formulario)
                $('#TipoInteresado').val(tipo);
                
                // Ocultar alerta de sugerencia si existe
                $('.card-info .alert-info').fadeOut();
                
                // Cerrar modal
                $('#modalBuscarInteresadoHubSpot').modal('hide');
                
                // Notificar éxito
                showNotification('success', 'Interesado asignado correctamente desde HubSpot');
            } else {
                mostrarAlertaModal('danger', response.message || 'Error al asignar el interesado');
                $btn.prop('disabled', false).html('<i class="fas fa-check"></i> Seleccionar');
            }
        },
        error: function(xhr, status, error) {
            let errorMessage = 'Error de comunicación con el servidor';
            
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            } else if (xhr.status === 403) {
                errorMessage = 'No tiene permisos para realizar esta operación';
            } else if (xhr.status === 404) {
                errorMessage = 'Servicio no disponible';
            } else if (xhr.status >= 500) {
                errorMessage = 'Error interno del servidor';
            }
            
            mostrarAlertaModal('danger', errorMessage);
            $btn.prop('disabled', false).html('<i class="fas fa-check"></i> Seleccionar');
        }
    });
}

/**
 * Limpia todos los campos y resultados del modal de HubSpot
 */
function limpiarModalBusquedaHubSpot() {
    $('#textoBusquedaHubSpot').val('');
    $('#radioContacto').prop('checked', true);
    limpiarResultadosHubSpot();
    ocultarAlertaModal();
}

/**
 * Limpia solo los resultados de búsqueda
 */
function limpiarResultadosHubSpot() {
    $('#tablaResultadosHubSpot tbody').empty();
    $('#resultadosBusquedaHubSpot').addClass('d-none');
    $('#sinResultadosHubSpot').addClass('d-none');
    $('#spinnerBusquedaHubSpot').addClass('d-none');
    $('#contadorResultados').text('0');
}

/**
 * Muestra una alerta dentro del modal
 */
function mostrarAlertaModal(tipo, mensaje) {
    const alert = $('#alertBuscarInteresado');
    
    // Mapeo de tipos a clases de Bootstrap
    const clases = {
        'success': 'alert-success',
        'danger': 'alert-danger',
        'warning': 'alert-warning',
        'info': 'alert-info'
    };
    
    const clase = clases[tipo] || 'alert-info';
    
    alert.removeClass('alert-success alert-danger alert-warning alert-info d-none')
         .addClass(clase)
         .html(`<i class="fas fa-exclamation-circle"></i> ${mensaje}`)
         .removeClass('d-none');
    
    // Scroll al inicio del modal para que se vea la alerta
    $('#modalBuscarInteresadoHubSpot .modal-body').scrollTop(0);
}

/**
 * Oculta la alerta del modal
 */
function ocultarAlertaModal() {
    $('#alertBuscarInteresado').addClass('d-none');
}

/**
 * Escapa caracteres HTML para evitar XSS
 */
function escapeHtml(text) {
    if (!text) return '';
    
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    
    return String(text).replace(/[&<>"']/g, function(m) { return map[m]; });
}

// Configurar eventos de HubSpot al cargar el documento
$(document).ready(function() {
    configurarEventosHubSpot();
});
