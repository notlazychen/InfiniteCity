using System;
using System.Collections.Generic;
using System.Text;

namespace IfCastle.Grain.Blocks
{
    public enum BlockTypes
    {
        I,
        J,
        L,
        O,
        S,
        Z,
        T
    }

    public interface IBlock
    {
        IEnumerable<(int X, int Y)> Shape();
        BlockTypes Type { get; }
    }

    public abstract class BlockBase : IBlock
    {
        public abstract IEnumerable<(int X, int Y)> Shape();
        public abstract BlockTypes Type { get; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Orientation { get { return _Orientation; } set { _Orientation = value; } }

        protected int _Orientation;
        protected abstract int RotateOrientations { get; }

        public void Rotate()
        {
            _Orientation = (_Orientation + 1) % RotateOrientations;
        }
    }
}
