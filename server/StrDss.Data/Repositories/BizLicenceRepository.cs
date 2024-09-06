using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StrDss.Data.Repositories
{
    public interface IBizLicenceRepository
    {
        Task<BizLicenceDto?> GetBizLicence(long businessLicenceId);
        Task<(string?, long?)> GetBizLicenceNoAndLgId(long businessLicenceId);
        Task CreateBizLicTempTable();
        Task InsertRowToBizLicTempTable(BizLicenceRowUntyped row, long providingOrganizationId);
        Task ProcessBizLicTempTable(long lgId);
        Task<(long?, string?)> GetMatchingBusinessLicenceIdAndNo(long orgId, string effectiveBizLicNo);
        Task<List<BizLicenceSearchDto>> SearchBizLicence(long orgId, string bizLicNo);

    }

    public class BizLicenceRepository : RepositoryBase<DssBusinessLicence>, IBizLicenceRepository
    {
        public BizLicenceRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<BizLicenceDto?> GetBizLicence(long businessLicenceId)
        {
            return _mapper.Map<BizLicenceDto>(await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.BusinessLicenceId == businessLicenceId));            
        }

        public async Task<(string?, long?)> GetBizLicenceNoAndLgId(long businessLicenceId)
        {
            var lincese = await _dbSet.AsNoTracking()
                .Where(x => x.BusinessLicenceId == businessLicenceId)
                .Select(x => new { x.BusinessLicenceNo, x.ProvidingOrganizationId })
                .FirstOrDefaultAsync();

            return lincese == null ? (null, null) : (lincese.BusinessLicenceNo, lincese.ProvidingOrganizationId);
        }

        public async Task CreateBizLicTempTable()
        {
            await _dbContext.Database.ExecuteSqlRawAsync(@"
                    CREATE TEMP TABLE biz_lic_table (
	                    business_licence_no varchar(50) NOT NULL,
	                    expiry_dt date NOT NULL,
	                    physical_rental_address_txt varchar(250) NULL,
	                    licence_type_txt varchar(320) NULL,
	                    restriction_txt varchar(320) NULL,
	                    business_nm varchar(320) NULL,
	                    mailing_street_address_txt varchar(100) NULL,
	                    mailing_city_nm varchar(100) NULL,
	                    mailing_province_cd varchar(2) NULL,
	                    mailing_postal_cd varchar(10) NULL,
	                    business_owner_nm varchar(320) NULL,
	                    business_owner_phone_no varchar(30) NULL,
	                    business_owner_email_address_dsc varchar(320) NULL,
	                    business_operator_nm varchar(320) NULL,
	                    business_operator_phone_no varchar(30) NULL,
	                    business_operator_email_address_dsc varchar(320) NULL,
	                    infraction_txt varchar(320) NULL,
	                    infraction_dt date NULL,
	                    property_zone_txt varchar(100) NULL,
	                    available_bedrooms_qty int2 NULL,
	                    max_guests_allowed_qty int2 NULL,
	                    is_principal_residence bool NULL,
	                    is_owner_living_onsite bool NULL,
	                    is_owner_property_tenant bool NULL,
	                    property_folio_no varchar(30) NULL,
	                    property_parcel_identifier_no varchar(30) NULL,
	                    property_legal_description_txt varchar(320) NULL,
	                    licence_status_type varchar(25) NOT NULL,
	                    providing_organization_id int8 NOT NULL
                    );
                ");
        }

        public async Task InsertRowToBizLicTempTable(BizLicenceRowUntyped row, long providingOrganizationId)
        {
            var insertSql = @"
                INSERT INTO biz_lic_table (
                    business_licence_no, expiry_dt, physical_rental_address_txt, licence_type_txt, restriction_txt, business_nm, mailing_street_address_txt, 
                    mailing_city_nm, mailing_province_cd, mailing_postal_cd, business_owner_nm, business_owner_phone_no, business_owner_email_address_dsc, 
                    business_operator_nm, business_operator_phone_no, business_operator_email_address_dsc, infraction_txt, infraction_dt, 
                    property_zone_txt, available_bedrooms_qty, max_guests_allowed_qty, is_principal_residence, is_owner_living_onsite, 
                    is_owner_property_tenant, property_folio_no, property_parcel_identifier_no, property_legal_description_txt, 
                    licence_status_type, providing_organization_id
                )
                VALUES (
                    @BusinessLicenceNo, @ExpiryDt, @PhysicalRentalAddressTxt, @LicenceTypeTxt, @RestrictionTxt, @BusinessNm, @MailingStreetAddressTxt, 
                    @MailingCityNm, @MailingProvinceCd, @MailingPostalCd, @BusinessOwnerNm, @BusinessOwnerPhoneNo, @BusinessOwnerEmailAddressDsc, 
                    @BusinessOperatorNm, @BusinessOperatorPhoneNo, @BusinessOperatorEmailAddressDsc, @InfractionTxt, @InfractionDt, 
                    @PropertyZoneTxt, @AvailableBedroomsQty, @MaxGuestsAllowedQty, @IsPrincipalResidence, @IsOwnerLivingOnsite, 
                    @IsOwnerPropertyTenant, @PropertyFolioNo, @PropertyParcelIdentifierNo, @PropertyLegalDescriptionTxt, 
                    @LicenceStatusType, @ProvidingOrganizationId
                );";

            bool ConvertToBool(string value) => value?.ToUpper() == "Y";

            DateTime? ConvertToDate(string value) => DateTime.TryParseExact(value, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var date) ? date : (DateTime?) null;

            short? ConvertToInt2(string value) => short.TryParse(value, out var result) ? result : (short?)null;

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@businessLicenceNo", row.BusinessLicenceNo.Trim().ToUpper()),

                new NpgsqlParameter("@expiryDt", NpgsqlTypes.NpgsqlDbType.Date)
                {
                    Value = ConvertToDate(row.ExpiryDt) ?? (object)DBNull.Value
                },

                new NpgsqlParameter("@physicalRentalAddressTxt", row.PhysicalRentalAddressTxt ?? (object)DBNull.Value),
                new NpgsqlParameter("@licenceTypeTxt", row.LicenceTypeTxt ?? (object)DBNull.Value),
                new NpgsqlParameter("@restrictionTxt", row.RestrictionTxt ?? (object)DBNull.Value),
                new NpgsqlParameter("@businessNm", row.BusinessNm ?? (object)DBNull.Value),
                new NpgsqlParameter("@mailingStreetAddressTxt", row.MailingStreetAddressTxt ?? (object)DBNull.Value),
                new NpgsqlParameter("@mailingCityNm", row.MailingCityNm ?? (object)DBNull.Value),
                new NpgsqlParameter("@mailingProvinceCd", row.MailingProvinceCd ?? (object)DBNull.Value),
                new NpgsqlParameter("@mailingPostalCd", row.MailingPostalCd ?? (object)DBNull.Value),
                new NpgsqlParameter("@businessOwnerNm", row.BusinessOwnerNm ?? (object)DBNull.Value),
                new NpgsqlParameter("@businessOwnerPhoneNo", row.BusinessOwnerPhoneNo ?? (object)DBNull.Value),
                new NpgsqlParameter("@businessOwnerEmailAddressDsc", row.BusinessOwnerEmailAddressDsc ?? (object)DBNull.Value),
                new NpgsqlParameter("@businessOperatorNm", row.BusinessOperatorNm ?? (object)DBNull.Value),
                new NpgsqlParameter("@businessOperatorPhoneNo", row.BusinessOperatorPhoneNo ?? (object)DBNull.Value),
                new NpgsqlParameter("@businessOperatorEmailAddressDsc", row.BusinessOperatorEmailAddressDsc ?? (object)DBNull.Value),
                new NpgsqlParameter("@infractionTxt", row.InfractionTxt ?? (object)DBNull.Value),

                new NpgsqlParameter("@infractionDt", NpgsqlTypes.NpgsqlDbType.Date)
                {
                    Value = ConvertToDate(row.InfractionDt) ?? (object)DBNull.Value
                },

                new NpgsqlParameter("@propertyZoneTxt", row.PropertyZoneTxt ?? (object)DBNull.Value),
                new NpgsqlParameter("@availableBedroomsQty", ConvertToInt2(row.AvailableBedroomsQty) ?? (object)DBNull.Value),
                new NpgsqlParameter("@maxGuestsAllowedQty", ConvertToInt2(row.MaxGuestsAllowedQty) ?? (object)DBNull.Value),
                new NpgsqlParameter("@isPrincipalResidence", ConvertToBool(row.IsPrincipalResidence)),
                new NpgsqlParameter("@isOwnerLivingOnsite", ConvertToBool(row.IsOwnerLivingOnsite)),
                new NpgsqlParameter("@isOwnerPropertyTenant", ConvertToBool(row.IsOwnerPropertyTenant)),
                new NpgsqlParameter("@propertyFolioNo", row.PropertyFolioNo ?? (object)DBNull.Value),
                new NpgsqlParameter("@propertyParcelIdentifierNo", row.PropertyParcelIdentifierNo ?? (object)DBNull.Value),
                new NpgsqlParameter("@propertyLegalDescriptionTxt", row.PropertyLegalDescriptionTxt ?? (object)DBNull.Value),
                new NpgsqlParameter("@licenceStatusType", row.LicenceStatusType),
                new NpgsqlParameter("@providingOrganizationId", providingOrganizationId) 
            };

            await _dbContext.Database.ExecuteSqlRawAsync(insertSql, parameters.ToArray());
        }

        public async Task ProcessBizLicTempTable(long lgId)
        {
            var processStopwatch = Stopwatch.StartNew();

            _dbContext.Database.SetCommandTimeout(2400);

            await _dbContext.Database.ExecuteSqlRawAsync($"CALL dss_process_biz_lic_table({lgId});");

            processStopwatch.Stop();

            _logger.LogInformation($"Business Licence Link to Listings finished  - {processStopwatch.Elapsed.TotalSeconds} seconds");
        }

        public async Task<(long?, string?)> GetMatchingBusinessLicenceIdAndNo(long orgId, string effectiveBizLicNo)
        {
            // Raw SQL query using PostgreSQL regexp_replace to remove non-alphanumeric characters in the database query
            var sqlQuery = @"
                SELECT business_licence_id, business_licence_no
                FROM dss_business_licence
                WHERE providing_organization_id = @orgId 
                  AND regexp_replace(business_licence_no, '[^a-zA-Z0-9]', '', 'g') = @sanitizedBizLicNo
                LIMIT 1";

            var licence = await _dbContext.DssBusinessLicences
                .FromSqlRaw(sqlQuery, new NpgsqlParameter("orgId", orgId), new NpgsqlParameter("sanitizedBizLicNo", effectiveBizLicNo))
                .Select(x => new { x.BusinessLicenceId, x.BusinessLicenceNo })
                .FirstOrDefaultAsync();

            return licence == null ? (null, null) : (licence.BusinessLicenceId, CommonUtils.SanitizeAndUppercaseString(licence.BusinessLicenceNo));
        }

        public async Task<List<BizLicenceSearchDto>> SearchBizLicence(long orgId, string bizLicNo)
        {
            bizLicNo = bizLicNo.ToUpper();

            var licences = await _dbSet.AsNoTracking()
                .Where(x => x.BusinessLicenceNo.Contains(bizLicNo))
                .ToListAsync();

            return _mapper.Map<List<BizLicenceSearchDto>>(licences);
        }

    }
}
