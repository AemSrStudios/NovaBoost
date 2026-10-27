using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace NovaBoost;

public partial class MainWindow : Window
{
    private readonly string dataDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "NovaBoost");
    private readonly string stateFile;
    private readonly DispatcherTimer timer = new();
    private ulong previousIdle, previousKernel, previousUser;

    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryStatus
    {
        public uint Length, Load;
        public ulong TotalPhysical, AvailablePhysical;
        public ulong TotalPageFile, AvailablePageFile;
        public ulong TotalVirtual, AvailableVirtual, AvailableExtendedVirtual;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FileTime
    {
        public uint Low, High;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MemoryStatus buffer);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSystemTimes(
        out FileTime idle, out FileTime kernel, out FileTime user);

    private static ulong ToUInt64(FileTime t) =>
        ((ulong)t.High << 32) | t.Low;

    public MainWindow()
    {
        InitializeComponent();
        Directory.CreateDirectory(dataDir);
        Directory.CreateDirectory(Path.Combine(dataDir, "Logs"));
        stateFile = Path.Combine(dataDir, "restore-state.txt");

        timer.Interval = TimeSpan.FromSeconds(2);
        timer.Tick += (_, _) => RefreshStats();
        Loaded += (_, _) =>
        {
            RefreshStats();
            timer.Start();
        };
        Closed += (_, _) => timer.Stop();
    }

    private void Log(string message)
    {
        StatusText.Text = message;
        try
        {
            File.AppendAllText(
                Path.Combine(dataDir, "Logs", "NovaBoost.log"),
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
        }
        catch { }
    }

    private void RefreshStats()
    {
        try
        {
            if (GetSystemTimes(out var idle, out var kernel, out var user))
            {
                ulong i = ToUInt64(idle);
                ulong k = ToUInt64(kernel);
                ulong u = ToUInt64(user);
                ulong total = (k - previousKernel) + (u - previousUser);
                ulong idleDelta = i - previousIdle;

                if (previousKernel != 0 && total > 0)
                {
                    double cpu = Math.Clamp(
                        100.0 * (total - Math.Min(total, idleDelta)) / total, 0, 100);
                    CpuValue.Text = $"{cpu:0}%";
                    CpuBar.Value = cpu;
                }

                previousIdle = i;
                previousKernel = k;
                previousUser = u;
            }
        }
        catch { CpuValue.Text = "N/A"; }

        try
        {
            var m = new MemoryStatus
            {
                Length = (uint)Marshal.SizeOf<MemoryStatus>()
            };

            if (GlobalMemoryStatusEx(ref m) && m.TotalPhysical > 0)
            {
                double used = 100.0 * (m.TotalPhysical - m.AvailablePhysical)
                    / m.TotalPhysical;
                RamValue.Text = $"{used:0}%";
                RamBar.Value = Math.Clamp(used, 0, 100);
                RamDetail.Text =
                    $"{(m.TotalPhysical - m.AvailablePhysical) / 1073741824d:0.0} / " +
                    $"{m.TotalPhysical / 1073741824d:0.0} GB";
            }
        }
        catch { RamValue.Text = "N/A"; }

        PowerValue.Text = GetPowerPlan();
    }

    private static string GetPowerPlan()
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo
            {
                FileName = "powercfg.exe",
                Arguments = "/getactivescheme",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
            if (p == null) return "Unknown";
            string output = p.StandardOutput.ReadToEnd().Trim();
            p.WaitForExit(3000);
            return output.Length > 0 ? output : "Unknown";
        }
        catch { return "Unavailable"; }
    }

    private void Dashboard_Click(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Dashboard";
        PageSubtitle.Text = "Every frame matters.";
        RefreshStats();
        Log("Dashboard refreshed.");
    }

    private void FPS_Click(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "FPS Lab";
        PageSubtitle.Text = "Apply and restore selected Windows gaming settings.";
        Log("FPS Lab opened. Choose settings and review before applying.");
    }

    private void Cleaner_Click(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Cleaner";
        PageSubtitle.Text = "Scan first. Delete only after confirmation.";

        try
        {
            string temp = Path.GetTempPath();
            int count = 0;
            long bytes = 0;

            foreach (string f in Directory.EnumerateFiles(
                temp, "*", SearchOption.TopDirectoryOnly))
            {
                try { bytes += new FileInfo(f).Length; count++; }
                catch { }
            }

            var result = MessageBox.Show(
                $"Temporary folder:\n{temp}\n\nFiles: {count}\n" +
                $"Approximate size: {bytes / 1048576d:0.0} MB\n\n" +
                "Delete accessible files directly in this folder? " +
                "Files in use will be skipped.",
                "NovaBoost Cleaner", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                Log("Temporary cleanup cancelled.");
                return;
            }

            int deleted = 0;
            foreach (string f in Directory.EnumerateFiles(
                temp, "*", SearchOption.TopDirectoryOnly))
            {
                try { File.Delete(f); deleted++; }
                catch { }
            }

            Log($"Temporary cleanup complete. Deleted {deleted} files.");
        }
        catch (Exception ex) { Log("Cleaner error: " + ex.Message); }
    }

    private void Startup_Click(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Startup Apps";
        PageSubtitle.Text = "Manage startup applications in Windows Settings.";
        try
        {
            Process.Start(new ProcessStartInfo("ms-settings:startupapps")
            {
                UseShellExecute = true
            });
            Log("Opened Windows Startup Apps.");
        }
        catch (Exception ex) { Log("Startup settings error: " + ex.Message); }
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        PageTitle.Text = "Settings";
        PageSubtitle.Text = "Open-source build. No paid features.";
        MessageBox.Show(
            $"Data and logs:\n{dataDir}\n\n" +
            "NovaBoost does not disable Windows Defender, updates, or essential services.",
            "NovaBoost Settings", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Optimize_Click(object sender, RoutedEventArgs e)
    {
        RefreshStats();
        Log("System analysis completed. No settings changed.");
        MessageBox.Show(
            "System readings refreshed.\n\n" +
            "These readings do not predict a specific FPS gain. " +
            "Compare frame rate and frame-time in the same game scene before and after changes.",
            "NovaBoost Analysis", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void SaveOriginalState()
    {
        if (File.Exists(stateFile))
            return;

        string scheme = GetSchemeGuid();
        string gameMode = ReadRegistryValue(
            @"Software\Microsoft\GameBar", "AutoGameModeEnabled");
        string allowMode = ReadRegistryValue(
            @"Software\Microsoft\GameBar", "AllowAutoGameMode");

        File.WriteAllLines(stateFile, new[]
        {
            scheme,
            gameMode,
            allowMode
        });
    }

    private static string GetSchemeGuid()
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo
            {
                FileName = "powercfg.exe",
                Arguments = "/getactivescheme",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
            if (p == null) return "";
            string s = p.StandardOutput.ReadToEnd();
            p.WaitForExit(3000);
            var match = System.Text.RegularExpressions.Regex.Match(
                s, @"[0-9a-fA-F]{8}-(?:[0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}");
            return match.Success ? match.Value : "";
        }
        catch { return ""; }
    }

    private static string ReadRegistryValue(string key, string name)
    {
        try
        {
            using var k = Registry.CurrentUser.OpenSubKey(key);
            if (k == null || k.GetValue(name) == null) return "<missing>";
            return Convert.ToString(k.GetValue(name)) ?? "<missing>";
        }
        catch { return "<missing>"; }
    }

    private static void WriteGameMode(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\GameBar");
        key.SetValue("AutoGameModeEnabled", enabled ? 1 : 0, RegistryValueKind.DWord);
        key.SetValue("AllowAutoGameMode", enabled ? 1 : 0, RegistryValueKind.DWord);
    }

    private static bool RunPowerCfg(string arguments)
    {
        using var p = Process.Start(new ProcessStartInfo
        {
            FileName = "powercfg.exe",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        });
        if (p == null) return false;
        p.WaitForExit(5000);
        return p.HasExited && p.ExitCode == 0;
    }

    private void ApplyProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SaveOriginalState();

            bool gameMode = GameModeCheck.IsChecked == true;
            WriteGameMode(gameMode);

            bool highPower = HighPowerCheck.IsChecked == true;
            bool powerOk = RunPowerCfg(
                highPower
                    ? "/setactive SCHEME_MIN"
                    : "/setactive SCHEME_BALANCED");

            RefreshStats();
            Log($"Profile applied. Game Mode: {gameMode}; power command success: {powerOk}.");

            MessageBox.Show(
                "Selected settings were processed.\n\n" +
                $"Windows Game Mode enabled: {gameMode}\n" +
                $"Power plan command succeeded: {powerOk}\n\n" +
                "If a setting is unavailable in your Windows edition, it may not take effect. " +
                "Restore saved settings can undo the recorded starting configuration.",
                "NovaBoost Gaming Profile", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            Log("Profile error: " + ex.Message);
            MessageBox.Show(ex.Message, "NovaBoost Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Restore_Click(object sender, RoutedEventArgs e)
    {
        if (!File.Exists(stateFile))
        {
            MessageBox.Show("No saved original settings were found.",
                "NovaBoost", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            string[] s = File.ReadAllLines(stateFile);
            if (s.Length < 3) throw new InvalidDataException("Restore file is incomplete.");

            if (!string.IsNullOrWhiteSpace(s[0]))
                RunPowerCfg("/setactive " + s[0]);

            RestoreRegistryValue("AutoGameModeEnabled", s[1]);
            RestoreRegistryValue("AllowAutoGameMode", s[2]);

            File.Delete(stateFile);
            RefreshStats();
            Log("Saved original settings restored.");
            MessageBox.Show("Restore attempt completed. Check the displayed power plan.",
                "NovaBoost", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            Log("Restore error: " + ex.Message);
            MessageBox.Show(ex.Message, "NovaBoost Restore",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void RestoreRegistryValue(string name, string value)
    {
        using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\GameBar");
        if (value == "<missing>")
            key.DeleteValue(name, false);
        else if (int.TryParse(value, out int n))
            key.SetValue(name, n, RegistryValueKind.DWord);
        else
            key.SetValue(name, value, RegistryValueKind.String);
    }
}
