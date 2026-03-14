// ============================================
// MODALS - JavaScript global para modales
// ============================================

$(document).ready(function() {
    
    // Los modales se centran automáticamente con CSS
    // Este archivo está disponible para funcionalidades adicionales futuras
    
    // Agregar animación al contenido dinámico
    $('.modal').on('show.bs.modal', function() {
        $(this).find('.modal-content').addClass('fade-in-modal');
    });
    
});