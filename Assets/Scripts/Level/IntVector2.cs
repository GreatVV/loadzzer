using System;

[Serializable]
public class IntVector2
{
    public int x;
    public int y;

    public IntVector2()
    {
        x = y = 0;
    }

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}