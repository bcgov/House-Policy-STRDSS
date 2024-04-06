using AutoMapper;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using StrDss.Service.CsvHelpers;

namespace StrDss.Service
{
    public interface IRentalListingReportService
    {
        Task<Dictionary<string, List<string>>> ValidateAndParseUploadFileAsync(string reportPeriod, long orgId, TextReader textReader);
    }
    public class RentalListingReportService : ServiceBase, IRentalListingReportService
    {
        private IConfiguration _config;
        private ILogger<RentalListingReportService> _logger;

        public RentalListingReportService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IConfiguration config, ILogger<RentalListingReportService> logger)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<Dictionary<string, List<string>>> ValidateAndParseUploadFileAsync(string reportPeriod, long orgId, TextReader textReader)
        {
            var errors = new Dictionary<string, List<string>>();

            var csvConfig = CsvHelperUtils.GetConfig(errors, false);

            using var csv = new CsvReader(textReader, csvConfig);

            var mandatoryFields = new string[] { "rpt_period", "org_cd", "listing_id" };

            csv.Read();
            var headerExists = csv.ReadHeader();

            if (!headerExists)
            {
                errors.AddItem("File", "Header deosn't exist.");
                return errors;
            }

            if (!CheckCommonMandatoryFields(csv.HeaderRecord, mandatoryFields, errors))
            {
                return errors;
            }

            var reportPeriodMismatch = false;
            var reportPeriodMissing = false;
            var orgCdMissing = false;
            var listingIdMissing = false;

            var orgCds = new List<string>();

            while (csv.Read())
            {
                RentalListingRowUntyped row = null!;

                try
                {
                    row = csv.GetRecord<RentalListingRowUntyped>();

                    if (row.RptPeriod != reportPeriod) reportPeriodMismatch = true;
                    if (row.RptPeriod.IsEmpty()) reportPeriodMissing = true;
                    if (row.OrgCd.IsEmpty()) orgCdMissing = true;
                    if (row.ListingId.IsEmpty()) listingIdMissing = true;

                    if (row.OrgCd.IsNotEmpty() && !orgCds.Contains(row.OrgCd.ToUpper())) orgCds.Add(row.OrgCd.ToUpper());
                }
                catch (TypeConverterException ex)
                {
                    errors.AddItem(ex.MemberMapData.Member.Name, ex.Message);
                    break;
                }
                catch (CsvHelper.MissingFieldException)
                {
                    break; 
                }
                catch (CsvHelper.ReaderException ex)
                {
                    _logger.LogWarning(ex.Message);
                    throw;
                }
                catch (CsvHelperException ex)
                {
                    _logger.LogInformation(ex.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw;
                }
            }

            //check if orgCds are all valid

            await Task.CompletedTask;

            return errors;
        }

        private bool CheckCommonMandatoryFields(string[] headers, string[] mandatoryFields, Dictionary<string, List<string>> errors)
        {
            headers = CsvHelperUtils.GetLowercaseFieldsFromCsvHeaders(headers);

            foreach (var field in mandatoryFields)
            {
                if (!headers.Any(x => x == field.ToLowerInvariant()))
                    errors.AddItem("File", $"Header [{field}] is missing");
            }

            if (errors.Count > 0)
                errors.AddItem("File", "Please ensure the file headers are correct");

            return errors.Count == 0;
        }
    }
}
