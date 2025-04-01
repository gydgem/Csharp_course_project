using System.Text;

namespace TwinFinder.CsvParsers;

public class CsvParser
{
    //castul
    public static string EscapeCsvValue(string value)
    {
        if (value.Contains(",") || value.Contains("\""))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\""; 
        }

        return value;
    }
    public static string[] ParseCsvLine(string csvLine)
    {
        var result = new List<string>();
        var currentValue = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < csvLine.Length; i++)
        {
            char currentChar = csvLine[i];

            if (inQuotes)
            {
                if (currentChar == '"')
                {
                    if (i + 1 < csvLine.Length && csvLine[i + 1] == '"')
                    {
                        currentValue.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    currentValue.Append(currentChar);
                }
            }
            else
            {
                if (currentChar == ',')
                {
                    result.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else if (currentChar == '"')
                {
                    inQuotes = true;
                }
                else
                {
                    currentValue.Append(currentChar);
                }
            }
        }

        result.Add(currentValue.ToString());

        return result.ToArray();
    }
}