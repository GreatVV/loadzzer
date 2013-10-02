using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// Mirrors the Unity camera class properties
/// </summary>
public class tk2dCameraSettings {
	public enum ProjectionType {
		Orthographic,
		Perspective
	}

	public enum OrthographicType {
		PixelsPerMeter,
		OrthographicSize,
	}

	public enum OrthographicOrigin {
		BottomLeft,
		Center
	}

	public ProjectionType projection = ProjectionType.Orthographic;
	public float orthographicSize = 10.0f;
	public float orthographicPixelsPerMeter = 20;
	public OrthographicOrigin orthographicOrigin = OrthographicOrigin.Center;
	public OrthographicType orthographicType = OrthographicType.PixelsPerMeter;
	public float fieldOfView = 60.0f;
	public Rect rect = new Rect( 0, 0, 1, 1 );
}