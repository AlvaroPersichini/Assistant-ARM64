# HA-Blazor-ARM64
La tengo actualmente para manejar ilumiunacion. 
Gracias Tailscale por existir!.  
Y RaspBerry Pi

![Platform](https://img.shields.io/badge/Platform-Linux%20ARM64-orange)
![Framework](https://img.shields.io/badge/.NET-Blazor-purple)
![Context](https://img.shields.io/badge/Context-Home%20Assistant-blue)

Aplicación desarrollada en **Blazor (.NET)** diseñada específicamente para entorno **Linux con arquitectura ARM64** 


* **Arquitectura:** Compilado y optimizado para `linux-arm64`.
* **Framework:** .NET Blazor (Server/Wasm) para una interfaz web reactiva.
* **Persistencia:** Integración con base de datos (Ej: SQLite/PostgreSQL) usando Entity Framework.
* **Integración HA:** Diseñado para coexistir o conectarse con Home Assistant.


* **Lenguaje:** C#
* **Framework Web:** Blazor
* **Base de Datos:** SQLite 
* **OS Target:** Linux (Debian/Alpine)
* **Hardware Target:** Procesadores ARM64 (Aarch64)

```bash
# 1. Clonar el repositorio
git clone [https://github.com/tu-usuario/ha-blazor-arm64.git](https://github.com/tu-usuario/ha-blazor-arm64.git)

# 2. Construir la imagen
docker build -t ha-blazor-app .

# 3. Ejecutar el contenedor (Mapeando el puerto 5000)
docker run -d -p 5000:80 --name mi-app-blazor ha-blazor-app
