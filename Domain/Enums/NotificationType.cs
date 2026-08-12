using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{

    public enum NotificationType
    {
        LabTestRequested = 1,
        LabResultReady = 2,
        AppointmentReminder = 3,
        AIReportGenerated = 4
    }
}
