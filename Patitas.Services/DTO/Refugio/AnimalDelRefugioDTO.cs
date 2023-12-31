﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patitas.Services.DTO.Refugio
{
    public class AnimalDelRefugioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Raza { get; set; } = string.Empty;
        public int Edad { get; set; }
        public char Genero { get; set; }
        public string Fotografia { get; set; } = string.Empty;
        public string SituacionPrevia { get; set; } = null!;
        public int Peso { get; set; }
        public decimal Altura { get; set; }
        public bool Esterilizado { get; set; }
        public bool Desparasitado { get; set; }
        public string FechaIngreso { get; set; } = string.Empty;
        public List<string> Vacunas { get; set; } = new List<string>();
        public string? DescripcionAdicional { get; set; }
        public bool EstaAdoptado { get; set; }
        public bool SolicitudActiva { get; set; }
        public DateTime? FechaAdopcion { get; set; }
        public int Id_Raza { get; set; }
        public int Id_Refugio { get; set; }
    }
}
