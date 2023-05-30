namespace PaperPlaneTools.AR {
	using OpenCvSharp;
	using UnityEngine;
	
	public class MyMainScript: WebCamera {

        #region Variables
		
		/// <summary>
		/// Mouth gameobject reference
		/// </summary>
		public GameObject mouthGO;

		/// <summary>
		/// Down mouth gameobject reference
		/// </summary>
		public Transform downMouthGO;

		//Brush tracking variables
		/// <summary>
		/// Brush GameObject reference
		/// </summary>
		public GameObject brushProxy;

		/// <summary>
		/// Brush GameObject reference
		/// </summary>
		public GameObject brushGO;

		// <summary>
        /// Processed texture
        /// </summary>
        public Mat image = null;

		/// <summary>
        /// Texture
        /// </summary>
        private Texture2D markerTexture;

		//Vectors for rotation
		private Vector3 vec90_0_0 = new Vector3(90,0,0);
		private Vector3 vec0_180_0 = new Vector3(0,180,0);
		private Vector3 vec180_180_180 = new Vector3(180,180,180);
		private Vector3 vec0_90_0 = new Vector3(0,90,0);

        //Reference for tracking scripts
        public MarkerTrackingScript markerTrackingScript;
		public FaceTrackingScript faceTrackingScript;
		public bool markerFlag = false;
		public bool faceFlag = false;

		private Plane facePosPlane = new Plane(Vector3.back, 0);
		private Ray faceRay;
		private float facePlaneDistance;

		/// <summary>
		///	Screen width
		/// </summary>
		//private float screenWidth;
		//screenWidth = ((RectTransform)Surface.transform).rect.width;
		#endregion
		
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
		}

		private void Start(){
			markerTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
		}

		protected override bool ProcessTexture() {

			//Get webcam pixels for markers
			markerTexture.SetPixels(webCamTexture.GetPixels());
			markerTrackingScript.img = Unity.TextureToMat(markerTexture, Unity.TextureConversionParams.Default);

			//Get texture for face tracking
			image = Unity.TextureToMat(webCamTexture as WebCamTexture, TextureParameters);

			//Dirty flag
			markerFlag = true;
			faceFlag = true;

			if (!ParentController.gamePaused) { 
				//Marker detector processing
				PositionBrush();

				//Locate mouth
				LocateMouth();
			}

			return true;
		}

		private void PositionBrush() {

			brushGO.transform.localRotation = markerTrackingScript.meanRot;
			brushGO.transform.localRotation = new Quaternion(-brushGO.transform.localRotation.x, brushGO.transform.localRotation.y, brushGO.transform.localRotation.z, -brushGO.transform.localRotation.w);
			brushGO.transform.localScale =  markerTrackingScript.curScale;
            brushProxy.transform.localPosition = markerTrackingScript.meanPos;

            //Locate object depending on marker
            switch (markerTrackingScript.smallerId){

				//Bottom marker
				case 0:
					brushGO.transform.Rotate(vec90_0_0);
                    brushGO.transform.Rotate(vec0_180_0);
					brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 2.5f);
				break;

				//Front marker
				case 2:
                    brushGO.transform.Rotate(vec0_180_0);
                    brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 1.5f + Vector3.back * 1.2f);
				break;

				//Back marker
				case 1:
                    brushGO.transform.Rotate(vec180_180_180);
                    brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 1.5f + Vector3.forward * 1.2f);
                    break;

				//Right marker
				case 3:
                    brushGO.transform.Rotate(-vec0_90_0);
                    brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 1.5f + Vector3.right * 1.2f);
				break;

				//Left marker
				case 4:
                    brushGO.transform.Rotate(vec0_90_0);
                    brushGO.transform.localPosition += brushGO.transform.TransformDirection(Vector3.up * 1.5f + Vector3.left * 1.2f);
                    break;

				default:
				break;
			}
        }
	
		private void LocateMouth(){

			//Move plane
			//Do ray things
			//Position mouth
			//mouthGO.transform.localPosition = faceTrackingScript.meanMouthPos;
			facePosPlane.SetNormalAndPosition(Vector3.back, Vector3.forward * faceTrackingScript.newValueZ);

            faceRay = Camera.main.ScreenPointToRay(faceTrackingScript.meanMouthPos);
            if (facePosPlane.Raycast(faceRay, out facePlaneDistance))
            {
                mouthGO.transform.localPosition = faceRay.GetPoint(facePlaneDistance);
            }

            //Rotate mouth
            mouthGO.transform.localRotation = Quaternion.Euler(0f, 180f, faceTrackingScript.prevFaceAngle * 55f);
			
			//Open mouth
			downMouthGO.localPosition = new Vector3(0f, faceTrackingScript.prevOpenning, 0f);
		}

        public void TurnOff(){
            webCamTexture.Stop();
			webCamTexture = null;
			webCamDevice = null;
        }
    }
}