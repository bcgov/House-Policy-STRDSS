using Microsoft.EntityFrameworkCore;
using DataBase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase.Entities;

namespace DataBase.UnitOfWork
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        DbContext _context;
        bool _disposed;

        //public GenericRepository<DssAccessRequestStatus> DssAccessRequestStatus { get; }
        //public GenericRepository<DssDbContext> DssDbContext { get; }
        //public GenericRepository<DssEmailMessage> DssEmailMessage { get; }
        //public GenericRepository<DssEmailMessageType> DssEmailMessageType { get; }
        //public GenericRepository<DssMessageReason> DssMessageReason { get; }
        //public GenericRepository<DssOrganization> DssOrganization { get; }
        //public GenericRepository<DssOrganizationContactPerson> DssOrganizationContactPerson { get; }
        //public GenericRepository<DssOrganizationType> DssOrganizationType { get; }
        //public GenericRepository<DssUserIdentity> DssUserIdentity { get; }
        //public GenericRepository<DssUserIdentityView> DssUserIdentityView { get; }
        //public GenericRepository<DssUserPrivilege> DssUserPrivilege { get; }
        //GenericRepository<DssUserRole> DssUserRole { get; }

        private readonly GenericRepository<DssAccessRequestStatus> _DssAccessRequestStatusRepository;
        private readonly GenericRepository<DssEmailMessage> _DssEmailMessageRepository;
        private readonly GenericRepository<DssEmailMessageType> _DssEmailMessageTypeRepository;
        private readonly GenericRepository<DssMessageReason> _DssMessageReasonRepository;
        private readonly GenericRepository<DssOrganization> _DssOrganizationRepository;
        private readonly GenericRepository<DssOrganizationContactPerson> _DssOrganizationContactPersonRepository;
        private readonly GenericRepository<DssOrganizationType> _DssOrganizationTypeRepository;
        private readonly GenericRepository<DssUserIdentity> _DssUserIdentityRepository;
        private readonly GenericRepository<DssUserIdentityView> _DssUserIdentityViewRepository;
        private readonly GenericRepository<DssUserPrivilege> _DssUserPrivilegeRepository;
        //private readonly GenericRepository<DssMessageReason> _DssMessageReasonRepository;
        private readonly GenericRepository<DssUserRole> _DssUserRoleRepository;


        public GenericRepository<DssAccessRequestStatus> DssAccessRequestStatusRepository
        {
            get => _DssAccessRequestStatusRepository ?? new GenericRepository<DssAccessRequestStatus>(_context);
        }

        public GenericRepository<DssEmailMessage> DssEmailMessageRepository
        {
            get => _DssEmailMessageRepository ?? new GenericRepository<DssEmailMessage>(_context);
        }

        public GenericRepository<DssEmailMessageType> DssEmailMessageTypeRepository
        {
            get => _DssEmailMessageTypeRepository ?? new GenericRepository<DssEmailMessageType>(_context);
        }

        public GenericRepository<DssMessageReason> DssMessageReasonRepository
        {
            get => _DssMessageReasonRepository ?? new GenericRepository<DssMessageReason>(_context);
        }

        public GenericRepository<DssOrganization> DssOrganizationRepository
        {
            get => _DssOrganizationRepository ?? new GenericRepository<DssOrganization>(_context);
        }

        public GenericRepository<DssOrganizationContactPerson> DssOrganizationContactPersonRepository
        {
            get => _DssOrganizationContactPersonRepository ?? new GenericRepository<DssOrganizationContactPerson>(_context);
        }
        public GenericRepository<DssOrganizationType> DssOrganizationTypeRepository
        {
            get => _DssOrganizationTypeRepository ?? new GenericRepository<DssOrganizationType>(_context);
        }

        public GenericRepository<DssUserIdentity> DssUserIdentityRepository
        {
            get => _DssUserIdentityRepository ?? new GenericRepository<DssUserIdentity>(_context);
        }

        public GenericRepository<DssUserPrivilege> DssUserPrivilegeRepository
        {
            get => _DssUserPrivilegeRepository ?? new GenericRepository<DssUserPrivilege>(_context);
        }
        public GenericRepository<DssUserIdentityView> DssUserIdentityViewRepository
        {
            get => _DssUserIdentityViewRepository ?? new GenericRepository<DssUserIdentityView>(_context);
        }
        public GenericRepository<DssUserRole> DssUserRoleRepository
        {
            get => _DssUserRoleRepository ?? new GenericRepository<DssUserRole>(_context);
        }


        public UnitOfWork(DbContext context)
        {
            if (context != null)
            {
                _context = context;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) _context.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
