using WBBEntity.Minions;

namespace WBBContract.Minions
{
    public class MinionGetSaveOrderNew : IQuery<MinionGetSaveOrderNewModel>
    {
        public string UrlEnpoint { get; set; }
        public string RequestData { get; set; }
    }
}