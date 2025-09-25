namespace WBBContract.Queries.WebServices.Tracking
{
    public class GetEncryptDecryptQuery : IQuery<GetEncryptDecryptModel>
    {
        public bool IsEncoded { get; set; }
        public string ToEncryptText { get; set; } = string.Empty;
        public string ToDecryptText { get; set; } = string.Empty;
    }

    public class GetEncryptDecryptModel
    {
        public string EncryptResult { get; set; } = string.Empty;
        public string DecryptResult { get; set; } = string.Empty;
    }
}
