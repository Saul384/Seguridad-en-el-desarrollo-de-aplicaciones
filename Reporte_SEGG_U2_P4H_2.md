# Reporte de Práctica: Integración y Enriquecimiento de Registros con Promtail y Loki

**Nombre del Alumno:** [Tu Nombre]  
**Matrícula:** [Tu Matrícula]  
**Materia:** Seguridad en el Desarrollo de Aplicaciones  
**Práctica:** SEGG-U2-P4H-2  
**Fecha:** 14 de Julio de 2026  

---

## 1. Objetivo
Configurar Promtail para recolectar y enriquecer los archivos de registro generados por VulnerableApp, enviarlos a Loki y validar que los mismos eventos puedan consultarse tanto desde Seq como desde Grafana, sin modificar la lógica de negocio de la aplicación.

## 2. Desarrollo de la Práctica

### Paso 1. Verificar la infraestructura existente
Se comprobó que los contenedores de Grafana, Loki y Promtail se encontraran en ejecución y que VulnerableApp estuviese generando registros tanto en consola como en Seq.

**Evidencia 1: Captura de `docker ps` y de Seq mostrando eventos.**
> *[ INSERTA AQUÍ TU CAPTURA DE PANTALLA DE DOCKER PS ]*
> *[ INSERTA AQUÍ TU CAPTURA DE PANTALLA DE SEQ ]*

### Paso 2. Análisis del archivo de configuración de Promtail (`promtail-config.yml`)
A continuación se describe la finalidad de cada sección principal del archivo:

| Sección | Función Principal |
| :--- | :--- |
| **`server`** | Configura el servidor interno (HTTP/gRPC) de Promtail. Define los puertos de escucha para exponer métricas de salud y herramientas de depuración. |
| **`clients`** | Define el destino al cual Promtail enviará los registros recolectados. En este caso, apunta a la API de ingesta de Loki (`http://loki:3100/loki/api/v1/push`). |
| **`positions`** | Especifica un archivo local (`/tmp/positions.yaml`) donde Promtail guarda la ubicación (posición en bytes) hasta donde ha leído cada archivo de log, evitando duplicados. |
| **`scrape_configs`** | Sección principal donde se definen los trabajos (*jobs*) de recolección de logs, es decir, de qué origen recolectar y cómo procesarlos. |
| **`static_configs`** | Define los archivos de origen a leer (mediante la etiqueta `__path__`) y asigna las etiquetas (*labels*) estáticas iniciales para ese conjunto de logs. |
| **`labels`** | Pares clave-valor que se adjuntan a cada línea de log. Constituyen la base de la indexación en Loki y permiten clasificar y filtrar registros eficientemente. |

### Paso 3. Configuración del monitoreo de múltiples archivos
Para que Promtail pueda leer múltiples archivos de registro de VulnerableApp, se utilizó un patrón de búsqueda dentro de la etiqueta `__path__`:
```yaml
__path__: /app/Logs/{log-*.txt,security-*.txt}
```
**¿Por qué Promtail requiere conocer la ubicación física de los archivos?**
Promtail funciona realizando *tailing* directamente sobre el sistema de archivos. Necesita la ruta física para abrir los descriptores de archivo, calcular los desplazamientos (*offsets*) y detectar rotaciones de archivos (cuando se crea un archivo nuevo). Por ello, se monta el volumen local de los logs hacia dentro del contenedor en Docker.

### Paso 4. Enriquecimiento de registros mediante Labels
Se configuraron las siguientes etiquetas para distinguir los registros:
- **Application**: Eventos generales de la aplicación (etiqueta por defecto).
- **Security**: Eventos críticos como intentos de acceso, fallos de login y logout.
- **Audit**: Operaciones como adición de comentarios, búsquedas y uso de controladores.

**Ventajas frente a una búsqueda textual:**
Las labels son indexadas por Loki. Al realizar una búsqueda por `{category="Security"}`, el motor no necesita escanear línea por línea todos los gigabytes de logs usando costosas expresiones regulares. Solo busca dentro del pequeño subconjunto pre-etiquetado, haciendo las consultas escalables y estructuradas.

### Pasos 5 al 7. Aplicación, Generación de Actividad y Validación en Grafana
Se reinició el contenedor de Promtail para aplicar la configuración y se interactuó con VulnerableApp. Los resultados filtrados por las etiquetas creadas se observan a continuación en Grafana:

**Evidencia 2: Captura de Grafana mostrando consultas por labels.**
> *[ INSERTA AQUÍ TUS CAPTURAS DE GRAFANA CON CONSULTAS COMO `{category="Security"}` O `{category="Audit"}` ]*

---

## 3. Análisis Comparativo: Seq vs Grafana + Loki (Paso 8)

| Característica | Seq | Grafana + Loki |
| :--- | :--- | :--- |
| **Búsqueda textual** | Excelente. Optimizada para buscar sobre propiedades dinámicas enviadas por Serilog. | Muy potente pero basada en texto plano usando expresiones regulares (LogQL: `\|~ "error"`). |
| **Filtros por labels** | Los filtros se basan en las propiedades nativas del log (ej. `@Level`, `MachineName`). | Se basa en *labels* indexadas inyectadas por el recolector (Promtail). Ideal para segmentar flujos de logs. |
| **Dashboards** | Posee paneles analíticos predefinidos útiles, pero con personalización visual limitada. | Excepcional. Permite crear dashboards altamente personalizables cruzando Logs, Métricas y Traces en una vista. |
| **Consultas avanzadas**| Usa sintaxis tipo SQL nativa para agrupar, filtrar y contar eventos estructurados. | Usa LogQL, que permite extraer métricas desde logs y graficar tasas (rates) en el tiempo. |
| **Escalabilidad** | Moderada. Requiere licenciamiento de pago en despliegues distribuidos a nivel clúster. | Altamente escalable, diseñado cloud-native para ingesta masiva (S3, GCS) y entornos de Kubernetes. |
| **Uso principal** | Análisis de logs para desarrolladores (.NET) buscando errores y trazas en el código. | Observabilidad centralizada de infraestructura (DevOps/SRE) uniendo múltiples microservicios. |

---

## 4. Pregunta de Reflexión
**¿Qué ocurriría si Promtail no almacenara la posición del último registro leído en `positions.yaml` y cómo afectaría la recolección de eventos?**
Si no se almacenara esta posición, al reiniciarse el servicio, Promtail abriría los archivos y comenzaría a leerlos nuevamente desde la línea 1. Esto afectaría gravemente la recolección provocando que **todos los eventos históricos se dupliquen** repetidamente en Loki cada vez que el contenedor arranque, saturando el almacenamiento y falsificando la información visualizada en los dashboards.

---

## 5. Reto: Configuración de Labels Adicionales
Se configuraron las siguientes labels dinámicas y estáticas en el archivo de Promtail:
- `environment=dev`
- `module=authentication`
- `module=orders`

**Procedimiento en Grafana:**
Para filtrar únicamente los registros del módulo de Autenticación, se accedió a la vista de **Explore** y se ejecutó la siguiente consulta LogQL:
```logql
{environment="dev", module="authentication"}
```

**Evidencia 3: Archivo `promtail-config.yml` y captura del reto en Grafana.**

**Archivo `promtail-config.yml` Final:**
```yaml
server:
  http_listen_port: 9080
  grpc_listen_port: 0

positions:
  filename: /tmp/positions.yaml

clients:
  - url: http://loki:3100/loki/api/v1/push

scrape_configs:
  - job_name: vulnerableapp
    static_configs:
      - targets:
          - localhost
        labels:
          job: vulnerableapp_logs
          __path__: /app/Logs/{log-*.txt,security-*.txt}
          category: Application
          environment: dev
    pipeline_stages:
      - match:
          selector: '{job="vulnerableapp_logs"} |~ "(?i)(Autenticación|Fallo de login|Intento de acceso|Logout)"'
          stages:
            - labels:
                category: "Security"
                module: "authentication"
      - match:
          selector: '{job="vulnerableapp_logs"} |~ "(?i)(AddComment|ApiController|Search|Usuario: )" !~ "(?i)(Autenticación|Fallo de login|Intento de acceso|Logout)"'
          stages:
            - labels:
                category: "Audit"
      - match:
          selector: '{job="vulnerableapp_logs"} |~ "(?i)(order|pedido)"'
          stages:
            - labels:
                module: "orders"
```
> *[ INSERTA AQUÍ LA CAPTURA DE PANTALLA EN GRAFANA MOSTRANDO LA CONSULTA `{module="authentication"}` ]*

---
*Fin del reporte.*
