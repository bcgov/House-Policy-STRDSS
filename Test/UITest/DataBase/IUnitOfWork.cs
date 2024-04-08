using DataBase.Entities;
using DataBase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public interface IUnitOfWork
    {
        GenericRepository<DssAccessRequestStatus> DssAccessRequestStatusRepository { get; }
        GenericRepository<DssEmailMessage> DssEmailMessageRepository { get; }
        GenericRepository<DssEmailMessageType> DssEmailMessageTypeRepository { get; }
        GenericRepository<DssMessageReason> DssMessageReasonRepository { get; }
        GenericRepository<DssOrganization> DssOrganizationRepository { get; }
        GenericRepository<DssOrganizationContactPerson> DssOrganizationContactPersonRepository { get; }
        GenericRepository<DssOrganizationType> DssOrganizationTypeRepository { get; }
        GenericRepository<DssUserIdentity> DssUserIdentityRepository { get; }
        GenericRepository<DssUserIdentityView> DssUserIdentityViewRepository { get; }
        GenericRepository<DssUserPrivilege> DssUserPrivilegeRepository { get; }
        GenericRepository<DssUserRole> DssUserRoleRepository { get; }

        void Dispose();
        void Save();
    }
}
