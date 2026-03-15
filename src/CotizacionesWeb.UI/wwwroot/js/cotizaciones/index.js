// ========================================
// COTIZACIONES INDEX - JavaScript
// ========================================

$(document).ready(function () {
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

    // Copiar Version Actual (con modal de confirmacion)
    $('.btn-copiar').on('click', function () {
        const cotizacionId = $(this).attr('data-id');
        const button = $(this);
        
        mostrarModalConfirmacion(
            'Copiar Versión',
            '¿Está seguro de que desea crear una nueva versión de esta cotización?',
            'info',
            function() {
                copiarVersion(cotizacionId, button);
            }
        );
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
});

// ========================================
// FUNCIONES AJAX
// ========================================

function copiarVersion(cotizacionId, button) {
    button.prop('disabled', true);

    $.ajax({
        url: '/Cotizaciones/CopiarVersion',
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
            showNotification('error', 'Error al copiar la versión');
            button.prop('disabled', false);
        }
    });
}

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

function copiarVersionEspecifica(cotizacionId, versionId, esVersionAntigua, button) {
    button.prop('disabled', true);

    $.ajax({
        url: '/Cotizaciones/CopiarVersionEspecifica',
        type: 'POST',
        data: {
            cotizacionId: cotizacionId,
            versionId: versionId,
            esVersionAntigua: esVersionAntigua,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                showNotification('success', response.message);
                $('#modalVersiones').modal('hide');
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                showNotification('error', response.message);
                button.prop('disabled', false);
            }
        },
        error: function () {
            showNotification('error', 'Error al copiar la versión');
            button.prop('disabled', false);
        }
    });
}

// ========================================
// EVENTOS DEL MODAL DE VERSIONES
// ========================================

function inicializarEventosVersiones() {
    $('.btn-copiar-version').on('click', function () {
        const cotizacionId = $(this).attr('data-cotizacion-id');
        const versionId = $(this).attr('data-version-id');
        const esActual = $(this).attr('data-es-actual') === 'true';
        const numeroVersion = $(this).attr('data-numero-version');
        const button = $(this);

        let titulo = 'Copiar Versión ' + numeroVersion;
        let mensaje = '¿Está seguro de que desea crear una nueva versión basada en la versión ' + numeroVersion + '?';
        let tipo = 'info';

        if (!esActual) {
            titulo = 'Advertencia: Versión Histórica';
            mensaje = '<div class="alert alert-warning mb-0">' +
                      '<strong>ATENCIÓN:</strong> Está a punto de crear una nueva versión basada en una versión histórica (v' + numeroVersion + ').' +
                      '</div>' +
                      '<p class="mt-3">Esto reemplazará la versión actual vigente de la cotización.</p>' +
                      '<p class="mb-0">¿Desea continuar?</p>';
            tipo = 'warning';
        }

        mostrarModalConfirmacion(titulo, mensaje, tipo, function() {
            copiarVersionEspecifica(cotizacionId, versionId, !esActual, button);
        });
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
}

// ========================================
// MODAL DE CONFIRMACION GENERICO
// ========================================

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