using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teszt
{
    class Coords
    {
        int x;
        int y;
        public Coords(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public int GetX() {
            return x;
        }
        public int GetY()
        {
            return y;
        }
    }
}
