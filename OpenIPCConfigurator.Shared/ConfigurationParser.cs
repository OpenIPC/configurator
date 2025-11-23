using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenIPCConfigurator.Shared;

public class ConfigurationParser
{
    public class ConfigurationValues
    {
        public string Frequency { get; set; } = "";
        public string Power { get; set; } = "";
        public string Freq24 { get; set; } = "";
        public string Power24 { get; set; } = "";
        public string Bandwidth { get; set; } = "";
        public string MCS { get; set; } = "";
        public string STBC { get; set; } = "";
        public string LDPC { get; set; } = "";
        public string FECK { get; set; } = "";
        public string FECN { get; set; } = "";
        public string LinkControl { get; set; } = "";
        public string WLANAdapter { get; set; } = "";
        public string Router { get; set; } = "";
        public string Serial { get; set; } = "";
        public string Resolution { get; set; } = "";
        public string FPS { get; set; } = "";
        public string Bitrate { get; set; } = "";
        public string Codec { get; set; } = "";
        public string MLink { get; set; } = "";
        public string Exposure { get; set; } = "";
        public string Contrast { get; set; } = "";
        public string Saturation { get; set; } = "";
        public string Hue { get; set; } = "";
        public string Luminance { get; set; } = "";
        public string Flip { get; set; } = "";
        public string Mirror { get; set; } = "";
        public string Sensor { get; set; } = "";
    }

    public static ConfigurationValues ParseConfiguration(DeviceType deviceType, bool useYaml)
    {
        var values = new ConfigurationValues();

        try
        {
            if (deviceType == DeviceType.OpenIPC)
            {
                if (useYaml)
                {
                    ParseYamlConfiguration(values);
                }
                else
                {
                    ParseWfbConfConfiguration(values);
                }
                
                ParseMajesticConfiguration(values);
            }
            else if (deviceType == DeviceType.NVR)
            {
                ParseNvrConfiguration(values);
            }
            else if (deviceType == DeviceType.Radxa)
            {
                ParseRadxaConfiguration(values);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't crash - return empty values
            Console.WriteLine($"Error parsing configuration: {ex.Message}");
        }

        return values;
    }

    private static void ParseWfbConfConfiguration(ConfigurationValues values)
    {
        var wfbPath = "wfb.conf";
        if (!File.Exists(wfbPath)) return;

        var lines = File.ReadAllLines(wfbPath).ToList();
        if (lines.Count > 7) values.Frequency = ReadLine(7, lines);
        if (lines.Count > 10) values.Power = ReadLine(10, lines);
        if (lines.Count > 8) values.Freq24 = ReadLine(8, lines);
        if (lines.Count > 9) values.Power24 = ReadLine(9, lines);
        if (lines.Count > 11) values.Bandwidth = ReadLine(11, lines);
        if (lines.Count > 14) values.MCS = ReadLine(14, lines);
        if (lines.Count > 12) values.STBC = ReadLine(12, lines);
        if (lines.Count > 13) values.LDPC = ReadLine(13, lines);
        if (lines.Count > 20) values.FECK = ReadLine(20, lines);
        if (lines.Count > 21) values.FECN = ReadLine(21, lines);
    }

    private static void ParseYamlConfiguration(ConfigurationValues values)
    {
        var wfbPath = "wfb.yaml";
        if (!File.Exists(wfbPath)) return;

        var content = File.ReadAllText(wfbPath);
        
        // Parse WFB YAML configuration with proper nested key support
        values.Frequency = ExtractYamlValue(content, "channel");
        values.Power = ExtractYamlValue(content, "txpower");
        values.Bandwidth = ExtractYamlValue(content, "width");
        values.WLANAdapter = ExtractYamlValue(content, "wlan_adapter");
        values.LinkControl = ExtractYamlValue(content, "link_control");
        values.MCS = ExtractYamlValue(content, "mcs_index");
        values.STBC = ExtractYamlValue(content, "stbc");
        values.LDPC = ExtractYamlValue(content, "ldpc");
        values.FECK = ExtractYamlValue(content, "fec_k");
        values.FECN = ExtractYamlValue(content, "fec_n");
        values.Router = ExtractYamlValue(content, "router");
        values.Serial = ExtractYamlValue(content, "serial");
        values.MLink = ExtractYamlValue(content, "mlink");
    }

    private static void ParseMajesticConfiguration(ConfigurationValues values)
    {
        var majesticPath = "majestic.yaml";
        if (!File.Exists(majesticPath)) return;

        var content = File.ReadAllText(majesticPath);
        
        // Parse Majestic YAML with proper nested key support
        values.Resolution = ExtractYamlValue(content, "size");
        values.FPS = ExtractYamlValue(content, "fps");
        values.Bitrate = ExtractYamlValue(content, "bitrate");
        values.Codec = ExtractYamlValue(content, "codec");
        values.Exposure = ExtractYamlValue(content, "exposure");
        values.Contrast = ExtractYamlValue(content, "contrast");
        values.Saturation = ExtractYamlValue(content, "saturation");
        values.Hue = ExtractYamlValue(content, "hue");
        values.Luminance = ExtractYamlValue(content, "luminance");
        values.Flip = ExtractYamlValue(content, "flip");
        values.Mirror = ExtractYamlValue(content, "mirror");
        values.Sensor = ExtractYamlValue(content, "sensorConfig");
    }

    private static void ParseNvrConfiguration(ConfigurationValues values)
    {
        var wfbPath = "wfb.conf";
        if (!File.Exists(wfbPath)) return;

        var lines = File.ReadAllLines(wfbPath).ToList();
        if (lines.Count > 7) values.Frequency = ReadLine(7, lines);
        if (lines.Count > 10) values.Power = ReadLine(10, lines);
        if (lines.Count > 8) values.Freq24 = ReadLine(8, lines);
        if (lines.Count > 9) values.Power24 = ReadLine(9, lines);
        if (lines.Count > 14) values.MCS = ReadLine(14, lines);
        if (lines.Count > 15) values.STBC = ReadLine(15, lines);
    }

    private static void ParseRadxaConfiguration(ConfigurationValues values)
    {
        var wfbngPath = "wifibroadcast.cfg";
        if (!File.Exists(wfbngPath)) return;

        var lines = File.ReadAllLines(wfbngPath).ToList();
        if (lines.Count > 3) values.Frequency = ReadLine(3, lines);
        if (lines.Count > 11) values.Bandwidth = ReadLine(11, lines);
        if (lines.Count > 7) values.MCS = ReadLine(7, lines);
        if (lines.Count > 10) values.STBC = ReadLine(10, lines);

        // Also check wfb.conf for power settings
        var wfbPath = "wfb.conf";
        if (File.Exists(wfbPath))
        {
            var wfbLines = File.ReadAllLines(wfbPath).ToList();
            for (int x = 0; x < wfbLines.Count; x++)
            {
                if (wfbLines[x].StartsWith("options 88XXau_wfb "))
                {
                    if (x + 1 < wfbLines.Count) values.Power = ReadLine(x + 1, wfbLines);
                    break;
                }
            }
        }
    }

    private static string ReadLine(int lineNumber, List<string> lines)
    {
        try
        {
            if (lineNumber > 0 && lineNumber <= lines.Count)
            {
                return lines[lineNumber - 1].Trim();
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return "";
    }

    private static string ExtractYamlValue(string content, string key)
    {
        try
        {
            var lines = content.Split('\n');
            var currentSection = "";
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;
                
                // Check if this is a section header (no leading spaces and ends with :)
                if (!line.StartsWith(" ") && trimmed.EndsWith(":") && !trimmed.Contains(" "))
                {
                    currentSection = trimmed.TrimEnd(':');
                    continue;
                }
                
                // Check for direct key match
                if (trimmed.StartsWith($"{key}:"))
                {
                    var parts = trimmed.Split(':', 2);
                    if (parts.Length == 2)
                    {
                        return CleanValue(parts[1].Trim());
                    }
                }
                
                // Check for nested key match (section.key)
                if (trimmed.Contains(":") && !string.IsNullOrEmpty(currentSection))
                {
                    var keyPart = trimmed.Split(':')[0].Trim();
                    var fullKey = $"{currentSection}.{keyPart}";
                    
                    if (fullKey == key || keyPart == key)
                    {
                        var parts = trimmed.Split(':', 2);
                        if (parts.Length == 2)
                        {
                            return CleanValue(parts[1].Trim());
                        }
                    }
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return "";
    }

    private static string CleanValue(string value)
    {
        // Remove common prefixes and clean values for UI display
        if (string.IsNullOrEmpty(value)) return "";
        
        // Remove quotes
        value = value.Trim('"', '\'');
        
        // Handle boolean values
        if (value.Equals("true", StringComparison.OrdinalIgnoreCase)) return "1";
        if (value.Equals("false", StringComparison.OrdinalIgnoreCase)) return "0";
        
        return value;
    }
}
