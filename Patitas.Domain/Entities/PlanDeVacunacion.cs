﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patitas.Domain.Entities
{
    public class PlanDeVacunacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Id_SolicitudDeAdopcion { get; set; }
        public int Id_Veterinaria { get; set; }
        public bool EstaActivo { get; set; }

        // 1 solicitud de adopcion <--> 1 plan de vacunacion
        [ForeignKey(nameof(Id_SolicitudDeAdopcion))]
        public SolicitudDeAdopcion SolicitudDeAdopcion { get; set; } = null!;

        // 1 veterinaria <--> 1 plan de vacunacion
        [ForeignKey(nameof(Id_Veterinaria))]
        public Veterinaria Veterinaria { get; set; } = null!;

        // N a N
        // N vacunas <--(VacunaDelPlan)--> N plan de vacunación
        public ICollection<Vacuna> Vacunas { get; } = new List<Vacuna>();
    }
}
