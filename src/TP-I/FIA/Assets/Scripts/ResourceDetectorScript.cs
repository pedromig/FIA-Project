
public class ResourceDetectorScript : DetectorScript
{

    void FixedUpdate()
    {
        ObjectInfo anObject = GetClosestPickup();
        strength = (anObject != null) ? 1.0f / (anObject.distance + 1.0f) : 0;
        angleToClosestObject = (anObject != null) ? anObject.angle : 0;
    }

    public float GetAngleToClosestResource()
    {
        return angleToClosestObject;
    }

    public ObjectInfo GetClosestPickup()
    {
        ObjectInfo[] a = (ObjectInfo[])GetVisibleObjects("Pickup").ToArray();
        return (a.Length == 0) ? null : a[a.Length - 1];
    }
}
