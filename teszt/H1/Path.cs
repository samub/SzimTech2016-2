using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotMover
{
	/// <summary>
	/// Ez az osztály egy lehetséges következő lépés adatait tárolja és
	/// kiszámítja az ahhoz tartozó jósági tényezőt (súlyt).
	/// </summary>
	class Path
	{
		private PointHOne	Start;
		private PointHOne	End;
		/// <summary>
		/// A megtett út/vonal hossza.
		/// </summary>
		public	double		Length { get; private set; }
		/// <summary>
		/// Elfordulás az előző irányhoz képest.
		/// </summary>
		public	double		Rotation { get; private set; }	// A súlyt csökkenti!
		/// <summary>
		/// A vonal pontjainak összsúlya.
		/// </summary>
		private double		Covered;	// A súlyt növeli (ha negatív, akkor csökkenti).
		/// <summary>
		/// A súly mindent figyelembe véve
		/// </summary>
		public	double		Importance { get; private set; }

		public Path(PointHOne Start, PointHOne End) {
			if (Start != null && End != null)
			{
			this.Start = Start;
			this.End = End;
			this.Length = Auxilary.Distance(Start, End);
			this.Rotation = Math.Abs(this.Start.theta - this.End.theta);
			this.Covered = sulyozas(Start.x, Start.y, End.x, End.y);
			this.Importance = Length - Rotation / 4.0 + Covered;
			}
		}

		/// <summary>
		/// Megállapítja, hogy a vonal pontjaira eső területet hányszor fedtük már le,
		/// és ez alapján számol egy súlyt a területlefedéshez.
		/// </summary>
		/// <param name="x1">A kezdőpont x koordinátája.</param>
		/// <param name="y1">A kezdőpont y koordinátája.</param>
		/// <param name="x2">A végpont x koordinátája.</param>
		/// <param name="y2">A végpont y koordinátája.</param>
		/// <returns>A lefedett terület súlya.</returns>
        private double sulyozas(int x1, int y1, int x2, int y2)
        {
            List<PointHOne> pontok = new List<PointHOne>();
            Alg.line(x1, y1, x2, y2, pontok, Alg.myIntMap);

            int nullak = 0;
            int nemNullaParos = 0;
            foreach (var pont in pontok)
            {
                if (pont.ertek == 0) nullak++;
                if ((pont.ertek != 0) && (pont.ertek % 2 == 0)) nemNullaParos += Alg.myIntMap[pont.x, pont.y];
            }

            return nullak - nemNullaParos;
        }

        private void RadiusLepesek(List<PointHOne> origLista, List<PointHOne> newLista, int radius) {
            int db = 0;

            //bejarjuk az eredeti (bresenham altal adott) listankat
            foreach (var l in origLista) {
                //az elso elemet adjuk hozza a listahoz
                if (l == origLista.First()) newLista.Add(l);
                //majd minden X. elemet (X == radius, a koztes elemek nem kerulnek hozzaadasra)
                if ((db % radius) == 0)
                {
                    newLista.Add(l);
                    db = 1;
                }            
                db++;
            }
            //ha az utolso hozzaadott elem nem az utolso, akkor adjuk azt is hozza
            if (newLista.Last() != origLista.Last()) newLista.Add(origLista.Last());
        }
    }
}
