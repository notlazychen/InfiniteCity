using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Grain.States
{
    public class PlayerState
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
    }
}
