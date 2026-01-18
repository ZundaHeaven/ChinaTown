namespace ChinaTown.Application.Helpers;

public class SlugHelper
{
    public static string GenerateSlug(string contentType, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        var slug = title.ToLowerInvariant()
            .Replace("ё", "е")
            .Replace(" ", "-")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("\"", "")
            .Replace("'", "")
            .Replace("&", "and")
            .Replace("@", "at")
            .Replace("#", "sharp")
            .Replace("%", "percent")
            .Replace("+", "plus")
            .Replace("=", "equals");

        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        slug = slug.Trim('-');

        if (string.IsNullOrEmpty(slug))
        {
            slug = $"{contentType}-{DateTime.UtcNow.Ticks}";
        }

        return slug;
    }
}