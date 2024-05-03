using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpecFlowProjectBDD.SFEnums;

namespace SpecFlowProjectBDD.Helpers
{
    public class UserHelper
    {
        public UserTypeEnum SetUserType(string UserType)
        {
            UserTypeEnum _UserType;
            switch (UserType.ToUpper())
            {                
                case "CODEENFOREMENTSTAFF":
                case "CODEENFORCEMENTADMIN":
                case "LOCALGOVERNMENTUSER":
                    {
                        _UserType = SFEnums.UserTypeEnum.BCGOVERNMENT;
                        break;
                    }
                case "PLATFORMUSER":
                    {
                        _UserType = SFEnums.UserTypeEnum.BCGOVERNMENT;
                        break;
                    }
                default:
                    throw new ArgumentException("Unknown User Type (" + UserType + ")");
            }
            return (_UserType);
        }
    }
}
