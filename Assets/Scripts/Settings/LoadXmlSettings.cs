using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class LoadXmlSettings : MonoBehaviour {
    public string xmlFilename;
    public GameObject mainCameraObject;
    public GameObject meshObject;
    public bool autoLoad = true;

    void Start() {
	if (autoLoad) Load();
    }

    void Update() {
	if (Input.GetKeyDown (KeyCode.Space)) {
	    Debug.Log("Reloading Settings...");
	    Load();
	}
    }

    void Load() {
	string path = xmlFilename;
	if (!File.Exists(path)) return;
		
	XmlTextReader textReader = new XmlTextReader(path);
	ScanToNode(textReader, "MainCamera");
	ReadTransform(textReader, mainCameraObject);
	ReadOrthographicSize(textReader, mainCameraObject);
	ScanToNode(textReader, "Mesh");
	ReadTransform(textReader, meshObject);
    }

    void ReadOrthographicSize(XmlTextReader textReader, GameObject go) {
	ScanToNode(textReader, "OrthographicSize");
	float size = GetFloat(textReader);
	Camera camera = go.GetComponent(typeof(Camera)) as Camera;
	camera.orthographicSize = size;
    }
	    
    void ReadTransform(XmlTextReader textReader, GameObject go) {
	ScanToNode(textReader, "Transform");
	ReadPosition(textReader, go);
	ReadRotation(textReader, go);
	ReadScale(textReader, go);
    }

    void ReadPosition(XmlTextReader textReader, GameObject go) {
	ScanToNode(textReader, "Position");
	ScanToNode(textReader, "X");
	float x, y, z;
	x = GetFloat(textReader);
	ScanToNode(textReader, "Y");
	y = GetFloat(textReader);
	ScanToNode(textReader, "Z");
	z = GetFloat(textReader);
	go.transform.position = new Vector3(x, y, z);
    }

    void ReadRotation(XmlTextReader textReader, GameObject go) {
	ScanToNode(textReader, "Rotation");
	ScanToNode(textReader, "X");
	float x, y, z;
	x = GetFloat(textReader);
	ScanToNode(textReader, "Y");
	y = GetFloat(textReader);
	ScanToNode(textReader, "Z");
	z = GetFloat(textReader);
	go.transform.eulerAngles = new Vector3(x, y, z);
    }

    void ReadScale(XmlTextReader textReader, GameObject go) {
	ScanToNode(textReader, "Scale");
	ScanToNode(textReader, "X");
	float x, y, z;
	x = GetFloat(textReader);
	ScanToNode(textReader, "Y");
	y = GetFloat(textReader);
	ScanToNode(textReader, "Z");
	z = GetFloat(textReader);
	go.transform.localScale = new Vector3(x, y, z);
    }

    void ScanToNode(XmlTextReader textReader, string name) {
	if (CheckCurrentNode(textReader, name)) return;
	while (textReader.Read()) {
	    if (CheckCurrentNode(textReader, name)) return;
	}
	Debug.Log("ERROR! Could noto find XML element named: " + name);
    }

    bool CheckCurrentNode(XmlTextReader textReader, string name) {
	XmlNodeType nType = textReader.NodeType;
	// if node type is an element
	if (nType == XmlNodeType.Element) {
	    if (textReader.Name.ToString() == name) {
		return true;
	    }
	}
	return false;
    }
	    
    float GetFloat(XmlTextReader textReader) {
	return (float) textReader.ReadElementContentAs(typeof(float), null);	    
    }
}
