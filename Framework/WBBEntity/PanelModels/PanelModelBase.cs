using System.Diagnostics;

namespace WBBEntity.PanelModels
{
    [DebuggerDisplay("ModelState={ModelState}")]
    public class PanelModelBase
    {
        public ModelState ModelState { get; set; }
    }

    public enum ModelState
    {
        None = 0,
        Insert = 1,
        Update = 2,
        Delete = 3,
    }
}
