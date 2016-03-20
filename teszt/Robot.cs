namespace teszt {
    internal class Robot {
        public Robot(int angleview, int x, int y, int cover, double theta) {
            AngleView = angleview;
            X = x;
            Y = y;
            Cover = cover;
            Theta = theta;
        }

        public double Theta { get; set; }

        public int Cover { get; set; }

        public int Y { get; set; }

        public int X { get; set; }

        public int AngleView { get; set; }
    }
}