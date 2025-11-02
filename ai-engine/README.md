# IA Backend - Simulación Conversacional (NeuroPuentes)

Este proyecto es el backend de la aplicación de simulación conversacional para NeuroPuentes. Es un servidor API (FastAPI) que recibe un archivo de audio, lo procesa a través de una cadena de modelos de IA y devuelve una respuesta en audio y texto.

**Flujo de la API:**
1.  **Entrada:** Audio del "estudiante" (`.mp3`, `.wav`, etc.) + un JSON de contexto.
2.  **STT (Voz a Texto):** El audio se transcribe usando **Whisper** (modelo `small`).
3.  **Generación de Respuesta:** El texto transcrito se envía a un LLM (**LLaMA 3.2 1B**) junto con un "guion" de personalidad dinámica.
4.  **TTS (Texto a Voz):** La respuesta generada por el LLM se convierte de nuevo en audio usando **Coqui TTS**.
5.  **Salida:** Un JSON que contiene la transcripción, el texto de la respuesta y el audio de la respuesta en Base64.

---

## 🚀 Puesta en Marcha y Configuración

Para que el proyecto funcione, necesitas 4 dependencias críticas **antes** de instalar Python.

### 1. Dependencias del Sistema (¡Crítico!)

Esta fue la parte más difícil. Asegúrate de tener esto instalado:

* **Python 3.11:** El proyecto **NO** es compatible con Python 3.12 o superior debido a la dependencia de `TTS`.
    * **Corroborar:** `py --list` (asegúrate de que `3.11` esté en la lista).
* **Visual C++ Build Tools:** Necesario para compilar `TTS`.
    * **Instalación:** Descarga el "Visual Studio Installer" -> Cargas de trabajo -> Marca **"Desarrollo de escritorio con C++"**.
    * **Corroborar:** Busca "Developer Command Prompt" en el menú inicio. Si aparece, está instalado.
* **FFmpeg:** Necesario para que `Whisper` pueda leer archivos `.mp3`.
    * **Instalación:** `winget install Gyan.FFmpeg`
    * **Corroborar:** Abre una terminal nueva y escribe `ffmpeg`. Si te muestra la información de la versión, está listo.
* **Cuenta de Hugging Face:**
    * **Permiso 1 (Modelo):** Ve a [https://huggingface.co/meta-llama/Llama-3.2-1B-Instruct](https://huggingface.co/meta-llama/Llama-3.2-1B-Instruct) y acepta los términos de Meta. Tu solicitud debe estar "accepted".
    * **Permiso 2 (Token):** Ve a `Settings > Access Tokens`. Edita tu token y asegúrate de que la casilla **"Access public gated repositories"** esté marcada.

### 2. Instalación de Python

1.  **Crea el entorno virtual (venv)** especificando Python 3.11:
    ```bash
    py -3.11 -m venv venv
    ```

2.  **Activa el venv.**
    * En Git Bash:
        ```bash
        source venv/Scripts/activate
        ```
    * En CMD / Developer Command Prompt:
        ```cmd
        venv\Scripts\activate
        ```

3.  **Inicia sesión en Hugging Face** (usa el token que creaste):
    ```bash
    huggingface-cli login
    ```

4.  **Instala los requerimientos:**
    ```bash
    pip install -r requirements.txt
    ```

### 3. Ejecutar el Servidor

1.  Asegúrate de que tu `venv` esté activo.
2.  Ejecuta `main.py`:
    ```bash
    python main.py
    ```
3.  La primera vez, el servidor tardará **varios minutos** en descargar los modelos de Whisper (`small`) y LLaMA (`1B`). Sé paciente.
4.  El servidor estará listo cuando veas:
    ```
    INFO:     Uvicorn running on [http://0.0.0.0:5001](http://0.0.0.0:5001) (Press CTRL+C to quit)
    ```

---

## 🧪 Cómo Probar la API

La forma más fácil de corroborar que todo funciona es usando la documentación interactiva:

1.  Con el servidor corriendo, abre tu navegador y ve a:
    **[http://127.0.0.1:5001/docs](http://127.0.0.1:5001/docs)**

2.  Prueba el endpoint `GET /health` para verificar que todos los modelos cargaron (`"whisper_loaded": true`, etc.).

3.  Prueba el endpoint `POST /process_audio`:
    * Haz clic en "Try it out".
    * **`audio`**: Sube un archivo `.mp3` o `.wav` corto.
    * **`context_id`**: Déjalo con su valor por defecto.
    * **`context_json`**: Aquí es donde pruebas la IA dinámica. Pega un JSON *como string*. Ejemplo:
        ```json
        '["rol_padre", "hija_15_anios", "emocion_preocupado_futuro", "regla_extension_corta"]'
        ```
    * Haz clic en "Execute".

Si todo funciona, recibirás un JSON de vuelta con la `transcription`, `response_text` y un `audio_base64`.

---

## 🧠 Arquitectura de Prompts (¡Importante!)

El "guion" de la IA se genera de dos maneras (el `context_json` siempre tiene prioridad).

### 1. Método Estático (context_id)

* **Qué es:** Personalidades completas pre-escritas.
* **Cómo funciona:** El frontend envía un `context_id` (ej: `"padre_hijo_8_anios"`).
* **Dónde se edita:** En el diccionario `STATIC_PROMPTS` dentro de `llama_module.py`.

### 2. Método Dinámico (context_json)

* **Qué es:** El método preferido. Se construye una personalidad fusionando "rasgos".
* **Cómo funciona:** El frontend envía un **string JSON** con una lista de "claves de rasgos". (ej. `'["rol_padre", "emocion_cansado"]'`).
* **Dónde se edita:** Todos los bloques de construcción (rasgos) están en el diccionario `PROMPT_TRAITS` dentro de `llama_module.py`. Puedes añadir nuevos rasgos aquí y el frontend podrá usarlos sin que tengas que cambiar el código de la API.

---

## ⚠️ Guía de Solución de Problemas (Lo que nos pasó)

Si algo falla, revisa esta lista.

| Error (En la terminal) | Causa del Problema | Solución |
| :--- | :--- | :--- |
| `Microsoft Visual C++ 14.0 or greater is required` | Faltan las herramientas de compilación de C++ para `TTS`. | Instala **"Desarrollo de escritorio con C++"** desde el Visual Studio Installer y reinicia. Si falla, ejecuta `pip install` en la **"Developer Command Prompt"**. |
| `[WinError 2] El sistema no puede encontrar el archivo especificado` | `Whisper` no puede encontrar `ffmpeg.exe` para leer tu `.mp3`. | Instala FFmpeg con `winget install Gyan.FFmpeg` y reinicia tu terminal. |
| `ERROR ... 401 Client Error` (Unauthorized) | No has iniciado sesión en Hugging Face. | Ejecuta `huggingface-cli login` en tu terminal (con el venv activado) y pega tu token. |
| `ERROR ... 403 Client Error ... not in the authorized list` | Iniciaste sesión, pero tu *cuenta* no tiene acceso al modelo. | Fuiste a la página del modelo LLaMA en Hugging Face y tu solicitud de acceso estaba "pendiente". |
| `ERROR ... 403 Forbidden: Please enable access to public gated...` | Tu *cuenta* tiene acceso, pero tu *token* no tiene permiso. | Ve a `Hugging Face > Settings > Access Tokens > Edit`. Marca la casilla **"Access public gated repositories"** y guarda. |
| `Segmentation fault` (Crash al iniciar) | Estás cargando un modelo (ej. `Mistral-7B`) que es demasiado grande para tu RAM/CPU. | Cambia el modelo en `llama_module.py` por uno más pequeño, como `meta-llama/Llama-3.2-1B-Instruct`. |
| `ERROR: No matching distribution found for TTS==0.22.0` | Estás usando una versión de Python incorrecta (ej. 3.12). | Desinstala Python 3.12. Instala **Python 3.11** y crea un nuevo `venv` con `py -3.11 -m venv venv`. |
| `AttributeError: module 'pkgutil' has no attribute 'ImpImporter'` | Similar al anterior, error de incompatibilidad de Python 3.12. | El proyecto **debe** usar Python 3.11. |
| `ERROR: Cannot import 'setuptools.build_meta'` | Tu `pip` y `setuptools` están desactualizados. | Ejecuta `pip install --upgrade pip setuptools wheel`. |