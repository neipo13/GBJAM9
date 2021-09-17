using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Input
{
    public class InputMapping
    {
        public int index { get; set; }
        public int[] Left { get; set; }
        public int[] Right { get; set; }
        public int[] Up { get; set; }
        public int[] Down { get; set; }
        public int[] AKey { get; set; }
        public int[] AButton { get; set; }
        public int[] BKey { get; set; }
        public int[] BButton { get; set; }
        public int[] StartKey { get; set; }
        public int[] StartButton { get; set; }
        public int[] SelectKey { get; set; }
        public int[] SelectButton { get; set; }
    }
}
