using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class LoadImageAndCrop : MonoBehaviour {

	public GameObject imgObj;
	public GameObject imgSlctArea;
	public Text savePathText;
	public InputField inputMinX;
	public InputField inputMinY;
	public InputField inputMaxX;
	public InputField inputMaxY;
	public InputField inputLengthX;
	public InputField inputLengthY;
	public Text infoText;
	string imgPath = "";
	string[] loadPath;
	string[] multiExtensions;
	string savePath = "";
	Texture[] imgUploaded;
	float PercentMinX=0;
	float PercentMinY=0;
	float PercentMaxX=100;
	float PercentMaxY=100;
	float LengthX=0;
	float LengthY=0;
	int minX;
	int minY;
	int maxX;
	int maxY;
	bool UsePercent = true;
	float slctWidth;
	float slctHeight;

	void Start () {
		loadPath =  new string[1];
		multiExtensions = new string[]{ ".png",".jpg",".jpeg",".jpe",".jfif",".tif",".tiff",".gif",".bmp",".dib",".tga"};
		slctWidth = imgSlctArea.GetComponent<RectTransform> ().rect.width;
		slctHeight = imgSlctArea.GetComponent<RectTransform> ().rect.height;
	}

	public void LoadImg(){
		imgPath =  EditorUtility.OpenFilePanel(
			"Choose an Image",
			"",
			"png,jpg,jpeg,jpe,jfif,tif,tiff,gif,bmp,dib,tga");
		if (imgPath.Length != 0) {
			loadPath [0] = imgPath;
			WWW www = new WWW("file:///" + loadPath[0]);
			imgObj.GetComponent<RawImage> ().texture = www.texture;
		}	
	}

	public void LoadFolder(){
		imgPath =  EditorUtility.OpenFolderPanel(
			"Choose the Directory",
			"",
			"");
		if (imgPath.Length != 0) {
			try{
				loadPath = Directory.GetFiles (imgPath, "*.*", SearchOption.AllDirectories).Where(extFile => multiExtensions.Any(extFile.ToLower().EndsWith)).ToArray();
				//Debug.Log (loadPath.Length);
			} 
			catch (DirectoryNotFoundException dirEx) {
				Debug.Log ("Directory Not Found: "+dirEx.Message);
			}
			if (loadPath.Length > 0){
				WWW www = new WWW("file:///" + loadPath[0]);
				imgObj.GetComponent<RawImage> ().texture = www.texture;
				//Debug.Log (loadPath[0]);
			} else{
				imgPath="";
				EditorUtility.DisplayDialog(
					"Choose Image(s) to Upload",
					"There Is No Image in This Folder!",
					"Ok");
			}
		}
	}

	public void SaveToFolder(){
		savePath = EditorUtility.OpenFolderPanel(
			"Choose the Directory",
			"",
			"");
		if (savePath.Length != 0) {
			savePathText.text = savePath;
		}
	}

	public void CropFiles(){
		if (savePath.Length != 0) {
			if (imgPath.Length != 0) {
				for (int i=0; i<loadPath.Length; i++){
					WWW www = new WWW("file:///" + loadPath[i]);
					StartCoroutine (waitForImgCrop (i, www));
				}
			} else {
				EditorUtility.DisplayDialog(
				"Choose Image(s) to Upload",
					"You Must Choose Image(s) to Upload first!",
				"Ok");
			}
		} else {
			EditorUtility.DisplayDialog(
			"Select Save Path",
			"You Must Select Save Path first!",
			"Ok");
		}
	}

	IEnumerator waitForImgCrop(int i, WWW www){
		yield return www;
		Texture2D srcImg = new Texture2D(1,1);;
		www.LoadImageIntoTexture (srcImg);
		int w = srcImg.width;
		int h = srcImg.height;
		minX = Mathf.RoundToInt(PercentMinX * w/100);
		minY = Mathf.RoundToInt(PercentMinY * h/100);
		if (UsePercent) {
			maxX = Mathf.RoundToInt(PercentMaxX * w/100);
			maxY = Mathf.RoundToInt(PercentMaxY * h/100);
		} else {
			maxX = Mathf.RoundToInt(minX + LengthX);
			maxY = Mathf.RoundToInt(minY + LengthY);
		}
		maxX = Mathf.Max (minX + 1, maxX);
		maxY = Mathf.Max (minY + 1, maxY);
		maxX = Mathf.Min (w, maxX);
		maxY = Mathf.Min (h, maxY);
		Texture2D cropTex = doCropTex (minX,minY,maxX-minX,maxY-minY, srcImg);
		byte[] imgCropFile = cropTex.EncodeToPNG ();
		Object.Destroy (srcImg);
		Object.Destroy (cropTex);
		File.WriteAllBytes(savePath + "/"+ Path.GetFileNameWithoutExtension(loadPath[i]) +".png", imgCropFile);
		Debug.Log (Path.GetFileNameWithoutExtension(loadPath[i]));
	}

	Texture2D doCropTex(int x, int y, int width, int height, Texture2D sourceImg){
		Color[] cropPixels = sourceImg.GetPixels (x, y, width, height);
		Texture2D destTex = new Texture2D (width, height);
		destTex.SetPixels (cropPixels);
		destTex.Apply();
		return destTex;
	}
		
	public void GetCropArea () {
		if (inputMinX.text.Length>0)
		PercentMinX = float.Parse(inputMinX.text);
		if (inputMinY.text.Length>0)
		PercentMinY = float.Parse(inputMinY.text);
		if (inputMaxX.text.Length>0)
		PercentMaxX = float.Parse(inputMaxX.text);
		if (inputMaxY.text.Length>0)
		PercentMaxY = float.Parse(inputMaxY.text);
		if (inputLengthX.text.Length>0)
		LengthX = float.Parse(inputLengthX.text);
		if (inputLengthY.text.Length>0)
		LengthY = float.Parse(inputLengthY.text);
		infoText.text = "";
//		if (LengthX < 0) {
//			LengthX = 0;
//			infoText.text = "LengthX cannot be smaller than 0 pixel.";
//		}
//		if (LengthY < 0) {
//			LengthY = 0;
//			infoText.text = "LengthX cannot be smaller than 0 pixel.";
//		}

//		if (PercentMinX < 0) {
//			PercentMinX = 0;
//			infoText.text = "Percent MinX cannot be smaller than 0%.";
//		}
//		if (PercentMinY < 0) {
//			PercentMinY = 0;
//			infoText.text = "Percent MinY cannot be smaller than 0%.";
//		}
		PercentMinX = Mathf.Max (PercentMinX, 0);
		PercentMinY = Mathf.Max (PercentMinY, 0);
		PercentMaxX = Mathf.Max (PercentMaxX, 0.01f);
		PercentMaxY = Mathf.Max (PercentMaxY, 0.01f);
		LengthX = Mathf.Max (LengthX, 0);
		LengthY = Mathf.Max (LengthY, 0);
		if ((LengthX > 0) && (LengthY > 0)) {
			UsePercent = false;
			infoText.text = "If LengthX and LengthY both are larger than 0, the image(s) will be cropped using LengthX, LengthY instead of Percent MaxX, MaxY.";
		} else {
			UsePercent = true;
		}
		if (PercentMaxX > 100) {
			PercentMaxX = 100;
			infoText.text = "Percent MaxX cannot be larger than 100%.";
		}
		if (PercentMaxY > 100) {
			PercentMaxY = 100;
			infoText.text = "Percent MaxY cannot be larger than 100%.";
		}
		if (PercentMinX >= PercentMaxX) {
			PercentMinX = Mathf.Max(PercentMaxX - 0.01f,0);
			infoText.text = "Percent MinX must be smaller than MaxX.";
		}
		if (PercentMinY >= PercentMaxY) {
			PercentMinY = Mathf.Max(PercentMaxY - 0.01f,0);
			infoText.text = "Percent MinY must be smaller than MaxY.";
		}
		inputMinX.text = PercentMinX.ToString();
		inputMinY.text = PercentMinY.ToString();
		inputMaxX.text = PercentMaxX.ToString();
		inputMaxY.text = PercentMaxY.ToString();
		inputLengthX.text = LengthX.ToString ();
		inputLengthY.text = LengthY.ToString ();
		//imgSlctArea.GetComponent<RectTransform>().position=(inputMinX,inputMinY,0);
		imgSlctArea.GetComponent<RectTransform>().anchoredPosition = new Vector3(PercentMinX*slctWidth/100,PercentMinY*slctHeight/100,0);
		if (UsePercent) {
			imgSlctArea.GetComponent<RectTransform> ().sizeDelta = new Vector2 ((PercentMaxX - PercentMinX) * slctWidth/100, (PercentMaxY - PercentMinY) * slctHeight/100);
		} else {
			imgSlctArea.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Mathf.Min(slctWidth*(100-PercentMinX)/100,LengthX), Mathf.Min(slctHeight*(100-PercentMinY)/100,LengthY));
		}
	}
		



//	public void OpenAndLoadFiles(string filePath){
//		bool openInsideOfFolder = false;
//		string winPath = filePath.Replace("/", "\\"); // windows explorer doesn't like forward slashes
//
//		if ( System.IO.Directory.Exists(winPath) )
//		{
//			openInsideOfFolder = true;
//		}
//
//		try
//		{
//			System.Diagnostics.Process.Start("explorer.exe", (openInsideOfFolder ? "/root," : "/select,") + winPath);
//
//		}
//		catch ( System.ComponentModel.Win32Exception e )
//		{
//			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
//		}
//	}


}
