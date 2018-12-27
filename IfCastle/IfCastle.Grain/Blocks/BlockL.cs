using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Grain.Blocks
{
    public class BlockL : BlockBase
    {
        public override BlockTypes Type => BlockTypes.L;

        protected override int RotateOrientations => 4;

        public override IEnumerable<(int X, int Y)> Shape()
        {
            switch (_Orientation)
            {
                case 0:
                    yield return (X, Y - 1);
                    yield return (X, Y - 2);
                    yield return (X, Y);
                    yield return (X + 1, Y);
                    break;
                case 1:
                    yield return (X + 1, Y);
                    yield return (X + 2, Y);
                    yield return (X, Y);
                    yield return (X, Y + 1);
                    break;
                case 2:
                    yield return (X - 1, Y);
                    yield return (X, Y);
                    yield return (X, Y + 1);
                    yield return (X, Y + 2);
                    break;
                case 3:
                    yield return (X, Y - 1);
                    yield return (X, Y);
                    yield return (X - 1, Y);
                    yield return (X - 2, Y);
                    break;
            }
        }
    }
}
