using CommandLine.Text;

namespace MarkXConsole
{
    public static class Comparator
    {
        public static Comparison<ComparableOption> RequiredThenAlphaShortComparison { get; } = (ComparableOption attr1, ComparableOption attr2) =>
        {
            if (attr1.IsOption && attr2.IsOption)
            {
                if (attr1.Required && !attr2.Required)
                {
                    return -1;
                }
                else if (!attr1.Required && attr2.Required)
                {
                    return 1;
                }

                if (string.IsNullOrEmpty(attr1.ShortName) && !string.IsNullOrEmpty(attr2.ShortName))
                {
                    return 1;
                }
                else if (!string.IsNullOrEmpty(attr1.ShortName) && string.IsNullOrEmpty(attr2.ShortName))
                {
                    return -1;
                }

                return String.Compare(attr1.ShortName, attr2.ShortName, StringComparison.Ordinal);

            }
            else if (attr1.IsOption && attr2.IsValue)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        };
    }
}
