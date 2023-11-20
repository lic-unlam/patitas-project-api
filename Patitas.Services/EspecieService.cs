﻿using Patitas.Infrastructure.Contracts.Manager;
using Patitas.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patitas.Services
{
    public class EspecieService : IEspecieService
    {
        private readonly IRepositoryManager _repositoryManager;

        public EspecieService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
    }
}
