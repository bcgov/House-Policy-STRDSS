using DataBase.Entities;
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
                case "CEU ADMIN":
                    {
                        _UserType = SFEnums.UserTypeEnum.CEUADMIN;
                        break;
                    }
                case "CEU STAFF":
                    {
                        _UserType = SFEnums.UserTypeEnum.CEUSTAFF;
                        break;
                    }
                case "BC GOVERNMENT STAFF":
                    {
                        _UserType = SFEnums.UserTypeEnum.BCGOVERNMENT;
                        break;
                    }
                case "LOCAL GOVERNMENT":
                    {
                        _UserType = SFEnums.UserTypeEnum.LG;
                        break;
                    }
                case "SHORT-TERM RENTAL PLATFORM":
                    {
                        _UserType = SFEnums.UserTypeEnum.PLATFORM;
                        break;
                    }
                default:
                    throw new ArgumentException("Unknown User Type (" + UserType + ")");
            }
            return (_UserType);
        }

        public DssUserRole GetRole(DssDbContext DBContext, string RoleName)
        {
            DssUserRole dssUserRole = DBContext.DssUserRoles.FirstOrDefault(p => p.UserRoleNm == RoleName);

            return (dssUserRole);            
        }
    }
}
