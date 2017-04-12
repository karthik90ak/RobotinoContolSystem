using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

using rec.robotino.api2;

// Author: Karthik Arakotaram

namespace RobotinoControlSystem
{


    public partial class Form1 : Form
    {
        //Initializing the cameras

        System.Diagnostics.Process cam1 = new System.Diagnostics.Process();
        System.Diagnostics.Process cam2 = new System.Diagnostics.Process();


        public Form1()
        {
            InitializeComponent();


            //Open Command Prompt 
            Console.WriteLine("Command Prompt activated");
            System.Diagnostics.Process cmd = new System.Diagnostics.Process();

            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            //Running arp -a command
            cmd.StandardInput.WriteLine("arp -a");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string IPs = cmd.StandardOutput.ReadToEnd();
            System.Diagnostics.Debug.WriteLine("COPYING DATA INTO IPs");
            System.Diagnostics.Debug.WriteLine(IPs);

            if (IPs.Contains("192.168.0.3") && IPs.Contains("192.168.0.4"))
            {
                System.Windows.Forms.MessageBox.Show("Robotino addresses are found and are within the network range");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Robotino IPs not found.Please refresh the network");
            }

            //Enter the camera executable path here

            cam1.StartInfo.FileName = "C:\\Users\\Karthik\\Desktop\\robotinoCameraAccess\\Robotino1Camera\\Camera.exe";
            cam2.StartInfo.FileName = "C:\\Users\\Karthik\\Desktop\\robotinoCameraAccess\\Robotino2Camera\\Camera.exe";
        }


        public float getRobotino1Battery()
        {

            MyCom com = new MyCom();
            PowerManagement pow = new PowerManagement();
            Program prg = new Program();
            var hostname = "192.168.0.3";

            float robotOneBattery = 0;
            try
            {
               do
                {
                    prg.init(hostname);
                    robotOneBattery = prg.getBatteryVoltage();
                    prg.destroy(); 
               } while (robotOneBattery == 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("First Robotino's Voltage........" + robotOneBattery);
            com.disconnectFromServer();

            return robotOneBattery;
        }


        public float getRobotino2Battery()
        {

            MyCom com = new MyCom();
            PowerManagement pow = new PowerManagement();
            float robotOneBattery = getRobotino1Battery();

            float robotTwoBattery = 0;
            var hostname = "192.168.0.4";
            Program prg = new Program();

            
            try
            {
                 do
                {
                prg.init(hostname);
                robotTwoBattery = prg.getBatteryVoltage();
                prg.destroy();
                } while  (robotTwoBattery == 0);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Second Robotino's Voltage........" + robotTwoBattery);
            com.disconnectFromServer();
            return robotTwoBattery;
        }


        //Beginning of Use Case 1

        private void moveToTargetOne(object sender, EventArgs e)
        {

            //code for target 1 equidistant target


            if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
            {
                System.Windows.Forms.MessageBox.Show("Please enable Camera");
                return;
            }

            if ((checkBox1.Checked) && (checkBox2.Checked))
            {
                System.Windows.Forms.MessageBox.Show("Both Robotinos have Camera..Comparing Battery Levels");
            }
            else if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
            {
                System.Windows.Forms.MessageBox.Show("Please enable Camera");
                return;
            }


            //initializing variables with false

            bool robotinoOneCameraEnabled = false;
            bool robotinoTwoCameraEnabled = false;

            if (checkBox1.Checked)
            {
                robotinoOneCameraEnabled = true;
            }

            if (checkBox2.Checked)
            {
                robotinoTwoCameraEnabled = true;
            }

            float robotino1Battery = getRobotino1Battery();
            float robotino2Battery = getRobotino2Battery();

            if (robotinoOneCameraEnabled == true)
            {
                if (robotino1Battery > robotino2Battery)
                {

                    //Open R1 Camera

                    cam1.Start();

                    //Move R1 towards target T1 

                    try
                    {
                        string hostname = "192.168.0.3";
                        Program prg = new Program();

                        prg.init(hostname);
                        List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
                        prg.destroy();


                        var value0 = distanceVoltageList[0].voltage();
                        var value1 = distanceVoltageList[1].voltage();
                        var value8 = distanceVoltageList[8].voltage();

                        Console.WriteLine("Zero Bumper's Voltage........" + value0);
                        Console.WriteLine("First Bumper's Voltage........" + value1);
                        Console.WriteLine("Eighth Bumper's Voltage........" + value8);



                        if ((value0 >= 0.7) || (value1 >= 0.7) || (value8 >= 0.7))
                        {
                            prg.init(hostname);
                            prg.obstacleDetectedRobotino1();
                            prg.destroy();
                        }

                        prg.init(hostname);
                        prg.driveRobotinoToTarget();
                        prg.destroy();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                if ((robotinoTwoCameraEnabled == true) || (robotinoOneCameraEnabled == true))
                {
                    if (robotino2Battery > robotino1Battery)
                    {

                        //Open R2 Camera 
                        cam2.Start();

                        //Move R2 towards target T1 

                        try
                        {
                            string hostname = "192.168.0.4";
                            Program prg = new Program();

                            prg.init(hostname);
                            List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
                            prg.destroy();


                            var value0 = distanceVoltageList[0].voltage();
                            var value1 = distanceVoltageList[1].voltage();
                            var value8 = distanceVoltageList[8].voltage();

                            Console.WriteLine("Zero Bumper's Voltage........" + value0);
                            Console.WriteLine("First Bumper's Voltage........" + value1);
                            Console.WriteLine("Eighth Bumper's Voltage........" + value8);



                            if ((value0 >= 0.7) || (value1 >= 0.7) || (value8 >= 0.7))
                            {
                                prg.init(hostname);
                                prg.obstacleDetectedRobotino2();
                                prg.destroy();
                            }

                            prg.init(hostname);
                            prg.driveRobotinoToTarget();
                            prg.destroy();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }


                    }
                    else if ((robotinoOneCameraEnabled == true) && (robotino1Battery > 20))
                    {
                        //Open R1 Camera

                        cam1.Start();

                        //Move R1 towards target T1 

                        try
                        {
                            string hostname = "192.168.0.3";
                            Program prg = new Program();

                            prg.init(hostname);
                            List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
                            prg.destroy();


                            var value0 = distanceVoltageList[0].voltage();
                            var value1 = distanceVoltageList[1].voltage();
                            var value8 = distanceVoltageList[8].voltage();

                            Console.WriteLine("Zero Bumper's Voltage........" + value0);
                            Console.WriteLine("First Bumper's Voltage........" + value1);
                            Console.WriteLine("Eighth Bumper's Voltage........" + value8);



                            if ((value0 >= 0.7) || (value1 >= 0.7) || (value8 >= 0.7))
                            {
                                prg.init(hostname);
                                prg.obstacleDetectedRobotino1();
                                prg.destroy();
                            }

                            prg.init(hostname);
                            prg.driveRobotinoToTarget();
                            prg.destroy();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }

                    else if ((robotinoTwoCameraEnabled == true) && (robotino2Battery > 20))
                    {

                        //Open R2 Camera 
                        cam2.Start();

                        //Move R2 towards target T1 

                        try
                        {
                            string hostname = "192.168.0.4";
                            Program prg = new Program();

                            prg.init(hostname);
                            List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
                            prg.destroy();


                            var value0 = distanceVoltageList[0].voltage();
                            var value1 = distanceVoltageList[1].voltage();
                            var value8 = distanceVoltageList[8].voltage();

                            Console.WriteLine("Zero Bumper's Voltage........" + value0);
                            Console.WriteLine("First Bumper's Voltage........" + value1);
                            Console.WriteLine("Eighth Bumper's Voltage........" + value8);



                            if ((value0 >= 0.7) || (value1 >= 0.7) || (value8 >= 0.7))
                            {
                                prg.init(hostname);
                                prg.obstacleDetectedRobotino2();
                                prg.destroy();
                            }

                            prg.init(hostname);
                            prg.driveRobotinoToTarget();
                            prg.destroy();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }

                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please try again");
                        return;
                    }
                }
            }
        }


        //End of Use Case 1

        //Beginning Use Case 2

        private void moveToTarget2(object sender, EventArgs e)
        {
            //code for target 2

            if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
            {
                System.Windows.Forms.MessageBox.Show("Please enable Camera");
                return;
            }

            float robotino1Battery = getRobotino1Battery();
            float robotino2Battery = getRobotino2Battery();

            bool robotinoOneCameraEnabled = false;
            bool robotinoTwoCameraEnabled = false;

            if (checkBox1.Checked)
            {
                robotinoOneCameraEnabled = true;
            }

            if (checkBox2.Checked)
            {
                robotinoTwoCameraEnabled = true;
            }


            if (robotinoOneCameraEnabled == true)
            {
                if (robotino1Battery > 20)
                {
                    //Open R1 Camera
                    cam1.Start();

                    //move robotino 1 to Target 2
                    try
                    {
                        string hostname = "192.168.0.3";
                        Program prg = new Program();

                        prg.init(hostname);
                        List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
                        prg.destroy();


                        var value0 = distanceVoltageList[0].voltage();
                        var value1 = distanceVoltageList[1].voltage();
                        var value8 = distanceVoltageList[8].voltage();


                        textBox3.Text = value8.ToString();
                        textBox4.Text = value0.ToString();
                        textBox5.Text = value1.ToString();


                        Console.WriteLine("Zero Bumper's Voltage........" + value0);
                        Console.WriteLine("First Bumper's Voltage........" + value1);
                        Console.WriteLine("Eighth Bumper's Voltage........" + value8);


                        if ((value0 >= 0.7) || (value1 >= 0.7) || (value8 >= 0.7))
                        {
                            prg.init(hostname);
                            prg.obstacleDetectedRobotino1();
                            prg.destroy();
                        }

                        prg.init(hostname);
                        prg.rotateRobotino1TowardsTargetTwo();
                        prg.destroy();

                        prg.init(hostname);
                        prg.driveRobotinoToTarget();
                        prg.destroy();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                else if (robotinoTwoCameraEnabled == true)
                {
                    if (robotino2Battery > 20)
                    {
                        //open robotino 2 camera
                        cam2.Start();

                        //move R2 towards target T2
                        try
                        {
                            string hostname = "192.168.0.4";
                            Program prg = new Program();

                            prg.init(hostname);
                            List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
                            prg.destroy();


                            var value0 = distanceVoltageList[0].voltage();
                            var value1 = distanceVoltageList[1].voltage();
                            var value8 = distanceVoltageList[8].voltage();

                            Console.WriteLine("Zero Bumper's Voltage........" + value0);
                            Console.WriteLine("First Bumper's Voltage........" + value1);
                            Console.WriteLine("Eighth Bumper's Voltage........" + value8);


                            if ((value0 >= 0.7) || (value1 >= 0.7) || (value8 >= 0.7))
                            {
                                prg.init(hostname);
                                prg.obstacleDetectedRobotino2();
                                prg.destroy();
                            }

                            prg.init(hostname);
                            prg.rotateRobotino2TowardsTargetTwo();
                            prg.destroy();

                            prg.init(hostname);
                            prg.driveRobotinoToTarget();
                            prg.destroy();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }


                    }

                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please try again");
                        return;
                    }

                }
            }
        }



        //End of Use Case 2

        //Beginning of Use Case 3
        private void moveToTarget3(object sender, EventArgs e)
        {
            //Code for target 3

            if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
            {
                System.Windows.Forms.MessageBox.Show("Please enable Camera");
                return;
            }

            float robotino1Battery = getRobotino1Battery();
            float robotino2Battery = getRobotino2Battery();

            bool robotinoOneCameraEnabled = false;
            bool robotinoTwoCameraEnabled = false;

            if (checkBox1.Checked)
            {
                robotinoOneCameraEnabled = true;
            }

            if (checkBox2.Checked)
            {
                robotinoTwoCameraEnabled = true;
            }

            if (robotinoTwoCameraEnabled == true)
            {
                if (robotino2Battery > 20)
                {
                    //open robotino 2 camera
                    cam2.Start();

                    // move robotino 2 towards target 3
                    try
                    {
                        string hostname = "192.168.0.4";
                        Program prg = new Program();

                        prg.init(hostname);
                        List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
                        prg.destroy();


                        var value0 = distanceVoltageList[0].voltage();
                        var value1 = distanceVoltageList[1].voltage();
                        var value8 = distanceVoltageList[8].voltage();


                        textBox3.Text = value8.ToString();
                        textBox4.Text = value0.ToString();
                        textBox5.Text = value1.ToString();


                        Console.WriteLine("Zero Bumper's Voltage........" + value0);
                        Console.WriteLine("First Bumper's Voltage........" + value1);
                        Console.WriteLine("Eighth Bumper's Voltage........" + value8);


                        if ((value0 >= 0.7) || (value1 >= 0.7) || (value8 >= 0.7))
                        {
                            prg.init(hostname);
                            prg.obstacleDetectedRobotino2();
                            prg.destroy();
                        }

                        prg.init(hostname);
                        prg.rotateRobotino2TowardsTargetThree();
                        prg.destroy();

                        prg.init(hostname);
                        prg.driveRobotinoToTarget();
                        prg.destroy();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }


                }

                else if (robotinoOneCameraEnabled == true)
                {
                    if (robotino1Battery > 20)
                    {
                        //Open R1 Camera
                        cam1.Start();

                        //move robotino towards target 3

                        //open robotino 2 camera
                        cam2.Start();

                        // move robotino 2 towards target 3
                        try
                        {
                            string hostname = "192.168.0.3";
                            Program prg = new Program();

                            prg.init(hostname);
                            List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
                            prg.destroy();


                            var value0 = distanceVoltageList[0].voltage();
                            var value1 = distanceVoltageList[1].voltage();
                            var value8 = distanceVoltageList[8].voltage();


                            textBox3.Text = value8.ToString();
                            textBox4.Text = value0.ToString();
                            textBox5.Text = value1.ToString();


                            Console.WriteLine("Zero Bumper's Voltage........" + value0);
                            Console.WriteLine("First Bumper's Voltage........" + value1);
                            Console.WriteLine("Eighth Bumper's Voltage........" + value8);


                            if ((value0 >= 0.7) || (value1 >= 0.7) || (value8 >= 0.7))
                            {
                                prg.init(hostname);
                                prg.obstacleDetectedRobotino1();
                                prg.destroy();
                            }

                            prg.init(hostname);
                            prg.rotateRobotino1TowardsTargetThree();
                            prg.destroy();

                            prg.init(hostname);
                            prg.driveRobotinoToTarget();
                            prg.destroy();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }


                    }
                }

                else {
                    System.Windows.Forms.MessageBox.Show("Please try again");
                    return;
                }

            }
        }


        //End of Use Case 3

//Invoked when user clicks on Get Battery Voltages Button

        private void getBatteryVoltages(object sender, EventArgs e)
        {
            float robotino1Battery = getRobotino1Battery();
            textBox1.Text = robotino1Battery.ToString();

            float robotino2Battery = getRobotino2Battery();
            textBox2.Text = robotino2Battery.ToString();
        }

        private void getRobotinoOneDistanceSensorValues(object sender, EventArgs e)
        {
            string hostname = "192.168.0.3";
            Program prg = new Program();

            prg.init(hostname);
            List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
            prg.destroy();


            var value0 = distanceVoltageList[0].voltage();
            var value1 = distanceVoltageList[1].voltage();
            var value8 = distanceVoltageList[8].voltage();


            textBox6.Text = value8.ToString();
            textBox7.Text = value0.ToString();
            textBox8.Text = value1.ToString();
        }


        private void getRobotinoTwoDistanceSensorVoltageList(object sender, EventArgs e)
        {
            string hostname = "192.168.0.4";
            Program prg = new Program();

            prg.init(hostname);
            List<MyDistanceSensor> distanceVoltageList = prg.distanceSensorList;
            prg.destroy();


            var value0 = distanceVoltageList[0].voltage();
            var value1 = distanceVoltageList[1].voltage();
            var value8 = distanceVoltageList[8].voltage();


            textBox3.Text = value8.ToString();
            textBox4.Text = value0.ToString();
            textBox5.Text = value1.ToString();

        }

    }
}
