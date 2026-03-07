# Lineamientos Técnicos - Manejo de Errores y Logging

## Estrategia
- Serilog
- Logs a archivo
- Logs críticos opcionales en base de datos

## Configuración
- Logs diarios
- Retención 30 días
- Formato JSON

## Niveles
- Information
- Warning
- Error
- Critical

## Reglas
- No usar Console.WriteLine.
- No mostrar stacktrace al usuario.
- Usar middleware global de excepciones.