namespace PaperPlaneTools.AR{
    using OpenCvSharp;
    using UnityEngine;
    using System.Threading;
    using System.Collections.Generic;
    using System;

    public class MarkerTrackingScript : MonoBehaviour
    {
        #region Variables
        private Thread markerThread;
        /// <summary>
        /// Main script
        /// </summary>
        public MyMainScript mainScript;

        /// <summary>
        /// The marker detector
        /// </summary>
        private MarkerDetector markerDetector;

        /// <summary>
        /// Marker id
        /// </summary>
        public int smallerId;
        /// <summary>
        /// Marker location
        /// </summary>
        public int idPos;

        /// <summary>
        /// List of previous poses
        /// </summary>
        private List<Vector3> prevPos;
        /// <summary>
        /// Mean position
        /// </summary>
        public Vector3 meanPos;
        /// <summary>
        /// Previous rotation
        /// </summary>
        private Quaternion prevRot;
        /// <summary>
        /// Current rotation
        /// </summary>
        private Quaternion curRot;
        /// <summary>
        /// Mean rotation
        /// </summary>
        public Quaternion meanRot;
        /// <summary>
        /// Current scale
        /// </summary>
        public Vector3 curScale;

        /// <summary>
        /// Texture
        /// </summary>
        public Mat img = null;

        /// <summary>
        /// Found marker arrays
        /// </summary>
        private List<int> markerIds;

        /// <summary>
        /// Transform matrix of marker
        /// </summary>
        private Matrix4x4 transformMatrix;
        /// <summary>
        /// Y matrix
        /// </summary>
        private Matrix4x4 matrixY;
        /// <summary>
        /// Z matrix
        /// </summary>
        private Matrix4x4 matrixZ;
        /// <summary>
        /// Transformed matrix
        /// </summary>
        private Matrix4x4 matrix;

        /// <summary>
        /// Actual position of marker
        /// </summary>
        private Vector3 pos;
        #endregion

        void Start()
        {
            markerDetector = new MarkerDetector ();
            prevPos = new List<Vector3>();
			prevRot = new Quaternion(0f, 0f, 0f, 0f);
			curRot = new Quaternion(0f, 0f, 0f, 0f);
            matrixY = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, -1, 1));
			matrixZ = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, 1, -1));
            markerThread = new Thread(MarkerDetectionThread);
            markerThread.Start();
        }

        void MarkerDetectionThread(){
            while(true){
                if(img != null && mainScript.markerFlag){            
                    markerIds = markerDetector.Detect (img, img.Cols, img.Rows);

                    if(markerIds.Count > 0){
                        //Get lower marker
                        smallerId = markerIds[0];
                        idPos = 0;
                        for(int i = 1; i < markerIds.Count; i++){
                            if(markerIds[i] < smallerId){
                                smallerId = markerIds[i];
                                idPos = i;
                            }
                        }
                    
                        //Locate GO using first marker
                        transformMatrix = markerDetector.TransfromMatrixForIndex(idPos);
                        //Get transformed matrix
                        matrix = matrixY * transformMatrix * matrixZ;

                        //Add actual pos to list
                        pos = MatrixHelper.GetPosition(matrix);
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
                        curRot = MatrixHelper.GetQuaternion(matrix);
                        if (Quaternion.Angle(prevRot, curRot) > 5)
                        {
                            meanRot = Quaternion.Slerp(prevRot, curRot, 0.75f);
                            prevRot = curRot;
                        }
                        else
                        {
                            meanRot = prevRot;
                        }

                        curScale = MatrixHelper.GetScale(matrix);
  
                    }//else
                        //Debug.Log("No marker!");
                }
            }
        }
    
        private void OnDestroy() {
            if(markerThread.ThreadState != ThreadState.Aborted){
                Debug.Log("Killing Marker thread");
                markerThread.Abort();
            }
        }

        void OnApplicationQuit(){
            if(markerThread.ThreadState != ThreadState.Aborted){
                Debug.Log("Killing Marker thread");
                markerThread.Abort();
            }
        }
    }
}
