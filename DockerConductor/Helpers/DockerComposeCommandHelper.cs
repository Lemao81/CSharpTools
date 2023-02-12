using DockerConductor.Constants;

namespace DockerConductor.Helpers
{
    public static class DockerComposeCommandHelper
    {
        public static string GetBasicBuildCommand(string dockerComposePath, string dockerComposeOverridePath)
        {
            var basicCommand = Helper.ConcatCommand(
                Consts.DockerCompose,
                Helper.ConcatFilePathArguments(dockerComposePath, dockerComposeOverridePath),
                "build"
            );

            return basicCommand;
        }

    }
}
