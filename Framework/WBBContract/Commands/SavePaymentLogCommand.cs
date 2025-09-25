namespace WBBContract.Commands
{
    public class SavePaymentLogCommand
    {
        public SavePaymentLogCommand()
        {
            this.Return_Code = "-1";
            this.Return_Desc = "";
        }

        public string Transaction_Id { get; set; }
        public string FullUrl { get; set; }
        public string p_action { get; set; }
        public string p_payment_order_id { get; set; }
        public string p_process_name { get; set; }
        public string p_endpoint { get; set; }
        public string p_req_project_code { get; set; }
        public string p_req_command { get; set; }
        public string p_req_sid { get; set; }
        public string p_req_redirect_url { get; set; }
        public string p_req_merchant_id { get; set; }
        public string p_req_order_id { get; set; }
        public string p_req_currency { get; set; }
        public string p_req_purchase_amt { get; set; }
        public string p_req_payment_method { get; set; }
        public string p_req_product_desc { get; set; }
        public string p_req_ref1 { get; set; }
        public string p_req_ref2 { get; set; }
        public string p_req_ref3 { get; set; }
        public string p_req_ref4 { get; set; }
        public string p_req_ref5 { get; set; }
        public string p_req_integrity_str { get; set; }
        public string p_req_sms_flag { get; set; }
        public string p_req_sms_mobile { get; set; }
        public string p_req_mobile_no { get; set; }
        public string p_req_token_key { get; set; }
        public string p_req_order_expire { get; set; }
        public string p_resp_status { get; set; }
        public string p_resp_resp_code { get; set; }
        public string p_resp_resp_desc { get; set; }
        public string p_resp_sale_id { get; set; }
        public string p_resp_endpoint_url { get; set; }
        public string p_resp_detail1 { get; set; }
        public string p_resp_detail2 { get; set; }
        public string p_resp_detail3 { get; set; }
        public string p_post_status { get; set; }
        public string p_post_resp_code { get; set; }
        public string p_post_resp_desc { get; set; }
        public string p_post_tran_id { get; set; }
        public string p_post_sale_id { get; set; }
        public string p_post_order_id { get; set; }
        public string p_post_currency { get; set; }
        public string p_post_exchange_rate { get; set; }
        public string p_post_purchase_amt { get; set; }
        public string p_post_amount { get; set; }
        public string p_post_inc_customer_fee { get; set; }
        public string p_post_exc_customer_fee { get; set; }
        public string p_post_payment_status { get; set; }
        public string p_post_payment_code { get; set; }
        public string p_post_order_expire_date { get; set; }
        //18.10 : QR Code
        public string p_req_app_id { get; set; }
        public string p_req_app_secret { get; set; }
        public string p_req_channel { get; set; }
        public string p_req_qr_type { get; set; }
        public string p_req_terminal_id { get; set; }
        public string p_req_service_id { get; set; }
        public string p_req_location_name { get; set; }
        public string p_req_tran_id { get; set; }
        public string p_resp_qr_format { get; set; }
        public string p_resp_qr_code_str { get; set; }
        public string p_resp_qr_code_validity { get; set; }
        public string p_resp_reference { get; set; }
        public string p_resp_tran_dtm { get; set; }
        public string p_resp_tran_id { get; set; }
        public string p_resp_service_id { get; set; }
        public string p_resp_terminal_id { get; set; }
        public string p_resp_location_name { get; set; }
        public string p_resp_amount { get; set; }
        public string p_resp_sof { get; set; }
        public string p_resp_qr_type { get; set; }
        public string p_resp_refund_dt { get; set; }
        public string p_resp_dispute_id { get; set; }
        public string p_resp_dispute_status { get; set; }
        public string p_resp_dispute_reason_id { get; set; }
        public string p_resp_ref1 { get; set; }
        public string p_resp_ref2 { get; set; }
        public string p_resp_ref3 { get; set; }
        public string p_resp_ref4 { get; set; }
        public string p_resp_ref5 { get; set; }
        public string p_post_tran_dtm { get; set; }
        public string p_post_service_id { get; set; }
        public string p_post_terminal_id { get; set; }
        public string p_post_location_name { get; set; }
        public string p_post_sof { get; set; }
        public string p_post_qr_type { get; set; }
        public string p_post_ref1 { get; set; }
        public string p_post_ref2 { get; set; }
        public string p_post_ref3 { get; set; }
        public string p_post_ref4 { get; set; }
        public string p_post_ref5 { get; set; }

        public string Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }

    public class UpdatOrderPaymentStatusCommand
    {
        public string Status { get; set; }
    }
}
