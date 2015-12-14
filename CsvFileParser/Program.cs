using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvFileParser
{
    class Program
    {
        private static int numberOfColumns = int.MinValue;
        private const char Comma = ',';
        private const char Quote = '"';
        private const char Carriage = '\r';
        private const char NewLine = '\n';

        static void Main(string[] args)
        {
            var result = GetCsvFileContents(@"c:\test\file.csv");
        }
        static List<String[]> GetCsvFileContents(string fileName)
        {
            List<String[]> result = new List<string[]>();

            if (!File.Exists(fileName))
                throw new FileNotFoundException();

            using (StreamReader sr = new StreamReader(fileName, Encoding.ASCII))
            {
                var content = sr.ReadToEnd();

                bool addRow = false;
                var row = new List<string>();

                for (int i = 0; i < content.Length; i++)
                {
                    string temp = String.Empty;

                    if (content[i] == Quote)
                    {
                        i++;
                        if (i + 1 < content.Length)
                        {
                            temp = GetValueInQuote(content, ref i, ref addRow);

                            row.Add(temp); //add field value
                            i++; //skip the comma
                        }
                    }
                    else
                    {
                        while (content[i] != Comma)
                        {
                            //If we cant compare the next character then break
                            if (i + 1 >= content.Length)
                                break;

                            if (content[i] == Carriage && content[i + 1] == NewLine)
                            {
                                //If its the end of line then add values to result
                                i++;
                                addRow = true;
                                break;
                            }

                            temp += content[i]; //Extract the value
                            i++;
                        }

                        row.Add(temp);
                    }

                    if (addRow)
                    {
                        var values = row.ToArray();
                        if (ValidateValues(values))
                        {
                            result.Add(values);
                            row.Clear();
                            addRow = false;
                        }
                    }
                }
            }

            return result;
        }

        private static string GetValueInQuote(string content, ref int i, ref bool addRow)
        {
            int numberOfQuotes = 1;
            string temp = String.Empty;
            while (!(content[i] == Quote && content[i + 1] == Comma))
            {
                //If we cant compare the next character then break
                if (i + 1 >= content.Length)
                    break;

                //Keep a track of how many quotes
                if (content[i] == '"')
                    numberOfQuotes++;

                if (content[i] == Carriage && content[i + 1] == NewLine &&
                    numberOfQuotes % 2 == 0)
                {
                    //If its the end of line and the number of quotes
                    //is even then add the values to result
                    addRow = true;
                    break;
                }

                temp += content[i]; //get the field value
                i++;
            }

            return temp;
        }

        private static bool ValidateValues(string[] values)
        {
            if (numberOfColumns == int.MinValue)
                numberOfColumns = values.Count();
            else if (numberOfColumns != values.Count())
                throw new FormatException("Csv file is not formed correctly");

            return true;
        }
    }
}
