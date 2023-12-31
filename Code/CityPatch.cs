﻿using System;
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
