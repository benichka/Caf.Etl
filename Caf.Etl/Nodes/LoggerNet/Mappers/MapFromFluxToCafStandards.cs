﻿using Caf.Etl.Nodes.LoggerNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caf.Etl.Models.LoggerNet.TOA5;
using Newtonsoft.Json.Linq;

namespace Caf.Etl.Nodes.LoggerNet.Mappers
{
    public class MapFromFluxDataTableToCafStandards : IMapper
    {
        private readonly string stationsMap;
        private readonly Dictionary<string, string> mapDataFieldsToMeasurementName;
        private readonly Dictionary<string, string> mapStationNameToFieldID;

        public MapFromFluxDataTableToCafStandards()
        {
            // TODO: GODS MANN!  Fix this hardcoded garbage.
            this.stationsMap = "{\"stations\":[{\"name\":\"LTAR_CookEast\",\"lat\":46.78152,\"lon\":-117.08205},{\"name\":\"LTAR_CookWest\",\"lat\":46.78404,\"lon\":-117.09083},{\"name\":\"LTAR_BoydNorth\",\"lat\":46.7551,\"lon\":-117.12605},{\"name\":\"LTAR_BoydSouth\",\"lat\":46.7503,\"lon\":-117.1285}]}";
            this.mapDataFieldsToMeasurementName = new Dictionary<string, string>()
            {
                { "TIMESTAMP", "DateTime" },
                { "RECORD", "RecordNumber" },
                { "Fc_molar", "CO2CorrectedMolar" },
                { "Fc_mass", "CO2CorrectedMass" },
                { "Fc_qc_grade", "CO2CorrectedQuality" },
                { "LE", "HeatFluxLatent" },
                { "LE_qc_grade", "HeatFluxLatentQuality" },
                { "H", "HeatFluxSensible" },
                { "H_qc_grade", "HeatFluxSensibleQuality" },
                { "Rn", "RadiationNet" },
                { "Bowen_ratio", "BowenRatio" },
                { "tau", "MomentumFluxTotal" },
                { "tau_qc_grade", "MomentumFluxTotalQuality" },
                { "u_star", "FrictionVelocity" },
                { "T_star", "TemperatureScalingParameter" },
                { "TKE", "TurbulentKineticEnergy" },
                { "amb_tmpr_Avg", "TemperatureAirTsAvg" },
                { "Td_Avg", "TemperatureDewpointTsAvg" },
                { "RH_Avg", "HumidityTsAvg" },
                { "e_sat_Avg", "VaporPressureTsAvg" },
                { "e_Avg", "VaporPressureSaturationTsAvg" },
                { "amb_press_Avg", "PressureAirTsAvg" },
                { "VPD_air", "VaporPressureDeficit" },
                { "Ux_Avg", "WindSpeedSonicXTsAvg" },
                { "Ux_Std", "WindSpeedSonicXTsStdDev" },
                { "Uy_Avg", "WindSpeedSonicYTsAvg" },
                { "Uy_Std", "WindSpeedSonicYTsStdDev" },
                { "Uz_Avg", "WindSpeedSonicZTsAvg" },
                { "Uz_Std", "WindSpeedSonicZTsStdDev" },
                { "Ts_Avg", "TemperatureSonicTsAvg" },
                { "Ts_Std", "TemperatureSonicTsStdDev" },
                { "sonic_azimuth", "WindDirectionSonic" },
                { "rslt_wnd_spd", "WindSpeedTsResultant" },
                { "std_wnd_dir", "WindDirectionTsStdDev" },
                { "wnd_dir_compass", "WindDirection" },
                { "CO2_molfrac_Avg", "CO2MolFractionWetTsAvg" },
                { "CO2_mixratio_Avg", "CO2MixingRatioDryTsAvg" },
                { "CO2_Avg", "CO2MassDensityTsAvg" },
                { "CO2_Std", "CO2MassDensityTsStdDev" },
                { "H2O_molfrac_Avg", "H2OMolFractionWetTsAvg" },
                { "H2O_mixratio_Avg", "H2OMixingRatioDryTsAvg" },
                { "H2O_Avg", "H2OMassDensityTsAvg" },
                { "H2O_Std", "H2OMassDensityTsStdDev" },
                { "CO2_sig_strgth_Min", "CO2SignalStrengthTsMin" },
                { "H2O_sig_strgth_Min", "H2OSignalStrengthTsMin" },
                { "T_probe_Avg", "ObukhovLengthTsAvg" },
                { "Td_probe_Avg", "ObukhovStabilityParamTsAvg" },
                { "RH_probe_Avg", "HeatFluxSensibleLatentRatioTsAvg" },
                { "Precipitation_Tot", "PrecipitationTsAccum" },
                { "PAR_density_Avg", "ParDensityTsAvg" },
                { "Tsoil_Avg", "TemperatureSoilTsAvg" },
                { "tdr315_wc_Avg(1)", "WaterContentSoilVolumetric1TsAvg" },
                { "tdr315_wc_Avg(2)", "WaterContentSoilVolumetric2TsAvg" },
                { "tdr315_tmpr_Avg(1)", "TemperatureSoil1TsAvg" },
                { "tdr315_tmpr_Avg(2)", "TemperatureSoil2TsAvg" },
                { "tdr315_E_Avg(1)", "ElectricConductivitySoil1TsAvg" },
                { "tdr315_E_Avg(2)", "ElectricConductivitySoil2TsAvg" },
                { "tdr315_bulkEC_Avg(1)", "ElectricConductivityBulkSoil1TsAvg" },
                { "tdr315_bulkEC_Avg(2)", "ElectricConductivityBulkSoil2TsAvg" },
                { "tdr315_poreEC_Avg(1)", "ElectricConductivityPoreSoil1TsAvg" },
                { "tdr315_poreEC_Avg(2)", "ElectricConductivityPoreSoil2TsAvg" },
                { "shf_plate_avg", "HeatFluxSoilTsAvg" },
                { "FP_max", "FootprintContributionDistMax" },
                { "FP_40", "FootprintContributionDist40Perc" },
                { "FP_55", "FootprintContributionDist55Perc" },
                { "FP_90", "FootprintContributionDist90Perc" },
                { "u_Avg_R", "WindSpeedRotationUTsAvg" },
                { "u_Std_R", "WindSpeedRotationUTsStdDev" },
                { "v_Avg_R", "WindSpeedRotationVTsAvg" },
                { "v_Std_R", "WindSpeedRotationVTsStdDev" },
                { "w_Avg_R", "WindSpeedRotationWTsAvg" },
                { "w_Std_R", "WindSpeedRotationWTsStdDev" },
                { "uv_Cov_R", "WindSpeedRotationUVTsCovar" },
                { "uw_Cov_R", "WindSpeedRotationUWTsCovar" },
                { "vw_Cov_R", "WindSpeedRotationVWTsCovar" },
                { "wTs_Cov_R_F_SND", "WTsRotationTsCovar" },
                { "sonic_samples_Tot", "SonicAnemometerSampleCountTsAccum" },
                { "wCO2_Cov_R_F", "CO2WCRotationTsCovar" },
                { "wH2O_Cov_R_F", "H2OWQRotationTsCovar" },
                { "CO2_samples_Tot", "CO2SampleCountTsAccum" },
                { "H2O_samples_Tot", "H2OSampleCountTsAccum" },
                { "height_measurement", "HeightECMeasurement" },
                { "height_canopy", "HeightCanopy" },
                { "surface_type_text", "LandCoverType" },
                { "d", "RoughnessLength" },
                { "z0", "HeightAerodynamic" },
                { "z", "HeightDisplacement" },
                { "L", "ObukhovLength" },
                { "stability_zL", "StabilitySurfaceLayerParameter" },
                { "latitude", "DegreeLatitude" },
                { "longitude", "DegreeLongitude" },
                { "rho_d_Avg", "DensityAirDryTsAvg" },
                { "rho_a_Avg", "DensityAirWetTsAvg" },
                { "door_is_open_Hst", "WasEnclosureOpened" },
                { "panel_tmpr_Avg", "TemperaturePanelTsAvg" },
                { "batt_volt_Avg", "BatteryVoltageTsAvg" }
            };
            this.mapStationNameToFieldID = new Dictionary<string, string>()
            {
                {"LTAR_CookEast", "CookEast" },
                {"LTAR_CookWest", "CookWest" },
                {"LTAR_BoydNorth", "BoydNorth" },
                {"LTAR_BoydSouth", "BoydSouth" }
            };
        }

        public string GetFieldID(Metadata metadata)
        {
            return mapStationNameToFieldID[metadata.StationName];
        }

        public double GetLatFromStation(Metadata metadata)
        {
            var stations = JObject.Parse(stationsMap);

            double lat = stations.Property("stations")
                .Values()
                .Single(s => s.Value<string>("name") == metadata.StationName)
                ["lat"].Value<double>();

            return lat;
        }

        public double GetLonFromStation(Metadata metadata)
        {
            var stations = JObject.Parse(stationsMap);
            double lon = stations.Property("stations")
                .Values()
                .Single(s => s.Value<string>("name") == metadata.StationName)
                ["lon"].Value<double>();

            return lon;
        }

        public string GetMeasurementName(string dataField)
        {
            if(mapDataFieldsToMeasurementName.ContainsKey(dataField))
            {
                return mapDataFieldsToMeasurementName[dataField];
            }
            else { return null; }   
        }
    }
}