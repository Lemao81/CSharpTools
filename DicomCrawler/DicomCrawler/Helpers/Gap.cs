using Eto.Drawing;

namespace DicomCrawler.Helpers
{
    public static class Gap
    {
        public const int Small = 4;
        public const int Medium = 8;
        public const int Large = 16;
        public const int XtraLarge = 32;

        public static class Horizontal
        {
            public static Size Small = new Size(4, 0);
            public static Size Medium = new Size(8, 0);
            public static Size Large = new Size(16, 0);
        }

        public static class Vertical
        {
            public static Size Small = new Size(0, 4);
            public static Size Medium = new Size(0, 8);
            public static Size Large = new Size(0, 16);
        }

        public static class Diagonal
        {
            public static Size Small = new Size(4, 4);
            public static Size Medium = new Size(8, 8);
            public static Size Large = new Size(16, 16);
        }
    }
}
