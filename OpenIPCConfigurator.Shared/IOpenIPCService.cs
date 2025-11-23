using System;
using System.Threading.Tasks;

namespace OpenIPCConfigurator.Shared;

public interface IOpenIPCService
{
    // Connection Management
    Task<bool> TestConnectionAsync(string ipAddress, string password, DeviceType deviceType);
    
    // Basic Operations
    Task<bool> DownloadConfigurationAsync(string ipAddress, string password, DeviceType deviceType, bool useYaml = false, string? outputDir = null);
    Task<bool> UploadConfigurationAsync(string ipAddress, string password, DeviceType deviceType, bool useYaml = false, bool reboot = false, string? inputDir = null);
    Task<bool> RebootDeviceAsync(string ipAddress, string password, DeviceType deviceType);
    
    // Key Management
    Task<bool> GenerateKeysAsync(string ipAddress, string password);
    Task<bool> DownloadKeysAsync(string ipAddress, string password, string target = "camera");
    Task<bool> UploadKeysAsync(string ipAddress, string password, string target = "camera");
    
    // UART Management
    Task<bool> EnableUartAsync(string ipAddress, string password);
    Task<bool> DisableUartAsync(string ipAddress, string password);
    
    // MSP/OSD Management
    Task<bool> InstallMspAirAsync(string ipAddress, string password);
    Task<bool> InstallMspGroundAsync(string ipAddress, string password);
    Task<bool> RemoveMspExtraAsync(string ipAddress, string password);
    Task<bool> SetOsdModeAsync(string ipAddress, string password, string mode);
    Task<bool> ConfigureMspGroundStationAsync(string ipAddress, string password);
    
    // Service Management
    Task<bool> RestartServiceAsync(string ipAddress, string password, string serviceName);
    
    // File Transfers
    Task<bool> UploadSensorAsync(string ipAddress, string password, string fileName);
    Task<bool> DownloadSensorAsync(string ipAddress, string password, string fileName);
    Task<bool> UploadKernelModuleAsync(string ipAddress, string password, string fileName);
    Task<bool> DownloadKernelModuleAsync(string ipAddress, string password, string fileName);
    Task<bool> UploadScriptsAsync(string ipAddress, string password);
    Task<bool> DownloadScriptsAsync(string ipAddress, string password);
    
    // Firmware Management
    Task<bool> OfflineFirmwareUpgradeAsync(string ipAddress, string password, string archivePath, string profile, bool force = false);
    
    // Feature Toggles
    Task<bool> EnableRecordingAsync(string ipAddress, string password);
    Task<bool> DisableRecordingAsync(string ipAddress, string password);
    Task<bool> EnableAudioAsync(string ipAddress, string password);
    Task<bool> DisableAudioAsync(string ipAddress, string password);
    Task<bool> SetMavlinkLevelAsync(string ipAddress, string password, int level);
    
    // System Management
    Task<bool> ResetRadxaAsync(string ipAddress, string password);
    Task<bool> FactoryResetCameraAsync(string ipAddress, string password);
    Task<bool> InstallAirManagerAsync(string ipAddress, string password);
    Task<bool> InstallPixelPilotAsync(string ipAddress, string password);
    Task<bool> DeployAlinkAsync(string ipAddress, string password, string profile);
    
    // Video Management
    Task<bool> SetCropModeAsync(string ipAddress, string password, int mode);
    Task<bool> DisableCropAsync(string ipAddress, string password);
    Task<bool> RunBitTestAsync(string ipAddress, string password, string mlinkValue);
    Task<bool> FixVideoResolutionAsync(string ipAddress, string password);
    
    // Events
    event EventHandler<string>? StatusChanged;
    event EventHandler<string>? OperationCompleted;
    event EventHandler<string>? ErrorOccurred;
}
