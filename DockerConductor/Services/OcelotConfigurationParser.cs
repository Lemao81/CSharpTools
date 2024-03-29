﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using DockerConductor.Models;

namespace DockerConductor.Services
{
    public class OcelotConfigurationParser
    {
        public List<OcelotRoute> Parse(string[] lines)
        {
            var          isRoutes           = false;
            var          isPendingRoute     = false;
            var          isPendingSubObject = false;
            OcelotRoute? pendingRoute       = null;
            var          routes             = new List<OcelotRoute>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Contains("\"Routes\": ["))
                {
                    isRoutes = true;
                    continue;
                }

                if (!isRoutes) continue;

                if (!isPendingRoute && IsObjectStart(line))
                {
                    isPendingRoute = true;
                    pendingRoute   = new OcelotRoute();
                    continue;
                }

                if (!isPendingSubObject && IsObjectStart(line))
                {
                    isPendingSubObject = true;
                    continue;
                }

                if (isPendingSubObject && IsObjectFinish(line))
                {
                    isPendingSubObject = false;
                    continue;
                }

                if (isPendingRoute && IsObjectFinish(line))
                {
                    isPendingRoute = false;
                    if (pendingRoute != null && !string.IsNullOrWhiteSpace(pendingRoute.Host))
                    {
                        routes.Add(pendingRoute);
                    }

                    pendingRoute = null;
                    continue;
                }

                if (isPendingRoute && pendingRoute != null)
                {
                    var hostMatch = Regex.Match(line, "\"Host\": \"(.*?)\"");
                    if (hostMatch.Success)
                    {
                        pendingRoute.Host      = hostMatch.Groups[1].Value;
                        pendingRoute.HostIndex = i;
                    }

                    var portMatch = Regex.Match(line, "\"Port\": (\\d*)");
                    if (portMatch.Success)
                    {
                        pendingRoute.Port      = portMatch.Groups[1].Value;
                        pendingRoute.PortIndex = i;
                    }

                    var nameMatch = Regex.Match(line, "\"SwaggerKey\": \"(.*?)\"");
                    if (nameMatch.Success)
                    {
                        pendingRoute.Name = nameMatch.Groups[1].Value;
                    }

                    var schemaMatch = Regex.Match(line, "\"DownstreamScheme\": \"(.*?)\"");
                    if (schemaMatch.Success)
                    {
                        pendingRoute.Schema = schemaMatch.Groups[1].Value;
                    }

                    var pathMatch = Regex.Match(line, "\"DownstreamPathTemplate\": \"(.*?)\"");
                    if (pathMatch.Success)
                    {
                        pendingRoute.Path = pathMatch.Groups[1].Value;
                    }
                }
            }

            return routes;
        }

        private bool IsObjectStart(string line) => Regex.IsMatch(line, "{\\s*$");

        private bool IsObjectFinish(string line) => Regex.IsMatch(line, "^\\s*(}|},)\\s*$");
    }
}
