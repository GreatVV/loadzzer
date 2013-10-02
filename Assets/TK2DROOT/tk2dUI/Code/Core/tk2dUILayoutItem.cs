using UnityEngine;

[System.Serializable]
public class tk2dUILayoutItem {
    public tk2dBaseSprite sprite = null;
    public tk2dUIMask UIMask = null;
    public tk2dUILayout layout = null;
    public GameObject gameObj = null;

    public bool snapLeft = false;
    public bool snapRight = false;
    public bool snapTop = false;
    public bool snapBottom = false;

    // ContainerSizer
    public bool fixedSize = false;
    public float fillPercentage = -1;
    public float sizeProportion = 1;
    public static tk2dUILayoutItem FixedSizeLayoutItem() {
        tk2dUILayoutItem item = new tk2dUILayoutItem();
        item.fixedSize = true;
        return item;
    }

    // Internal
    public bool inLayoutList = false;
    public int childDepth = 0;
    public Vector3 oldPos = Vector3.zero;
}