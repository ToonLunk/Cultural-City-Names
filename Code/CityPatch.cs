using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using ReflectionUtility;
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
            string infoMessage = "<INFO>Cultural City Names: ";

            var kingdom = (Kingdom)Reflection.GetField(__instance.GetType(), __instance, "kingdom");
            string kingdomName = kingdom.name.ToString();

            string cultureName;
            try
            {
                Culture culture = kingdom.getCulture();
                cultureName = culture.data.name.ToString();
            }
            catch
            {
                Debug.Log(infoMessage + "No culture found for new city; must be a capital?");
                cultureName = "NONE";
            }

            string raceTemplateName;
            try
            {
                Race race = (Race)Reflection.GetField(__instance.GetType(), __instance, "race");
                raceTemplateName = race.name_template_city.ToString();
            }
            catch
            {
                Debug.Log(infoMessage + "No race found for new city!");
                raceTemplateName = "NONE";
            }

            // Check if names.json file exists in .modconfig folder
            string modConfigPath = Path.Combine(Application.dataPath, "../modConfigs");
            string filePath = Path.Combine(modConfigPath, "names.json");
            List<CityTitle> cityTitles = new List<CityTitle>(); // Initialize the list to an empty list

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
                Debug.LogError("For some reason, names.json wasn't created.");
            }

            // Apply city name modifications using the data from names.json
            string suffix = "";
            string prefix = "";

            bool endingFound = false;

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
                    endingFound = true;
                    break;
                }
            }

            if (!endingFound)
            {
                var newCityTitle = GenerateUniqueCityTitle(cultureName, cityTitles);
                cityTitles.Add(newCityTitle);
                SaveConfig(modConfigPath, cityTitles);
                if (newCityTitle.type == "prefix")
                {
                    prefix = newCityTitle.extension;
                }
                else
                {
                    suffix = newCityTitle.extension;
                }
            }

            __instance.data.name = prefix + NameGenerator.getName(raceTemplateName, ActorGender.Male) + suffix;
            return false;
        }

        private static CityTitle GenerateUniqueCityTitle(string cultureName, List<CityTitle> existingCityTitles)
        {
            var random = new System.Random();
            string newEnding = cultureName.Substring(cultureName.Length - 3); // Use last 3 characters as new ending

            // Meaningful default prefixes and suffixes not already in use
            List<string> defaultPrefixes = new List<string> {"Fort ", "Settlement of ", "Commonwealth of ", "Gin'",};
            List<string> defaultSuffixes = new List<string> {" Settlement", " Outpost", " Town", " Commune", "'s Reach", " Fork"};

            // Remove existing prefixes and suffixes from defaults
            foreach (var cityTitle in existingCityTitles)
            {
                if (cityTitle.type == "prefix")
                {
                    defaultPrefixes.Remove(cityTitle.extension);
                }
                else if (cityTitle.type == "suffix")
                {
                    defaultSuffixes.Remove(cityTitle.extension);
                }
            }

            // Randomly choose new type and corresponding extension
            string newType = random.Next(2) == 0 ? "prefix" : "suffix";
            string newExtension = newType == "prefix" ? defaultPrefixes[random.Next(defaultPrefixes.Count)] : defaultSuffixes[random.Next(defaultSuffixes.Count)];

            return new CityTitle
            {
                ending = newEnding,
                type = newType,
                extension = newExtension
            };
        }

        private static void SaveConfig(string modConfigPath, List<CityTitle> cityTitles)
        {
            string filePath = Path.Combine(modConfigPath, "names.json");
            string json = JsonConvert.SerializeObject(cityTitles, Formatting.Indented);
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError("Error writing names.json: " + e.Message);
            }
        }
    }
}
