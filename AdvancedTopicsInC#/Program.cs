string fileName = "theMachineStops.txt";

Directory.SetCurrentDirectory(@"D:\Downloads");

string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

if (!File.Exists(inputFilePath))
{
    Console.WriteLine($"The input file '{fileName}' does not exist.");
    return;
}

string outputFileName = "TelegramCopy.txt";

string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), outputFileName);

try
{
    string inpText = File.ReadAllText(inputFilePath);

    string modifiedText = inpText.Replace(".", "STOP");

    File.WriteAllText(outputFilePath, modifiedText);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine("Changes finished, check TelegramCopy.txt");