using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotMover
{
	/// <summary>
	/// Ez az osztály a lehetséges utak adatait tárolja és
	/// kiszámítja az ahhoz tartozó jósági tényezőt (súlyt).
	/// </summary>
	class Path
	{
		private PointHOne	Start;
		private PointHOne	End;
		public double		Length;
		public double		Rotation;
		//private double	Újonnan lefedett terület súlya
		public double		Importance;

		public Path(PointHOne Start, PointHOne End) {
			if (Start != null && End != null)
			{
			this.Start = Start;
			this.End = End;
			this.Length = Auxilary.Distance(Start, End);
			this.Rotation = Math.Abs(this.Start.theta - this.End.theta);
			this.Importance = I();
			}
		}

		private double I() {
			double res;

			res = this.Length + this.Rotation / 4.0;    // Hossz + elfordulás/4

			//res += Újonnan lefedett terület súlya

			return res;
		}

        public double sulyozas(int x1, int y1, int x2, int y2, int theta)
        {
            double d = Math.Sqrt(Math.Pow(Math.Abs(x2 - x1), 2) + Math.Pow(Math.Abs(y2 - y1), 2));
            List<PointHOne> pontok = new List<PointHOne>();

            Alg.line(x1, y1, x2, y2, pontok, Alg.myIntMap);

            int nullak = 0;
            int nemNullaParos = 0;
            foreach (var pont in pontok)
            {
                if (pont.ertek == 0) nullak++;
                if ((pont.ertek != 0) && (pont.ertek % 2 == 0)) nemNullaParos += Alg.myIntMap[pont.x, pont.y];
            }

            return d + (theta / 4) + nullak - nemNullaParos;
        }

    }
}
