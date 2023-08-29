namespace LemonTree.Pipeline.Tools.SemanticVersioning.Runner.CommandLineOptions
{
    public enum ExitCode
    {
        Error = 2,
        ErrorCmdParameter = 1, //Parsing error of command line parameter
        Success = 0, // Successful merge
    }
}
