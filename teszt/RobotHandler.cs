using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teszt
{
    //ezt az osztalyt, amiből objektum lesz, ez tartalmazza majd a robot jelenlegi poziciojat
    //az ebből az osztályból képzett objektum oljda meg a kommunikációt a az algoritmusok és a GUI között

    public class RobotHandler
    {
        List<Tuple<int, int, double>> coordinates;
        //TODO add common variable
        public List<Tuple<int, int, double>> getCurrentCoordinates() {
            return coordinates;
        }
        public void setCurrentCoordinates(List<Tuple<int, int, double>> coords)
        {
            coordinates=coords;
        }


    }
}
