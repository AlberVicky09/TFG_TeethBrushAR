namespace PaperPlaneTools.AR {
	using OpenCvSharp;
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections.Generic;
	
	public class MyMainScript: WebCamera {

        #region Variables
		//Face tracking variables
		public Text posTxt;
		
		/// <summary>
		/// Mouth gameobject reference
		/// </summary>
		public GameObject mouthGO;

		/// <summary>
		/// Down mouth gameobject reference
		/// </summary>
		private Transform downMouthGO;

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
		private float prevOpenning;

		//List of previous mouth positions
		private List<Vector3> prevMouthPos;

		//Mean of mouth positions
		private Vector3 meanMouthPos;

		/// <summary>
		/// Mouth position
		/// </summary>
		private Point mouthPos;

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
		private float prevFaceAngle;

		/// <summary>
		/// Distance between upper and lower lip
		/// </summary>
		private float mouthOpenning;

		//Brush tracking variables
		/// <summary>
		/// Brush GameObject reference
		/// </summary>
		public GameObject brushGO;

		/// <summary>
		/// The marker detector
		/// </summary>
		private MarkerDetector markerDetector;

		/// <summary>
		/// List of previous poses
		/// </summary>
		private List<Vector3> prevPos;
		/// <summary>
		/// Mean position
		/// </summary>
		private Vector3 meanPos;
		/// <summary>
		/// Previous rotation
		/// </summary>
		private Quaternion prevRot;
		/// <summary>
		/// Current rotation
		/// </summary>
		private Quaternion actRot;
		/// <summary>
		/// Mean rotation
		/// </summary>
		private Quaternion meanRot;

		/// <summary>
		///	Screen width
		/// </summary>
		private float screenWidth;
		#endregion
		
        void Start () {
			markerDetector = new MarkerDetector ();
            prevPos = new List<Vector3>();
			prevRot = new Quaternion(0f, 0f, 0f, 0f);
			actRot = new Quaternion(0f, 0f, 0f, 0f);
			mouthPos = new Point(0,0);
			prevMouthPos = new List<Vector3>();
            downMouthGO = mouthGO.gameObject.transform.GetChild(0);
			screenWidth = ((RectTransform)Surface.transform).rect.width;
		}

		protected override void Awake() {
			int cameraIndex = -1;
			for (int i = 0; i < WebCamTexture.devices.Length; i++) {
				WebCamDevice webCamDevice = WebCamTexture.devices [i];
				if (webCamDevice.isFrontFacing == true){
					cameraIndex = i;
					break;
				}
				if (cameraIndex < 0) {
					cameraIndex = i;
				}
			}

			if (cameraIndex >= 0) {
				DeviceName = WebCamTexture.devices [cameraIndex].name;
				//webCamDevice = WebCamTexture.devices [cameraIndex];
			}

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

		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output) {
			//Marker detector processing
			var texture = new Texture2D(input.width, input.height);
			texture.SetPixels(input.GetPixels());
			var img = Unity.TextureToMat(texture, Unity.TextureConversionParams.Default);
			ProcessFrame(img, img.Cols, img.Rows);
			output = Unity.MatToTexture(img, output);

			//Face detection processing
			processor.ProcessTexture(input, TextureParameters);
			processor.MarkDetected(ref mouthPos, ref mouthOpenning, ref faceHeight, ref faceAngle);
			output = Unity.MatToTexture(processor.Image, output);
			posTxt.text = "X: " + mouthPos.X + "\nY: " + mouthPos.Y + "\nZ: " + faceHeight;

			//Locate mouth
			LocateMouth(mouthPos.X, mouthPos.Y);

			return true;
		}

		private void ProcessFrame (Mat mat, int width, int height) {
			List<int> markerIds = markerDetector.Detect (mat, width, height);

            if(markerIds.Count > 0)
                ProcessMarkers(markerIds);
			
		}

        private void ProcessMarkers(List<int> foundedMarkers) {

			//Get lower marker
			int smallerId = foundedMarkers[0];
			int idPos = 0;
			for(int i = 0; i < foundedMarkers.Count; i++){
				if(foundedMarkers[i] < smallerId){
					smallerId = foundedMarkers[i];
					idPos = i;
				}
			}
			
			//Locate GO using first marker
			Matrix4x4 transformMatrix = markerDetector.TransfromMatrixForIndex(idPos);
			//Position gameobject
			PositionObject(transformMatrix, smallerId);
		}

		private void PositionObject(Matrix4x4 transformMatrix, int id) {
		    //Debug.Log("Marker is: " + id);
			Matrix4x4 matrixY = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, -1, 1));
			Matrix4x4 matrixZ = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, 1, -1));
			Matrix4x4 matrix = matrixY * transformMatrix * matrixZ;
			
			//
			
			//Add actual pos to list
			Vector3 pos = MatrixHelper.GetPosition(matrix);
			pos.x = -pos.x;
			prevPos.Add(pos);

			//Remove excesive positions
			if(prevPos.Count >= 5){

				prevPos.RemoveAt(0);
				//Compute mean matrix using prev poses
				meanPos = (prevPos[3] * 0.6f + prevPos[2] * 0.25f + prevPos[1] * 0.10f + prevPos[0] * 0.05f);

			}else{

				meanPos = prevPos[prevPos.Count - 1];
			}
			
			//Avoid vibration
			actRot = MatrixHelper.GetQuaternion(matrix);
			if(Quaternion.Angle(prevRot, actRot) > 5){
				meanRot = Quaternion.Slerp(prevRot, actRot, 0.75f);
				prevRot = actRot;
			}else{
				meanRot = prevRot;
			}

			brushGO.transform.localPosition = meanPos;
			brushGO.transform.localRotation = meanRot;
			brushGO.transform.localRotation = new Quaternion(-brushGO.transform.rotation.x, brushGO.transform.rotation.y, brushGO.transform.rotation.z, -brushGO.transform.rotation.w);
			brushGO.transform.localScale = MatrixHelper.GetScale(matrix) * 0.15f;
			//Debug.Log(meanRot.eulerAngles);

			//Locate object depending on marker
			switch(id){

				//Bottom marker
				case 0:
					brushGO.transform.Rotate(new Vector3(90, 0, 0));
					brushGO.transform.Rotate(new Vector3(0, 180, 0));
					brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up) * 2.5f;
				break;

				//Front marker
				case 2:
					brushGO.transform.Rotate(new Vector3(0, 180, 0));	
					brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 1.5f + Vector3.back);
				break;

				//Back marker
				case 1:
					brushGO.transform.Rotate(new Vector3(180, 180, 180));
					brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 1.5f + Vector3.forward);
				break;

				//Right marker
				case 4:
					brushGO.transform.Rotate(new Vector3(0, -90, 0));
					brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 1.5f + Vector3.right);
				break;

				//Left marker
				case 3:
					brushGO.transform.Rotate(new Vector3(0, 90, 0));
					brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 1.5f + Vector3.left);
				break;

				default:
				break;
			}	
        }
	
		private void LocateMouth(float oldX, float oldY){
			
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

			//Clamp X in screen values
			if(oldX <= oldMinX)
				newValueX = newMinX;
			else if(oldX >= oldMaxX)
				newValueX = newMaxX;
			else
				newValueX = (((oldX - oldMinX) * newRangeX) / oldRangeX) + newMinX;

			//Clamp Y in screen values
			if(oldY <= oldMinY)
				newValueY = newMinY;
			else if(oldY >= oldMaxY)
				newValueY = newMaxY;
			else
				newValueY = (((oldY - oldMinY) * newRangeY) / oldRangeY) + newMinY;
			newValueY = (newMaxY + newMinY) - newValueY;			
			
			//Remove excesive positions
			prevMouthPos.Add(new Vector3(-newValueX, newValueY, newValueZ));
			if(prevMouthPos.Count >= 5){

				prevMouthPos.RemoveAt(0);
				//Compute mean matrix using prev poses
				meanMouthPos = (prevMouthPos[3] * 0.6f + prevMouthPos[2] * 0.25f + prevMouthPos[1] * 0.10f + prevMouthPos[0] * 0.05f);

			}else{

				meanMouthPos = prevMouthPos[prevMouthPos.Count - 1];
			}

			//Position mouth
			mouthGO.transform.position = meanMouthPos;

			//Rotate mouth
			if(Mathf.Abs(prevFaceAngle - faceAngle) > 0.05){
				mouthGO.transform.rotation = Quaternion.Euler(0f, 180f, faceAngle * 55f);
				prevFaceAngle = faceAngle;
			}else{
				mouthGO.transform.rotation = Quaternion.Euler(0f, 180f, prevFaceAngle * 55f);
			}
			
			//Get min and max X and Y face positions depending on distance to camera
			// oldMaxOpenning = OLDNEARMAXOPENNING - (zDistance * OLDMAXOPENNINGRANGE);
			// oldOpenningRange = Mathf.Abs(oldMaxOpenning - OLDNMINOPENNING);

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
			if(Mathf.Abs(prevOpenning - newOpenning) > 0.0025){
				downMouthGO.localPosition = new Vector3(0f, newOpenning, 0f);
				prevOpenning = newOpenning;
			}else{
				downMouthGO.localPosition = new Vector3(0f, prevOpenning, 0f);
			}
		}

        public void TurnOff(){
            webCamTexture.Stop();
			webCamTexture = null;
			webCamDevice = null;
        }
    }
}