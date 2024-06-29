﻿using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class VPR
    {
        public const byte JobID = 41;

        public const uint
            DreadFangs = 34607,
            DreadMaw = 34615,
            Dreadwinder = 34620,
            HuntersCoil = 34621,
            HuntersDen = 34624,
            HuntersSnap = 39166,
            PitofDread = 34623,
            RattlingCoil = 39189,
            Reawaken = 34626,
            SerpentsIre = 34647,
            SerpentsTail = 35920,
            Slither = 34646,
            SteelFangs = 34606,
            SteelMaw = 34614,
            SwiftskinsCoil = 34622,
            SwiftskinsDen = 34625,
            Twinblood = 35922,
            Twinfang = 35921,
            UncoiledFury = 34633,
            WrithingSnap = 34632,
            SwiftskinsSting = 34609,
            TwinfangBite = 34636,
            TwinbloodBite = 34637,
            UncoiledTwinfang = 34644,
            UncoiledTwinblood = 34645,
            HindstingStrike = 34612,
            DeathRattle = 34634,
            HuntersSting = 34608,
            HindsbaneFang = 34613,
            FlankstingStrike = 34610,
            FlanksbaneFang = 34611;

        public static class Buffs
        {
            public const ushort
                FellhuntersVenom = 3659,
                FellskinsVenom = 3660,
                FlanksbaneVenom = 3646,
                FlankstungVenom = 3645,
                HindstungVenom = 3647,
                HindsbaneVenom = 3648,
                GrimhuntersVenom = 3649,
                GrimskinsVenom = 3650,
                HuntersVenom = 3657,
                SwiftskinsVenom = 3658,
                HuntersInstinct = 3668,
                Swiftscaled = 3669;
        }

        public static class Debuffs
        {
            public const ushort
                NoxiousGnash = 3667;
        }

        public static class Config
        {
            public static UserInt
                VPR_NoxiousRefreshRange = new("VPR_NoxiousRefreshRange"),
                VPR_ST_SecondWind_Threshold = new("VPR_STSecondWindThreshold"),
                VPR_ST_Bloodbath_Threshold = new("VPR_STBloodbathThreshold");

        }

        internal class VPR_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VPR_ST_AdvancedMode;
            // internal static VPROpenerLogic VPROpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // VPRGauge? gauge = GetJobGauge<VPRGauge>();
                bool trueNorthReady = TargetNeedsPositionals() && HasCharges(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth);
                int NoxiousRefreshRange = Config.VPR_NoxiousRefreshRange;

                if (actionID is SteelFangs)
                {
                    // Opener for VPR
                    /* if (IsEnabled(CustomComboPreset.VPR_ST_Opener))
                     {
                         if (VPROpener.DoFullOpener(ref actionID, false))
                             return actionID;
                     }*/

                    if (IsEnabled(CustomComboPreset.VPR_ST_RangedUptime) &&
                        LevelChecked(WrithingSnap) && !InMeleeRange() && HasBattleTarget())
                        return WrithingSnap;

                    if (IsEnabled(CustomComboPreset.VPR_ST_Dreadwinder) &&
                        ActionReady(Dreadwinder) && comboTime is 0.0f && HasEffect(Buffs.Swiftscaled))
                        return Dreadwinder;

                    // healing
                    if (IsEnabled(CustomComboPreset.VPR_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.VPR_ST_SecondWind_Threshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.VPR_ST_Bloodbath_Threshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    //1-2-3 (4-5-6) Combo
                    if (IsEnabled(CustomComboPreset.VPR_ST_SerpentsTail) &&
                        CanWeave(actionID) && LevelChecked(SerpentsTail) &&
                        (lastComboMove is HindstingStrike or HindsbaneFang or FlankstingStrike or FlanksbaneFang))
                        return OriginalHook(SerpentsTail);

                    if (GetBuffRemainingTime(Buffs.Swiftscaled) < 10 ||
                        GetDebuffRemainingTime(Debuffs.NoxiousGnash) <= NoxiousRefreshRange)
                        return OriginalHook(DreadFangs);

                    if (GetBuffRemainingTime(Buffs.HuntersInstinct) < 10 ||
                        (GetBuffRemainingTime(Buffs.Swiftscaled) > 10 && GetDebuffRemainingTime(Debuffs.NoxiousGnash) > NoxiousRefreshRange))
                        return OriginalHook(SteelFangs);

                    if (comboTime > 0)
                    {
                        if (LevelChecked(SwiftskinsSting) &&
                            (lastComboMove is DreadFangs or SteelFangs) &&
                            (HasEffect(Buffs.HindstungVenom) || HasEffect(Buffs.HindsbaneVenom)))
                            return OriginalHook(DreadFangs);

                        if (lastComboMove is SwiftskinsSting && LevelChecked(HindstingStrike))
                        {
                            if (HasEffect(Buffs.HindstungVenom))
                                return OriginalHook(SteelFangs);

                            if (HasEffect(Buffs.HindsbaneVenom))
                                return OriginalHook(DreadFangs);

                            if (!HasEffect(Buffs.HindsbaneVenom) && !HasEffect(Buffs.HindstungVenom))
                                return OriginalHook(DreadFangs);
                        }

                        if (LevelChecked(HuntersSting) &&
                            (lastComboMove is DreadFangs or SteelFangs) &&
                            (HasEffect(Buffs.FlankstungVenom) || HasEffect(Buffs.FlanksbaneVenom)))
                            return OriginalHook(SteelFangs);

                        if (lastComboMove is HuntersSting && LevelChecked(FlankstingStrike))
                        {
                            if (HasEffect(Buffs.FlankstungVenom))
                                return OriginalHook(SteelFangs);

                            if (HasEffect(Buffs.FlanksbaneVenom))
                                return OriginalHook(DreadFangs);

                            if (!HasEffect(Buffs.FlankstungVenom) && !HasEffect(Buffs.FlanksbaneVenom))
                                return OriginalHook(SteelFangs);
                        }
                    }
                    return OriginalHook(DreadFangs);
                }
                return actionID;
            }
        }
    }
}
