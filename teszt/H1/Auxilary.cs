using System;						// Math.Abs, Math.Sqrt, Math.Atan, Math.PI
using System.Collections.Generic;	// List

namespace RobotMover
{
	class Auxilary
	{

		public static void Bresenham(int x, int y, int x2, int y2, ref List<PointHOne> list)
		{
			//x and y represents the starting postition of the line
			//x2 and y2 represents the ending postition of the line
			int xOriginal = x;
			int yOriginal = y;
            
			int w = x2 - x; // Width
			int h = y2 - y; // Height
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
			if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;   // Left or right
			if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;   // Up or down
			if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;   // Left or right
			int longest = Math.Abs(w);
			int shortest = Math.Abs(h);
			if (!(longest > shortest))
			{
				longest = Math.Abs(h);
				shortest = Math.Abs(w);
				if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
				dx2 = 0;
			}
			int numerator = longest >> 1;
			for (int i = 0; i <= longest; i++)
			{
				list.Add(new PointHOne(x, y, 0.0));


				numerator += shortest;
				if (!(numerator < longest))
				{
					numerator -= longest;
					x += dx1;
					y += dy1;
				}
				else
				{
					x += dx2;
					y += dy2;
				}
			}
		}

		/// <summary>
		/// Vonal húzása egy kezdőpontból a megadott irányba
		/// akadályig vagy a térkép széléig.
		/// A vonal pontjainak listáját adja vissza.
		/// </summary>	
		public static List<PointHOne> Bresenham2(PointHOne Start, double Direction, ref int[,] Matrix)
		{
			if (Start == null || Matrix == null) return null;
			List<PointHOne> L = new List<PointHOne>();
			int x = Start.x;
			int y = Start.y;
			int dx, dy;
			double St, Steepness;
			
			dy = 0; // Vízszintes vonal // Horizontal line
			dx = 0; // Függőleges vonal // Vertical line
			
			// Jobbra vagy balra			// Right or left
			if (Direction < 90 || Direction > 270) dx = 1;
			else if (Direction > 90 && Direction < 270) dx = -1;

			// Fel vagy le			// Up or down
			if (Direction < 180) dy = -1;
			else if (Direction > 180) dy = 1;
			
			//	90 vagy 270 fokra nem értelmezett a tangens			// 90 or 270 degrees are invalid on a tangent
			if(Direction == 90 || Direction == 270) {
				Steepness = 1;
			} else {
				Steepness = Math.Abs(Math.Tan(Direction / 180.0 * Math.PI));
			}
			
			// Ha a meredekség abszolútértéke legfeljebb 1, akkor
			// az x értéke lesz mindig egész és az y mindig meredekségnyivel változik,
			// egyébként az y lesz mindig egész és az x változik mindig (1 / meredekség)-nyivel 
			if (Steepness <= 1) {
				St = y + (dy * Steepness);
				x += dx;
				y += (int)Math.Round(dy * Steepness);
			} else {
				St = x + (dx / Steepness);
				x += (int)Math.Round(dx / Steepness);
				y += dy;
			}

			// Vonal húzása akadályig vagy a térkép széléig
			while (
				   x >= 0
				&& y >= 0
				&& x < Matrix.GetLength(1)
				&& y < Matrix.GetLength(0)
				&& Matrix[y, x] != 1
			) {
				L.Add(new PointHOne(x, y, Direction));

				if (Steepness <= 1) {
					x += dx;
					St += dy * Steepness;
					y = (int)Math.Round(St);
				} else {
					St += dx / Steepness;
					x = (int)Math.Round(St);
					y += dy;
				}
			}

			if (L.Count == 0) {
				return null;
			} else {
				return L;
			}
		}

		public static double Distance(PointHOne p1, PointHOne p2) {
			if (p1 == null || p2 == null) 
				return 0.0;
			return Math.Sqrt( 
				(double)(p1.x - p2.x) * (p1.x - p2.x) + 
				(p1.y - p2.y) * (p1.y - p2.y) 
			);
		}

		private static double Atan(double tg) {
			return Math.Atan(tg) * 180.0 / Math.PI;
		}

		public static double Angle(PointHOne p1, PointHOne p2) {
			int xDist = p2.x - p1.x;
			int yDist = p2.y - p1.y;
			
			// Hibaellenőrzés
			if (p1 == null || p2 == null) return 0.0;
			// A két pont egybe esik
			if(xDist == 0 && yDist == 0) return 360;
			// Függőleges vonal
			if(xDist == 0) {
				if(p2.y > p1.y) {
					return 270.0;
				} else {
					return 90.0;
				}
			}
			// A végpont jobbra van
			else // A végpont jobbra van
			if(xDist >= 0) {
				// A végpont feljebb van (0° - 90°)
				if(yDist > 0) {
					return 360 - Atan(yDist/xDist);
				// A végpont lejjebb van (270° - 360°)
				} else {
					return 0 - Atan(yDist/xDist);
				}
			// A végpont balra van (90° - 180°)
			} else {
				return 180 - Atan(yDist/xDist);
			}
		}

	}
}
