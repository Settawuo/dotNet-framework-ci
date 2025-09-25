using System;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class FBSSTimeSlot
    {
        public FBSSTimeSlot()
        {
            this.AppointmentDate = DateTime.Now;
        }

        public DateTime? AppointmentDate { get; set; }
        public string TimeSlot { get; set; }
        public string InstallationCapacity { get; set; }

        public string TimeSlotId { get; set; }
        public string FirstTimeSlot { get; set; }
        public string FirstInstallDate { get; set; }

        public string AppointmentDate_Initial { get; set; }
        public string CurrentDate_Initial { get; set; }
        public string TimeSlotRegisterHR { get; set; }

        public string DayOffFlag { get; set; }
        public string TimeFlag { get; set; }
        public string ActiveFlag { get; set; }

        public DateTime? OrderByTimeSlot { get; set; }
    }

    public class FBSSTimeSlotChangePro
    {
        public string AppointmentDate { get; set; }
        public string TimeSlot { get; set; }
        public string InstallationCapacity { get; set; }

        public string TimeSlotId { get; set; }
    }

    public class DataForAppointment
    {
        public string INSTALLATION_DATE { get; set; }
        public string ACCESS_MODE { get; set; }
        public string PROD_SPEC_CODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string DAYS { get; set; }
        public string SUBDISTRICT { get; set; }
        public string POSTCODE { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
    }

    public class DataForReserveTimeslot
    {
        public string APPOINTMENT_DATE { get; set; }
        public string TIME_SLOT { get; set; }
        public string ACCESS_MODE { get; set; }
        public string PROD_SPEC_CODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string SUBDISTRICT { get; set; }
        public string POSTCODE { get; set; }
        public string ASSIGN_RULE { get; set; }
        public string SUB_ACCESS_MODE { get; set; }
    }

    public class CheckNoToPB
    {
        public string CHECK_NO_TO_PB { get; set; }
    }

    public class GetInputGetListPackageBySFFPROMO
    {
        public string p_sff_promocode { get; set; }
        public string p_product_subtype { get; set; }
        public string p_owner_product { get; set; }
        public string p_vas_service { get; set; }
    }

    public class DataDATAILChangepro
    {
        public string START_DATE { get; set; }
        public string INSTALL_DATE { get; set; }
        public string RETURN_CODE { get; set; }
    }

    public class FBSSTimeSlotPreSurvey
    {
        public DateTime? PreSurveyDate { get; set; }
        public string PreSurveyTimeSlot { get; set; }
        public string PreSurveyTimeSlotId { get; set; }
    }
}
