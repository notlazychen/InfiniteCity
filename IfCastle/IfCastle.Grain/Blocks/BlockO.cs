using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Grain.Blocks
{
    public class BlockO : BlockBase
    {
        public override BlockTypes Type => BlockTypes.O;

        protected override int RotateOrientations => 1;

        public override IEnumerable<(int X, int Y)> Shape()
        {
            switch (_Orientation)
            {
                // **
                // @*
                case 0:
                    yield return (X, Y - 1);
                    yield return (X, Y);
                    yield return (X + 1, Y - 1);
                    yield return (X + 1, Y);
                    break;
            }
        }
    }
}
