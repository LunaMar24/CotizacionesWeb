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

        // Editar Cotización (navegar a página de edición)
        $('.btn-editor').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            
            // DEBUGGING TEMPORAL
            console.log('=== CLICK EN BOTÓN EDITOR ===');
            console.log('Botón clickeado:', this);
            console.log('CotizacionId del botón:', cotizacionId);
            console.log('Fila del botón:', $(this).closest('tr').attr('data-cotizacion-id'));
            console.log('URL que se va a abrir:', '/Cotizaciones/Editor/' + cotizacionId);
            console.log('============================');
            
            // Verificar que el ID no esté vacío o undefined
            if (!cotizacionId || cotizacionId === 'undefined') {
                console.error('ERROR: cotizacionId está vacío o undefined');
                showNotification('error', 'Error: ID de cotización no válido');
                return;
            }
            
            // Navegar directamente a la vista de edición
            window.location.href = '/Cotizaciones/Editor/' + cotizacionId;
        });

        // Ver Detalle (navegar a página de detalle)
        $('.btn-detalle').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            
            // Navegar directamente a la vista de detalle
            window.location.href = '/Cotizaciones/Detalle/' + cotizacionId;
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

        // Devolver a Borrador (Pendiente -> Borrador) - CON NOTA OBLIGATORIA
        $('.btn-devolver-borrador').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            
            // Abrir modal genérico para solicitar nota
            abrirModalCambioEstadoConNota({
                cotizacionId: cotizacionId,
                estadoOrigen: 'P',
                estadoDestino: 'B',
                accion: 'DevolverBorrador',
                titulo: 'Devolver a Borrador',
                icono: 'fa-undo',
                mensaje: 'Está a punto de devolver esta cotización al estado <strong>Borrador</strong>.',
                etiquetaNota: 'Motivo del rechazo (obligatorio):',
                placeholder: 'Indique la razón por la cual devuelve la cotización a borrador...',
                colorHeader: 'bg-warning text-dark',
                colorBoton: 'btn-warning',
                textoBoton: 'Devolver a Borrador'
            });
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

        // Marcar como Rechazada (Enviada -> Rechazada) - CON NOTA OBLIGATORIA
        $('.btn-marcar-rechazada').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            
            // Abrir modal genérico para solicitar nota
            abrirModalCambioEstadoConNota({
                cotizacionId: cotizacionId,
                estadoOrigen: 'E',
                estadoDestino: 'R',
                accion: 'MarcarRechazada',
                titulo: 'Marcar como Rechazada',
                icono: 'fa-times-circle',
                mensaje: 'Está a punto de marcar esta cotización como <strong>Rechazada</strong>.',
                etiquetaNota: 'Motivo del rechazo (obligatorio):',
                placeholder: 'Indique por qué el cliente rechazó la cotización...',
                colorHeader: 'bg-danger text-white',
                colorBoton: 'btn-danger',
                textoBoton: 'Marcar como Rechazada'
            });
        });

        // Archivar Cotización - CON NOTA OBLIGATORIA
        $('.btn-archivar').on('click', function () {
            const cotizacionId = $(this).attr('data-id');
            
            // Abrir modal genérico para solicitar nota
            abrirModalCambioEstadoConNota({
                cotizacionId: cotizacionId,
                estadoOrigen: $(this).closest('tr').attr('data-estado'), // Estado actual dinámico
                estadoDestino: 'X',
                accion: 'Archivar',
                titulo: 'Archivar Cotización',
                icono: 'fa-archive',
                mensaje: 'Está a punto de archivar esta cotización. <strong>Esta acción no se puede deshacer</strong>.',
                etiquetaNota: 'Motivo del archivado (obligatorio):',
                placeholder: 'Indique el motivo por el cual se archiva la cotización...',
                colorHeader: 'bg-dark text-white',
                colorBoton: 'btn-dark',
                textoBoton: 'Archivar Cotización'
            });
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
                    // Para cambios de estado, recargar normalmente ya que no cambia fechas
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
        const fromArchived = $(this).attr('data-from-archived') === 'true';

        // Navegar a la vista de detalle de la versión específica con parámetro de origen
        let url = '/Cotizaciones/Detalle/' + cotizacionId + '?versionId=' + versionId;
        if (fromArchived) {
            url += '&fromArchived=true';
        }
        window.location.href = url;
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

// Hacer la función disponible globalmente
window.inicializarEventosVersiones = inicializarEventosVersiones;

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

function abrirModalCambioEstadoConNota(opciones) {
    // Configurar el modal con las opciones proporcionadas
    $('#tituloNotaCambioEstado').html(`<i class="fas ${opciones.icono}"></i> ${opciones.titulo}`);
    $('#mensajeNotaCambioEstado').html(opciones.mensaje);
    $('#etiquetaNotaCambioEstado').text(opciones.etiquetaNota);
    $('#notaCambioEstado').attr('placeholder', opciones.placeholder).val('');
    $('#contadorNotaCambioEstado').text('0').removeClass('text-warning text-danger').addClass('text-muted');
    
    // Configurar límites de caracteres según el tipo de operación
    if (opciones.estadoDestino === 'X') { // Es archivado
        // Para archivado: motivo 200 chars, comentario adicional 500 chars
        $('#notaCambioEstado').attr('maxlength', '200');
        $('#limiteNotaCambioEstado').text('200');
        $('#etiquetaNotaCambioEstado').text('Motivo del archivado (obligatorio):');
        
        // Mostrar campo adicional
        $('#grupoComentarioAdicionalArchivo').show();
        $('#comentarioAdicionalArchivo').val('');
        $('#contadorComentarioArchivo').text('0').removeClass('text-warning text-danger').addClass('text-muted');
    } else {
        // Para otros estados: motivo 500 chars, sin comentario adicional
        $('#notaCambioEstado').attr('maxlength', '500');
        $('#limiteNotaCambioEstado').text('500');
        
        // Ocultar campo adicional
        $('#grupoComentarioAdicionalArchivo').hide();
    }
    
    // Configurar campos ocultos
    $('#cotizacionIdCambioEstado').val(opciones.cotizacionId);
    $('#estadoOrigenCambioEstado').val(opciones.estadoOrigen);
    $('#estadoDestinoCambioEstado').val(opciones.estadoDestino);
    $('#accionCambioEstado').val(opciones.accion);
    
    // Configurar estilos del header y botón
    $('#headerNotaCambioEstado').removeClass().addClass('modal-header ' + opciones.colorHeader);
    $('#btnConfirmarCambioEstado').removeClass().addClass('btn ' + opciones.colorBoton)
        .html(`<i class="fas ${opciones.icono}"></i> ${opciones.textoBoton}`);
    
    // Mostrar modal
    $('#modalNotaCambioEstado').modal('show');
}

function ejecutarCambioEstadoConNota(cotizacionId, estadoOrigen, estadoDestino, nota, button, comentarioAdicional = '') {
    button.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Procesando...');

    // Preparar los datos para enviar
    const data = {
        cotizacionId: cotizacionId,
        estadoOrigen: estadoOrigen,
        estadoDestino: estadoDestino,
        nota: nota,
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    };

    // Agregar comentario adicional si existe (para archivado)
    if (comentarioAdicional) {
        data.comentarioAdicional = comentarioAdicional;
    }

    $.ajax({
        url: '/Cotizaciones/CambiarEstadoConNota',
        type: 'POST',
        data: data,
        success: function (response) {
            if (response.success) {
                showNotification('success', response.message);
                
                // Si el response indica que se debe recargar, hacerlo
                if (response.shouldReload) {
                    setTimeout(function () {
                        location.reload();
                    }, 1500);
                } else {
                    // Restaurar botón en caso de que no se recargue
                    setTimeout(function () {
                        location.reload();
                    }, 1500);
                }
            } else {
                showNotification('error', response.message);
                // Restaurar botón original usando información del modal
                restaurarBotonOriginal(button);
            }
        },
        error: function (xhr) {
            let errorMsg = 'Error al cambiar el estado de la cotización';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMsg = xhr.responseJSON.message;
            } else if (xhr.status === 403) {
                errorMsg = 'No tiene permisos para realizar esta operación';
            } else if (xhr.status === 401) {
                errorMsg = 'Su sesión ha expirado. Por favor, inicie sesión nuevamente';
            }
            
            showNotification('error', errorMsg);
            restaurarBotonOriginal(button);
        }
    });
}

function restaurarBotonOriginal(button) {
    // Obtener información del modal para restaurar el botón
    const accion = $('#accionCambioEstado').val();
    const iconos = {
        'DevolverBorrador': 'fa-undo',
        'MarcarRechazada': 'fa-times-circle',
        'Archivar': 'fa-archive'
    };
    const textos = {
        'DevolverBorrador': 'Devolver a Borrador',
        'MarcarRechazada': 'Marcar como Rechazada',
        'Archivar': 'Archivar Cotización'
    };
    
    const icono = iconos[accion] || 'fa-edit';
    const texto = textos[accion] || 'Acción';
    
    button.prop('disabled', false).html(`<i class="fas ${icono}"></i> ${texto}`);
}