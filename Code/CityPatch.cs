using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using ReflectionUtility;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NCMS;
using UnityEngine;

namespace CulturalCityNames
{
    public class CityTitle
    {
        public string ending { get; set; }
        public string type { get; set; }
        public string extension { get; set; }
    }

    [HarmonyPatch(typeof(City), "generateName")]
    internal class CityPatch
    {
        static bool Prefix(City __instance)
        {
            // Check if names.json file exists in .modconfig folder
            string modConfigPath = Path.Combine(Application.dataPath, "../mods/.modconfig");
            string filePath = Path.Combine(modConfigPath, "names.json");
            List<CityTitle> cityTitles;

            if (File.Exists(filePath))
            {
                // If the file exists, read its contents and deserialize the JSON data
                try
                {
                    string json = File.ReadAllText(filePath);
                    cityTitles = JsonConvert.DeserializeObject<List<CityTitle>>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error reading names.json: " + e.Message);
                    return false; // Return false to prevent mod from changing city name if JSON cannot be read.
                }
            }
            else
            {
                Debug.LogError("names.json file not found. Make sure it exists in .modconfig folder.");
                return false; // Return false to prevent mod from changing city name if names.json is missing.
            }

            // Apply city name modifications using the data from names.json
            string suffix = "";
            string prefix = "";

            foreach (var cityTitle in cityTitles)
            {
                if (cultureName.EndsWith(cityTitle.ending))
                {
                    if (cityTitle.type == "prefix")
                    {
                        prefix = cityTitle.extension;
                    }
                    else
                    {
                        suffix = cityTitle.extension;
                    }
                }
            }

            __instance.data.name = prefix + NameGenerator.getName(raceTemplateName, ActorGender.Male) + suffix;
            return false;
        }
    }
}
