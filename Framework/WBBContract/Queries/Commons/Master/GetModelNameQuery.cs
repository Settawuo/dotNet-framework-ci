namespace WBBContract.Queries.Commons.Master
{
    public class GetModelNameQuery : IQuery<bool>
    {

        public string ModelName { get; set; }
        public string ResultModel { get; set; }
        public bool ResultBI { get; set; }
    }
}
