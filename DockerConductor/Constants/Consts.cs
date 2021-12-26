using System.Collections.Immutable;

namespace DockerConductor.Constants
{
    public static class Consts
    {
        public const  string                DockerCompose        = "docker-compose";
        public static ImmutableList<string> OcelotPortSelections = ImmutableList.Create("5000", "5001", "5002", "5003");
    }
}
