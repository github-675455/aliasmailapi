namespace AliasMailApi.Extensions
{
    public static class StringNotEmptyExtension
    {
    public static bool NotEmpty(this string expression)
    {
        return !string.IsNullOrWhiteSpace(expression);
    }
}
}