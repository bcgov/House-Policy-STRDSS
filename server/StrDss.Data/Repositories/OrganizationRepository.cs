using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Npgsql;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using System.Data;
using System.Linq.Expressions;

namespace StrDss.Data.Repositories
{
    public interface IOrganizationRepository
    {
        Task<List<LocalGovTypeDto>> GetLocalGovTypesAsync();
        Task<List<EconomicRegionDto>> GetEconomicRegionsAsync();
        Task<List<OrganizationTypeDto>> GetOrganizationTypesAsync();
        Task<List<OrganizationDto>> GetOrganizationsAsync(string? type);
        Task<List<TDto>> GetOrganizationsAsync<TDto>(string? type, Expression<Func<DssOrganization, TDto>> projection);
        Task<OrganizationDto?> GetOrganizationByIdAsync(long orgId);
        Task<List<string>> GetManagingOrgCdsAsync(long orgId);
        Task<OrganizationDto> GetOrganizationByOrgCdAsync(string orgCd);
        Task<long?> GetContainingOrganizationId(Point point);
        Task<long?> GetManagingOrgId(long orgId);
        Task<StrRequirementsDto?> GetStrRequirements(double longitude, double latitude);
        Task<PagedDto<PlatformViewDto>> GetPlatforms(int pageSize, int pageNumber, string orderBy, string direction);
        Task<PlatformViewDto?> GetPlatform(long id);
        Task<List<PlatformTypeDto>> GetPlatformTypesAsync();
        Task<DssOrganization> CreatePlatformAsync(PlatformCreateDto dto);
        Task<bool> DoesOrgCdExist(string orgCd);
        Task UpdatePlatformAsync(PlatformUpdateDto dto);
        Task<DssOrganization> CreatePlatformSubAsync(PlatformSubCreateDto dto);
        Task UpdatePlatformSubAsync(PlatformSubUpdateDto dto);
        Task<PagedDto<LocalGovViewDto>> GetLocalGovs(int pageSize, int pageNumber, string orderBy, string direction);
        Task UpdateLocalGovAsync(LocalGovUpdateDto dto);
        Task<LocalGovViewDto?> GetLocalGov(long id);
        Task<JurisdictionsViewDto?> GetJurisdiction(long id);
        Task UpdateJurisdictionAsync(JurisdictionUpdateDto dto);
    }
    public class OrganizationRepository : RepositoryBase<DssOrganization>, IOrganizationRepository
    {
        private readonly ConnectionStringProvider _connectionStringProvider;

        private static readonly Expression<Func<DssOrganization, OrganizationDto>> OrganizationDtoProjection = x => new OrganizationDto
        {
            OrganizationId = x.OrganizationId,
            OrganizationType = x.OrganizationType,
            OrganizationCd = x.OrganizationCd,
            OrganizationNm = x.OrganizationNm,
            EconomicRegionDsc = x.EconomicRegionDsc,
            LocalGovernmentType = x.LocalGovernmentType,
            ManagingOrganizationId = x.ManagingOrganizationId,
            UpdDtm = x.UpdDtm,
            UpdUserGuid = x.UpdUserGuid,
            IsLgParticipating = x.IsLgParticipating,
            IsPrincipalResidenceRequired = x.IsPrincipalResidenceRequired,
            IsBusinessLicenceRequired = x.IsBusinessLicenceRequired,
            IsStrProhibited = x.IsStrProhibited,
            IsStraaExempt = x.IsStraaExempt,
            IsActive = x.IsActive,
            PlatformType = x.PlatformType,
            BusinessLicenceFormatTxt = x.BusinessLicenceFormatTxt,
            ContactPeople = x.DssOrganizationContactPeople.Select(contact => new ContactPersonDto
            {
                OrganizationContactPersonId = contact.OrganizationContactPersonId,
                IsPrimary = contact.IsPrimary ?? false,
                GivenNm = contact.GivenNm,
                FamilyNm = contact.FamilyNm,
                PhoneNo = contact.PhoneNo,
                EmailAddressDsc = contact.EmailAddressDsc,
                ContactedThroughOrganizationId = contact.ContactedThroughOrganizationId,
                EmailMessageType = contact.EmailMessageType,
                UpdDtm = contact.UpdDtm,
                UpdUserGuid = contact.UpdUserGuid
            }).ToList()
        };

        private static readonly Expression<Func<DssOrganization, OrganizationDto>> OrganizationDtoWithoutContactsProjection = x => new OrganizationDto
        {
            OrganizationId = x.OrganizationId,
            OrganizationType = x.OrganizationType,
            OrganizationCd = x.OrganizationCd,
            OrganizationNm = x.OrganizationNm,
            EconomicRegionDsc = x.EconomicRegionDsc,
            LocalGovernmentType = x.LocalGovernmentType,
            ManagingOrganizationId = x.ManagingOrganizationId,
            UpdDtm = x.UpdDtm,
            UpdUserGuid = x.UpdUserGuid,
            IsLgParticipating = x.IsLgParticipating,
            IsPrincipalResidenceRequired = x.IsPrincipalResidenceRequired,
            IsBusinessLicenceRequired = x.IsBusinessLicenceRequired,
            IsStrProhibited = x.IsStrProhibited,
            IsStraaExempt = x.IsStraaExempt,
            IsActive = x.IsActive,
            PlatformType = x.PlatformType,
            BusinessLicenceFormatTxt = x.BusinessLicenceFormatTxt
        };

        private static readonly Expression<Func<DssPlatformVw, PlatformViewDto>> PlatformViewProjection = x => new PlatformViewDto
        {
            OrganizationId = x.OrganizationId ?? 0,
            OrganizationType = x.OrganizationType,
            OrganizationCd = x.OrganizationCd,
            OrganizationNm = x.OrganizationNm,
            UpdDtm = x.UpdDtm ?? default,
            UpdUserGuid = x.UpdUserGuid,
            UpdUserDisplayNm = x.UpdUserDisplayNm,
            UpdUserGivenNm = x.UpdUserGivenNm,
            UpdUserFamilyNm = x.UpdUserFamilyNm,
            ManagingOrganizationId = x.ManagingOrganizationId,
            PrimaryNoticeOfTakedownContactId = x.PrimaryNoticeOfTakedownContactId,
            PrimaryNoticeOfTakedownContactEmail = x.PrimaryNoticeOfTakedownContactEmail,
            PrimaryTakedownRequestContactId = x.PrimaryTakedownRequestContactId,
            PrimaryTakedownRequestContactEmail = x.PrimaryTakedownRequestContactEmail,
            SecondaryNoticeOfTakedownContactId = x.SecondaryNoticeOfTakedownContactId,
            SecondaryNoticeOfTakedownContactEmail = x.SecondaryNoticeOfTakedownContactEmail,
            SecondaryTakedownRequestContactId = x.SecondaryTakedownRequestContactId,
            SecondaryTakedownRequestContactEmail = x.SecondaryTakedownRequestContactEmail,
            IsActive = x.IsActive,
            PlatformType = x.PlatformType
        };

        private static readonly Expression<Func<DssLocalGovVw, LocalGovViewDto>> LocalGovViewProjection = x => new LocalGovViewDto
        {
            OrganizationId = x.OrganizationId ?? 0,
            OrganizationNm = x.OrganizationNm,
            OrganizationType = x.OrganizationType,
            LocalGovernmentType = x.LocalGovernmentType,
            LocalGovernmentTypeNm = x.LocalGovernmentTypeNm,
            OrganizationCd = x.OrganizationCd,
            BusinessLicenceFormatTxt = x.BusinessLicenceFormatTxt,
            UpdDtm = x.UpdDtm ?? default
        };

        private static readonly Expression<Func<DssOrganization, JurisdictionsViewDto>> JurisdictionViewProjection = x => new JurisdictionsViewDto
        {
            OrganizationId = x.OrganizationId,
            OrganizationNm = x.OrganizationNm,
            OrganizationCd = x.OrganizationCd,
            ManagingOrganizationId = x.ManagingOrganizationId,
            IsPrincipalResidenceRequired = x.IsPrincipalResidenceRequired,
            IsStrProhibited = x.IsStrProhibited,
            IsBusinessLicenceRequired = x.IsBusinessLicenceRequired,
            IsStraaExempt = x.IsStraaExempt,
            EconomicRegionDsc = x.EconomicRegionDsc,
            IsActive = x.IsActive,
            UpdDtm = x.UpdDtm
        };

        public OrganizationRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger,
            ConnectionStringProvider connectionStringProvider)
            : base(dbContext, mapper, currentUser, logger)
        {
            _connectionStringProvider = connectionStringProvider;
        }
        public async Task<List<LocalGovTypeDto>> GetLocalGovTypesAsync()
        {
            return await _dbContext.DssLocalGovernmentTypes.AsNoTracking()
                .OrderBy(x => x.LocalGovernmentTypeSortNo)
                .Select(x => new LocalGovTypeDto
                {
                    LocalGovernmentType = x.LocalGovernmentType,
                    LocalGovernmentTypeNm = x.LocalGovernmentTypeNm,
                    LocalGovernmentTypeSortNo = x.LocalGovernmentTypeSortNo
                })
                .ToListAsync();
        }

        public async Task<List<EconomicRegionDto>> GetEconomicRegionsAsync()
        {
            return await _dbContext.DssEconomicRegions.AsNoTracking()
                .OrderBy(x => x.EconomicRegionSortNo)
                .Select(x => new EconomicRegionDto
                {
                    EconomicRegionDsc = x.EconomicRegionDsc,
                    EconomicRegionNm = x.EconomicRegionNm,
                    EconomicRegionSortNo = x.EconomicRegionSortNo
                })
                .ToListAsync();
        }

        public async Task<List<OrganizationTypeDto>> GetOrganizationTypesAsync()
        {
            return await _dbContext.DssOrganizationTypes.AsNoTracking()
                .Where(x => x.OrganizationType != OrganizationTypes.LGSub)
                .Select(x => new OrganizationTypeDto
                {
                    OrganizationType = x.OrganizationType,
                    OrganizationTypeNm = x.OrganizationTypeNm
                })
                .ToListAsync();
        }

        public async Task<List<PlatformTypeDto>> GetPlatformTypesAsync()
        {
            return await _dbContext.DssPlatformTypes.AsNoTracking()
                .Select(x => new PlatformTypeDto
                {
                    PlatformTypeNm = x.PlatformTypeNm,
                    PlatformType = x.PlatformType
                })
                .ToListAsync();
        }

        public Task<List<OrganizationDto>> GetOrganizationsAsync(string? type)
        {
            return GetOrganizationsAsync(type, OrganizationDtoProjection);
        }

        public async Task<List<TDto>> GetOrganizationsAsync<TDto>(string? type, Expression<Func<DssOrganization, TDto>> projection)
        {
            var query = _dbSet.AsNoTracking();

            if (type != null && type != "All")
            {
                query = query.Where(x => x.OrganizationType == type);
            }
            else
            {
                query = query.Where(x => x.OrganizationType != OrganizationTypes.LGSub);
            }

            return await query.OrderBy(x => x.OrganizationNm)
                .Select(projection)
                .ToListAsync();
        }

        public async Task<OrganizationDto?> GetOrganizationByIdAsync(long orgId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.OrganizationId == orgId)
                .Select(OrganizationDtoProjection)
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetManagingOrgCdsAsync(long orgId)
        {
            var org = await _dbSet.AsNoTracking()
                .Where(x => x.OrganizationId == orgId)
                .Select(x => new { x.OrganizationType, x.OrganizationCd })
                .FirstOrDefaultAsync();

            if (org == null) return new List<string>();

            if (org.OrganizationType != OrganizationTypes.Platform)
                return new List<string> { org.OrganizationCd };

            var orgCds = await _dbSet.AsNoTracking()
                .Where(x => x.OrganizationId == orgId || x.ManagingOrganizationId == orgId)
                .Select(x => x.OrganizationCd)
                .ToListAsync();

            return orgCds;
        }

        public async Task<OrganizationDto> GetOrganizationByOrgCdAsync(string orgCd)
        {
            var org = await _dbSet.AsNoTracking()
                .Where(x => x.OrganizationCd == orgCd.ToUpper())
                .Select(OrganizationDtoWithoutContactsProjection)
                .FirstOrDefaultAsync();

            return org!;
        }

        public async Task<long?> GetContainingOrganizationId(Point point)
        {
            using var connection = new NpgsqlConnection(_connectionStringProvider.GetConnectionString());

            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT dss_containing_organization_id(@p_point)", connection);

            command.Parameters.AddWithValue("@p_point", point);

            var result = await command.ExecuteScalarAsync();

            return result == DBNull.Value ? null : (long)result!;
        }

        public async Task<long?> GetManagingOrgId(long orgId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.OrganizationId == orgId)
                .Select(x => x.ManagingOrganizationId)
                .FirstOrDefaultAsync();
        }

        public async Task<StrRequirementsDto?> GetStrRequirements(double longitude, double latitude)
        {
            var point = new Point(longitude, latitude) { SRID = 4326 };

            var strRequirement = await _dbContext.DssOrganizations
                .FromSqlRaw(@"
                    SELECT p.organization_nm, c.is_principal_residence_required, c.is_business_licence_required, c.is_str_prohibited, c.is_straa_exempt
                    FROM dss_organization c, dss_organization p
                    WHERE p.organization_id = c.managing_organization_id and c.organization_id = dss_containing_organization_id({0})", point)

                .Select(o => new StrRequirementsDto
                {
                    OrganizationNm = o.OrganizationNm,
                    IsPrincipalResidenceRequired = o.IsPrincipalResidenceRequired,
                    IsBusinessLicenceRequired = o.IsBusinessLicenceRequired,
                    IsStrProhibited = o.IsStrProhibited,
                    IsStraaExempt = o.IsStraaExempt
                })
                .OrderBy(o => o.OrganizationNm)
                .FirstOrDefaultAsync();

            return strRequirement;
        }

        public async Task<PagedDto<PlatformViewDto>> GetPlatforms(int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = _dbContext.DssPlatformVws.AsNoTracking().Where(x => x.ManagingOrganizationId == null);

            var platforms = await Page<DssPlatformVw, PlatformViewDto>(query, pageSize, pageNumber, orderBy, direction, PlatformViewProjection);

            foreach (var platform in platforms.SourceList)
            {
                platform.Subsidiaries = await _dbContext.DssPlatformVws.AsNoTracking()
                    .Where(x => x.ManagingOrganizationId == platform.OrganizationId)
                    .Select(PlatformViewProjection)
                    .ToListAsync();
            }

            return platforms;
        }

        public async Task<PlatformViewDto?> GetPlatform(long id)
        {
            var platform = await _dbContext.DssPlatformVws.AsNoTracking()
                .Where(x => x.OrganizationId == id)
                .Select(PlatformViewProjection)
                .FirstOrDefaultAsync();

            if (platform == null)
                return null;

            platform.Subsidiaries = await _dbContext.DssPlatformVws.AsNoTracking()
                .Where(x => x.ManagingOrganizationId == id)
                .Select(PlatformViewProjection)
                .ToListAsync();

            return platform;
        }

        public async Task<DssOrganization> CreatePlatformAsync(PlatformCreateDto dto)
        {
            var entity = _mapper.Map<DssOrganization>(dto);

            entity.OrganizationCd = dto.OrganizationCd.ToUpperInvariant();
            entity.OrganizationType = OrganizationTypes.Platform;

            await _dbSet.AddAsync(entity);

            CreateContact(entity, EmailMessageTypes.NoticeOfTakedown, dto.PrimaryNoticeOfTakedownContactEmail, true);
            CreateContact(entity, EmailMessageTypes.NoticeOfTakedown, dto.SecondaryNoticeOfTakedownContactEmail, false);
            CreateContact(entity, EmailMessageTypes.TakedownRequest, dto.PrimaryTakedownRequestContactEmail, true);
            CreateContact(entity, EmailMessageTypes.TakedownRequest, dto.SecondaryTakedownRequestContactEmail, false);
            CreateContact(entity, EmailMessageTypes.BatchTakedownRequest, dto.PrimaryTakedownRequestContactEmail, true);
            CreateContact(entity, EmailMessageTypes.BatchTakedownRequest, dto.SecondaryTakedownRequestContactEmail, false);

            return entity;
        }

        private void CreateContact(DssOrganization entity, string messageType, string? emailAddress, bool isPrimary)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                entity.DssOrganizationContactPeople.Add(new DssOrganizationContactPerson
                {
                    EmailAddressDsc = emailAddress,
                    IsPrimary = isPrimary,
                    EmailMessageType = messageType
                });
            }
        }

        public async Task<bool> DoesOrgCdExist(string orgCd)
        {
            return await _dbSet.AnyAsync(x => x.OrganizationCd == orgCd.ToUpperInvariant());
        }

        public async Task UpdatePlatformAsync(PlatformUpdateDto dto)
        {
            var entity = await _dbSet
                .Include(x => x.DssOrganizationContactPeople)
                .FirstAsync(x => x.OrganizationId == dto.OrganizationId);

            _mapper.Map(dto, entity);

            UpdateContact(entity, EmailMessageTypes.NoticeOfTakedown, dto.PrimaryNoticeOfTakedownContactEmail, true);
            UpdateContact(entity, EmailMessageTypes.NoticeOfTakedown, dto.SecondaryNoticeOfTakedownContactEmail, false);
            UpdateContact(entity, EmailMessageTypes.TakedownRequest, dto.PrimaryTakedownRequestContactEmail, true);
            UpdateContact(entity, EmailMessageTypes.TakedownRequest, dto.SecondaryTakedownRequestContactEmail, false);
            UpdateContact(entity, EmailMessageTypes.BatchTakedownRequest, dto.PrimaryTakedownRequestContactEmail, true);
            UpdateContact(entity, EmailMessageTypes.BatchTakedownRequest, dto.SecondaryTakedownRequestContactEmail, false);
        }

        private void UpdateContact(DssOrganization entity, string messageType, string? emailAddress, bool isPrimary)
        {
            var contact = entity.DssOrganizationContactPeople
                .FirstOrDefault(x => x.EmailMessageType == messageType
                    && (isPrimary ? x.IsPrimary == isPrimary : x.IsPrimary == false || x.IsPrimary == null));

            if (contact == null)
            {
                CreateContact(entity, messageType, emailAddress, isPrimary);
            }
            else
            {
                if (string.IsNullOrEmpty(emailAddress))
                {
                    _dbContext.DssOrganizationContactPeople.Remove(contact);
                }
                else
                {
                    contact.EmailAddressDsc = emailAddress;
                }
            }
        }

        public async Task<DssOrganization> CreatePlatformSubAsync(PlatformSubCreateDto dto)
        {
            var entity = _mapper.Map<DssOrganization>(dto);

            entity.OrganizationCd = dto.OrganizationCd.ToUpperInvariant();
            entity.OrganizationType = OrganizationTypes.Platform;

            await _dbSet.AddAsync(entity);

            CreateContact(entity, EmailMessageTypes.NoticeOfTakedown, dto.PrimaryNoticeOfTakedownContactEmail, true);
            CreateContact(entity, EmailMessageTypes.NoticeOfTakedown, dto.SecondaryNoticeOfTakedownContactEmail, false);
            CreateContact(entity, EmailMessageTypes.TakedownRequest, dto.PrimaryTakedownRequestContactEmail, true);
            CreateContact(entity, EmailMessageTypes.TakedownRequest, dto.SecondaryTakedownRequestContactEmail, false);
            CreateContact(entity, EmailMessageTypes.BatchTakedownRequest, dto.PrimaryTakedownRequestContactEmail, true);
            CreateContact(entity, EmailMessageTypes.BatchTakedownRequest, dto.SecondaryTakedownRequestContactEmail, false);

            return entity;
        }

        public async Task UpdatePlatformSubAsync(PlatformSubUpdateDto dto)
        {
            var entity = await _dbSet
                .Include(x => x.DssOrganizationContactPeople)
                .FirstAsync(x => x.OrganizationId == dto.OrganizationId);

            _mapper.Map(dto, entity);

            UpdateContact(entity, EmailMessageTypes.NoticeOfTakedown, dto.PrimaryNoticeOfTakedownContactEmail, true);
            UpdateContact(entity, EmailMessageTypes.NoticeOfTakedown, dto.SecondaryNoticeOfTakedownContactEmail, false);
            UpdateContact(entity, EmailMessageTypes.TakedownRequest, dto.PrimaryTakedownRequestContactEmail, true);
            UpdateContact(entity, EmailMessageTypes.TakedownRequest, dto.SecondaryTakedownRequestContactEmail, false);
            UpdateContact(entity, EmailMessageTypes.BatchTakedownRequest, dto.PrimaryTakedownRequestContactEmail, true);
            UpdateContact(entity, EmailMessageTypes.BatchTakedownRequest, dto.SecondaryTakedownRequestContactEmail, false);
        }

        public async Task<PagedDto<LocalGovViewDto>> GetLocalGovs(int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = _dbContext.DssLocalGovVws.AsNoTracking().AsQueryable();

            var lgs = await Page<DssLocalGovVw, LocalGovViewDto>(query, pageSize, pageNumber, orderBy, direction, LocalGovViewProjection);

            foreach (var lg in lgs.SourceList)
            {
                lg.Jurisdictions = await _dbSet.AsNoTracking()
                    .Where(x => x.ManagingOrganizationId == lg.OrganizationId)
                    .Select(JurisdictionViewProjection)
                    .ToListAsync();
            }

            return lgs;
        }

        public async Task UpdateLocalGovAsync(LocalGovUpdateDto dto)
        {
            var entity = await _dbSet
                .FirstAsync(x => x.OrganizationId == dto.OrganizationId);

            _mapper.Map(dto, entity);
        }

        public async Task<LocalGovViewDto?> GetLocalGov(long id)
        {
            return await _dbContext.DssLocalGovVws.AsNoTracking()
                .Where(x => x.OrganizationId == id)
                .Select(LocalGovViewProjection)
                .FirstOrDefaultAsync();
        }

        public async Task<JurisdictionsViewDto?> GetJurisdiction(long id)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.OrganizationType == OrganizationTypes.LGSub && x.OrganizationId == id)
                .Select(JurisdictionViewProjection)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateJurisdictionAsync(JurisdictionUpdateDto dto)
        {
            var entity = await _dbSet
                .FirstAsync(x => x.OrganizationId == dto.OrganizationId);

            _mapper.Map(dto, entity);
        }
    }
}
