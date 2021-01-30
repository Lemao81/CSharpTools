using PlaywrightSharp;

namespace UiRenderTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var playwright = Playwright.CreateAsync().Result;
            var browser = playwright.Chromium.LaunchAsync().Result;
            var page = browser.NewPageAsync().Result;
            page.GoToAsync("http://rrtestneo-q.vialutions.biz/Frontend/login").Wait();
            page.ScreenshotAsync("renderoutput.html").Wait();
        }
    }
}
