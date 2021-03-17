using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static SourceFileEditor.Helper;

namespace SourceFileEditor
{
    public static class Archive
    {
        public static void AddGeneratedControllerAttribute()
        {
            var pageTypeFiles = GetSourceFiles("*Page").WithoutPathPart("KneeMRT").PathMatches(@"RadioReport\..+\.Domain\\Models.+Page");

            var notMatched = new List<Match>();
            foreach (var file in pageTypeFiles)
            {
                var match = file.Match(@"\\RadioReport\..+\\RadioReport\.(.+)\.Domain\\Models\\.+\\(.+)Page");
                if (match.Groups.Count < 3)
                {
                    notMatched.Add(match);
                }

                var moduleName = match.Groups[1];
                var pageName = match.Groups[2];

                var lines = file.ReadLines();
                var indexPublicClass = lines.IndexOf("public class");
                var lineToInsert = $"    [GeneratedController(\"{moduleName}/{pageName}\")]";
                lines.Insert(indexPublicClass, lineToInsert);
                var containsUsing = lines.HasUsing("RadioReport.Common.Logic.Attributes");
                if (!containsUsing)
                {
                    lines.InsertUsing("RadioReport.Common.Logic.Attributes");
                }
                lines.WriteAll(file);
            }

            Console.WriteLine("Not matched: " + notMatched.Count);
        }

        public static void CompareControllerAttributeRoutes()
        {
            var pageControllerFiles = GetSourceFiles("*PageController");
            var pageTypeFiles = GetSourceFiles("*Page");

            var modules = GetModuleNames(ModuleType.KneeMRT);
            foreach (var module in modules)
            {
                var controllers = pageControllerFiles.OfModule(module).SortByName();
                var pages = pageTypeFiles.OfModule(module).SortByName();
                if (controllers.Count != pages.Count) throw new InvalidOperationException();

                for (var i = 0; i < controllers.Count; i++)
                {
                    var controllerFile = controllers[i];
                    var pageFile = pages[i];

                    var controllerLines = controllerFile.ReadLines();
                    var pageLines = pageFile.ReadLines();

                    var routeLine = controllerLines.LineWith("[Route(");
                    var attributeLine = pageLines.LineWith("[GeneratedController(");

                    var controllerRoute = routeLine.Extract("\"api\\/v1\\/(.*)\"");
                    var attributeRoute = attributeLine.Extract("\"(.*)\"");
                    Print(controllerRoute);
                    Print(attributeRoute);

                    if (controllerRoute != attributeRoute)
                    {
                        throw new InvalidOperationException($"!!! Unequal !!! {controllerRoute} - {attributeRoute}");
                    }
                }
            }
        }

        public static void PrintTypesWithImage()
        {
            var imagePageControllerFiles = GetSourceFiles("*Controller")
                .ContainsLineWith("PageWithImageBaseController<")
                .ContainsLineWith("[Route(");
            var routeAndTypes = imagePageControllerFiles.Select(f =>
            {
                var lines = f.ReadLines();

                return (lines.LineWith("[Route(").Extract("\"api\\/v1\\/(.*)\""), lines.LineWith("public class").Extract(",\\s(.*)>"));
            });
            foreach (var (route, type) in routeAndTypes)
            {
                Print(route);
                Print(type);
                Print();
            }
        }

        public static void ReplaceGeneratedControllerAttribute()
        {
            var imagePageControllerFiles = GetSourceFiles("*Controller")
                .ContainsLineWith("PageWithImageBaseController")
                .ContainsLineWith("[Route(");
            var routes = imagePageControllerFiles.Select(f => f.ReadLines().LineWith("[Route(").Extract("\"api\\/v1\\/(.*)\"")).ToList();

            var pageTypeFiles = GetSourceFiles("*Page").ContainsLineWith("[GeneratedControllerPageModel(");
            var filtered = pageTypeFiles.Where(f => routes.Contains(f.ReadLines().LineWith("GeneratedControllerPageModel").Extract("\"(.*)\"")));
            foreach (var file in filtered)
            {
                file.ReplaceInLine("GeneratedControllerPageModel", "GeneratedControllerPageWithImageModel");
            }
        }
        
        public static void AddPageModelGeneric()
        {
            var sourceFiles = GetSourceFiles("*Page").ContainsLineWith(" : PageModelBase", ": PageModelBase", " :PageModelBase", ":PageModelBase");
            foreach (var file in sourceFiles)
            {
                var classLine = file.ReadLines().LineWith("PageModelBase");
                var typeName = classLine.Extract(@"public\s+class\s+(.*)\s*:").Trim();
                file.ReplaceInLine("PageModelBase", $"PageModelBase<{typeName}>");
            }
        }
    }
}
