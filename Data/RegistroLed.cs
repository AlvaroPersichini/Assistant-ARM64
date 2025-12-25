using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Data
{
    public class RegistroLed
    {
        [Key]
        public int Id { get; set; } // Identificador único

        public DateTime Fecha { get; set; } // Cuándo ocurrió

        public bool Estado { get; set; } // true = Prendido, false = Apagado
    }
}