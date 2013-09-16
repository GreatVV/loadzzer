using UnityEngine;
using System.Collections;

public class Handler : MonoBehaviour {

	void OnMapClick()
    {
        Application.LoadLevel("map");
    }
}
