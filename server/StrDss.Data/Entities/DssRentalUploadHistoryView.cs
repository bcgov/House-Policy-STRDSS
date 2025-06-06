﻿using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssRentalUploadHistoryView
{
    public long? UploadDeliveryId { get; set; }

    public string? UploadDeliveryType { get; set; }

    public DateOnly? ReportPeriodYm { get; set; }

    public long? ProvidingOrganizationId { get; set; }

    public string? OrganizationNm { get; set; }

    public DateTime? UpdDtm { get; set; }

    public string? GivenNm { get; set; }

    public string? FamilyNm { get; set; }

    public long? Total { get; set; }

    public long? Processed { get; set; }

    public long? Errors { get; set; }

    public long? RegistrationErrors { get; set; }

    public long? Success { get; set; }

    public long? RegistrationSuccess { get; set; }

    public string? Status { get; set; }

    public string? RegistrationStatus { get; set; }

    public DateTime? UploadDate { get; set; }
}
