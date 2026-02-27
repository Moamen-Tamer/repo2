using System.Drawing;

namespace HospitalApp
{
    public static class Theme
    {
        // Background
        public static Color Background = ColorTranslator.FromHtml("#0a0f1e");
        public static Color Sidebar = ColorTranslator.FromHtml("#0d1426");
        public static Color Card = ColorTranslator.FromHtml("#111827");
        public static Color CardHover = ColorTranslator.FromHtml("#162033");
        public static Color Input = ColorTranslator.FromHtml("#1a2540");

        // Border color
        public static Color Border = ColorTranslator.FromHtml("#1e2d45");
        public static Color BorderLight = ColorTranslator.FromHtml("#243352");

        // Accent
        public static Color Accent = ColorTranslator.FromHtml("#38bdf8");
        public static Color AccentDeep = ColorTranslator.FromHtml("#0284c7");
        public static Color AccentGlow = ColorTranslator.FromHtml("#1e3a5f");

        // Status Color
        public static Color Success = ColorTranslator.FromHtml("#34d399");
        public static Color Warning = ColorTranslator.FromHtml("#fbbf24");
        public static Color Danger = ColorTranslator.FromHtml("#f87171"); 

        // text color
        public static Color TextPrimary = ColorTranslator.FromHtml("#e2e8f0");
        public static Color TextSecondary = ColorTranslator.FromHtml("#94a3b8");
        public static Color TextMuted = ColorTranslator.FromHtml("#475569");

        // fonts
        public static Font FontTitle = new Font("Segoe UI", 22, FontStyle.Bold);
        public static Font FontHeading = new Font("Segoe UI", 14, FontStyle.Bold);
        public static Font FontSubhead = new Font("Segoe UI", 10, FontStyle.Bold);
        public static Font FontBody = new Font("Segoe UI", 10);
        public static Font FontSmall = new Font("Segoe UI", 8);
        public static Font FontLabel = new Font("Segoe UI", 8, FontStyle.Bold);
    }
}