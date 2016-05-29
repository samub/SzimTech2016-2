namespace RobotMover {
    internal class Mapcover {
        private const int m_size = 640;
        private int cover;
        private bool[,] fcmap;
        private int freearea;
        public double[,] fullmap;
        private int number;
        private double[,] optmap;
        private int radius;
        private int sx;
        private int sy;

        public void setStart(int x, int y) {
            sx = y;
            sy = x;
            MessageHandler.Write("koordinatak:" + sx + sy);
        }

        public void setRadius(int rad) {
            radius = rad;
        }

        public void setCover(int cov) {
            cover = cov;
        }

        public int getNumber() {
            return number;
        }

        public void fullMapCover() {
            fcmap = new bool[m_size, m_size];
            MessageHandler.Write("Középpontok lehelyezése.");
            fcmap[sx, sy] = true;
            MessageHandler.Write("Kezdőpontok: X:" + sx + " Y" + sy);
            for (var i = radius; i < m_size; i += 2 * radius) {
                for (var j = radius; j < m_size; j += 2 * radius) { fcmap[i, j] = true; }
            }
            MessageHandler.Write("Középpontok lehelyezése, megtörtént.");
        }

        public bool[,] obstacleCover(bool[,] obst) {
            MessageHandler.Write("Akadályoknál lévő középpontok eltávolítása.");

            for (var i = 0; i < m_size; i++) {
                for (var j = 0; j < m_size; j++) {
                    if (obst[i, j] == fcmap[i, j]) { fcmap[i, j] = false; }
                }
            }

            MessageHandler.Write("Akadályoknál lévő középpontok eltávolítása, megtörtént.");
            return fcmap;
        }

        public double[,] createMap(bool[,] obst) {
            optmap = new double[m_size, m_size];
            MessageHandler.Write("Lefedési térkép létrehozása");
            for (var i = 0; i < m_size; i++) {
                for (var j = 0; j < m_size; j++) {
                    if (fcmap[i, j]) {
                        for (var k = i - radius; (k < m_size) && (k <= i + radius); k++) {
                            for (var l = j - radius; (l < m_size) && (l <= j + radius); l++) { optmap[k, l] = 0.5; }
                        }
                    }
                }
            }
            for (var i = 0; i < m_size; i++) {
                for (var j = 0; j < m_size; j++) {
                    if (obst[i, j]) { optmap[i, j] = 1; }
                }
            }
            MessageHandler.Write("Lefedési térkép létrehozása, megtörtént.");
            return optmap;
        }

        public double isTheMapOptimized() {
            var coveredarea = 0;
            double percent = 0;

            for (var i = 0; i < m_size; i++) {
                for (var j = 0; j < m_size; j++) {
                    if (optmap[i, j] == 0.5) { coveredarea += 1; }
                }
            }

            percent = 100 * (coveredarea / (double) freearea);
            return percent;
        }

        public double isTheMapOptimized(bool[,] obst) {
            var coveredarea = 0;
            double percent = 0;

            MessageHandler.Write("Lefedés optimalizáltságának vizsgálata.");
            for (var i = 0; i < m_size; i++) {
                for (var j = 0; j < m_size; j++) {
                    if (optmap[i, j] == 0.5) { coveredarea += 1; }
                }
            }
            for (var i = 0; i < m_size; i++) {
                for (var j = 0; j < m_size; j++) {
                    if (obst[i, j] == false) { freearea += 1; }
                }
            }
            percent = 100 * (coveredarea / (double) freearea);
            MessageHandler.Write("Lefedés optimalizáltságának vizsgálata megtörtént: " + percent + "%");
            return percent;
        }

        public double[,] mapOptimizedCover() {
            int x = 0, y = 0;
            bool ok;

            if (isTheMapOptimized() < cover) {
                MessageHandler.Write("Meghatározott lefedési százalék elérése a középpontok optimalizálásával.");
                do {
                    ok = false;
                    for (var i = 0; i < m_size; i++) {
                        for (var j = 0; j < m_size; j++) {
                            if ((optmap[i, j] == 0) && !ok) {
                                x = i;
                                y = j;
                                fcmap[i, j] = true;
                                ok = true;
                            }
                        }
                    }
                    if ((x >= radius) && (y >= radius)) {
                        for (var k = x - radius; (k < m_size) && (k <= x + radius); k++) {
                            for (var l = y - radius; (l < m_size) && (l <= y + radius); l++) {
                                if (optmap[k, l] == 0) { optmap[k, l] = 0.5; }
                            }
                        }
                    }
                    else {
                        for (var k = x; (k < m_size) && (k < x + radius + 1); k++) {
                            for (var l = y; (l < m_size) && (l < y + radius + 1); l++) {
                                if (optmap[k, l] == 0) { optmap[k, l] = 0.5; }
                            }
                        }
                    }
                }
                while (isTheMapOptimized() < cover);
                MessageHandler.Write(
                                     "Meghatározott lefedési százalék elérése a középpontok optimalizálásával, megtörtént: " +
                                     isTheMapOptimized() + "%");
            }
            return optmap;
        }

        public void createFullmap(bool[,] obst) {
            fullmap = new double[m_size, m_size];
            MessageHandler.Write("Gráfbejáráshoz szükséges mátrix létrehozása.");
            for (var i = 0; i < m_size; i++) {
                for (var j = 0; j < m_size; j++) {
                    if (fcmap[i, j]) {
                        fullmap[i, j] = 0.5;
                        number += 1;
                    }
                    if (obst[i, j]) { fullmap[i, j] = 1; }
                }
            }
            MessageHandler.Write("Gráfbejáráshoz szükséges mátrix létrehozása, megtörtént");
        }

        public void cleanUp() {
            fcmap = null;
            optmap = null;
            fullmap = null;
            freearea = 0;
            number = 0;
        }
    }
}