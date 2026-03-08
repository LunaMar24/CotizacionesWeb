// ============================================
// SITE.JS - Funciones globales
// ============================================

// Función global para mostrar notificaciones
window.showNotification = function(type, message) {
    const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    
    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="fas ${icon} mr-2"></i>
            ${message}
            <button type="button" class="close" data-dismiss="alert">
                <span>&times;</span>
            </button>
        </div>
    `;
    
    // Insertar al inicio del content
    const contentElement = $('.content, .content-wrapper .container-fluid').first();
    contentElement.prepend(alertHtml);
    
    // Auto-cerrar después de 5 segundos
    setTimeout(function() {
        $('.alert').fadeOut('slow', function() {
            $(this).remove();
        });
    }, 5000);
};

