﻿using System;
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

	}
}
