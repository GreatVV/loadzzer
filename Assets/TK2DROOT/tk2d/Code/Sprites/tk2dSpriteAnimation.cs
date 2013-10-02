using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteAnimation")]
/// <summary>
/// Holds a collection of clips
/// </summary>
public class tk2dSpriteAnimation : MonoBehaviour 
{
	/// <summary>
	/// Array of <see cref="tk2dSpriteAnimationClip">clips</see>
	/// </summary>
	public tk2dSpriteAnimationClip[] clips;
	
	/// <summary>
	/// Resolves an animation clip by name and returns a reference to it
	/// </summary>
	/// <returns> tk2dSpriteAnimationClip reference, null if not found </returns>
	/// <param name='name'>Case sensitive clip name, as defined in <see cref="tk2dSpriteAnimationClip"/>. </param>
	public tk2dSpriteAnimationClip GetClipByName(string name)
	{
		for (int i = 0; i < clips.Length; ++i)
			if (clips[i].name == name) return clips[i];
		return null;
	}

	/// <summary>
	/// Resolves an animation clip by id and returns a reference to it
	/// </summary>
	/// <returns> tk2dSpriteAnimationClip reference, null if not found </returns>
	public tk2dSpriteAnimationClip GetClipById(int id) {
		if (id < 0 || id >= clips.Length || clips[id].Empty) {
			return null;
		}
		else {
			return clips[id];
		}
	}

	/// <summary>
	/// Resolves an animation clip by name and returns a clipId
	/// </summary>
	/// <returns> Unique clip id, -1 if not found </returns>
	/// <param name='name'>Case sensitive clip name, as defined in <see cref="tk2dSpriteAnimationClip"/>. </param>
	public int GetClipIdByName(string name) {
		for (int i = 0; i < clips.Length; ++i)
			if (clips[i].name == name) return i;
		return -1;
	}

	/// <summary>
	/// Gets a clip id from a clip
	/// </summary>
	/// <returns> Unique clip id, -1 if not found in the animation collection </returns>
	public int GetClipIdByName(tk2dSpriteAnimationClip clip) {
		for (int i = 0; i < clips.Length; ++i)
			if (clips[i] == clip) return i;
		return -1;
	}

	/// <summary>
	/// The first valid clip in the animation collection. Null if no valid clips are found.
	/// </summary>
	public tk2dSpriteAnimationClip FirstValidClip {
		get {
			for (int i = 0; i < clips.Length; ++i) {
				if (!clips[i].Empty && clips[i].frames[0].spriteCollection != null && clips[i].frames[0].spriteId != -1) {
					return clips[i];
				}
			}
			return null;
		}
	}
}


