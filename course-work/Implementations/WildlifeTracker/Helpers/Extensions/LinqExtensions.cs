namespace WildlifeTracker.Helpers.Extensions
{
    public static class LinqExtensions
    {
        private static readonly Random _random = new();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(_ => _random.Next());
        }
    }
}
