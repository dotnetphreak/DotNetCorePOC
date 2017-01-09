﻿using DotNetCorePOC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCorePOC.Persistence
{
    public class DotNetCorePOCDbContext : DbContext
    {
        //these options are set in startup
        public DotNetCorePOCDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Restaurant> Restaurants { get; set; }
    }
}
