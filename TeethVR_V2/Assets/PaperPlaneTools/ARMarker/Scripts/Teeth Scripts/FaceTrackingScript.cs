namespace PaperPlaneTools.AR{
    using OpenCvSharp;
    using System.Threading;
    using System.Collections.Generic;
    using UnityEngine;

    public class FaceTrackingScript : MonoBehaviour
    {
        #region Variables
        public MyMainScript mainScript;

        public Camera mainCamera;

        private Thread faceTrackingThread;

        //Info of facetracking
		public TextAsset faces;
		public TextAsset eyes;
		public TextAsset shapes;

		/// <summary>
		/// Face tracking processor
		/// </summary>
		private OpenCvSharp.Demo.FaceProcessorLive<WebCamTexture> processor;

		//Mouth clamping values
		//Face position X values
		private const float OLDNEARMINX = 115.0f;
		private const float OLDFARMINX = 65.0f;	
		private const float OLDNEARMAXX = 530.0f;
		private const float OLDFARMAXX = 580.0f;
		private const float OLDRANGEX = 50.0f;

		//Face position Y values
		private const float OLDNEARMINY = 230.0f;
		private const float OLDFARMINY = 200.0f;
		private const float OLDNEARMAXY = 360.0f;		
		private const float OLDFARMAXY = 390.0f;
		private const float OLDRANGEY = 30.0f;

		//Face position clamped values
		private float oldMinX;
		private float oldMaxX;
		private float oldRangeX;
		private float oldMinY;
		private float oldMaxY;
		private float oldRangeY;

		//Screen position X values
		private const float NEWNEARMAXX = 0.06f;
		private const float NEWFARMAXX = 0.27f;
		private const float NEWRANGEX = 0.21f;
	
		//Screen position Y values
		private const float NEWNEARMINY = -0.08f;
		private const float NEWFARMINY = -0.45f;
		private const float NEWMINRANGEY = 0.23f;
		private const float NEWNEARMAXY = 0.15f;		
		private const float NEWFARMAXY = 0.35f;
		private const float NEWMAXRANGEY = 0.8f;	

		//Face position clamped values
		private float newMinX;
		private float newMaxX;
		private float newRangeX;
		private float newMinY;
		private float newMaxY;
		private float newRangeY;		

		//Screen position Z values
		private const float OLDRANGEZ = 195.0f;
		private const float OLDMAXVALUEZ = 300.0f;
		private const float OLDMINVALUEZ = 105f;
		private const float NEWRANGEZ = 1.5f;
		private const float NEWMINVALUEZ = 1.0f;
		private const float NEWMAXVALUEZ = 2.5f;

		//Screen distance between mouths
		private const float OLDNMINOPENNING = 6.0f;
		private const float OLDMAXOPENNING = 25.0f;
		private const float OLDRANGEOPENNING = 19.0f;
		// private const float OLDNEARMAXOPENNING = 45.0f;
		// private const float OLDFARMAXOPENNING = 10.0f;
		// private const float OLDMAXOPENNINGRANGE = 35.0f;

		// private float oldMaxOpenning;
		// private float oldOpenningRange;

		private const float NEWMAXOPENNING = -0.04f;
		private const float NEWMINOPENNING = 0f;
		private const float NEWRANGEOPENNING = 0.04f;

		private float newValueX, newValueY, newValueZ, newOpenning;

		//Previous mouth openning
		public float prevOpenning;

		//List of previous mouth positions
		private List<Vector3> prevMouthPos;

		//Mean of mouth positions
		public Vector3 meanMouthPos;

		/// <summary>
		/// Mouth position
		/// </summary>
		private Point mouthPos;

		/// <summary>
		/// Mouth position (in vec3)
		/// </summary>
		private Vector3 vec3MouthPos;

		/// <summary>
		/// Z distance of mouth
		/// </summary>
		private float zDistance;
		
		/// <summary>
		/// Face size
		/// </summary>
		private int faceHeight;

		/// <summary>
		/// Face angle
		/// </summary>
		private float faceAngle;

		/// <summary>
		/// Previous frame face angle
		/// </summary>
		public float prevFaceAngle;

		/// <summary>
		/// Distance between upper and lower lip
		/// </summary>
		private float mouthOpenning;
        #endregion

        void Awake() {
			processor = new OpenCvSharp.Demo.FaceProcessorLive<WebCamTexture>();
			processor.Initialize(faces.text, eyes.text, shapes.bytes);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = true;        // enable stabilizer
			processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
        }

        void Start()
        {
            mouthPos = new Point(0,0);
            vec3MouthPos = new Vector3(0,0,0);
			prevMouthPos = new List<Vector3>();
            faceTrackingThread = new Thread(FaceDetectionThread);
            faceTrackingThread.Start();
        }

        void FaceDetectionThread(){
            while(true){
                if(mainScript.image != null && mainScript.faceFlag){
                    //Face detection processing
                    processor.ProcessTexture(mainScript.image, mainScript.TextureParameters);
                    processor.MarkDetected(mainScript.image, ref mouthPos, ref mouthOpenning, ref faceHeight, ref faceAngle);
                    //renderedTexture = Unity.MatToTexture(processor.Image, renderedTexture);
                
                    //Calculate new position
                    #region ClampValues
                    //Get Z proportional
                    if(faceHeight <= OLDMINVALUEZ){
                        zDistance = 1;
                        newValueZ = NEWMAXVALUEZ;
                    }else if(faceHeight >= OLDMAXVALUEZ){
                        zDistance = 0;
                        newValueZ = NEWMINVALUEZ;
                    }else{

                        zDistance = (faceHeight - OLDMINVALUEZ) / OLDRANGEZ;
                        zDistance = 1 - zDistance;
                        //Clamp Z in screen values
                        newValueZ = (((faceHeight - OLDMINVALUEZ) * NEWRANGEZ) / OLDRANGEZ) + NEWMINVALUEZ;
                        newValueZ = (NEWMAXVALUEZ + NEWMINVALUEZ) - newValueZ;
                    }

                    //Get min and max X and Y face positions depending on distance to camera
                    oldMinX = OLDNEARMINX - (zDistance * OLDRANGEX);
                    oldMaxX = OLDNEARMAXX + (zDistance * OLDRANGEX);
                    oldRangeX = Mathf.Abs(oldMaxX - oldMinX);

                    oldMinY = OLDNEARMINY - (zDistance * OLDRANGEY);
                    oldMaxY = OLDNEARMAXY + (zDistance * OLDRANGEY);
                    oldRangeY = Mathf.Abs(oldMaxY - oldMinY);

                    //Get min and max X and Y screen positions depending on distance to camera
                    newMaxX = NEWNEARMAXX + (zDistance * NEWRANGEX);
                    newMinX = -newMaxX;
                    newRangeX = Mathf.Abs(newMaxX - newMinX);

                    newMinY = NEWNEARMINY - (zDistance * NEWMINRANGEY);
                    newMaxY = NEWNEARMAXY + (zDistance * NEWMAXRANGEY);
                    newRangeY = Mathf.Abs(newMaxY - newMinY);
                    #endregion

                    #region XPos
                    //Clamp X in screen values
                    if(mouthPos.X <= oldMinX)
                        newValueX = newMinX;
                    else if(mouthPos.X >= oldMaxX)
                        newValueX = newMaxX;
                    else
                        newValueX = (((mouthPos.X - oldMinX) * newRangeX) / oldRangeX) + newMinX;
                    #endregion

                    #region YPos
                    //Clamp Y in screen values
                    if(mouthPos.Y <= oldMinY)
                        newValueY = newMinY;
                    else if(mouthPos.Y >= oldMaxY)
                        newValueY = newMaxY;
                    else
                        newValueY = (((mouthPos.Y - oldMinY) * newRangeY) / oldRangeY) + newMinY;
                    newValueY = (newMaxY + newMinY) - newValueY;			
                    #endregion

                    #region MouthPos
                    //Remove excesive positions
                    prevMouthPos.Add(new Vector3(-newValueX, newValueY, newValueZ));
                    if(prevMouthPos.Count >= 5){

                        prevMouthPos.RemoveAt(0);
                        //Compute mean matrix using prev poses
                        meanMouthPos = (prevMouthPos[3] * 0.6f) + (prevMouthPos[2] * 0.25f) + (prevMouthPos[1] * 0.10f) + (prevMouthPos[0] * 0.05f);

                    }else{

                        meanMouthPos = prevMouthPos[prevMouthPos.Count - 1];
                    }
                    #endregion

                    #region MouthRotation
                    if(Mathf.Abs(prevFaceAngle - faceAngle) > 0.05)
                        prevFaceAngle = faceAngle;
                    #endregion

                    #region MouthOpenning
                    //Get proportion of mouth openning
                    if(mouthOpenning < OLDNMINOPENNING){
                        newOpenning = NEWMINOPENNING;
                    }else if(mouthOpenning > OLDMAXOPENNING){
                    // }else if(mouthOpenning > oldMaxOpenning){
                        newOpenning = NEWMAXOPENNING;
                    }else{
                        newOpenning = -(((mouthOpenning - OLDNMINOPENNING) * NEWRANGEOPENNING) / OLDRANGEOPENNING);
                    }

                    //Open mouth
                    if(Mathf.Abs(prevOpenning - newOpenning) > 0.0025)
                        prevOpenning = newOpenning;
                    #endregion
                }
                //Sleep
                //Thread.Sleep(200);
            }
        }

        private void OnDestroy() {
            if(faceTrackingThread.ThreadState != ThreadState.Aborted){
                Debug.Log("Killing Face Tracking thread");
                faceTrackingThread.Abort();
            }
        }

        void OnApplicationQuit(){
            if(faceTrackingThread.ThreadState != ThreadState.Aborted){
                Debug.Log("Killing Face Tracking thread");
                faceTrackingThread.Abort();
            }
        }
    }
}
