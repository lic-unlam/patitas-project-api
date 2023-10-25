﻿using Microsoft.AspNetCore.Server.IIS.Core;
using Patitas.Domain.Entities;
using Patitas.Infrastructure.Contracts.Manager;
using Patitas.Infrastructure.Enums;
using Patitas.Services.Contracts;
using Patitas.Services.DTO.Refugio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public async Task<ExplorarRefugiosDTO> ExplorarRefugios()
        {
            try
            {
                // Obtengo los refugios
                IEnumerable<Refugio> refugios = await _repositoryManager.RefugioRepository.GetAllAsync();

                // Inicializo el ExplorarRefugiosDTO a devolver a la UI
                ExplorarRefugiosDTO explorarRefugios = new ExplorarRefugiosDTO();

                // Inicializo las propiedades que va a devolver ExplorarRefugiosDTO
                List<string> barriosDTO = new List<string>();
                List<RefugioDTO> refugiosDTO = new List<RefugioDTO>();

                IEnumerable<Barrio> barrios = await _repositoryManager.BarrioRepository.GetAllAsync();

                foreach (Barrio b in barrios)
                {
                    barriosDTO.Add(b.Nombre);
                }

                foreach (Refugio refugio in refugios)
                {
                    // Obtengo la tabla Usuario que corresponde al Refugio y carga el Barrio que le pertenece
                    Usuario? usuario = await _repositoryManager.UsuarioRepository.GetByIdAsync(refugio.Id, IncludeTypes.REFERENCE_TABLE_NAME, "Barrio");
                    
                    // Obtengo todos los comentarios hechos hacia este refugio
                    IEnumerable<Comentario?> comentarios = await _repositoryManager.ComentarioRepository.FindAllByAsync(c => c.Id_Refugio.Equals(refugio.Id));
                    double puntaje = 0;

                    if (comentarios != null)
                    {
                        puntaje = comentarios.Count() > 0 ? comentarios.Average(c => c!.Nro_Estrellas) : puntaje;
                    }

                    RefugioDTO refugioDTO = new RefugioDTO();
                    refugioDTO.Id = refugio.Id;
                    refugioDTO.Nombre = refugio.Nombre;
                    refugioDTO.Puntaje = puntaje;
                    refugioDTO.Fotografia = refugio.Fotografia;
                    refugioDTO.Direccion = usuario!.Direccion!;
                    refugioDTO.Barrio = usuario.Barrio.Nombre;

                    refugiosDTO.Add(refugioDTO);
                }

                explorarRefugios.Barrios = barriosDTO;
                explorarRefugios.Refugios = refugiosDTO;

                return explorarRefugios;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Ha ocurrido un problema al obtener los datos de los refugios. \nCausa: " + ex.Message);
            }
        }

        public async Task<IEnumerable<RefugioDTO>> BuscarRefugios(string? nombre, string? barrio)
        {
            IEnumerable<Refugio> refugios;

            if (!string.IsNullOrWhiteSpace(nombre))
                // Obtengo los refugios que contienen la palabra que busqué
                refugios = await _repositoryManager.RefugioRepository.FindAllByAsync(r => r.Nombre.Contains(nombre.Trim()));
            else
                // Obtengo todos los refugios si el query string 'nombre' se envió vacio
                refugios = await _repositoryManager.RefugioRepository.GetAllAsync();

            List<RefugioDTO> refugiosDTO = new List<RefugioDTO>();

            if(!string.IsNullOrWhiteSpace(barrio) && barrio!.ToLower() != "todos")
            {
                foreach(Refugio refugio in refugios)
                {
                    // Obtengo la tabla Usuario que corresponde al Refugio y carga el Barrio que le pertenece
                    Usuario? usuario = await _repositoryManager.UsuarioRepository.GetByIdAsync(refugio.Id, IncludeTypes.REFERENCE_TABLE_NAME, "Barrio");
                    barrio = barrio.Trim().ToLower();

                    if (usuario!.Barrio.Nombre.ToLower().Contains(barrio))
                    {
                        // Obtengo todos los comentarios hechos hacia este refugio
                        IEnumerable<Comentario?> comentarios = await _repositoryManager.ComentarioRepository.FindAllByAsync(c => c.Id_Refugio.Equals(refugio.Id));
                        double puntaje = 0;

                        if (comentarios != null)
                            puntaje = comentarios.Count() > 0 ? comentarios.Average(c => c!.Nro_Estrellas) : puntaje;

                        refugiosDTO.Add(new RefugioDTO()
                        {
                            Id = refugio.Id,
                            Nombre = refugio.Nombre,
                            Fotografia = refugio.Fotografia,
                            Puntaje = puntaje,
                            Direccion = usuario.Direccion!,
                            Barrio = usuario.Barrio.Nombre
                        });
                    }
                }
            }
            else
            {
                foreach(Refugio refugio in refugios)
                {
                    // Obtengo la tabla Usuario que corresponde al Refugio y carga el Barrio que le pertenece
                    Usuario? usuario = await _repositoryManager.UsuarioRepository.GetByIdAsync(refugio.Id, IncludeTypes.REFERENCE_TABLE_NAME, "Barrio");

                    // Obtengo todos los comentarios hechos hacia este refugio
                    IEnumerable<Comentario?> comentarios = await _repositoryManager.ComentarioRepository.FindAllByAsync(c => c.Id_Refugio.Equals(refugio.Id));
                    double puntaje = 0;

                    if (comentarios != null)
                        puntaje = comentarios.Count() > 0 ? comentarios.Average(c => c!.Nro_Estrellas) : puntaje;

                    refugiosDTO.Add(new RefugioDTO()
                    {
                        Id = refugio.Id,
                        Nombre = refugio.Nombre,
                        Fotografia = refugio.Fotografia,
                        Puntaje = puntaje,
                        Direccion = usuario!.Direccion!,
                        Barrio = usuario.Barrio.Nombre
                    });
                }
            }

            return refugiosDTO;
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

        public async Task<RefugioResponseDTO> GetAnimalesDelRefugio(int refugioId, ClaimsIdentity? identity)
        {
            IEnumerable<Animal> animalesDelRefugio = await _repositoryManager.AnimalRepository.FindAllByAsync(a => a.Id_Refugio.Equals(refugioId));
            List<AnimalDelRefugioDTO> animalesDelRefugioDTO = new List<AnimalDelRefugioDTO>();
            List<string> vacunasAplicadas = new List<string>();
            int adoptanteId = 0;
            bool solicitudActiva = false;

            if (identity != null && identity.IsAuthenticated)
                adoptanteId = Convert.ToInt32(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            foreach (Animal animal in animalesDelRefugio)
                if (!animal.EstaAdoptado)
                {
                    Raza? raza = await _repositoryManager.RazaRepository.GetByIdAsync(animal.Id_Raza);
                    Animal? animalConVacunas = await _repositoryManager.AnimalRepository.GetByIdAsync(animal.Id, IncludeTypes.COLLECTION_TABLE_NAME, "Vacunas");

                    if(animalConVacunas is not null)
                        foreach(var vacuna in animalConVacunas.Vacunas)
                            vacunasAplicadas.Add(vacuna.Nombre);

                    if(adoptanteId > 0)
                        solicitudActiva = await _repositoryManager.SolicitudDeAdopcionRepository
                                                                    .ExistsAsync(s => s.EstaActivo == true
                                                                            && s.Id_Adoptante.Equals(adoptanteId)
                                                                            && s.Id_Animal.Equals(animal.Id));

                    animalesDelRefugioDTO.Add(
                        new AnimalDelRefugioDTO()
                        {
                            Id = animal.Id,
                            Nombre = animal.Nombre,
                            Raza = raza!.Nombre,
                            Nacimiento = animal.Nacimiento,
                            Genero = animal.Genero,
                            Fotografia = animal.Fotografia,
                            SituacionPrevia = animal.SituacionPrevia,
                            Peso = animal.Peso,
                            Altura = animal.Altura,
                            Esterilizado = animal.Esterilizado,
                            Desparasitado = animal.Desparasitado,
                            FechaIngreso = animal.FechaIngreso,
                            Vacunas = vacunasAplicadas,
                            DescripcionAdicional = animal.DescripcionAdicional,
                            EstaAdoptado = animal.EstaAdoptado,
                            SolicitudActiva = solicitudActiva,
                            FechaAdopcion = animal.FechaAdopcion,
                            Id_Raza = animal.Id_Raza,
                            Id_Refugio = animal.Id_Refugio,
                        }
                    );
                }

            return new RefugioResponseDTO()
            {
                InfoBasica = await this.GetInformacionBasicaDelRefugio(refugioId),
                Animales = animalesDelRefugioDTO
            };
        }

        public async Task<RefugioResponseDTO> GetComentariosDelRefugio(int refugioId, ClaimsIdentity? identity)
        {
            IEnumerable<Comentario> comentarios = await _repositoryManager.ComentarioRepository.FindAllByAsync(c => c.Id_Refugio.Equals(refugioId));
            List<ComentarioDelRefugioDTO> comentariosDTO = new List<ComentarioDelRefugioDTO>();

            foreach (var comentario in comentarios)
                if (comentario.EstaActivo)
                {
                    Usuario? usuario = await _repositoryManager.UsuarioRepository.FindByAsync(u => u.Id.Equals(comentario.Id_Adoptante));
                    DetalleEstrella? detalleEstrella = await _repositoryManager.DetalleEstrellaRepository.GetByIdAsync(comentario.Nro_Estrellas);

                    comentariosDTO.Add(
                        new ComentarioDelRefugioDTO()
                        {
                            Id = comentario.Id,
                            Descripcion = comentario.Descripcion,
                            FechaCreacion = comentario.FechaCreacion,
                            EstaActivo = comentario.EstaActivo,
                            FechaEdicion = comentario.FechaEdicion,
                            Nro_Estrellas = comentario.Nro_Estrellas,
                            DescripcionEstrella = detalleEstrella!.Descripcion,
                            Id_Refugio = comentario.Id_Refugio,
                            Id_Adoptante = comentario.Id_Adoptante,
                            NombreDeUsuario = usuario!.NombreUsuario,
                            FotoDePerfil = usuario.FotoDePerfil
                        }
                    );
                }

            return new RefugioResponseDTO()
            {
                InfoBasica = await this.GetInformacionBasicaDelRefugio(refugioId),
                Comentarios = comentariosDTO,
                PuedeComentar = await _repositoryManager.SolicitudDeAdopcionRepository.AdoptantePuedeComentar(identity)
            };
        }

        public async Task<RefugioResponseDTO> GetVeterinariasAsociadas(int refugioId)
        {
            try
            {
                Refugio? refugio = await _repositoryManager.RefugioRepository.GetByIdAsync(refugioId, IncludeTypes.COLLECTION_TABLE_NAME, "Veterinarias");
                List<VeterinariaAsociadaDTO> veterinariasAsociadas = new List<VeterinariaAsociadaDTO>();

                foreach (Veterinaria veterinaria in refugio!.Veterinarias)
                {
                    Usuario? usuario = await _repositoryManager.UsuarioRepository.GetByIdAsync(veterinaria.Id, IncludeTypes.REFERENCE_TABLE_NAME, "Barrio");

                    veterinariasAsociadas.Add(
                        new VeterinariaAsociadaDTO()
                        {
                            Id = veterinaria.Id,
                            Nombre = veterinaria.Nombre,
                            RazonSocial = veterinaria.RazonSocial,
                            Direccion = usuario!.Direccion!,
                            Barrio = usuario.Barrio.Nombre,
                            Fotografia = veterinaria.Fotografia,
                            Especialidades = veterinaria.Especialidades,
                            FechaFundacion = veterinaria.FechaFundacion,
                            Telefono = usuario.Telefono!,
                            TelefonoAlternativo = veterinaria.TelefonoAlternativo,
                            Email = usuario.Email,
                            SitioWeb = veterinaria.SitioWeb,
                            Descripcion = veterinaria.Descripcion,
                            DiasDeAtencion = veterinaria.DiasDeAtencion,
                            HorarioApertura = veterinaria.HorarioApertura,
                            HorarioCierre = veterinaria.HorarioCierre
                        }
                    );
                }

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

                // Obtengo la tabla Usuario que corresponde al Refugio y carga el Barrio que le pertenece
                Usuario? usuario = await _repositoryManager.UsuarioRepository.GetByIdAsync(refugioId);

                RefugioInfoCompletaDTO infoCompleta = new RefugioInfoCompletaDTO()
                {
                    Id = refugio!.Id,
                    Nombre = refugio.Nombre,
                    RazonSocial = refugio.RazonSocial,
                    Fotografia = refugio.Fotografia,
                    NombreResponsable = refugio.NombreResponsable,
                    ApellidoResponsable = refugio.ApellidoResponsable,
                    Telefono = usuario!.Telefono,
                    Email = usuario.Email,
                    SitioWeb = refugio.SitioWeb,
                    Descripcion = refugio.Descripcion,
                    DiasDeAtencion = refugio.DiasDeAtencion,
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
