using FutuEvents.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace FutuEvents.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<FutuEvent> FutuEvents { get; set; }
        public DbSet<PossibleDate> PossibleDates { get; set; }
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

        }
    }
}
