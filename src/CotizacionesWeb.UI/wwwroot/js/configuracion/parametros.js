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
            
        case 'Booleano (S/N)':
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
            },
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
    $feedback.text('? Valor válido');
    
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
            case 'Booleano (S/N)':
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

// Advertir antes de salir si hay cambios
$(window).on('beforeunload', function(e) {
    if (checkUnsavedChanges()) {
        const mensaje = 'Tiene cambios sin guardar. ¿Está seguro de que desea salir?';
        e.returnValue = mensaje;
        return mensaje;
    }
});

/**
 * Funciones de notificación
 */
function mostrarNotificacion(tipo, mensaje) {
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