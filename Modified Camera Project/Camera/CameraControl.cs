using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace rec.robotino.api2.examples.camera
{
    /// <summary>
    /// This control listens for camera events and displays incoming images.
    /// </summary>
    public partial class CameraControl : UserControl
    {
        protected readonly Robot robot;
        private volatile Image img;

        public CameraControl(Robot robot)
        {
            this.robot = robot;
            robot.ImageReceived += new Robot.ImageReceivedEventHandler(robot_ImageReceived);

            InitializeComponent();
        }

        void robot_ImageReceived(Robot sender, Image img)
        {
            this.img = img;
            if(this.InvokeRequired)
                this.Invoke(new MethodInvoker(Invalidate));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (img != null)
            {
                e.Graphics.DrawImage(img, new Rectangle(new Point(0, 0), Size));
            }
            else
            {
                SolidBrush brushWhite = new SolidBrush(Color.White);
                e.Graphics.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), Size));

                Font font = new Font("Arial", 16);
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                e.Graphics.DrawString("No camera image", font, brushBlack, Width / 2 - 80, Height / 2 - 20);
            }
        }

        private void CameraControl_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void CameraControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (img != null)
            {
                Bitmap imageBMP = new Bitmap(this.img);
                imageBMP.Save(@"C:\\Users\\Karthik\\Desktop\\robotinoCameraAccess\\Robotino2Camera\\Robotino2Capture\\R2_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".png");
                System.Windows.Forms.MessageBox.Show("Image Saved");
                imageBMP.Dispose();
                imageBMP = null;
            }
            else
            {

                System.Windows.Forms.MessageBox.Show("Click on Start Camera before saving an image");
            }
        }
    }
}
