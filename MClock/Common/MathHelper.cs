namespace MClock.Common;

public static class MathHelper
{
    public static double GetFraction(double startHour, double partialDay)
    {
        return partialDay / (startHour * 60.0);
    }
}