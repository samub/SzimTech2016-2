
namespace RobotMover
{
	class PointHOne
	{
		public int x { get; private set; }
		public int y { get; private set; }
        public int ertek { get; private set; }
        public double theta { get; private set; }

		public PointHOne(int x, int y, int ertek, double theta) {
			this.x = x;
			this.y = y;
			this.theta = theta;
            this.ertek = ertek;
		}

	}
}
