// ============================================
// DASHBOARD - JavaScript
// ============================================

$(document).ready(function() {
    
    // ============================================
    // ANIMACIÓN DE NÚMEROS (Counter Animation)
    // ============================================
    
    function animateValue(element, start, end, duration) {
        let startTimestamp = null;
        const step = (timestamp) => {
            if (!startTimestamp) startTimestamp = timestamp;
            const progress = Math.min((timestamp - startTimestamp) / duration, 1);
            const value = Math.floor(progress * (end - start) + start);
            element.textContent = value;
            if (progress < 1) {
                window.requestAnimationFrame(step);
            }
        };
        window.requestAnimationFrame(step);
    }
    
    // Animar los números de las estadísticas
    $('.small-box .inner h3, .info-box-number').each(function() {
        const $this = $(this);
        const text = $this.text().trim();
        
        // Solo animar si es un número
        if (!isNaN(text) && text !== '') {
            const finalValue = parseInt(text);
            $this.text('0');
            animateValue(this, 0, finalValue, 1500);
        }
    });
    
    // ============================================
    // ACTUALIZACIÓN AUTOMÁTICA DEL RELOJ
    // ============================================
    
    function updateClock() {
        const now = new Date();
        const timeString = now.toLocaleTimeString('es-ES', { 
            hour: '2-digit', 
            minute: '2-digit',
            second: '2-digit'
        });
        $('.current-time').text(timeString);
    }
    
    // Actualizar cada segundo
    setInterval(updateClock, 1000);
    updateClock();
    
    // ============================================
    // TOOLTIPS
    // ============================================
    
    $('[data-toggle="tooltip"]').tooltip();
    
    // ============================================
    // MENSAJES DE BIENVENIDA
    // ============================================
    
    function showWelcomeMessage() {
        const hour = new Date().getHours();
        let greeting = '';
        
        if (hour < 12) {
            greeting = '¡Buenos días!';
        } else if (hour < 18) {
            greeting = '¡Buenas tardes!';
        } else {
            greeting = '¡Buenas noches!';
        }
        
        // Solo si existe un elemento para mostrar el saludo
        if ($('.greeting-message').length > 0) {
            $('.greeting-message').text(greeting);
        }
    }
    
    showWelcomeMessage();
});