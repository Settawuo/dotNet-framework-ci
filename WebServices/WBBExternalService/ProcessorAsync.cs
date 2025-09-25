using WBBContract;

namespace WBBExternalService
{
    internal class ProcessorAsync
    {
        public IQueryProcessor queryProcessor { get; set; }
        public IQueryProcessorAsync queryProcessorAsync { get; set; }
    }
}