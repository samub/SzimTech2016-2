﻿using System;						// Math.Abs
using System.Collections.Generic;	// List
using System.Linq;					// ElementAt
using System.Windows.Media.Imaging;	// BitmapSource
using System.Windows.Media;         // PixelFormats
using System.Threading.Tasks;

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
		/// A lefedett terület arányát adja eredményül.
		/// </summary>
		/// <returns>0 és 1 közé eső lebegőpontos szám.</returns>
		private float CurrentCoverage() {
			long barrier = 0;
			long covered = 0;

			for (int i = 0; i < 640; i++) {
				for (int j = 0; j < 640; j++) {
					if (Alg.myIntMap[i, j] > 0) {
						if (Alg.myIntMap[i, j] % 2 == 1)
						    barrier += 1;
						else
							covered += 1;
					}
				}
			}
            float ret_value = (float)covered / (640 * 640 - barrier);
            return ret_value;
		}

		/// <summary>
		/// Területlefedés a Robot osztály segítségével.
		/// </summary>
		private void Cover() {
            List<Tuple<int, int, double>> OurOwnRoute = new List<Tuple<int, int, double>>();
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
                    
                    //robot mozgatása pontonként:

                    gui.Route.Add(new Tuple<int, int, double> (line.ElementAt(i).x, line.ElementAt(i).y, line.ElementAt(i).theta));
                    OurOwnRoute.Add(new Tuple<int, int, double>(line.ElementAt(i).x, line.ElementAt(i).y, line.ElementAt(i).theta));
                    MessageHandler.Write(" [" + line.ElementAt(i).x + ", " + line.ElementAt(i).y + "] CC:" + CurrentCoverage());

					i += gui.Radius;

                    gui.ExecuteRobot();

                    // Terület súlyozása
                    for (int j = 0; j < gui.CurrentlyCoveredArea.Count; ++j)
                    {
                        int a = gui.CurrentlyCoveredArea[j].Item1;
                        int b = gui.CurrentlyCoveredArea[j].Item2;
                        Alg.myIntMap[a, b] += 2;
                    }

                    gui.Route.Clear();
                }
                // A Route-ba berakjuk a bejárt pontokat:
                foreach (var p in OurOwnRoute) gui.Route.Add(new Tuple<int, int, double>(p.Item1, p.Item2, p.Item3));
                
                // A robottal bejárjuk a területet (most már egyben is)
                gui.ExecuteRobot();

                // Pillanatnyi lefedettség
                Coverage += 0.1f;//CurrentCoverage();
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
			Waypoints.Add(Best);      // Az új pont hozzáadása az útpontok listájához

            // Területlefedés a GUI segítségével, Coverage frissítése!!
            this.Cover();
		}
		
		/// <summary>
		/// Útkeresés.
		/// </summary>
		private void FindWay() {
			int i = 0;
			while (i < 100 && Coverage < gui.Cover / 100.0) {
				NewPoint();
                ++i;
            }
		}


		public Way(ref int[,] map, ref Robot gui) {
			this.Waypoints = new List<PointHOne>();
			this.Waypoints.Add(new PointHOne(gui.X, gui.Y, gui.Theta));	// Kezdőpont hozzáadása
			this.Length = 0;
			this.map = map;
			this.gui = gui;
			this.Coverage = 0;
			this.FindWay();
		}

	}
}
