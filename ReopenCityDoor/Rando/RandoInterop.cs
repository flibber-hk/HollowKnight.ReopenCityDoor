using System;
using System.IO;
using System.Linq;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using Modding;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.Logging;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace ReopenCityDoor.Rando
{
    public static class RandoInterop
    {
        public static void Hook(bool rando, bool ic)
        {
            if (ic) DefineGateItem();

            if (rando) HookRandomizer();
        }

        private static void HookRandomizer()
        {
            DefineLogic();
            AddMenuPage();
            AddInfoToRequest();
            SetInitialization();

            if (ModHooks.GetMod("RandoSettingsManager") is not null)
            {
                RandoSettingsManagerInterop.Hook();
            }

            CondensedSpoilerLogger.AddCategory("ReopenCityDoor", (args) => true, new() { Consts.FungalCityGateItemName });
        }

        private static void DefineLogic()
        {
            RCData.RuntimeLogicOverride.Subscribe(48f, DefineTermsAndItems);
            RCData.RuntimeLogicOverride.Subscribe(48f, SetTransitionLogic);
        }

        private static void DefineTermsAndItems(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (ReopenCityDoor.GS.RandoSetting == GlobalSettings.RandoGateSetting.Disabled) return;

            Term cityGateTerm = lmb.GetOrAddTerm(Consts.CityGateOpenLogicTerm);
            lmb.AddItem(new SingleItem(Consts.FungalCityGateItemName, new TermValue(cityGateTerm, 1)));
        }

        private static void SetTransitionLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (ReopenCityDoor.GS.RandoSetting == GlobalSettings.RandoGateSetting.Disabled) return;

            lmb.AddTransition(new RawLogicDef(
                $"Fungus2_08[{Consts.FungalGateName}]", 
                $"(Fungus2_08[left1] | Fungus2_08[left2] | Fungus2_08[right1] | Fungus2_08[{Consts.FungalGateName}]) + {Consts.CityGateOpenLogicTerm}"));
            lmb.AddTransition(new RawLogicDef(
                $"Crossroads_49b[{Consts.CityGateName}]", 
                $"(Crossroads_49b[right1] | Left_Elevator | Crossroads_49b[{Consts.CityGateName}]) + {Consts.CityGateOpenLogicTerm}"));

            lmb.DoLogicEdit(new RawLogicDef("Fungus2_08[left1]", $"ORIG | Fungus2_08[{Consts.FungalGateName}]"));
            lmb.DoLogicEdit(new RawLogicDef("Fungus2_08[left2]", $"ORIG | Fungus2_08[{Consts.FungalGateName}]"));
            lmb.DoLogicEdit(new RawLogicDef("Fungus2_08[right1]", $"ORIG | Fungus2_08[{Consts.FungalGateName}]"));

            lmb.DoLogicEdit(new RawLogicDef("Crossroads_49b[right1]", $"ORIG | Crossroads_49b[{Consts.CityGateName}]"));

            lmb.DoSubst(new RawSubstDef("Left_Elevator", "Crossroads_49b[right1]", $"Crossroads_49b[right1] | Crossroads_49b[{Consts.CityGateName}]"));
        }

        private static void AddMenuPage()
        {
            RandoMenuPage.Hook();
        }

        private static void AddInfoToRequest()
        {
            RequestBuilder.OnUpdate.Subscribe(-1000, SetupRefs);
            RequestBuilder.OnUpdate.Subscribe(-799, PlaceTransition);
            RequestBuilder.OnUpdate.Subscribe(0.5f, AddKeyItem);
            // Deranged constraint auto-handled by randomizer
        }

        private static void SetupRefs(RequestBuilder rb)
        {
            if (ReopenCityDoor.GS.RandoSetting == GlobalSettings.RandoGateSetting.Disabled) return;

            rb.EditItemRequest(Consts.FungalCityGateItemName, info =>
            {
                info.getItemDef = () => new()
                {
                    Name = Consts.FungalCityGateItemName,
                    Pool = "Keys",
                    MajorItem = false,
                    PriceCap = 500
                };
            });

            rb.EditTransitionRequest($"Fungus2_08[{Consts.FungalGateName}]", info =>
            {
                info.getTransitionDef = () => new()
                {
                    SceneName = SceneNames.Fungus2_08,
                    DoorName = Consts.FungalGateName,
                    VanillaTarget = $"Crossroads_49b[{Consts.CityGateName}]",
                    Direction = RandomizerMod.RandomizerData.TransitionDirection.Right,
                    IsTitledAreaTransition = true,
                    IsMapAreaTransition = true,
                    Sides = RandomizerMod.RandomizerData.TransitionSides.Both
                };
            });

            rb.EditTransitionRequest($"Crossroads_49b[{Consts.CityGateName}]", info =>
            {
                info.getTransitionDef = () => new()
                {
                    SceneName = SceneNames.Crossroads_49b,
                    DoorName = Consts.CityGateName,
                    VanillaTarget = $"Fungus2_08[{Consts.FungalGateName}]",
                    Direction = RandomizerMod.RandomizerData.TransitionDirection.Left,
                    IsTitledAreaTransition = true,
                    IsMapAreaTransition = true,
                    Sides = RandomizerMod.RandomizerData.TransitionSides.Both
                };
            });

            rb.OnGetGroupFor.Subscribe(0f, MatchGroups);

            static bool MatchGroups(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
            {
                if (item == Consts.FungalCityGateItemName && (type == RequestBuilder.ElementType.Unknown || type == RequestBuilder.ElementType.Item))
                {
                    gb = rb.GetGroupFor(ItemNames.Tram_Pass);
                    return true;
                }

                if (item == $"Crossroads_49b[{Consts.CityGateName}]" && (type == RequestBuilder.ElementType.Unknown || type == RequestBuilder.ElementType.Transition))
                {
                    gb = rb.GetGroupFor($"Ruins1_01[left1]");
                    return true;
                }

                if (item == $"Fungus2_08[{Consts.FungalGateName}]" && (type == RequestBuilder.ElementType.Unknown || type == RequestBuilder.ElementType.Transition))
                {
                    gb = rb.GetGroupFor($"Fungus2_21[right1]");
                    return true;
                }

                gb = default;
                return false;
            }
        }

        private static void PlaceTransition(RequestBuilder rb)
        {
            bool shouldRandomizeTransition = rb.gs.TransitionSettings.Mode == TransitionSettings.TransitionMode.MapAreaRandomizer
                    || rb.gs.TransitionSettings.Mode == TransitionSettings.TransitionMode.FullAreaRandomizer
                    || rb.gs.TransitionSettings.Mode == TransitionSettings.TransitionMode.RoomRandomizer;

            switch (ReopenCityDoor.GS.RandoSetting)
            {
                case GlobalSettings.RandoGateSetting.Disabled:
                    break;
                case GlobalSettings.RandoGateSetting.DefineRefs:
                case GlobalSettings.RandoGateSetting.Unlockable when !shouldRandomizeTransition:
                case GlobalSettings.RandoGateSetting.Open when !shouldRandomizeTransition:
                    rb.EnsureVanillaSourceTransition($"Crossroads_49b[{Consts.CityGateName}]");
                    rb.EnsureVanillaSourceTransition($"Fungus2_08[{Consts.FungalGateName}]");
                    break;
                case GlobalSettings.RandoGateSetting.Unlockable when shouldRandomizeTransition:
                case GlobalSettings.RandoGateSetting.Open when shouldRandomizeTransition:
                    switch (rb.gs.TransitionSettings.TransitionMatching)
                    {
                        case TransitionSettings.TransitionMatchingSetting.MatchingDirections:
                        case TransitionSettings.TransitionMatchingSetting.MatchingDirectionsAndNoDoorToDoor:
                            SymmetricTransitionGroupBuilder stgb = rb.EnumerateTransitionGroups().First(x => x.label == RBConsts.InLeftOutRightGroup) as SymmetricTransitionGroupBuilder;
                            stgb.Group1.Add($"Fungus2_08[{Consts.FungalGateName}]");
                            stgb.Group2.Add($"Crossroads_49b[{Consts.CityGateName}]");
                            break;
                        case TransitionSettings.TransitionMatchingSetting.NonmatchingDirections:
                            SelfDualTransitionGroupBuilder tgb = rb.EnumerateTransitionGroups().First(x => x.label == RBConsts.TwoWayGroup) as SelfDualTransitionGroupBuilder;
                            tgb.Transitions.Add($"Crossroads_49b[{Consts.CityGateName}]");
                            tgb.Transitions.Add($"Fungus2_08[{Consts.FungalGateName}]");
                            break;
                    }
                    break;
            }
        }

        private static void AddKeyItem(RequestBuilder rb)
        {
            if (ReopenCityDoor.GS.RandoSetting == GlobalSettings.RandoGateSetting.Unlockable)
            {
                rb.AddItemByName(Consts.FungalCityGateItemName);
                if (rb.gs.DuplicateItemSettings.DuplicateUniqueKeys)
                {
                    rb.AddItemByName($"{PlaceholderItem.Prefix}{Consts.FungalCityGateItemName}");
                }
            }
        }

        private static void SetInitialization()
        {
            ProgressionInitializer.OnCreateProgressionInitializer += UnlockGateInLogic;
            RandoController.OnExportCompleted += OpenGateOnExportComplete;
            SettingsLog.AfterLogSettings += AddSettingsToLog;
        }

        private static void AddSettingsToLog(LogArguments args, TextWriter tw)
        {
            tw.WriteLine($"ReopenCityDoor.RandoSetting: {ReopenCityDoor.GS.RandoSetting}");
        }

        private static void OpenGateOnExportComplete(RandoController rc)
        {
            if (ReopenCityDoor.GS.RandoSetting == GlobalSettings.RandoGateSetting.Open)
            {
                ItemChangerMod.Modules.GetOrAdd<CityGateModule>().GateOpen = true;
            }
        }

        private static void UnlockGateInLogic(LogicManager lm, GenerationSettings gs, ProgressionInitializer pi)
        {
            if (ReopenCityDoor.GS.RandoSetting == GlobalSettings.RandoGateSetting.Open)
            {
                if (lm.TermLookup.TryGetValue(Consts.CityGateOpenLogicTerm, out Term t))
                {
                    pi.Setters.Add(new TermValue(t, 1));
                }
                else
                {
                    ReopenCityDoor.instance.LogError("Term not found");
                }
            }
        }

        private static void DefineGateItem()
        {
            AbstractItem cityGateItem = new CustomSkillItem()
            {
                name = Consts.FungalCityGateItemName,
                boolName = Consts.FungalCityGateBoolName,
                moduleName = "ReopenCityDoor.Rando.CityGateModule, ReopenCityDoor",
                UIDef = new MsgUIDef()
                {
                    name = new BoxedString("Fungal-City Gate Key"),
                    shopDesc = new BoxedString("A mysterious traveller dropped this key. It seemed like they were waiting for something..."),
                    sprite = new EmbeddedSprite("citydoorkey")
                }
            };

            InteropTag tag = cityGateItem.AddTag<InteropTag>();
            tag.Message = "RandoSupplementalMetadata";
            tag.Properties["ModSource"] = nameof(ReopenCityDoor);
            tag.Properties["PoolGroup"] = "Keys";

            Finder.DefineCustomItem(cityGateItem);
        }
    }
}
