namespace WBBEntity.PanelModels.WebServiceModels
{
    public class MobileDataUsageModel
    {
        /// <summary>
        /// (PRE/POST)หมายเลขโทรศัพท์
        /// </summary>
        public string MOBILE_NO { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Day  8:00 - 16:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_AIS_MORNING_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Evening  17:00 - 21:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_AIS_EVENING_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Night 22:00 - 7:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_AIS_NIGHT_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Weekday (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_AIS_WEEKDAY_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Weekend (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_AIS_WEEKEND_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Day  8:00 - 16:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_OTHER_MORNING_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Evening  17:00 - 21:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_OTHER_EVENING_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Night 22:00 - 7:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_OTHER_NIGHT_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Weekday (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_OTHER_WEEKDAY_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Weekend (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string OUT_OTHER_WEEKEND_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Day  8:00 - 16:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_AIS_MORNING_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Evening  17:00 - 21:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_AIS_EVENING_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Night 22:00 - 7:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_AIS_NIGHT_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Weekday (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_AIS_WEEKDAY_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายในเครือข่าย Weekend (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_AIS_WEEKEND_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Day  8:00 - 16:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_OTHER_MORNING_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Evening  17:00 - 21:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_OTHER_EVENING_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Night 22:00 - 7:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_OTHER_NIGHT_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Weekday (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_OTHER_WEEKDAY_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE)โทรออกภายนอกเครือข่าย Weekend (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string OUT_OTHER_WEEKEND_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE/POST)โทรเฉลี่ย 3 เดือน (นาที)
        /// </summary>
        public string TOTAL_OUTGOING_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE/POST)โทรรวมเดือนล่าสด (เดือนล่าสุด)
        /// </summary>
        public string TOTAL_OUTGOING_MINUTE { get; set; }
        /// <summary>
        /// (PRE/POST)โทรออกภายในเครือข่าย(เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string TOTAL_AIS_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE/POST)โทรออกภายนอกเครือข่าย(เฉลี่ย 3 เดือน) (นาที)
        /// </summary>
        public string TOTAL_OTHER_AVG_MINUTE { get; set; }
        /// <summary>
        /// (PRE/POST)โทรออกภายในเครือข่าย(เฉลี่ย 3 เดือน)  (บาท)
        /// </summary>
        public string TOTAL_AIS_AVG_BAHT { get; set; }
        /// <summary>
        /// (PRE/POST)โทรออกภายนอกเครือข่าย(เฉลี่ย 3 เดือน) (บาท)
        /// </summary>
        public string TOTAL_OTHER_AVG_BAHT { get; set; }
        /// <summary>
        /// (PRE/POST)โทรออกภายในเครือข่าย(เดือนล่าสุด)  (นาที)
        /// </summary>
        public string TOTAL_AIS_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE/POST)โทรออกภายนอกเครือข่าย(เดือนล่าสุด) (นาที)
        /// </summary>
        public string TOTAL_OTHER_LAST_MINUTE { get; set; }
        /// <summary>
        /// (PRE/POST)โทรออกภายในเครือข่าย(เดือน ล่าสุด)  (บาท)
        /// </summary>
        public string TOTAL_AIS_LAST_BAHT { get; set; }
        /// <summary>
        /// (PRE/POST)โทรออกภายนอกเครือข่าย(เดือนล่าสุด) (บาท)
        /// </summary>
        public string TOTAL_OTHER_LAST_BAHT { get; set; }
        /// <summary>
        /// (PRE)arpu (เดือนล่าสุด)
        /// </summary>
        public string INVOICE_AMOUNT_LAST { get; set; }
        /// <summary>
        /// (PRE)arpu (เฉลี่ย 3 เดือน)
        /// </summary>
        public string INVOICE_AMOUNT_AVG { get; set; }
        /// <summary>
        /// (PRE)รับสายรวม (เดือนล่าสุด) (นาที)
        /// </summary>
        public string TOTAL_INCOMING_LAST { get; set; }
        /// <summary>
        /// (PRE)รับสายรวม (เฉลี่ย 3 เดือน) (นาที)
        /// </summary>
        public string TOTAL_INCOMING_AVG { get; set; }
        /// <summary>
        /// (PRE)รับสายจากเบอร์ AIS (เดือนล่าสุด) (นาที)
        /// </summary>
        public string IN_FROM_AIS_LAST { get; set; }
        /// <summary>
        /// (PRE)รับสายจากเบอร์ AIS (เฉลี่ย 3 เดือน) (นาที)
        /// </summary>
        public string IN_FROM_AIS_AVG { get; set; }
        /// <summary>
        /// (PRE)รับสายจากค่ายอื่น (เดือนล่าสุด) (นาที)
        /// </summary>
        public string IN_FROM_OTHER_LAST { get; set; }
        /// <summary>
        /// (PRE)รับสายจากค่ายอื่น (เฉลี่ย 3 เดือน) (นาที)
        /// </summary>
        public string IN_FROM_OTHER_AVG { get; set; }
        /// <summary>
        /// (PRE)ยอด net ic (เดือนล่าสุด) (บาท)
        /// </summary>
        public string NET_IC_LAST { get; set; }
        /// <summary>
        /// (PRE)ยอด net ic (เฉลี่ย 3 เดือน) (บาท)
        /// </summary>
        public string NET_IC_AVG { get; set; }

        /// <summary>
        /// (POST)Day  6:00 - 17:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string DAY_MIN_AVG_3M { get; set; }
        /// <summary>
        /// (POST)Evening  18:00 - 21:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string EVENING_MIN_AVG_3M { get; set; }
        /// <summary>
        /// (POST)Night 22:00 - 23:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string NIGHT_MIN_AVG_3M { get; set; }
        /// <summary>
        /// (POST)Midnight 24:00 - 5:59 (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string MIDNIGHT_MIN_AVG_3M { get; set; }
        /// <summary>
        /// (POST)Weekday (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string WEEKDAY_MIN_AVG_3M { get; set; }
        /// <summary>
        /// (POST)Weekend (เฉลี่ย 3 เดือน)  (นาที)
        /// </summary>
        public string WEEKEND_MIN_AVG_3M { get; set; }
        /// <summary>
        /// (POST)Day  6:00 - 17:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string DAY_MIN { get; set; }
        /// <summary>
        /// (POST)Evening  18:00 - 21:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string EVENING_MIN { get; set; }
        /// <summary>
        /// (POST)Night 22:00 - 23:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string NIGHT_MIN { get; set; }
        /// <summary>
        /// (POST)Midnight 24:00 - 5:59 (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string MIDNIGHT_MIN { get; set; }
        /// <summary>
        /// (POST)Weekday (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string WEEKDAY_MIN { get; set; }
        /// <summary>
        /// (POST)Weekend (เดือนล่าสุด)  (นาที)
        /// </summary>
        public string WEEKEND_MIN { get; set; }
        /// <summary>
        /// (POST)arpu (เดือนล่าสุด)
        /// </summary>
        public string ARPU { get; set; }
        /// <summary>
        /// (POST)arpu (เฉลี่ย 3 เดือน)
        /// </summary>
        public string ARPU_AVG_3M { get; set; }
        /// <summary>
        /// (POST)รับสายรวม (เดือนล่าสุด) (นาที)
        /// </summary>
        public string TOTAL_INCOMING { get; set; }
        /// <summary>
        /// (POST)รับสายรวม (เฉลี่ย 3 เดือน) (นาที)
        /// </summary>
        public string TOTAL_INCOMING_AVG_3M { get; set; }
        /// <summary>
        /// (POST)รับสายจากเบอร์ AIS (เดือนล่าสุด) (นาที)
        /// </summary>
        public string INCOMING_FROM_AIS { get; set; }
        /// <summary>
        /// (POST)รับสายจากเบอร์ AIS (เฉลี่ย 3 เดือน) (นาที)
        /// </summary>
        public string INCOMING_FROM_AIS_AVG_3M { get; set; }
        /// <summary>
        /// (POST)รับสายจากค่ายอื่น (เดือนล่าสุด) (นาที)
        /// </summary>
        public string INCOMING_FROM_OTHER { get; set; }
        /// <summary>
        /// (POST)รับสายจากค่ายอื่น (เฉลี่ย 3 เดือน) (นาที)
        /// </summary>
        public string INCOMING_FROM_OTHER_AVG_3M { get; set; }
        /// <summary>
        /// (POST)ยอด net ic (เดือนล่าสุด) (บาท)
        /// </summary>
        public string NET_IC { get; set; }
        /// <summary>
        /// (POST)ยอด net ic (เฉลี่ย 3 เดือน) (บาท)
        /// </summary>
        public string NET_IC_AVG_3M { get; set; }

        /// <summary>
        /// Code สถานะของการการใช้ service (Retuen Code)
        /// </summary>
        public string codeResponse { get; set; }
        /// <summary>
        /// รายละเอียดของสถานะ (Return Message)
        /// </summary>
        public string description { get; set; }
    }
}
