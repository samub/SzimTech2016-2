using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teszt
{
    //ezt az osztalyt, amiből objektum lesz, ez tartalmazza majd a robot jelenlegi poziciojat
    //A jelenleg lefedett területet, ezekkel tudnak számolni az algosok.
    //az ebből az osztályból képzett objektum oljda meg a kommunikációt a az algoritmusok és a GUI között

    public class RobotHandler
    {
        List<Tuple<int, int, double>> Coordinates;
        List<Tuple<int, int>> CurrentlyCoveredArea;

        public RobotHandler()
        {
       
        }
        public RobotHandler(List<Tuple<int, int, double>> coords, List<Tuple<int, int>> coveredarea)
        {
            Coordinates = coords;
            CurrentlyCoveredArea = coveredarea;
        }
        public List<Tuple<int, int, double>> getCurrentCoordinates() {
            return Coordinates;
        }
        public void setCurrentlyCoveredArea(List<Tuple<int, int>> coveredarea)
        {
            CurrentlyCoveredArea = coveredarea;
        }
     
        public List<Tuple<int, int>> getCurrentCoveredArea()
        {
            return CurrentlyCoveredArea;
        }


    }
}
