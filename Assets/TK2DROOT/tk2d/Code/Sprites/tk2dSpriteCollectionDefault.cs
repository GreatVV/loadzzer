using UnityEngine;

[System.Serializable]
public class tk2dSpriteCollectionDefault
{
    public bool additive = false;
    public Vector3 scale = new Vector3(1,1,1);
    public tk2dSpriteCollectionDefinition.Anchor anchor = tk2dSpriteCollectionDefinition.Anchor.MiddleCenter;
    public tk2dSpriteCollectionDefinition.Pad pad = tk2dSpriteCollectionDefinition.Pad.Default;	
	
    public tk2dSpriteCollectionDefinition.ColliderType colliderType = tk2dSpriteCollectionDefinition.ColliderType.UserDefined;
}