namespace LutieBot.Commands.Utilities
{
    public class CommandUtilities
    {
        public Dictionary<string, string> ParseOptionalArguments(Queue<string> arguments, IEnumerable<string> validArguments)
        {
            var parseResult = new Dictionary<string, string>();

            while (arguments.Any())
            {
                string argument = arguments.Dequeue();
                
                if (!argument.StartsWith("--"))
                {
                    throw new Exception($"Unexpected token {argument}, expecting an argument name starting with `--`!");
                }

                argument = argument.Substring(2);

                if (parseResult.ContainsKey(argument))
                {
                    throw new Exception($"Duplicate argument {argument}!");
                }

                if (!validArguments.Contains(argument))
                {
                    throw new Exception($"Unexpected argument {argument}!");
                }

                if (!arguments.Any())
                {
                    throw new Exception($"Missing value for argument {argument}!"); 
                }

                string value = arguments.Dequeue();

                parseResult.Add(argument, value);
            }

            return parseResult;
        }
    }
}
