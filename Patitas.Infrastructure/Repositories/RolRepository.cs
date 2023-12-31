﻿using Microsoft.EntityFrameworkCore;
using Patitas.Domain.Entities;
using Patitas.Infrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patitas.Infrastructure.Repositories
{
    internal class RolRepository : Repository<Rol, int>, IRolRepository
    {
        private readonly PatitasContext _context;

        public RolRepository(PatitasContext context) : base(context)
        {
            _context = context;
        }
    }
}
