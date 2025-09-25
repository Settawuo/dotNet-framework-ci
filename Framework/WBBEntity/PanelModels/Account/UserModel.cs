using System.Collections.Generic;

namespace WBBEntity.PanelModels.Account
{
    public class UserModel
    {
        public List<ProgramModel> ProgramModel { get; set; }
        public List<ComponentModel> ComponentModel { get; set; }
        public List<decimal> Groups { get; set; }

        public decimal GroupId { get; set; }
        public decimal UserId { get; set; }
        public string UserName { get; set; }
        public string TitleInThai { get; set; }
        public string FirstNameInThai { get; set; }
        public string LastNameInThai { get; set; }
        public string UserFullNameInThai
        {
            get
            {
                return string.Format("{0} {1} {2}", TitleInThai, FirstNameInThai, LastNameInThai);
            }
        }

        public string TitleInEng { get; set; }
        public string FirstNameInEng { get; set; }
        public string LastNameInEng { get; set; }
        public string UserFullNameInEng
        {
            get
            {
                return string.Format("{0} {1} {2}", TitleInEng, FirstNameInEng, LastNameInEng);
            }
        }

        public string PhotoPath { get; set; }
        public string PositionName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public bool PreferredSMS { get; set; }
        public bool PreferredEmail { get; set; }

        public AuthenticateType AuthenticateType { get; set; }
        public SSOFields SSOFields { get; set; }
        public bool ForceLogOut { get; set; }
    }

    public class ProgramModel
    {
        public decimal ProgramID { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public string ProgramDescription { get; set; }
        public decimal? ParentID { get; set; }
        public List<ProgramModel> ProgramModels { get; set; }
    }

    public class ComponentModel
    {
        public string ProgramID { get; set; }
        public string ComponentName { get; set; }
        public string ComponentType { get; set; }
        public string EnableFlag { get; set; }
        public string ReadOnlyFlag { get; set; }
        public decimal GroupID { get; set; }
    }

    public enum AuthenticateType
    {
        NotLoggedOn = 0,
        SSO = 1,
        SSOPartner = 2,
        LDAP = 3
    }

    public class SSOFields
    {
        public string Token { get; set; }
        public string SessionID { get; set; }
        public string UserName { get; set; }
        public string GroupID { get; set; }
        public string SubModuleIDInToken { get; set; }
        public string ClientIP { get; set; }
        public string RoleID { get; set; }
        public string SubModuleID { get; set; }
        public string RoleName { get; set; }
        public string SubModuleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ThemeName { get; set; }
        public string TemplateName { get; set; }
        public string EmployeeServiceWebRootUrl { get; set; }
        public string LocationCode { get; set; }
        public string GroupLocation { get; set; }
        public string DepartmentCode { get; set; }
        public string SectionCode { get; set; }
        public string PositionByJob { get; set; }
    }
}
