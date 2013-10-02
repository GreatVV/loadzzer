using UnityEngine;

[System.Serializable]
/// <summary>
/// Sprite Animation Clip contains a collection of frames and associated properties required to play it.
/// </summary>
public class tk2dSpriteAnimationClip
{
    /// <summary>
    /// Name of animation clip
    /// </summary>
    public string name = "Default";
	
    /// <summary>
    /// Array of frames
    /// </summary>
    public tk2dSpriteAnimationFrame[] frames = null;
	
    /// <summary>
    /// FPS of clip
    /// </summary>
    public float fps = 30.0f;
	
    /// <summary>
    /// Defines the start point of the loop when <see cref="WrapMode.LoopSection"/> is selected
    /// </summary>
    public int loopStart = 0;
	
    /// <summary>
    /// Wrap mode for the clip
    /// </summary>
    public enum WrapMode
    {
        /// <summary>
        /// Loop indefinitely
        /// </summary>
        Loop,
		
        /// <summary>
        /// Start from beginning, and loop a section defined by <see cref="tk2dSpriteAnimationClip.loopStart"/>
        /// </summary>
        LoopSection,
		
        /// <summary>
        /// Plays the clip once and stops at the last frame
        /// </summary>
        Once,
		
        /// <summary>
        /// Plays the clip once forward, and then once in reverse, repeating indefinitely
        /// </summary>
        PingPong,
		
        /// <summary>
        /// Simply choses a random frame and stops
        /// </summary>
        RandomFrame,
		
        /// <summary>
        /// Starts at a random frame and loops indefinitely from there. Useful for multiple animated sprites to start at a different phase.
        /// </summary>
        RandomLoop,
		
        /// <summary>
        /// Switches to the selected sprite and stops.
        /// </summary>
        Single
    };
	
    /// <summary>
    /// The wrap mode.
    /// </summary>
    public WrapMode wrapMode = WrapMode.Loop;

    /// <summary>
    /// Default contstructor
    /// </summary>
    public tk2dSpriteAnimationClip() {

    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public tk2dSpriteAnimationClip(tk2dSpriteAnimationClip source) {
        CopyFrom( source );
    }

    /// <summary>
    /// Copies the source animation clip into the current one.
    /// All frames are duplicated.
    /// </summary>
    public void CopyFrom(tk2dSpriteAnimationClip source)
    {
        name = source.name;
        if (source.frames == null) 
        {
            frames = null;
        }
        else
        {
            frames = new tk2dSpriteAnimationFrame[source.frames.Length];
            for (int i = 0; i < frames.Length; ++i)
            {
                if (source.frames[i] == null)
                {
                    frames[i] = null;
                }
                else
                {
                    frames[i] = new tk2dSpriteAnimationFrame();
                    frames[i].CopyFrom(source.frames[i]);
                }
            }
        }
        fps = source.fps;
        loopStart = source.loopStart;
        wrapMode = source.wrapMode;
        if (wrapMode == tk2dSpriteAnimationClip.WrapMode.Single && frames.Length > 1)
        {
            frames = new tk2dSpriteAnimationFrame[] { frames[0] };
            Debug.LogError(string.Format("Clip: '{0}' Fixed up frames for WrapMode.Single", name));
        }
    }

    /// <summary>
    /// Clears the clip, removes all frames
    /// </summary>
    public void Clear()
    {
        name = "";
        frames = new tk2dSpriteAnimationFrame[0];
        fps = 30.0f;
        loopStart = 0;
        wrapMode = WrapMode.Loop;
    }

    /// <summary>
    /// Is the clip empty?
    /// </summary>
    public bool Empty
    {
        get { return name.Length == 0 || frames == null || frames.Length == 0; }
    }

    /// <summary>
    /// Gets the tk2dSpriteAnimationFrame for a particular frame
    /// </summary>
    public tk2dSpriteAnimationFrame GetFrame(int frame) {
        return frames[frame];
    }
}