using System;
using System.Collections.Generic;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using OpenIPCConfigurator.Cli;
using OpenIPCConfigurator.Shared;

namespace OpenIPCConfigurator.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IOpenIPCService _openIPCService;
    
    private string _ipAddress = "";
    private string _password = "";
    private DeviceType _selectedDevice = DeviceType.OpenIPC;
    private bool _isConnected = false;
    private string _connectionStatus = "Disconnected";
    private bool _isConnecting = false;
    private bool _useYamlFormat = true;
    
    // Configuration values
    private string _frequency = "";
    private string _power = "";
    private string _resolution = "";
    private string _fps = "";
    private string _bitrate = "";
    private string _mcs = "";
    private string _bandwidth = "";
    private string _stbc = "";
    private string _ldpc = "";
    
    // Numeric and boolean configuration values for proper controls
    private decimal _powerValue = 20;
    private decimal _mcsValue = 1;
    private bool _stbcEnabled = false;
    private bool _ldpcEnabled = false;
    
    // Additional configuration fields
    private string _codec = "";
    private string _wlanAdapter = "";
    private string _linkControl = "";
    private string _fecK = "";
    private string _fecN = "";
    private string _router = "";
    private string _serial = "";
    private string _exposure = "";
    private string _contrast = "";
    private string _saturation = "";
    private string _hue = "";
    private string _luminance = "";
    private bool _flipEnabled = false;
    private bool _mirrorEnabled = false;
    private string _mlink = "";

    public string IpAddress
    {
        get => _ipAddress;
        set => this.RaiseAndSetIfChanged(ref _ipAddress, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public DeviceType SelectedDevice
    {
        get => _selectedDevice;
        set => this.RaiseAndSetIfChanged(ref _selectedDevice, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        set => this.RaiseAndSetIfChanged(ref _isConnected, value);
    }

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
    }

    public bool IsConnecting
    {
        get => _isConnecting;
        set => this.RaiseAndSetIfChanged(ref _isConnecting, value);
    }

    public bool UseYamlFormat
    {
        get => _useYamlFormat;
        set => this.RaiseAndSetIfChanged(ref _useYamlFormat, value);
    }

    // Configuration Value Properties
    public string Frequency
    {
        get => _frequency;
        set => this.RaiseAndSetIfChanged(ref _frequency, value);
    }

    public string Power
    {
        get => _power;
        set => this.RaiseAndSetIfChanged(ref _power, value);
    }

    public string Resolution
    {
        get => _resolution;
        set => this.RaiseAndSetIfChanged(ref _resolution, value);
    }

    public string FPS
    {
        get => _fps;
        set => this.RaiseAndSetIfChanged(ref _fps, value);
    }

    public string Bitrate
    {
        get => _bitrate;
        set => this.RaiseAndSetIfChanged(ref _bitrate, value);
    }

    public string MCS
    {
        get => _mcs;
        set => this.RaiseAndSetIfChanged(ref _mcs, value);
    }

    public string Bandwidth
    {
        get => _bandwidth;
        set => this.RaiseAndSetIfChanged(ref _bandwidth, value);
    }

    public string STBC
    {
        get => _stbc;
        set => this.RaiseAndSetIfChanged(ref _stbc, value);
    }

    public string LDPC
    {
        get => _ldpc;
        set => this.RaiseAndSetIfChanged(ref _ldpc, value);
    }
    
    // Numeric and boolean properties for proper controls
    public decimal PowerValue
    {
        get => _powerValue;
        set => this.RaiseAndSetIfChanged(ref _powerValue, value);
    }

    public decimal MCSValue
    {
        get => _mcsValue;
        set => this.RaiseAndSetIfChanged(ref _mcsValue, value);
    }

    public bool STBCEnabled
    {
        get => _stbcEnabled;
        set => this.RaiseAndSetIfChanged(ref _stbcEnabled, value);
    }

    public bool LDPCEnabled
    {
        get => _ldpcEnabled;
        set => this.RaiseAndSetIfChanged(ref _ldpcEnabled, value);
    }
    
    // Additional configuration properties
    public string Codec
    {
        get => _codec;
        set => this.RaiseAndSetIfChanged(ref _codec, value);
    }

    public string WLANAdapter
    {
        get => _wlanAdapter;
        set => this.RaiseAndSetIfChanged(ref _wlanAdapter, value);
    }

    public string LinkControl
    {
        get => _linkControl;
        set => this.RaiseAndSetIfChanged(ref _linkControl, value);
    }

    public string FECK
    {
        get => _fecK;
        set => this.RaiseAndSetIfChanged(ref _fecK, value);
    }

    public string FECN
    {
        get => _fecN;
        set => this.RaiseAndSetIfChanged(ref _fecN, value);
    }

    public string Router
    {
        get => _router;
        set => this.RaiseAndSetIfChanged(ref _router, value);
    }

    public string Serial
    {
        get => _serial;
        set => this.RaiseAndSetIfChanged(ref _serial, value);
    }

    public string Exposure
    {
        get => _exposure;
        set => this.RaiseAndSetIfChanged(ref _exposure, value);
    }

    public string Contrast
    {
        get => _contrast;
        set => this.RaiseAndSetIfChanged(ref _contrast, value);
    }

    public string Saturation
    {
        get => _saturation;
        set => this.RaiseAndSetIfChanged(ref _saturation, value);
    }

    public string Hue
    {
        get => _hue;
        set => this.RaiseAndSetIfChanged(ref _hue, value);
    }

    public string Luminance
    {
        get => _luminance;
        set => this.RaiseAndSetIfChanged(ref _luminance, value);
    }

    public bool FlipEnabled
    {
        get => _flipEnabled;
        set => this.RaiseAndSetIfChanged(ref _flipEnabled, value);
    }

    public bool MirrorEnabled
    {
        get => _mirrorEnabled;
        set => this.RaiseAndSetIfChanged(ref _mirrorEnabled, value);
    }

    public string MLink
    {
        get => _mlink;
        set => this.RaiseAndSetIfChanged(ref _mlink, value);
    }

    public ObservableCollection<DeviceType> DeviceTypes { get; } = new()
    {
        DeviceType.OpenIPC,
        DeviceType.NVR,
        DeviceType.Radxa
    };

    public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
    public ReactiveCommand<Unit, Unit> DownloadCommand { get; }
    public ReactiveCommand<Unit, Unit> UploadCommand { get; }
    public ReactiveCommand<Unit, Unit> RebootCommand { get; }
    public ReactiveCommand<Unit, Unit> RestartMajesticCommand { get; }

    // ComboBox item collections
    public List<string> FrequencyOptions { get; } = new List<string>
    {
        "5180 MHz [36]", "5200 MHz [40]", "5220 MHz [44]", "5240 MHz [48]",
        "5260 MHz [52]", "5280 MHz [56]", "5300 MHz [60]", "5320 MHz [64]",
        "5500 MHz [100]", "5520 MHz [104]", "5540 MHz [108]", "5560 MHz [112]",
        "5580 MHz [116]", "5600 MHz [120]", "5620 MHz [124]", "5640 MHz [128]",
        "5660 MHz [132]", "5680 MHz [136]"
    };
    
    public List<string> ResolutionOptions { get; } = new List<string>
    {
        "1280x720", "1456x816", "1920x1080", "1440x1080", "1920x1440",
        "2104x1184", "2208x1248", "2240x1264", "2312x1304", "2436x1828", 
        "2512x1416", "2560x1440", "2560x1920", "3200x1800", "3840x2160"
    };
    
    public List<string> BitrateOptions { get; } = new List<string>
    {
        "5000", "8000", "10000", "12000", "15000", "18000", "20000", "20480",
        "25000", "30000", "35000", "40000"
    };
    
    public List<string> FPSOptions { get; } = new List<string>
    {
        "20", "30", "40", "50", "60", "70", "80", "90", "100", "110", "120"
    };
    
    public List<string> BandwidthOptions { get; } = new List<string>
    {
        "20", "40"
    };
    
    public List<string> CodecOptions { get; } = new List<string>
    {
        "h264", "h265"
    };
    
    public List<string> ExposureOptions { get; } = new List<string>
    {
        "5", "6", "8", "10", "11", "12", "14", "16", "33", "50"
    };
    
    public List<string> ContrastOptions { get; } = new List<string>
    {
        "1", "5", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100"
    };
    
    public List<string> SaturationOptions { get; } = new List<string>
    {
        "1", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100"
    };
    
    public List<string> HueOptions { get; } = new List<string>
    {
        "1", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100"
    };
    
    public List<string> LuminanceOptions { get; } = new List<string>
    {
        "1", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100"
    };

    public MainWindowViewModel() : this(new OpenIPCService())
    {
    }

    public MainWindowViewModel(IOpenIPCService openIPCService)
    {
        _openIPCService = openIPCService;
        
        // Subscribe to service events
        _openIPCService.StatusChanged += (sender, status) => ConnectionStatus = status;
        _openIPCService.ErrorOccurred += (sender, error) => ConnectionStatus = $"Error: {error}";
        
        ConnectCommand = ReactiveCommand.CreateFromTask(Connect);
        DownloadCommand = ReactiveCommand.CreateFromTask(Download, this.WhenAnyValue(x => x.IsConnected));
        UploadCommand = ReactiveCommand.CreateFromTask(Upload, this.WhenAnyValue(x => x.IsConnected));
        RebootCommand = ReactiveCommand.CreateFromTask(Reboot, this.WhenAnyValue(x => x.IsConnected));
        RestartMajesticCommand = ReactiveCommand.CreateFromTask(RestartMajestic, this.WhenAnyValue(x => x.IsConnected));
        
        // Load saved settings if available
        LoadSettings();
    }

    private async Task Connect()
    {
        if (string.IsNullOrWhiteSpace(IpAddress) || string.IsNullOrWhiteSpace(Password))
        {
            ConnectionStatus = "Please enter IP address and password";
            return;
        }

        IsConnecting = true;
        ConnectionStatus = "Testing connection...";
        
        try
        {
            IsConnected = await _openIPCService.TestConnectionAsync(IpAddress, Password, SelectedDevice);
            if (IsConnected)
            {
                ConnectionStatus = $"Connected to {IpAddress} ({SelectedDevice})";
                SaveSettings();
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Connection failed: {ex.Message}";
            IsConnected = false;
        }
        finally
        {
            IsConnecting = false;
        }
    }

    private async Task Download()
    {
        try
        {
            var success = await _openIPCService.DownloadConfigurationAsync(
                IpAddress, Password, SelectedDevice, UseYamlFormat);
            
            if (success)
            {
                // Parse and display the configuration values
                LoadConfigurationValues();
                ConnectionStatus = "Configuration downloaded and loaded";
            }
            else
            {
                ConnectionStatus = "Download failed - check connection and try again";
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Download failed: {ex.Message}";
        }
    }
    
    private void LoadConfigurationValues()
    {
        try
        {
            var config = ConfigurationParser.ParseConfiguration(SelectedDevice, UseYamlFormat);
            
            // Set values for ComboBox SelectedIndex approach
            Frequency = MapFrequencyChannelToDropdownValue(config.Frequency); // Convert "132" → "5660 MHz [132]"
            Power = config.Power;
            Resolution = config.Resolution; // "1920x1080" for direct Content match
            FPS = config.FPS; // "90" for direct Content match  
            Bitrate = config.Bitrate; // "20480" for direct Content match
            MCS = config.MCS;
            Bandwidth = config.Bandwidth; // "20" for direct Content match
            STBC = config.STBC;
            LDPC = config.LDPC;
            
            // Debug logging to see what values we're getting
            ConnectionStatus = $"Loaded: Freq={config.Frequency}, Res={config.Resolution}, FPS={config.FPS}, Bitrate={config.Bitrate}, BW={config.Bandwidth}";
            Console.WriteLine($"DEBUG: Freq='{Frequency}', Res='{Resolution}', FPS='{FPS}', Bitrate='{Bitrate}', BW='{Bandwidth}'");
            
            // Additional configuration values (store raw values to match ComboBox Tags/Content)
            Codec = config.Codec; // "h265" matches Content
            WLANAdapter = config.WLANAdapter;
            LinkControl = config.LinkControl;
            FECK = config.FECK;
            FECN = config.FECN;
            Router = config.Router;
            Serial = config.Serial;
            Exposure = config.Exposure; // "16" matches Content
            Contrast = config.Contrast; // "50" matches Content
            Saturation = config.Saturation; // "50" matches Content  
            Hue = config.Hue; // "50" matches Content
            Luminance = config.Luminance; // "50" matches Content
            MLink = config.MLink;
            
            // Set numeric and boolean values for proper controls
            if (decimal.TryParse(config.Power, out var powerVal))
                PowerValue = powerVal;
                
            if (decimal.TryParse(config.MCS, out var mcsVal))
                MCSValue = mcsVal;
                
            STBCEnabled = config.STBC == "1" || config.STBC.Equals("true", StringComparison.OrdinalIgnoreCase);
            LDPCEnabled = config.LDPC == "1" || config.LDPC.Equals("true", StringComparison.OrdinalIgnoreCase);
            FlipEnabled = config.Flip == "1" || config.Flip.Equals("true", StringComparison.OrdinalIgnoreCase);
            MirrorEnabled = config.Mirror == "1" || config.Mirror.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Error parsing configuration: {ex.Message}";
        }
    }
    
    private async Task RestartMajestic()
    {
        try
        {
            ConnectionStatus = "Restarting Majestic service...";
            
            // Use CLI to restart Majestic service
            var success = await Task.Run(() =>
            {
                try
                {
                    using var session = new SshSession(IpAddress, 22, "root", Password);
                    session.ExecuteCommands(new[] { "/etc/init.d/S95majestic restart" });
                    return true;
                }
                catch (Exception ex)
                {
                    ConnectionStatus = $"Failed to restart Majestic: {ex.Message}";
                    return false;
                }
            });

            if (success)
            {
                ConnectionStatus = "Majestic service restarted successfully";
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Restart Majestic failed: {ex.Message}";
        }
    }
    
    private string MapFrequencyChannelToDropdownValue(string channelOrFreq)
    {
        // If it's already in the right format, return as-is
        if (channelOrFreq.Contains("MHz")) return channelOrFreq;
        
        // Map common channel numbers to frequency dropdown values
        return channelOrFreq switch
        {
            "36" => "5180 MHz [36]",
            "40" => "5200 MHz [40]",
            "44" => "5220 MHz [44]",
            "48" => "5240 MHz [48]",
            "52" => "5260 MHz [52]",
            "56" => "5280 MHz [56]",
            "60" => "5300 MHz [60]",
            "64" => "5320 MHz [64]",
            "100" => "5500 MHz [100]",
            "104" => "5520 MHz [104]",
            "108" => "5540 MHz [108]",
            "112" => "5560 MHz [112]",
            "116" => "5580 MHz [116]",
            "120" => "5600 MHz [120]",
            "124" => "5620 MHz [124]",
            "128" => "5640 MHz [128]",
            "132" => "5660 MHz [132]",
            "136" => "5680 MHz [136]",
            _ => channelOrFreq  // Return original if no mapping found
        };
    }

    private async Task Upload()
    {
        try
        {
            var success = await _openIPCService.UploadConfigurationAsync(
                IpAddress, Password, SelectedDevice, UseYamlFormat, reboot: false);
            
            if (!success)
            {
                ConnectionStatus = "Upload failed - see logs for details";
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Upload failed: {ex.Message}";
        }
    }

    private async Task Reboot()
    {
        try
        {
            var success = await _openIPCService.RebootDeviceAsync(IpAddress, Password, SelectedDevice);
            
            if (success)
            {
                IsConnected = false; // Device will be offline after reboot
            }
            else
            {
                ConnectionStatus = "Reboot failed - see logs for details";
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Reboot failed: {ex.Message}";
        }
    }

    private void LoadSettings()
    {
        try
        {
            var store = OpenIPCConfigurator.Cli.SettingsStore.Load("settings.conf");
            var address = store.TryGetAddress(SelectedDevice.ToKey());
            if (!string.IsNullOrEmpty(address))
            {
                IpAddress = address;
            }
        }
        catch
        {
            // Ignore settings loading errors
        }
    }

    private void SaveSettings()
    {
        try
        {
            var store = OpenIPCConfigurator.Cli.SettingsStore.Load("settings.conf");
            store.SetAddress(SelectedDevice.ToKey(), IpAddress);
            store.Save();
        }
        catch
        {
            // Ignore settings saving errors
        }
    }
}
