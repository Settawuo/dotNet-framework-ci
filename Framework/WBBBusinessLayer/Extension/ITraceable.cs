namespace WBBBusinessLayer.Extension
{
    public interface ITraceable
    {
        bool IsTraceRequestEnabled { get; set; }
        bool IsTraceResponseEnabled { get; set; }
        string ComponentName { get; set; }
    }
}
