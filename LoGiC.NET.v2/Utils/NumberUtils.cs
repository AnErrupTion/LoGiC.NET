namespace LoGiC.NET.v2.Utils;

public static class NumberUtils
{
    public static readonly Random Random = new();

    public static int[] GetAddOperationFor(int total, int count, int lowerBound, int upperBound)
    {
        var result = new int[count];
        var currentSum = 0;

        for (var index = 0; index < count; index++)
        {
            var calc = total - currentSum - upperBound * (count - 1 - index);
            var low = calc < lowerBound ? lowerBound : calc;
            calc = total - currentSum - lowerBound * (count - 1 - index);
            var high = (calc > upperBound ? upperBound : calc) + 1;

            var value = Random.Next(low, high);
            result[index] = value;
            currentSum += value;
        }

        return result;
    }
}