namespace NetCord.Logging;

public class ConsoleLogger(LogLevel logLevel) : TextWriterLogger(logLevel, Console.Out)
{
}
