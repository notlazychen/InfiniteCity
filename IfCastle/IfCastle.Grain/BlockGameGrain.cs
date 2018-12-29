using IfCastle.Grain.Blocks;
using IfCastle.Grains;
using IfCastle.Interface;
using IfCastle.Interface.Model;
using IfCastle.Interface.ObServers;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfCastle.Grain
{
    public class BlockGameGrain : Grain<BlockTable>, IBlockGame
    {
        private IAsyncStream<GameFrameMsg> _stream;

        public override Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.GameRoomStreamProvider);
            _stream = streamProvider.GetStream<GameFrameMsg>(this.GetPrimaryKey(), Constants.GameRoomStreamNameSpace);
            return base.OnActivateAsync();
        }

        public Task<Guid> Start()
        {
            var state = this.State;
            if(!state.IsBuild)
            {
                state.Build(10, 20);
                this.RegisterTimer(OnFrameAsync, state, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(500));
            }
            return Task.FromResult(_stream.Guid);
        }

        private async Task OnFrameAsync(object state)
        {
            var cells = this.State.Cells;

            //擦除当前的方块
            State.ClearBlock();

            //检查游戏结束
            bool isGameOver = false;
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                if (cells[x, 0].IsFill)
                {
                    isGameOver = true;
                    break;
                }
            }
            if (isGameOver)
            {
                //_obss.Notify(client => client.ReceiveMessage("Game Over!"));
                await _stream.OnNextAsync(new GameFrameMsg("Game Over!"));
                return;
            }

            //检查消除
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                bool allfill = true;
                for (int x = 0; x < cells.GetLength(0); x++)
                {
                    if (!cells[x, y].IsFill)
                    {
                        allfill = false;
                        break;
                    }
                }
                if (allfill)
                {
                    //生成一行新的
                    for (int x = 0; x < cells.GetLength(0); x++)
                    {
                        var cell0 = cells[x, 0];
                        cell0.IsFill = false;
                        cell0.Color = ConsoleColor.White;
                    }
                    //上面的行全部下移
                    for (int cury = y; cury > 0; cury--)
                    {
                        for (int x = 0; x < cells.GetLength(0); x++)
                        {
                            var cucell = cells[x, cury];
                            var upcell = cells[x, cury - 1];
                            cucell.IsFill = upcell.IsFill;
                            cucell.Color = upcell.Color;
                        }
                    }
                }
            }

            //方块碰撞检测，如果到底取消方块
            if (this.State.Block != null)
            {
                ///检查是否触底，触底则放弃方块
                var maxY = State.Block.Shape().Where(s => s.X >= 0 && s.Y >= 0 && s.X < this.State.Width && s.Y < this.State.Height)
                    .Max(x => x.Y);
                if (State.Block.Shape().Where(s => s.X >= 0 && s.Y >= 0 && s.X < this.State.Width && s.Y < this.State.Height).Where(s => s.Y == maxY)
                    .Any(s => s.Y + 1 == State.Height || this.State.Cells[s.X, s.Y + 1].IsFill))
                {
                    this.State.PlaceBlock();
                    this.State.Block = null;
                }
            }
            //如果没有的话就生成一只新的
            if (this.State.Block == null)
            {
                var rand = new Random(DateTime.Now.Second);
                int i = rand.Next(0, this.State.Blocks.Count);
                this.State.Block = this.State.Blocks[i];
                this.State.Block.X = this.State.Width / 2;
                this.State.Block.Y = 0;
            }

            //方块往下走一
            this.State.Block.Y += 1;
            //渲染方块
            this.State.PlaceBlock();

            //通知前端当前帧状态
            var sb = this.State.Print();
            await _stream.OnNextAsync(new GameFrameMsg($"[{this.GetPrimaryKey()}]" + sb));
            //_obss.Notify(client => client.ReceiveMessage(sb));
            //return Task.CompletedTask;
        }

        public async Task Move(Direction direction)
        {
            if (this.State.Block != null)
            {
                this.State.ClearBlock();
                int x = this.State.Block.X;
                int y = this.State.Block.Y;
                int rotate = this.State.Block.Orientation;
                switch (direction)
                {
                    case Direction.Up:
                        this.State.Block.Rotate();
                        break;
                    case Direction.Right:
                        this.State.Block.X += 1;
                        break;
                    case Direction.Down:
                        this.State.Block.Y += 1;
                        //for(int i = y; i< this.State.Height; i++)
                        //{
                        //    if(this.State.Cells[x, i].IsFill)
                        //    {
                        //        break;
                        //    }
                        //    this.State.Block.Y = i;
                        //}
                        break;
                    case Direction.Left:
                        this.State.Block.X -= 1;
                        break;
                }
                //边界判断
                bool touch = this.State.Block.Shape().Any(s => s.X < 0 || s.X >= this.State.Width || s.Y >= this.State.Height
                    || (s.Y>=0 && this.State.Cells[s.X, s.Y].IsFill));
                if (touch)
                {
                    this.State.Block.X = x;
                    this.State.Block.Y = y;
                    this.State.Block.Orientation = rotate;
                }
                //渲染方块
                this.State.PlaceBlock();
                var sb = this.State.Print();
                //_obss.Notify(client => client.ReceiveMessage(sb));
                await _stream.OnNextAsync(new GameFrameMsg(sb));
            }
        }

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Resume()
        {
            throw new NotImplementedException();
        }
    }

    public class BlockTable
    {
        public readonly IList<BlockBase> Blocks = new List<BlockBase> {
            new BlockI(),
            new BlockJ(),
            new BlockL(),
            new BlockT(),
            new BlockZ(),
            new BlockS(),
            new BlockO(),
        };

        public bool IsBuild { get; private set; } = false;
        public int Width { get; private set; }
        public int Height { get; private set; }
        /// <summary>
        /// 一维是行数Y，二维是列数X
        /// </summary>
        public Cell[,] Cells { get; private set; }

        public BlockBase Block { get; set; }

        public void Build(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[width, height];
            for (int x = 0; x < Cells.GetLength(0); x++)
            {
                for (int y = 0; y < Cells.GetLength(1); y++)
                {
                    var cell = new Cell(x, y);
                    Cells[x, y] = cell;
                }
            }
        }

        /// <summary>
        /// 擦除当前的方块
        /// </summary>
        public void ClearBlock()
        {
            if (this.Block != null)
            {
                foreach (var s in Block.Shape())
                {
                    if (s.X >= 0 && s.Y >= 0 && s.X < this.Width && s.Y < this.Height)
                    {
                        this.Cells[s.X, s.Y].IsFill = false;
                    }
                }
            }
        }

        /// <summary>
        /// 渲染方块
        /// </summary>
        public void PlaceBlock()
        {
            foreach (var s in this.Block.Shape())
            {
                if (s.X >= 0 && s.Y >= 0 && s.X < this.Width && s.Y < this.Height)
                {
                    this.Cells[s.X, s.Y].IsFill = true;
                }
            }
        }

        public string Print()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"------[{DateTime.UtcNow.ToString("HH:mm:ss")}]------");
            for (int y = 0; y < this.Cells.GetLength(1); y++)
            {
                sb.Append("|");
                for (int x = 0; x < this.Cells.GetLength(0); x++)
                {
                    var cell = this.Cells[x, y];
                    sb.AppendFormat("{0}|", cell.ToString());
                }
                sb.AppendLine();
            }
            sb.AppendLine("-----------------------------");
            return sb.ToString();
        }
    }

    public class Cell
    {
        public ConsoleColor Color { get; set; }
        public bool IsFill { get; set; } = false;

        public int X { get; }
        public int Y { get; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return this.IsFill ? "+" : " ";
        }
    }
}
