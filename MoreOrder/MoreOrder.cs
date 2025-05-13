using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace MoreOrder
{
    [BepInPlugin("com.monkeytotok.shrinemod", "MoreOrder", "1.0.0")]
    public class MoreOrder : BaseUnityPlugin
    {
        public static ConfigEntry<float> shrineChance_stage1;
        public static ConfigEntry<float> shrineChance_stage2;
        public static ConfigEntry<float> shrineChance_stage3;
        public static ConfigEntry<float> shrineChance_stage4;
        public static ConfigEntry<float> shrineChance_stage5;
        public static ConfigEntry<float> shrineChance_finalStage;

        public static ConfigEntry<bool> spawn_in_stage1;
        public static ConfigEntry<bool> spawn_in_stage2;
        public static ConfigEntry<bool> spawn_in_stage3;
        public static ConfigEntry<bool> spawn_in_stage4;
        public static ConfigEntry<bool> spawn_in_stage5;
        public static ConfigEntry<bool> spawn_in_finalStage;

        // Liste des noms de scène pour les différentes étapes
        private static readonly string[] stages1 = new string[]
        {
            "blackbeach",
            "golemplains",
            "snowyforest",
            "lakes",
            "lakesnight",
            "village",
            "villagenight"
        };

        private static readonly string[] stages2 = new string[]
        {
            "goolake",
            "foggyswamp",
            "ancientloft",
            "lemuriantemple"
        };

        private static readonly string[] stages3 = new string[]
        {
            "frozenwall",
            "wispgraveyard",
            "sulfurpools",
            "habitat",
            "habitatfall"
        };

        private static readonly string[] stages4 = new string[]
        {
            "dampcavesimple",
            "shipgraveyard",
            "rootjungle",
            "meridian"
        };

        private static readonly string[] stages5 = new string[]
        {
            "skymeadow",
            "helminthroost"
        };

        private static readonly string[] finalStage = new string[]
        {
            "moon2"
        };



        public void Awake()
        {
            shrineChance_stage1 = Config.Bind("General", "ShrineOfOrderChance_Stage1", 0.0f, "Probability of spawning the Shrine of Order in the stage (0.0 = 0%, 1.0 = 100%)");
            shrineChance_stage2 = Config.Bind("General", "ShrineOfOrderChance_Stage2", 0.0f, "Probability of spawning the Shrine of Order in the stage (0.0 = 0%, 1.0 = 100%)");
            shrineChance_stage3 = Config.Bind("General", "ShrineOfOrderChance_Stage3", 1.0f, "Probability of spawning the Shrine of Order in the stage (0.0 = 0%, 1.0 = 100%)");
            shrineChance_stage4 = Config.Bind("General", "ShrineOfOrderChance_Stage4", 0.0f, "Probability of spawning the Shrine of Order in the stage (0.0 = 0%, 1.0 = 100%)");
            shrineChance_stage5 = Config.Bind("General", "ShrineOfOrderChance_Stage5", 0.0f, "Probability of spawning the Shrine of Order in the stage (0.0 = 0%, 1.0 = 100%)");
            shrineChance_finalStage = Config.Bind("General", "ShrineOfOrderChance_FinalStage", 1.0f, "Probability of spawning the Shrine of Order in the stage (0.0 = 0%, 1.0 = 100%)");

            spawn_in_stage1 = Config.Bind("General", "Stage 1", false, "The Shrine of Order will have a chance of spawning in any map of stage 1");
            spawn_in_stage2 = Config.Bind("General", "Stage 2", false, "The Shrine of Order will have a chance of spawning in any map of stage 2");
            spawn_in_stage3 = Config.Bind("General", "Stage 3", true, "The Shrine of Order will have a chance of spawning in any map of stage 3");
            spawn_in_stage4 = Config.Bind("General", "Stage 4", false, "The Shrine of Order will have a chance of spawning in any map of stage 4");
            spawn_in_stage5 = Config.Bind("General", "Stage 5", false, "The Shrine of Order will have a chance of spawning in any map of stage 5");
            spawn_in_finalStage = Config.Bind("General", "Final Stage", true, "The Shrine of Order will have a chance of spawning in the final stage");

            Set_Options();


            Logger.LogInfo("MoreOrder : loaded 22 !");
            // Hook sur la création de la scène
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
        }


        private void Set_Options()
        {

            ModSettingsManager.SetModDescription("Ce mod permet d'augmenter la probabilité d'apparition des Shrines of Order.");
            ModSettingsManager.AddOption(new CheckBoxOption(spawn_in_stage1));

            ModSettingsManager.AddOption(new SliderOption(shrineChance_stage1,
            new StepSliderConfig
            {
                min = 0f,
                max = 1f,
                increment = 0.01f,
                checkIfDisabled = Get_Spawn_In_Stage1
            }
            ));

            ModSettingsManager.AddOption(new CheckBoxOption(spawn_in_stage2));
            ModSettingsManager.AddOption(new SliderOption(shrineChance_stage2,
                new StepSliderConfig
                {
                    min = 0f,
                    max = 1f,
                    increment = 0.01f,
                    checkIfDisabled = Get_Spawn_In_Stage2
                }
            ));
            ModSettingsManager.AddOption(new CheckBoxOption(spawn_in_stage3));
            ModSettingsManager.AddOption(new SliderOption(shrineChance_stage3,
                new StepSliderConfig
                {
                    min = 0f,
                    max = 1f,
                    increment = 0.01f,
                    checkIfDisabled = Get_Spawn_In_Stage3
                }
            ));
            ModSettingsManager.AddOption(new CheckBoxOption(spawn_in_stage4));
            ModSettingsManager.AddOption(new SliderOption(shrineChance_stage4,
                new StepSliderConfig
                {
                    min = 0f,
                    max = 1f,
                    increment = 0.01f,
                    checkIfDisabled = Get_Spawn_In_Stage4
                }
            ));
            ModSettingsManager.AddOption(new CheckBoxOption(spawn_in_stage5));
            ModSettingsManager.AddOption(new SliderOption(shrineChance_stage5,
                new StepSliderConfig
                {
                    min = 0f,
                    max = 1f,
                    increment = 0.01f,
                    checkIfDisabled = Get_Spawn_In_Stage5
                }
            ));
            ModSettingsManager.AddOption(new CheckBoxOption(spawn_in_finalStage));
            ModSettingsManager.AddOption(new SliderOption(shrineChance_finalStage,
                new StepSliderConfig
                {
                    min = 0f,
                    max = 1f,
                    increment = 0.01f,
                    checkIfDisabled = Get_Spawn_In_FinalStage
                }
            ));
        }

        private bool Get_Spawn_In_Stage1()
        {
            return !spawn_in_stage1.Value;
        }
        private bool Get_Spawn_In_Stage2()
        {
            return !spawn_in_stage2.Value;
        }
        private bool Get_Spawn_In_Stage3()
        {
            return !spawn_in_stage3.Value;
        }
        private bool Get_Spawn_In_Stage4()
        {
            return !spawn_in_stage4.Value;
        }
        private bool Get_Spawn_In_Stage5()
        {
            return !spawn_in_stage5.Value;
        }
        private bool Get_Spawn_In_FinalStage()
        {
            return !spawn_in_finalStage.Value;
        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);

            string sceneName = SceneManager.GetActiveScene().name;
            Logger.LogInfo($"MoreOrder : Scene active : {sceneName}");
            if (CheckStageName(sceneName))
            {
                SpawnShrineOfOrder();
            }
        }

        private bool CheckStageName(string sceneName) {
            if (spawn_in_stage1.Value)
            {
                if (UnityEngine.Random.value <= shrineChance_stage1.Value && stages1.Any(prefix => sceneName.StartsWith(prefix)))
                {
                    Logger.LogInfo($"MoreOrder : Stage Valid {sceneName}");
                    return true;
                }
            }

            if (spawn_in_stage2.Value)
            {
                if (UnityEngine.Random.value <= shrineChance_stage2.Value && stages2.Any(prefix => sceneName.StartsWith(prefix)))
                {
                    return true;
                }
            }
            if (spawn_in_stage3.Value)
            {
                if (UnityEngine.Random.value <= shrineChance_stage3.Value && stages3.Any(prefix => sceneName.StartsWith(prefix)))
                {
                    return true;
                }
            }
            if (spawn_in_stage4.Value)
            {
                if (UnityEngine.Random.value <= shrineChance_stage4.Value && stages4.Any(prefix => sceneName.StartsWith(prefix)))
                {
                    return true;
                }
            }
            if (spawn_in_stage5.Value)
            {
                if (UnityEngine.Random.value <= shrineChance_stage5.Value && stages5.Any(prefix => sceneName.StartsWith(prefix)))
                {
                    return true;
                }
            }
            if (spawn_in_finalStage.Value)
            {
                if (UnityEngine.Random.value <= shrineChance_finalStage.Value && finalStage.Any(prefix => sceneName.StartsWith(prefix)))
                {
                    return true;
                }
            }

            return false;
        }

        private void SpawnShrineOfOrder()
        {
            var shrineCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineRestack");
            if (shrineCard == null)
            {
                Logger.LogError("MoreOrder : spawn card not found. ");
                return;
            }

            var placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Random
            };

            var spawnRequest = new DirectorSpawnRequest(shrineCard, placementRule, RoR2.Run.instance.stageRng);
            spawnRequest.ignoreTeamMemberLimit = true;

            var spawned = DirectorCore.instance.TrySpawnObject(spawnRequest);
            if (spawned != null)
            {
                Logger.LogInfo("MoreOrder : Shrined spawned!");
            }
        }
    }
}
