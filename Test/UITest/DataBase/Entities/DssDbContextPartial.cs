using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Entities
{
    public partial class DssDbContext : DbContext
    {
        private string _ConnectionString = string.Empty;

        public DssDbContext(DbContextOptions<DssDbContext> options, string ConnectionString) : base(options)
        {
            _ConnectionString = ConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_ConnectionString);

    }
}
