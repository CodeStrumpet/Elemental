using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Settings")]
	[Tooltip("Write Settings to Disk")]
	public class SaveSettings : FsmStateAction
	{
	    [RequiredField]
		public FsmString xmlFilename;

	    [RequiredField]
		public FsmGameObject mainCameraObject;

	    [RequiredField]
		public FsmGameObject meshObject;

	    public override void Reset()
	    {
	    }

	    public override void OnEnter()
	    {
		Finish();
		
		string path = xmlFilename.Value;
		XmlTextWriter textWriter = new XmlTextWriter(path, null) ;
		GameObject go;
		textWriter.WriteStartDocument(); 
		{
		    textWriter.WriteStartElement("Settings", "");
		    {
			go = mainCameraObject.Value;
			textWriter.WriteStartElement("MainCamera", "");
			{
			    WriteTransform(textWriter, go);
			    Camera camera = go.GetComponent(typeof(Camera)) as Camera;
			    WriteValue(textWriter, "OrthographicSize", camera.orthographicSize);
			}
			textWriter.WriteEndElement();
			go = meshObject.Value;
			textWriter.WriteStartElement("Mesh", "");
			{
			    WriteTransform(textWriter, go);
			}
			textWriter.WriteEndElement();
		    }
		    textWriter.WriteEndElement();
		}
		textWriter.WriteEndDocument(); 
		textWriter.Close();  
	    }

	    void WriteValue(XmlTextWriter textWriter, string key, float value) {
		textWriter.WriteStartElement(key, "");
		textWriter.WriteValue(value);
		textWriter.WriteEndElement();
	    }

	    void WriteTransform(XmlTextWriter textWriter, GameObject go) {
		textWriter.WriteStartElement("Transform", "");
		WritePosition(textWriter, go);
		WriteRotation(textWriter, go);
		WriteScale(textWriter, go);
		textWriter.WriteEndElement();
	    }

	    void WritePosition(XmlTextWriter textWriter, GameObject go) {
		textWriter.WriteStartElement("Position", "");
		WriteValue(textWriter, "X", go.transform.position.x);
		WriteValue(textWriter, "Y", go.transform.position.y);
		WriteValue(textWriter, "Z", go.transform.position.z);
		textWriter.WriteEndElement();
	    }

	    void WriteRotation(XmlTextWriter textWriter, GameObject go) {
		textWriter.WriteStartElement("Rotation", "");
		WriteValue(textWriter, "X", go.transform.eulerAngles.x);
		WriteValue(textWriter, "Y", go.transform.eulerAngles.y);
		WriteValue(textWriter, "Z", go.transform.eulerAngles.z);
		textWriter.WriteEndElement();
	    }

	    void WriteScale(XmlTextWriter textWriter, GameObject go) {
		textWriter.WriteStartElement("Scale", "");
		WriteValue(textWriter, "X", go.transform.localScale.x);
		WriteValue(textWriter, "Y", go.transform.localScale.y);
		WriteValue(textWriter, "Z", go.transform.localScale.z);
		textWriter.WriteEndElement();
	    }
	}
}
