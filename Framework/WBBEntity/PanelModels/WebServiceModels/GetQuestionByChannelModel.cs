using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetQuestionByChannelModel
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public List<QuestionData> questionDatas { get; set; }
        public List<AnswerData> answerDatas { get; set; }
        public List<SubAnswerData> subAnswerDatas { get; set; }
    }

    public class QuestionData
    {
        public string GROUP_ID { get; set; }
        public string GROUP_NAME_TH { get; set; }
        public string GROUP_NAME_EN { get; set; }
        public decimal GROUP_SEQ { get; set; }
        public string QUESTION_ID { get; set; }
        public decimal QUESTION_SEQ { get; set; }
        public string QUESTION_TH { get; set; }
        public string QUESTION_EN { get; set; }
        public string QUESTION_DESC_TH { get; set; }
        public string QUESTION_DESC_EN { get; set; }
        public string REQUIRE_ANSWER_FLAG { get; set; }
        public string CHECK_ACTION_FLAG { get; set; }
        public string CHANNEL { get; set; }
        public string TECHNOLOGY { get; set; }
    }

    public class AnswerData
    {
        public string QUESTION_ID { get; set; }
        public string ANSWER_ID { get; set; }
        public decimal ANSWER_SEQ { get; set; }
        public string ANSWER_TH { get; set; }
        public string ANSWER_EN { get; set; }
        public string PARENT_ANSWER_ID { get; set; }
        public string ACTION_WFM { get; set; }
        public string ACTION_FOA { get; set; }
        public string DISPLAY_TYPE { get; set; }
        public string ACTION { get; set; }
        public string VALUE { get; set; }
    }

    public class SubAnswerData
    {
        public string ANSWER_ID { get; set; }
        public string ANSWER_VALUE_TH { get; set; }
        public string ANSWER_VALUE_EN { get; set; }
    }
}
