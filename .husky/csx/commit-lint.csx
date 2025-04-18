using System.Text.RegularExpressions;

private var pattern = @"(?m)^((?=.{1,90}$)(?:build|feat|ci|chore|docs|fix|perf|refactor|revert|style|test|wip)(?:\(.+\))*\!?(?::).{4,}(?:#\d+)*(?<![\.\s]))$(?:(?:\s*[\r\n]){2,}((?:.|[\r\n])*))?";
private string msg = File.ReadAllLines(Args[0])[0];

if (msg.StartsWith("Merge branch") || msg.StartsWith("Revert \""))
{
    return 0;
}

if (Regex.IsMatch(msg, pattern))
{
    return 0;
}

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("Invalid commit message");
Console.ResetColor();
Console.WriteLine("e.g: 'feat(scope): subject' or 'fix: subject'");
Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine("more info: https://www.conventionalcommits.org/en/v1.0.0/");

return 1;