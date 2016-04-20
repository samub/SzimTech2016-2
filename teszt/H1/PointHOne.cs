
namespace RobotMover
{
	class PointHOne
	{
		public int x { get; private set; }
		public int y { get; private set; }
		public double theta { get; private set; }

		public PointHOne(int x, int y, double theta) {
			this.x = x;
			this.y = y;
			this.theta = theta;
		}

	}
}
