using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CsvParser
{
    private const char DELIMITER = ',';

    public static Dictionary<int, Dictionary<string, string>> ParseCsv(string csvText)
    {
        var csvData = new Dictionary<int, Dictionary<string, string>>();

        // Split the CSV text into lines
        var lines = csvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        // Read the header row to get the column names
        var header = lines[0].Split(DELIMITER);

        for (var i = 1; i < lines.Length; i++)
        {
            // Read a line from the CSV file
            var line = lines[i].Split(DELIMITER);

            if (line.Length == 0) continue;

            var sku = int.Parse(line[0], NumberStyles.Integer, CultureInfo.InvariantCulture);

            var rowData = new Dictionary<string, string>();

            // Iterate through the columns and add them to the rowData dictionary
            for (var j = 0; j < header.Length; j++)
            {
                var column = header[j];
                var value = line[j];

                if (column.Equals("SKU")) continue;

                rowData.Add(column, value);
            }

            csvData.Add(sku, rowData);
        }

        return csvData;
    }
}

public class WearableDatabaseReader
{
    private Dictionary<int, Dictionary<string, string>> _data;

    public void LoadData(string resourceName)
    {
        var textAsset = Resources.Load(resourceName) as TextAsset;

        if (textAsset == null)
        {
            Debug.LogError($"Failed to load CSV file '{resourceName}' from Resources folder!");
            return;
        }

        var csvText = textAsset.text;

        _data = CsvParser.ParseCsv(csvText);
    }

    public void UseData()
    {
        foreach (var sku in _data.Keys)
        {
            var rowData = _data[sku];

            // Access data using column names
            var slots = rowData["SLOTS"];
            var slug = rowData["SLUG"];
            var rarityLevel = rowData["RARITY LEVEL"];
            var costToRepair = rowData["COST TO REPAIR"];
            var costToMerge = rowData["COST TO MERGE"];
            var objectHealth = rowData["OBJECT HEALTH"];
            var extraPoints = rowData["EXTRA POINTS"];
            var spd = rowData["SPD"];
            var tek = rowData["TEK"];
            var atk = rowData["ATK"];
            var def = rowData["DEF"];

            // Use data as needed
        }
    }

    public string GetSlot(int sku)
    {
        return _data[sku]["SLOTS"];
    }

    public string GetSlug(int sku)
    {
        return _data[sku]["SLUG"];
    }

    public int GetRarityLevel(int sku)
    {
        return int.Parse(_data[sku]["RARITY LEVEL"]);
    }

    public int GetCostToRepair(int sku)
    {
        return int.Parse(_data[sku]["COST TO REPAIR"]);
    }

    public int GetCostToMerge(int sku)
    {
        return int.Parse(_data[sku]["COST TO MERGE"]);
    }

    public int GetTotalHealth(int sku)
    {
        return int.Parse(_data[sku]["OBJECT HEALTH"]);
    }

    public int GetExtraPoints(int sku)
    {
        return int.Parse(_data[sku]["EXTRA POINTS"]);
    }

    public int GetAtk(int sku)
    {
        return int.Parse(_data[sku]["ATK"]);
    }

    public int GetDef(int sku)
    {
        return int.Parse(_data[sku]["DEF"]);
    }

    public int GetSpd(int sku)
    {
        return int.Parse(_data[sku]["SPD"]);
    }

    public int GetTek(int sku)
    {
        return int.Parse(_data[sku]["TEK"]);
    }

}
