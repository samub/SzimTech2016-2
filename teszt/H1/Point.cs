
namespace heuristic_one
{
	class Point
	{
		public int x { get; private set; }
		public int y { get; private set; }
		public double theta { get; private set; }

		public Point(int x, int y, double theta) {
			this.x = x;
			this.y = y;
			this.theta = theta;
		}

	}
}
