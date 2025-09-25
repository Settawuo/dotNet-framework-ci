using System.Collections.Generic;
using System.Xml.Serialization;

namespace WBBContract.Minions
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "ExternalSmsSurveyFoa", Namespace = "", IsNullable = false)]
    public class MinionExternalSmsSurveyFoaCommand
    {
        [XmlArray("External_Sms_Survey_Foa"), XmlArrayItem(typeof(MinionExternalSmsSurveyFoaRec), ElementName = "REC_EXTERNAL_SMS_SURVEY_FOA")]
        public List<MinionExternalSmsSurveyFoaRec> ExternalSmsSurveyFoaList { get; set; }

        [SkipProperty]
        public List<MinionExternalSmsSurveyFoaResult> Results { get; set; }

        public MinionExternalSmsSurveyFoaCommand()
        {
            ExternalSmsSurveyFoaList = new List<MinionExternalSmsSurveyFoaRec>();
            Results = new List<MinionExternalSmsSurveyFoaResult>();
        }

        [XmlTypeAttribute(AnonymousType = true)]
        [XmlRootAttribute(ElementName = "REC_EXTERNAL_SMS_SURVEY_FOA", Namespace = "", IsNullable = false)]
        public class MinionExternalSmsSurveyFoaRec
        {
            public string InterfaceId { get; set; }
            public string OrderNo { get; set; }
            public string NonMobile { get; set; }
        }

        [XmlTypeAttribute(AnonymousType = true)]
        [XmlRootAttribute(ElementName = "RESULT_EXTERNAL_SMS_SURVEY_FOA", Namespace = "", IsNullable = false)]
        public class MinionExternalSmsSurveyFoaResult
        {
            public string InterfaceId { get; set; }
            public string OrderNo { get; set; }
            public string NonMobile { get; set; }
            public string ReturnCode { get; set; }
            public string ReturnDesc { get; set; }
        }
    }
}
