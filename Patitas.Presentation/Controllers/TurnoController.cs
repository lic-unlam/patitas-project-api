﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Patitas.Services.Contracts.Manager;
using Patitas.Services.DTO.Turno;

namespace Patitas.Presentation.Controllers
{
    [Route("api/turnos")]
    [ApiController]
    public class TurnoController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public TurnoController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        [Authorize(Roles = "Refugio")]
        public async Task<IActionResult> CreateTurno(TurnoCreateDTO turnoDTO)
        {
            try
            {
                await _serviceManager.TurnoService.CreateTurno(HttpContext.User.Identity, turnoDTO);
                return StatusCode(201);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("adoptante")]
        [Authorize(Roles = "Adoptante")]
        public async Task<IActionResult> GetMisTurnos()
        {
            try
            {
                TurnoResponseDTO turnos = await _serviceManager.TurnoService.GetTurnos(HttpContext.User.Identity, Services.Helpers.Enums.RolTypes.ADOPTANTE);
                return Ok(turnos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("refugio")]
        [Authorize(Roles = "Refugio")]
        public async Task<IActionResult> GetTurnos()
        {
            try
            {
                TurnoResponseDTO turnos = await _serviceManager.TurnoService.GetTurnosRefugio(HttpContext.User.Identity);
                return Ok(turnos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
