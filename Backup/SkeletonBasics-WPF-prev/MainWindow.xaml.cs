/*
 * My Edited Code for upper skeleton tracking.
 * */

namespace Microsoft.Samples.Kinect.SkeletonBasics
{

    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Net.Sockets;
    using System.Speech;
    using System.IO;
    using System.Threading;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System;
    using System.Threading;
    using System.Text;
    using System.IO.Ports;
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using System.Collections.Generic;
    using System.Speech.Synthesis;



    public partial class MainWindow : Window
    {
 
        //static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        private const float RenderWidth = 640.0f;

        private const float RenderHeight = 480.0f;

        private const double JointThickness = 3;

        private const double BodyCenterThickness = 10;

        private const double ClipBoundsThickness = 10;

        private readonly Brush centerPointBrush = Brushes.Blue;

        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        private readonly Brush inferredJointBrush = Brushes.Yellow;

        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        private KinectSensor sensor;

        private DrawingGroup drawingGroup;

        private DrawingImage imageSource;

        TeleConnector teleConnector;
        private SpeechSynthesizer sp;


        private Boolean soundOn = false;


        List<Skeleton> prevFrames = new List<Skeleton>();
        private int allowedFrameNumber = 30;

        //****************************************
        //Create an instance of your kinect sensor
        public KinectSensor CurrentSensor;

        private SpeechRecognitionEngine speechRecognizer;

        
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                System.Diagnostics.Debug.Write(recognizer.Culture.Name + "\n\n");
                //string value;
                //recognizer.AdditionalInfo.TryGetValue("Kinect",out value);
                if ("en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }

            }

            return null;
        }



        private KinectSensor InitializeKinect()
        {
            //get the first available sensor and set it to the current sensor variable
            //CurrentSensor = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
            CurrentSensor = this.sensor;
            //speechRecognizer = CreateSpeechRecognizer();
            //Start the sensor
            CurrentSensor.Start();
            //then run the start method to start streaming audio
            Start();



            return CurrentSensor;
        }


       
        private void Start()
        {
            //set sensor audio source to variable
            var audioSource = CurrentSensor.AudioSource;
            //Set the beam angle mode - the direction the audio beam is pointing
            //we want it to be set to adaptive
            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            //start the audiosource 
            var kinectStream = audioSource.Start();
            //configure incoming audio stream
            speechRecognizer.SetInputToAudioStream(
                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            //make sure the recognizer does not stop after completing     
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            //reduce background and ambient noise for better accuracy
            CurrentSensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;
            CurrentSensor.AudioSource.AutomaticGainControlEnabled = false;
        }


        //here is the fun part: create the speech recognizer
        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            //set recognizer info
            RecognizerInfo ri = GetKinectRecognizer();
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ri.Id);
           /* if (null != ri)
            {
                //this.sre = new SpeechRecognitionEngine(ri.Id);
                SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ri.Id);
            }*/
            //create instance of SRE
            
            

            //Now we need to add the words we want our program to recognise
           
            var grammar = new Choices();
            grammar.Add("lal");
            grammar.Add("holud");
            grammar.Add("chhintai");
            grammar.Add("amar");
            grammar.Add("hello");
            


            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(grammar);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            var g = new Grammar(gb);

            //Set events for recognizing, hypothesising and rejecting speech
            sre.SpeechRecognized += SreSpeechRecognized;
            sre.SpeechHypothesized += SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
            sre.SetInputToAudioStream(
      sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            return sre;
        }

        //if speech is rejected
        private void RejectSpeech(RecognitionResult result)
        {
            //textBox2.Text = "Pardon Moi?";
            MessageBox.Show("Pardon Moi?");
        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            RejectSpeech(e.Result);
        }

        //hypothesized result
        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            //textBox1.Text = "Hypothesized: " + e.Result.Text + " " + e.Result.Confidence;
        }
        
        //Speech is recognised
        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //Very important! - change this value to adjust accuracy - the higher the value
            //the more accurate it will have to be, lower it if it is not recognizing you
           

            spchblk.AppendText("Recognized");

            if (e.Result.Text.ToLower() == "lal" && e.Result.Confidence >= 0.40)
            {

                //MessageBox.Show("You've just said lal");
                spchblk.AppendText("lal");

            }
            else
                if (e.Result.Text.ToLower() == "holud" && e.Result.Confidence >= 0.85)
                {
                    MessageBox.Show("You've just said holud");
                    spchblk.AppendText("holud");
                }
            if (e.Result.Text.ToLower() == "chhintai" && e.Result.Confidence >= 0.85)
            {
                MessageBox.Show("You are being mugged? :O ");
            }

            if (e.Result.Text.ToLower() == "hello" && e.Result.Confidence >= 0.85)
            {
                MessageBox.Show("Hi");
            }

            string status = "Recognized: " + e.Result.Text + " " + e.Result.Confidence;
            this.ReportSpeechStatus(status);

        }

        private void ReportSpeechStatus(string status)
        {
            Dispatcher.BeginInvoke(new Action(() => { ts.Text = status; }));
        }







        //************************************
        


        Boolean isTried = false;
        public MainWindow()
        {
            InitializeComponent();

        }

        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;
            sp = new SpeechSynthesizer();
            sp.Volume = 100;
            sp.Rate = 0;
            sp.SpeakAsync("Welcome to prohori");

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();
 
                //this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    //speechRecognizer = CreateSpeechRecognizer();

                    this.sensor.Start();
                    //speechRecognizer = CreateSpeechRecognizer();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
               
            }

            if (null == this.sensor)
            {
                //  this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }

            //Connect to the server
            teleConnector = new TeleConnector();
            teleConnector.connect("192.168.1.253");

            txtBox.Clear();
            txtBox.AppendText("Welcome to Prohori, please enter your card");


        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    //if (skeletonFrame.SkeletonArrayLength > 1)
                    //{
                    //    for(int i=0;i<10;i++)System
                    //}
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            int nofskeleton = 0;

            Boolean first;
            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                //MessageBox.Show("More than one\n");

                /*if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    if (first)
                    {
                         skeletonTracked = skeleton;
                         trackingId = skeleton.TrackingID;
       
                         first = false;
                     }

                    if (skeleton.TrackingID == trackingId)
                     {
               
                     }
                 }*/



                if (skeletons.Length != 0)
                {
                    if(skeletons.Length > 1)
                    {
                        
                        //Console.WriteLine("More than one skeleton");
                        //txtBox3.Clear();
                        //txtBox3.AppendText("More than one skeleton");
                        sp.SpeakAsync("More than one person detected\n");

                    }
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            modifyFrames(skeleton, allowedFrameNumber);
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);

            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);

            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);


            
            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }

            printInfo(skeleton);
        }

        int e1MidY = 26;
        int prevPos = 27;
        int prevPosX = 23;

        private double Distance(Joint A, Joint B)
        {

            double dist = 100000000;

            dist = Math.Sqrt((A.Position.X - B.Position.X) * (A.Position.X - B.Position.X) + (A.Position.Y - B.Position.Y) * (A.Position.Y - B.Position.Y) + (A.Position.Z - B.Position.Z) * (A.Position.Z - B.Position.Z));
            
            return dist;
        }


        private void printInfo(Skeleton skeleton)
        {

            //Get different joint coordinates
            Joint rightWrist = skeleton.Joints[JointType.WristRight];
            Joint leftWrist = skeleton.Joints[JointType.WristLeft];
            Joint head = skeleton.Joints[JointType.Head] ;
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            Joint rightHand = skeleton.Joints[JointType.HandRight];
            Joint hipRight = skeleton.Joints[JointType.HipRight];
            Joint hipLeft = skeleton.Joints[JointType.HipLeft];
            Joint rightShoulder = skeleton.Joints[JointType.ShoulderRight];
            Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
            Joint leftElbow = skeleton.Joints[JointType.ElbowLeft];
            Joint hipCenter = skeleton.Joints[JointType.HipCenter];
            Joint leftFoot = skeleton.Joints[JointType.FootLeft];
            Joint rightFoot = skeleton.Joints[JointType.FootRight];
            Joint shoulderCenter = skeleton.Joints[JointType.ShoulderCenter];

            //Output the different coordinates
            txtBox2.Clear();
            txtBox2.AppendText("Head(" + head.Position.X + ", " + head.Position.Y + "," + head.Position.Y+ ")\n"+
                "RightHand(" + rightHand.Position.X + ", " + rightHand.Position.Y + "," + rightHand.Position.Y + ")\n"+
                "LeftHand(" + leftHand.Position.X + ", " + leftHand.Position.Y + "," + leftHand.Position.Y + ")\n"+
                "LeftWrist(" + leftWrist.Position.X + ", " + leftWrist.Position.Y + "," + leftWrist.Position.Y + ")\n"+
                "RightWrist(" + rightWrist.Position.X + ", " + rightWrist.Position.Y + "," + rightWrist.Position.Y + ")\n");


            double lw2rw = 100 * Distance(leftWrist, rightWrist);
            double lw2hd = 100 * Distance(leftWrist, head);
            double rw2hd = 100 * Distance(rightWrist, head);

            double lh2rh = 100 * Distance(leftHand, rightHand);
            double lh2hd = 100 * Distance(leftHand, head);
            double rh2hd = 100 * Distance(rightHand, head);


            double hc2lh = 100 * Distance(hipCenter, leftHand);
            double hc2rh = 100 * Distance(hipCenter, rightHand);
            double hc2lf = 100 * Distance(hipCenter, leftFoot);
            double hc2rf = 100 * Distance(hipCenter, rightFoot);

            double rhInHip = 100 * Distance(rightHand, hipRight);
           


            //txtBox3.Clear();
            //txtBox3.AppendText("lh2rh: " + lh2rh + "\nlw2hd: "+ lw2hd + "\nrw2hd: "+ rw2hd + "\n");


            string sts = "Far";
           

            //Gesture 1
            if(lh2rh < 50 && lh2hd < 30 && rh2hd < 20) // Hands and Head are close
            {
                sts = "Close";  //
                doSound();
            }
            //Gesture 2

            if (lh2hd < 20 && rhInHip < 20) //if  left hand in is in head and right hand in hip 
            {
                doSound();
            }


            //txtBox3.AppendText("hc2lh: " + hc2lh + "\nhc2rh: " + hc2rh + "\nhc2lf: " + hc2lf + "\nhc2rf: " + hc2rf)


            //Gesture 3
            checkRightHandMove(10);

            //Gesture 4
            checkRightLegMove(skeleton, 10);

            //Gesture 5
            checkRightHandInHeapAndLeftInHead(skeleton, 10);

            //Gesture 6
            checkFootOverlapping(skeleton, 10);
        

            //for control
            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.JointType == JointType.WristRight)
                {
                    try
                    {
                        int x = (int)(100 * joint.Position.X);
                        int y = (int)(100 * joint.Position.Y);
                        int z = (int)(100 * joint.Position.Z);

     

                        int offset = y - prevPos;
                        int offsetX = prevPosX - x;

                        int speed = offset;
                        if (offset < 5 && offset > -5)
                        {
                            //  speed = 0;
                        }

                        if (isTried)        //if is tried to connect.
                        {
                            String cmd = "s1:" + speed + "\n";      //base joint. up and down.
          
                        }
                        //txtBox.Clear();
                        //txtBox.AppendText("Pos: " + x + "," + y + "," + z + " offset=" + offset);
                         
                        prevPos = y;
                        prevPosX = x;
                    }
                    catch (Exception ex)
                    {
                    }
                    break;
                }
            }
        }
        private void modifyFrames(Skeleton skeleton, int numFrame)
        {
            //store the frame,if frame size exceeds by 30 delete the first item
            prevFrames.Add(skeleton);
            //if sze>30 delete the oldest one
            if (prevFrames.Count > 30) prevFrames.RemoveAt(0);

        }

        private void checkRightHandMove(int numAlarm)
        {
            if (prevFrames.Count == allowedFrameNumber && (prevFrames[allowedFrameNumber - 1].Joints[JointType.WristRight].Position.X - prevFrames[0].Joints[JointType.WristRight].Position.X) > 0.35)
            {
                //for (int i = 0; i < numAlarm; i++) Console.Beep();
                doSound();
            }
        }

        private void checkRightLegMove(Skeleton skeleton, int numAlarm)
        {
            if (Math.Abs(skeleton.Joints[JointType.KneeRight].Position.Y - skeleton.Joints[JointType.AnkleRight].Position.Y) < 0.25)
            {
                //for (int i = 0; i < numAlarm; i++) Console.Beep();
                doSound();
            }
        }

        private void checkRightHandInHeapAndLeftInHead(Skeleton skeleton, int numAlarm)
        {
            double threshold = 0.2;
            bool right = (skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.HipRight].Position.X) < threshold && Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.HipRight].Position.Y) < threshold;

            bool left = Math.Abs(skeleton.Joints[JointType.HandLeft].Position.X - skeleton.Joints[JointType.Head].Position.X) < threshold && Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.Head].Position.Y) < threshold;

            if (left && right)
            {
                //for (int i = 0; i < numAlarm; i++) Console.Beep();
                doSound();
            }

        }

        private void checkFootOverlapping(Skeleton skeleton, int numAlarm)
        {
            //double threshold = 0.1;
            if ((Math.Abs(skeleton.Joints[JointType.FootRight].Position.X - skeleton.Joints[JointType.FootLeft].Position.X) < 0.075 && Math.Abs(skeleton.Joints[JointType.FootRight].Position.Y - skeleton.Joints[JointType.FootLeft].Position.Y) < 0.12)&&
                Math.Abs(skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.FootLeft].Position.X) < 0.075 && Math.Abs(skeleton.Joints[JointType.FootRight].Position.Y - skeleton.Joints[JointType.FootLeft].Position.Y) < 0.12)
            {
                //for (int i = 0; i < numAlarm; i++) Console.Beep();
                doSound();
            }
        }


        private void doSound()
        {
           // System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"E:\kolpokoushol project\SkeletonBasics-WPF\buz.wav");
           // player.Play();

            soundOn = true;

            //txtBox.Clear();
            //txtBox.AppendText("Detected");

            Console.WriteLine("detected\n");
            
           teleConnector.send("Hi");
           teleConnector.send("Hello");

            //soundOn = false;
           //teleConnector.send("Hello");
            
        }

        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }


        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
                //Thread thread = new Thread(new ThreadStart();
                //thread.Start();
                MessageBox.Show("Connected!");
          

        }


    }


    class TeleConnector
    {
        int port = 5424;
        String serverIP = "192.168.1.253";

        TcpClient client;
        StreamReader reader;
        StreamWriter writer;
        Boolean isConnected = false;
        public void connect(String server)
        {
            serverIP = server;
            try
            {
                client = new TcpClient(serverIP, port);
                isConnected = true;
               // MessageBox.Show("Connected");
                Stream s = client.GetStream();
                reader = new StreamReader(s);
                writer = new StreamWriter(s);
                writer.AutoFlush = true;
                Thread thread = new Thread(new ThreadStart(doRead));
                thread.Start();

            }
            catch (Exception ex)
            {
                //
                isConnected = false;
                throw new Exception();
            }
        }

        /*
         * Reader Thread
         * */

        private void doRead()
        {
            while (isConnected)
            {
                try
                {
                    String line = reader.ReadLine();
                    if (line != null)
                    {
                        // txtBox2.AppendText("rcv: " + line + "\n");
                    }
                }
                catch (Exception ex)
                {
                    // log errors
                }
            }
        }

        public void send(String line)
        {
            try
            {
                writer.WriteLine(line);
            }
            catch (Exception e){
                
            }
        }

        public void send_dxdydz(int dx, int dy, int dz)
        {
            String line = "dxdydz:" + dx + ":" + dy + ":" + dz;
            writer.WriteLine(line);
        }

        public void close()
        {
            try
            {
                client.Close();
            }
            catch (Exception ex)
            {

            }
            isConnected = false;
        }

    }








}