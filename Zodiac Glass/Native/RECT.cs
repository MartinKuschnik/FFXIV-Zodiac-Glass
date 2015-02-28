namespace ZodiacGlass.Native
{
    using System.Diagnostics;

    public struct RECT
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int left;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int top;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int right;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int bottom;
        
        public int Left
        {
            get 
            {
                return this.left; 
            }
            set 
            {
                this.left = value; 
            }
        }

        public int Top
        {
            get
            {
                return this.top;
            }
            set
            {
                this.top = value;
            }
        }

        public int Right
        {
            get
            {
                return this.right;
            }
            set
            {
                this.right = value;
            }
        }

        public int Bottom
        {
            get
            {
                return this.bottom;
            }
            set
            {
                this.bottom = value;
            }
        }
        
        public int Width
        {
            get
            {
                return this.Right - this.Left;
            }
        }

        public int Height
        {
            get
            {
                return this.Bottom - this.Top;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.Left >= this.Right || this.Top >= this.Bottom;
            }
        }

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public void Offset(int dx, int dy)
        {
            this.Left += dx;
            this.Top += dy;
            this.Right += dx;
            this.Bottom += dy;
        }
    }
}
