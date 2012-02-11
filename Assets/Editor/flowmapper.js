class flowmapper extends EditorWindow {

// @MenuItem ("Component/FlowMachine/FlowMapper") <-- This tool does not show up under the Component submenu ( Reason unknown... )
@MenuItem ("GameObject/FlowMachine/FlowMapper")
	static function Init () {
		var window : flowmapper = EditorWindow.GetWindow ( flowmapper, false, "Flow Mapper" );
		window.Show ();
	}
	
	private var realtimeRender = false;
	private var target : Transform;
	private var probes : GameObject[];
	
	function OnGUI () {
		GUI.Label( Rect( 10, 10, 256, 18 ), "Flow texture target mesh" );
		target = EditorGUI.ObjectField( Rect( 10,30,256, 18 ), target, Transform, true );
		if( GUI.Button( Rect( 10, 50, 256,18 ), "Render" ) ) RenderFlow();
		realtimeRender = GUI.Toggle( Rect( 10, 80, 256, 18 ), realtimeRender, "Real time render" );
		
	}
	
	function Update() {
		if( realtimeRender ) {
			Repaint();
			RenderFlow();
		}
	}
	
	function RenderFlow() {
		
		// Get all probes
		probes = GameObject.FindGameObjectsWithTag("FlowProbe");
		
		if( target == null ) return;
		var meshF = target.GetComponent(MeshFilter) as MeshFilter;
		if( meshF == null ) return;
		
		// Make instance if mesh is not instance already
		var mesh : Mesh;
		if( meshF.sharedMesh.name.Substring(0,2) != "i_" ) {
			mesh = meshF.mesh;
			mesh.name = "i_" + mesh.name;
		} else {
			mesh = meshF.sharedMesh;
		}
		
		// Calculate flow vectors using scene FlowProbes
		var verts = mesh.vertices;
		var colors = mesh.colors;
		for( var i=0;i<verts.length;i++ ) {
			var vert = target.TransformPoint( verts[i] );
			colors[i] = CalcFlow( vert );
		}
		
		// Smooth vertex alpha colors
		SmoothVertexColors( mesh.triangles, colors );
		SmoothVertexColors( mesh.triangles, colors );
		
		mesh.colors = colors;
	}
	
	function CalcFlow( v : Vector3 ) : Color
	{
		var flow_direction = Vector3( 0.5, 0.5, 0.5 );
		for( var i=0;i<probes.length;i++ ) {
			var probe = probes[i];
			var PD = Vector3.Distance( probe.transform.position, v );
			var atten = 1.0 - Mathf.Clamp01( PD / probe.transform.localScale.magnitude );
			atten = Mathf.Pow( atten, 0.75 );
			var flow : Vector3 = target.InverseTransformDirection( -probe.transform.forward ) * atten;
			// Pack to 0..1
			flow = flow * 0.5 + Vector3( 0.5, 0.5, 0.5 );
			// Overlay blend
			flow_direction[0] = OverlayBlend( flow_direction[0], flow[0] );
			flow_direction[1] = OverlayBlend( flow_direction[1], flow[1] );
			flow_direction[2] = OverlayBlend( flow_direction[2], flow[2] );
		}
		
		var result : Color;
		result.r = flow_direction[0];
		result.g = flow_direction[1];
		result.b = flow_direction[2];
		
		// Write distance for alpha
		result.a = DistanceCheck( v );
		
		// Concentrate alpha blend
		result.a = Mathf.Clamp01( Mathf.Pow( result.a, 20 ) * 15.0 );
		
		return result;
	}
	
	function OverlayBlend( B : float, L : float ) {
		B *= 255.0;
		L *= 255.0;
		return ((L < 128.0) ? (2 * B * L / 255.0):(255.0 - 2.0 * (255.0 - B) * (255.0 - L) / 255.0)) / 255.0;
	}
	
	function DistanceCheck( v : Vector3 ) {
		var distAverage = 1.0;
		for( var i=0;i<32;i++ ) {
			var vec : Vector3;
			vec[0] = Random.Range( -1.0, 1.0 );
			vec[1] = Random.Range( -1.0, -0.125 ); // Never point vector up ( -1.0 .. -0.125 )
			vec[2] = Random.Range( -1.0, 1.0 );
			var hit : RaycastHit;
			if( Physics.Raycast( v + Vector3( 0, 0.5, 0 ), vec, hit, 1.0 )) {
				distAverage += Mathf.Min( 1.0, hit.distance );
				distAverage *= 0.5;
			}
		}
		return distAverage;
	}
	
	function SmoothVertexColors( triangles : int[], colors : Color[] ) {
		for( i=0;i<triangles.length;i+=3 ) {
			var color0 : Color = colors[ triangles[i] ];
			var color1 : Color = colors[ triangles[i+1] ];
			var color2 : Color = colors[ triangles[i+2] ];
			
			var avg_color : Color  = color0 + color1 + color2;
			avg_color *= 0.333;
			
			colors[ triangles[i] ] = avg_color;
			colors[ triangles[i+1] ] = avg_color;
			colors[ triangles[i+2] ] = avg_color;
		}
		return colors;
	}
}