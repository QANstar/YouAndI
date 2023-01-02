namespace YouAndI_API.Utils
{
    public static class MathUtils
    {
        public static double EuclideanDistance(double x1,double y1,double x2,double y2)
        {
            double distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return distance;
        }
    }
}
