using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class ExistingRegisterBulkCorpQuery : IQuery<returnExistRegister>
    {
        public string p_user { get; set; }
        public string p_bulk_number { get; set; }
        public string p_errormessage { get; set; }
        public string p_total { get; set; }
        public string p_rownum { get; set; }
        public string p_rowid { get; set; }
        public string p_accntno { get; set; }
        public string p_accntclass { get; set; }
        public string p_name { get; set; }
        public string p_idcardnum { get; set; }
        public string p_idcardtype { get; set; }
        public string p_contactbirthdt { get; set; }
        public string p_statuscd { get; set; }
        public string p_accntcategory { get; set; }
        public string p_accntsubcategory { get; set; }
        public string p_mainphone { get; set; }
        public string p_mainmobile { get; set; }
        public string p_legalflg { get; set; }
        public string p_houseno { get; set; }
        public string p_buildingname { get; set; }
        public string p_floor { get; set; }
        public string p_room { get; set; }
        public string p_moo { get; set; }
        public string p_mooban { get; set; }
        public string p_streetname { get; set; }
        public string p_soi { get; set; }
        public string p_zipcode { get; set; }
        public string p_tumbol { get; set; }
        public string p_amphur { get; set; }
        public string p_provincename { get; set; }
        public string p_country { get; set; }
        public string p_vatname { get; set; }
        public string p_vatrate { get; set; }
        public string p_vataddress1 { get; set; }
        public string p_vataddress2 { get; set; }
        public string p_vataddress3 { get; set; }
        public string p_vataddress4 { get; set; }
        public string p_vataddress5 { get; set; }
        public string p_vatpostalcd { get; set; }
        public string p_accounttitle { get; set; }

        //return
        public string output_bulk_no { get; set; }
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }
    }
}
