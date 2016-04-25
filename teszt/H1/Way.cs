using System;						// Math.Abs
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
		private double			Length;
		private int[,]			map;
		private Robot			gui;
		private float			NeededCoverage;

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
		private void NewPoint() {
			byte Directions = DirsFromRadius(gui.Radius);
			PointHOne	New = null;		// Az aktuális lehetséges végpont
			Path		Paths = null;	// A legjobb út

			for (int i = 0; i < Directions; i++) {

				// Vonal pontjainak megállapítása a megadott irányokban
				List<PointHOne> line = Auxilary.Bresenham2(
					Waypoints.ElementAt(Waypoints.Count-1),
					360 / Directions * i,
					ref map
				);

				// Visszalépés annyit, amennyi a sugár
				New = Back(ref line, gui.Radius);
				
				// A legjobb út kiválasztása
				Path Paths2 = new Path(Waypoints.ElementAt(Waypoints.Count-1),New);
				if (Paths == null || Paths.Importance < Paths2.Importance) {
					Paths = Paths2;
				}
			}
			
			// Távolság megállapítása, úthossz frissítése
			Length += Auxilary.Distance(Waypoints.ElementAt(Waypoints.Count-1),New);

			// Elfordulás megállapítása, úthossz frissítése
			Length += Math.Abs(Waypoints.ElementAt(Waypoints.Count-1).theta - New.theta) / 4;

			// Új pont hozzáadása az útpontok listájához
			Waypoints.Add(New);
			
			// Területlefedés a GUI segítségével, Cover frissítése
			;

		}
		
		private void FindWay() {
			//while (Robot.Cover < (NeededCoverage)) {
				NewPoint();
			//}
		}

		// Az út pontjainak visszaadása
		private void UpdateGUI() {
			for (int i = 0; i < Waypoints.Count; i++) {
				gui.Route.Add(new Tuple<int, int, double>(
					Waypoints.ElementAt(i).x, 
					Waypoints.ElementAt(i).y,
					Waypoints.ElementAt(i).theta));
			}
		}

		public Way(float NeededCoverage, ref int[,] map, ref Robot gui) {
			this.Waypoints = new List<PointHOne>();
			this.Waypoints.Add(new PointHOne(gui.X, gui.Y, 0, gui.Theta));	// Kezdőpont hozzáadása
			this.Length = 0;
			this.map = map;
			this.NeededCoverage = NeededCoverage;
			this.gui = gui;
			//this.FindWay();
			//UpDateGUI();
		}

	}
}
