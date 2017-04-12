using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using rec.robotino.api2;

namespace RobotinoControlSystem
{
    //ROBOTINO CLASSES

    public class MyCom : Com
    {
        public MyCom()
        {
        }

        override public void errorEvent(string errorString)
        {
            Console.WriteLine("Error: " + errorString);
        }

        override public void connectedEvent()
        {
            Console.WriteLine("Connected.");
        }

        override public void connectionClosedEvent()
        {
            Console.WriteLine("Connection closed.");
        }

        public void modeChangeEvent(bool isPassiveMode)
        {
            if (isPassiveMode)
                Console.WriteLine("Connected in passive mode.");
        }
    }

    public class MyBumper : Bumper
    {
        bool _bumped;
        public MyBumper(bool val = false)
        {
            _bumped = val;
        }

        public bool Value
        {
            get { return _bumped; }
        }
        override public void bumperEvent(bool hasContact)
        {
            _bumped |= hasContact;
        }
    }

    public class MyDistanceSensor : DistanceSensor
    {
        private float _distance;
        private uint _sensorNumber;

        // Default Constructor
        public MyDistanceSensor(uint sensorNumber)
        {
            this._sensorNumber = sensorNumber;
            setSensorNumber(_sensorNumber);
        }

        public float Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        public uint SensorNumber
        {
            get { return _sensorNumber; }
            set { _sensorNumber = value; }
        }

        public override void distanceChangedEvent(float distance)
        {
            _distance = distance;
        }
    }

     //END OF ROBOTINO CLASSES

     class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// [STAThread]

        MyCom com;
        OmniDrive omniDrive;
        PowerManagement pow;

        public List<MyDistanceSensor> distanceSensorList;

        public Program()
        {
            com = new MyCom();
            omniDrive = new OmniDrive();
            omniDrive.setComId(com.id());
            distanceSensorList = new List<MyDistanceSensor>();
            pow = new PowerManagement();

            for (uint j = 0; j < 9; j++)
            {
                distanceSensorList.Add(new MyDistanceSensor(j));
                distanceSensorList[(int)j].setComId(com.id());
            }

        }

        public void init(string hostname)
        {
            Console.WriteLine("Connecting ... ");
            com.setAddress(hostname);
            com.connectToServer();

            Console.WriteLine("Connected.");
        }
        public void destroy()
        {
            com.disconnectFromServer();
        }

        //starting code to move robotino


        //public List<MyDistanceSensor> readDistSensVltg() {
        //    for (uint j = 0; j < 9; j++)
        //    {
        //        distanceSensorList.Add(new MyDistanceSensor(j));
        //        distanceSensorList[(int)j].setComId(com.id());
        //    }
        //    return distanceSensorList;
        //}

        //public void rotateTowardsTargetOne()
        //{
        //    Console.Write("Robotino 2 rotating towards TARGET 1 after avoiding obstacle... ");

        //    for (int i = 0; i < 1; i++)
        //    {
        //        omniDrive.setVelocity(0.15F, 0, 0);

        //        System.Threading.Thread.Sleep(100);
        //    }
        //}

        public float getBatteryVoltage()
        {
            pow.setComId(com.id());
            float btry;
            btry = pow.voltage();
            return btry;

        }

        //Common method used by both the robotinos after rotation

        public void driveRobotinoToTarget()
        {
            Console.Write("Robotino driving To TARGET 1... ");
            
            for (int i = 0; i < 60; i++)
            {
                omniDrive.setVelocity(0.25F, 0, 0);

                System.Threading.Thread.Sleep(100);
            }
        }


        public void rotateRobotino1TowardsTargetTwo()
        {
            Console.Write("Robotino 1 rotating towards TARGET 2... ");

            for (int i = 0; i < 3; i++)
            {
                omniDrive.setVelocity(0.25F, 0, 1.6F);

                System.Threading.Thread.Sleep(100);
            }

        }


        public void rotateRobotino2TowardsTargetTwo()
        {
            Console.Write("Robotino 2 rotating towards TARGET 2... ");

            for (int i = 0; i < 3; i++)
            {
                omniDrive.setVelocity(0.25F, 0, 1.2F);

                System.Threading.Thread.Sleep(100);
            }

        }

        public void rotateRobotino1TowardsTargetThree()
        {

            Console.Write("Robotino 1 rotating towards TARGET 3... ");

            for (int i = 0; i < 2; i++)
            {
                omniDrive.setVelocity(0.25F, 0, -1.2F);

                System.Threading.Thread.Sleep(100);
            }

        }

        public void rotateRobotino2TowardsTargetThree()
        {

            Console.Write("Robotino 2 rotating towards TARGET 3... ");

            for (int i = 0; i < 2; i++)
            {
                omniDrive.setVelocity(0.25F, 0, -1.6F);

                System.Threading.Thread.Sleep(100);
            }
            
        }

       
        public void obstacleDetectedRobotino1()
        {
            Console.Write("Robotino 1 avoiding obstacle... ");

            for (int i = 0; i < 25; i++)
            {
                omniDrive.setVelocity(0, 0.35F, 0);

                System.Threading.Thread.Sleep(100);
            }

        }


        public void obstacleDetectedRobotino2()
        {
            Console.Write("Robotino 2 avoiding obstacle... ");

            for (int i = 0; i < 25; i++)
            {
                omniDrive.setVelocity(0, -0.35F, 0);

                System.Threading.Thread.Sleep(100);
            }
            
        }

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    } 
}
