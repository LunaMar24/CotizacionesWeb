// ========================================
// COTIZACIONES INDEX - JavaScript
// ========================================

// Usar DOMContentLoaded para asegurar que el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    // Esperar a que jQuery esté disponible
    function waitForJQuery(callback) {
        if (typeof $ !== 'undefined') {
            callback();
        } else {
            setTimeout(function() {
                waitForJQuery(callback);
            }, 50);
        }
    }

    // Ejecutar cuando jQuery esté disponible
    waitForJQuery(function() {
        // Evitar que el dropdown se cierre al hacer click en los checkboxes
        $('.dropdown-menu-estados').on('click', function (e) {
            if ($(e.target).is('input[type="checkbox"]') || $(e.target).closest('.dropdown-item-custom').length) {
                e.stopPropagation();
            }
        });

        // Ver Historial
        $('.btn-historial').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            
            $('#modalHistorialContent').html('<div class="text-center p-5"><div class="spinner-border text-primary"></div></div>');
            $('#modalHistorial').modal('show');
            
            $.get('/Cotizaciones/Historial/' + cotizacionId, function (data) {
                $('#modalHistorialContent').html(data);
            }).fail(function () {
                $('#modalHistorialContent').html('<div class="p-4 text-center text-danger"><i class="fas fa-exclamation-triangle fa-2x mb-2"></i><p>Error al cargar el historial</p></div>');
            });
        });

        // Ver Versiones
        $('.btn-versiones').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            
            $('#modalVersionesContent').html('<div class="text-center p-5"><div class="spinner-border text-primary"></div></div>');
            $('#modalVersiones').modal('show');
            
            $.get('/Cotizaciones/Versiones/' + cotizacionId, function (data) {
                $('#modalVersionesContent').html(data);
                inicializarEventosVersiones();
            }).fail(function () {
                $('#modalVersionesContent').html('<div class="p-4 text-center text-danger"><i class="fas fa-exclamation-triangle fa-2x mb-2"></i><p>Error al cargar las versiones</p></div>');
            });
        });

        // Copiar Version Actual (con modal de comentario)
        $('.btn-copiar').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const fila = $(this).closest('tr');
            const estado = fila.attr('data-estado');
            
            // Validar que se puede copiar en este estado
            if (!puedeCopiarse(estado)) {
                showNotification('warning', 'No se puede copiar una cotización en estado actual. Solo es posible copiar cotizaciones Aprobadas o Rechazadas.');
                return;
            }
            
            // Abrir modal de comentario
            abrirModalComentarioCopia({
                cotizacionId: cotizacionId,
                tipoCopia: 'actual'
            });
        });

        // Duplicar Cotizacion (con modal de confirmacion)
        $('.btn-duplicar').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Duplicar Cotización',
                '¿Está seguro de que desea duplicar esta cotización?<br><br>' +
                '<small class="text-muted">Se creará una nueva cotización en estado Borrador sin cliente asignado.</small>',
                'warning',
                function() {
                    duplicarCotizacion(cotizacionId, button);
                }
            );
        });

        // ========================================
        // BOTONES DE TRANSICION DE ESTADOS
        // ========================================

        // Enviar a Aprobación (Borrador -> Pendiente Aprobación)
        $('.btn-enviar-aprobacion').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Enviar a Aprobación',
                '¿Está seguro de que desea enviar esta cotización para aprobación?',
                'warning',
                function() {
                    cambiarEstadoCotizacion(cotizacionId, 'EnviarAprobacion', button);
                }
            );
        });

        // Aprobar (Pendiente -> Aprobada)
        $('.btn-aprobar').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Aprobar Cotización',
                '¿Está seguro de que desea aprobar esta cotización?<br><br>' +
                '<small class="text-muted">Una vez aprobada, podrá ser enviada al cliente.</small>',
                'success',
                function() {
                    cambiarEstadoCotizacion(cotizacionId, 'Aprobar', button);
                }
            );
        });

        // Devolver a Borrador (Pendiente -> Borrador)
        $('.btn-devolver-borrador').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Devolver a Borrador',
                '¿Está seguro de que desea devolver esta cotización a estado Borrador?',
                'warning',
                function() {
                    cambiarEstadoCotizacion(cotizacionId, 'DevolverBorrador', button);
                }
            );
        });

        // Enviar al Cliente (Aprobada -> Enviada)
        $('.btn-enviar-cliente').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Enviar al Cliente',
                '¿Está seguro de que desea enviar esta cotización al cliente?<br><br>' +
                '<small class="text-muted">Se registrará la fecha de envío.</small>',
                'info',
                function() {
                    cambiarEstadoCotizacion(cotizacionId, 'EnviarCliente', button);
                }
            );
        });

        // Marcar como Aceptada (Enviada -> Aceptada)
        $('.btn-marcar-aceptada').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Marcar como Aceptada',
                '¿Confirma que el cliente ha aceptado esta cotización?<br><br>' +
                '<small class="text-muted">La cotización podrá ser enviada al ERP.</small>',
                'success',
                function() {
                    cambiarEstadoCotizacion(cotizacionId, 'MarcarAceptada', button);
                }
            );
        });

        // Marcar como Rechazada (Enviada -> Rechazada)
        $('.btn-marcar-rechazada').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Marcar como Rechazada',
                '¿Confirma que el cliente ha rechazado esta cotización?',
                'danger',
                function() {
                    cambiarEstadoCotizacion(cotizacionId, 'MarcarRechazada', button);
                }
            );
        });

        // Archivar Cotización
        $('.btn-archivar').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Archivar Cotización',
                '¿Está seguro de que desea archivar esta cotización?<br><br>' +
                '<small class="text-muted">Una cotización archivada no puede ser modificada.</small>',
                'warning',
                function() {
                    cambiarEstadoCotizacion(cotizacionId, 'Archivar', button);
                }
            );
        });

        // Enviar al ERP
        $('.btn-enviar-erp').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            const button = $(this);

            mostrarModalConfirmacion(
                'Enviar al ERP',
                '¿Está seguro de que desea enviar esta cotización al ERP?<br><br>' +
                '<small class="text-muted">Esta acción integrará la cotización con el sistema ERP.</small>',
                'info',
                function() {
                    cambiarEstadoCotizacion(cotizacionId, 'EnviarERP', button);
                }
            );
        });

        // ========================================
        // APLICAR REGLAS DE NEGOCIO AL CARGAR
        // ========================================
        aplicarReglasCopia();
    });
});

// ========================================
// FUNCIONES GLOBALES (Definidas fuera para accesibilidad global)
// ========================================

function duplicarCotizacion(cotizacionId, button) {
    button.prop('disabled', true);

    $.ajax({
        url: '/Cotizaciones/Duplicar',
        type: 'POST',
        data: {
            cotizacionId: cotizacionId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                showNotification('success', response.message);
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                showNotification('error', response.message);
                button.prop('disabled', false);
            }
        },
        error: function () {
            showNotification('error', 'Error al duplicar la cotización');
            button.prop('disabled', false);
        }
    });
}

function cambiarEstadoCotizacion(cotizacionId, accion, button) {
    button.prop('disabled', true);

    $.ajax({
        url: '/Cotizaciones/' + accion,
        type: 'POST',
        data: {
            cotizacionId: cotizacionId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                showNotification('success', response.message);
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                showNotification('error', response.message);
                button.prop('disabled', false);
            }
        },
        error: function (xhr) {
            let errorMsg = 'Error al cambiar el estado de la cotización';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMsg = xhr.responseJSON.message;
            }
            showNotification('error', errorMsg);
            button.prop('disabled', false);
        }
    });
}

function inicializarEventosVersiones() {
    $('.btn-copiar-version').on('click', function () {
        const cotizacionId = $(this).attr('data-cotizacion-id');
        const versionId = $(this).attr('data-version-id');
        const esActual = $(this).attr('data-es-actual') === 'true';
        const numeroVersion = $(this).attr('data-numero-version');

        // Obtener el estado actual de la cotización desde la fila de la tabla principal
        const estadoActual = $(`tr[data-cotizacion-id="${cotizacionId}"]`).attr('data-estado');
        
        // Validar que se puede copiar en este estado
        if (!puedeCopiarse(estadoActual)) {
            showNotification('warning', 'No se puede copiar una cotización en estado actual. Solo es posible copiar cotizaciones Aprobadas o Rechazadas.');
            return;
        }

        // Abrir modal de comentario
        abrirModalComentarioCopia({
            cotizacionId: cotizacionId,
            versionId: versionId,
            numeroVersion: numeroVersion,
            esVersionAntigua: !esActual,
            tipoCopia: 'especifica'
        });
    });

    // También aplicar reglas de copia a botones de versiones específicas
    $('.btn-copiar-version').each(function() {
        const cotizacionId = $(this).attr('data-cotizacion-id');
        const estadoActual = $(`tr[data-cotizacion-id="${cotizacionId}"]`).attr('data-estado');
        
        if (!puedeCopiarse(estadoActual)) {
            $(this).prop('disabled', true)
                   .addClass('disabled')
                   .attr('title', 'No se puede copiar en estado actual')
                   .removeClass('btn-action-copy')
                   .addClass('btn-action-disabled');
        }
    });

    $('.btn-duplicar-version').on('click', function () {
        const cotizacionId = $(this).attr('data-cotizacion-id');
        const button = $(this);

        mostrarModalConfirmacion(
            'Duplicar Cotización',
            '¿Está seguro de que desea duplicar esta cotización?<br><br>' +
            '<small class="text-muted">Se creará una nueva cotización en estado Borrador sin cliente asignado.</small>',
            'warning',
            function() {
                duplicarCotizacion(cotizacionId, button);
            }
        );
    });

    $('.btn-detalle-version').on('click', function () {
        const versionId = $(this).attr('data-version-id');
        const cotizacionId = $(this).attr('data-cotizacion-id');

        // Navegar a la vista de detalle de la versión
        window.location.href = '/Cotizaciones/Detalle/' + cotizacionId + '?versionId=' + versionId;
    });

    // NUEVO: Ver Historial de Versión Específica
    $('.btn-historial-version').on('click', function () {
        const versionId = $(this).attr('data-version-id');
        const cotizacionId = $(this).attr('data-cotizacion-id');
        const numeroVersion = $(this).attr('data-numero-version');
        
        // Cerrar la modal de versiones primero
        $('#modalVersiones').modal('hide');
        
        // Esperar a que la modal se cierre completamente antes de abrir la nueva
        $('#modalVersiones').on('hidden.bs.modal.historial', function () {
            // Remover el event listener para evitar múltiples bindings
            $(this).off('hidden.bs.modal.historial');
            
            // Mostrar la modal de historial
            $('#modalHistorialContent').html('<div class="text-center p-5"><div class="spinner-border text-primary"></div></div>');
            $('#modalHistorial').modal('show');
            
            $.get('/Cotizaciones/HistorialVersion/' + versionId, {
                cotizacionId: cotizacionId,
                numeroVersion: numeroVersion
            }, function (data) {
                $('#modalHistorialContent').html(data);
            }).fail(function () {
                $('#modalHistorialContent').html('<div class="p-4 text-center text-danger"><i class="fas fa-exclamation-triangle fa-2x mb-2"></i><p>Error al cargar el historial de la versión</p></div>');
            });
        });
        
        // Trigger el cierre si la modal ya está cerrada
        if (!$('#modalVersiones').hasClass('show')) {
            $('#modalVersiones').trigger('hidden.bs.modal.historial');
        }
    });
}

function mostrarModalConfirmacion(titulo, mensaje, tipo, onConfirm) {
    const headerClasses = {
        'info': 'bg-info text-white',
        'warning': 'bg-warning text-dark',
        'danger': 'bg-danger text-white',
        'success': 'bg-success text-white'
    };
    
    const iconos = {
        'info': 'fa-info-circle',
        'warning': 'fa-exclamation-triangle',
        'danger': 'fa-exclamation-circle',
        'success': 'fa-check-circle'
    };

    const btnClasses = {
        'info': 'btn-primary',
        'warning': 'btn-warning',
        'danger': 'btn-danger',
        'success': 'btn-success'
    };

    $('#modalConfirmacionHeader').removeClass().addClass('modal-header ' + headerClasses[tipo]);
    $('#btnConfirmarAccion').removeClass().addClass('btn ' + btnClasses[tipo]);
    $('#modalConfirmacionTitulo').html('<i class="fas ' + iconos[tipo] + '"></i> ' + titulo);
    $('#modalConfirmacionMensaje').html(mensaje);
    
    $('#btnConfirmarAccion').off('click').on('click', function() {
        $('#modalConfirmacion').modal('hide');
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
    });
    
    $('#modalConfirmacion').modal('show');
}

function abrirModalComentarioCopia(datos) {
    // Configurar título y mensaje según el tipo de copia
    if (datos.tipoCopia === 'actual') {
        $('#tituloComentarioCopia').text('Copiar Versión Actual');
        $('#mensajeComentarioCopia').text('Se creará una nueva versión basada en la versión actual de la cotización.');
    } else {
        $('#tituloComentarioCopia').text('Copiar Versión Específica');
        $('#mensajeComentarioCopia').text(`Se creará una nueva versión basada en la versión ${datos.numeroVersion}.`);
    }
    
    // Configurar campos ocultos
    $('#cotizacionIdCopia').val(datos.cotizacionId);
    $('#versionIdCopia').val(datos.versionId || '');
    $('#esVersionAntiguaCopia').val(datos.esVersionAntigua || 'false');
    $('#tipoCopiaCopia').val(datos.tipoCopia);
    
    // Mostrar modal
    $('#modalComentarioCopia').modal('show');
}

function puedeCopiarse(estado) {
    // Solo se puede copiar en estados: Aprobada (A) y Rechazada (R)
    return estado === 'A' || estado === 'R';
}

function aplicarReglasCopia() {
    $('.btn-copiar').each(function() {
        const fila = $(this).closest('tr');
        const estado = fila.attr('data-estado');
        
        if (!puedeCopiarse(estado)) {
            $(this).prop('disabled', true)
                   .addClass('disabled')
                   .attr('title', 'No se puede copiar en estado actual')
                   .removeClass('btn-action-copy')
                   .addClass('btn-action-disabled');
        }
    });
}