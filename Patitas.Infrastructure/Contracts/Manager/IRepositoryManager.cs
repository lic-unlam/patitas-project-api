﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patitas.Infrastructure.Contracts.Manager
{
    public interface IRepositoryManager
    {
        IUsuarioRepository UsuarioRepository { get; }
        IAdoptanteRepository AdoptanteRepository { get; }
        IRefugioRepository RefugioRepository { get; }
        IVeterinariaRepository VeterinariaRepository { get; }
        IBarrioRepository BarrioRepository { get; }
        IRolRepository RolRepository { get; }
        IComentarioRepository ComentarioRepository { get; }
        IAnimalRepository AnimalRepository { get; }
        IDetalleEstrellaRepository DetalleEstrellaRepository { get; }
        ISolicitudDeAdopcionRepository SolicitudDeAdopcionRepository { get; }
        IRazaRepository RazaRepository { get; }
        IFormularioPreAdopcionRepository FormularioPreAdopcionRepository { get; }
        ITurnoRepository TurnoRepository { get; }
        ICancelacionDeAdopcionRepository CancelacionDeAdopcionRepository { get; }
        ISeguimientoRepository SeguimientoRepository { get; }
        IPlanDeVacunacionRepository PlanDeVacunacionRepository { get; }
        IEspecieRepository EspecieRepository { get; }
        IVacunaRepository VacunaRepository { get; }
        IVacunaDelPlanRepository VacunaDelPlanRepository { get; }
    }
}
