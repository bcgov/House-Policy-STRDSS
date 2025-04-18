﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Npgsql;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.OrganizationDtos;
using System.Data;

namespace StrDss.Data.Repositories
{
    public interface IOrganizationRepository
    {
        Task<List<LocalGovTypeDto>> GetLocalGovTypesAsync();
        Task<List<EconomicRegionDto>> GetEconomicRegionsAsync();
        Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc();
        Task<List<OrganizationDto>> GetOrganizationsAsync(string? type);
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
        private IConfiguration _config;

        public OrganizationRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger,
            IConfiguration config)
            : base(dbContext, mapper, currentUser, logger)
        {
            _config = config;
        }
        public async Task<List<LocalGovTypeDto>> GetLocalGovTypesAsync()
        {
            var types = _mapper.Map<List<LocalGovTypeDto>>(
                await _dbContext.DssLocalGovernmentTypes.AsNoTracking()
                .OrderBy(x => x.LocalGovernmentTypeSortNo)
                .ToListAsync());

            return types;
        }

        public async Task<List<EconomicRegionDto>> GetEconomicRegionsAsync()
        {
            var regions = _mapper.Map<List<EconomicRegionDto>>(
                await _dbContext.DssEconomicRegions.AsNoTracking()
                .OrderBy(x => x.EconomicRegionSortNo)
                .ToListAsync());

            return regions;
        }

        public async Task<List<OrganizationTypeDto>> GetOrganizationTypesAsnc()
        {
            var types = _mapper.Map<List<OrganizationTypeDto>>(
                await _dbContext.DssOrganizationTypes.AsNoTracking()
                .Where(x => x.OrganizationType != OrganizationTypes.LGSub)
                .ToListAsync());

            return types;
        }

        public async Task<List<PlatformTypeDto>> GetPlatformTypesAsync()
        {
            var platformTypes = _mapper.Map<List<PlatformTypeDto>>(
                await _dbContext.DssPlatformTypes.AsNoTracking().ToListAsync());


            //await _dbContext.DssPlatformTypes.ToListAsync();

            return platformTypes;
        }

        public async Task<List<OrganizationDto>> GetOrganizationsAsync(string? type)
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

            query = query.Include(x => x.DssOrganizationContactPeople);

            return _mapper.Map<List<OrganizationDto>>(await query.ToListAsync());
        }

        public async Task<OrganizationDto?> GetOrganizationByIdAsync(long orgId)
        {
            var org = await _dbSet.AsNoTracking()
                .Include(x => x.DssOrganizationContactPeople)
                .FirstOrDefaultAsync(x => x.OrganizationId == orgId);

            return _mapper.Map<OrganizationDto>(org);
        }

        public async Task<List<string>> GetManagingOrgCdsAsync(long orgId)
        {
            var org = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == orgId);

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
                .FirstOrDefaultAsync(x => x.OrganizationCd == orgCd.ToUpper());

            return _mapper.Map<OrganizationDto>(org);
        }

        public async Task<long?> GetContainingOrganizationId(Point point)
        {
            var dbHost = _config.GetValue<string>("DB_HOST");
            var dbName = _config.GetValue<string>("DB_NAME");
            var dbUser = _config.GetValue<string>("DB_USER");
            var dbPass = _config.GetValue<string>("DB_PASS");
            var dbPort = _config.GetValue<string>("DB_PORT");

            var connString = $"Host={dbHost};Username={dbUser};Password={dbPass};Database={dbName};Port={dbPort};";

            using var connection = new NpgsqlConnection(connString);

            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT dss_containing_organization_id(@p_point)", connection);

            command.Parameters.AddWithValue("@p_point", point);

            var result = await command.ExecuteScalarAsync();

            return result == DBNull.Value ? null : (long)result!;
        }

        public async Task<long?> GetManagingOrgId(long orgId)
        {
            var org = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == orgId);
            return org?.ManagingOrganizationId;
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

            var platforms = await Page<DssPlatformVw, PlatformViewDto>(query, pageSize, pageNumber, orderBy, direction);

            foreach (var platform in platforms.SourceList)
            {
                platform.Subsidiaries = _mapper.Map<List<PlatformViewDto>>
                    (await _dbContext.DssPlatformVws.AsNoTracking().Where(x => x.ManagingOrganizationId == platform.OrganizationId).ToListAsync());
            }

            return platforms;
        }

        public async Task<PlatformViewDto?> GetPlatform(long id)
        {
            var platform = _mapper.Map<PlatformViewDto>(await _dbContext.DssPlatformVws.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == id));

            if (platform == null)
                return null;

            platform.Subsidiaries = _mapper.Map<List<PlatformViewDto>>(await _dbContext.DssPlatformVws.AsNoTracking().Where(x => x.ManagingOrganizationId == id).ToListAsync());

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

            var lgs = await Page<DssLocalGovVw, LocalGovViewDto>(query, pageSize, pageNumber, orderBy, direction);

            foreach (var lg in lgs.SourceList)
            {
                lg.Jurisdictions = _mapper.Map<List<JurisdictionsViewDto>>
                    (await _dbSet.AsNoTracking().Where(x => x.ManagingOrganizationId == lg.OrganizationId).ToListAsync());
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
            var localGov = _mapper.Map<LocalGovViewDto>
                (await _dbContext.DssLocalGovVws.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == id));

            return localGov;
        }

        public async Task<JurisdictionsViewDto?> GetJurisdiction(long id)
        {
            var jurisdiction = _mapper.Map<JurisdictionsViewDto>
                (await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationType == OrganizationTypes.LGSub && x.OrganizationId == id));

            return jurisdiction;
        }

        public async Task UpdateJurisdictionAsync(JurisdictionUpdateDto dto)
        {
            var entity = await _dbSet
                .FirstAsync(x => x.OrganizationId == dto.OrganizationId);

            _mapper.Map(dto, entity);
        }
    }
}
