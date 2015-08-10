using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using MiniJSON;

/// <summary>
/// Object detect sample.
/// </summary>
public class ObjectDetectSample : MonoBehaviour
{
		/// <summary>
		/// The texture array.
		/// </summary>
		public Texture2D[] textureArray;

		/// <summary>
		/// The index of the texture.
		/// </summary>
		int textureIndex = 0;

		/// <summary>
		/// The result prefab.
		/// </summary>
		public GameObject resultPrefab;

		/// <summary>
		/// The result game object.
		/// </summary>
		IList<GameObject> resultGameObjects;
	    

		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start ()
		{

				//Reads the cascade file to be used for object detection.
				#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5
				OpenCVObjectDetector.LoadCascade ("haarcascade_frontalface_alt");
				OpenCVObjectDetector.LoadCascade ("haarcascade_mcs_lefteye");
				OpenCVObjectDetector.LoadCascade ("haarcascade_mcs_righteye");
				OpenCVObjectDetector.LoadCascade ("haarcascade_mcs_nose");
				OpenCVObjectDetector.LoadCascade ("haarcascade_mcs_mouth");

				#endif

				resultGameObjects = new List<GameObject> ();
		}
	
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update ()
		{


		}

		/// <summary>
		/// Raises the destroy event.
		/// </summary>
		void OnDestroy ()
		{
				#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5
		
				OpenCVObjectDetector.UnloadAllCascade ();
		
				#endif
		}

		/// <summary>
		/// Raises the GU event.
		/// </summary>
		void OnGUI ()
		{
				float screenScale = Screen.width / 240.0f;
				Matrix4x4 scaledMatrix = Matrix4x4.Scale (new Vector3 (screenScale, screenScale, screenScale));
				GUI.matrix = scaledMatrix;

				GUILayout.BeginVertical ();
		
				if (GUILayout.Button ("Show License")) {
						Application.LoadLevel ("ShowLicense");
				}
			
				if (GUILayout.Button ("Simple Faces Detect(Sync)")) {
						Texture2D texture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;

						OpenCVObjectDetector.RemoveAllObjectDetectorParam ();
			
			
						IDictionary<string,object> param = new Dictionary<string,object> ();

						//set OpenCV cvHaarDetectObjects() params.
						param.Add ("filename", "haarcascade_frontalface_alt");
						param.Add ("scaleFactor", 1.1);
						param.Add ("minNeighbors", 2);
						param.Add ("flags", 0 | OpenCVObjectDetector.CV_HAAR_SCALE_IMAGE);
						param.Add ("minWidth", 80);
						param.Add ("minHeight", 80);

						//flip the image in Detect(), because Color32[] that GetPixels32() of Texture2D return is bottom-left origin.
						param.Add ("flipCode", 0);

						Debug.Log ("Simple Faces Detect(Sync) param " + Json.Serialize (param));
			
						OpenCVObjectDetector.AddObjectDetectorParam (Json.Serialize (param));

						OpenCVObjectDetector.Detect (texture, gameObject.name, "SimpleFacesDetectCallback");



				}

				if (GUILayout.Button ("Simple Faces Detect(Async)")) {
						Texture2D texture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;

						OpenCVObjectDetector.RemoveAllObjectDetectorParam ();
			
			
						IDictionary<string,object> param = new Dictionary<string,object> ();
						param.Add ("filename", "haarcascade_frontalface_alt");
						param.Add ("scaleFactor", 1.1);
						param.Add ("minNeighbors", 2);
						param.Add ("flags", 0 | OpenCVObjectDetector.CV_HAAR_SCALE_IMAGE);
						param.Add ("minWidth", 80);
						param.Add ("minHeight", 80);
						param.Add ("flipCode", 0);

						Debug.Log ("Simple Faces Detect(Async) param " + Json.Serialize (param));
			
						OpenCVObjectDetector.AddObjectDetectorParam (Json.Serialize (param));
			
						OpenCVObjectDetector.DetectAsync (texture, gameObject.name, "SimpleFacesDetectCallback");
				}

				if (GUILayout.Button ("Face Parts Detect(Async)")) {

						Texture2D texture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;

						OpenCVObjectDetector.RemoveAllObjectDetectorParam ();

						Dictionary<string,object> param = new Dictionary<string,object> ();
						param.Add ("filename", "haarcascade_frontalface_alt");
						param.Add ("scaleFactor", 1.1);
						param.Add ("minNeighbors", 2);
						param.Add ("flags", 0 | OpenCVObjectDetector.CV_HAAR_SCALE_IMAGE);
						param.Add ("minWidth", texture.width / 20);
						param.Add ("minHeight", texture.height / 20);
						param.Add ("flipCode", 0);

						Debug.Log ("Face Parts Detect(Async) param " + Json.Serialize (param));
			
						OpenCVObjectDetector.AddObjectDetectorParam (Json.Serialize (param));
			
						OpenCVObjectDetector.DetectAsync (texture, gameObject.name, "DetectFacesCallback");

				}

				if (GUILayout.Button ("Change Picture")) {
						textureIndex++;
						if (textureIndex > textureArray.Length - 1)
								textureIndex = 0;
						GetComponent<Renderer> ().material.mainTexture = textureArray [textureIndex];

						gameObject.transform.localScale = new Vector3 (GetComponent<Renderer> ().material.mainTexture.width, GetComponent<Renderer> ().material.mainTexture.height, 1);

			         
						//Distroy resultGameObjects;
						foreach (GameObject result in resultGameObjects) {
								GameObject.Destroy (result);
						}
						resultGameObjects.Clear ();

				}

				GUILayout.EndVertical ();
		}

		/// <summary>
		/// Simples the faces detect callback.
		/// </summary>
		/// <param name="result">Result.</param>
		void SimpleFacesDetectCallback (string result)
		{
				Debug.Log ("SimpleFacesDetectCallback result" + result);

				string json = result;
		
				IDictionary detects = (IDictionary)Json.Deserialize (json);
		
				foreach (DictionaryEntry detect in detects) {
						Debug.Log ("detects key " + detect.Key);
			
						string key = (string)detect.Key;
			
						if (key.Equals ("error")) {
								Debug.Log ((string)detects [detect.Key]);
						} else {
				
								IList<object> rects = (IList<object>)detects [detect.Key];


				                

								//flip Rects by convenient method,
								IList<object> flipRects = OpenCVObjectDetector.FlipRects (rects, ((Texture2D)GetComponent<Renderer> ().material.mainTexture).width, ((Texture2D)GetComponent<Renderer> ().material.mainTexture).height, 0);
				
				
								#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5
								OpenCVObjectDetector.DrawRects ((Texture2D)GetComponent<Renderer> ().material.mainTexture, Json.Serialize (flipRects), 0, 0, 255, 2);
								#endif


								ResultRectsToResultGameObjects (flipRects, new Color (0.0f, 0.0f, 1.0f, 0.3f), -40);

						}
				}
		}

		/// <summary>
		/// Detects the faces callback.
		/// </summary>
		/// <param name="result">Result.</param>
		void DetectFacesCallback (string result)
		{
				Debug.Log ("DetectFacesCallback result" + result);
		
				Texture2D texture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		
				string json = result;
		
				IDictionary detects = (IDictionary)Json.Deserialize (json);
		
				foreach (DictionaryEntry detect in detects) {
						Debug.Log ("detects key " + detect.Key);
			
						string key = (string)detect.Key;
			
						if (key.Equals ("error")) {
								Debug.Log ((string)detects [detect.Key]);
						} else {
				
								IList<object> rects = (IList<object>)detects [detect.Key];

								//flip Rects by convenient method,
								IList<object> flipRects = OpenCVObjectDetector.FlipRects (rects, texture.width, texture.height, 0);
				
				
								#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5
								OpenCVObjectDetector.DrawRects (texture, Json.Serialize (flipRects), 0, 0, 255, 2);
								#endif

								ResultRectsToResultGameObjects (flipRects, new Color (0.0f, 0.0f, 1.0f, 0.3f), -40);

				
								OpenCVObjectDetector.RemoveAllObjectDetectorParam ();


								int id = 0;
								foreach (IDictionary rect in rects) {
					
										IDictionary<string,object> leftEyeParam = new Dictionary<string,object> ();
										leftEyeParam.Add ("filename", "haarcascade_mcs_lefteye");
										leftEyeParam.Add ("scaleFactor", 1.1);
										leftEyeParam.Add ("minNeighbors", 2);
										leftEyeParam.Add ("flags", 0 | OpenCVObjectDetector.CV_HAAR_SCALE_IMAGE);
										leftEyeParam.Add ("minWidth", (long)rect ["width"] / 15);
										leftEyeParam.Add ("minHeight", (long)rect ["height"] / 15);
										leftEyeParam.Add ("flipCode", 0);
					
										IList<object> leftEyeRects = new List<object> ();
										IDictionary<string,long> leftEyeRect = new Dictionary<string,long> ();
										leftEyeRect.Add ("x", (long)rect ["x"] + (long)rect ["width"] / 3);
										leftEyeRect.Add ("y", (long)rect ["y"]);
										leftEyeRect.Add ("width", (long)rect ["width"] / 3 * 2);
										leftEyeRect.Add ("height", (long)rect ["height"] / 3 * 2);
										leftEyeRect.Add ("id", id);
										leftEyeRects.Add (leftEyeRect);
										leftEyeParam.Add ("rects", leftEyeRects);
					
										OpenCVObjectDetector.AddObjectDetectorParam (Json.Serialize (leftEyeParam));
					
					
					
					
										IDictionary<string,object> rightEyeParam = new Dictionary<string,object> ();
										rightEyeParam.Add ("filename", "haarcascade_mcs_righteye");
										rightEyeParam.Add ("scaleFactor", 1.1);
										rightEyeParam.Add ("minNeighbors", 2);
										rightEyeParam.Add ("flags", 0 | OpenCVObjectDetector.CV_HAAR_SCALE_IMAGE);
										rightEyeParam.Add ("minWidth", (long)rect ["width"] / 15);
										rightEyeParam.Add ("minHeight", (long)rect ["height"] / 15);
										rightEyeParam.Add ("flipCode", 0);
					
										IList<object> rightEyeRects = new List<object> ();
										IDictionary<string,long> rightEyeRect = new Dictionary<string,long> ();
										rightEyeRect.Add ("x", (long)rect ["x"]);
										rightEyeRect.Add ("y", (long)rect ["y"]);
										rightEyeRect.Add ("width", (long)rect ["width"] / 3 * 2);
										rightEyeRect.Add ("height", (long)rect ["height"] / 3 * 2);
										rightEyeRect.Add ("id", id);
										rightEyeRects.Add (rightEyeRect);
					
										rightEyeParam.Add ("rects", rightEyeRects);
					
										OpenCVObjectDetector.AddObjectDetectorParam (Json.Serialize (rightEyeParam));
					
					
					
										IDictionary<string,object> noseParam = new Dictionary<string,object> ();
										noseParam.Add ("filename", "haarcascade_mcs_nose");
										noseParam.Add ("scaleFactor", 1.1);
										noseParam.Add ("minNeighbors", 2);
										noseParam.Add ("flags", 0 | OpenCVObjectDetector.CV_HAAR_SCALE_IMAGE);
										noseParam.Add ("minWidth", (long)rect ["width"] / 15);
										noseParam.Add ("minHeight", (long)rect ["height"] / 15);
										noseParam.Add ("flipCode", 0);
					
					
										IList<object> noseRects = new List<object> ();
					
										IDictionary<string,long> noseRect = new Dictionary<string,long> ();
										noseRect.Add ("x", (long)rect ["x"] + (long)rect ["width"] / 4);
										noseRect.Add ("y", (long)rect ["y"] + (long)rect ["height"] / 2);
										noseRect.Add ("width", (long)rect ["width"] / 2);
										noseRect.Add ("height", (long)rect ["height"] / 3);
										noseRect.Add ("id", id);
										noseRects.Add (noseRect);
					
										noseParam.Add ("rects", noseRects);
					
										OpenCVObjectDetector.AddObjectDetectorParam (Json.Serialize (noseParam));
					
					
					
										IDictionary<string,object> mouthParam = new Dictionary<string,object> ();
										mouthParam.Add ("filename", "haarcascade_mcs_mouth");
										mouthParam.Add ("scaleFactor", 1.1);
										mouthParam.Add ("minNeighbors", 2);
										mouthParam.Add ("flags", 0 | OpenCVObjectDetector.CV_HAAR_SCALE_IMAGE);
										mouthParam.Add ("minWidth", (long)rect ["width"] / 10);
										mouthParam.Add ("minHeight", (long)rect ["height"] / 10);
										mouthParam.Add ("flipCode", 0);
					
					
										IList<object> mouthRects = new List<object> ();
					
										IDictionary<string,long> mouthRect = new Dictionary<string,long> ();
										mouthRect.Add ("x", (long)rect ["x"]);
										mouthRect.Add ("y", (long)rect ["y"] + (long)rect ["height"] / 2);
										mouthRect.Add ("width", (long)rect ["width"]);
										mouthRect.Add ("height", (long)rect ["height"] / 2);
										mouthRect.Add ("id", id);
										mouthRects.Add (mouthRect);

					
										mouthParam.Add ("rects", mouthRects);
					
										OpenCVObjectDetector.AddObjectDetectorParam (Json.Serialize (mouthParam));
					
										id++;
					
								}
				
								OpenCVObjectDetector.DetectAsync (texture, gameObject.name, "DetectFacePartsCallback");
				
						}
				}
		}

		/// <summary>
		/// Detects the face parts callback.
		/// </summary>
		/// <param name="result">Result.</param>
		void DetectFacePartsCallback (string result)
		{
				Debug.Log ("DetectFacePartsCallback result" + result);
		
				Texture2D texture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		
				string json = result;
		
				IDictionary detects = (IDictionary)Json.Deserialize (json);

		
				foreach (DictionaryEntry detect in detects) {
			
						Debug.Log ("detects key " + detect.Key);
			
						string key = (string)detect.Key;
			
						if (key.Equals ("error")) {
								Debug.Log ((string)detects [detect.Key]);
						} else {
				
								int r = 0;
								int g = 0;
								int b = 0;
				
				
								if (key.Equals ("haarcascade_mcs_lefteye")) {
										r = 255;
										g = 0;
										b = 0;
								} else if (key.Equals ("haarcascade_mcs_righteye")) {
										r = 0;
										g = 255;
										b = 0;
								} else if (key.Equals ("haarcascade_mcs_nose")) {
										r = 255;
										g = 255;
										b = 0;
								} else if (key.Equals ("haarcascade_mcs_mouth")) {
										r = 0;
										g = 255;
										b = 255;
								}
				
								IList<object> rects = (IList<object>)detects [detect.Key];

								//flip Rects by convenient method,
								IList<object> flipRects = OpenCVObjectDetector.FlipRects (rects, texture.width, texture.height, 0);
				
								#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5
								OpenCVObjectDetector.DrawRects (texture, Json.Serialize (flipRects), r, g, b, 2);
								#endif

								ResultRectsToResultGameObjects (flipRects, new Color ((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, 0.3f), -80);

						}
				}
		}

		/// <summary>
		/// Results the rects to result game objects.
		/// </summary>
		/// <param name="rects">Rects.</param>
		/// <param name="color">Color.</param>
		/// <param name="zPos">Z position.</param>
		private void ResultRectsToResultGameObjects (IList<object> rects, Color color, float zPos)
		{

				Vector3[] resultPoints = new Vector3[rects.Count];



				float textureWidth = GetComponent<Renderer> ().material.mainTexture.width;
				float textureHeight = GetComponent<Renderer> ().material.mainTexture.height;

				Matrix4x4 transCenterM = 
			Matrix4x4.TRS (new Vector3 (-textureWidth / 2, -textureHeight / 2, 0), Quaternion.identity, new Vector3 (1, 1, 1));


				Vector3 translation = new Vector3 (gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);

				Quaternion rotation = 
			Quaternion.Euler (gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z);

				Vector3 scale = new Vector3 (gameObject.transform.localScale.x / textureWidth, gameObject.transform.localScale.y / textureHeight, 1);

				Matrix4x4 trans2Dto3DM = 
			Matrix4x4.TRS (translation, rotation, scale);


				for (int i = 0; i < resultPoints.Length; i++) {
						IDictionary rect = (IDictionary)rects [i];

						//get center of rect.
						resultPoints [i] = new Vector3 ((long)rect ["x"] + (long)rect ["width"] / 2, (long)rect ["y"] + (long)rect ["height"] / 2, 0);


						//translate origin to center.
						resultPoints [i] = transCenterM.MultiplyPoint3x4 (resultPoints [i]);

						//transform from 2D to 3D
						resultPoints [i] = trans2Dto3DM.MultiplyPoint3x4 (resultPoints [i]);


						//Add resultGameObject.
						GameObject result = Instantiate (resultPrefab, resultPoints [i], Quaternion.identity) as GameObject;
						result.transform.parent = gameObject.transform;

						result.transform.localPosition = new Vector3 (result.transform.localPosition.x, result.transform.localPosition.y, zPos);
						result.transform.localEulerAngles = new Vector3 (0, 0, 0);
						result.transform.localScale = new Vector3 ((long)rect ["width"] / textureWidth, (long)rect ["height"] / textureHeight, 20);

						result.GetComponent<Renderer> ().material.color = color;

						resultGameObjects.Add (result);
				}

		}

		
}
