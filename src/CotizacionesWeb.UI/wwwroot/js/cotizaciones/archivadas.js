/**
 * JavaScript específico para la pantalla de Cotizaciones Archivadas
 * Maneja la funcionalidad de filtros, modales y acciones específicas de archivadas
 */

$(document).ready(function() {
    
// ===============================
// INICIALIZACIÓN
// ===============================
    
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
            // Solo procesar si hay valor
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
                
                // ✅ MEJORA: Scroll al inicio para ver el mensaje de validación
                scrollToTop();
            }
        });
        
        // Validar rango de fecha de archivado
        $('input[name="Filtros.FechaArchivadoDesde"], input[name="Filtros.FechaArchivadoHasta"]').on('change', function() {
            const desde = $('input[name="Filtros.FechaArchivadoDesde"]').val();
            const hasta = $('input[name="Filtros.FechaArchivadoHasta"]').val();
            
            if (desde && hasta && desde > hasta) {
                showNotification('warning', 'La fecha de archivado "Desde" no puede ser mayor que la fecha "Hasta"');
                $(this).val('');
                
                // ✅ MEJORA: Scroll al inicio para ver el mensaje de validación
                scrollToTop();
            }
        });
    }
    
    function initUsuarioArchivoAutocomplete() {
        // TODO: Implementar autocomplete para usuario que archivó
        // cuando se conecte con el servicio de usuarios
        $('input[name="Filtros.UsuarioArchivo"]').on('input', function() {
            const value = $(this).val();
            if (value && value.length >= 2) {
                // Aquí iría la lógica de autocomplete cuando se implemente
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

        // Duplicar Cotización (nueva funcionalidad)
        $('.btn-duplicar').on('click', function(e) {
            e.preventDefault();
            const cotizacionId = $(this).attr('data-id');
            confirmarDuplicacion(cotizacionId);
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
            })
            .fail(function(xhr, status, error) {
                console.error('Error al cargar historial:', error);
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
                
                // Inicializar eventos específicos para versiones desde archivadas
                inicializarEventosVersionesArchivadas();
            })
            .fail(function(xhr, status, error) {
                console.error('Error al cargar versiones:', error);
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
        // Mostrar indicador de carga
        showNotification('info', 'Cargando detalle del archivo de cotización...');
        
        // Navegar a la vista específica de detalle de archivo
        window.location.href = '/Cotizaciones/ArchivoDetalle/' + cotizacionId;
    }
    
    function confirmarReactivacion(cotizacionId) {
        // Configurar el modal de reactivación
        $('#cotizacionIdReactivacion').val(cotizacionId);
        $('#motivoReactivacion').val('');
        $('#contadorMotivoReactivacion').text('0');
        
        // Mostrar el modal
        $('#modalReactivacion').modal('show');
    }

    function confirmarDuplicacion(cotizacionId) {
        // Configurar el modal de confirmación para duplicar
        $('#modalConfirmacionTitulo').html('<i class="fas fa-copy text-primary"></i> Duplicar Cotización Archivada');
        $('#modalConfirmacionMensaje').html(`
            <div class="alert alert-info mb-3">
                <i class="fas fa-info-circle"></i>
                <strong>¿Duplicar cotización ${cotizacionId}?</strong>
            </div>
            <p class="mb-3">
                Se creará una <strong>nueva cotización independiente</strong> basada en los datos 
                de la cotización archivada <strong>${cotizacionId}</strong>.
            </p>
            <div class="alert alert-warning mb-0">
                <i class="fas fa-exclamation-triangle"></i>
                <strong>Importante:</strong> La nueva cotización:
                <ul class="mb-0 mt-2">
                    <li>Tendrá un <strong>código</strong> diferente</li>
                    <li>Estará en estado <strong>Borrador</strong></li>
                    <li>Podrá ser editada normalmente</li>
                    <li>La cotización original permanecerá archivada</li>
                </ul>
            </div>
        `);
        
        // Configurar botón de confirmación
        $('#btnConfirmarAccion').off('click').on('click', function() {
            $('#modalConfirmacion').modal('hide');
            ejecutarDuplicacion(cotizacionId);
        });
        
        // Mostrar modal
        $('#modalConfirmacion').modal('show');
    }
    
    // ===============================
    // UTILIDADES GLOBALES
    // ===============================

    // ✅ NUEVA: Función para hacer scroll suave al inicio de la página
    function scrollToTop(duration = 300) {
        $('html, body').animate({ scrollTop: 0 }, duration);
    }

    // ✅ Hacer la función disponible globalmente
    window.scrollToTop = scrollToTop;

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
            $('#modalConfirmacion').modal('hide');
            
            // ✅ MEJORA: Scroll al inicio antes de navegar para mejor transición
            scrollToTop();
            
            // Pequeña pausa para completar el scroll antes de navegar
            setTimeout(function() {
                window.location.href = limpiarUrl;
            }, 200);
        });
        
        // Mostrar modal
        $('#modalConfirmacion').modal('show');
    }
    
    // ===============================
    // FUNCIONES DE NOTIFICACIÓN
    // ===============================
    
    function showNotification(type, message, duration = 5000) {
        // Usar la función global del sistema si está disponible
        if (typeof window.showNotification === 'function' && window.showNotification !== showNotification) {
            try {
                window.showNotification(type, message);
                return;
            } catch (error) {
                console.warn('Error al usar notificación global, usando fallback local');
            }
        }
        
        // Fallback: Crear notificación toast personalizada
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
    
    // ✅ NUEVA: Función para mostrar alertas dentro de modales
    function showModalAlert(modalId, message, type = 'info') {
        // Usar el ID completo del área de alertas
        const alertId = modalId === 'modalReactivacion' ? 'modalReactivacionAlert' : modalId + 'Alert';
        const contentId = modalId === 'modalReactivacion' ? 'modalReactivacionAlertContent' : modalId + 'AlertContent';
        
        const alertElement = $('#' + alertId);
        const contentElement = $('#' + contentId);
        
        if (alertElement.length && contentElement.length) {
            // Limpiar clases de tipo anterior
            alertElement.removeClass('alert-success alert-danger alert-warning alert-info');
            
            // Agregar clase del tipo actual
            alertElement.addClass('alert-' + (type === 'error' ? 'danger' : type));
            
            // Configurar icono según el tipo
            const iconClass = {
                'success': 'fas fa-check-circle',
                'error': 'fas fa-exclamation-circle', 
                'danger': 'fas fa-exclamation-circle',
                'warning': 'fas fa-exclamation-triangle',
                'info': 'fas fa-info-circle'
            }[type] || 'fas fa-info-circle';
            
            // Insertar contenido con icono
            contentElement.html(`<i class="${iconClass}"></i> <strong>${message}</strong>`);
            
            // Mostrar alerta
            alertElement.removeClass('d-none').addClass('d-block');
            
            // Hacer scroll al inicio del modal para que sea visible
            const modalBody = alertElement.closest('.modal-body');
            if (modalBody.length) {
                modalBody.animate({ scrollTop: 0 }, 300);
            }
        } else {
            console.warn('Área de alertas no encontrada para modal:', modalId);
            // Fallback a notificación global
            showNotification(type === 'danger' ? 'error' : type, message);
        }
    }
    
    // ✅ NUEVA: Función para ocultar alertas de modales
    function hideModalAlert(alertId) {
        const alertElement = $('#' + alertId);
        if (alertElement.length) {
            alertElement.removeClass('d-block').addClass('d-none');
        }
    }
    
    // ✅ Hacer funciones disponibles globalmente
    window.showModalAlert = showModalAlert;
    window.hideModalAlert = hideModalAlert;
    
    // ✅ Función alternativa para mostrar errores sin depender de estructura específica
    window.showReactivationError = function(message) {
        console.log('🚨 Error reactivación:', message);
        
        // 1. SIEMPRE mostrar en notificación global para que se vea
        showNotification('error', message);
        
        // 2. TAMBIÉN mostrar en el modal si está abierto
        const modalBody = $('#modalReactivacion .modal-body');
        if (modalBody.length && $('#modalReactivacion').hasClass('show')) {
            // ✅ CORRECCIÓN: Buscar específicamente alertas de error dinámicas por clase
            let errorAlert = modalBody.find('.alert-danger.dynamic-error').first();
            
            if (errorAlert.length === 0) {
                // ✅ CREAR nueva alerta de error dinámico
                errorAlert = $(`
                    <div class="alert alert-danger alert-dismissible dynamic-error" style="margin-bottom: 15px;">
                        <button type="button" class="close" onclick="$(this).parent().remove()">
                            <span>&times;</span>
                        </button>
                        <i class="fas fa-exclamation-circle"></i> <strong id="reactivation-error-content">${message}</strong>
                    </div>
                `);
                
                // ✅ INSERTAR al INICIO del modal-body, ANTES de cualquier contenido
                // Esto asegura que esté visible sin afectar el contenido estático
                modalBody.prepend(errorAlert);
            } else {
                // ✅ Actualizar mensaje existente
                errorAlert.find('#reactivation-error-content, strong').last().html(message);
            }
            
            // ✅ Scroll al inicio del modal para que sea visible
            modalBody.animate({ scrollTop: 0 }, 300);
        }
    };
    
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
            const errorMessage = 'Debe proporcionar un motivo de al menos 10 caracteres';
            
            // ✅ CORRECCIÓN: Mostrar error SOLO en el modal para que el usuario lo vea
            window.showReactivationError(errorMessage);
            
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
                // ✅ CORRECCIÓN: Siempre restablecer el botón primero
                btn.prop('disabled', false).html('<i class="fas fa-undo"></i> Reactivar Cotización');
                
                if (response && response.success) {
                    // ✅ Éxito: Cerrar modal y mostrar mensaje de éxito
                    $('#modalReactivacion').modal('hide');
                    
                    // 🆕 Mensaje actualizado para reflejar que se creó una nueva cotización
                    const mensaje = response.cotizacionId && response.nuevaVersion
                        ? `Nueva cotización ${response.cotizacionId} creada exitosamente (versión ${response.nuevaVersion})`
                        : (response.message || 'Nueva cotización creada exitosamente por reactivación');
                    
                    showNotification('success', mensaje);
                    
                    // ✅ MEJORA: Scroll al inicio de la página para ver el mensaje
                    scrollToTop();
                    
                    // 🆕 Redirigir al listado normal después de un momento para ver la nueva cotización
                    setTimeout(function() {
                        window.location.href = '/Cotizaciones'; // Ir al listado normal donde aparecerá la nueva cotización
                    }, 2000);
                } else {
                    // ❌ Error del servidor: Mostrar mensaje específico pero mantener modal abierto
                    const errorMessage = response?.message || 'Error desconocido al reactivar la cotización';
                    
                    // ✅ MEJORA: Mostrar error SOLO en el modal para que el usuario lo vea
                    window.showReactivationError(errorMessage);
                    
                    // Enfocar el campo de motivo para que el usuario pueda corregir
                    $('#motivoReactivacion').focus();
                }
            },
            error: function(xhr, status, error) {
                // ✅ CORRECCIÓN: Restablecer botón en caso de error
                btn.prop('disabled', false).html('<i class="fas fa-undo"></i> Reactivar Cotización');
                
                console.error('❌ Error en reactivación:', error);
                console.error('❌ Status:', status);
                console.error('❌ Response:', xhr.responseText);
                
                // ✅ MEJORA: Mensaje de error más específico según el código de estado
                let errorMessage = 'Error al procesar la reactivación';
                
                if (xhr.status === 403) {
                    errorMessage = 'No tiene permisos para reactivar esta cotización';
                } else if (xhr.status === 404) {
                    errorMessage = 'La cotización no fue encontrada';
                } else if (xhr.status === 500) {
                    errorMessage = 'Error interno del servidor. Contacte al administrador';
                } else if (xhr.status === 0) {
                    errorMessage = 'Error de conexión. Verifique su conexión a internet';
                } else if (xhr.responseText) {
                    try {
                        const errorResponse = JSON.parse(xhr.responseText);
                        errorMessage = errorResponse.message || errorMessage;
                    } catch (e) {
                        // Si no es JSON válido, usar el mensaje por defecto
                    }
                }
                
                // ✅ MEJORA: Mostrar error SOLO en el modal para que el usuario lo vea
                window.showReactivationError(errorMessage);
                
                // Enfocar el campo para que el usuario pueda reintentar
                $('#motivoReactivacion').focus();
            }
        });
    });
    
    // Limpiar modal al cerrar
    $('#modalReactivacion').on('hidden.bs.modal', function() {
        $('#motivoReactivacion').val('');
        $('#contadorMotivoReactivacion').text('0').removeClass('text-warning text-danger').addClass('text-muted');
        $('#cotizacionIdReactivacion').val('');
        $('#btnConfirmarReactivacion').prop('disabled', false).html('<i class="fas fa-undo"></i> Reactivar Cotización');
        
        // ✅ CORRECCIÓN: Limpiar solo alertas dinámicas, NO las estáticas
        hideModalAlert('modalReactivacionAlert');
        // ✅ Remover solo alertas de error dinámicas, mantener las estáticas
        $(this).find('.modal-body .alert-danger.dynamic-error').remove();
    });
    
    // ===============================
    // FUNCIONES ESPECÍFICAS PARA VERSIONES ARCHIVADAS
    // ===============================

    function ejecutarDuplicacion(cotizacionId) {
        // Mostrar notificación de progreso
        showNotification('info', 'Duplicando cotización...');
        
        // Enviar solicitud de duplicación
        $.ajax({
            url: '/Cotizaciones/Duplicar',
            type: 'POST',
            data: {
                cotizacionId: cotizacionId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                if (response && response.success) {
                    // ✅ Éxito: Mostrar mensaje de éxito
                    showNotification('success', response.message || 'Cotización duplicada exitosamente');
                    
                    // ✅ MEJORA: Scroll al inicio de la página para ver el mensaje
                    scrollToTop();
                    
                    // Opcional: Redirigir a la nueva cotización
                    if (response.cotizacionId) {
                        setTimeout(function() {
                            // Redirigir al listado normal donde aparecerá la nueva cotización
                            window.location.href = '/Cotizaciones';
                        }, 2000);
                    }
                } else {
                    // ❌ Error del servidor
                    const errorMessage = response?.message || 'Error al duplicar la cotización';
                    showNotification('error', errorMessage);
                    
                    // ✅ MEJORA: Scroll al inicio para ver el error
                    scrollToTop();
                }
            },
            error: function(xhr, status, error) {
                console.error('❌ Error en duplicación:', error);
                console.error('❌ Status:', status);
                console.error('❌ Response:', xhr.responseText);
                
                // ✅ Mensaje de error específico según el código de estado
                let errorMessage = 'Error al procesar la duplicación';
                
                if (xhr.status === 403) {
                    errorMessage = 'No tiene permisos para duplicar esta cotización';
                } else if (xhr.status === 404) {
                    errorMessage = 'La cotización no fue encontrada';
                } else if (xhr.status === 500) {
                    errorMessage = 'Error interno del servidor. Contacte al administrador';
                } else if (xhr.status === 0) {
                    errorMessage = 'Error de conexión. Verifique su conexión a internet';
                } else if (xhr.responseText) {
                    try {
                        const errorResponse = JSON.parse(xhr.responseText);
                        errorMessage = errorResponse.message || errorMessage;
                    } catch (e) {
                        // Si no es JSON válido, usar el mensaje por defecto
                    }
                }
                
                showNotification('error', errorMessage);
                
                // ✅ MEJORA: Scroll al inicio para ver el error
                scrollToTop();
            }
        });
    }
    
    // ===============================
    // FUNCIONES ESPECÍFICAS PARA VERSIONES ARCHIVADAS
    // ===============================
    
    function inicializarEventosVersionesArchivadas() {
        // Manejar click en "Ver Detalle" desde modal de versiones de cotizaciones archivadas
        $('.btn-detalle-version').off('click').on('click', function () {
            const versionId = $(this).attr('data-version-id');
            const cotizacionId = $(this).attr('data-cotizacion-id');
            const fromArchived = $(this).attr('data-from-archived') === 'true';

            // Navegar a la vista de detalle de la versión específica con parámetro de origen
            let url = '/Cotizaciones/Detalle/' + cotizacionId + '?versionId=' + versionId;
            if (fromArchived) {
                url += '&fromArchived=true';
            }
            
            showNotification('info', 'Cargando detalle de versión...');
            window.location.href = url;
        });

        // Manejar click en "Ver Historial" desde modal de versiones de cotizaciones archivadas
        $('.btn-historial-version').off('click').on('click', function () {
            const versionId = $(this).attr('data-version-id');
            const cotizacionId = $(this).attr('data-cotizacion-id');
            const numeroVersion = $(this).attr('data-numero-version');
            
            // Cerrar el modal de versiones primero
            $('#modalVersiones').modal('hide');
            
            // Esperar a que se cierre completamente antes de abrir el historial
            $('#modalVersiones').on('hidden.bs.modal.historialArchivadas', function () {
                // Remover el event listener para evitar múltiples bindings
                $(this).off('hidden.bs.modal.historialArchivadas');
                
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
                $('#modalVersiones').trigger('hidden.bs.modal.historialArchivadas');
            }
        });

        // ✅ NUEVO: Manejar click en "Duplicar" desde modal de versiones de cotizaciones archivadas
        $('.btn-duplicar-version').off('click').on('click', function () {
            const cotizacionId = $(this).attr('data-cotizacion-id');
            const versionId = $(this).attr('data-version-id');
            
            // Cerrar el modal de versiones primero
            $('#modalVersiones').modal('hide');
            
            // Esperar a que se cierre completamente antes de mostrar confirmación
            $('#modalVersiones').on('hidden.bs.modal.duplicarArchivadas', function () {
                // Remover el event listener para evitar múltiples bindings
                $(this).off('hidden.bs.modal.duplicarArchivadas');
                
                // Mostrar confirmación de duplicación
                confirmarDuplicacion(cotizacionId);
            });
            
            // Trigger el cierre si la modal ya está cerrada
            if (!$('#modalVersiones').hasClass('show')) {
                $('#modalVersiones').trigger('hidden.bs.modal.duplicarArchivadas');
            }
        });
    }
    
    // ===============================
    // FUNCIONES GLOBALES (accesibles desde cualquier parte)
    // ===============================
    
    // Hacer la función disponible globalmente para que pueda ser llamada 
    // desde archivos cargados dinámicamente o cuando el modal se reutilice
    window.inicializarEventosVersiones = inicializarEventosVersionesArchivadas;
    window.inicializarEventosVersionesArchivadas = inicializarEventosVersionesArchivadas;
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