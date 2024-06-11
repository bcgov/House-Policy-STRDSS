using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore;
using Configuration;
using System.Runtime.CompilerServices;

namespace SpecFlowProjectBDD.Helpers
{
    public class DataBaseHelper
    {
        private DssDbContext _DssDBContext;
        public DataBaseHelper()
        {
            DbContextOptions<DssDbContext> dbContextOptions = new DbContextOptions<DssDbContext>();

            _DssDBContext = new DssDbContext(dbContextOptions);
        }

        public List<DssUserIdentity> GetIdentities()
        {
            return (_DssDBContext.DssUserIdentities.ToList());
        }

        public DssUserIdentity? GetIdentity(string email)
        {
            if (null == email)
            {
                throw new ArgumentNullException("email");
            }
            var identity = _DssDBContext.DssUserIdentities.Where(p => p.EmailAddressDsc == email).FirstOrDefault<DssUserIdentity>();

            return (identity);
        }
    }
}
