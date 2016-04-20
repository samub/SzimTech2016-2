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

		// A megadott nevű pontosvesszővel elválasztott csv fájlból olvas be adatokat.
		public bool ReadFromFile(String FileName) {
			StreamReader sr;	// Változó a fájl olvasásához
			string line;		// Változó egy sor beolvasásához
			int lines = 1;		// A csv sorainak száma
			int colunms = 1;	// A csv oszlopainak száma
			int i = 0;			// Ciklusváltozó
			int j;				// Ciklusváltozó

			// Fájl megnyitása
			try {
				sr = new StreamReader(FileName);
			} catch (IOException) {
				return false;
			}
			// Oszlopok és sorok számlálása
			line = sr.ReadLine();
			// Az első sorban megszámolja a pontosvesszőket.
			while (i < line.Length) if(line[i++] == ';') ++colunms;
			// A második sortól kezdve megszámolja a sorokat.
			while ((line = sr.ReadLine()) != null) ++lines;
			// Vissza a fájl elejére
			sr.BaseStream.Seek(0, SeekOrigin.Begin); 
			// Helyfoglalás a térkép mátrixához
			Matrix = new int[lines, colunms];
			// Adatok beolvasása a mátrixba
			i = 0;
			line = sr.ReadLine();
			while (i < lines) {
				j = 0;
				while (j < colunms) {
					// Adatok beolvasása a ;-k átugrásával
					Matrix[i, j] = line[j * 2] - '0';
					++j;
				}
				++i;
				line = sr.ReadLine();
			}

			sr.Close();
			return true;
		}
		
		public int GetWidth() { return Matrix.GetLength(1); }
		public int GetHeight() { return Matrix.GetLength(0); }

		// A térkép egy pontjának megjelölse (csak teszteléshez)
		public void NewPoint(int x, int y, int jel) {
			Matrix[y, x] = jel;
		}

		// Teszteléshez
		public override String ToString() {
			StringBuilder s = new StringBuilder();

			for (int i = 0; i < GetHeight(); ++i) {
				for (int j = 0; j < GetWidth(); ++j) {
					if (Matrix[i, j] == 0) {
						s.Insert(s.Length, (char)Empty + " ");
					} else if (Matrix[i, j] == 1) {
						s.Insert(s.Length, (char)Barrirer + " ");
					} else {
						s.Insert(s.Length, (char)Matrix[i, j] + " ");
					}
				}
				s.Insert(s.Length, '\n');
			}
			return s.ToString();
		}

	}
}
