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
		/// Start this instance.
		/// </summary>
		void Start ()
		{

				//Reads the cascade file to be used for object detection.
				#if (UNITY_ANDROID||UNITY_IPHONE) && !UNITY_EDITOR
		OpenCVObjectDetector.LoadCascade("haarcascade_frontalface_alt");
		OpenCVObjectDetector.LoadCascade("haarcascade_mcs_lefteye");
		OpenCVObjectDetector.LoadCascade("haarcascade_mcs_righteye");
		OpenCVObjectDetector.LoadCascade("haarcascade_mcs_nose");
		OpenCVObjectDetector.LoadCascade("haarcascade_mcs_mouth");

				#endif
		
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
				#if (UNITY_ANDROID||UNITY_IPHONE) && !UNITY_EDITOR
		
		OpenCVObjectDetector.UnloadAllCascade();
		
				#endif
		}

		/// <summary>
		/// Raises the GU event.
		/// </summary>
		void OnGUI ()
		{
				if (GUI.Button (new Rect (10, 10, 200, 80), "Simple Faces Detect(Sync)")) {
						Texture2D texture = (Texture2D)renderer.material.mainTexture;

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

				if (GUI.Button (new Rect (230, 10, 200, 80), "Simple Faces Detect(Async)")) {
						Texture2D texture = (Texture2D)renderer.material.mainTexture;

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

				if (GUI.Button (new Rect (10, 110, 200, 80), "Face Parts Detect(Async)")) {

						Texture2D texture = (Texture2D)renderer.material.mainTexture;

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

				if (GUI.Button (new Rect (230, 110, 200, 80), "Change Picture")) {
						textureIndex++;
						if (textureIndex > textureArray.Length - 1)
								textureIndex = 0;
						renderer.material.mainTexture = textureArray [textureIndex];

						gameObject.transform.localScale = new Vector3 (renderer.material.mainTexture.width, renderer.material.mainTexture.height, 1);
				}
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
								IList<object> flipRects = OpenCVObjectDetector.FlipRects (rects, ((Texture2D)renderer.material.mainTexture).width, ((Texture2D)renderer.material.mainTexture).height, 0);
				
				
								#if (UNITY_ANDROID||UNITY_IPHONE) && !UNITY_EDITOR
				OpenCVObjectDetector.DrawRects((Texture2D)renderer.material.mainTexture,Json.Serialize(flipRects),0,0,255,2);
								#endif

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
		
				Texture2D texture = (Texture2D)renderer.material.mainTexture;
		
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
				
				
								#if (UNITY_ANDROID||UNITY_IPHONE) && !UNITY_EDITOR
				OpenCVObjectDetector.DrawRects(texture,Json.Serialize(flipRects),0,0,255,2);
								#endif
				
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
		
				Texture2D texture = (Texture2D)renderer.material.mainTexture;
		
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
				
								#if (UNITY_ANDROID||UNITY_IPHONE) && !UNITY_EDITOR
				OpenCVObjectDetector.DrawRects(texture,Json.Serialize(flipRects),r,g,b,2);
								#endif
						}
				}
		}

		
}
