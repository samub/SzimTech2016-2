using System;						// String, Math
using System.Collections.Generic;	// List<>
using System.Linq;					// ElementAt
using System.Text;					// StringBuilder
using System.IO;					// StreamReader, SeekOrigin, IOException

namespace RobotMover
{
	class Map
	{
		public int[,]	Matrix;
		private int		Empty;		// Csak teszteléshez
		private int		Barrirer;   // Csak teszteléshez

		private List<Tuple<int,int,double>> Way = new List<Tuple<int,int,double>>();

		public Map(int Empty, int Barrirer) {
			this.Empty = Empty;
			this.Barrirer = Barrirer;
		}
		
		public int GetWidth() { return Matrix.GetLength(1); }
		public int GetHeight() { return Matrix.GetLength(0); }

	}
}
