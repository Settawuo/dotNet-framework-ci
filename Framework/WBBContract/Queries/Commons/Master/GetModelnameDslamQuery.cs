namespace WBBContract.Queries.Commons.Master
{
    public class GetModelnameDslamQuery : IQuery<bool>
    {
        public string ModelName { get; set; }
        public string ResultNameCommandDslam { get; set; }
    }
}
