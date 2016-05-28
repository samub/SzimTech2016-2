using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotMover
{
     class SimulateAlgos     
    {
        public static void setRobot(ref Robot r)
        {
            Console.WriteLine("RobotCover: "+r.Cover);
            Console.WriteLine("Robotroute: " + r.Route);
            Console.WriteLine("Robotradius", r.Radius);
            r.Cover = 50;
        }
       
    }
}
