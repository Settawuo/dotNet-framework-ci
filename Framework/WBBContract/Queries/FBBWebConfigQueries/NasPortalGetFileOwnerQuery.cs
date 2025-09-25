namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class NasPortalGetFileOwnerQuery : IQuery<NasPortalFileOwnerModel>
    {
        public string p_file_name { get; set; }
    }

    public class NasPortalFileOwnerModel
    {
        public string ret_file_owner { get; set; }
        public int ret_code { get; set; }
    }
}
