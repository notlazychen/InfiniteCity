using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Grain.Blocks
{
    public class BlockJ : BlockBase
    {
        public override BlockTypes Type => BlockTypes.J;

        protected override int RotateOrientations => 4;

        public override IEnumerable<(int X, int Y)> Shape()
        {
            switch (_Orientation)
            {
                //  *
                //  x
                // **
                case 0:
                    yield return (X - 1, Y + 1);
                    yield return (X, Y + 1);
                    yield return (X, Y);
                    yield return (X, Y - 1);
                    break;
                // *
                // *x*
                // 
                case 1:
                    yield return (X - 1, Y -1);
                    yield return (X - 1, Y);
                    yield return (X, Y);
                    yield return (X + 1, Y);
                    break;
                //  **
                //  x
                //  *
                case 2:
                    yield return (X + 1, Y - 1);
                    yield return (X, Y - 1);
                    yield return (X, Y);
                    yield return (X, Y + 1);
                    break;
                //  
                // *x*
                //   *
                case 3:
                    yield return (X - 1, Y);
                    yield return (X, Y);
                    yield return (X + 1, Y);
                    yield return (X + 1 , Y + 1);
                    break;
            }
        }
    }
}
