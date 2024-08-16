using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// The type or purpose of a system generated message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)
/// </summary>
public partial class DssEmailMessageType
{
    /// <summary>
    /// System-consistent code for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)
    /// </summary>
    public string EmailMessageType { get; set; } = null!;

    /// <summary>
    /// Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)
    /// </summary>
    public string EmailMessageTypeNm { get; set; } = null!;

    public virtual ICollection<DssEmailMessage> DssEmailMessages { get; } = new List<DssEmailMessage>();

    public virtual ICollection<DssOrganizationContactPerson> DssOrganizationContactPeople { get; } = new List<DssOrganizationContactPerson>();
}
