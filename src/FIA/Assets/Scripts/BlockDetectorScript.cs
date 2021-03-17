
public class BlockDetectorScript : DetectorScript
{
    void FixedUpdate()
    {
        ObjectInfo anObject = GetClosestWall();
        strength = (anObject != null) ? 1.0f / (anObject.distance + 1.0f) : 0;
        angleToClosestObject = (anObject != null) ? anObject.angle : 0;
    }

    public float GetAngleToClosestObstacle()
    {
        return angleToClosestObject;
    }

    public ObjectInfo GetClosestWall()
    {
        ObjectInfo[] a = (ObjectInfo[])GetVisibleObjects("Wall").ToArray();
        return (a.Length == 0) ? null : a[a.Length - 1];
    }

}
