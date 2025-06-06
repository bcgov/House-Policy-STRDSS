﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using StrDss.Test.AutoDomainDataBuilder;

namespace StrDss.Test
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public AutoDomainDataAttribute()
            : base(() => new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new TakedownNoticeCreateDtoCustomization())
                .Customize(new TakedownRequestCreateDtoCustomization())
                .Customize(new EmailContentCustomization())
                .Customize(new OrganizationDtoCustomization())
                .Customize(new UserDtoCustomization())
                .Customize(new DssUploadLineCustomization())
                .Customize (new RentalListingRowUntypedCustomization())
                .Customize(new UserUpdateDtoCustomization())
                .Customize(new RoleUpdateDtoCustomization())
                .Customize(new RoleDtoCustomization())
                .Customize(new AccessRequestCreateDtoCustomization())
            )                
        {
        }
    }
}
