# Manual de Usuario - Sistema CotizacionesWeb

---

## 📋 Índice

1. [Introducción](#1-introducción)
2. [Objetivo del Sistema](#2-objetivo-del-sistema)
3. [Requisitos del Sistema](#3-requisitos-del-sistema)
4. [Acceso al Sistema](#4-acceso-al-sistema)
5. [Descripción de Módulos](#5-descripción-de-módulos)
   - 5.1 [Dashboard](#51-dashboard)
   - 5.2 [Administración](#52-administración)
     - 5.2.1 [Gestión de Usuarios](#521-gestión-de-usuarios)
     - 5.2.2 [Gestión de Roles](#522-gestión-de-roles)
   - 5.3 [Cotizaciones](#53-cotizaciones)
     - 5.3.1 [Gestión de Cotizaciones](#531-gestión-de-cotizaciones)
     - 5.3.2 [Nueva Cotización](#532-nueva-cotización)
     - 5.3.3 [Cotizaciones Archivadas](#533-cotizaciones-archivadas)
   - 5.4 [Configuración](#54-configuración)
     - 5.4.1 [Parámetros del Sistema](#541-parámetros-del-sistema)
6. [Errores Comunes y Soluciones](#6-errores-comunes-y-soluciones)
7. [Conclusión](#7-conclusión)

---

## 1. Introducción

Bienvenido al **Manual de Usuario del Sistema CotizacionesWeb**, una herramienta diseñada para facilitar y optimizar la gestión de cotizaciones en su organización.

Este manual ha sido elaborado con el objetivo de proporcionar una guía completa y de fácil comprensión para todos los usuarios del sistema, independientemente de su nivel de conocimientos técnicos. A través de instrucciones paso a paso, capturas ilustrativas , podrá aprender a utilizar todas las funcionalidades que CotizacionesWeb ofrece.

### ¿Qué encontrará en este manual?

El documento está estructurado de manera lógica para facilitar su navegación:

- **Información general**: Requisitos del sistema y proceso de acceso
- **Descripción detallada de módulos**: Cada sección del sistema explicada con sus funcionalidades
- **Guías paso a paso**: Instrucciones claras para realizar cada operación
- **Solución de problemas**: Respuestas a errores comunes y sus soluciones

### ¿A quién está dirigido?

Este manual está diseñado para:

- **Usuarios finales**: Personal que crea, modifica y gestiona cotizaciones en su día a día
- **Administradores del sistema**: Responsables de la gestión de usuarios, roles y permisos
- **Personal de configuración**: Encargados de mantener los parámetros del sistema actualizados

### ¿Cómo usar este manual?

- Si es la **primera vez** que utiliza el sistema, le recomendamos leer las secciones en orden secuencial
- Si busca realizar una **tarea específica**, puede utilizar el índice para navegar directamente a la sección correspondiente
- Si encuentra algún **problema o error**, consulte la sección de "Errores Comunes y Soluciones"

### Convenciones utilizadas

A lo largo de este manual, se utilizan los siguientes iconos para facilitar la lectura:

- ✅ **Nota importante**: Información relevante que debe tener en cuenta
- ⚠️ **Advertencia**: Precauciones que debe tomar antes de realizar una acción
- 💡 **Sugerencia**: Consejos útiles para optimizar su trabajo
- 📝 **Ejemplo**: Casos prácticos de uso

Este manual es un documento vivo que se actualiza periódicamente para reflejar mejoras y nuevas funcionalidades del sistema. Consulte siempre la versión más reciente disponible.

---

## 2. Objetivo del Sistema

**CotizacionesWeb** es un sistema web integral diseñado para **optimizar y centralizar la gestión de cotizaciones** de su organización, proporcionando un control completo sobre el proceso de cotización desde su creación hasta su aprobación final.

### Propósito Principal

El sistema tiene como objetivo principal **simplificar y agilizar** el proceso de elaboración, seguimiento y control de cotizaciones comerciales, permitiendo a su equipo:

- Crear y gestionar cotizaciones de manera rápida y eficiente
- Mantener un registro histórico completo de todas las versiones de cada cotización
- Controlar los estados y flujos de aprobación de las cotizaciones
- Gestionar múltiples monedas (Colón Costarricense, Dólar Estadounidense, etc.)
- Administrar permisos y accesos mediante un sistema robusto de usuarios y roles

### Objetivos Específicos

#### 📊 **Gestión Eficiente de Cotizaciones**
- **Crear** cotizaciones con información detallada de productos/servicios, cantidades, precios y descuentos
- **Modificar** cotizaciones existentes manteniendo un historial de cambios
- **Copiar y duplicar** cotizaciones para reutilizar información y agilizar el trabajo
- **Versionar** cotizaciones para llevar un registro de todas las modificaciones realizadas
- **Archivar** cotizaciones finalizadas manteniendo el sistema organizado

#### 👥 **Administración de Usuarios y Seguridad**
- Gestionar usuarios del sistema con información completa y actualizada
- Asignar roles específicos que definen qué puede hacer cada usuario
- Resetear contraseñas de forma segura cuando sea necesario
- Controlar el acceso a diferentes módulos según los permisos asignados

#### 🔐 **Control de Permisos Granular**
- Definir roles personalizados según las necesidades de la organización
- Asociar permisos específicos a cada rol (crear, modificar, eliminar, visualizar)
- Garantizar que cada usuario solo tenga acceso a las funciones necesarias para su trabajo
- Mantener la seguridad de la información sensible

#### ⚙️ **Configuración Centralizada**
- Administrar parámetros del sistema desde una ubicación única
- Personalizar el comportamiento del sistema según las necesidades del negocio
- Mantener configuraciones actualizadas sin intervención técnica

### Beneficios para la Organización

✅ **Eficiencia Operativa**: Reducción del tiempo necesario para crear y gestionar cotizaciones

✅ **Trazabilidad Completa**: Registro histórico de todas las modificaciones y versiones de cada cotización

✅ **Control de Accesos**: Seguridad mejorada mediante gestión de usuarios, roles y permisos

✅ **Reducción de Errores**: Validaciones automáticas que previenen errores comunes

✅ **Centralización de Información**: Toda la información de cotizaciones en un solo lugar accesible

✅ **Soporte Multi-Moneda**: Manejo de diferentes monedas según las necesidades comerciales

✅ **Auditoría**: Capacidad de rastrear quién hizo qué y cuándo en cada cotización

### Alcance del Sistema

El sistema CotizacionesWeb abarca los siguientes procesos:

1. **Gestión completa del ciclo de vida** de las cotizaciones (desde borrador hasta aprobación)
2. **Administración de usuarios** y sus accesos al sistema
3. **Configuración de roles** y permisos personalizados
4. **Mantenimiento de parámetros** del sistema
5. **Generación de versiones** de cotizaciones con historial completo
6. **Archivo de cotizaciones** finalizadas para mantener el sistema organizado

Este sistema está diseñado para adaptarse a las necesidades de organizaciones de diferentes tamaños, desde pequeñas empresas hasta grandes corporaciones que requieren un control detallado de sus procesos de cotización.

---

## 3. Requisitos del Sistema

Para utilizar **CotizacionesWeb** de manera óptima, su equipo de cómputo y conexión a internet deben cumplir con ciertos requisitos mínimos. A continuación, se detallan las especificaciones necesarias para acceder y trabajar con el sistema sin inconvenientes.

### 💻 Requisitos de Hardware

#### **Mínimos (Básico)**
- **Procesador**: Intel Core i3 o equivalente
- **Memoria RAM**: 4 GB
- **Espacio en disco**: 500 MB de espacio libre
- **Resolución de pantalla**: 1366 x 768 píxeles

#### **Recomendados (Óptimo)**
- **Procesador**: Intel Core i5 o superior
- **Memoria RAM**: 8 GB o más
- **Espacio en disco**: 1 GB de espacio libre
- **Resolución de pantalla**: 1920 x 1080 píxeles (Full HD)

💡 **Sugerencia**: Una resolución de pantalla mayor facilita la visualización de tablas y formularios con múltiples campos.

### 🌐 Requisitos de Software

#### **Navegadores Web Compatibles**

El sistema **CotizacionesWeb** es compatible con las versiones más recientes de los siguientes navegadores:

| Navegador | Versión Mínima | Recomendación |
|-----------|---------------|---------------|
| **Google Chrome** | 90 o superior | ✅ Recomendado |
| **Microsoft Edge** | 90 o superior | ✅ Recomendado |
| **Mozilla Firefox** | 88 o superior | ✅ Recomendado |
| **Safari** | 14 o superior | Compatible |
| **Opera** | 76 o superior | Compatible |

✅ **Nota importante**: Se recomienda mantener su navegador **siempre actualizado** para garantizar la mejor experiencia de usuario y la seguridad del sistema.

⚠️ **Advertencia**: Navegadores obsoletos o versiones antiguas pueden presentar problemas de visualización o funcionalidades limitadas.

#### **Sistema Operativo**

El sistema es accesible desde cualquier sistema operativo que soporte los navegadores mencionados:

- ✅ **Windows** 10 o superior
- ✅ **macOS** 10.14 (Mojave) o superior
- ✅ **Linux** (distribuciones modernas con navegadores actualizados)
- ✅ **Chrome OS** (Chromebooks)

### 🔌 Requisitos de Conectividad

#### **Conexión a Internet**

- **Velocidad mínima**: 2 Mbps (megabits por segundo)
- **Velocidad recomendada**: 5 Mbps o superior
- **Tipo de conexión**: Cableada (LAN) o inalámbrica (WiFi) estable

💡 **Sugerencia**: Una conexión estable y rápida mejora significativamente la experiencia al cargar cotizaciones con múltiples líneas de detalle.

⚠️ **Advertencia**: Conexiones inestables pueden causar la pérdida de datos no guardados. Se recomienda guardar frecuentemente su trabajo.

### 📱 Dispositivos Móviles (Opcional)

Aunque el sistema está optimizado para uso en computadoras de escritorio y portátiles, también es accesible desde dispositivos móviles:

- **Tablets**: iPad, Samsung Galaxy Tab, u otros tablets con pantalla de 10" o superior
- **Smartphones**: Compatible, pero se recomienda para consultas rápidas únicamente (la creación/edición de cotizaciones es más eficiente en pantallas grandes)

✅ **Nota importante**: Para tareas complejas como crear cotizaciones con múltiples líneas, se recomienda usar una computadora de escritorio o portátil.

### 👤 Requisitos de Usuario

Para poder utilizar el sistema, cada usuario necesita:

#### **Credenciales de Acceso**
- ✅ **Nombre de usuario** o **correo electrónico** asignado por el administrador del sistema
- ✅ **Contraseña** personal y segura

#### **Permisos y Roles**
- ✅ Tener un **rol asignado** por el administrador del sistema
- ✅ Contar con los **permisos necesarios** para las funcionalidades que necesita utilizar

💡 **Sugerencia**: Si no tiene credenciales de acceso o necesita permisos adicionales, contacte al administrador del sistema de su organización.

### 🔐 Requisitos de Seguridad

Para mantener la seguridad de su cuenta y la información del sistema:

- ✅ **No compartir** su contraseña con otras personas
- ✅ **Cerrar sesión** al terminar de usar el sistema, especialmente en computadoras compartidas
- ✅ **Utilizar contraseñas seguras** que combinen letras mayúsculas, minúsculas, números y símbolos
- ✅ **Cambiar su contraseña periódicamente** (se recomienda cada 90 días)

### ⚙️ Configuraciones Recomendadas del Navegador

Para una experiencia óptima, configure su navegador de la siguiente manera:

- ✅ **Habilitar JavaScript**: El sistema requiere JavaScript para funcionar correctamente
- ✅ **Permitir cookies**: Necesarias para mantener su sesión activa
- ✅ **Deshabilitar bloqueadores de ventanas emergentes** para este sitio (algunas funcionalidades usan modales)
- ✅ **Permitir almacenamiento local**: Mejora el rendimiento del sistema

### 📋 Resumen de Requisitos Esenciales

| Requisito | Especificación |
|-----------|---------------|
| **Navegador** | Chrome, Edge o Firefox actualizado |
| **Internet** | Conexión estable de 2 Mbps mínimo |
| **Resolución** | 1366 x 768 píxeles mínimo |
| **RAM** | 4 GB mínimo (8 GB recomendado) |
| **Credenciales** | Usuario y contraseña asignados |

📝 **Ejemplo**: Un usuario típico puede trabajar eficientemente con una laptop con Windows 11, 8GB de RAM, Google Chrome actualizado y una conexión WiFi de 10 Mbps.

---

Si su equipo cumple con estos requisitos, está listo para acceder al sistema y comenzar a trabajar con CotizacionesWeb.

---

## 4. Acceso al Sistema

Para comenzar a utilizar **CotizacionesWeb**, es necesario acceder al sistema mediante el proceso de autenticación. Esta sección le guiará paso a paso en el proceso de inicio de sesión.

### 4.1 Inicio de Sesión (Login)

El inicio de sesión es el primer paso para acceder al sistema CotizacionesWeb. Debe contar con credenciales válidas proporcionadas por el administrador del sistema.

#### **¿Qué necesita para iniciar sesión?**

Antes de comenzar, asegúrese de tener:

✅ **Correo electrónico** registrado en el sistema
✅ **Contraseña** asignada por el administrador o la que haya establecido
✅ **Conexión a internet** activa
✅ **Navegador web** actualizado (Chrome, Edge o Firefox recomendados)

---

#### **Pantalla de Inicio de Sesión**

Al acceder a la dirección web del sistema, se mostrará la pantalla de inicio de sesión con los siguientes elementos:

**Elementos de la pantalla:**

1. **Logotipo de la organización**: APLIX Logo
2. **Título del sistema**: "CotizacionesWeb"
3. **Subtítulo**: "Sistema de Gestión de Cotizaciones"
4. **Formulario de acceso**:
   - Campo "Correo Electrónico" (con icono de sobre)
   - Campo "Contraseña" (con icono de llave)
   - Botón "Ingresar" (con icono de inicio de sesión)

---

#### **Pasos para Iniciar Sesión**

Siga estos pasos para acceder al sistema:

##### **Paso 1: Acceder a la página de inicio de sesión**

1. Abra su navegador web (Chrome, Edge o Firefox)
2. Ingrese la dirección (URL) del sistema CotizacionesWeb proporcionada por su organización
3. Presione **Enter** para cargar la página

💡 **Sugerencia**: Agregue la página a marcadores/favoritos para acceder rápidamente en el futuro.

---

##### **Paso 2: Ingresar sus credenciales**

1. En el campo **"Correo Electrónico"**:
   - Haga clic en el campo de texto
   - Escriba su correo electrónico completo (ejemplo: `usuario@ejemplo.com`)
   - Asegúrese de escribirlo correctamente, respetando mayúsculas y minúsculas si aplica

2. En el campo **"Contraseña"**:
   - Haga clic en el campo de contraseña
   - Escriba su contraseña
   - Los caracteres se mostrarán como puntos (••••••••) por seguridad
   - Verifique que la tecla **Bloq Mayús** no esté activada si su contraseña no usa mayúsculas

📝 **Ejemplo**:
```
Correo Electrónico: juan.perez@empresa.com
Contraseña: ••••••••
```

⚠️ **Advertencia**: El sistema distingue entre mayúsculas y minúsculas en la contraseña. Asegúrese de escribirla exactamente como fue configurada.

---

##### **Paso 3: Iniciar sesión**

1. Haga clic en el botón **"Ingresar"** (azul con icono de inicio de sesión)
2. El sistema validará sus credenciales
3. Si los datos son correctos:
   - Será redirigido automáticamente al **Dashboard** (página principal)
   - Verá su nombre de usuario en la esquina superior derecha
   - Podrá comenzar a trabajar con el sistema

---

#### **Mensajes de Error Comunes**

Si las credenciales son incorrectas o hay algún problema, el sistema mostrará un mensaje de error. A continuación, los errores más comunes y sus soluciones:

##### **❌ Error: "Error al iniciar sesión"**

**Causa**: Correo electrónico o contraseña incorrectos

**Solución**:
1. Verifique que el correo electrónico esté escrito correctamente
2. Asegúrese de que la contraseña sea la correcta
3. Revise que la tecla **Bloq Mayús** no esté activada
4. Intente nuevamente

---

##### **❌ Error: "El campo Correo Electrónico es requerido"**

**Causa**: No ingresó el correo electrónico

**Solución**:
1. Complete el campo de correo electrónico
2. Intente iniciar sesión nuevamente

---

##### **❌ Error: "El campo Contraseña es requerido"**

**Causa**: No ingresó la contraseña

**Solución**:
1. Complete el campo de contraseña
2. Intente iniciar sesión nuevamente

---

##### **❌ Error: "Formato de correo electrónico inválido"**

**Causa**: El correo ingresado no tiene un formato válido

**Solución**:
1. Verifique que el correo tenga el formato: `usuario@dominio.com`
2. Asegúrese de incluir el símbolo `@` y el dominio completo
3. Intente nuevamente

---

#### **Consideraciones de Seguridad**

Para mantener la seguridad de su cuenta, tenga en cuenta las siguientes recomendaciones:

🔒 **Buenas prácticas de seguridad:**

1. **No comparta su contraseña** con otras personas bajo ninguna circunstancia
2. **No anote su contraseña** en lugares visibles (notas adhesivas, documentos sin protección)
3. **Cierre sesión** al terminar de trabajar, especialmente si usa una computadora compartida
4. **No active "Recordar contraseña"** en navegadores de computadoras públicas o compartidas
5. **Cambie su contraseña periódicamente** (recomendado cada 90 días)
6. **Use contraseñas seguras**:
   - Mínimo 8 caracteres
   - Combine letras mayúsculas y minúsculas
   - Incluya números
   - Incluya caracteres especiales (@, #, $, %, etc.)

---

#### **Después de Iniciar Sesión**

Una vez que haya iniciado sesión correctamente:

1. **Será redirigido al Dashboard** (página principal del sistema)
2. **Verá el menú de navegación** en el lateral izquierdo con las opciones disponibles según sus permisos
3. **Su nombre de usuario** aparecerá en la esquina superior derecha
4. **Podrá comenzar a trabajar** con los módulos a los que tenga acceso

💡 **Sugerencia**: Familiarícese con el Dashboard antes de comenzar a crear cotizaciones. Esto le ayudará a entender la estructura del sistema.

---

#### **Cerrar Sesión**

Para cerrar sesión de forma segura:

1. Haga clic en su **nombre de usuario** en la esquina superior derecha
2. Seleccione la opción **"Cerrar Sesión"** o **"Salir"** del menú desplegable
3. Será redirigido automáticamente a la pantalla de inicio de sesión
4. Su sesión quedará cerrada de forma segura

⚠️ **Advertencia**: Siempre cierre sesión antes de cerrar el navegador o alejarse de la computadora, especialmente en equipos compartidos.

---

### 4.2 Recuperación de Contraseña

Actualmente, el sistema **CotizacionesWeb** no cuenta con un módulo automatizado de recuperación de contraseña por motivos de seguridad.

#### **¿Olvidó su contraseña?**

Si olvidó su contraseña o no puede acceder al sistema, siga estos pasos:

##### **Paso 1: Contactar al Administrador del Sistema**

1. **Identifique** quién es el administrador del sistema en su organización
2. **Comuníquese** con el administrador mediante:
   - Correo electrónico corporativo
   - Llamada telefónica interna
   - Sistema de tickets de soporte (si existe)
   - Cualquier otro canal oficial de comunicación

##### **Paso 2: Solicitar Reseteo de Contraseña**

1. **Proporcione** al administrador la siguiente información:
   - Su nombre completo
   - Correo electrónico registrado en el sistema
   - Departamento o área de trabajo
   - Razón de la solicitud (olvidó contraseña, primera vez ingresando, etc.)

📝 **Ejemplo de solicitud**:
```
Asunto: Solicitud de reseteo de contraseña - CotizacionesWeb

Estimado Administrador:

Por medio de la presente, solicito el reseteo de mi contraseña para 
acceder al sistema CotizacionesWeb.

Datos del usuario:
- Nombre: Juan Pérez García
- Correo: juan.perez@empresa.com
- Departamento: Ventas
- Motivo: Olvidé mi contraseña de acceso

Quedo atento a su respuesta.

Saludos cordiales,
Juan Pérez
```

##### **Paso 3: Recibir Nueva Contraseña**

1. El administrador **reseteará su contraseña** en el sistema
2. Recibirá una **nueva contraseña temporal** mediante un canal seguro:
   - Correo electrónico
   - Mensaje directo
   - Llamada telefónica
3. **Guarde** la nueva contraseña de forma segura

##### **Paso 4: Iniciar Sesión con Nueva Contraseña**

1. Acceda a la página de inicio de sesión
2. Ingrese su correo electrónico
3. Ingrese la **contraseña ** proporcionada por el administrador
4. Haga clic en "Ingresar"

💡 **Sugerencia**: Una vez que haya ingresado con la contraseña temporal, se recomienda cambiarla inmediatamente por una contraseña personal y segura que solo usted conozca.

---

#### **Cambio de Contraseña (Después de Recuperación)**

Aunque el sistema actual no incluye un módulo de cambio de contraseña por parte del usuario, como buena práctica de seguridad:

1. **Solicite al administrador** cambiar la contraseña temporal por una permanente
2. **Proporcione** una nueva contraseña segura que cumpla con los requisitos:
   - Mínimo 8 caracteres
   - Letras mayúsculas y minúsculas
   - Números
   - Caracteres especiales

📝 **Ejemplo de contraseña segura**: `Aplix2026@Cotiz`

⚠️ **Advertencia**: No reutilice contraseñas de otros sistemas o servicios. Cada sistema debe tener una contraseña única.

---

#### **Prevención de Problemas de Acceso**

Para evitar situaciones de recuperación de contraseña en el futuro:

✅ **Anote su contraseña de forma segura**:
- Use un administrador de contraseñas (LastPass, 1Password, Bitwarden)
- Guárdela en un documento cifrado
- Anótela en un lugar seguro fuera de su estación de trabajo

✅ **Practique el acceso regularmente** si no usa el sistema con frecuencia

✅ **Actualice su información de contacto** con el administrador para facilitar futuras comunicaciones

✅ **Familiarícese con el proceso** de recuperación de su organización

---

#### **Contacto de Soporte**

Si tiene problemas para acceder al sistema o necesita asistencia adicional:

📧 **Contacte al Administrador del Sistema** de su organización

📞 **Mesa de ayuda** (si su organización cuenta con una)

💡 **Sugerencia**: Mantenga a la mano los datos de contacto del administrador del sistema para solicitudes urgentes.

---

## 5. Descripción de Módulos

El sistema **CotizacionesWeb** está organizado en módulos funcionales que le permiten gestionar diferentes aspectos del proceso de cotización. Esta sección describe cada módulo en detalle, las pantallas que lo componen y las acciones que puede realizar.

### Navegación General del Sistema

Después de iniciar sesión, encontrará la interfaz principal del sistema que consta de los siguientes elementos:

#### **Elementos de la Interfaz**

1. **Barra Superior (Navbar)**:
   - Botón de menú (☰) para mostrar/ocultar el menú lateral
   - Nombre de usuario y foto de perfil en la esquina superior derecha
   - Menú desplegable con opciones de usuario

2. **Menú Lateral Izquierdo (Sidebar)**:
   - Logotipo de APLIX
   - Nombre del sistema "CotizacionesWeb"
   - Opciones de navegación organizadas por módulos:
     - **Inicio** (Dashboard)
     - **Administración** (Usuarios y Roles)
     - **Cotizaciones** (Gestión, Nueva Cotización, Archivadas)
     - **Configuración** (Parámetros del Sistema)

3. **Área de Contenido Central**:
   - Muestra el contenido del módulo seleccionado
   - Varía según la sección en la que se encuentre

4. **Pie de Página (Footer)**:
   - Información de derechos de autor
   - Versión del sistema (1.0.0)

💡 **Sugerencia**: El menú lateral puede ocultarse o mostrarse haciendo clic en el icono de menú (☰) para obtener más espacio de trabajo.

⚠️ **Advertencia**: Las opciones del menú que vea dependerán de los permisos asignados a su usuario. No todos los usuarios verán todas las opciones.

---

### 5.1 Dashboard

El **Dashboard** es la pantalla principal del sistema y lo primero que verá después de iniciar sesión. Proporciona una vista general del estado de las cotizaciones en su organización.

#### 5.1.1 Pantalla Principal

Al acceder al Dashboard, encontrará:

**Encabezado de la Página:**
- **Título**: "Dashboard" con icono de velocímetro
- **Mensaje de bienvenida**: "Bienvenido, [Su Nombre]. Aquí tienes un resumen de las cotizaciones por estado."

**Área Principal:**
El Dashboard muestra tarjetas (cards) con estadísticas visuales de las cotizaciones organizadas por su estado actual.

---

#### 5.1.2 Indicadores y Métricas

El Dashboard presenta **6 tarjetas de estadísticas**, cada una representando un estado diferente de las cotizaciones:

##### **1. Borrador (Gris)**
- **Color**: Gris/Secundario
- **Icono**: 📄 Documento
- **Descripción**: Cotizaciones que están en proceso de creación y aún no han sido enviadas para aprobación
- **Acción**: Hacer clic en "Ver más" para ver todas las cotizaciones en estado Borrador

📝 **Ejemplo**: Si el número es "15", significa que tiene 15 cotizaciones guardadas como borrador que pueden ser editadas.

---

##### **2. Pendiente Aprobación (Amarillo)**
- **Color**: Amarillo/Warning
- **Icono**: ⏰ Reloj
- **Descripción**: Cotizaciones que han sido enviadas y están esperando aprobación de un supervisor o gerente
- **Acción**: Hacer clic en "Ver más" para ver todas las cotizaciones pendientes de aprobación

💡 **Sugerencia**: Este indicador es importante para supervisores que necesitan revisar y aprobar cotizaciones.

---

##### **3. Aprobada (Verde)**
- **Color**: Verde/Success
- **Icono**: ✓ Marca de verificación
- **Descripción**: Cotizaciones que han sido revisadas y aprobadas, listas para ser enviadas al cliente
- **Acción**: Hacer clic en "Ver más" para ver todas las cotizaciones aprobadas

✅ **Nota importante**: Una cotización aprobada puede ser enviada al cliente.

---

##### **4. Enviada (Azul Claro)**
- **Color**: Azul claro/Info
- **Icono**: ✉️ Avión de papel (envío)
- **Descripción**: Cotizaciones que ya han sido enviadas al cliente y están esperando respuesta
- **Acción**: Hacer clic en "Ver más" para ver todas las cotizaciones enviadas

📝 **Ejemplo**: Una cotización pasa a este estado después de que se envía el documento al cliente por correo electrónico.

---

##### **5. Aceptada (Azul Oscuro)**
- **Color**: Azul oscuro/Primary
- **Icono**: 👍 Pulgar arriba
- **Descripción**: Cotizaciones que el cliente ha aceptado formalmente
- **Acción**: Hacer clic en "Ver más" para ver todas las cotizaciones aceptadas

✅ **Nota importante**: Este es un estado exitoso que indica que el cliente ha decidido proceder con la propuesta.

---

##### **6. Rechazada (Rojo)**
- **Color**: Rojo/Danger
- **Icono**: ✖️ X en círculo
- **Descripción**: Cotizaciones que el cliente ha rechazado o declinado
- **Acción**: Hacer clic en "Ver más" para ver todas las cotizaciones rechazadas

💡 **Sugerencia**: Las cotizaciones rechazadas pueden servir como referencia histórica o base para nuevas propuestas.

---

#### **Cómo Usar el Dashboard**

##### **Interpretar las Métricas**

1. **Revisar los números**: Cada tarjeta muestra la cantidad total de cotizaciones en ese estado
2. **Identificar prioridades**: 
   - Muchas cotizaciones en "Pendiente Aprobación" pueden requerir atención inmediata
   - Muchas en "Borrador" pueden indicar trabajo pendiente de completar
3. **Acceder a detalles**: Haga clic en "Ver más" en cualquier tarjeta para ver la lista filtrada

##### **Acciones Disponibles desde el Dashboard**

1. **Ver Lista Filtrada**: 
   - Haga clic en el enlace "Ver más" de cualquier tarjeta
   - Será dirigido a la lista de cotizaciones filtrada por ese estado específico

2. **Navegar a Otros Módulos**:
   - Use el menú lateral izquierdo para acceder a otras funcionalidades
   - El Dashboard siempre está disponible haciendo clic en "Inicio"

##### **Ejemplo de Uso Práctico**

📝 **Escenario**: Es lunes por la mañana y quiere revisar el trabajo pendiente

1. **Inicie sesión** en el sistema
2. **Observe el Dashboard**:
   - Borrador: 5 cotizaciones
   - Pendiente Aprobación: 3 cotizaciones
   - Aprobada: 2 cotizaciones
   - Enviada: 8 cotizaciones
   - Aceptada: 4 cotizaciones
   - Rechazada: 1 cotización

3. **Interprete la información**:
   - Tiene 5 cotizaciones que necesita completar (Borrador)
   - Hay 3 cotizaciones esperando su aprobación (si es supervisor)
   - 8 cotizaciones están esperando respuesta del cliente

4. **Tome acción**:
   - Haga clic en "Ver más" en "Pendiente Aprobación" para revisar las que necesitan aprobación
   - O haga clic en "Borrador" para continuar trabajando en las cotizaciones incompletas

---

#### **Beneficios del Dashboard**

✅ **Vista Rápida**: Vea el estado general de todas las cotizaciones en segundos

✅ **Identificación de Cuellos de Botella**: Detecte rápidamente si hay muchas cotizaciones pendientes en algún estado

✅ **Acceso Directo**: Navegue directamente a las cotizaciones que necesita revisar

✅ **Toma de Decisiones**: Base sus prioridades diarias en las métricas mostradas

✅ **Seguimiento Visual**: Los colores facilitan la identificación rápida de cada estado

---

#### **Actualización de Datos**

⚠️ **Advertencia**: Los datos del Dashboard se actualizan cada vez que accede a la página. Si hace cambios en cotizaciones, regrese al Dashboard para ver las estadísticas actualizadas.

💡 **Sugerencia**: Actualice la página (F5 o Ctrl+R) si sospecha que los números no están actualizados.

---

### 5.2 Administración

El módulo de **Administración** permite gestionar los usuarios del sistema y los roles que definen sus permisos. Este módulo es fundamental para mantener la seguridad y el control de acceso en CotizacionesWeb.

⚠️ **Advertencia**: Este módulo solo está disponible para usuarios con permisos de administración. Si no puede ver este menú, contacte al administrador del sistema.

---

#### 5.2.1 Gestión de Usuarios

La **Gestión de Usuarios** permite crear, modificar, asignar roles y administrar las cuentas de usuario del sistema.

##### **Acceso al Módulo**

Para acceder a la gestión de usuarios:

1. En el **menú lateral izquierdo**, haga clic en **"Administración"**
2. Seleccione **"Usuarios"**
3. Se mostrará la pantalla de lista de usuarios

---

##### Pantalla de Lista de Usuarios

La pantalla principal de usuarios muestra todos los usuarios registrados en el sistema.

**Elementos de la Pantalla:**

1. **Encabezado**:
   - Título: "Mantenimiento de Usuarios" con icono de usuarios
   - Descripción: "Gestiona los usuarios del sistema, sus roles y permisos"
   - Botón **"CREAR USUARIO"** (azul, esquina superior derecha)

2. **Sección de Filtros** (Card colapsable):
   - **Filtrar por nombre**: Campo de búsqueda para buscar usuarios por nombre
   - **Filtrar por email**: Campo de búsqueda para buscar por correo electrónico
   - **Filtrar por estado**: Dropdown para filtrar usuarios activos o inactivos

3. **Tabla de Usuarios**:
   Columnas de la tabla:
   - **Usuario**: Nombre completo y correo electrónico
   - **Roles**: Botón "Ver (X)" mostrando la cantidad de roles asignados
   - **Estado**: Badge que indica si el usuario está Activo (verde) o Inactivo (rojo)
   - **Fecha Registro**: Fecha en que se creó el usuario
   - **Último Acceso**: Fecha y hora del último inicio de sesión
   - **Acciones**: Botones para gestionar el usuario

4. **Botones de Acciones** (por cada usuario):
   - 🟡 **Editar** (Amarillo) - Modificar información del usuario
   - 🔵 **Gestionar Roles** (Azul) - Asignar/quitar roles
   - ⚪ **Resetear Contraseña** (Gris) - Cambiar contraseña
   - 🔴 **Eliminar** (Rojo) - Eliminar el usuario

---

**Cómo Usar los Filtros:**

1. **Filtro por Nombre**:
   - Escriba parte del nombre del usuario
   - La tabla se filtra automáticamente mientras escribe
   - No es necesario presionar Enter

2. **Filtro por Email**:
   - Escriba parte del correo electrónico
   - Funciona de forma similar al filtro por nombre

3. **Filtro por Estado**:
   - Seleccione "Activos" para ver solo usuarios activos
   - Seleccione "Inactivos" para ver solo usuarios deshabilitados
   - Seleccione "Todos los estados" para ver todos los usuarios

💡 **Sugerencia**: Puede combinar múltiples filtros para búsquedas más específicas.

---

##### Crear Usuario

Para crear un nuevo usuario en el sistema:

**Paso 1: Acceder al Formulario de Creación**

1. En la pantalla de lista de usuarios, haga clic en el botón **"CREAR USUARIO"** (esquina superior derecha)
2. Se abrirá el formulario de creación de usuario

---

**Paso 2: Completar la Información del Usuario**

Complete los siguientes campos:

1. **Nombre Completo** (Obligatorio):
   - Ingrese el nombre completo del usuario
   - Ejemplo: `Juan Pérez García`

2. **Correo Electrónico** (Obligatorio):
   - Ingrese el correo electrónico del usuario
   - Debe ser único (no puede estar registrado previamente)
   - Ejemplo: `juan.perez@empresa.com`

3. **Contraseña** (Obligatorio):
   - Ingrese una contraseña segura
   - Mínimo 6 caracteres
   - Recomendado: Combine letras mayúsculas, minúsculas, números y símbolos

4. **Confirmar Contraseña** (Obligatorio):
   - Vuelva a escribir la contraseña exactamente igual
   - Debe coincidir con el campo "Contraseña"

---

**Paso 3: Asignar Roles**

En la sección **"Roles"**:

1. Se muestra una lista de todos los roles disponibles
2. Marque las casillas de los roles que desea asignar al usuario
3. Puede seleccionar múltiples roles
4. Cada rol muestra:
   - Nombre del rol
   - Descripción (si está disponible)

📝 **Ejemplo**: 
- Para un vendedor, podría asignar el rol "Vendedor"
- Para un gerente, podría asignar los roles "Vendedor" y "Supervisor"

---

**Paso 4: Establecer Estado del Usuario**

1. Use el **interruptor "Usuario Activo"**:
   - **Activado** (azul): El usuario podrá iniciar sesión inmediatamente
   - **Desactivado** (gris): El usuario estará creado pero no podrá acceder al sistema

💡 **Sugerencia**: Deje el usuario activo si desea que pueda acceder de inmediato.

---

**Paso 5: Guardar el Usuario**

1. Revise que toda la información sea correcta
2. Haga clic en el botón **"Guardar Usuario"** (azul)
3. Si hay errores, aparecerán mensajes en rojo indicando qué campos corregir
4. Si todo es correcto:
   - Verá una notificación de éxito
   - Será redirigido a la lista de usuarios
   - El nuevo usuario aparecerá en la tabla

**Botón Cancelar:**
- Haga clic en **"Cancelar"** si desea volver sin guardar cambios
- Perderá toda la información ingresada

---

**Errores Comunes al Crear Usuario:**

❌ **"El correo electrónico ya está registrado"**
- **Causa**: Ya existe un usuario con ese correo
- **Solución**: Use un correo electrónico diferente

❌ **"Las contraseñas no coinciden"**
- **Causa**: Los campos "Contraseña" y "Confirmar Contraseña" son diferentes
- **Solución**: Asegúrese de escribir la misma contraseña en ambos campos

❌ **"Debe seleccionar al menos un rol"**
- **Causa**: No seleccionó ningún rol
- **Solución**: Marque al menos una casilla de rol

---

##### Modificar Usuario

Para editar la información de un usuario existente:

**Paso 1: Acceder al Formulario de Edición**

1. En la lista de usuarios, localice el usuario que desea modificar
2. Haga clic en el botón **amarillo con icono de lápiz** (Editar)
3. Se abrirá el formulario de edición

---

**Paso 2: Modificar la Información**

Puede modificar los siguientes campos:

1. **Nombre Completo**: Cambie el nombre si es necesario
2. **Correo Electrónico**: Modifique el correo (debe ser único)
3. **Roles**: Agregue o quite roles marcando/desmarcando las casillas
4. **Estado Activo**: Active o desactive el usuario con el interruptor

⚠️ **Advertencia**: NO puede cambiar la contraseña desde aquí. Use la función "Resetear Contraseña" para eso.

---

**Paso 3: Guardar Cambios**

1. Revise las modificaciones realizadas
2. Haga clic en **"Guardar Cambios"**
3. El sistema validará y guardará la información actualizada
4. Verá una notificación de éxito y será redirigido a la lista

**Cancelar Cambios:**
- Haga clic en **"Cancelar"** para volver sin guardar
- Los cambios no se aplicarán

💡 **Sugerencia**: Si solo quiere cambiar los roles, use el botón "Gestionar Roles" en lugar de editar el usuario completo.

---

##### Asignar Roles a Usuario

Existen **dos formas** de asignar roles a un usuario:

**Opción 1: Desde el Formulario de Edición**
1. Edite el usuario (botón amarillo de lápiz)
2. En la sección "Roles", marque o desmarque las casillas
3. Guarde los cambios

**Opción 2: Gestión Rápida de Roles (Recomendado)**

Esta opción es más rápida para cambiar solo los roles sin editar otros datos.

**Pasos:**

1. En la lista de usuarios, haga clic en el **botón azul con icono de etiqueta** (Gestionar Roles)
2. Se abrirá una ventana modal con el título "Gestionar Roles de [Nombre Usuario]"
3. Verá una lista de todos los roles disponibles:
   - **Casillas marcadas**: Roles actualmente asignados al usuario
   - **Casillas sin marcar**: Roles disponibles para asignar

4. **Agregar un rol**: Marque la casilla correspondiente
5. **Quitar un rol**: Desmarque la casilla correspondiente
6. Haga clic en **"Guardar Cambios"**
7. Los roles se actualizarán inmediatamente

---

**Ver Roles Asignados:**

1. En la columna "Roles" de la tabla, haga clic en el botón **"Ver (X)"**
2. Se abrirá un modal mostrando:
   - Lista de todos los roles asignados al usuario
   - Descripción de cada rol
3. Este modal es solo de consulta, no permite editar

📝 **Ejemplo**:
```
Usuario: Juan Pérez
Roles Asignados:
✓ Vendedor - Puede crear y gestionar cotizaciones
✓ Supervisor - Puede aprobar cotizaciones de otros usuarios
```

---

##### Resetear Contraseña

Para cambiar la contraseña de un usuario (función administrativa):

**Paso 1: Acceder al Reseteo de Contraseña**

1. En la lista de usuarios, localice el usuario
2. Haga clic en el **botón gris con icono de llave** (Resetear Contraseña)
3. Se abrirá un modal de confirmación

---

**Paso 2: Ingresar Nueva Contraseña**

1. **Confirme** que desea resetear la contraseña del usuario
2. Complete los campos:
   - **Nueva Contraseña**: Ingrese la nueva contraseña (mínimo 6 caracteres)
   - **Confirmar Contraseña**: Repita la contraseña exactamente igual

3. Haga clic en **"Resetear Contraseña"**

---

**Paso 3: Comunicar al Usuario**

⚠️ **Importante**: 
1. El sistema cambiará la contraseña inmediatamente
2. El usuario no recibirá ninguna notificación automática
3. **Usted debe comunicarle al usuario la nueva contraseña** mediante un canal seguro:
   - Correo electrónico
   - Llamada telefónica
   - Mensaje directo

💡 **Sugerencia de Seguridad**:
- Use contraseñas temporales que el usuario deba cambiar
- No envíe contraseñas por correo electrónico sin cifrar
- Considere crear contraseñas aleatorias seguras

📝 **Ejemplo de comunicación**:
```
Asunto: Reseteo de contraseña - CotizacionesWeb

Estimado Juan:

Su contraseña ha sido reseteada exitosamente.
Nueva contraseña temporal: Temp2026@Pass

Por favor, solicite al administrador que la cambie por 
una contraseña personal una vez que inicie sesión.

Saludos,
Administrador del Sistema
```

---

##### Eliminar Usuario

Para eliminar un usuario del sistema:

⚠️ **Advertencia Importante**: La eliminación de usuarios es una acción que **elimina permanentemente** el registro. Use esta función con precaución.

**Paso 1: Iniciar Eliminación**

1. En la lista de usuarios, localice el usuario a eliminar
2. Haga clic en el **botón rojo con icono de papelera** (Eliminar)
3. Se abrirá un modal de confirmación

---

**Paso 2: Confirmar Eliminación**

1. Lea cuidadosamente el mensaje de confirmación:
   ```
   ¿Está seguro que desea eliminar al usuario [Nombre]?
   Esta acción no se puede deshacer.
   ```

2. **Opciones**:
   - **Confirmar**: Haga clic en **"Eliminar"** (botón rojo)
   - **Cancelar**: Haga clic en **"Cancelar"** para abortar la operación

---

**Paso 3: Verificar Eliminación**

1. Si confirma, el sistema eliminará el usuario
2. El usuario desaparecerá de la lista
3. Verá una notificación de éxito

---

**Consideraciones Importantes:**

✅ **Buenas Prácticas**:
- **No elimine usuarios activos**: Primero desactívelos con el interruptor "Activo"
- **Mantenga historial**: En lugar de eliminar, considere desactivar el usuario
- **Verifique dependencias**: Asegúrese de que el usuario no tenga cotizaciones activas importantes

⚠️ **Limitaciones**:
- No puede eliminar su propio usuario (el que está usando actualmente)
- No puede eliminar el usuario administrador principal del sistema

💡 **Alternativa Recomendada**: 
En lugar de eliminar, considere **desactivar** el usuario:
1. Edite el usuario
2. Desactive el interruptor "Usuario Activo"
3. Guarde los cambios
4. El usuario no podrá iniciar sesión pero mantendrá el historial

---

#### 5.2.2 Gestión de Roles

La **Gestión de Roles** permite crear, modificar y configurar los roles del sistema junto con sus permisos asociados.

##### **¿Qué es un Rol?**

Un **rol** es un conjunto de permisos que define qué acciones puede realizar un usuario en el sistema. Al asignar un rol a un usuario, este obtiene todos los permisos asociados a ese rol.

📝 **Ejemplos de Roles**:
- **Vendedor**: Puede crear, modificar y ver cotizaciones
- **Supervisor**: Puede aprobar cotizaciones y gestionar las de su equipo
- **Administrador**: Tiene acceso completo al sistema

---

##### **Acceso al Módulo**

Para acceder a la gestión de roles:

1. En el **menú lateral izquierdo**, haga clic en **"Administración"**
2. Seleccione **"Roles"**
3. Se mostrará la pantalla de lista de roles

---

##### Pantalla de Lista de Roles

La pantalla principal de roles muestra todos los roles configurados en el sistema.

**Elementos de la Pantalla:**

1. **Encabezado**:
   - Título: "Mantenimiento de Roles" con icono de escudo
   - Descripción: "Gestiona los roles del sistema y sus permisos"
   - Botón **"CREAR ROL"** (azul, esquina superior derecha)

2. **Sección de Filtros** (Card colapsable):
   - **Filtrar por nombre**: Buscar roles por nombre
   - **Filtrar por estado**: Ver roles activos, inactivos o todos

3. **Tabla de Roles**:
   Columnas de la tabla:
   - **Rol**: Nombre y descripción del rol
   - **Usuarios Asignados**: Cantidad de usuarios que tienen este rol
   - **Permisos**: Cantidad de permisos asociados al rol
   - **Estado**: Activo o Inactivo
   - **Fecha Creación**: Cuándo se creó el rol
   - **Acciones**: Botones para gestionar el rol

4. **Botones de Acciones** (por cada rol):
   - 🟡 **Modificar** (Amarillo) - Editar nombre y descripción
   - 🔵 **Gestionar Permisos** (Azul) - Asignar/quitar permisos
   - 🔴 **Eliminar** (Rojo) - Eliminar el rol

---

##### Crear Rol

Para crear un nuevo rol:

**Paso 1: Acceder al Formulario**

1. Haga clic en el botón **"CREAR ROL"**
2. Se abrirá el formulario de creación

---

**Paso 2: Ingresar Información del Rol**

Complete los siguientes campos:

1. **Nombre del Rol** (Obligatorio):
   - Ingrese un nombre descriptivo
   - Debe ser único
   - Ejemplo: `Gerente de Ventas`, `Contador`, `Asistente Administrativo`

2. **Descripción** (Opcional pero recomendado):
   - Describa brevemente qué hace este rol
   - Ayuda a otros administradores a entender el propósito del rol
   - Ejemplo: `Puede aprobar cotizaciones y gestionar el equipo de ventas`

3. **Estado Activo**:
   - Use el interruptor para activar o desactivar el rol
   - **Activado**: El rol puede ser asignado a usuarios
   - **Desactivado**: El rol existe pero no puede asignarse

---

**Paso 3: Guardar el Rol**

1. Haga clic en **"Guardar Rol"**
2. Si es exitoso:
   - Verá una notificación de éxito
   - Será redirigido a la lista de roles
   - El nuevo rol aparecerá en la tabla

⚠️ **Advertencia**: Un rol recién creado **no tiene permisos asignados**. Debe configurar los permisos después de crearlo usando la opción "Gestionar Permisos".

---

##### Modificar Rol

Para editar un rol existente:

**Paso 1: Acceder al Formulario de Edición**

1. En la lista de roles, localice el rol
2. Haga clic en el **botón amarillo** (Modificar)
3. Se abrirá el formulario de edición

---

**Paso 2: Modificar Información**

Puede cambiar:
1. **Nombre del Rol**: Cambie el nombre si es necesario
2. **Descripción**: Actualice o agregue una descripción
3. **Estado**: Active o desactive el rol

⚠️ **Advertencia**: 
- Cambiar el nombre de un rol no afecta a los usuarios que ya lo tienen asignado
- Desactivar un rol no elimina los permisos de usuarios que lo tienen, pero no podrá asignarse a nuevos usuarios

---

**Paso 3: Guardar Cambios**

1. Revise las modificaciones
2. Haga clic en **"Guardar Cambios"**
3. El sistema actualizará el rol

💡 **Sugerencia**: Para cambiar permisos, use "Gestionar Permisos" en lugar de editar el rol.

---

##### Asociar Permisos a Rol

Esta es la función más importante de la gestión de roles, donde define qué puede hacer un usuario con este rol.

**Paso 1: Acceder a Gestión de Permisos**

1. En la lista de roles, localice el rol
2. Haga clic en el **botón azul con icono de candado** (Gestionar Permisos)
3. Se abrirá un modal con todos los permisos disponibles

---

**Paso 2: Comprender los Permisos**

Los permisos están organizados por **categorías** del sistema. El sistema cuenta con **44 permisos** distribuidos en **6 categorías**:

#### **1. Permisos de Clientes (CLI_) - 5 permisos**
Gestionan el acceso a la información de clientes e interesados:
- `CLI_VIEW`: Ver clientes/interesados
- `CLI_CREATE`: Crear nuevos clientes/interesados
- `CLI_EDIT`: Editar clientes/interesados
- `CLI_DELETE`: Eliminar clientes/interesados
- `CLI_SYNC`: Sincronizar con HubSpot

---

#### **2. Permisos de Configuración (CFG_) - 5 permisos**
Controlan el acceso a la configuración del sistema:
- `CFG_PARAMS_VIEW`: Ver parámetros del sistema
- `CFG_PARAMS_EDIT`: Editar parámetros del sistema
- `CFG_PARAMS_RESET`: Resetear parámetros a valores por defecto
- `CFG_PARAMS_VIEW_SECRET`: Ver valores reales de parámetros sensitivos (contraseñas, tokens, API keys)
- `CFG_LOGS`: Ver logs del sistema

---

#### **3. Permisos de Cotizaciones (COT_) - 19 permisos**
La categoría más extensa, cubre todas las operaciones relacionadas con cotizaciones:

**Visualización:**
- `COT_VIEW`: Ver cotizaciones (solo lectura)
- `COT_VIEW_DETAIL`: Ver detalle de cotización (solo lectura)
- `COT_VIEW_HISTORY`: Ver historial de cotización
- `COT_VIEW_VERSIONS`: Ver versiones de cotización

**Gestión:**
- `COT_CREATE`: Crear nuevas cotizaciones
- `COT_EDIT`: Editar cotizaciones
- `COT_DELETE`: Eliminar cotizaciones
- `COT_COPY`: Copiar versión de cotización
- `COT_DUPLICATE`: Duplicar cotizaciones

**Flujo de Aprobación:**
- `COT_APPROVE`: Aprobar cotizaciones
- `COT_REJECT`: Rechazar cotizaciones
- `COT_ACCEPT`: Aceptar cotizaciones por parte del cliente
- `COT_SEND_CLIENT`: Enviar cotización al cliente luego de aprobada

**Archivo:**
- `COT_ARCHIVE`: Archivar cotizaciones
- `COT_ARCHIVE_VIEW`: Ver cotizaciones archivadas
- `COT_ARCHIVE_VIEW_DETAIL`: Ver detalle de cotizaciones archivadas
- `COT_ARCHIVE_REACTIVATE`: Reactivar cotizaciones archivadas

**Exportación e Integración:**
- `COT_EXPORT`: Exportar cotizaciones
- `COT_SEND_ERP`: Enviar cotizaciones al ERP

---

#### **4. Permisos de Reportes (RPT_) - 4 permisos**
Controlan el acceso a reportes y análisis:
- `RPT_VIEW`: Ver reportes
- `RPT_EXPORT`: Exportar reportes
- `RPT_DASHBOARD`: Acceso al dashboard ejecutivo
- `RPT_HISTORIAL`: Ver historial de cotizaciones

---

#### **5. Permisos de Roles (ROL_) - 5 permisos**
Gestionan la administración de roles:
- `ROL_VIEW`: Ver roles del sistema
- `ROL_CREATE`: Crear nuevos roles
- `ROL_EDIT`: Editar roles existentes
- `ROL_DELETE`: Eliminar roles
- `ROL_PERMISOS`: Gestionar permisos de roles

---

#### **6. Permisos de Usuarios (USR_) - 6 permisos**
Controlan la administración de usuarios:
- `USR_VIEW`: Ver usuarios del sistema
- `USR_CREATE`: Crear nuevos usuarios
- `USR_EDIT`: Editar usuarios existentes
- `USR_DELETE`: Eliminar usuarios
- `USR_ROLES`: Gestionar roles de usuarios
- `USR_RESET_PWD`: Resetear contraseñas

---

✅ **Total: 44 permisos** distribuidos en 6 categorías funcionales

---

**Paso 3: Asignar Permisos**

1. **Revisar permisos actuales**: Las casillas marcadas indican permisos ya asignados
2. **Agregar permiso**: Marque la casilla del permiso que desea agregar
3. **Quitar permiso**: Desmarque la casilla del permiso que desea remover
4. Repita para todos los permisos necesarios

---

**Paso 4: Guardar Configuración**

1. Haga clic en **"Guardar Permisos"**
2. Los cambios se aplicarán inmediatamente
3. Todos los usuarios con este rol obtendrán/perderán los permisos modificados

⚠️ **Advertencia Importante**: Los cambios de permisos afectan a **todos los usuarios** que tienen este rol asignado, incluso si ya están usando el sistema.

---

**Ejemplos de Configuración de Roles:**

📝 **Rol: Vendedor**
```
Permisos recomendados:
✓ CLI_VIEW - Ver clientes/interesados
✓ CLI_CREATE - Crear clientes
✓ CLI_EDIT - Editar clientes
✓ COT_VIEW - Ver cotizaciones
✓ COT_VIEW_DETAIL - Ver detalle de cotizaciones
✓ COT_CREATE - Crear cotizaciones
✓ COT_EDIT - Modificar sus cotizaciones
✓ COT_COPY - Copiar versiones de cotizaciones
✓ COT_DUPLICATE - Duplicar cotizaciones
✓ COT_VIEW_HISTORY - Ver historial
✓ COT_EXPORT - Exportar cotizaciones
✓ RPT_VIEW - Ver reportes
✗ COT_APPROVE - NO puede aprobar
✗ COT_DELETE - NO puede eliminar
✗ COT_SEND_CLIENT - NO puede enviar al cliente
✗ COT_ARCHIVE - NO puede archivar
✗ USR_* - NO puede gestionar usuarios
✗ ROL_* - NO puede gestionar roles
✗ CFG_* - NO puede cambiar configuración
```

📝 **Rol: Supervisor**
```
Permisos recomendados:
✓ CLI_VIEW, CLI_CREATE, CLI_EDIT - Gestión de clientes
✓ COT_VIEW, COT_VIEW_DETAIL - Ver cotizaciones completas
✓ COT_CREATE, COT_EDIT - Crear y modificar cotizaciones
✓ COT_APPROVE - Aprobar cotizaciones
✓ COT_REJECT - Rechazar cotizaciones
✓ COT_SEND_CLIENT - Enviar cotizaciones al cliente
✓ COT_COPY, COT_DUPLICATE - Copiar y duplicar
✓ COT_ARCHIVE - Archivar cotizaciones
✓ COT_ARCHIVE_VIEW - Ver cotizaciones archivadas
✓ COT_VIEW_HISTORY, COT_VIEW_VERSIONS - Ver historial y versiones
✓ COT_EXPORT - Exportar cotizaciones
✓ RPT_VIEW, RPT_EXPORT - Ver y exportar reportes
✓ RPT_DASHBOARD - Acceso al dashboard ejecutivo
✗ COT_DELETE - NO puede eliminar cotizaciones
✗ COT_SEND_ERP - NO puede enviar al ERP
✗ USR_DELETE, ROL_DELETE - NO puede eliminar usuarios/roles
✗ CFG_PARAMS_EDIT - NO puede cambiar configuración
✗ CFG_PARAMS_VIEW_SECRET - NO puede ver datos sensitivos
```

📝 **Rol: Administrador**
```
Permisos: TODOS (44 permisos)
✓ Todos los permisos CLI_* (5 permisos de Clientes)
✓ Todos los permisos CFG_* (5 permisos de Configuración)
✓ Todos los permisos COT_* (19 permisos de Cotizaciones)
✓ Todos los permisos RPT_* (4 permisos de Reportes)
✓ Todos los permisos ROL_* (5 permisos de Roles)
✓ Todos los permisos USR_* (6 permisos de Usuarios)
```

📝 **Rol: Contador/Auditor**
```
Permisos recomendados (solo lectura y reportes):
✓ CLI_VIEW - Ver clientes (solo lectura)
✓ COT_VIEW - Ver cotizaciones
✓ COT_VIEW_DETAIL - Ver detalle de cotizaciones
✓ COT_VIEW_HISTORY - Ver historial de cambios
✓ COT_VIEW_VERSIONS - Ver versiones
✓ COT_ARCHIVE_VIEW - Ver archivadas
✓ COT_ARCHIVE_VIEW_DETAIL - Ver detalle de archivadas
✓ RPT_VIEW - Ver reportes
✓ RPT_EXPORT - Exportar reportes
✓ RPT_DASHBOARD - Acceso al dashboard
✓ RPT_HISTORIAL - Ver historial completo
✓ CFG_LOGS - Ver logs del sistema
✗ COT_CREATE, COT_EDIT, COT_DELETE - NO puede modificar
✗ COT_APPROVE, COT_REJECT - NO puede aprobar/rechazar
✗ USR_*, ROL_* - NO puede gestionar usuarios/roles
✗ CFG_PARAMS_EDIT - NO puede cambiar configuración
```

💡 **Sugerencia**: Siga el principio de **mínimo privilegio**: Otorgue solo los permisos necesarios para que cada rol cumpla su función.

---

##### Eliminar Rol

Para eliminar un rol del sistema:

⚠️ **Advertencia Crítica**: No puede eliminar un rol que tiene usuarios asignados. Primero debe reasignar o eliminar esos usuarios.

**Paso 1: Verificar Usuarios Asignados**

1. En la columna "Usuarios Asignados", verifique el número
2. Si muestra "0 usuarios", puede proceder
3. Si muestra "1" o más, **NO podrá eliminar** el rol hasta que:
   - Quite el rol de todos los usuarios
   - O elimine los usuarios

---

**Paso 2: Eliminar el Rol**

1. Haga clic en el **botón rojo** (Eliminar)
2. Se mostrará una confirmación:
   ```
   ¿Está seguro que desea eliminar el rol [Nombre]?
   Esta acción no se puede deshacer.
   ```

3. **Si hay usuarios asignados**, verá:
   ```
   ❌ No se puede eliminar este rol porque tiene X usuarios asignados.
   Primero debe reasignar estos usuarios a otro rol.
   ```

4. Si **no hay usuarios**, puede confirmar la eliminación

---

**Paso 3: Confirmar**

1. Haga clic en **"Eliminar"** para confirmar
2. El rol será eliminado permanentemente
3. El rol desaparecerá de la lista

---

**Consideraciones:**

✅ **Buenas Prácticas**:
- No elimine roles del sistema que vienen predefinidos (Admin, Usuario básico)
- Considere desactivar el rol en lugar de eliminarlo
- Documente los roles personalizados que cree

💡 **Alternativa Recomendada**: 
Desactive el rol en lugar de eliminarlo:
1. Edite el rol
2. Desactive el interruptor "Activo"
3. El rol no estará disponible para asignar, pero se mantiene el historial

---

### 5.3 Cotizaciones

El módulo de **Cotizaciones** es el núcleo del sistema CotizacionesWeb. Aquí podrá crear, gestionar, aprobar y dar seguimiento a todas las cotizaciones de su organización.

⚠️ **Advertencia**: Las opciones y funcionalidades que vea en este módulo dependerán de los permisos de cotización (COT_*) asignados a su usuario.

---

#### 5.3.1 Gestión de Cotizaciones

La **Gestión de Cotizaciones** es la pantalla principal donde puede ver, buscar, filtrar y realizar acciones sobre todas las cotizaciones activas del sistema.

##### **Acceso al Módulo**

Para acceder a la gestión de cotizaciones:

1. En el **menú lateral izquierdo**, haga clic en **"Cotizaciones"**
2. Seleccione **"Gestión de Cotizaciones"** (primera opción)
3. Se mostrará la pantalla de lista de cotizaciones

---

##### Pantalla de Lista de Cotizaciones

La pantalla principal muestra todas las cotizaciones activas (no archivadas) del sistema con múltiples opciones de filtrado.

**Elementos de la Pantalla:**

1. **Encabezado**:
   - Título: "Gestión de Cotizaciones" con icono de factura
   - Descripción: "Administra las cotizaciones del sistema, sus versiones y estados"
   - Botón **"CREAR COTIZACIÓN"** (azul, esquina superior derecha) - Requiere permiso `COT_CREATE`

2. **Sección de Filtros** (Card colapsable con icono de filtro):
   El sistema ofrece filtros avanzados para localizar cotizaciones específicas:

   **Filtros Disponibles:**

   - **Búsqueda General**: 
     - Busca por ID de cotización o nombre de cliente
     - Búsqueda en tiempo real mientras escribe

   - **Versión**: 
     - Filtre por número de versión específico (ej: 1.0, 2.0, 3.5)
     - Formato decimal con un decimal

   - **Rango de Fechas**:
     - **Fecha Desde**: Fecha inicial del rango
     - **Fecha Hasta**: Fecha final del rango
     - ✅ **Por defecto**: Últimos 30 días si no especifica fechas

   - **Rango de Montos**:
     - **Monto Desde**: Monto mínimo de cotización
     - **Monto Hasta**: Monto máximo de cotización
     - Formato decimal con dos decimales

   - **Filtrar por Estado** (Dropdown con checkboxes):
     - 🟦 **Borrador**: Cotizaciones en creación
     - 🟨 **Pendiente Aprobación**: Esperando aprobación
     - 🟩 **Aprobada**: Cotizaciones aprobadas
     - 🟦 **Enviada**: Enviadas al cliente
     - 🟪 **Aceptada**: Aceptadas por el cliente
     - 🟥 **Rechazada**: Rechazadas por el cliente
     - Puede seleccionar múltiples estados simultáneamente

   - **Filtrar por Moneda**:
     - Dropdown con monedas disponibles
     - Opciones: CRC (₡ Colón), USD ($ Dólar), EUR (€ Euro)
     - "Todas las monedas" para no filtrar

   - **Botones de Acción**:
     - 🔵 **Buscar**: Aplica los filtros seleccionados
     - ⚪ **Limpiar**: Reinicia todos los filtros a valores por defecto

💡 **Sugerencia**: Combine múltiples filtros para búsquedas muy específicas. Por ejemplo: "Cotizaciones en estado Aprobada, en dólares, del último mes".

---

3. **Tabla de Cotizaciones**:

   La tabla muestra todas las cotizaciones que coinciden con los filtros aplicados.

   **Columnas de la Tabla:**

   | Columna | Descripción |
   |---------|-------------|
   | **ID COTIZACIÓN** | Identificador único de la cotización (ej: COT-2025-001) |
   | **CLIENTE** | Nombre del cliente y empresa (si aplica) |
   | **EMPRESA** | Empresa del interesado |
   | **MONEDA** | Badge con símbolo y código de moneda (₡ CRC, $ USD) |
   | **ESTADO** | Badge de color según estado actual |
   | **VERSIÓN** | Número de versión actual (ej: 1.0, 2.0) |
   | **MONTO** | Monto total formateado con símbolo de moneda |
   | **FECHA CREACIÓN** | Fecha en que se creó la cotización |
   | **FECHA ENVÍO** | Fecha de envío al cliente (si aplica) |
   | **ESTADOS** | Botones para cambiar estado de la cotización |
   | **ACCIONES** | Botones para gestionar la cotización |

---

4. **Estados de Cotización** (Badges de color):

   Cada cotización tiene un estado actual identificado por color:

   - 🟦 **Borrador** (Gris): En proceso de creación, puede editarse libremente
   - 🟨 **Pendiente Aprobación** (Amarillo): Enviada para aprobación, esperando revisión
   - 🟩 **Aprobada** (Verde): Aprobada por supervisor, lista para enviar al cliente
   - 🟦 **Enviada** (Azul claro): Enviada al cliente, esperando respuesta
   - 🟪 **Aceptada** (Azul oscuro): Cliente aceptó la cotización
   - 🟥 **Rechazada** (Rojo): Cliente rechazó la cotización

📝 **Ejemplo**: Una cotización pasa por este flujo típico:
```
Borrador → Pendiente Aprobación → Aprobada → Enviada → Aceptada
```

---

5. **Botones de Transición de Estados** (Columna ESTADOS):

   Estos botones permiten cambiar el estado de la cotización según el flujo de trabajo:

   **Desde Borrador:**
   - 📤 **Enviar a Aprobación** (Azul) - Cambia a "Pendiente Aprobación" - Requiere `COT_EDIT`

   **Desde Pendiente Aprobación:**
   - ✅ **Aprobar** (Verde) - Cambia a "Aprobada" - Requiere `COT_APPROVE`
   - ❌ **Rechazar** (Rojo) - Cambia a "Rechazada" - Requiere `COT_REJECT`

   **Desde Aprobada:**
   - 📧 **Enviar a Cliente** (Azul) - Cambia a "Enviada" - Requiere `COT_SEND_CLIENT`

   **Desde Enviada:**
   - 👍 **Marcar como Aceptada** (Verde) - Cambia a "Aceptada" - Requiere `COT_ACCEPT`
   - 👎 **Marcar como Rechazada** (Rojo) - Cambia a "Rechazada" - Requiere `COT_REJECT`

⚠️ **Advertencia**: Los cambios de estado son irreversibles en algunos casos. Asegúrese de seleccionar el estado correcto.

---

6. **Botones de Acciones** (Columna ACCIONES):

   Cada cotización tiene botones de acción para gestionarla:

   - 👁️ **Ver Detalle** (Azul) - Ver información completa de la cotización - Requiere `COT_VIEW_DETAIL`
   - 📝 **Editar** (Amarillo) - Modificar la cotización - Requiere `COT_EDIT`
   - 📋 **Copiar Versión** (Gris) - Crear nueva versión de la cotización - Requiere `COT_COPY`
   - 📑 **Duplicar** (Verde) - Crear cotización nueva con datos copiados - Requiere `COT_DUPLICATE`
   - 📚 **Versiones** (Azul oscuro) - Ver todas las versiones de esta cotización - Requiere `COT_VIEW_VERSIONS`
   - 📜 **Historial** (Púrpura) - Ver historial de cambios - Requiere `COT_VIEW_HISTORY`
   - 📥 **Archivar** (Naranja) - Mover a archivo - Requiere `COT_ARCHIVE`
   - 🗑️ **Eliminar** (Rojo) - Eliminar cotización (solo Borrador) - Requiere `COT_DELETE`

💡 **Sugerencia**: Pase el cursor sobre los botones para ver información adicional (tooltips).

---

##### **Cómo Usar los Filtros - Ejemplos Prácticos**

📝 **Ejemplo 1: Buscar cotizaciones aprobadas del último mes**
1. En "Filtrar por Estado", marque solo ✅ **Aprobada**
2. En "Fecha Desde", seleccione la fecha de hace 30 días
3. En "Fecha Hasta", seleccione la fecha de hoy
4. Haga clic en **Buscar**
5. La tabla mostrará solo cotizaciones aprobadas de los últimos 30 días

📝 **Ejemplo 2: Buscar cotizaciones en dólares mayores a $5,000**
1. En "Filtrar por Moneda", seleccione **$ USD - Dólar**
2. En "Monto Desde", escriba `5000`
3. Deje "Monto Hasta" vacío (sin límite superior)
4. Haga clic en **Buscar**
5. Verá solo cotizaciones en dólares con monto mayor o igual a $5,000

📝 **Ejemplo 3: Buscar cotización específica de un cliente**
1. En "Búsqueda General", escriba el nombre del cliente
2. Mientras escribe, la tabla se filtra automáticamente
3. Localice la cotización en la lista resultante

---

##### Ver Detalle de Cotización

Para ver la información completa de una cotización:

**Paso 1: Acceder al Detalle**

1. En la lista de cotizaciones, localice la cotización que desea ver
2. Haga clic en el botón **azul con icono de ojo** (Ver Detalle)
3. Se abrirá la pantalla de detalle de la cotización

⚠️ **Nota**: Requiere permiso `COT_VIEW_DETAIL`

---

**Paso 2: Información Mostrada en el Detalle**

La pantalla de detalle muestra toda la información de la cotización organizada en secciones:

**Sección 1: Información General**
- **ID Cotización**: Identificador único (ej: COT-2025-001)
- **Estado Actual**: Badge de color mostrando el estado
- **Versión**: Número de versión que está viendo (ej: 2.0)
- **Fecha de Creación**: Cuándo se creó la cotización
- **Última Actualización**: Última modificación realizada
- **Moneda**: Moneda utilizada en la cotización

**Sección 2: Información del Cliente**
- **Nombre del Cliente**: Nombre completo del interesado
- **Correo Electrónico**: Email de contacto
- **Empresa**: Empresa del cliente (si aplica)
- **Tipo**: Persona física o jurídica

**Sección 3: Detalles de Productos/Servicios**

Tabla con todas las líneas de la cotización:

| Columna | Descripción |
|---------|-------------|
| **#** | Número de línea |
| **Producto/Servicio** | Código o nombre del producto |
| **Descripción** | Descripción detallada del ítem |
| **Cantidad** | Cantidad solicitada |
| **Precio Unitario** | Precio por unidad |
| **Descuento** | Descuento aplicado (%) |
| **Impuesto** | Porcentaje de impuesto aplicado |
| **Subtotal** | Total de la línea sin impuestos |
| **Total** | Total de la línea con impuestos |

**Sección 4: Resumen Financiero**

Cuadro con totales de la cotización:
- **Subtotal**: Suma de todos los productos antes de descuentos e impuestos
- **Descuento**: Total de descuentos aplicados
- **Impuesto**: Total de impuestos calculados
- **TOTAL**: Monto final de la cotización

📝 **Ejemplo de Resumen**:
```
Subtotal:    $1,000.00
Descuento:   -$100.00
Impuesto:    +$117.00
-----------------------
TOTAL:       $1,017.00 USD
```

**Sección 5: Información Adicional**
- **Notas**: Observaciones o comentarios sobre la cotización
- **Tipo de Cambio**: Si aplica conversión de moneda
- **Fecha de Envío**: Cuándo se envió al cliente (si aplica)
- **Fecha de Aceptación/Rechazo**: Respuesta del cliente (si aplica)
- **Enviado a ERP**: Indica si se integró con el ERP (S/N)

---

**Paso 3: Acciones desde el Detalle**

Desde la pantalla de detalle puede realizar las siguientes acciones:

- **Volver a la Lista**: Botón "Regresar" para volver al listado
- **Editar**: Si tiene permisos y el estado lo permite
- **Copiar Versión**: Crear nueva versión basada en esta
- **Ver Historial**: Consultar cambios realizados
- **Exportar PDF**: Generar documento de la cotización (si está implementado)

💡 **Sugerencia**: Use el detalle para revisar toda la información antes de aprobar, enviar o modificar una cotización.

---

##### Crear Cotización

La creación de cotizaciones es el proceso de generar una nueva propuesta comercial para un cliente.

⚠️ **Requisito**: Necesita permiso `COT_CREATE` para crear cotizaciones.

📝 **Nota**: La documentación completa del proceso de creación se encuentra en la sección [5.3.2 Nueva Cotización](#532-nueva-cotización).

**Acceso Rápido:**
1. Haga clic en el botón **"CREAR COTIZACIÓN"** en la esquina superior derecha
2. O use el menú: **Cotizaciones** > **Nueva Cotización**

---

##### Modificar Cotización

Para editar una cotización existente:

**Paso 1: Verificar si Puede Editar**

✅ **Puede editar cotizaciones en estado:**
- **Borrador**: Edición completa sin restricciones
- **Pendiente Aprobación**: Solo si tiene permisos especiales
- **Aprobada**: Generalmente requiere crear nueva versión

⚠️ **NO puede editar cotizaciones en estado:**
- **Enviada**: Ya fue enviada al cliente
- **Aceptada**: Fue aceptada por el cliente
- **Rechazada**: Fue rechazada por el cliente
- **Archivada**: Está en el archivo

💡 **Sugerencia**: Si necesita modificar una cotización que no puede editar, use la función **"Copiar Versión"** para crear una nueva versión editable.

---

**Paso 2: Acceder a Edición**

1. En la lista de cotizaciones, localice la cotización a modificar
2. Verifique que esté en estado **Borrador** (recomendado)
3. Haga clic en el botón **amarillo con icono de lápiz** (Editar)
4. Se abrirá el formulario de edición

---

**Paso 3: Realizar Modificaciones**

Puede modificar los siguientes elementos:

**Información del Cliente:**
- Cambiar cliente seleccionado
- Actualizar datos de contacto
- Modificar empresa

**Detalles de Productos:**
- Agregar nuevas líneas de productos/servicios
- Eliminar líneas existentes
- Modificar cantidades, precios, descuentos
- Cambiar descripciones

**Configuración Financiera:**
- Cambiar moneda (⚠️ **solo si no hay líneas de detalle agregadas**)
- Actualizar tipo de cambio
- Modificar descuentos globales

**Notas y Observaciones:**
- Agregar o modificar notas internas
- Actualizar condiciones comerciales

---

**Paso 4: Guardar Cambios**

1. Revise todas las modificaciones realizadas
2. Verifique que los totales sean correctos
3. Haga clic en **"Guardar Cambios"** o **"Actualizar Cotización"**
4. El sistema validará la información
5. Si es exitoso:
   - Verá una notificación de éxito
   - Será redirigido al detalle o lista de cotizaciones
   - Los cambios se registrarán en el historial

**Errores Comunes al Editar:**

❌ **"No se puede cambiar la moneda con líneas de detalle"**
- **Causa**: Intentó cambiar moneda con productos ya agregados
- **Solución**: Elimine todas las líneas primero o cree nueva cotización

❌ **"La cotización no está en estado editable"**
- **Causa**: El estado actual no permite edición
- **Solución**: Use "Copiar Versión" para crear versión editable

❌ **"Debe agregar al menos una línea de detalle"**
- **Causa**: Intentó guardar sin productos/servicios
- **Solución**: Agregue al menos un producto antes de guardar

---

##### Copiar Versión de Cotización

La función **Copiar Versión** crea una nueva versión de la misma cotización, manteniendo el mismo ID pero incrementando el número de versión.

⚠️ **Requisito**: Permiso `COT_COPY`

**¿Cuándo usar Copiar Versión?**

✅ Use copiar versión cuando:
- El cliente solicita ajustes a una cotización enviada
- Necesita actualizar precios pero mantener el histórico
- Quiere modificar productos pero preservar versiones anteriores
- Necesita crear variantes de la misma propuesta

**Diferencia con Duplicar:**
- **Copiar Versión**: Mismo ID, nueva versión (COT-2025-001 v1.0 → v2.0)
- **Duplicar**: Nuevo ID, nueva cotización (COT-2025-001 → COT-2025-002)

---

**Paso 1: Copiar la Versión Actual**

1. En la lista de cotizaciones, localice la cotización
2. Haga clic en el botón **gris con icono de portapapeles** (Copiar Versión)
3. Se abrirá un modal de confirmación

---

**Paso 2: Agregar Comentario**

1. En el modal, ingrese un **comentario** (opcional pero recomendado)
2. El comentario explica por qué se creó la nueva versión
3. Ejemplo: `Actualización de precios según solicitud del cliente`

---

**Paso 3: Confirmar Creación**

1. Haga clic en **"Crear Nueva Versión"**
2. El sistema creará una copia exacta con nuevo número de versión
3. La nueva versión estará en estado **Borrador**
4. Verá una notificación: `"Nueva versión 2.0 creada exitosamente"`

---

**Paso 4: Editar la Nueva Versión**

1. La nueva versión aparecerá en la lista en estado Borrador
2. Localícela y haga clic en **Editar**
3. Realice las modificaciones necesarias
4. Guarde los cambios

📝 **Ejemplo de Versionado**:
```
Versión 1.0 (Enviada)    → Primera propuesta al cliente
Versión 2.0 (Borrador)   → Ajuste de precios solicitado
Versión 2.0 (Aprobada)   → Aprobada con nuevos precios
Versión 2.0 (Enviada)    → Enviada al cliente actualizada
```

---

**Copiar Versión Específica (Antigua)**

También puede copiar una versión anterior específica:

1. Haga clic en **"Versiones"** de la cotización
2. En el modal de versiones, localice la versión antigua deseada
3. Haga clic en **"Copiar esta Versión"**
4. Agregue comentario explicativo
5. La nueva versión se basará en esa versión antigua

💡 **Sugerencia**: Use esto para "recuperar" una versión anterior que el cliente prefiere sobre la actual.

---

##### Duplicar Cotización

La función **Duplicar** crea una cotización completamente nueva con un ID diferente, copiando los datos de una cotización existente.

⚠️ **Requisito**: Permiso `COT_DUPLICATE`

**¿Cuándo usar Duplicar?**

✅ Use duplicar cuando:
- Necesita crear cotización para cliente diferente con productos similares
- Quiere usar una cotización como plantilla
- Necesita propuesta similar pero totalmente independiente
- Desea reutilizar configuración de productos/servicios

---

**Paso 1: Iniciar Duplicación**

1. En la lista de cotizaciones, localice la cotización a duplicar
2. Haga clic en el botón **verde con icono de documentos** (Duplicar)
3. Se abrirá un modal de confirmación

---

**Paso 2: Confirmar Duplicación**

1. Lea el mensaje: `"¿Está seguro que desea duplicar la cotización [ID]?"`
2. Entienda que se creará una **nueva cotización** con **nuevo ID**
3. Haga clic en **"Duplicar Cotización"**

---

**Paso 3: Verificar Nueva Cotización**

1. El sistema creará la nueva cotización con:
   - **Nuevo ID** generado automáticamente (ej: COT-2025-015)
   - **Versión 1.0** (siempre inicia en 1.0)
   - **Estado Borrador**
   - **Mismos productos, precios y configuración** de la original
   - **Fecha de creación actual**

2. Verá notificación: `"Cotización COT-2025-015 creada exitosamente"`

---

**Paso 4: Editar Cotización Duplicada**

1. Localice la nueva cotización en la lista (use el ID mostrado)
2. Haga clic en **Editar**
3. **Modifique obligatoriamente**:
   - Cliente/Interesado (si es para otro cliente)
   - Ajuste precios si es necesario
   - Actualice fechas o condiciones

4. Guarde los cambios

⚠️ **Advertencia**: Asegúrese de cambiar el cliente si la cotización duplicada es para otra persona. La duplicación copia TODO, incluyendo el cliente original.

📝 **Ejemplo de Uso**:
```
Cotización Original:  COT-2025-010 - Cliente: Empresa ABC - Productos: 5 computadoras
Cotización Duplicada: COT-2025-015 - Cliente: Empresa XYZ - Productos: 5 computadoras (copiadas)
```

---

##### Ver Versiones de Cotización

El sistema mantiene un historial completo de todas las versiones de cada cotización.

⚠️ **Requisito**: Permiso `COT_VIEW_VERSIONS`

**Paso 1: Abrir Modal de Versiones**

1. En la lista de cotizaciones, localice la cotización
2. Haga clic en el botón **azul oscuro con icono de libro** (Versiones)
3. Se abrirá un modal mostrando todas las versiones

---

**Paso 2: Información de Versiones**

El modal muestra una tabla con todas las versiones:

| Columna | Descripción |
|---------|-------------|
| **Versión** | Número de versión (1.0, 2.0, 3.0) |
| **Fecha** | Cuándo se creó esta versión |
| **Cliente** | Datos del interesado en esa versión |
| **Total** | Monto de la cotización en esa versión |
| **Moneda** | Moneda utilizada |
| **Actual** | Indica si es la versión activa |

✅ **Versión Actual**: Se marca con badge verde **"ACTUAL"**

---

**Paso 3: Acciones sobre Versiones**

Para cada versión puede realizar:

- 👁️ **Ver Detalle**: Ver información completa de esa versión específica
- 📋 **Copiar**: Crear nueva versión basada en esta versión antigua
- 📜 **Historial**: Ver cambios de esta versión específica

📝 **Ejemplo de Historial de Versiones**:
```
Versión 1.0 - 15/01/2025 - $1,000.00 - (Original)
Versión 2.0 - 20/01/2025 - $950.00  - Descuento aplicado
Versión 3.0 - 25/01/2025 - $980.00  - ACTUAL - Ajuste final
```

💡 **Sugerencia**: Use el historial de versiones para rastrear cómo ha evolucionado la negociación con el cliente.

---

##### Ver Historial de Cotización

El historial registra TODOS los eventos y cambios realizados en una cotización.

⚠️ **Requisito**: Permiso `COT_VIEW_HISTORY`

**Paso 1: Abrir Historial**

1. En la lista de cotizaciones, localice la cotización
2. Haga clic en el botón **púrpura con icono de reloj** (Historial)
3. Se abrirá un modal con el historial completo

---

**Paso 2: Tipos de Eventos Registrados**

El historial muestra todos los eventos cronológicamente:

**Eventos de Creación:**
- ✅ Cotización creada
- ✅ Nueva versión creada
- ✅ Cotización duplicada

**Eventos de Modificación:**
- 📝 Datos modificados
- 📝 Productos agregados/eliminados
- 📝 Precios actualizados
- 📝 Cliente cambiado

**Eventos de Estados:**
- 📤 Enviada a aprobación
- ✅ Aprobada
- ❌ Rechazada
- 📧 Enviada al cliente
- 👍 Aceptada por cliente
- 👎 Rechazada por cliente

**Eventos de Archivo:**
- 📥 Archivada
- 📤 Reactivada desde archivo

---

**Paso 3: Información del Historial**

Cada entrada muestra:
- **Fecha y Hora**: Timestamp exacto del evento
- **Usuario**: Quién realizó la acción
- **Tipo de Evento**: Qué se hizo
- **Comentario**: Detalles adicionales (si los hay)
- **Versión**: En qué versión ocurrió (si aplica)

📝 **Ejemplo de Historial**:
```
25/01/2025 10:30 AM - Juan Pérez - Cotización creada (v1.0)
25/01/2025 11:45 AM - Juan Pérez - Productos agregados (v1.0)
25/01/2025 02:15 PM - Juan Pérez - Enviada a aprobación (v1.0)
26/01/2025 09:00 AM - María Gómez - Aprobada (v1.0) - "Precios correctos"
26/01/2025 09:30 AM - María Gómez - Enviada al cliente (v1.0)
28/01/2025 03:00 PM - Juan Pérez - Nueva versión creada (v2.0) - "Cliente solicitó descuento"
```

💡 **Sugerencia**: Use el historial para auditorías o para entender el flujo de aprobación de cotizaciones complejas.

---

##### Cambiar Estado de Cotización

Las cotizaciones siguen un flujo de estados definido. Cada transición requiere permisos específicos.

**Flujo Normal de Estados:**

```
┌──────────┐    Enviar a      ┌──────────────────┐
│ Borrador │ ──Aprobación──> │ Pend. Aprobación │
└──────────┘                  └──────────────────┘
                                      │
                                  Aprobar
                                      ↓
                              ┌────────────┐    Enviar      ┌──────────┐
                              │  Aprobada  │ ──Cliente───> │ Enviada  │
                              └────────────┘                └──────────┘
                                                                   │
                                                            Respuesta Cliente
                                                                   ↓
                                                         ┌─────────┴────────┐
                                                         ↓                  ↓
                                                   ┌──────────┐      ┌────────────┐
                                                   │ Aceptada │      │ Rechazada  │
                                                   └──────────┘      └────────────┘
```

---

**Acciones de Cambio de Estado:**

**1. Enviar a Aprobación** (Desde Borrador)
- **Requiere**: `COT_EDIT`
- **Acción**: Haga clic en botón azul "Enviar a Aprobación"
- **Resultado**: Cambia a "Pendiente Aprobación"
- **Notas**: La cotización queda bloqueada para edición hasta que se apruebe o rechace

**2. Aprobar Cotización** (Desde Pendiente Aprobación)
- **Requiere**: `COT_APPROVE`
- **Acción**: Haga clic en botón verde "Aprobar"
- **Resultado**: Cambia a "Aprobada"
- **Modal**: Puede agregar comentario de aprobación
- **Notas**: Solo usuarios con permiso de aprobar pueden hacer esto

**3. Rechazar Cotización** (Desde Pendiente o Enviada)
- **Requiere**: `COT_REJECT`
- **Acción**: Haga clic en botón rojo "Rechazar"
- **Resultado**: Cambia a "Rechazada"
- **Modal**: **Obligatorio** agregar motivo del rechazo
- **Notas**: Registra quién y por qué se rechazó

**4. Enviar al Cliente** (Desde Aprobada)
- **Requiere**: `COT_SEND_CLIENT`
- **Acción**: Haga clic en botón azul "Enviar a Cliente"
- **Resultado**: Cambia a "Enviada"
- **Registra**: Fecha de envío al cliente
- **Notas**: Indica que la propuesta fue enviada formalmente

**5. Marcar como Aceptada** (Desde Enviada)
- **Requiere**: `COT_ACCEPT`
- **Acción**: Haga clic en botón verde "Marcar como Aceptada"
- **Resultado**: Cambia a "Aceptada"
- **Registra**: Fecha de aceptación del cliente
- **Notas**: Indica éxito comercial

**6. Marcar como Rechazada** (Desde Enviada)
- **Requiere**: `COT_REJECT`
- **Acción**: Haga clic en botón rojo "Marcar como Rechazada"
- **Resultado**: Cambia a "Rechazada"
- **Modal**: Agregar motivo del cliente
- **Registra**: Fecha de rechazo

---

⚠️ **Advertencias Importantes:**

- **No puede revertir** algunos cambios de estado sin crear nueva versión
- **Los estados finales** (Aceptada/Rechazada) generalmente son permanentes
- **Cada cambio se registra** en el historial con usuario y timestamp
- **Los permisos son estrictos**: Solo usuarios autorizados pueden cambiar estados

💡 **Buenas Prácticas:**
1. Agregue siempre comentarios al aprobar o rechazar
2. Verifique la cotización antes de enviar al cliente
3. Confirme con el cliente antes de marcar como aceptada/rechazada
4. Use versiones si necesita modificar cotizaciones ya enviadas

---

##### Archivar Cotización

El archivado permite mover cotizaciones finalizadas fuera del listado principal, manteniendo el historial completo.

⚠️ **Requisito**: Permiso `COT_ARCHIVE`

**¿Cuándo Archivar una Cotización?**

✅ Archive cotizaciones cuando:
- El proceso comercial finalizó (aceptada o rechazada)
- Ya no requiere seguimiento activo
- Desea limpiar el listado principal
- La cotización es muy antigua pero debe conservarse

⚠️ **NO archive si:**
- La cotización aún está en negociación
- Puede requerir seguimiento en corto plazo
- No está seguro del estado final

---

**Paso 1: Verificar Estado**

Puede archivar cotizaciones en cualquier estado, pero generalmente se archivan:
- **Aceptadas**: Proceso comercial exitoso finalizado
- **Rechazadas**: Cliente declinó la propuesta
- **Borradores antiguos**: Cotizaciones obsoletas no completadas

---

**Paso 2: Archivar**

1. En la lista de cotizaciones, localice la cotización a archivar
2. Haga clic en el botón **naranja con icono de archivo** (Archivar)
3. Se abrirá un modal de confirmación

---

**Paso 3: Confirmar Archivado**

1. Lea el mensaje: `"¿Está seguro que desea archivar la cotización [ID]?"`
2. **Importante**: Entienda que:
   - La cotización desaparecerá del listado principal
   - Se podrá consultar en "Cotizaciones Archivadas"
   - Se puede reactivar posteriormente (con permiso `COT_ARCHIVE_REACTIVATE`)
   - El historial se conserva intacto

3. Agregue un **comentario** (opcional): Motivo del archivado
   - Ejemplo: `"Cliente no dio seguimiento después de 3 meses"`
   - Ejemplo: `"Proyecto cancelado por el cliente"`
   - Ejemplo: `"Cotización aceptada y facturada - finalizada"`

4. Haga clic en **"Archivar"**

---

**Paso 4: Verificar Archivado**

1. La cotización desaparece del listado principal
2. Verá notificación: `"Cotización archivada exitosamente"`
3. Se registra en el historial:
   - Fecha de archivado
   - Usuario que archivó
   - Comentario (si se agregó)

---

**Consultar Cotizaciones Archivadas:**

Para ver cotizaciones archivadas:
1. Use el menú: **Cotizaciones** > **Cotizaciones Archivadas**
2. Requiere permiso `COT_ARCHIVE_VIEW`

Para más detalles, vea la sección [5.3.3 Cotizaciones Archivadas](#533-cotizaciones-archivadas).

💡 **Sugerencia**: Archive periódicamente cotizaciones antiguas finalizadas para mantener el listado principal limpio y ágil.

---

#### 5.3.2 Nueva Cotización
*[Sección en construcción - Requiere análisis del formulario de creación]*

Esta sección documentará el proceso completo de crear una nueva cotización:
- Selección de cliente/interesado
- Configuración de moneda y tipo de cambio
- Agregado de productos/servicios
- Aplicación de descuentos e impuestos
- Notas y condiciones
- Guardado de cotización

---

#### 5.3.3 Cotizaciones Archivadas

El módulo de **Cotizaciones Archivadas** permite consultar, ver detalles y reactivar cotizaciones que han sido archivadas.

⚠️ **Requisito**: Permiso `COT_ARCHIVE_VIEW` para ver archivadas

---

##### **Acceso al Módulo**

Para acceder a cotizaciones archivadas:

1. En el **menú lateral izquierdo**, haga clic en **"Cotizaciones"**
2. Seleccione **"Cotizaciones Archivadas"** (tercera opción)
3. Se mostrará la pantalla de cotizaciones archivadas

---

##### Pantalla de Cotizaciones Archivadas

Similar a la pantalla principal de cotizaciones, pero enfocada en archivadas.

**Elementos de la Pantalla:**

1. **Encabezado**:
   - Título: "Cotizaciones Archivadas" con icono de archivo
   - Descripción: "Consulta y gestiona las cotizaciones archivadas del sistema"
   - Sin botón de crear (no aplica)

2. **Filtros Disponibles**:

   Similar a cotizaciones activas, pero con algunos ajustes:

   - **Búsqueda General**: ID, cliente, empresa
   - **Versión**: Número de versión
   - **Rango de Fechas de Creación**: Fecha Desde/Hasta
   - **Rango de Fechas de Archivado**: Cuándo fueron archivadas
   - **Rango de Montos**: Monto Desde/Hasta
   - **Filtrar por Moneda**: CRC, USD, EUR, etc.
   - **Usuario que Archivó**: Quién archivó la cotización

   ✅ **Por defecto**: Últimos 6 meses de cotizaciones archivadas

3. **Tabla de Cotizaciones Archivadas**:

   **Columnas adicionales específicas:**
   - **Usuario Archivó**: Quién archivó la cotización
   - **Fecha Archivado**: Cuándo fue archivada
   - Resto de columnas igual que listado principal

---

##### **Acciones Disponibles en Archivadas**

**Botones de Consulta:**
- 👁️ **Ver Detalle** - Requiere `COT_ARCHIVE_VIEW_DETAIL`
- 📚 **Versiones** - Requiere `COT_VIEW_VERSIONS`
- 📜 **Historial** - Requiere `COT_VIEW_HISTORY`

**Botones de Gestión:**
- 📤 **Reactivar** - Requiere `COT_ARCHIVE_REACTIVATE` - Devuelve al listado principal
- 📑 **Duplicar** - Requiere `COT_DUPLICATE` - Crea nueva cotización basada en archivada

⚠️ **Nota**: **NO** puede editar, archivar, ni cambiar estados de cotizaciones archivadas directamente. Debe reactivarlas primero.

---

##### Reactivar Cotización Archivada

Para devolver una cotización archivada al listado principal:

**Paso 1: Localizar Cotización**

1. En la pantalla de cotizaciones archivadas, use los filtros para encontrar la cotización
2. Verifique que sea la cotización correcta

---

**Paso 2: Reactivar**

1. Haga clic en el botón **verde con icono de flecha arriba** (Reactivar)
2. Se abrirá un modal de confirmación

---

**Paso 3: Confirmar Reactivación**

1. Lea el mensaje: `"¿Está seguro que desea reactivar la cotización [ID]?"`
2. **Importante**: La cotización:
   - Volverá al listado principal de cotizaciones activas
   - Mantendrá su estado original (Aceptada, Rechazada, etc.)
   - Conservará todo su historial
   - Podrá ser editada/gestionada según permisos y estado

3. Agregue un **comentario** (opcional): Motivo de reactivación
   - Ejemplo: `"Cliente retomó el proyecto"`
   - Ejemplo: `"Requiere seguimiento adicional"`

4. Haga clic en **"Reactivar"**

---

**Paso 4: Verificar Reactivación**

1. La cotización desaparece de la lista de archivadas
2. Verá notificación: `"Cotización reactivada exitosamente"`
3. La cotización aparece nuevamente en el listado principal
4. Se registra en el historial:
   - Fecha de reactivación
   - Usuario que reactivó
   - Comentario (si se agregó)

---

**Después de Reactivar:**

Una vez reactivada, puede:
- Verla en el listado principal de cotizaciones
- Editarla si el estado lo permite
- Cambiar su estado según flujo normal
- Archivarla nuevamente si es necesario

💡 **Sugerencia**: Use la reactivación cuando un cliente retoma una negociación que se consideraba finalizada.

---

##### Duplicar desde Archivadas

Puede crear una nueva cotización basada en una archivada:

1. Haga clic en **"Duplicar"** en la cotización archivada deseada
2. Se creará una **nueva cotización** (nuevo ID)
3. La nueva cotización estará en estado **Borrador**
4. La cotización archivada permanece en el archivo sin cambios

📝 **Ejemplo de Uso**: Cliente rechazó propuesta hace 6 meses, ahora solicita cotización similar para nuevo proyecto.

---

**Resumen de Gestión de Archivadas:**

| Acción | Permiso | Resultado |
|--------|---------|-----------|
| Ver Lista | `COT_ARCHIVE_VIEW` | Consulta cotizaciones archivadas |
| Ver Detalle | `COT_ARCHIVE_VIEW_DETAIL` | Ve información completa |
| Reactivar | `COT_ARCHIVE_REACTIVATE` | Devuelve a listado principal |
| Duplicar | `COT_DUPLICATE` | Crea nueva cotización independiente |
| Ver Versiones | `COT_VIEW_VERSIONS` | Consulta historial de versiones |
| Ver Historial | `COT_VIEW_HISTORY` | Consulta eventos registrados |

---

### 5.4 Configuración

El módulo de **Configuración** permite gestionar los parámetros del sistema que controlan el comportamiento de CotizacionesWeb. Este módulo es crítico para personalizar el sistema según las necesidades de su organización.

⚠️ **Advertencia**: Este módulo solo está disponible para usuarios con permisos de configuración. Cambios incorrectos pueden afectar el funcionamiento del sistema.

---

#### 5.4.1 Parámetros del Sistema

Los **Parámetros del Sistema** son valores configurables que controlan diferentes aspectos del funcionamiento de CotizacionesWeb.

##### **Acceso al Módulo**

Para acceder a la configuración de parámetros:

1. En el **menú lateral izquierdo**, haga clic en **"Configuración"**
2. Seleccione **"Parámetros del Sistema"**
3. Se mostrará la pantalla de configuración organizada por categorías

⚠️ **Requisito**: Necesita permiso `CFG_PARAMS_VIEW` para ver parámetros

---

##### Pantalla de Parámetros

La pantalla de configuración presenta los parámetros organizados por categorías mediante pestañas.

**Elementos de la Pantalla:**

1. **Encabezado**:
   - Título: "Configuración de Parámetros del Sistema"
   - Descripción: "Gestión de parámetros de configuración del sistema"
   - Alertas de validación (si aplica)

2. **Barra de Pestañas** (Categorías):

   Las categorías disponibles son:

   - 📊 **Financiero** - Parámetros de monedas, impuestos y tipos de cambio
   - 🔢 **Consecutivos** - Configuración de numeración automática
   - 🔗 **Integración HubSpot** - Conexión con HubSpot CRM
   - 🔗 **Integración ERP** - Conexión con sistema ERP
   - 🔔 **Notificaciones** - Configuración de notificaciones automáticas
   - 🔒 **Seguridad** - Parámetros de seguridad y sesiones
   - 📁 **Archivos** - Configuración de almacenamiento de archivos
   - 📄 **Reportes** - Información para generación de reportes
   - ⚙️ **Workflow** - Reglas de flujo de aprobación
   - 📝 **Plantilla Cotización** - Textos y formato de cotizaciones
   - 💼 **Negocio** - Reglas de negocio generales

   Cada pestaña muestra un **badge** con la cantidad de parámetros en esa categoría.

3. **Área de Parámetros**:

   **Cabecera de Categoría:**
   - Icono y nombre de la categoría
   - Descripción de la categoría
   - Cantidad de parámetros configurados
   - Badge de permisos (si solo tiene lectura)

   **Grid de Parámetros:**
   Cada parámetro se muestra en una tarjeta que incluye:
   - Código del parámetro
   - Tipo de valor (Texto, Número, Booleano)
   - Badges de estado (Solo lectura, Sensitivo)
   - Valor por defecto
   - Descripción detallada
   - Control de edición (input, textarea, select)
   - Botones de acción
   - Notas adicionales (si aplica)

---

##### **Tipos de Parámetros**

Los parámetros se clasifican según su tipo de valor:

**1. Texto (S - String)**
- Valores alfanuméricos
- Ejemplos: Códigos, URLs, correos electrónicos
- Control: Campo de texto simple

**2. Número (N - Numeric)**
- Valores numéricos (enteros o decimales)
- Ejemplos: Tasas, montos, cantidades
- Control: Campo numérico

**3. Booleano (B - Boolean)**
- Valores de Sí/No
- Representados como: `S` (Sí) o `N` (No)
- Control: Dropdown con opciones S/N

**4. Texto Largo**
- Textos extensos con saltos de línea
- Ejemplos: Condiciones comerciales, notas
- Control: Textarea con contador de caracteres

---

##### **Clasificación de Parámetros**

**Parámetros Editables:**
- Pueden ser modificados por usuarios con permiso `CFG_PARAMS_EDIT`
- Tienen controles de edición activos
- Botón "Guardar" visible

**Parámetros de Solo Lectura:**
- No pueden ser modificados (configurados por el sistema)
- Valor mostrado en modo lectura
- Ejemplos: Consecutivos automáticos
- Badge "Solo lectura"

**Parámetros Sensitivos:**
- Contienen información sensible (contraseñas, tokens, API keys)
- Valor enmascarado por defecto (`*********`)
- Badge rojo "🔒 Sensitivo"
- Requieren permiso `CFG_PARAMS_VIEW_SECRET` para ver valor real

---

##### Categorías de Parámetros

A continuación se describen las principales categorías de parámetros:

##### **1. Parámetros Financieros**

Controlan aspectos financieros del sistema:

| Código | Descripción | Tipo | Ejemplo |
|--------|-------------|------|---------|
| `TASA_IMPUESTO` | Tasa de impuesto por defecto (%) | Número | `13` |
| `MONEDA_DEFECTO` | Moneda por defecto del sistema | Texto | `CRC`, `USD` |
| `TIPO_CAMBIO_BASE` | Tipo de cambio base USD/CRC | Número | `540.00` |

📝 **Ejemplo de Uso**: Si cambia `TASA_IMPUESTO` de 13% a 15%, todas las nuevas cotizaciones usarán 15% como impuesto por defecto.

---

##### **2. Parámetros de Consecutivos**

Controlan la numeración automática de cotizaciones:

| Código | Descripción | Tipo | Editable |
|--------|-------------|------|----------|
| `MASCARA_CONSECUTIVO_COTIZACION` | Máscara para generar IDs | Texto | Sí |
| `CONSECUTIVO_COTIZACION` | Consecutivo actual | Texto | No |

**Formato de Máscara:**
- `A` = Letra
- `9` = Número
- `-` = Separador fijo
- Ejemplo: `COT-9999` genera: COT-0001, COT-0002, ...

⚠️ **Advertencia**: No modifique la máscara si ya tiene cotizaciones creadas. Esto puede generar conflictos de IDs.

---

##### **3. Parámetros de Integración HubSpot**

Configuran la conexión con HubSpot CRM:

| Código | Descripción | Sensitivo |
|--------|-------------|-----------|
| `HUBSPOT_ENABLED` | Activa integración (S/N) | No |
| `HUBSPOT_AUTH_TYPE` | Tipo de autenticación | No |
| `HUBSPOT_ACCESS_TOKEN` | Token de acceso | **Sí** 🔒 |
| `HUBSPOT_API_BASE_URL` | URL base del API | No |
| `HUBSPOT_PAGE_SIZE` | Registros por página | No |
| `HUBSPOT_TIMEOUT_SECONDS` | Timeout en segundos | No |
| `HUBSPOT_ACCOUNT_NAME` | Nombre de la cuenta | No |

⚠️ **Importante**: 
- Configure `HUBSPOT_ACCESS_TOKEN` antes de activar la integración
- Una vez asignado el token, no se puede modificar (solo lectura)
- Active `HUBSPOT_ENABLED = S` solo después de verificar la conexión

---

##### **4. Parámetros de Integración ERP**

Configuran la conexión con el sistema ERP:

| Código | Descripción | Tipo |
|--------|-------------|------|
| `ERP_ENABLED` | Activa integración (S/N) | Booleano |
| `ERP_NIVELPRECIO_LOCAL` | Nivel precio moneda local | Texto |
| `ERP_NIVELPRECIO_DOLAR` | Nivel precio dólar | Texto |
| `ERP_CIA` | Compañía en el ERP | Texto |
| `ERP_USAR_IMPUESTOS` | Calcular impuestos desde ERP | Booleano |

---

##### **5. Parámetros de Notificaciones**

Controlan el envío de notificaciones automáticas:

| Código | Descripción | Tipo |
|--------|-------------|------|
| `NOTIFICACIONES_COTIZACIONES_ENABLED` | Activa notificaciones | Booleano |
| `EMAIL_NOTIFICACIONES_PEND_APROBAR` | Email para avisos de aprobación | Texto |
| `DIAS_NOTIFICACION_ENVIADAS` | Días para notificar seguimiento | Número |

📝 **Ejemplo**: Si `DIAS_NOTIFICACION_ENVIADAS = 3`, el sistema notificará si una cotización enviada no tiene respuesta en 3 días.

---

##### **6. Parámetros de Seguridad**

Configuran aspectos de seguridad del sistema:

| Código | Descripción | Valor Defecto |
|--------|-------------|---------------|
| `SESSION_TIMEOUT_MINUTOS` | Timeout de sesión | 60 minutos |
| `MAX_INTENTOS_LOGIN` | Intentos fallidos antes de bloqueo | 5 |
| `BLOQUEO_CUENTA_MINUTOS` | Tiempo de bloqueo | 15 minutos |

⚠️ **Advertencia**: Valores muy bajos pueden bloquear usuarios legítimos frecuentemente.

---

##### **7. Parámetros de Workflow**

Controlan reglas de flujo de aprobación:

| Código | Descripción | Tipo |
|--------|-------------|------|
| `APROBACION_AUTOMATICA_CRC_MONTO` | Monto máximo CRC para aprobación automática | Número |
| `APROBACION_AUTOMATICA_DOL_MONTO` | Monto máximo USD para aprobación automática | Número |
| `REQUIERE_APROBACION_DESCUENTO` | % de descuento que requiere aprobación | Número |
| `DIAS_VIGENCIA_COTIZACION` | Días de vigencia por defecto | Número |

📝 **Ejemplo**: Si `APROBACION_AUTOMATICA_DOL_MONTO = 1000`, cotizaciones menores a $1,000 se aprueban automáticamente.

💡 **Sugerencia**: Use `0` en montos de aprobación automática para desactivar esta función.

---

##### **8. Parámetros de Plantilla Cotización**

Personalizan textos en las cotizaciones generadas:

| Código | Descripción | Tipo |
|--------|-------------|------|
| `FORMATO_COTIZACION` | Formato de exportación (PDF/EXCEL/WORD) | Texto |
| `COT_VIGENCIA` | Texto de vigencia de propuesta | Texto |
| `COT_CONDICIONES_PAGO` | Condiciones de pago | Texto Largo |
| `COT_NOTAS_COMERCIALES` | Notas comerciales adicionales | Texto Largo |
| `COT_TITULO_DETALLE` | Título sección de detalle | Texto |
| `COT_TITULO_RESUMEN` | Título sección de resumen | Texto |

📝 **Ejemplo**:
```
COT_VIGENCIA = "2 semanas"
COT_CONDICIONES_PAGO = "50% anticipo, 50% contra entrega"
COT_NOTAS_COMERCIALES = "Incluye instalación y capacitación"
```

---

##### Modificar Parámetros del Sistema

Para cambiar el valor de un parámetro editable:

⚠️ **Requisito**: Permiso `CFG_PARAMS_EDIT`

**Paso 1: Navegar a la Categoría**

1. Haga clic en la pestaña de la categoría que contiene el parámetro
2. Localice el parámetro que desea modificar en el grid

---

**Paso 2: Editar el Valor**

Según el tipo de parámetro:

**Para parámetros de Texto o Número:**
1. Haga clic en el campo de entrada
2. Modifique el valor
3. El sistema valida mientras escribe

**Para parámetros Booleanos:**
1. Use el dropdown para seleccionar `S` (Sí) o `N` (No)

**Para parámetros de Texto Largo (Textarea):**
1. Escriba o modifique el texto
2. Puede usar saltos de línea
3. Hay un contador de caracteres (límite: 1000)

---

**Paso 3: Validación Automática**

El sistema valida el valor ingresado:

✅ **Validaciones según tipo:**
- **Número**: Debe ser un número válido
- **Texto**: Longitud máxima permitida
- **Booleano**: Solo acepta S o N
- **Email**: Formato de correo válido
- **URL**: Formato de URL válido

Si hay error, verá un mensaje indicando el problema.

---

**Paso 4: Guardar Cambios**

1. Haga clic en el botón **"Guardar"** (azul con icono de diskette)
2. El sistema guardará el nuevo valor
3. Verá una notificación de éxito o error
4. El cambio se aplica inmediatamente en el sistema

📝 **Ejemplo**:
```
Parámetro: TASA_IMPUESTO
Valor actual: 13
Nuevo valor: 15
Acción: Clic en campo → Borrar "13" → Escribir "15" → Clic "Guardar"
Resultado: "Parámetro TASA_IMPUESTO actualizado correctamente"
```

---

##### **Resetear Parámetros**

Puede restaurar un parámetro a su valor por defecto.

⚠️ **Requisito**: Permiso `CFG_PARAMS_RESET`

**Paso 1: Localizar el Parámetro**

1. Navegue a la categoría correspondiente
2. Localice el parámetro que desea resetear
3. Verifique que tenga un **valor por defecto** definido

---

**Paso 2: Resetear**

1. Haga clic en el **botón con icono de flecha circular** (⟲ Resetear)
2. Se abrirá una confirmación
3. Confirme la acción

---

**Paso 3: Verificar**

1. El parámetro volverá a su valor original
2. Verá notificación: `"Parámetro [código] reseteado al valor por defecto"`
3. El cambio es inmediato

💡 **Sugerencia**: Use esta función si hizo cambios incorrectos y quiere volver al valor inicial.

---

##### **Parámetros Sensitivos**

Los parámetros sensitivos contienen información confidencial y tienen tratamiento especial.

**Características:**

1. **Valor Enmascarado**:
   - Se muestran como `*********` por defecto
   - Protege tokens, contraseñas, API keys

2. **Permisos Especiales**:
   - `CFG_PARAMS_EDIT`: Puede modificar (sin ver valor actual)
   - `CFG_PARAMS_VIEW_SECRET`: Puede ver valor real

3. **Edición de Parámetros Sensitivos:**

   **Si puede editar pero NO ver:**
   - Campo tipo password (`••••••`)
   - Puede escribir nuevo valor
   - No puede ver el valor actual
   - Botón "👁️ Mostrar/Ocultar" para ver lo que escribe

   **Si NO puede editar (Solo lectura):**
   - Valor mostrado enmascarado
   - Botón "Ver Valor Real" (si tiene permiso `CFG_PARAMS_VIEW_SECRET`)
   - Botón "🔒 Protegido" (si NO tiene permiso)

---

**Ver Valor Real de Parámetro Sensitivo:**

Si tiene permiso `CFG_PARAMS_VIEW_SECRET`:

1. Localice el parámetro sensitivo (badge rojo "Sensitivo")
2. Haga clic en el botón **"👁️ Ver Valor Real"**
3. El valor se revelará temporalmente
4. Se registra en logs quién vio el valor (auditoría de seguridad)

⚠️ **Advertencia de Seguridad**: 
- Solo usuarios de confianza deben tener este permiso
- Cada visualización se registra en logs del sistema
- No comparta valores sensitivos por canales inseguros

📝 **Ejemplo de Parámetro Sensitivo**:
```
Código: HUBSPOT_ACCESS_TOKEN
Descripción: Token de acceso HubSpot
Valor Mostrado: *********
Valor Real (con permiso): pat-na1-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```

---

##### **Validación de Consecutivos**

El sistema valida automáticamente la configuración de consecutivos.

**Validaciones Realizadas:**

1. ✅ Formato de máscara válido
2. ✅ Consecutivo actual coincide con máscara
3. ✅ Consecutivo no está duplicado
4. ✅ Máscara permite suficientes IDs

**Alertas:**

Si la configuración no es válida, verá una alerta en la parte superior:

```
⚠️ Atención: La configuración de consecutivos no es válida.
Revise los parámetros de la categoría "Consecutivos".
```

**Solución:**
1. Vaya a la pestaña "Consecutivos"
2. Verifique que `MASCARA_CONSECUTIVO_COTIZACION` tenga formato correcto
3. Ejemplo válido: `COT-9999`, `QUOTE-AA-9999`
4. El `CONSECUTIVO_COTIZACION` debe coincidir con el formato

---

##### **Mejores Prácticas**

Al modificar parámetros del sistema:

✅ **DO - Hacer:**
1. **Documente cambios**: Anote qué cambió y por qué
2. **Pruebe en desarrollo**: Si es posible, pruebe cambios antes de producción
3. **Haga backup**: Anote valores anteriores para poder revertir
4. **Lea descripciones**: Comprenda qué hace cada parámetro antes de cambiar
5. **Use valores por defecto**: Si no está seguro, resetee al valor original
6. **Valide resultados**: Verifique que el cambio tuvo el efecto esperado

⚠️ **DON'T - No Hacer:**
1. **No cambie sin entender**: Cambios incorrectos pueden afectar el sistema
2. **No modifique consecutivos**: Con cotizaciones existentes
3. **No desactive integraciones**: Sin verificar dependencias
4. **No cambie parámetros de seguridad**: Sin evaluar impacto
5. **No comparta valores sensitivos**: Tokens, passwords, API keys
6. **No ignore validaciones**: Si el sistema marca error, corrija antes de guardar

---

##### **Casos de Uso Comunes**

**Caso 1: Cambiar Tasa de Impuesto**

Escenario: El gobierno cambió el IVA de 13% a 15%

Pasos:
1. Vaya a pestaña **"Financiero"**
2. Localice `TASA_IMPUESTO`
3. Cambie valor de `13` a `15`
4. Clic en **"Guardar"**
5. Nuevas cotizaciones usarán 15% automáticamente

---

**Caso 2: Configurar Integración HubSpot**

Escenario: Quiere sincronizar clientes con HubSpot

Pasos:
1. Vaya a pestaña **"Integración HubSpot"**
2. Configure `HUBSPOT_ACCESS_TOKEN` con su token (⚠️ sensitivo)
3. Verifique `HUBSPOT_API_BASE_URL` = `https://api.hubapi.com`
4. Configure `HUBSPOT_ACCOUNT_NAME` con nombre de su cuenta
5. Active `HUBSPOT_ENABLED = S`
6. Guarde cada cambio

✅ **Verificación**: Vaya al módulo de Clientes y pruebe la sincronización

---

**Caso 3: Personalizar Textos de Cotización**

Escenario: Quiere agregar condiciones de pago específicas

Pasos:
1. Vaya a pestaña **"Plantilla Cotización"**
2. Localice `COT_CONDICIONES_PAGO`
3. Escriba en el textarea:
   ```
   - 50% de anticipo al firmar contrato
   - 25% a la entrega del producto
   - 25% después de instalación exitosa
   ```
4. Clic en **"Guardar"**
5. Las cotizaciones mostrarán este texto automáticamente

---

**Caso 4: Configurar Aprobación Automática**

Escenario: Cotizaciones menores a $500 no requieren aprobación

Pasos:
1. Vaya a pestaña **"Workflow"**
2. Localice `APROBACION_AUTOMATICA_DOL_MONTO`
3. Cambie valor a `500`
4. Clic en **"Guardar"**
5. Cotizaciones bajo $500 se aprobarán automáticamente

⚠️ **Precaución**: Asegúrese de que esta regla se alinee con políticas de la empresa

---

##### **Errores Comunes y Soluciones**

**Error: "Formato de valor inválido"**
- **Causa**: El valor ingresado no coincide con el tipo esperado
- **Solución**: 
  - Para números: Use solo dígitos y punto decimal (ej: `540.00`)
  - Para booleanos: Use solo `S` o `N`
  - Para texto: Respete longitud máxima

**Error: "No tiene permisos para modificar este parámetro"**
- **Causa**: Falta permiso `CFG_PARAMS_EDIT`
- **Solución**: Contacte al administrador para asignar permisos

**Error: "Este parámetro no puede ser modificado"**
- **Causa**: Es un parámetro de solo lectura (sistema)
- **Solución**: No se puede modificar directamente, el sistema lo actualiza

**Advertencia: "Consecutivos no válidos"**
- **Causa**: Máscara o consecutivo mal configurados
- **Solución**: 
  1. Vaya a categoría "Consecutivos"
  2. Verifique formato de `MASCARA_CONSECUTIVO_COTIZACION`
  3. Ejemplo válido: `COT-9999`
  4. Resetee si es necesario

---

##### **Resumen de Permisos**

| Permiso | Descripción | Puede |
|---------|-------------|-------|
| `CFG_PARAMS_VIEW` | Ver parámetros | Ver todos los parámetros (valores enmascarados si son sensitivos) |
| `CFG_PARAMS_EDIT` | Editar parámetros | Modificar valores de parámetros editables |
| `CFG_PARAMS_RESET` | Resetear parámetros | Restaurar valores por defecto |
| `CFG_PARAMS_VIEW_SECRET` | Ver valores sensitivos | Revelar valores reales de parámetros sensitivos |
| `CFG_LOGS` | Ver logs del sistema | Consultar logs de auditoría (no documentado en esta sección) |

---

💡 **Sugerencia Final**: Mantenga una lista documentada de los parámetros que ha modificado y sus valores originales. Esto facilitará troubleshooting y reversión de cambios si es necesario.

---

## 6. Errores Comunes y Soluciones

Esta sección proporciona soluciones a los problemas más frecuentes que pueden encontrar los usuarios del sistema CotizacionesWeb. Los errores están organizados por módulo para facilitar su búsqueda.

💡 **Sugerencia**: Use **Ctrl+F** para buscar el mensaje de error específico que está viendo en su pantalla.

---

### 6.1 Problemas de Inicio de Sesión

Errores relacionados con el acceso al sistema y autenticación de usuarios.

---

#### **Error: "Correo electrónico o contraseña incorrectos"**

**Síntoma**: Al intentar iniciar sesión, aparece el mensaje "Error al iniciar sesión" o "Credenciales inválidas"

**Causas Posibles:**
1. Contraseña escrita incorrectamente
2. Correo electrónico incorrecto o con errores tipográficos
3. Cuenta de usuario no existe en el sistema
4. Tecla Bloq Mayús activada

**Soluciones:**

✅ **Solución 1: Verificar Contraseña**
1. Asegúrese de que la tecla **Bloq Mayús** esté desactivada
2. Verifique que no hay espacios antes o después de la contraseña
3. Las contraseñas distinguen entre mayúsculas y minúsculas
4. Intente escribir la contraseña en un editor de texto primero para verificarla

✅ **Solución 2: Verificar Correo Electrónico**
1. Verifique que el correo esté escrito correctamente
2. Confirme que incluye el símbolo `@` y el dominio completo
3. Ejemplo correcto: `usuario@empresa.com`
4. Ejemplo incorrecto: `usuario empresa.com` o `usuario@empresa`

✅ **Solución 3: Solicitar Reseteo de Contraseña**
1. Contacte al administrador del sistema
2. Solicite el reseteo de su contraseña
3. Vea la sección [4.2 Recuperación de Contraseña](#42-recuperación-de-contraseña)

---

#### **Error: "Cuenta bloqueada"**

**Síntoma**: Mensaje indica que la cuenta ha sido bloqueada temporalmente

**Causa**: Demasiados intentos fallidos de inicio de sesión (por defecto: 5 intentos)

**Solución:**

✅ **Esperar el Tiempo de Bloqueo**
1. Por defecto, el bloqueo dura **15 minutos**
2. Espere el tiempo indicado sin intentar acceder
3. Después del tiempo, intente nuevamente con las credenciales correctas

✅ **Contactar al Administrador**
1. Si necesita acceso urgente, contacte al administrador
2. El administrador puede desbloquear su cuenta manualmente
3. Proporcione su correo electrónico y nombre completo

⚠️ **Prevención**: Después de 2 intentos fallidos, pare y verifique sus credenciales antes de continuar

---

#### **Error: "Sesión expirada"**

**Síntoma**: Después de estar inactivo, el sistema lo regresa a la pantalla de login

**Causa**: La sesión ha expirado por inactividad (timeout configurado en parámetros del sistema)

**Solución:**

✅ **Normal - Volver a Iniciar Sesión**
1. Esto es un comportamiento de seguridad normal
2. Simplemente ingrese sus credenciales nuevamente
3. Por defecto, las sesiones expiran después de **60 minutos** de inactividad

💡 **Prevención**:
- Guarde su trabajo frecuentemente
- Realice alguna acción en el sistema cada 30-40 minutos
- Cierre sesión manualmente si va a estar ausente por tiempo prolongado

---

#### **Error: "Usuario inactivo"**

**Síntoma**: Mensaje indica que el usuario está desactivado

**Causa**: El administrador desactivó su cuenta de usuario

**Solución:**

✅ **Contactar al Administrador**
1. Su cuenta fue desactivada intencionalmente
2. Contacte al administrador del sistema
3. Solicite la reactivación de su cuenta
4. Explique por qué necesita acceso

---

### 6.2 Errores al Gestionar Cotizaciones

Errores relacionados con la creación, modificación y gestión de cotizaciones.

---

#### **Error: "No se puede cambiar la moneda con líneas de detalle"**

**Síntoma**: Al intentar cambiar la moneda de una cotización, aparece este error

**Causa**: Ya hay productos/servicios agregados a la cotización

**Solución:**

✅ **Solución 1: Eliminar Líneas Primero**
1. Elimine todas las líneas de productos/servicios
2. Cambie la moneda
3. Vuelva a agregar los productos

⚠️ **Advertencia**: Perderá toda la información de productos ingresada

✅ **Solución 2: Crear Nueva Cotización**
1. Si ya tiene mucho trabajo hecho, es mejor crear una nueva cotización
2. Use "Duplicar" para copiar la estructura
3. Cambie la moneda en la nueva cotización
4. Elimine la antigua

💡 **Prevención**: Seleccione la moneda correcta **antes** de agregar productos

---

#### **Error: "La cotización no está en estado editable"**

**Síntoma**: No puede editar una cotización y aparece este mensaje

**Causa**: El estado actual de la cotización no permite edición

**Estados que NO Permiten Edición:**
- Enviada al cliente
- Aceptada por el cliente
- Rechazada
- Archivada

**Solución:**

✅ **Solución 1: Usar "Copiar Versión"**
1. En la lista de cotizaciones, localice la cotización
2. Haga clic en el botón "Copiar Versión"
3. Se creará una nueva versión en estado **Borrador**
4. Edite la nueva versión
5. La versión anterior se conserva como histórico

✅ **Solución 2: Reactivar si está Archivada**
1. Si la cotización está archivada, vaya a "Cotizaciones Archivadas"
2. Localice la cotización
3. Haga clic en "Reactivar"
4. Una vez reactivada, cree una copia si necesita modificarla

📝 **Nota**: Este comportamiento es intencional para mantener integridad del historial

---

#### **Error: "Debe agregar al menos una línea de detalle"**

**Síntoma**: No puede guardar la cotización sin productos

**Causa**: Intentó guardar una cotización sin productos o servicios

**Solución:**

✅ **Agregar Productos**
1. En el formulario de cotización, vaya a la sección de productos
2. Haga clic en "Agregar Producto" o botón similar
3. Complete la información del producto:
   - Descripción
   - Cantidad
   - Precio unitario
4. Guarde la línea de detalle
5. Ahora podrá guardar la cotización

⚠️ **Validación**: El sistema requiere al menos 1 producto para crear una cotización válida

---

#### **Error: "Monto total excede el límite"**

**Síntoma**: No puede guardar cotización con monto muy alto

**Causa**: El monto supera límites configurados en el sistema

**Solución:**

✅ **Solución 1: Dividir en Múltiples Cotizaciones**
1. Divida el proyecto en fases o partes
2. Cree una cotización por cada fase
3. Cada una con monto menor al límite

✅ **Solución 2: Contactar Administrador**
1. Si el monto es correcto y necesario
2. Solicite al administrador que ajuste el límite
3. O que apruebe manualmente la cotización

---

#### **Error: "ID de cotización duplicado"**

**Síntoma**: Error al crear cotización indicando ID duplicado

**Causa**: Problema con los consecutivos del sistema

**Solución:**

✅ **Contactar al Administrador Inmediatamente**
1. Este es un error crítico de configuración
2. **NO** intente solucionarlo usted mismo
3. Contacte al administrador del sistema
4. El administrador debe:
   - Ir a Configuración > Parámetros > Consecutivos
   - Verificar `CONSECUTIVO_COTIZACION` y `MASCARA_CONSECUTIVO_COTIZACION`
   - Corregir el consecutivo actual

⚠️ **Advertencia**: No cree cotizaciones hasta que esto se resuelva

---

### 6.3 Problemas con Permisos

Errores relacionados con falta de permisos o accesos denegados.

---

#### **Error: "No tiene permisos para realizar esta acción"**

**Síntoma**: Aparece mensaje de acceso denegado al intentar una operación

**Causa**: Su usuario no tiene el permiso necesario para esa acción específica

**Solución:**

✅ **Verificar Permisos Necesarios**
1. Identifique qué acción estaba intentando hacer
2. Consulte la tabla de permisos en la sección del manual correspondiente
3. Ejemplos:
   - Crear cotización: requiere `COT_CREATE`
   - Aprobar cotización: requiere `COT_APPROVE`
   - Ver parámetros: requiere `CFG_PARAMS_VIEW`

✅ **Solicitar Permisos al Administrador**
1. Contacte al administrador del sistema
2. Especifique qué acción necesita realizar
3. El administrador evaluará y asignará el permiso si es apropiado
4. Espere confirmación antes de intentar nuevamente

📝 **Ejemplo de Solicitud**:
```
Asunto: Solicitud de permiso - CotizacionesWeb

Estimado Administrador:

Necesito permiso para aprobar cotizaciones (COT_APPROVE).
Mi rol actual no me permite aprobar las cotizaciones 
de mi equipo.

Usuario: juan.perez@empresa.com
Rol actual: Vendedor
Permiso requerido: COT_APPROVE

Gracias.
```

---

#### **Error: "No puede acceder a este módulo"**

**Síntoma**: Un menú o módulo completo no está visible o está bloqueado

**Causa**: No tiene ningún permiso relacionado con ese módulo

**Solución:**

✅ **Verificar Roles Asignados**
1. Su rol actual no incluye permisos para ese módulo
2. Ejemplos:
   - Módulo Administración: requiere permisos `USR_*` o `ROL_*`
   - Módulo Configuración: requiere permisos `CFG_*`
   - Módulo Cotizaciones: requiere permisos `COT_*`

✅ **Solicitar Asignación de Rol**
1. Contacte al administrador
2. Solicite que le asignen un rol con acceso a ese módulo
3. Justifique por qué necesita acceso

---

#### **Error: "Solo puede editar sus propias cotizaciones"**

**Síntoma**: No puede editar cotizaciones creadas por otros usuarios

**Causa**: Restricción de permisos basada en propiedad

**Solución:**

✅ **Solución Temporal: Solicitar al Creador**
1. Contacte al usuario que creó la cotización
2. Solicítele que realice los cambios necesarios

✅ **Solución Permanente: Solicitar Permisos Ampliados**
1. Si frecuentemente necesita editar cotizaciones de otros
2. Solicite al administrador permisos de supervisor
3. O que le asignen el rol "Supervisor" que tiene acceso amplio

---

### 6.4 Errores de Validación

Errores relacionados con validaciones de datos y formularios.

---

#### **Error: "El campo [nombre] es requerido"**

**Síntoma**: No puede guardar porque falta completar un campo obligatorio

**Causa**: Campo obligatorio vacío

**Solución:**

✅ **Completar el Campo**
1. Localice el campo marcado en rojo
2. Complete la información requerida
3. Los campos obligatorios suelen tener un asterisco (*) o están marcados

💡 **Campos Comúnmente Obligatorios**:

**En Usuarios:**
- Nombre Completo
- Correo Electrónico
- Contraseña
- Al menos 1 rol

**En Cotizaciones:**
- Cliente/Interesado
- Moneda
- Al menos 1 producto

**En Roles:**
- Nombre del Rol

---

#### **Error: "Formato de correo electrónico inválido"**

**Síntoma**: El sistema no acepta el correo ingresado

**Causa**: El correo no tiene formato válido

**Solución:**

✅ **Verificar Formato**
1. **Formato correcto**: `usuario@dominio.com`
2. **Debe incluir**:
   - Nombre de usuario
   - Símbolo `@`
   - Dominio completo
   - Extensión (.com, .net, .cr, etc.)

📝 **Ejemplos**:
```
✅ Correcto: juan.perez@empresa.com
✅ Correcto: maria_gomez@empresa.co.cr
✅ Correcto: ventas.norte@miempresa.net

❌ Incorrecto: juan.perez (falta @dominio)
❌ Incorrecto: juan@empresa (falta extensión)
❌ Incorrecto: @empresa.com (falta usuario)
```

---

#### **Error: "Las contraseñas no coinciden"**

**Síntoma**: Al crear/resetear usuario, indica que las contraseñas no son iguales

**Causa**: Los campos "Contraseña" y "Confirmar Contraseña" son diferentes

**Solución:**

✅ **Escribir Exactamente Igual**
1. Borre ambos campos
2. Escriba la contraseña en el primer campo
3. Cópiela (Ctrl+C) y péguela (Ctrl+V) en el segundo campo
4. O escríbala cuidadosamente igual en ambos

⚠️ **Recuerde**: Las contraseñas distinguen entre mayúsculas y minúsculas

💡 **Truco**: Use el botón "Mostrar/Ocultar" (👁️) para ver lo que escribe

---

#### **Error: "Fecha inválida" o "Fecha debe ser mayor a hoy"**

**Síntoma**: El sistema no acepta la fecha ingresada

**Causa**: Fecha en formato incorrecto o no cumple reglas de negocio

**Solución:**

✅ **Verificar Formato**
1. Use el selector de fechas (calendario) en lugar de escribir manualmente
2. Formato esperado: `DD/MM/AAAA`
3. Ejemplo: `15/01/2025`

✅ **Verificar Reglas de Negocio**
1. **Fecha Hasta** debe ser mayor que **Fecha Desde**
2. Fechas futuras pueden no estar permitidas en algunos campos
3. Fechas muy antiguas pueden ser rechazadas

---

#### **Error: "Valor numérico inválido"**

**Síntoma**: El sistema no acepta el número ingresado

**Causa**: Formato numérico incorrecto

**Solución:**

✅ **Verificar Formato Numérico**
1. Use **punto** (.) como separador decimal, no coma (,)
2. No use separadores de miles
3. Solo números, punto decimal opcional

📝 **Ejemplos**:
```
✅ Correcto: 1500
✅ Correcto: 1500.50
✅ Correcto: 0.15

❌ Incorrecto: 1,500 (coma como separador de miles)
❌ Incorrecto: 1500,50 (coma decimal)
❌ Incorrecto: $1500 (símbolo de moneda)
```

---

### 6.5 Errores de Conexión y Rendimiento

Problemas relacionados con la conexión al sistema y velocidad.

---

#### **Error: "No se puede conectar al servidor"**

**Síntoma**: Página no carga o muestra error de conexión

**Causa**: Problema de red, servidor caído, o URL incorrecta

**Solución:**

✅ **Solución 1: Verificar Conexión a Internet**
1. Abra otro sitio web (ej: www.google.com) para verificar conectividad
2. Si no funciona, hay problema con su internet
3. Reinicie su router/módem
4. Contacte a su proveedor de internet si persiste

✅ **Solución 2: Verificar URL**
1. Verifique que la dirección del sistema sea correcta
2. Confirme con el administrador si cambió la URL

✅ **Solución 3: Contactar a Soporte Técnico**
1. Si otros sitios funcionan pero CotizacionesWeb no
2. El servidor puede estar caído
3. Contacte al equipo de soporte técnico o administrador
4. Reporte el horario exacto del problema

---

#### **Error: "La página carga muy lento"**

**Síntoma**: El sistema tarda mucho en responder

**Causa**: Conexión lenta, caché del navegador, o carga en el servidor

**Solución:**

✅ **Solución 1: Limpiar Caché del Navegador**

**En Chrome:**
1. Presione `Ctrl + Shift + Delete`
2. Seleccione "Imágenes y archivos en caché"
3. Seleccione "Últimas 4 semanas"
4. Haga clic en "Borrar datos"
5. Recargue la página

**En Edge:**
1. Presione `Ctrl + Shift + Delete`
2. Seleccione "Archivos e imágenes en caché"
3. Haga clic en "Borrar ahora"
4. Recargue la página

✅ **Solución 2: Cerrar Pestañas Innecesarias**
1. Cierre otras pestañas del navegador
2. Cierre aplicaciones que usen internet
3. Deje solo CotizacionesWeb abierto

✅ **Solución 3: Verificar Conexión a Internet**
1. Haga un test de velocidad (www.speedtest.net)
2. Velocidad mínima recomendada: 5 Mbps
3. Si es menor, considere mejorar su conexión

---

### 6.6 Problemas de Configuración

Errores relacionados con parámetros del sistema.

---

#### **Error: "Consecutivos no válidos" (Alerta)**

**Síntoma**: Alerta amarilla en la parte superior de la pantalla de parámetros

**Causa**: Configuración incorrecta de máscaras de consecutivos

**Solución (Solo Administradores):**

✅ **Verificar Configuración**
1. Vaya a **Configuración** > **Parámetros del Sistema**
2. Pestaña **"Consecutivos"**
3. Verifique `MASCARA_CONSECUTIVO_COTIZACION`
4. Formato válido: `COT-9999`, `QUOTE-AA-9999`
5. Verifique que `CONSECUTIVO_COTIZACION` coincida con el formato

📝 **Ejemplos Válidos**:
```
Máscara: COT-9999
Consecutivo: COT-0001 ✅
Consecutivo: COT-1234 ✅

Máscara: QUOTE-AA-9999
Consecutivo: QUOTE-AB-0001 ✅
```

📝 **Ejemplos Inválidos**:
```
Máscara: COT-9999
Consecutivo: QUOTE-0001 ❌ (no coincide con máscara)

Máscara: COT-999
Consecutivo: COT-10000 ❌ (excede máscara)
```

---

### 6.7 Recomendaciones Generales

Mejores prácticas para evitar errores comunes.

---

#### **✅ Prevención de Errores**

**Antes de Guardar:**
1. **Revise toda la información** ingresada
2. **Verifique campos obligatorios** estén completos
3. **Compruebe totales** si son cantidades/montos
4. **Lea mensajes de validación** si aparecen

**Durante el Trabajo:**
1. **Guarde frecuentemente** (cada 10-15 minutos)
2. **No cierre el navegador** sin guardar cambios
3. **Realice una acción ocasional** si está leyendo documentos largos (evita timeout)

**Buenas Prácticas:**
1. **Use navegadores actualizados** (Chrome, Edge, Firefox)
2. **Mantenga conexión estable** a internet
3. **Cierre sesión** al terminar, especialmente en equipos compartidos
4. **Anote IDs importantes** (cotizaciones, usuarios) para referencia

---

#### **🆘 ¿Cuándo Contactar al Administrador?**

Contacte al administrador del sistema si:

❌ **Problemas de Acceso:**
- No puede iniciar sesión después de varios intentos
- Olvidó su contraseña
- Necesita acceso a módulos bloqueados

❌ **Problemas de Permisos:**
- No puede realizar acciones necesarias para su trabajo
- Necesita permisos adicionales
- Debe cambiar de rol

❌ **Errores del Sistema:**
- Errores de "servidor" o "base de datos"
- Consecutivos duplicados
- Problemas de configuración

❌ **Datos Críticos:**
- Necesita recuperar información eliminada
- Problemas con cotizaciones importantes
- Pérdida de datos

---

#### **📞 Información de Contacto**

Para reportar problemas o solicitar asistencia:

1. **Administrador del Sistema**: [Contacto de su organización]
2. **Soporte Técnico**: [Si aplica en su organización]
3. **Mesa de Ayuda**: [Si existe en su organización]

📝 **Al reportar un error, proporcione**:
- Descripción detallada del problema
- Pasos que realizó antes del error
- Mensaje de error exacto (captura de pantalla si es posible)
- Hora y fecha del incidente
- Navegador y versión que está usando

---

## 7. Conclusión

Felicitaciones por completar la lectura del **Manual de Usuario de CotizacionesWeb**. Esperamos que este documento le haya proporcionado una comprensión clara y completa de todas las funcionalidades del sistema.

---

### Resumen del Sistema

**CotizacionesWeb** es una herramienta poderosa diseñada para:

✅ **Optimizar** el proceso de creación y gestión de cotizaciones  
✅ **Centralizar** toda la información comercial en un solo lugar  
✅ **Controlar** el flujo de aprobación con permisos granulares  
✅ **Rastrear** el historial completo de cada cotización  
✅ **Facilitar** la colaboración entre equipos comerciales  
✅ **Asegurar** la integridad y trazabilidad de la información  

---

### Lo Que Ha Aprendido

A través de este manual, ha explorado:

📚 **Conceptos Fundamentales**:
- Estructura del sistema y navegación
- Roles y permisos
- Flujo de estados de cotizaciones
- Configuración del sistema

📋 **Operaciones Principales**:
- Gestión de usuarios y roles
- Creación y modificación de cotizaciones
- Versionado y duplicación
- Archivo y reactivación
- Configuración de parámetros

🔧 **Solución de Problemas**:
- Errores comunes y sus soluciones
- Mejores prácticas de uso
- Cuándo contactar al administrador

---

### Próximos Pasos

Para aprovechar al máximo CotizacionesWeb:

**1️⃣ Practique con Datos de Prueba**
- Familiarícese con el sistema antes de trabajar con datos reales
- Explore todas las funcionalidades de su rol
- Pruebe crear, modificar y gestionar cotizaciones de prueba

**2️⃣ Personalice su Experiencia**
- Agregue marcadores a páginas frecuentes
- Aprenda los atajos de teclado disponibles
- Configure alertas de navegador si el sistema las soporta

**3️⃣ Mantenga Este Manual a Mano**
- Guárdelo en un lugar accesible
- Consúltelo cuando tenga dudas
- Use el índice para navegación rápida

**4️⃣ Comparta Conocimiento**
- Ayude a nuevos usuarios de su equipo
- Reporte errores o mejoras sugeridas
- Contribuya a la documentación de su organización

---

### Mejores Prácticas para el Uso Diario

Para un uso eficiente y seguro del sistema:

#### **Seguridad**
🔒 **Proteja su cuenta**:
- Use contraseñas seguras y únicas
- No comparta sus credenciales con nadie
- Cierre sesión al terminar de trabajar
- Reporte actividad sospechosa inmediatamente

#### **Eficiencia**
⚡ **Optimice su trabajo**:
- Guarde con frecuencia para evitar pérdida de datos
- Use filtros para encontrar información rápidamente
- Aproveche la función de duplicar para cotizaciones similares
- Revise el Dashboard regularmente para priorizar tareas

#### **Calidad de Datos**
📊 **Mantenga información precisa**:
- Verifique dos veces antes de enviar cotizaciones al cliente
- Complete todos los campos relevantes
- Use descripciones claras en productos/servicios
- Agregue comentarios en cambios de versión para facilitar trazabilidad

#### **Colaboración**
👥 **Trabaje en equipo**:
- Use el historial para entender cambios de otros usuarios
- Agregue comentarios al aprobar o rechazar cotizaciones
- Respete los flujos de aprobación establecidos
- Comuníquese con su equipo sobre cotizaciones compartidas

---

### Evolución Continua del Sistema

**CotizacionesWeb** es un sistema en constante mejora:

🔄 **Actualizaciones**:
- El sistema puede recibir actualizaciones periódicas
- Nuevas funcionalidades pueden ser agregadas
- Este manual se actualiza para reflejar cambios

📢 **Mantente Informado**:
- Consulte regularmente este manual
- Preste atención a notificaciones del administrador
- Reporte sugerencias de mejora

---

### Agradecimiento

Gracias por dedicar tiempo a leer este manual. Su compromiso con aprender el sistema correctamente contribuye al éxito de toda la organización.

El equipo de desarrollo de CotizacionesWeb ha trabajado arduamente para crear una herramienta que:
- Simplifique su trabajo diario
- Reduzca errores y reprocesos
- Mejore la experiencia de sus clientes
- Proporcione información valiosa para la toma de decisiones

---

### Soporte y Asistencia

Recuerde que no está solo en este proceso:

📧 **Administrador del Sistema**
- Primera línea de soporte para su organización
- Puede asignar permisos y resolver problemas de acceso
- Contacto: [Proporcionado por su organización]

💻 **Soporte Técnico**
- Asistencia con problemas técnicos del sistema
- Reporte de errores y bugs
- Contacto: [Si aplica en su organización]

📚 **Este Manual**
- Referencia permanente disponible
- Actualizado periódicamente
- Versión actual: **1.0**

---

### Versión y Control de Cambios

**Versión del Manual**: 1.0  
**Fecha de Publicación**: Abril 2026  
**Elaborado por**: Marcela Jimenez Arguedas  
**Última Revisión**: Abril 2026

**Historial de Versiones**:
- **v1.0 (Abril 2026)**: Primera versión completa del manual
  - Documentación de todos los módulos principales
  - Guías paso a paso para todas las operaciones
  - Sección completa de errores y soluciones
  - Ejemplos prácticos y casos de uso

---

### Comentarios y Retroalimentación

Su experiencia usando este manual es importante para nosotros:

💡 **Ayúdenos a Mejorar**:
- ¿Encontró información confusa o incompleta?
- ¿Tiene sugerencias para mejorar explicaciones?
- ¿Descubrió errores o información desactualizada?
- ¿Necesita documentación sobre algún tema específico?

📝 **Cómo Proporcionar Retroalimentación**:
1. Contacte al administrador del sistema de su organización
2. Especifique la sección del manual (número de página o sección)
3. Describa su sugerencia o corrección
4. Proporcione ejemplos si es posible

---

### Declaración Final

Este manual es un **documento vivo** que evolucionará con el sistema. Esperamos que le haya servido como una guía completa y clara para aprovechar al máximo **CotizacionesWeb**.

Recuerde:
- 🎯 **Practique regularmente** para dominar el sistema
- 📖 **Consulte este manual** cuando tenga dudas
- 🤝 **Colabore con su equipo** para optimizar procesos
- 📈 **Aproveche las métricas** del Dashboard para mejorar resultados

---

### ¡Le Deseamos Mucho Éxito!

Esperamos que **CotizacionesWeb** se convierta en una herramienta indispensable para su trabajo diario, ayudándole a:

✨ Crear cotizaciones profesionales más rápidamente  
✨ Mantener un mejor control de sus propuestas comerciales  
✨ Mejorar la comunicación con sus clientes  
✨ Aumentar su tasa de aceptación de cotizaciones  
✨ Reducir el tiempo de cierre de ventas  

**¡Gracias por usar CotizacionesWeb!**

---

<div style="text-align: center; padding: 20px; border-top: 2px solid #007bff; margin-top: 40px;">

**Manual de Usuario - Sistema CotizacionesWeb**

**Versión 1.0** | Abril 2026

© 2026 CotizacionesWeb - Todos los derechos reservados

*Desarrollado por Marcela Jimenez Arguedas con dedicación para optimizar su gestión de cotizaciones*

🌐 **Sistema Web Profesional de Gestión de Cotizaciones**

</div>
