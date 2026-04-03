/**
 * JavaScript específico para la pantalla de Cotizaciones Archivadas
 * Maneja la funcionalidad de filtros, modales y acciones específicas de archivadas
 */

$(document).ready(function() {
    
    // ===============================
    // INICIALIZACIÓN
    // ===============================
    
    console.log('??? Pantalla de Cotizaciones Archivadas cargada');
    
    // Inicializar tooltips
    initTooltips();
    
    // Configurar eventos de filtros
    initFiltrosArchivadas();
    
    // Configurar botones de acción
    initBotonesAccion();
    
    // Configurar modales
    initModales();
    
    // ===============================
    // FUNCIONES DE INICIALIZACIÓN
    // ===============================
    
    function initTooltips() {
        $('[data-toggle="tooltip"]').tooltip();
        $('[title]').tooltip();
    }
    
    function initFiltrosArchivadas() {
        // Auto-submit al cambiar filtros de fecha
        $('input[type="date"]').on('change', function() {
            const value = $(this).val();
            if (value) {
                console.log('?? Filtro de fecha cambiado:', $(this).attr('name'), value);
            }
        });
        
        // Validación de rangos de fecha
        validateDateRanges();
        
        // Auto-complete para usuario que archivó
        initUsuarioArchivoAutocomplete();
    }
    
    function validateDateRanges() {
        // Validar rango de fecha de cotización
        $('input[name="Filtros.FechaDesde"], input[name="Filtros.FechaHasta"]').on('change', function() {
            const desde = $('input[name="Filtros.FechaDesde"]').val();
            const hasta = $('input[name="Filtros.FechaHasta"]').val();
            
            if (desde && hasta && desde > hasta) {
                showNotification('warning', 'La fecha "Desde" no puede ser mayor que la fecha "Hasta"');
                $(this).val('');
            }
        });
        
        // Validar rango de fecha de archivado
        $('input[name="Filtros.FechaArchivadoDesde"], input[name="Filtros.FechaArchivadoHasta"]').on('change', function() {
            const desde = $('input[name="Filtros.FechaArchivadoDesde"]').val();
            const hasta = $('input[name="Filtros.FechaArchivadoHasta"]').val();
            
            if (desde && hasta && desde > hasta) {
                showNotification('warning', 'La fecha de archivado "Desde" no puede ser mayor que la fecha "Hasta"');
                $(this).val('');
            }
        });
    }
    
    function initUsuarioArchivoAutocomplete() {
        // TODO: Implementar autocomplete para usuario que archivó
        // cuando se conecte con el servicio de usuarios
        $('input[name="Filtros.UsuarioArchivo"]').on('input', function() {
            const value = $(this).val();
            if (value && value.length >= 2) {
                // Aquí iría la lógica de autocomplete
                console.log('?? Buscando usuarios que coincidan con:', value);
            }
        });
    }
    
    function initBotonesAccion() {
        // Ver Historial
        $('.btn-historial').on('click', function(e) {
            e.preventDefault();
            const cotizacionId = $(this).attr('data-id');
            verHistorial(cotizacionId);
        });

        // Ver Versiones
        $('.btn-versiones').on('click', function(e) {
            e.preventDefault();
            const cotizacionId = $(this).attr('data-id');
            verVersiones(cotizacionId);
        });

        // Ver Detalle
        $('.btn-detalle').on('click', function(e) {
            e.preventDefault();
            const cotizacionId = $(this).attr('data-id');
            verDetalle(cotizacionId);
        });

        // Reactivar Cotización (funcionalidad futura)
        $('.btn-reactivar').on('click', function(e) {
            e.preventDefault();
            const cotizacionId = $(this).attr('data-id');
            confirmarReactivacion(cotizacionId);
        });
        
        // Limpiar filtros con confirmación
        $('a[href*="Archivadas"]:contains("Limpiar")').on('click', function(e) {
            const filtrosActivos = contarFiltrosActivos();
            if (filtrosActivos > 0) {
                e.preventDefault();
                confirmarLimpiarFiltros($(this).attr('href'));
            }
        });
    }
    
    function initModales() {
        // Limpiar contenido de modales al cerrar
        $('#modalHistorial, #modalVersiones').on('hidden.bs.modal', function() {
            $(this).find('.modal-content').html(
                '<div class="text-center p-3"><div class="spinner-border text-primary"></div></div>'
            );
        });
        
        // Ajustar tamaño de modales según contenido
        $('#modalHistorial').on('show.bs.modal', function() {
            $(this).find('.modal-dialog').addClass('modal-lg');
        });
        
        $('#modalVersiones').on('show.bs.modal', function() {
            $(this).find('.modal-dialog').addClass('modal-xl');
        });
    }
    
    // ===============================
    // ACCIONES DE COTIZACIONES
    // ===============================
    
    function verHistorial(cotizacionId) {
        console.log('?? Cargando historial para:', cotizacionId);
        
        $('#modalHistorialContent').html(
            '<div class="text-center p-5">' +
            '<div class="spinner-border text-primary mb-3"></div>' +
            '<p class="text-muted">Cargando historial...</p>' +
            '</div>'
        );
        $('#modalHistorial').modal('show');
        
        $.get('/Cotizaciones/Historial/' + cotizacionId)
            .done(function(data) {
                $('#modalHistorialContent').html(data);
                console.log('? Historial cargado exitosamente');
            })
            .fail(function(xhr, status, error) {
                console.error('? Error al cargar historial:', error);
                $('#modalHistorialContent').html(
                    '<div class="p-4 text-center text-danger">' +
                    '<i class="fas fa-exclamation-triangle fa-2x mb-3"></i>' +
                    '<h5>Error al cargar el historial</h5>' +
                    '<p class="text-muted">Por favor, inténtalo de nuevo más tarde.</p>' +
                    '</div>'
                );
            });
    }
    
    function verVersiones(cotizacionId) {
        console.log('?? Cargando versiones para:', cotizacionId);
        
        $('#modalVersionesContent').html(
            '<div class="text-center p-5">' +
            '<div class="spinner-border text-primary mb-3"></div>' +
            '<p class="text-muted">Cargando versiones...</p>' +
            '</div>'
        );
        $('#modalVersiones').modal('show');
        
        $.get('/Cotizaciones/Versiones/' + cotizacionId)
            .done(function(data) {
                $('#modalVersionesContent').html(data);
                console.log('? Versiones cargadas exitosamente');
            })
            .fail(function(xhr, status, error) {
                console.error('? Error al cargar versiones:', error);
                $('#modalVersionesContent').html(
                    '<div class="p-4 text-center text-danger">' +
                    '<i class="fas fa-exclamation-triangle fa-2x mb-3"></i>' +
                    '<h5>Error al cargar las versiones</h5>' +
                    '<p class="text-muted">Por favor, inténtalo de nuevo más tarde.</p>' +
                    '</div>'
                );
            });
    }
    
    function verDetalle(cotizacionId) {
        console.log('??? Navegando a detalle de archivo de:', cotizacionId);
        
        // Mostrar indicador de carga
        showNotification('info', 'Cargando detalle del archivo de cotización...');
        
        // Navegar a la vista específica de detalle de archivo
        window.location.href = '/Cotizaciones/ArchivoDetalle/' + cotizacionId;
    }
    
    function confirmarReactivacion(cotizacionId) {
        console.log('?? Solicitando reactivación para:', cotizacionId);
        
        // Configurar el modal de reactivación
        $('#cotizacionIdReactivacion').val(cotizacionId);
        $('#motivoReactivacion').val('');
        $('#contadorMotivoReactivacion').text('0');
        
        // Mostrar el modal
        $('#modalReactivacion').modal('show');
    }
    
    // ===============================
    // UTILIDADES DE FILTROS
    // ===============================
    
    function contarFiltrosActivos() {
        let count = 0;
        
        // Contar campos de texto con valor
        $('input[type="text"], input[type="number"]', '#filtrosForm').each(function() {
            if ($(this).val().trim()) count++;
        });
        
        // Contar campos de fecha con valor
        $('input[type="date"]', '#filtrosForm').each(function() {
            if ($(this).val()) count++;
        });
        
        // Contar selects con valor diferente al default
        $('select', '#filtrosForm').each(function() {
            if ($(this).val() && $(this).val() !== '') count++;
        });
        
        return count;
    }
    
    function confirmarLimpiarFiltros(limpiarUrl) {
        const filtrosActivos = contarFiltrosActivos();
        
        // Configurar modal de confirmación
        $('#modalConfirmacionTitulo').html('<i class="fas fa-broom"></i> Limpiar Filtros');
        $('#modalConfirmacionMensaje').html(
            `<p>¿Deseas limpiar todos los filtros activos?</p>
             <div class="alert alert-info">
                <i class="fas fa-info-circle"></i>
                Se eliminarán <strong>${filtrosActivos}</strong> filtro${filtrosActivos !== 1 ? 's' : ''} aplicado${filtrosActivos !== 1 ? 's' : ''}
             </div>`
        );
        
        // Configurar botón de confirmación
        $('#btnConfirmarAccion').off('click').on('click', function() {
            console.log('?? Limpiando filtros...');
            $('#modalConfirmacion').modal('hide');
            window.location.href = limpiarUrl;
        });
        
        // Mostrar modal
        $('#modalConfirmacion').modal('show');
    }
    
    // ===============================
    // FUNCIONES DE NOTIFICACIÓN
    // ===============================
    
    function showNotification(type, message, duration = 5000) {
        // Crear notificación toast personalizada
        const toastId = 'toast-' + Date.now();
        const iconClass = {
            'success': 'fas fa-check-circle text-success',
            'error': 'fas fa-exclamation-circle text-danger',
            'warning': 'fas fa-exclamation-triangle text-warning',
            'info': 'fas fa-info-circle text-info'
        }[type] || 'fas fa-info-circle text-info';
        
        const toast = $(`
            <div id="${toastId}" class="toast" role="alert" style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
                <div class="toast-header bg-${type === 'error' ? 'danger' : type} text-white">
                    <i class="${iconClass.replace('text-' + type, '')}"></i>
                    <strong class="me-auto ms-2">${type.charAt(0).toUpperCase() + type.slice(1)}</strong>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
                </div>
                <div class="toast-body">${message}</div>
            </div>
        `);
        
        $('body').append(toast);
        
        // Mostrar toast
        toast.fadeIn();
        
        // Auto-ocultar después del tiempo especificado
        setTimeout(() => {
            toast.fadeOut(() => toast.remove());
        }, duration);
        
        // Manejar cierre manual
        toast.find('.btn-close').on('click', function() {
            toast.fadeOut(() => toast.remove());
        });
    }
    
    function showConfirmation(title, message, icon, callback) {
        // Usar el modal de confirmación de la página
        $('#modalConfirmacionTitulo').html(`<i class="fas fa-${icon}"></i> ${title}`);
        $('#modalConfirmacionMensaje').html(message);
        
        // Configurar botón de confirmación
        $('#btnConfirmarAccion').off('click').on('click', function() {
            $('#modalConfirmacion').modal('hide');
            if (callback) callback();
        });
        
        // Mostrar modal
        $('#modalConfirmacion').modal('show');
    }
    
    // ===============================
    // KEYBOARD SHORTCUTS
    // ===============================
    
    $(document).on('keydown', function(e) {
        // Ctrl/Cmd + Enter para buscar
        if ((e.ctrlKey || e.metaKey) && e.keyCode === 13) {
            e.preventDefault();
            $('#filtrosForm').submit();
            console.log('?? Búsqueda activada via teclado');
        }
        
        // Escape para limpiar filtros
        if (e.keyCode === 27 && contarFiltrosActivos() > 0) {
            e.preventDefault();
            const limpiarUrl = $('a[href*="Archivadas"]:contains("Limpiar")').attr('href');
            if (limpiarUrl) {
                confirmarLimpiarFiltros(limpiarUrl);
            }
        }
    });
    
    // ===============================
    // EVENTOS GLOBALES
    // ===============================
    
    // Highlight de filas al hacer hover
    $('tbody tr').hover(
        function() {
            $(this).addClass('table-row-hover');
        },
        function() {
            $(this).removeClass('table-row-hover');
        }
    );
    
    // Mostrar información adicional en tooltips
    $('.badge-archived').tooltip({
        title: 'Esta cotización ha sido archivada y está en modo de solo lectura',
        placement: 'top'
    });
    
    // ===============================
    // EVENT HANDLERS PARA MODALES
    // ===============================
    
    // Modal de Reactivación
    $('#modalReactivacion').on('shown.bs.modal', function() {
        $('#motivoReactivacion').focus();
    });
    
    // Contador de caracteres para motivo de reactivación
    $('#motivoReactivacion').on('input', function() {
        const count = $(this).val().length;
        $('#contadorMotivoReactivacion').text(count);
        
        if (count > 450) {
            $('#contadorMotivoReactivacion').removeClass('text-muted text-warning').addClass('text-danger');
        } else if (count > 400) {
            $('#contadorMotivoReactivacion').removeClass('text-muted text-danger').addClass('text-warning');
        } else {
            $('#contadorMotivoReactivacion').removeClass('text-warning text-danger').addClass('text-muted');
        }
    });
    
    // Confirmar reactivación
    $('#btnConfirmarReactivacion').on('click', function() {
        const btn = $(this);
        const motivo = $('#motivoReactivacion').val().trim();
        const cotizacionId = $('#cotizacionIdReactivacion').val();
        
        if (!motivo || motivo.length < 10) {
            showNotification('warning', 'Debe proporcionar un motivo de al menos 10 caracteres');
            $('#motivoReactivacion').focus();
            return;
        }
        
        // Deshabilitar botón y mostrar loading
        btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Reactivando...');
        
        // Enviar solicitud de reactivación
        $.ajax({
            url: '/Cotizaciones/Reactivar',
            type: 'POST',
            data: {
                cotizacionId: cotizacionId,
                motivo: motivo,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                btn.prop('disabled', false).html('<i class="fas fa-undo"></i> Reactivar Cotización');
                
                if (response.success) {
                    $('#modalReactivacion').modal('hide');
                    showNotification('success', response.message);
                    
                    // Recargar página después de un momento para reflejar cambios
                    setTimeout(function() {
                        location.reload();
                    }, 1500);
                } else {
                    showNotification('error', response.message);
                }
            },
            error: function(xhr, status, error) {
                btn.prop('disabled', false).html('<i class="fas fa-undo"></i> Reactivar Cotización');
                console.error('Error en reactivación:', error);
                showNotification('error', 'Error al procesar la reactivación');
            }
        });
    });
    
    // Limpiar modal al cerrar
    $('#modalReactivacion').on('hidden.bs.modal', function() {
        $('#motivoReactivacion').val('');
        $('#contadorMotivoReactivacion').text('0').removeClass('text-warning text-danger').addClass('text-muted');
        $('#cotizacionIdReactivacion').val('');
        $('#btnConfirmarReactivacion').prop('disabled', false).html('<i class="fas fa-undo"></i> Reactivar Cotización');
    });
    
    console.log('? JavaScript de Cotizaciones Archivadas inicializado completamente');
});

// ===============================
// FUNCIONES EXPORTADAS (si se necesitan desde otros scripts)
// ===============================

window.CotizacionesArchivadas = {
    verHistorial: function(cotizacionId) {
        verHistorial(cotizacionId);
    },
    verVersiones: function(cotizacionId) {
        verVersiones(cotizacionId);
    },
    verDetalle: function(cotizacionId) {
        verDetalle(cotizacionId);
    },
    limpiarFiltros: function() {
        const limpiarUrl = $('a[href*="Archivadas"]:contains("Limpiar")').attr('href');
        if (limpiarUrl) {
            window.location.href = limpiarUrl;
        }
    }
};