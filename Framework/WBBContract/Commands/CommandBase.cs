using System;

namespace WBBContract.Commands
{
    public enum ActionType
    {
        None = 0,
        Insert = 1,
        Update = 2,
        Delete = 3,
        UpdateImage = 4
    }

    public class CommandBase
    {
        public ActionType ActionType { get; set; }

        public string ActionBy { get; set; }

        public DateTime ActionDate { get; set; }
    }
}
