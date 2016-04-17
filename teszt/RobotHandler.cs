using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teszt
{
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
        //A jelenlegi pozicioját adja vissza ezen a metóduson keresztül az algoritmusos csoportok

        //void setCurrentCoordinates(); //Itt beallíthatják

    }
}
