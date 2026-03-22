// ========================================
// CONFIGURACIÓN CENTRALIZADA DE FORMATO
// ========================================

/**
 * Configuración global para formateo de datos
 * Mantiene consistencia con FormatHelper de C#
 */
window.FormatConfig = window.FormatConfig || {
    // Configuración de monedas (debe coincidir con FormatHelper.cs)
    currencies: {
        'CRC': '\u00A2',    // Colón costarricense (¢ - Unicode: U+00A2)
        'USD': '$',         // Dólar estadounidense 
        'DOL': '$',         // Dólar (alias)
        'EUR': '\u20AC',    // Euro (Unicode: U+20AC)
        'MXN': '$',         // Peso mexicano
        'CAD': '$',         // Dólar canadiense
        'GBP': '\u00A3',    // Libra esterlina (Unicode: U+00A3)
        'JPY': '\u00A5',    // Yen japonés (Unicode: U+00A5)
        'CNY': '\u00A5'     // Yuan chino (Unicode: U+00A5)
    },
    
    // Estados de cotización
    estados: {
        'B': { texto: 'Borrador', editable: true, class: 'badge-secondary-custom' },
        'P': { texto: 'Pendiente Aprobación', editable: false, class: 'badge-warning-custom' },
        'A': { texto: 'Aprobada', editable: false, class: 'badge-success-custom' },
        'E': { texto: 'Enviada', editable: false, class: 'badge-info-custom' },
        'T': { texto: 'Aceptada', editable: false, class: 'badge-primary-custom' },
        'R': { texto: 'Rechazada', editable: false, class: 'badge-danger-custom' },
        'C': { texto: 'Cancelada', editable: false, class: 'badge-warning-custom' },
        'X': { texto: 'Archivada', editable: false, class: 'badge-dark-custom' }
    },
    
    // Reglas de negocio para estados
    transicionesPermitidas: {
        'B': ['P', 'X'], // Borrador ? Pendiente Aprobación, Archivada
        'P': ['A', 'B'], // Pendiente ? Aprobada, Borrador
        'A': ['E'],      // Aprobada ? Enviada
        'E': ['T', 'R'], // Enviada ? Aceptada, Rechazada
        'T': ['X'],      // Aceptada ? Archivada
        'R': ['X'],      // Rechazada ? Archivada
        'X': []          // Archivada ? Sin transiciones
    },
    
    // Configuración por defecto
    defaults: {
        moneda: 'CRC',
        decimales: 2,
        maxNotasLength: 2000
    }
};

/**
 * Funciones de utilidad globales para formato
 */
window.FormatUtils = {
    /**
     * Formatea una cantidad como moneda
     */
    formatCurrency: function(value, currency = null) {
        if (isNaN(value)) {
            const symbol = this.getCurrencySymbol(currency);
            return symbol + '0.00';
        }
        
        const symbol = this.getCurrencySymbol(currency);
        const formattedNumber = parseFloat(value).toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
        
        // IMPORTANTE: No usar template strings para evitar problemas de encoding
        return symbol + formattedNumber;
    },
    
    /**
     * Obtiene el símbolo de moneda
     */
    getCurrencySymbol: function(currency) {
        if (!currency) {
            // Usar moneda desde configuración del servidor o default
            currency = window.FormatConfig?.moneda || window.FormatConfig?.defaults?.moneda || 'CRC';
        }
        
        const upperCurrency = currency.toUpperCase().trim();
        return window.FormatConfig?.currencies?.[upperCurrency] || upperCurrency;
    },
    
    /**
     * Formatea números
     */
    formatNumber: function(value, decimals = 2) {
        if (isNaN(value)) return '0.00';
        return parseFloat(value).toFixed(decimals);
    },
    
    /**
     * Verifica si un estado es editable
     */
    isEditable: function(estado) {
        return window.FormatConfig?.estados?.[estado]?.editable || false;
    },
    
    /**
     * Obtiene el texto de un estado
     */
    getEstadoTexto: function(estado) {
        return window.FormatConfig?.estados?.[estado]?.texto || 'Desconocido';
    },
    
    /**
     * Obtiene la clase CSS de un estado
     */
    getEstadoClass: function(estado) {
        return window.FormatConfig?.estados?.[estado]?.class || 'badge-secondary';
    },
    
    /**
     * Verifica si una transición de estado es permitida
     */
    isTransicionPermitida: function(estadoActual, estadoDestino) {
        const transiciones = window.FormatConfig?.transicionesPermitidas?.[estadoActual] || [];
        return transiciones.includes(estadoDestino);
    }
};

console.log('FormatConfig y FormatUtils cargados correctamente');