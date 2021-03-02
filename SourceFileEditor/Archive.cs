using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static SourceFileEditor.Helper;

namespace SourceFileEditor
{
    public static class Archive
    {
        public static void AddGeneratedControllerAttribute()
        {
            var pageTypeFiles = GetSourceFiles("*Page").WithoutPart("KneeMRT").Matches(@"RadioReport\..+\.Domain\\Models.+Page");

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
    }
}
