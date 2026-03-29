// ============================================
// MODALS - JavaScript global para modales
// ============================================

/**
 * Función global reutilizable para mostrar confirmaciones con modal Bootstrap
 * Reemplaza el uso de confirm() nativo del navegador
 * 
 * @param {string} titulo - Título del modal
 * @param {string} mensaje - Mensaje HTML permitido
 * @param {string} tipo - Tipo de modal: 'info', 'warning', 'danger', 'success', 'exit'
 * @param {function} onConfirm - Callback si el usuario confirma
 * @param {function} onCancel - Callback opcional si el usuario cancela
 * @param {object} opciones - Opciones adicionales (btnTextoConfirmar, btnTextoCancelar)
 */
window.mostrarModalConfirmacionGlobal = function(titulo, mensaje, tipo, onConfirm, onCancel, opciones) {
    const modal = $('#modalConfirmacion');
    
    // Validar que el modal existe
    if (modal.length === 0) {
        console.error('Modal de confirmación no encontrado en el DOM');
        usarConfirmacionNativaGlobal(titulo, mensaje, onConfirm, onCancel);
        return;
    }
    
    // Limpiar eventos anteriores
    modal.off('.globalconfirm');
    $('#btnConfirmarAccion').off('.globalconfirm');
    
    // Configuración visual según tipo
    const config = obtenerConfiguracionModalGlobal(tipo);
    
    // Aplicar estilos al header
    const header = $('#modalConfirmacionHeader');
    header.removeClass('bg-info bg-warning bg-danger bg-success bg-primary text-white text-dark')
          .addClass(config.headerClass);
    
    // Establecer título con icono
    $('#modalConfirmacionTitulo').html(`<i class="fas ${config.icono}"></i> ${titulo}`);
    
    // Establecer mensaje
    $('#modalConfirmacionMensaje').html(mensaje);
    
    // Configurar botón de confirmación
    const btnConfirmar = $('#btnConfirmarAccion');
    const textoConfirmar = (opciones && opciones.btnTextoConfirmar) || config.btnTexto;
    btnConfirmar.removeClass('btn-info btn-warning btn-danger btn-success btn-primary')
                .addClass(config.btnClass)
                .html(`<i class="fas fa-check"></i> ${textoConfirmar}`);
    
    // Configurar botón de cancelar si se especificó texto personalizado
    if (opciones && opciones.btnTextoCancelar) {
        $('#modalConfirmacion .btn-secondary[data-dismiss="modal"]')
            .html(`<i class="fas fa-times"></i> ${opciones.btnTextoCancelar}`);
    } else {
        $('#modalConfirmacion .btn-secondary[data-dismiss="modal"]')
            .html('<i class="fas fa-times"></i> Cancelar');
    }
    
    // Variable para controlar si se confirmó
    let accionConfirmada = false;
    
    // Evento de confirmación
    btnConfirmar.on('click.globalconfirm', function(e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        
        accionConfirmada = true;
        modal.modal('hide');
    });
    
    // Evento al cerrar el modal
    modal.one('hidden.bs.modal.globalconfirm', function() {
        // Ejecutar callback correspondiente
        if (accionConfirmada) {
            if (typeof onConfirm === 'function') {
                setTimeout(onConfirm, 150);
            }
        } else {
            if (typeof onCancel === 'function') {
                setTimeout(onCancel, 150);
            }
        }
        
        // Limpiar eventos
        modal.off('.globalconfirm');
        btnConfirmar.off('.globalconfirm');
    });
    
    // Mostrar modal
    modal.modal({
        backdrop: 'static',
        keyboard: false,
        show: true
    });
};

/**
 * Obtiene la configuración visual según el tipo de modal
 */
function obtenerConfiguracionModalGlobal(tipo) {
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
            btnTexto: 'Salir de Todas Formas'
        }
    };
    
    return configuraciones[tipo] || configuraciones['info'];
}

/**
 * Fallback a confirmación nativa del navegador
 * Solo se usa si el modal de Bootstrap no está disponible
 */
function usarConfirmacionNativaGlobal(titulo, mensaje, onConfirm, onCancel) {
    // Convertir HTML a texto plano
    const mensajeTexto = mensaje
        .replace(/<br\s*\/?>/gi, '\n')
        .replace(/<\/p>\s*<p>/gi, '\n\n')
        .replace(/<[^>]*>/g, '')
        .replace(/\s+/g, ' ')
        .trim();
    
    const textoCompleto = `${titulo}\n\n${mensajeTexto}`;
    
    // IMPORTANTE: setTimeout para evitar bloqueo del navegador
    // y permitir que el flujo de eventos se complete correctamente
    setTimeout(function() {
        const confirmacion = confirm(textoCompleto);
        
        if (confirmacion && typeof onConfirm === 'function') {
            onConfirm();
        } else if (!confirmacion && typeof onCancel === 'function') {
            onCancel();
        }
    }, 10);
}

$(document).ready(function() {
    // Los modales se centran automáticamente con CSS
    
    // Agregar animación al contenido dinámico
    $('.modal').on('show.bs.modal', function() {
        $(this).find('.modal-content').addClass('fade-in-modal');
    });
});