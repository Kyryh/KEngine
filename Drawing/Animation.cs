using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Drawing {
    public class Animation {
        public string Name { get; }
        public string NextAnimation { get; }
        public int Frames { get; }

        private readonly Frame[] frames;
        public Frame this[int index] => frames[index];
        
        public Animation(string name, string nextAnimation, params Frame[] frames) {
            Name = name;
            NextAnimation = nextAnimation ?? name;
            this.frames = (Frame[])frames.Clone();
            Frames = this.frames.Length;
        }

        public Animation(string name, params Frame[] frames) : this(name, null, frames) { }


        public record Frame(int SpriteSheetIndex, TimeSpan Duration);
    }
}
