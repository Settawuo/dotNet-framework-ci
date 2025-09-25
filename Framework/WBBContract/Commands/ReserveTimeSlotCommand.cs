using System;

namespace WBBContract.Commands
{
    public class ReserveTimeSlotCommand
    {
        public ReserveTimeSlotCommand()
        {
            this.Return_Code = -1;
        }

        public Guid IdReserve { get; set; }
        public decimal TimeSlotId { get; set; }
        public string ReserveDTM { get; set; }

        //for return
        public int Return_Code { get; set; }
        public string Return_Message { get; set; }
    }
}
