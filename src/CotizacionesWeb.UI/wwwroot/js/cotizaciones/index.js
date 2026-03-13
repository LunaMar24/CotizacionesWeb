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
            'Copiar Version',
            'Esta seguro de que desea crear una nueva version de esta cotizacion?',
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
            'Duplicar Cotizacion',
            'Esta seguro de que desea duplicar esta cotizacion?<br><br>' +
            '<small class="text-muted">Se creara una nueva cotizacion en estado Borrador sin cliente asignado.</small>',
            'warning',
            function() {
                duplicarCotizacion(cotizacionId, button);
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
            showNotification('error', 'Error al copiar la version');
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
            showNotification('error', 'Error al duplicar la cotizacion');
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
            showNotification('error', 'Error al copiar la version');
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

        let titulo = 'Copiar Version ' + numeroVersion;
        let mensaje = 'Esta seguro de que desea crear una nueva version basada en la version ' + numeroVersion + '?';
        let tipo = 'info';
        
        if (!esActual) {
            titulo = 'Advertencia: Version Historica';
            mensaje = '<div class="alert alert-warning mb-0">' +
                      '<strong>ATENCION:</strong> Esta a punto de crear una nueva version basada en una version historica (v' + numeroVersion + ').' +
                      '</div>' +
                      '<p class="mt-3">Esto reemplazara la version actual vigente de la cotizacion.</p>' +
                      '<p class="mb-0">Desea continuar?</p>';
            tipo = 'warning';
        }

        mostrarModalConfirmacion(titulo, mensaje, tipo, function() {
            copiarVersionEspecifica(cotizacionId, versionId, !esActual, button);
        });
    });

    $('.btn-historial-version').on('click', function () {
        showNotification('info', 'Funcion de historial de version en desarrollo');
    });

    $('.btn-ver-detalle-version').on('click', function () {
        showNotification('info', 'Funcion de detalle de version en desarrollo');
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
