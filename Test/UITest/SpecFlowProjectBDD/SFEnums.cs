using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlowProjectBDD
{
    public static class SFEnums
    {
        public enum UserTypeEnum { CEUSTAFF, CEUADMIN, LOCALGOVERNMENT, BCGOVERNMENTSTAFF, SHORTTERMRENTALPLATFORM }
        public enum LogonTypeEnum { IDIR, BCID }
        public enum Environment { LOCAL,DEV,TEST,UAT,PROD}
    }
}
