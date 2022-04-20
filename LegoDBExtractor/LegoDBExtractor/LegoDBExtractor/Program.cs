using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LegoDBExtractor
{
    class Program
    {
        private static string folderPath = "source";
        private static string outputSqlFileName = "01_lego_db_import.sql";
        private static string outputCsvFileName = "02_lego_db_import.csv";


        static void Main(string[] args)
        {

            //Build list of valid inventories
            Console.WriteLine("Getting valid inventories");
            var validInventories = getValidInventories();

            //Get sets
            var sets = getSets(validInventories);

            //Get set categories
            var setCategories = getSetCategories();

            //Get part categories
            var partCategories = getPartCategories();

            //Get colors
            var colors = getColors();

            //Get set_parts list
            var setPartsRes = getSetParts(validInventories);
            var setParts = setPartsRes.SetsFormatted;

            //Get parts list
            var parts = getParts(setPartsRes.UniquePartNums);

            //Delete file to start fresh
            File.Delete(outputSqlFileName);

            //Write sql to script
            writeToFile(outputSqlFileName, 2, "INSERT INTO dbo.SetCategory (SetCategoryId, SetCategoryName)", setCategories);
            writeToFile(outputSqlFileName, 2, "INSERT INTO dbo.PieceCategory (PieceCategoryId, PieceCategoryName)", partCategories);
            writeToFile(outputSqlFileName, 2, "INSERT INTO dbo.Colour (ColourId, ColourName)", colors);
            writeToFile(outputSqlFileName, 4, "INSERT INTO dbo.[Set] (SetId, SetName, SetCategory, PiecesAmount)", sets);
            writeToFile(outputSqlFileName, 3, "INSERT INTO dbo.Piece (PieceId, PieceName, PieceCategory)", parts);

            //Write set parts to csv file
            File.Delete(outputCsvFileName);
            File.WriteAllText(outputCsvFileName, "SetPieceId,Piece,SetId,Amount,Colour\r\n");
            File.AppendAllLines(outputCsvFileName, setParts);

        }

        //Used to process inventories.csv
        //Get list of valid inventories based on filtering set ids
        private static List<InventoryItem> getValidInventories()
        {
            string[] inventoriesLines = File.ReadAllLines($"{folderPath}/inventories.csv");

            List<InventoryItem> output = new List<InventoryItem>();

            foreach(string line in inventoriesLines)
            {
                
                //Skip header
                if (line == "id,version,set_num")
                {
                    continue;
                }

                string[] parts = line.Split(",");

                int id = Int32.Parse(parts[0]);

                string setId = parts[2];
                string subVersion = setId.Substring(setId.IndexOf("-") + 1);
                setId = setId.Substring(0, setId.IndexOf("-"));
                
                int setIdInt = 0;
                int subVersionInt = 0;

                //We only want the sets where the id is valid (does not overflow, and only contains numbers)
                if (!Int32.TryParse(setId, out setIdInt))
                {
                    continue;
                }

                if (!Int32.TryParse(subVersion, out subVersionInt))
                {
                    continue;
                }

                var item = output.Find(x => x.SetId == setIdInt);

                if (item != null)
                {
                    if (subVersionInt < item.SubVersion)
                    {
                        item.SubVersion = subVersionInt;
                        item.Id = id;
                    }

                } else
                {
                    output.Add(new InventoryItem(id, setIdInt, subVersionInt));
                }
                
            }

            return output;
        }


        //Used to process sets.csv
        private static List<string> getSets(List<InventoryItem> validInventories)
        {
            string[] setsLines = File.ReadAllLines($"{folderPath}/sets.csv");
            HashSet<string> setIds = new HashSet<string>();

            Console.WriteLine("Raw Sets: " + setsLines.Length);

            List<string> output = new List<string>();

            foreach(string line in setsLines)
            {

                //Skip header
                if (line == "set_num,name,year,theme_id,num_parts")
                {
                    continue;
                }

                //Get set id
                string setId = line.Substring(0, line.IndexOf("-"));

                //Check if set id is NOT in filter
                if (!validInventories.Any((item) => item.SetId.ToString() == setId))
                {
                    continue;
                }

                //Check for duplicates
                if (setIds.Contains(setId))
                {
                    continue;
                }

                setIds.Add(setId);

                string lineTemp = line;

                //The set id
                string lineOutput = $"({setId},";
                lineTemp = lineTemp.Substring(lineTemp.IndexOf(",") + 1);

                var res = processNameField(lineTemp);

                lineOutput += res.Item1 + ",";
                lineTemp = res.Item2;

                //Skip name field
                lineTemp = lineTemp.Substring(lineTemp.IndexOf(",") + 1);

                //Rest of fields
                lineOutput += lineTemp + "),";

                output.Add(lineOutput);
            }

            Console.WriteLine("Filtered Sets: " + output.Count);

            return output;
        }


        //Used to process themes.csv
        private static List<string> getSetCategories()
        {
            string[] catLines = File.ReadAllLines($"{folderPath}/themes.csv");
            Console.WriteLine("Raw Set_Categories: " + catLines.Length);

            List<string> output = new List<string>();

            foreach (string line in catLines)
            {

                //Skip header
                if (line == "id,name,parent_id")
                {
                    continue;
                }

                string[] parts = line.Split(",");
                string lineTemp = line;

                string catId = parts[0];

                //The cat id
                string lineOutput = $"({catId},";
                lineTemp = lineTemp.Substring(lineTemp.IndexOf(",") + 1);

                var res = processNameField(lineTemp);

                lineOutput += res.Item1;

                //Rest of fields
                lineOutput += "),";

                output.Add(lineOutput);
            }

            Console.WriteLine("Filtered Set_Categories: " + output.Count);

            return output;
        }

        //Used to process inventory_parts.csv
        private static SetItemsResult getSetParts(List<InventoryItem> validInventories)
        {
            string[] inventoryPartsLines = File.ReadAllLines($"{folderPath}/inventory_parts.csv");
            Console.WriteLine("Raw Set_Parts: " + inventoryPartsLines.Length);

            List<string> output = new List<string>();
            HashSet<string> outputParts = new HashSet<string>();

            string prevInvId = null;
            int prevSetNum = -1;
            string prevColorId = null;
            string prevBlockNum = null;
            int prevBlockCount = -1;

            foreach (string line in inventoryPartsLines)
            {
                //Skip header
                if (line == "inventory_id,part_num,color_id,quantity,is_spare")
                {
                    continue;
                }

                string[] parts = line.Split(",");

                int setNum = -1;

                //If the last inventory id is same as now, we dont have to search to validate
                if (parts[0] == prevInvId)
                {   
                    setNum = prevSetNum;

                } else // We need to search to find the inventory id
                {

                    foreach (InventoryItem item in validInventories)
                    {
                        if (item.Id.ToString() == parts[0])
                        {
                            setNum = item.SetId;                   
                            break;
                        }
                    }
                }

                prevInvId = parts[0];
                prevSetNum = setNum;

                //We did not find the inventory, so its not valid, and we can skip
                if (setNum == -1)
                {
                    //Console.WriteLine("Invalid Set Id: " + parts[0]);
                    continue;
                }

                int blockCount = Int32.Parse(parts[3]);

                //The same block and color as prev entry
                if (parts[1] == prevBlockNum && parts[2] == prevColorId)
                {
                    output.RemoveAt(output.Count - 1);
                    blockCount += prevBlockCount;
                }

                //setNum,partNum,color,amount
                string lineOutput = $",{parts[1]},{setNum},{blockCount},{parts[2]}";

                prevBlockNum = parts[1];
                prevColorId = parts[2];
                prevBlockCount = blockCount;

                output.Add(lineOutput);
                outputParts.Add(parts[1]);
            }

            Console.WriteLine("Filtered Set_Parts: " + output.Count);

            return new SetItemsResult(output, outputParts);
        }


        //Used to process parts.csv
        private static List<string> getParts(HashSet<string> validPartNums)
        {
            string[] partsLines = File.ReadAllLines($"{folderPath}/parts.csv");
            Console.WriteLine("Raw Parts: " + partsLines.Length);

            List<string> output = new List<string>();

            foreach (string line in partsLines)
            {
                //Skip header
                if (line == "part_num,name,part_cat_id,part_material")
                {
                    continue;
                }

                string[] parts = line.Split(",");

                //If the part num is not valid, we skip
                if (!validPartNums.Contains(parts[0]))
                {
                    continue;
                }

                string lineTemp = line;

                //The partNum id
                string lineOutput = $"('{parts[0]}',";
                lineTemp = lineTemp.Substring(lineTemp.IndexOf(",") + 1);

                var res = processNameField(lineTemp);

                lineOutput += res.Item1 + ",";
                lineTemp = res.Item2;

                //Rest of fields (we skip part material)
                lineOutput += lineTemp.Substring(0, lineTemp.IndexOf(",")) + "),";

                output.Add(lineOutput);
            }

            Console.WriteLine("Filtered Parts: " + output.Count);

            return output;
        }


        //Used to process part_categories.csv
        private static List<string> getPartCategories()
        {
            string[] catLines = File.ReadAllLines($"{folderPath}/part_categories.csv");
            Console.WriteLine("Raw Part_Categories: " + catLines.Length);

            List<string> output = new List<string>();

            foreach (string line in catLines)
            {

                //Skip header
                if (line == "id,name")
                {
                    continue;
                }

                string[] parts = line.Split(",");
                string lineTemp = line;

                string catId = parts[0];

                //The cat id
                string lineOutput = $"({catId},";
                lineTemp = lineTemp.Substring(lineTemp.IndexOf(",") + 1);

                string name = lineTemp;

                //Process the name field
                if (lineTemp[0] == '"')
                {
                    name = lineTemp.Substring(1, lineTemp.Length - 2);
                }

                name.Replace("'", "''");
                name = $"'{name}'),";

                lineOutput += name;

                output.Add(lineOutput);
            }

            Console.WriteLine("Filtered Part_Categories: " + output.Count);

            return output;
        }


        //Used to process colors.csv
        private static List<string> getColors()
        {
            string[] colLines = File.ReadAllLines($"{folderPath}/colors.csv");
            Console.WriteLine("Raw Colors: " + colLines.Length);

            List<string> output = new List<string>();

            foreach (string line in colLines)
            {

                //Skip header
                if (line == "id,name,rgb,is_trans")
                {
                    continue;
                }

                string[] parts = line.Split(",");

                output.Add($"({parts[0]},'{parts[1]}'),");
            }

            Console.WriteLine("Filtered Part_Categories: " + output.Count);

            return output;
        }


        private static Tuple<string, string> processNameField(string linePart)
        {
            string output = "";
            string name = "";

            //The name field will contain commas since its in quotes in csv
            if (linePart[0] == '"')
            {
                name = linePart.Substring(1, linePart.LastIndexOf("\"") - 1);

                linePart = linePart.Substring(linePart.LastIndexOf("\"") + 2);

            }
            else //The name field is normal
            {
                name = linePart.Substring(0, linePart.IndexOf(","));
                
                linePart = linePart.Substring(linePart.IndexOf(",") + 1);
            }

            name = name.Replace("'", "''");
            output += "'" + name + "'";

            return Tuple.Create(output, linePart);
        }

        private static void writeToFile(string fileName, int colCount, string sqlHeader, List<string> lines)
        {
            var linesArr = lines.ToArray();
            
            var lastLine = linesArr[linesArr.Length - 1];
            lastLine = lastLine.Substring(0, lastLine.LastIndexOf(","));
            linesArr[linesArr.Length - 1] = lastLine;

            List<string> footer = new List<string>();

            for (int i = 0; i < colCount; i++)
            {
                footer.Add($"Col{i + 1}");
            }

            File.AppendAllText(fileName, $"{sqlHeader}\r\n");
            File.AppendAllText(fileName, "SELECT * FROM (VALUES\r\n");
            File.AppendAllLines(fileName, linesArr);
            File.AppendAllText(fileName, $") A({String.Join(", ", footer)})\r\n");
            File.AppendAllText(fileName, "\r\nGO\r\n\r\n");
        }
    }
}
