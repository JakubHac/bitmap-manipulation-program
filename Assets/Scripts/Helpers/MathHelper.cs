using UnityEngine;

public static class MathHelper
{
    public static float AngleBetweenTwoPoints(Vector2 p1, Vector2 p2)
    {
        float deltaY = (p1.y - p2.y);
        float deltaX = (p2.x - p1.x);
        float result = Mathf.Rad2Deg * Mathf.Atan2(deltaY, deltaX);
        return (result < 0) ? (360f + result) : result;
    }
}
