/**
 * JavaScript para la gestión de parámetros de configuración
 * Funcionalidades: validación en tiempo real, reseteo, UX mejorado
 */

$(document).ready(function() {
    initConfiguracionParametros();
});

function initConfiguracionParametros() {
    // Inicializar componentes
    initValidacionTiempoReal();
    initBotonesResetear();
    initFormSubmission();
    initTooltips();
    
    // Nuevas funcionalidades para parámetros sensitivos
    initToggleVisibilidadSensitivos();
    initRevelarSecretoBloqueado();
    
    // Nueva funcionalidad para textareas de plantilla
    initParametrosPlantilla();
    
    console.log('Sistema de configuración de parámetros inicializado');
}

/**
 * Validación en tiempo real de parámetros
 */
function initValidacionTiempoReal() {
    $('.parametro-input').on('input blur', function() {
        const $input = $(this);
        const valor = $input.val().trim();
        const tipo = $input.data('tipo');
        const codigo = $input.closest('.parametro-card').data('codigo');
        
        if (valor === '') {
            resetValidationState($input);
            return;
        }
        
        // Validación inmediata por tipo
        const validacionLocal = validarTipoLocal(valor, tipo);
        if (!validacionLocal.esValido) {
            mostrarErrorValidacion($input, validacionLocal.mensaje);
            return;
        }
        
        // Validación en servidor para reglas específicas
        if (codigo) {
            validarEnServidor(codigo, valor, tipo, $input);
        }
    });
}

/**
 * Validación local inmediata por tipo
 */
function validarTipoLocal(valor, tipo) {
    switch (tipo) {
        case 'Decimal':
            if (!/^\d+(\.\d+)?$/.test(valor)) {
                return { esValido: false, mensaje: 'Debe ser un número decimal válido' };
            }
            break;
            
        case 'Entero':
            if (!/^\d+$/.test(valor)) {
                return { esValido: false, mensaje: 'Debe ser un número entero válido' };
            }
            break;
            
        case 'Booleano':
            const valorUpper = valor.toUpperCase();
            if (!['S', 'N', 'TRUE', 'FALSE'].includes(valorUpper)) {
                return { esValido: false, mensaje: 'Debe ser S, N, true o false' };
            }
            break;
            
        case 'Fecha':
            if (isNaN(Date.parse(valor))) {
                return { esValido: false, mensaje: 'Debe ser una fecha válida' };
            }
            break;
    }
    
    return { esValido: true };
}

/**
 * Validación en servidor
 */
function validarEnServidor(codigo, valor, tipo, $input) {
    // Debounce para evitar muchas llamadas
    clearTimeout($input.data('validationTimeout'));
    
    const timeout = setTimeout(function() {
        $.ajax({
            url: '/Configuracion/ValidarParametro',
            type: 'GET',
            data: {
                codigo: codigo,
                valor: valor,
                tipo: tipo
            }
        })
        .done(function(response) {
            if (response.esValido) {
                mostrarValidacionExitosa($input);
            } else {
                mostrarErrorValidacion($input, response.mensaje);
            }
        })
        .fail(function() {
            mostrarErrorValidacion($input, 'Error al validar el parámetro');
        });
    }, 500); // 500ms de debounce
    
    $input.data('validationTimeout', timeout);
}

/**
 * Estados de validación visual
 */
function mostrarErrorValidacion($input, mensaje) {
    $input.removeClass('is-valid').addClass('is-invalid');
    
    let $feedback = $input.siblings('.invalid-feedback');
    if ($feedback.length === 0) {
        $feedback = $('<div class="invalid-feedback"></div>');
        $input.after($feedback);
    }
    $feedback.text(mensaje);
    
    // Deshabilitar botón de guardar
    $input.closest('form').find('button[type="submit"]').prop('disabled', true);
}

function mostrarValidacionExitosa($input) {
    $input.removeClass('is-invalid').addClass('is-valid');
    $input.siblings('.invalid-feedback').remove();
    
    let $feedback = $input.siblings('.valid-feedback');
    if ($feedback.length === 0) {
        $feedback = $('<div class="valid-feedback"></div>');
        $input.after($feedback);
    }
    $feedback.text('Valor válido');
    
    // Habilitar botón de guardar
    $input.closest('form').find('button[type="submit"]').prop('disabled', false);
}

function resetValidationState($input) {
    $input.removeClass('is-valid is-invalid');
    $input.siblings('.invalid-feedback, .valid-feedback').remove();
    $input.closest('form').find('button[type="submit"]').prop('disabled', false);
}

/**
 * Manejo de botones de resetear
 */
function initBotonesResetear() {
    $('.btn-resetear').on('click', function(e) {
        e.preventDefault();
        
        const codigo = $(this).data('codigo');
        $('#parametroResetear').text(codigo);
        $('#codigoResetear').val(codigo);
        $('#modalResetear').modal('show');
    });
}

/**
 * Mejorar UX del envío de formularios
 */
function initFormSubmission() {
    $('.parametro-form').on('submit', function() {
        const $form = $(this);
        const $btn = $form.find('button[type="submit"]');
        
        // Prevenir múltiples envíos
        if ($btn.hasClass('loading')) {
            return false;
        }
        
        // Marcar que estamos guardando intencionalmente
        window.isSavingForm = true;
        
        // Estado de carga
        $btn.addClass('loading');
        $form.addClass('submitting');
        
        // Timeout de seguridad
        setTimeout(function() {
            $btn.removeClass('loading');
            $form.removeClass('submitting');
        }, 5000);
    });
}

/**
 * Tooltips informativos
 */
function initTooltips() {
    $('[data-toggle="tooltip"]').tooltip();
    
    // Agregar tooltips dinámicos para tipos de datos
    $('.parametro-input').each(function() {
        const tipo = $(this).data('tipo');
        let tooltip = '';
        
        switch (tipo) {
            case 'Decimal':
                tooltip = 'Ingrese un número decimal (ej: 123.45)';
                break;
            case 'Entero':
                tooltip = 'Ingrese un número entero positivo';
                break;
            case 'Booleano':
                tooltip = 'Seleccione Sí (S) o No (N)';
                break;
            case 'Fecha':
                tooltip = 'Formato: YYYY-MM-DD o DD/MM/YYYY';
                break;
            case 'Texto':
                tooltip = 'Ingrese texto libre';
                break;
        }
        
        if (tooltip) {
            $(this).attr('title', tooltip);
            $(this).tooltip();
        }
    });
}

/**
 * Utilidades adicionales
 */

// Confirmar antes de salir si hay cambios sin guardar
function checkUnsavedChanges() {
    let hasChanges = false;
    
    $('.parametro-input').each(function() {
        const $input = $(this);
        const originalValue = $input.attr('data-original-value') || '';
        const currentValue = $input.val() || '';
        
        if (originalValue !== currentValue) {
            hasChanges = true;
            return false; // break
        }
    });
    
    return hasChanges;
}

// Guardar valores originales al cargar la página
$(window).on('load', function() {
    $('.parametro-input').each(function() {
        $(this).attr('data-original-value', $(this).val() || '');
    });
});

// ⚠️ LIMITACIÓN DEL NAVEGADOR: beforeunload requiere mensaje nativo
// Los navegadores modernos NO permiten usar modales personalizados en beforeunload
// Este evento solo se dispara para: cerrar tab, cerrar ventana, refresh (F5)
// Para navegación interna (links, botones), se debe interceptar el click
$(window).on('beforeunload', function(e) {
    // No mostrar alerta si estamos guardando el formulario
    if (window.isSavingForm) {
        return undefined;
    }
    
    if (checkUnsavedChanges()) {
        // Mensaje genérico (navegadores modernos muestran su propio texto)
        const mensaje = 'Tiene cambios sin guardar que se perderán';
        e.returnValue = mensaje;
        return mensaje;
    }
});

// ? NAVEGACIÓN INTERNA: Interceptar clicks en pestañas y enlaces
$(document).on('click', '.categoria-tab', function(e) {
    if (checkUnsavedChanges()) {
        e.preventDefault();
        const urlDestino = $(this).attr('href');
        
        // Usar modal Bootstrap para navegación interna
        if (typeof window.mostrarModalConfirmacionGlobal === 'function') {
            window.mostrarModalConfirmacionGlobal(
                'Cambios Sin Guardar',
                '¿Está seguro de que desea cambiar de categoría sin guardar los cambios?<br><br>' +
                '<strong class="text-danger">Se perderán todos los cambios realizados.</strong>',
                'exit',
                function() {
                    // Usuario confirmó: navegar
                    window.location.href = urlDestino;
                },
                null,
                {
                    btnTextoConfirmar: 'Cambiar Sin Guardar',
                    btnTextoCancelar: 'Quedarme Aquí'
                }
            );
        } else {
            // Fallback a confirm nativo si modal no disponible
            if (confirm('Tiene cambios sin guardar. ¿Está seguro de que desea salir?')) {
                window.location.href = urlDestino;
            }
        }
        
        return false;
    }
});

/**
 * Función de notificación (usa la función global de site.js)
 * ? PRINCIPIO DRY: No duplicar código, usar función centralizada
 */
function mostrarNotificacion(tipo, mensaje) {
    // Usar función global si está disponible (recomendado)
    if (typeof window.showNotification === 'function') {
        window.showNotification(tipo, mensaje);
        return;
    }
    
    // Fallback solo si la función global no está disponible
    console.warn('?? window.showNotification no disponible, usando implementación local');
    
    const alertClass = `alert-${tipo}`;
    const iconClass = tipo === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    
    const $alert = $(`
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="fas ${iconClass}"></i>
            ${mensaje}
            <button type="button" class="close" data-dismiss="alert">
                <span>&times;</span>
            </button>
        </div>
    `);
    
    $('.container-fluid').first().prepend($alert);
    
    // Auto-dismiss después de 5 segundos
    setTimeout(function() {
        $alert.alert('close');
    }, 5000);
}

/**
 * Atajos de teclado útiles
 */
$(document).on('keydown', function(e) {
    // Ctrl+S para guardar el formulario enfocado
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        const $focusedInput = $('.parametro-input:focus');
        if ($focusedInput.length) {
            $focusedInput.closest('form').submit();
        }
    }
    
    // Escape para cancelar edición
    if (e.key === 'Escape') {
        $('.parametro-input:focus').blur();
        resetValidationState($('.parametro-input'));
    }
});

// Exportar funciones para uso global
window.ConfiguracionParametros = {
    validarEnServidor,
    mostrarNotificacion,
    resetValidationState
};

// ============================================
// FUNCIONALIDADES PARA PARÁMETROS SENSITIVOS
// ============================================

/**
 * Inicializar toggle de visibilidad para inputs sensitivos editables
 */
function initToggleVisibilidadSensitivos() {
    // Botón de toggle visibilidad (ojo) para inputs password
    $('.btn-toggle-visibility').on('click', function(e) {
        e.preventDefault();
        
        const $btn = $(this);
        const $input = $btn.closest('.input-group').find('.parametro-input-sensitivo');
        const $icon = $btn.find('i');
        
        if ($input.length === 0) return;
        
        // Toggle entre password y text
        const esPassword = $input.attr('type') === 'password';
        
        if (esPassword) {
            // Mostrar valor
            $input.attr('type', 'text');
            $icon.removeClass('fa-eye').addClass('fa-eye-slash');
            $btn.addClass('active');
            $btn.attr('title', 'Ocultar valor');
        } else {
            // Ocultar valor
            $input.attr('type', 'password');
            $icon.removeClass('fa-eye-slash').addClass('fa-eye');
            $btn.removeClass('active');
            $btn.attr('title', 'Mostrar valor');
        }
        
        // Actualizar tooltip
        $btn.tooltip('dispose').tooltip();
    });
}

/**
 * Inicializar revelación de valores para parámetros sensitivos bloqueados
 */
function initRevelarSecretoBloqueado() {
    // Botón para revelar secreto de parámetros bloqueados
    $('.btn-revelar-secreto').on('click', function(e) {
        e.preventDefault();
        
        const $btn = $(this);
        const parametroId = $btn.data('parametro-id');
        const codigo = $btn.data('codigo');
        const $wrapper = $btn.closest('.valor-sensitivo-wrapper');
        const $valorDisplay = $wrapper.find('.valor-display');
        const $valorReal = $wrapper.find('.valor-real');
        
        // Verificar si ya está revelado
        const estaRevelado = $btn.hasClass('revelado');
        
        if (estaRevelado) {
            // Ocultar valor real
            ocultarValorSecreto($wrapper, $btn, $valorDisplay, $valorReal);
        } else {
            // Verificar si el valor ya fue cargado anteriormente
            const valorCacheado = $valorReal.data('valor-cacheado');
            
            if (valorCacheado) {
                // Mostrar valor cacheado
                mostrarValorSecreto($wrapper, $btn, $valorDisplay, $valorReal, valorCacheado, codigo);
            } else {
                // Cargar valor desde el servidor
                revelarValorSecreto(parametroId, codigo, $wrapper, $btn, $valorDisplay, $valorReal);
            }
        }
    });
}

/**
 * Revelar valor secreto desde el servidor
 */
function revelarValorSecreto(parametroId, codigo, $wrapper, $btn, $valorDisplay, $valorReal) {
    // Estado de loading
    $btn.addClass('loading').prop('disabled', true);
    
    $.ajax({
        url: '/Configuracion/RevelarValorSensitivo',
        type: 'GET',
        data: { parametroId: parametroId },
        dataType: 'json'
    })
    .done(function(response) {
        if (response.success && response.valor) {
            // Cachear el valor para no volver a pedirlo
            $valorReal.data('valor-cacheado', response.valor);
            
            // Mostrar valor real
            mostrarValorSecreto($wrapper, $btn, $valorDisplay, $valorReal, response.valor, codigo);
            
            // Notificación
            if (typeof window.showNotification === 'function') {
                window.showNotification('success', `Valor real de ${codigo} revelado`);
            }
        } else {
            // Error en respuesta
            const mensaje = response.message || 'No se pudo obtener el valor del parámetro';
            if (typeof window.showNotification === 'function') {
                window.showNotification('error', mensaje);
            } else {
                console.error(mensaje);
            }
        }
    })
    .fail(function(xhr, status, error) {
        // Error de servidor
        let mensaje = 'Error al revelar el valor del parámetro';
        
        if (xhr.status === 403) {
            mensaje = 'No tiene permisos para ver valores sensitivos';
        } else if (xhr.status === 404) {
            mensaje = 'Parámetro no encontrado';
        }
        
        if (typeof window.showNotification === 'function') {
            window.showNotification('error', mensaje);
        } else {
            console.error(mensaje, error);
        }
    })
    .always(function() {
        // Remover estado de loading
        $btn.removeClass('loading').prop('disabled', false);
    });
}

/**
 * Mostrar valor secreto revelado
 */
function mostrarValorSecreto($wrapper, $btn, $valorDisplay, $valorReal, valorTexto, codigo) {
    // Animar salida del valor enmascarado
    $valorDisplay.addClass('fade-out');
    
    setTimeout(function() {
        // Ocultar valor enmascarado
        $valorDisplay.addClass('d-none').removeClass('fade-out');
        
        // Mostrar valor real
        $valorReal.text(valorTexto).removeClass('d-none').addClass('fade-in');
        
        // Cambiar botón a estado "revelado"
        $btn.addClass('revelado');
        $btn.html('<i class="fas fa-eye-slash"></i> Ocultar Valor');
        $btn.attr('title', 'Ocultar valor');
        $btn.tooltip('dispose').tooltip();
        
        // Remover animación después de completar
        setTimeout(function() {
            $valorReal.removeClass('fade-in');
        }, 200);
    }, 200);
}

/**
 * Ocultar valor secreto
 */
function ocultarValorSecreto($wrapper, $btn, $valorDisplay, $valorReal) {
    // Animar salida del valor real
    $valorReal.addClass('fade-out');
    
    setTimeout(function() {
        // Ocultar valor real
        $valorReal.addClass('d-none').removeClass('fade-out');
        
        // Mostrar valor enmascarado
        $valorDisplay.removeClass('d-none').addClass('fade-in');
        
        // Cambiar botón a estado normal
        $btn.removeClass('revelado');
        $btn.html('<i class="fas fa-eye"></i> Ver Valor Real');
        $btn.attr('title', 'Revelar valor real');
        $btn.tooltip('dispose').tooltip();
        
        // Remover animación después de completar
        setTimeout(function() {
            $valorDisplay.removeClass('fade-in');
        }, 200);
    }, 200);
}

/**
 * Limpiar cache de valores revelados al cambiar de categoría
 */
$(document).on('click', '.categoria-tab', function() {
    // Limpiar cache de valores sensitivos revelados
    $('.valor-real').each(function() {
        $(this).removeData('valor-cacheado');
        $(this).text('').addClass('d-none');
    });
    
    // Resetear botones a estado inicial
    $('.btn-revelar-secreto').each(function() {
        const $btn = $(this);
        if ($btn.hasClass('revelado')) {
            $btn.removeClass('revelado');
            $btn.html('<i class="fas fa-eye"></i> Ver Valor Real');
            
            const $wrapper = $btn.closest('.valor-sensitivo-wrapper');
            $wrapper.find('.valor-display').removeClass('d-none');
        }
    });
});
    
// ============================================
// FUNCIONALIDADES PARA PARÁMETROS DE PLANTILLA
// ============================================

/**
 * Inicializar funcionalidades específicas para parámetros de plantilla de cotización
 */
function initParametrosPlantilla() {
    // Inicializar contador de caracteres para textareas
    initContadorCaracteres();
    
    // Mejorar UX del textarea
    initTextareaUX();
    
    console.log('Funcionalidades de parámetros de plantilla inicializadas');
}

/**
 * Contador de caracteres en tiempo real para textareas
 */
function initContadorCaracteres() {
    $('.parametro-textarea').each(function() {
        const $textarea = $(this);
        const $contador = $textarea.closest('.parametro-control').find('.character-count');
        
        if ($contador.length === 0) {
            console.warn('No se encontró contador para textarea');
            return;
        }
        
        // Función para actualizar contador
        const actualizarContador = function() {
            const longitudActual = $textarea.val().length;
            const longitudMaxima = parseInt($textarea.attr('maxlength')) || 1000;
            
            // Actualizar número
            $contador.text(longitudActual);
            
            // Cambiar estilo según proximidad al límite
            $contador.removeClass('warning danger');
            
            const porcentaje = (longitudActual / longitudMaxima) * 100;
            if (porcentaje >= 95) {
                $contador.addClass('danger');
            } else if (porcentaje >= 80) {
                $contador.addClass('warning');
            }
            
            // Efecto visual cuando se alcanza el límite
            if (longitudActual >= longitudMaxima) {
                $contador.addClass('animate-pulse');
                setTimeout(() => $contador.removeClass('animate-pulse'), 1000);
            }
        };
        
        // Eventos
        $textarea.on('input keyup paste', actualizarContador);
        
        // Actualizar al cargar
        actualizarContador();
    });
}

/**
 * Mejorar UX del textarea para plantillas
 */
function initTextareaUX() {
    $('.parametro-textarea').each(function() {
        const $textarea = $(this);
        
        // Auto-resize basado en contenido
        autoResizeTextarea($textarea);
        
        // Atajos de teclado útiles
        $textarea.on('keydown', function(e) {
            // Ctrl+Enter para enviar formulario
            if (e.ctrlKey && e.key === 'Enter') {
                e.preventDefault();
                $textarea.closest('form').submit();
            }
            
            // Tab para insertar espacios en lugar de cambiar foco (útil para formato)
            if (e.key === 'Tab' && !e.shiftKey) {
                e.preventDefault();
                insertarTextoEnCursor($textarea[0], '    '); // 4 espacios
            }
        });
        
        // Guardar estado inicial para detectar cambios
        $textarea.data('valor-inicial', $textarea.val());
        
        // Detectar cambios para UX
        $textarea.on('input', function() {
            const $form = $textarea.closest('form');
            const valorInicial = $textarea.data('valor-inicial');
            const valorActual = $textarea.val();
            
            if (valorActual !== valorInicial) {
                $form.addClass('has-changes');
                $form.find('button[type="submit"]').removeClass('btn-outline-primary').addClass('btn-primary');
            } else {
                $form.removeClass('has-changes');
                $form.find('button[type="submit"]').removeClass('btn-primary').addClass('btn-outline-primary');
            }
        });
        
        // Placeholder animado
        if ($textarea.attr('placeholder')) {
            $textarea.on('focus blur', function() {
                $textarea.closest('.parametro-card').toggleClass('textarea-focused');
            });
        }
    });
}

/**
 * Auto-resize del textarea basado en el contenido
 */
function autoResizeTextarea($textarea) {
    const textarea = $textarea[0];
    
    const resize = function() {
        // Resetear altura para calcular correctamente
        textarea.style.height = 'auto';
        
        // Calcular altura necesaria
        const scrollHeight = textarea.scrollHeight;
        const minHeight = 100; // Altura mínima
        const maxHeight = 400; // Altura máxima
        
        const nuevaAltura = Math.max(minHeight, Math.min(maxHeight, scrollHeight));
        textarea.style.height = nuevaAltura + 'px';
        
        // Mostrar scrollbar solo si se alcanza el máximo
        if (scrollHeight > maxHeight) {
            textarea.style.overflowY = 'scroll';
        } else {
            textarea.style.overflowY = 'hidden';
        }
    };
    
    // Eventos
    $textarea.on('input keyup paste', resize);
    
    // Resize inicial
    setTimeout(resize, 100);
}

/**
 * Insertar texto en la posición del cursor
 */
function insertarTextoEnCursor(textarea, texto) {
    const inicio = textarea.selectionStart;
    const fin = textarea.selectionEnd;
    const valorActual = textarea.value;
    
    const nuevoValor = valorActual.substring(0, inicio) + texto + valorActual.substring(fin);
    textarea.value = nuevoValor;
    
    // Mantener cursor después del texto insertado
    const nuevaPosicion = inicio + texto.length;
    textarea.setSelectionRange(nuevaPosicion, nuevaPosicion);
    
    // Trigger eventos para actualizar contador
    $(textarea).trigger('input');
}

/**
 * Funciones de ayuda para plantillas - SIMPLIFICADAS
 */
function insertarVariablePlantilla($textarea, variable) {
    const variableTexto = `{{${variable}}}`;
    insertarTextoEnCursor($textarea[0], variableTexto);
}