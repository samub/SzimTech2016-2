namespace teszt {
    internal class Robot {
        public Robot(int angleview, int x, int y, int cover) {
            AngleViwe = angleview;
            X = x;
            Y = y;
            Cover = cover;
        }

        public int Cover { get; set; }

        public int Y { get; set; }

        public int X { get; set; }

        public int AngleViwe { get; set; }
    }
}