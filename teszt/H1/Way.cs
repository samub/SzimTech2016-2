using System;						// Math.Abs
using System.Collections.Generic;	// List
using System.Linq;					// ElementAt
using System.Windows.Media.Imaging;	// BitmapSource
using System.Windows.Media;			// PixelFormats

namespace RobotMover
{
	/// <summary>
	/// Ez az osztály egy bejárást készít a megadott térképen.
	/// </summary>
	class Way
	{

		/// <summary>
		/// Az út pontjai.
		/// </summary>
		public List<PointHOne> Waypoints;
		/// <summary>
		/// Az út hossza.
		/// </summary>
		private double Length;
		/// <summary>
		/// A térkép mátrix.
		/// </summary>
		private int[,] map;
		/// <summary>
		/// A Robot osztály példányának referenciája.
		/// </summary>
		private Robot gui;
		/// <summary>
		/// Az elvárt lefedettség.
		/// </summary>
		private float NeededCoverage;
		/// <summary>
		/// A jelenlegi lefedettség.
		/// </summary>
		private float Coverage;

		/// <summary>
		/// Annak eldöntése, a robot sugara alapján,
		/// hogy hány irányba keressünk új pontot.
		/// </summary>
        /// <param name="r">A sugár.</param>
		/// <returns>Ennyi irányba keressünk új pontot.</returns>
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
		/// nem nagyobb, mint a sugár.
		/// </summary>
        /// <param name="line">A vonal, amin visszalépünk.</param>
        /// <param name="r">A sugár</param>
		/// <returns>A visszalépés után megállapított pont.</returns>
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
		/// Területlefedés a Robot osztály segítségével.
		/// </summary>
		private void Cover() {
			List<PointHOne> line = new List<PointHOne>();
			int i = 0;
			
			if (Waypoints.Count >= 2) {

			// Vonal a két előzőleg meghatározott útpont között
			Auxilary.Bresenham(
				Waypoints.ElementAt(Waypoints.Count-2).x,
				Waypoints.ElementAt(Waypoints.Count-2).y, 
				Waypoints.ElementAt(Waypoints.Count-1).x, 
				Waypoints.ElementAt(Waypoints.Count-1).y,
				ref line
			);
			
			// Területlefedés a vonalon sugár távolságonként
			while (i < line.Count) {
				gui.Reposition(
					line.ElementAt(i).x,
					line.ElementAt(i).y,
					line.ElementAt(i).theta
				);
				i += gui.Radius;
				//gui.SetCurrentlyCoveredArea(map);
			}
			Alg.MapRefresh(false);
			
			// Új pont átadása a robotnak
			i = Waypoints.Count-1;
			gui.Route.Add(new Tuple<int, int, double>(
				Waypoints.ElementAt(i).x, 
				Waypoints.ElementAt(i).y,
				Waypoints.ElementAt(i).theta)
			);
			byte[] pixels = new byte[640*640];
			for (i = 0; i < 640; i++) {
				for (int j = 0; j < 640; j++) {
					pixels[j + i * 640] = (byte) Alg.myIntMap[i,j];
				}
			}
			//BitmapSource MyBitmap = BitmapSource.Create(640, 640, 96, 96, PixelFormats.Pbgra32, null, pixels, 640 * 4);
			Coverage += 0.1f;// TODO: pillanatnyi lefedettség
			}
		}
		

		/// <summary>
		/// Új útpont keresése.
		/// </summary>
		/// <returns>Új pont.</returns>
		private void NewPoint() {
			byte Directions = DirsFromRadius(gui.Radius);
			PointHOne		New = null;     // Az aktuális lehetséges végpont
			PointHOne		Best = null;	// A legjobb végpont
			Path			Path1 = null;	// A legjobb út
			List<PointHOne> line = null;	// A vonal pontjai a visszalépéshez
			int				i;
			
			for (i = 0; i < Directions; i++) {

				// Vonal pontjainak megállapítása a megadott irányban
				line = Auxilary.Bresenham2(
					Waypoints.ElementAt(Waypoints.Count-1),
					360 / Directions * i,
					ref map
				);
				
				// Visszalépés annyit, amennyi a sugár
				New = Back(ref line, gui.Radius);
				
				// A legjobb út kiválasztása
				Path Paths2 = new Path(Waypoints.ElementAt(Waypoints.Count-1),New);
				if (Path1 == null || Path1.Importance < Paths2.Importance) {
					Path1 = Paths2;
					Best = New;
				}

			}

			Length += Path1.Length + Path1.Rotation / 4;	// Úthossz frissítése
			Waypoints.Add(Best);	  // Az új pont hozzáadása az útpontok listájához
			
			// Területlefedés a GUI segítségével, Coverage frissítése!!
			this.Cover();

		}
		
		/// <summary>
		/// Útkeresés.
		/// </summary>
		private void FindWay() {
			int i = 0;
			while (i < 1000 && Coverage < NeededCoverage) {
				NewPoint();
				MessageHandler.Write("\t" + Waypoints[i].x + ", " + Waypoints[i].y);
				++i;
			}
		}


		public Way(float NeededCoverage, ref int[,] map, ref Robot gui) {
			this.Waypoints = new List<PointHOne>();
			this.Waypoints.Add(new PointHOne(gui.X, gui.Y, gui.Theta));	// Kezdőpont hozzáadása
			this.Length = 0;
			this.map = map;
			this.gui = gui;
			this.NeededCoverage = NeededCoverage;
			this.Coverage = 0;
			this.FindWay();
		}

	}
}
