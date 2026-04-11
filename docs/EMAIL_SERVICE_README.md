# Configuración del Servicio de Email

## Resumen de Implementación

El `EmailService` ha sido actualizado para soportar envío real de correos electrónicos usando SMTP, compatible tanto con desarrollo local como Azure App Service.

## Archivos Modificados

### 1. **Nuevo:** `src/CotizacionesWeb.Infrastructure/Options/EmailOptions.cs`
- Clase de configuración para las opciones de email
- Validación integrada de configuración
- Soporte para habilitación/deshabilitación del servicio

### 2. **Actualizado:** `src/CotizacionesWeb.Infrastructure/Services/EmailService.cs`
- Implementación real usando `System.Net.Mail.SmtpClient`
- Fallback a simulación cuando está deshabilitado
- Manejo robusto de errores con logging detallado
- Validación de configuración y conectividad

### 3. **Actualizado:** `src/CotizacionesWeb.UI/Program.cs`
- Registro de `EmailOptions` con inyección de dependencias
- Configuración usando patrón Options de .NET

### 4. **Actualizado:** `src/CotizacionesWeb.UI/appsettings.json`
- Configuración de email deshabilitada por defecto para desarrollo
- Valores placeholder para todos los parámetros

### 5. **Nuevo:** `src/CotizacionesWeb.UI/appsettings.Production.json`
- Configuración optimizada para producción
- Comentarios indicando variables de entorno de Azure

## Configuración Local (Desarrollo)

### appsettings.json o appsettings.Development.json

```json
{
  "Email": {
    "Enabled": false,                    // Usar simulación en desarrollo
    "SmtpHost": "",                      // Vacío = simulación
    "SmtpPort": 587,
    "SmtpUser": "",
    "SmtpPassword": "",
    "FromAddress": "",
    "FromName": "Sistema de Cotizaciones",
    "EnableSsl": true,
    "TimeoutMs": 30000
  }
}
```

### Para Pruebas Reales en Desarrollo

```json
{
  "Email": {
    "Enabled": true,
    "SmtpHost": "smtp.gmail.com",        // Ejemplo con Gmail
    "SmtpPort": 587,
    "SmtpUser": "tu-email@gmail.com",
    "SmtpPassword": "tu-app-password",   // App Password, no contraseña normal
    "FromAddress": "tu-email@gmail.com",
    "FromName": "Sistema de Cotizaciones - Dev",
    "EnableSsl": true,
    "TimeoutMs": 30000
  }
}
```

## Configuración Azure App Service (Producción)

### Variables de Entorno en Application Settings

En Azure Portal > App Service > Configuration > Application settings:

| **Nombre** | **Valor (Ejemplo)** | **Tipo** |
|------------|---------------------|----------|
| `Email__Enabled` | `true` | String |
| `Email__SmtpHost` | `smtp.office365.com` | String |
| `Email__SmtpPort` | `587` | String |
| `Email__SmtpUser` | `notificaciones@tuempresa.com` | String |
| `Email__SmtpPassword` | `****************` | String (Slot Setting) |
| `Email__FromAddress` | `notificaciones@tuempresa.com` | String |
| `Email__FromName` | `Sistema de Cotizaciones` | String |
| `Email__EnableSsl` | `true` | String |
| `Email__TimeoutMs` | `45000` | String |

### Configuración Recomendada por Proveedor

#### Gmail/Google Workspace
```
SmtpHost: smtp.gmail.com
SmtpPort: 587
EnableSsl: true
SmtpUser: tu-email@gmail.com
SmtpPassword: [App Password]
```

#### Microsoft 365/Outlook
```
SmtpHost: smtp.office365.com
SmtpPort: 587
EnableSsl: true
SmtpUser: tu-email@empresa.com
SmtpPassword: [Password o App Token]
```

#### SendGrid
```
SmtpHost: smtp.sendgrid.net
SmtpPort: 587
EnableSsl: true
SmtpUser: apikey
SmtpPassword: [API Key]
```

## Comportamiento del Sistema

### Modo Simulación (Enabled = false)
- ? Logs detallados de emails "enviados"
- ? No requiere configuración SMTP
- ? Ideal para desarrollo y testing
- ? Simula fallos ocasionales (2% tasa de fallo)

### Modo Real (Enabled = true)
- ? Envío real vía SMTP configurado
- ? Validación de configuración antes del envío
- ? Verificación de conectividad en `EstaDisponibleAsync()`
- ? Manejo robusto de errores SMTP
- ? Logging detallado de éxitos y fallos

### Validaciones Implementadas
- ? Formato válido de email destinatario
- ? Configuración completa y válida
- ? Conectividad con servidor SMTP
- ? Timeout configurable para evitar bloqueos

## Integración con NotificacionCotizacionBackgroundService

? **Compatible:** El servicio mantiene la misma interfaz `IEmailService`

? **Robusto:** No lanza excepciones que puedan romper el BackgroundService

? **Logging:** Proporciona información detallada para debugging

? **Performante:** Timeouts configurables y manejo asíncrono

## Seguridad

### Variables de Entorno
- ? No hardcodea credenciales
- ? Usa Azure Application Settings
- ? Password como Slot Setting (no se copia entre slots)

### Conexiones
- ? SSL/TLS habilitado por defecto
- ? Credenciales seguras vía NetworkCredential
- ? Timeout configurable para evitar bloqueos

## Logs y Monitoreo

### Logs Informativos
```
? Email enviado exitosamente a usuario@ejemplo.com
?? [SIMULADO] Email enviado exitosamente a usuario@ejemplo.com
?? Servicio de email en modo simulación
```

### Logs de Error
```
? Error SMTP al enviar email a usuario@ejemplo.com: MailboxUnavailable
?? Configuración de email inválida
? Timeout al verificar conectividad SMTP con smtp.ejemplo.com:587
```

## Testing

### Verificar Configuración Local
```csharp
// En desarrollo, verificar que funciona la simulación
var emailService = serviceProvider.GetService<IEmailService>();
var disponible = await emailService.EstaDisponibleAsync();
var enviado = await emailService.EnviarEmailAsync("test@example.com", "Test", "Contenido");
```

### Verificar Configuración Azure
1. Verificar logs en Azure Portal > App Service > Log stream
2. Buscar logs con pattern "Email enviado" o "Error SMTP"
3. Probar desde una acción que triggee notificaciones

## Resolución de Problemas

### Email no se envía
1. ? Verificar `Email__Enabled = true` en Azure
2. ? Verificar credenciales SMTP correctas
3. ? Verificar que el servidor SMTP permite conexiones externas
4. ? Revisar logs para errores específicos

### Timeout al conectar
1. ? Verificar que Azure App Service puede acceder al servidor SMTP
2. ? Aumentar `TimeoutMs` si es necesario
3. ? Verificar firewall del proveedor SMTP

### Credenciales incorrectas
1. ? Gmail: Usar App Password, no contraseña normal
2. ? Office 365: Verificar que SMTP está habilitado
3. ? Verificar formato de usuario (email completo vs username)

Esta implementación es robusta, segura y completamente compatible con la arquitectura existente.