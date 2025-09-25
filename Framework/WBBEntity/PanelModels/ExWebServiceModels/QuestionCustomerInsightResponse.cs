using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class QuestionCustomerInsightResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<ListQuestionData> ListQuestion { get; set; }
        public List<ListAnswerData> ListAnswer { get; set; }
        public List<ListSubAnswerData> ListSubAnswer { get; set; }
    }

    public class ListQuestionData
    {
        public string group_id { get; set; }
        public string group_name_th { get; set; }
        public string group_name_en { get; set; }
        public string group_seq { get; set; }
        public string question_id { get; set; }
        public string question_seq { get; set; }
        public string question_th { get; set; }
        public string question_en { get; set; }
        public string question_desc_th { get; set; }
        public string question_desc_en { get; set; }
        public string require_answer_flag { get; set; }
        public string check_action_flag { get; set; }
        public string channel { get; set; }
        public string technology { get; set; }
    }

    public class ListAnswerData
    {
        public string question_id { get; set; }
        public string answer_id { get; set; }
        public string answer_seq { get; set; }
        public string answer_th { get; set; }
        public string answer_en { get; set; }
        public string parent_answer_id { get; set; }
        public string action_wfm { get; set; }
        public string action_foa { get; set; }
        public string display_type { get; set; }
        public string action { get; set; }
        public string value { get; set; }
    }

    public class ListSubAnswerData
    {
        public string answer_id { get; set; }
        public string answer_value_th { get; set; }
        public string answer_value_en { get; set; }
    }


}
