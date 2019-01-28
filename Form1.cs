using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PPT = Microsoft.Office.Interop.PowerPoint;

namespace AirNav
{
    public partial class AirNavForm : Form
    {
        Capture WebCamFeed = null;
        Thread t;

        Seq<Point> Hull;
        Seq<MCvConvexityDefect> defects;
        Image<Bgr, byte> imgFrame;

        MCvConvexityDefect[] defectArray;
        MCvBox2D box;
        MemStorage storage = new MemStorage();

        Cursor current = Cursor.Current;    // Stores current cursor for prsentation mode
        PPT.Application pptApplication;     // Creates Object for PowerPoint Application
        //PPT.Presentation pptPresentation; // Creates Object for PowerPoint Presentation
        bool isPresenting = false;          // Checks if User has Started Presenting
        bool presentationMode = false;      // Optimize AirNav for PowerPoint

        int sensitivity = 30;
        int previousX = 0, previousY = 0;
        int difference = 0;                 // Used for Scroll Amount
        int xDifference = 0;                // Used for Several Gestures
        int yDifference = 0;                // Used for Several Gestures

        Ycc minRange = new Ycc(0, 131, 80);
        Ycc maxRange = new Ycc(255, 185, 135);

        bool startPressed = false;      // Status of 'Start AirNav' Button
        bool started = false;           // do not start unless you get a finger count of more than 4
        bool displayToggle = false;     // Toggle between normal feed and a gray feed
        bool invertScrolling = false;   // Toggled through the settings page
        bool showCoordinates = false;   // Toggled through the settings page

        bool cycleApps = false;
        bool threefingers = false;
        bool closedhand = false;

        bool clicked = false;       // Toggled in a click-event
        bool rclicked = false;      // Toggled in a right-click-event
        bool isScrolling = false;   // Toggled in a scroll-event
        bool outOfFrame = true;     // Toggled when hand enters/leave frame for a large amount of time
        bool yScience = false;      // Used for Closing Windows and Scrolling - Tribute to Jesse Pinkman
        bool closingWindow = false; // Toggles when user attempts to close a window

        bool quitAirNav = false;
        bool showTargettedTips = true;
        bool disableTips = false;
        int processCount = 0;
        string activeApplication = null;
        bool[] randomTipsShown = {false, false, false, false, false};

        Random random = new Random();
        Stopwatch timer = new Stopwatch();
        Stopwatch cycleTimeOut = new Stopwatch();
        Stopwatch scrollingTipsTimer = new Stopwatch();
        Stopwatch randomTipsTimer = new Stopwatch();
        Stopwatch targettedTipsTimer = new Stopwatch();

        public delegate void RunDelegateFunction(object sender, EventArgs e);
                                    // To Run AirNav in a Thread

        public AirNavForm()
        {
            InitializeComponent();
        }

        private void AirNav_Load(object sender, EventArgs e)
        {
            // Connecting to WebCam
            try
            {
                WebCamFeed = new Capture();
            }
            catch (NullReferenceException exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            t = new Thread(new ThreadStart(RunAirNav));
            t.IsBackground = true;
            t.Start();
        }

        public void RunAirNav()
        {
            while(!quitAirNav)     // Continue to Run AirNav in Background (For Lifetime of Application)
                Invoke(new RunDelegateFunction(ProcessFrameAndUpdateGUI), new object(), new EventArgs());
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (!startPressed)
            {
                startPressed = true;
                startBtn.Text = "Stop Navigating";
                presentationBtn.Enabled = true;
            }
            else
            {
                startPressed = false;
                startBtn.Text = "Navigate";
                started = false;

                // Following Code Disables Presentation Mode when Navigation is Stopped
                presentationMode = false;
                isPresenting = false;
                presentationBtn.Text = "Presentation Mode";
                presentationBtn.Enabled = false;
            }
        }

        public void ProcessFrameAndUpdateGUI(object sender, EventArgs e)
        {
            int fingerCount = 0;
            Double contourArea = 0;
            Double maxArea = 0;

            imgFrame = WebCamFeed.QueryFrame();
            
            sensitivity = sensitivitySlider.Value;      //Defines scope for navigation ; changed from the SETTINGS page.
            
            if (imgFrame == null)
                return;

            if (gloveMode.Checked)          // Changes 'minRange' & 'maxRange' for Contrasting Backgrounds
                gloveColor.Enabled = true;
            else
                gloveColor.Enabled = false;

            Image<Ycc, Byte> currentYCrCbFrame = imgFrame.Convert<Ycc, byte>();
            //Converts the original frame to a YCrCb frame for filtering.
            Image<Gray, byte> skin = new Image<Gray, byte>(imgFrame.Width, imgFrame.Height);
            //Stores Gray image of areas of interest
            skin = currentYCrCbFrame.InRange(minRange, maxRange);
            //Extracts areas within range for skin color

            //Noise Filtering
            StructuringElementEx rect_12 = new StructuringElementEx(10, 10, 5, 5, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvErode(skin, skin, rect_12, 1);
            StructuringElementEx rect_6 = new StructuringElementEx(6, 6, 3, 3, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvDilate(skin, skin, rect_6, 2);
            skin = skin.Flip(FLIP.HORIZONTAL);
            skin = skin.SmoothGaussian(9);

            imgFrame = imgFrame.Flip(FLIP.HORIZONTAL);

            Contour<Point> contours = skin.FindContours();
            Contour<Point> biggestContour = null;

            while (contours != null)
            {
                contourArea = contours.Area;
                if (contourArea > maxArea)
                {
                    maxArea = contourArea;
                    biggestContour = contours;
                }
                contours = contours.HNext;          //checks for the next contour
            }

            #region Convexity Defects Algorithm 
            if (biggestContour != null)
            {
                fingerCount = 0;

                biggestContour = biggestContour.ApproxPoly(0.00025);
                imgFrame.Draw(biggestContour, new Bgr(Color.LimeGreen), 2);  //For debugging

                Hull = biggestContour.GetConvexHull(ORIENTATION.CV_CLOCKWISE);
                defects = biggestContour.GetConvexityDefacts(storage, ORIENTATION.CV_CLOCKWISE);
                imgFrame.DrawPolyline(Hull.ToArray(), true, new Bgr(0, 0, 256), 2);     //For debugging

                box = biggestContour.GetMinAreaRect();

                defectArray = defects.ToArray();
                for (int i = 0; i < defects.Total; i++)
                {
                    PointF startPoint = new PointF((float)defectArray[i].StartPoint.X,
                                                (float)defectArray[i].StartPoint.Y);

                    PointF depthPoint = new PointF((float)defectArray[i].DepthPoint.X,
                                                    (float)defectArray[i].DepthPoint.Y);

                    PointF endPoint = new PointF((float)defectArray[i].EndPoint.X,
                                                    (float)defectArray[i].EndPoint.Y);


                    CircleF startCircle = new CircleF(startPoint, 5f);
                    CircleF depthCircle = new CircleF(depthPoint, 5f);
                    CircleF endCircle = new CircleF(endPoint, 5f);


                    if ((startCircle.Center.Y < box.center.Y || depthCircle.Center.Y < box.center.Y) &&
                            (startCircle.Center.Y < depthCircle.Center.Y) &&
                            (Math.Sqrt(Math.Pow(startCircle.Center.X - depthCircle.Center.X, 2) +
                                       Math.Pow(startCircle.Center.Y - depthCircle.Center.Y, 2)) >
                                       box.size.Height / 6.5))
                    {
                        fingerCount++;
                    }
                }
            }
            #endregion             

            MCvMoments moment = new MCvMoments();

            try
            {
                moment = biggestContour.GetMoments();
            }
            catch (NullReferenceException except)
            {
                AirNav.BalloonTipTitle = "AirNav";
                AirNav.BalloonTipText = "Contour Not Found: " + except.Message;
                AirNav.ShowBalloonTip(1000);
                return;
            }

            try
            {
                CvInvoke.cvMoments(biggestContour, ref moment, 0);
            }
            catch (Exception)
            {
                throw;
            }

            double m00 = CvInvoke.cvGetSpatialMoment(ref moment, 0, 0);
            double m10 = CvInvoke.cvGetSpatialMoment(ref moment, 1, 0);
            double m01 = CvInvoke.cvGetSpatialMoment(ref moment, 0, 1);

            int currentX = Convert.ToInt32(m10 / m00) / 10;     
            int currentY = Convert.ToInt32(m01 / m00) / 10;
            // Current coordinates of the center of the palm 

            if (showCoordinates)
                coordinatesTextBox.Text = "X: " + currentX.ToString() + "     Y: " + currentY.ToString();

            if(fingerCount >= 4 && !started && startPressed)
                started = true;
            if(started)
                Navigate(fingerCount, currentX, currentY);
            
            display.SetZoomScale(0.5, new Point(100, 100));
            if (displayToggle)
                display.Image = skin;
            else
                display.Image = imgFrame;
        }

        #region Mouse Directives
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;  // Mouse Left Button Pressed
        private const int MOUSEEVENTF_LEFTUP = 0x04;    // Mouse Left Button Released
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08; // Mouse Right Button Pressed
        private const int MOUSEEVENTF_RIGHTUP = 0x10;   // Mouse Right Button Released
        private const int MOUSEEVENTF_WHEEL = 0x800;    // Mouse Scroll Event
        #endregion

        #region Basic Mouse Actions
        private void DoMouseClick()
        {
            uint X = Convert.ToUInt32(Cursor.Position.X);
            uint Y = Convert.ToUInt32(Cursor.Position.Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        private void RightMouseClick()
        {
            uint X = Convert.ToUInt32(Cursor.Position.X);
            uint Y = Convert.ToUInt32(Cursor.Position.Y);
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }
        
        private void MouseScroll(int distance)
        {
            uint X = Convert.ToUInt32(Cursor.Position.X);
            uint Y = Convert.ToUInt32(Cursor.Position.Y);
            uint scrollAmount = unchecked((uint)(distance * 60));
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, scrollAmount, 0);
        }
        #endregion

        public void Navigate(int fingerCount, int currentX, int currentY)
        {
            if (pptApplication != null)
            {
                pptApplication.SlideShowBegin += pptApplication_SlideShowBegin;
                pptApplication.SlideShowEnd += pptApplication_SlideShowEnd;
            }

            if (!disableTips && !presentationMode)
            {
                randomTips();
                scrollingTips();
                if (targettedTipsTimer.Elapsed >= new TimeSpan(0, 0, 30))
                {
                    targettedTips();
                    targettedTipsTimer.Reset();
                }
            }

            if ((fingerCount == 4 || fingerCount == 5))
            {
                if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT") && threefingers)
                    StartLaser();

                closedhand = false;     // Hand is no longer closed
                threefingers = false;     // User can open hand to exit 'threefinger mode'
                cycleApps = false;

                if (!clicked && !rclicked && !isScrolling)
                {
                    if (closingWindow)
                    {
                        if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT"))
                        {
                            ReleaseLaser();
                            if (xDifference > 0)
                                System.Windows.Forms.SendKeys.Send("{LEFT}");
                            else if (xDifference < 0)
                                System.Windows.Forms.SendKeys.Send("{RIGHT}");
                            StartLaser();
                        }
                        else
                        {
                            AirNav.BalloonTipTitle = "Closing Application";
                            AirNav.BalloonTipText = "AirNav just attempted to close " + GetActiveFileNameTitle();
                            AirNav.ShowBalloonTip(2000);
                            System.Windows.Forms.SendKeys.Send("%{F4}");
                        }
                        closingWindow = false;
                    }
                    Cursor.Position = new Point(currentX * sensitivity, currentY * sensitivity);
                    outOfFrame = false;     // Hand is no longer out of the frame
                }
                else if (clicked)
                {
                    if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT"))
                    {
                        ReleaseLaser();
                        DoMouseClick();
                        StartLaser();
                    }
                    else
                        DoMouseClick();
                    clicked = false;
                    yScience = false;
                    timer.Reset();

                    if (!disableTips && !presentationMode)
                    {
                        if (showTargettedTips)
                        {
                            showTargettedTips = false;
                            targettedTipsTimer.Start();
                        }
                    }
                }
                else if (rclicked)
                {
                    if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT"))
                    {
                        ReleaseLaser();
                        RightMouseClick();
                        StartLaser();
                    }
                    else
                        RightMouseClick();
                    rclicked = false;
                    timer.Reset();
                }
                else if (isScrolling)
                {
                    if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT"))
                        StartLaser();
                    
                    isScrolling = false;
                    yScience = false;
                    timer.Reset();
                }
            }
            
            if (fingerCount == 3 && !closedhand)
            {
                if (!cycleApps)
                {
                    cycleApps = true;
                    cycleTimeOut.Start();
                    previousX = currentX;
                    previousY = currentY;
                }

                if (cycleTimeOut.Elapsed >= new TimeSpan(0, 0, 0, 0, 500) && cycleTimeOut.Elapsed <= new TimeSpan(0, 0, 0, 0, 800) && fingerCount == 3)
                {
                    AirNav.BalloonTipTitle = "Three-Finger Mode";
                    AirNav.BalloonTipText = "Keep three fingers open in the same position for 2 seconds to enter Three-Finger Mode.";
                    AirNav.ShowBalloonTip(1000);
                }

                if (cycleTimeOut.Elapsed >= new TimeSpan(0, 0, 2) && !threefingers)
                {
                    if (fingerCount == 3 && (Math.Abs(currentY - previousY)) <= 3 && (Math.Abs(currentX - previousX)) <= 3)
                    {
                        previousX = currentX;
                        previousY = currentY;
                        threefingers = true;        // Program Mustn't Enter Closed Hand Case
                        if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT"))
                            ReleaseLaser();

                        AirNav.BalloonTipTitle = "Three-Finger Mode";
                        AirNav.BalloonTipText = "You are now in Three-Finger Mode. Open your hand to resume navigation.";
                        AirNav.ShowBalloonTip(1000);
                    }
                    else
                    {
                        cycleTimeOut.Reset();
                        cycleApps = false;
                    }
                }

                xDifference = currentX - previousX;
                yDifference = currentY - previousY;

                if (Math.Abs(yDifference) <= 5 && (Math.Abs(xDifference) % 5) == 0 && xDifference != 0 && threefingers)
                {
                    if (xDifference > 0)
                    {
                        System.Windows.Forms.SendKeys.Send("%{ESC}");
                        cycleTimeOut.Reset();
                    }
                    else if (xDifference < 0)
                    {
                        System.Windows.Forms.SendKeys.Send("+(%{ESC})");
                        cycleTimeOut.Reset();
                    }

                    previousX = currentX;
                    previousY = currentY;
                }

                else if (Math.Abs(yDifference) >= 7 && Math.Abs(xDifference) <= 5 && threefingers)
                {
                    if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT"))
                    {
                        presentationMode = false;
                        isPresenting = false;
                        presentationBtn.Text = "Presentation Mode";

                        AirNav.BalloonTipTitle = "Presentation Mode";
                        AirNav.BalloonTipText = "You have just left Presentation Mode. To resume Presentation Mode, open AirNav and click on 'Presentation Mode'.";
                        AirNav.ShowBalloonTip(1000);
                    }
                    else
                        System.Windows.Forms.SendKeys.Send("^({ESC})");
                    
                    previousX = currentX;
                    previousY = currentY;
                    cycleTimeOut.Reset();
                    cycleApps = false;
                }
            }
            
            if (fingerCount <= 1 && !outOfFrame && !threefingers)
            {
                closedhand = true;

                if (!clicked)
                    clicked = true;

                if (!yScience)
                {
                    previousX = currentX;
                    previousY = currentY;
                    timer.Start();
                    yScience = true;
                }

                if (timer.Elapsed > new TimeSpan(0, 0, 3))
                {   // Hand has been out of the frame for too long
                    rclicked = false;
                    outOfFrame = true;
                    timer.Reset();

                    AirNav.BalloonTipTitle = "AirNav";
                    AirNav.BalloonTipText = "AirNav has detected that your hand is out of the scope of your webcam. Navigation will be resumed when your hand is back in scope.";
                    AirNav.ShowBalloonTip(2000);
                }
                else if (timer.Elapsed > new TimeSpan(0, 0, 2))
                {
                    rclicked = true;
                    clicked = false;
                }
                else if (timer.Elapsed > new TimeSpan(0, 0, 1) && !isScrolling)
                {
                    if (Math.Abs(currentY - previousY) <= 10 && Math.Abs(currentX - previousX) >= 10)
                    {
                        if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT"))
                            xDifference = currentX - previousX;

                        closingWindow = true;
                        yScience = false;
                    }
                    else if (Math.Abs(currentX - previousX) <= 10 && Math.Abs(currentY - previousY) >= 5)
                    {
                        difference = currentY - previousY;
                        timer.Reset();
                        isScrolling = true;

                        AirNav.BalloonTipTitle = "Scrolling";
                        AirNav.BalloonTipText = "You can now scroll (up or down) while your fist is closed. Open your hand to resume navigation.";
                        AirNav.ShowBalloonTip(2000);
                    }

                    else
                        yScience = false;

                    previousX = currentX;
                    previousY = currentY;

                    clicked = false;
                }

                if (isScrolling)
                {
                    if (presentationMode && isPresenting && GetActiveFileNameTitle("POWERPNT"))
                        ReleaseLaser();

                    MouseScroll((!invertScrolling) ? difference : (difference * -1));
                    difference = currentY - previousY;
                    previousY = currentY;
                    clicked = false;
                    rclicked = false;
                }
            }
        }

        // For Laser Pointer
        #region Important Key Directives
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const uint KEYEVENTF_KEYUP = 0x02;
        public const uint VK_CONTROL = 0x11;
        //public const uint VK_MENU = 0x12;       // Directive for 'ALT'
        //public const uint VK_TAB = 0x09;        // Directive for 'TAB'
        #endregion

        #region Laser Functions
        private void StartLaser()
        {
            keybd_event((byte) VK_CONTROL, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
        }

        private void ReleaseLaser()
        {
            keybd_event((byte)VK_CONTROL, 0, (int) KEYEVENTF_KEYUP, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, Convert.ToUInt32(Cursor.Position.X), Convert.ToUInt32(Cursor.Position.Y), 0, 0);
        }
        #endregion

        #region Checking Which Application is Active
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public bool GetActiveFileNameTitle(string ApplicationName)
        {
            IntPtr hWnd = GetForegroundWindow();
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);
            Process p = Process.GetProcessById((int)processId);
            //p.MainModule.FileName.Dump();   // For Dubugging
            return (p.ProcessName.Equals(ApplicationName));
        }

        public string GetActiveFileNameTitle()
        {
            IntPtr hWnd = GetForegroundWindow();
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);
            Process p = Process.GetProcessById((int)processId);
            //p.MainModule.FileName.Dump();   // For Dubugging
            return p.ProcessName;
        }
        #endregion

        void ReleaseAllDirectives()
        {
            ReleaseLaser();
            uint X = Convert.ToUInt32(Cursor.Position.X);
            uint Y = Convert.ToUInt32(Cursor.Position.Y);
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            //mouse_event(MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);       // For Debugging
        }

        void pptApplication_SlideShowBegin(PPT.SlideShowWindow Wn)
        {
            StartLaser();
            isPresenting = true;
        }
        
        void pptApplication_SlideShowEnd(PPT.Presentation Pres)
        {
            ReleaseLaser();
            isPresenting = false;
        }

        void scrollingTips()
        {
            int randomAmountOfTime = random.Next(1, 11);
            string[] applications = {"chrome", "iexplore", "iexplorer", "firefox", "WINWORD", "AcroRdr32", "AcroRdr64"};
            if (activeApplication == null)
            {
                foreach (string application in applications)
                {
                    if (GetActiveFileNameTitle(application))
                    {
                        scrollingTipsTimer.Start();
                        activeApplication = GetActiveFileNameTitle();
                    }
                }
            }

            if (scrollingTipsTimer.Elapsed >= new TimeSpan(0, randomAmountOfTime, 0))
            {
                if (GetActiveFileNameTitle(activeApplication))
                {   // If the application is still active after a randomAmountOfTime
                    AirNav.BalloonTipTitle = "AirNav Tip";
                    AirNav.BalloonTipText = "You can close your fist and move it up or down to start scrolling in this application.";
                    AirNav.ShowBalloonTip(1000);
                }
                scrollingTipsTimer.Reset();
                activeApplication = null;
            }
        }

        void targettedTips()
        {
            Process[] processes = Process.GetProcesses();
            if (processCount < processes.Length)
            {
                int randomizer = random.Next(1, 3);
                switch (randomizer)
                {
                    case 1:
                        AirNav.BalloonTipTitle = "AirNav Tip";
                        AirNav.BalloonTipText = "Hold out three fingers for 2 seconds and then move your hand left or right to cycle through your active applications.";
                        AirNav.ShowBalloonTip(1000);
                        break;
                    case 2:
                        AirNav.BalloonTipTitle = "AirNav Tip";
                        AirNav.BalloonTipText = "Close your fist, move it left or right and then open it to close an active application.";
                        AirNav.ShowBalloonTip(1000);
                        break;
                    default:
                        break;
                }
                processCount = processes.Length;
            }
        }

        void randomTips()
        {
            bool dontDoThisAgain = true;

            for (int i = 1; i < 6; i++ )
                dontDoThisAgain = dontDoThisAgain && randomTipsShown[i];

            if (!dontDoThisAgain)
            {
                if (randomTipsShown[0] == false)
                {
                    randomTipsTimer.Start();
                    randomTipsShown[0] = true;
                }

                if (randomTipsTimer.Elapsed >= new TimeSpan(0, random.Next(1, 11), 0))
                {
                    int randomizer = random.Next(1, 6);

                    if (randomTipsShown[randomizer] == true)
                        randomizer = random.Next(1, 6);
                    else
                    {
                        randomTipsTimer.Reset();
                        randomTipsShown[0] = false;
                        randomTipsShown[randomizer] = true;
                        switch (randomizer)
                        {
                            case 1:
                                AirNav.BalloonTipTitle = "AirNav Tip";
                                AirNav.BalloonTipText = "Hold out three fingers for 2 seconds and move up or down to open the Start Menu. Continue this motion to return.";
                                AirNav.ShowBalloonTip(1000);
                                break;
                            case 2:
                                AirNav.BalloonTipTitle = "AirNav Tip";
                                AirNav.BalloonTipText = "Close your fist for 2 seconds and open your hand to simulate a 'Right-Click'.";
                                AirNav.ShowBalloonTip(1000);
                                break;
                            case 3:
                                AirNav.BalloonTipTitle = "AirNav Tip";
                                AirNav.BalloonTipText = "Hold out three fingers for 2 seconds and move up or down while in Presentation Mode to leave Presentation Mode.";
                                AirNav.ShowBalloonTip(1000);
                                break;
                            case 4:
                                AirNav.BalloonTipTitle = "AirNav Tip";
                                AirNav.BalloonTipText = "Close your fist, move left or right and open your hand to navigate through your presentation while in Presentation Mode.";
                                AirNav.ShowBalloonTip(1000);
                                break;
                            case 5:
                                AirNav.BalloonTipTitle = "AirNav Tip";
                                AirNav.BalloonTipText = "Check out 'Glove Mode' in Settings if AirNav can't identify your hand properly.";
                                AirNav.ShowBalloonTip(1000);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            // What happens when the user clicks on the 'Settings' button
            display.Visible = false;
            settingsBtn.Visible = false;
            presentationBtn.Visible = false;
            startBtn.Visible = false;
            coordinatesTextBox.Visible = false;

            settingsTextBox.Visible = true;
            invertScroll.Visible = true;
            disableTipsCheckbox.Visible = true;
            coordinates.Visible = true;
            displayDefault.Visible = true;
            gloveMode.Visible = true;
            gloveColor.Visible = true;
            sensitivityTextBox.Visible = true;
            sensitivitySlider.Visible = true;
            leaveSettingsBtn.Visible = true;
            quitAirNavBtn.Visible = true;
        }

        private void display_DoubleClick(object sender, EventArgs e)
        {
            displayToggle = !displayToggle;
            displayDefault.Checked = !displayDefault.Checked;
        }

        private void presentationBtn_Click(object sender, EventArgs e)
        {
            if (!presentationMode)
            {
                presentationMode = true;
                presentationBtn.Text = "Leave Presentation Mode";
                
                try
                {
                    pptApplication = Marshal.GetActiveObject("PowerPoint.Application") as PPT.Application;
                }
                catch
                {
                    presentationMode = false;
                    presentationBtn.Text = "Presentation Mode";
                    MessageBox.Show("Please Launch PowerPoint First", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }

                AirNav.BalloonTipTitle = "Presentation Mode";
                AirNav.BalloonTipText = "Actions will be optimized while presenting in PowerPoint.";
                AirNav.ShowBalloonTip(2000);
            }
            else
            {
                presentationMode = false;
                isPresenting = false;
                presentationBtn.Text = "Presentation Mode";
            }
        }

        public void setColorRange(string color)
        {
            if (color.Equals("Black"))
            {
                minRange = new Ycc(0, 96, 96);
                maxRange = new Ycc(32, 160, 160);
            }

            else if (color.Equals("Blue"))
            {
                minRange = new Ycc(9, 208, 78);
                maxRange = new Ycc(73, 272, 142);
            }

            else if (color.Equals("Green"))
            {
                minRange = new Ycc(113, 22, 2);
                maxRange = new Ycc(177, 86, 66);
            }

            else if (color.Equals("Grey"))
            {
                minRange = new Ycc(149, 96, 96);
                maxRange = new Ycc(213, 160, 160);
            }

            else if (color.Equals("Red"))
            {
                minRange = new Ycc(50, 58, 208);
                maxRange = new Ycc(114, 122, 272);
            }

            else if (color.Equals("Yellow"))
            {
                minRange = new Ycc(178, 0, 114);
                maxRange = new Ycc(242, 48, 178);
            }

            else if (color.Equals("default"))
            {
                minRange = new Ycc(0, 131, 80);
                maxRange = new Ycc(255, 185, 135);
            }
        }

        private void AirNavForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
            AirNav.Text = "AirNav (Click to Restore)";
            AirNav.BalloonTipTitle = "AirNav";
            AirNav.BalloonTipText = "AirNav is Minimized to System Tray. Click to Restore.";
            AirNav.ShowBalloonTip(3000);
        }

        private void AirNav_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void restoreAirNavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void quitApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            quitAirNav = true;
            t.Abort();
            ReleaseAllDirectives();
            WebCamFeed.Dispose();
            this.Dispose();
            AirNav.Dispose();
            Application.Exit();
        }

        private void leaveSettingsBtn_Click(object sender, EventArgs e)
        {
            invertScrolling = invertScroll.Checked;
            disableTips = disableTipsCheckbox.Checked;
            displayToggle = displayDefault.Checked;
            showCoordinates = coordinates.Checked;

            if (gloveMode.Checked)
                setColorRange((gloveColor.SelectedItem).ToString());
            else
                setColorRange("default");

            display.Visible = true;
            settingsBtn.Visible = true;
            presentationBtn.Visible = true;
            startBtn.Visible = true;
            if(showCoordinates)
                coordinatesTextBox.Visible = true;

            settingsTextBox.Visible = false;
            invertScroll.Visible = false;
            disableTipsCheckbox.Visible = false;
            coordinates.Visible = false;
            displayDefault.Visible = false;
            gloveMode.Visible = false;
            gloveColor.Visible = false;
            sensitivityTextBox.Visible = false;
            sensitivitySlider.Visible = false;
            leaveSettingsBtn.Visible = false;
            quitAirNavBtn.Visible = false;
        }

        private void AirNav_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
    }
}
