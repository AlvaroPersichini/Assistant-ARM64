using System;
using System.Device.Gpio;
using System.Runtime.InteropServices;
using BlazorApp1.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1
{
    public class LedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public bool EstaEncendido { get; private set; } = false;
        public event Action? OnChange;
        private GpioController? _gpio;
        private const int PIN = 18;

        public LedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            Console.WriteLine("--- INICIANDO LEDSERVICE ---"); // DEBUG

            RecuperarUltimoEstado();

            // Solo intenta iniciar hardware si estamos en Linux (Raspberry Pi real)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                InicializarHardware();
            }
            else
            {
                Console.WriteLine("AVISO: Estás en Windows. El hardware GPIO está desactivado.");
            }
        }

        private void RecuperarUltimoEstado()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // DEBUG: Contar cuántos registros hay
                    var total = db.Registros.Count();
                    Console.WriteLine($"Registros encontrados en DB: {total}");

                    // Buscamos el último
                    var ultimoRegistro = db.Registros
                        .OrderByDescending(r => r.Fecha)
                        .FirstOrDefault();

                    if (ultimoRegistro != null)
                    {
                        EstaEncendido = ultimoRegistro.Estado;
                        Console.WriteLine($"[RECUPERADO] Último estado: {(EstaEncendido ? "ON" : "OFF")} - Fecha: {ultimoRegistro.Fecha}");
                    }
                    else
                    {
                        Console.WriteLine("[INFO] No hay historial previo. Iniciando en OFF.");
                        EstaEncendido = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR CRÍTICO] Falló al leer DB: {ex.Message}");
            }
        }

        private void InicializarHardware()
        {
            try
            {
                Console.WriteLine($"[HARDWARE] Configurando GPIO 18 en estado: {EstaEncendido}");
                _gpio = new GpioController();
                _gpio.OpenPin(PIN, PinMode.Output);
                _gpio.Write(PIN, EstaEncendido ? PinValue.High : PinValue.Low);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Hardware: {ex.Message}");
            }
        }

        public async Task Alternar()
        {
            EstaEncendido = !EstaEncendido;
            Console.WriteLine($"[ACCION] Nuevo estado: {EstaEncendido}");

            // Hardware (solo si es Linux)
            if (_gpio != null)
            {
                _gpio.Write(PIN, EstaEncendido ? PinValue.High : PinValue.Low);
            }

            // DB
            await GuardarEnBaseDeDatos(EstaEncendido);

            NotifyStateChanged();
        }

        private async Task GuardarEnBaseDeDatos(bool estado)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var registro = new RegistroLed
                    {
                        Fecha = DateTime.Now,
                        Estado = estado
                    };
                    db.Registros.Add(registro);
                    await db.SaveChangesAsync();
                    Console.WriteLine("[DB] Guardado exitoso.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando en DB: {ex.Message}");
            }
        }

        public async Task<List<RegistroLed>> ObtenerHistorial()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                return await db.Registros
                    .OrderByDescending(r => r.Fecha)
                    .Take(10)
                    .ToListAsync();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}