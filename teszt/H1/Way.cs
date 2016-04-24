using System.Collections.Generic;	// List
using System.Linq;					// ElementAt

namespace RobotMover
{
	/// <summary>
	/// Ez az osztály egy bejárást készít a megadott térképen.
	/// </summary>
	class Way
	{

		public List<PointHOne>	Waypoints;
		private double		Length;
		private int[,]		map;
		private Robot			gui;
		private float		NeededCoverage;	// 0.8
		public byte Dirs;

		/// <summary>
		/// Annak eldöntése, a robot sugara alapján,
		/// hogy hány irányba keressünk új pontot
		/// </summary>
        /// <param name="r">A sugár</param>
		/// <returns>Ennyi irányba keressünk új pontot</returns>
		private byte DirsFromRadius(int r) {
			// Nagyon kicsi sugár
			if (r <= 10) {
				return 36;
			// Közepes sugár
			} else if (r <= 50) {
				return 12;
			// Nagy sugár
			} else {
				return 8;
			}
		}

		/// <summary>
		/// Visszalépés a vonalon amíg a végponttól levő távolság 
		/// nem nagyobb, mint a sugár
		/// </summary>
        /// <param name="line">A vonal, amin visszalépünk</param>
        /// <param name="r">A sugár</param>
		/// <returns>A visszalépés után megállapított pont</returns>
		private PointHOne Back(ref List<PointHOne> line, int r) {
			int i;
			double Distance;

			if (line == null) return null;
			i = line.Count - 1;
			Distance = Auxilary.Distance(line.ElementAt(line.Count - 1), line.ElementAt(i));
			while (i > 0 && Distance < r) {
				--i;
				Distance = Auxilary.Distance(line.ElementAt(line.Count - 1), line.ElementAt(i));
			}
			return line.ElementAt(i);
		}

		/// <summary>
		/// Új útpont keresése
		/// </summary>
		/// <returns>Új pont</returns>
		private PointHOne NewPoint() {
			byte Directions = this.Dirs;//DirsFromRadius(gui.Radius);
			PointHOne New = null;						// Azaktuális lehetséges végpont
			List<PointHOne> Points = new List<PointHOne>();	// A lehetséges végpontok listája
			List<Path> Paths = new List<Path>();	// A lehetséges utak listája

			for (int i = 0; i < Directions; i++) {
				// Vonal húzása
				List<PointHOne> line = Auxilary.Bresenham2(
					Waypoints.ElementAt(Waypoints.Count-1),
					360 / Directions * i,
					ref map
				);
				for (int j = 0; j < line.Count; j++) {
					map[line.ElementAt(j).y, line.ElementAt(j).x] = 'x';
				}
				// Visszalépés annyit, amennyi a sugár
				New = Back(ref line, gui.Radius);
				Points.Add(New);
				// Területlefedés a GUI segítségével
				;
				// Az újonnan lefedett terület súlya
				int count = 0;
				for (int j = 0; j < gui.CurrentlyCoveredArea.Count; j++) {
					int x = gui.CurrentlyCoveredArea.ElementAt(i).Item1;
					int y = gui.CurrentlyCoveredArea.ElementAt(i).Item2;
					count += map[y, x];
				}
				Paths.Add(new Path(Waypoints.ElementAt(Waypoints.Count-1),New));
			}
			// A legjobb út kiválasztása
			;
			// Távolság megállapítása, úthossz frissítése
			;
			// Elfordulás megállapítása, úthossz frissítése
			;
			return New;
		}
		
		public void FindWay() {
			//while (Robot.Cover < (NeededCoverage)) {
				NewPoint();
			//}
		}

		public Way(float NeededCoverage, ref int[,] map, ref Robot gui) {
			this.Waypoints = new List<PointHOne>();
			this.Waypoints.Add(new PointHOne(gui.X, gui.Y, gui.Theta));	// Kezdőpont hozzáadása
			this.Length = 0;
			this.map = map;
			this.NeededCoverage = NeededCoverage;
			this.gui = gui;
			//this.FindWay();
		}

	}
}
