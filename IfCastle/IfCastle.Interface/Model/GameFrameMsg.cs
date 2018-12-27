using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Interface.Model
{
    [Serializable]
    public class GameFrameMsg
    {
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public string Text { get; set; }


        public GameFrameMsg()
        {

        }

        public GameFrameMsg(string msg)
        {
            Text = msg;
        }
    }
}
