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
                case "CEU_STAFF":
                    {
                        _UserType = UserTypeEnum.CEUSTAFF;
                        break;
                    }
                case "CEU_ADMIN":
                    {
                        _UserType = UserTypeEnum.CEUADMIN;
                        break;
                    }
                case "LG_STAFF":
                    {
                        _UserType = UserTypeEnum.LOCALGOVERNMENT;
                        break;
                    }
                case "PLATFORM_STAFF":
                    {
                        _UserType = UserTypeEnum.SHORTTERMRENTALPLATFORM;
                        break;
                    }
                case "bC_STAFF":
                    {
                        _UserType = UserTypeEnum.BCGOVERNMENTSTAFF;
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

