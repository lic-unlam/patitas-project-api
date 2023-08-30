﻿using Microsoft.AspNetCore.Server.IIS.Core;
using Patitas.Domain.Entities;
using Patitas.Infrastructure.Contracts.Manager;
using Patitas.Infrastructure.Enums;
using Patitas.Services.Contracts;
using Patitas.Services.DTO.Refugio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patitas.Services
{
    internal class RefugioService : IRefugioService
    {
        private readonly IRepositoryManager _repositoryManager;

        public RefugioService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<RefugioInfoBasicaDTO> GetInformacionBasicaDelRefugio(int refugioId)
        {
            try
            {
                // Obtengo el refugio
                Refugio? refugio = await _repositoryManager.RefugioRepository.GetByIdAsync(refugioId);

                // Obtengo la tabla Usuario que corresponde al Refugio y carga el Barrio que le pertenece
                Usuario? usuario = await _repositoryManager.UsuarioRepository.GetByIdAsync(refugioId, IncludeTypes.REFERENCE_TABLE_NAME, "Barrio");

                // Obtengo todos los comentarios hechos hacia este refugio
                IEnumerable<Comentario?> comentarios = await _repositoryManager.ComentarioRepository.FindAllByAsync(c => c.Id_Refugio.Equals(refugioId));
                
                double puntaje = 0;
                int cantidadDeComentarios = 0;

                if (comentarios != null)
                {
                    cantidadDeComentarios = comentarios.Count();
                    puntaje = cantidadDeComentarios > 0 ? comentarios.Average(c => c!.Nro_Estrellas) : cantidadDeComentarios;
                }

                return new RefugioInfoBasicaDTO()
                {
                    Nombre = refugio!.Nombre,
                    Direccion = usuario!.Direccion!,
                    Barrio = usuario.Barrio.Nombre,
                    Puntaje = puntaje,
                    CantidadDeComentarios = cantidadDeComentarios
                };
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Ha ocurrido un problema al obtener los datos del refugio. \nCausa: " + ex.Message);
            }
        }

        public async Task<RefugioResponseDTO> GetAnimalesDelRefugio(int refugioId)
        {
            IEnumerable<Animal> animalesDelRefugio = await _repositoryManager.AnimalRepository.FindAllByAsync(a => a.Id_Refugio.Equals(refugioId));
            List<AnimalDelRefugioDTO> animalesDelRefugioDTO = new List<AnimalDelRefugioDTO>();

            foreach (Animal animal in animalesDelRefugio)
                if(!animal.EstaAdoptado)
                    animalesDelRefugioDTO.Add(
                        new AnimalDelRefugioDTO()
                        {
                            Id = animal.Id,
                            Nombre = animal.Nombre,
                            Nacimiento = animal.Nacimiento,
                            Genero = animal.Genero,
                            SituacionPrevia = animal.SituacionPrevia,
                            Altura = animal.Altura,
                            Esterilizado = animal.Esterilizado,
                            Desparasitado = animal.Desparasitado,
                            FechaIngreso = animal.FechaIngreso,
                            DescripcionAdicional = animal.DescripcionAdicional,
                            EstaAdoptado = animal.EstaAdoptado,
                            FechaAdopcion = animal.FechaAdopcion,
                            Id_Raza = animal.Id_Raza,
                            Id_Refugio = animal.Id_Refugio,
                        }
                    );

            return new RefugioResponseDTO()
            {
                InfoBasica = await this.GetInformacionBasicaDelRefugio(refugioId),
                Animales = animalesDelRefugioDTO
            };
        }

        public async Task<RefugioResponseDTO> GetComentariosDelRefugio(int refugioId)
        {
            IEnumerable<Comentario> comentarios = await _repositoryManager.ComentarioRepository.FindAllByAsync(c => c.Id_Refugio.Equals(refugioId));
            List<ComentarioDelRefugioDTO> comentariosDTO = new List<ComentarioDelRefugioDTO>();

            foreach (var comentario in comentarios)
                if (!comentario.PendienteDeAprobacion && comentario.EstaActivo)
                    comentariosDTO.Add(
                        new ComentarioDelRefugioDTO()
                        {
                            Id = comentario.Id,
                            Descripcion = comentario.Descripcion,
                            FechaCreacion = comentario.FechaCreacion,
                            EstaActivo = comentario.EstaActivo,
                            FechaEdicion = comentario.FechaEdicion,
                            PendienteDeAprobacion = comentario.PendienteDeAprobacion,
                            AprobadoPor_IdAdministrador = comentario.AprobadoPor_IdAdministrador,
                            Nro_Estrellas = comentario.Nro_Estrellas,
                            Id_Refugio = comentario.Id_Refugio,
                            Id_Adoptante = comentario.Id_Adoptante
                        }
                    );

            return new RefugioResponseDTO()
            {
                InfoBasica = await this.GetInformacionBasicaDelRefugio(refugioId),
                Comentarios = comentariosDTO
            };
        }

        public async Task<RefugioResponseDTO> GetVeterinariasAsociadas(int refugioId)
        {
            try
            {
                Refugio? refugio = await _repositoryManager.RefugioRepository.GetByIdAsync(refugioId, IncludeTypes.COLLECTION_TABLE_NAME, "Veterinarias");
                List<VeterinariaAsociadaDTO> veterinariasAsociadas = new List<VeterinariaAsociadaDTO>();

                foreach (Veterinaria veterinaria in refugio!.Veterinarias)
                    veterinariasAsociadas.Add(
                        new VeterinariaAsociadaDTO()
                        {
                            Id = veterinaria.Id,
                            Nombre = veterinaria.Nombre,
                            RazonSocial = veterinaria.RazonSocial,
                            Especialidades = veterinaria.Especialidades,
                            FechaFundacion = veterinaria.FechaFundacion,
                            TelefonoAlternativo = veterinaria.TelefonoAlternativo,
                            SitioWeb = veterinaria.SitioWeb,
                            HorarioApertura = veterinaria.HorarioApertura,
                            HorarioCierre = veterinaria.HorarioCierre
                        }
                    );

                return new RefugioResponseDTO()
                {
                    InfoBasica = await this.GetInformacionBasicaDelRefugio(refugioId),
                    VeterinariasAsociadas = veterinariasAsociadas
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException("No se pudieron cargar las veterinarias. \nCausa: ", ex.Message);
            }
        }

        public async Task<RefugioResponseDTO> GetInformacionCompleta(int refugioId)
        {
            try
            {
                Refugio? refugio = await _repositoryManager.RefugioRepository.GetByIdAsync(refugioId);

                RefugioInfoCompletaDTO infoCompleta = new RefugioInfoCompletaDTO()
                {
                    Id = refugio!.Id,
                    Nombre = refugio.Nombre,
                    RazonSocial = refugio.RazonSocial,
                    NombreResponsable = refugio.NombreResponsable,
                    ApellidoResponsable = refugio.ApellidoResponsable,
                    SitioWeb = refugio.SitioWeb,
                    HorarioApertura = refugio.HorarioApertura,
                    HorarioCierre = refugio.HorarioCierre
                };

                return new RefugioResponseDTO()
                {
                    InfoBasica = await this.GetInformacionBasicaDelRefugio(refugioId),
                    InfoCompleta = infoCompleta
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException("No se pudo cargar la información del refugio. \nCausa: " + ex.Message);
            }
        }
    }
}