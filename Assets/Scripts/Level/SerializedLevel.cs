using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalBlock
{   
    public int x;
    public int y;

    public int toX;
    public int toY;    
}

public class SerializedLevel {

    public int Width;
    public int Height;

    public List<PortalBlock> blockedZoneAndPortals;
    

}
