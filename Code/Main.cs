using System.IO.Pipes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NCMS;
using HarmonyLib;
using UnityEngine;


namespace CulturalCityNames
{
    [ModEntry]
    class Main : MonoBehaviour
    {
        const string ModName = "Cultural City Mod";

        void Awake()
        {
            Debug.Log($"Hello world from {ModName} by Toon Lunk!");
            Harmony harmony = new Harmony(ModName);
            harmony.PatchAll();

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
                    return; // Return without creating the file if JSON cannot be read.
                }
            }
            else
            {
                // If the file doesn't exist, create a new list of CityTitle objects (same as the original JSON)
                cityTitles = new List<CityTitle>
                {
                    cityTitles = new List<CityTitle>
                {
                    new CityTitle { ending = "ian", type = "prefix", extension = "City of " },
                    new CityTitle { ending = "dia", type = "prefix", extension = "Ankh-" },
                    new CityTitle { ending = "hin", type = "suffix", extension = " Prefecture" },
                    new CityTitle { ending = "ese", type = "prefix", extension = "Province of " },
                    new CityTitle { ending = "lia", type = "prefix", extension = "Holy " },
                    new CityTitle { ending = "lic", type = "prefix", extension = "State of " },
                    new CityTitle { ending = "jen", type = "prefix", extension = "Jarldom of " },
                    new CityTitle { ending = "bio", type = "suffix", extension = " Zone" },
                    new CityTitle { ending = "deb", type = "suffix", extension = " Lands" },
                    new CityTitle { ending = "den", type = "suffix", extension = " Precinct" },
                    new CityTitle { ending = "ana", type = "suffix", extension = " Parish" },
                    new CityTitle { ending = "tsk", type = "suffix", extension = " Canton" },
                    new CityTitle { ending = "kin", type = "suffix", extension = " Oblast" },
                    new CityTitle { ending = "ova", type = "prefix", extension = "County of " },
                    new CityTitle { ending = "gin", type = "prefix", extension = "Duchy of " },
                    new CityTitle { ending = "for", type = "prefix", extension = "Barony of " },
                    new CityTitle { ending = "esk", type = "prefix", extension = "Township of " },
                    new CityTitle { ending = "ink", type = "prefix", extension = "Village of " },
                    new CityTitle { ending = "bia", type = "prefix", extension = "Fiefdom of " },
                    new CityTitle { ending = "ask", type = "suffix", extension = " Place" }
                };

                // Convert the list to JSON
                string json = JsonConvert.SerializeObject(cityTitles, Formatting.Indented);

                // Save the JSON data to names.json file in the .modconfig folder
                Directory.CreateDirectory(modConfigPath); // Create the .modconfig folder if it doesn't exist
                try
                {
                    File.WriteAllText(filePath, json);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error writing names.json: " + e.Message);
                    return;
                }
            }
        }
    }