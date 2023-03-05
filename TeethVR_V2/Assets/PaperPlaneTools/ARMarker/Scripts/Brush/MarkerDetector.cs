namespace PaperPlaneTools.AR {
	using UnityEngine;
	using System.Collections;
	using System.Runtime.InteropServices;
	using System;
	using System.Collections.Generic;
	using OpenCvSharp;
	using OpenCvSharp.Aruco;

	public class MarkerDetector {
		// List of detected markers
		private List<Matrix4x4> markerTransforms;
		private Matrix4x4 prevMarkerTransform;
		private List<int> result;
		// Create default parameres for detection
		DetectorParameters detectorParameters;
		// Dictionary holds set of all available markers
		Dictionary dictionary;
		// Variables to hold results
		Point2f[][] corners;
		int[] ids;
		Point2f[][] rejectedImgPoints;
		// Convert image to grasyscale
		Mat grayMat;
		static float markerSizeInMeters = 1f;
		Point3f[] markerPoints;

		double[] distCoeffs;
		double[] rvec;
		double[] tvec;
		double[,] rotMat;
		
		Matrix4x4 matrix;
		double[,] cameraMatrix;

		double max_wh;
		double fx;
		double fy;
		double cx;
		double cy;

		Vector4 auxRow;
		Vector4 w1;

		/// <summary>
		/// Initializes a new instance of the <see cref="PaperPlaneTools.AR.MarkerDetector"/> class.
		/// </summary>
		public MarkerDetector() {
			markerTransforms = new List<Matrix4x4>();
			result = new List<int>();
			grayMat = new Mat ();
			markerPoints = new Point3f[] {
				new Point3f(-markerSizeInMeters / 2f,  markerSizeInMeters / 2f, 0f),
				new Point3f( markerSizeInMeters / 2f,  markerSizeInMeters / 2f, 0f),
				new Point3f( markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f),
				new Point3f(-markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f)
			};

			distCoeffs = new double[4] {0d, 0d, 0d, 0d};
			rvec = new double[3]{0d, 0d, 0d};
			tvec = new double[3]{0d, 0d, 0d};
			rotMat = new double[3, 3] {{0d, 0d, 0d}, {0d, 0d, 0d}, {0d, 0d, 0d}};

			matrix = new Matrix4x4();
			cameraMatrix = new double[3, 3];

			auxRow = new Vector4();
			w1 = new Vector4(0f, 0f, 0f, 1f);
			
			detectorParameters = DetectorParameters.Create();
			dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_50);
		}


		/// <summary>
		/// Detect markers.
		/// </summary>
		/// <param name="pixels">
		///   The image where to detect markers.
		///   For example, you can use get pixels from web camera https://docs.unity3d.com/ScriptReference/WebCamTexture.GetPixels32.html
		/// </param>
		public List<int> Detect(Mat mat, int width, int height) {
			markerTransforms.Clear ();
			
			grayMat = Mat.Zeros (mat.Size (), MatType.CV_8UC1);

			Cv2.CvtColor (mat, grayMat, ColorConversionCodes.BGR2GRAY);
			
			// Detect markers
			CvAruco.DetectMarkers (grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

			Debug.Log(ids.Length);

//			CvAruco.DrawDetectedMarkers (mat, corners, ids);

			max_wh = (double)Math.Max (width, height);
			fx = max_wh;
			fy = max_wh;
			cx = width / 2.0d;
			cy = height / 2.0d;
//			
			cameraMatrix[0,0] = fx;
			cameraMatrix[0,1] = cx;
			cameraMatrix[1,1] = fy;
			cameraMatrix[1,2] = cy;

			for (int i=0; i<ids.Length; i++) {

				Cv2.SolvePnP(markerPoints, corners[i], cameraMatrix, distCoeffs, out rvec, out tvec, false, SolvePnPFlags.Iterative);

//				CvAruco.DrawAxis(mat, cameraMatrix, distCoeffs, rvec, tvec, 1.0f);
				Cv2.Rodrigues (rvec, out rotMat);
				auxRow.x = (float)rotMat[0, 0]; auxRow.y = (float)rotMat[0, 1]; auxRow.z = (float)rotMat[0, 2]; auxRow.w = (float)tvec[0];
				matrix.SetRow(0, auxRow);
				auxRow.x = (float)rotMat[1, 0]; auxRow.y = (float)rotMat[1, 1]; auxRow.z = (float)rotMat[1, 2]; auxRow.w = (float)tvec[1];
				matrix.SetRow(1, auxRow);
				auxRow.x = (float)rotMat[2, 0]; auxRow.y = (float)rotMat[2, 1]; auxRow.z = (float)rotMat[2, 2]; auxRow.w = (float)tvec[2];
				matrix.SetRow(2, auxRow);
				matrix.SetRow(3, w1);

				result.Add(ids[i]);
				markerTransforms.Add(matrix);
			}

			return result;
		}

		/// <summary>
		/// Return transfrom matrix for previously detected markers
		/// </summary>
		/// <returns>Return transfrom matrix for previously detected markers</returns>
		/// <param name="markerIndex">Index in the result liist of <see cref="PaperPlaneTools.AR.MarkerDetector.Detect"/> function</param>
		public Matrix4x4 TransfromMatrixForIndex(int markerIndex) {
			if (markerTransforms.Count != 0)
			{
				prevMarkerTransform = markerTransforms[markerIndex];
				return markerTransforms[markerIndex];
			}
			else { 
				return prevMarkerTransform;
            }
		}
	}

}