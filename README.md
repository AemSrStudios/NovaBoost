# ⚡ NovaBoost

### Push your PC further. Take control of your performance.

**NovaBoost is a free, open-source Windows optimization project built for gamers who want more control over their system, a cleaner gaming setup, and a smarter way to manage performance tweaks.**

Forget bloated optimizer apps, fake FPS promises, and features locked behind premium subscriptions. NovaBoost aims to put practical Windows tuning tools into one modern, gaming-focused desktop application.

> 🚧 **Active development:** NovaBoost is a work in progress. Features are being developed and tested, and results will vary depending on your hardware, Windows configuration, and the games you play.

## 🔥 Features

- ⚡ **Ultra Performance Lab** — Explore performance-focused Windows settings and power profiles.
- 🎯 **Low-Latency Lab** — Investigate gaming-related settings that may affect responsiveness and frame pacing.
- 🛠️ **Reversible Registry Tweaks** — Review supported registry changes before applying them, with backups and restoration wherever implemented.
- 🎮 **Gaming Profiles** — Organize optimization settings for different performance goals.
- 🧹 **System Cleanup** — Remove selected temporary files to reclaim disk space.
- 📊 **Live System Monitoring** — View CPU and memory usage in real time.
- 🔋 **Power Plan Control** — Switch between supported Windows power plans.
- 🛡️ **Safety First** — Clear warnings, explicit confirmations, and a focus on undoable changes.
- 🌙 **Modern Dark UI** — A native Windows interface designed around a clean, gaming-inspired aesthetic.
- 🆓 **Free and Open Source** — No subscriptions, premium unlocks, or paid optimization tiers.

*Features are subject to change while development continues. See the current application for what is implemented and available.*

## 🚀 Our goal

NovaBoost isn't about throwing random registry values at Windows and hoping for the best.

The goal is to build a practical optimization toolkit that helps users:

- Reduce unnecessary background overhead where safe and appropriate.
- Explore performance settings without blindly copying internet tweaks.
- Understand the risks and trade-offs before changing system configuration.
- Keep a record of changes and restore previous settings when possible.
- Make informed decisions based on their own hardware and measurements.

**No fake benchmarks. No guaranteed FPS multipliers. Just tools, transparency, and testing.**

## 🎯 Who is NovaBoost for?

NovaBoost is being developed for:

- Gamers using older or lower-end PCs.
- Users who want a more understandable Windows tuning experience.
- People who prefer native Windows applications over web-based interfaces.
- Open-source enthusiasts who want to inspect the code and contribute.
- Anyone who wants to understand what a performance tweak actually changes.

## 🖥️ Technology

| Component | Technology |
|---|---|
| Language | C# |
| Interface | Windows Presentation Foundation (WPF) |
| Framework | .NET 10 |
| Platform | Windows |
| License | MIT |

NovaBoost uses a native WPF interface rather than Electron or an embedded web browser.

## ⚠️ Important safety disclaimer

NovaBoost can expose settings that affect Windows configuration. Incorrect or incompatible changes may cause instability, increased power consumption, unexpected behavior, or degraded gaming performance.

**Before applying system or registry changes:**

1. Read the description and risk warning for each tweak.
2. Review the exact registry key and value being changed.
3. Create a backup or restore point where appropriate.
4. Save your work and close important applications.
5. Apply one change at a time and test the result.
6. Use the restoration feature when available if a change causes problems.

Advanced settings may require administrator privileges. Never apply a tweak merely because it is labeled *Ultra*, *Extreme*, or *Maximum Performance*.

NovaBoost does not promise a specific FPS increase, lower ping, or a guaranteed reduction in input latency. Results depend on your system, drivers, game engine, network, and workload.

NovaBoost is an independent community project and is not affiliated with Microsoft or any game publisher.

## 📈 How to measure results

For meaningful comparisons:

- Use the same game, map, resolution, and graphics settings.
- Compare average FPS alongside frame-time consistency and 1% lows.
- Test input responsiveness using repeatable methods where possible.
- Monitor CPU usage, memory pressure, temperatures, and throttling.
- Repeat tests before and after changes rather than relying on one run.

A tweak that improves one PC may do nothing—or make another PC slower.

## 🛠️ Building from source

### Requirements

- Windows
- .NET 10 SDK
- Git (optional, for cloning the repository)

### Build

```powershell
git clone https://github.com/YOUR-USERNAME/NovaBoost.git
cd NovaBoost
dotnet restore .\NovaBoost.csproj
dotnet build .\NovaBoost.csproj -c Release
```

Replace `YOUR-USERNAME` with the actual GitHub account or organization that hosts the repository.

## 🤝 Contributing

Contributions, bug reports, testing results, and improvement ideas are welcome.

When proposing a new optimization:

- Explain what the setting changes.
- Include reliable documentation or reproducible test results.
- Identify potential risks and supported Windows versions.
- Provide a rollback method when practical.
- Avoid claims that cannot be supported by evidence.

Keep NovaBoost transparent, safe, and useful for the community.

## 🗺️ Roadmap

- [x] Initial native WPF application
- [x] Dark gaming-inspired interface
- [x] Basic CPU and memory monitoring
- [x] Basic power-plan controls
- [ ] Expanded Ultra Performance Lab
- [ ] Low-Latency Lab with documented settings
- [ ] Per-tweak risk labels and confirmation dialogs
- [ ] Automatic registry backup and restoration workflow
- [ ] More robust rollback and failure handling
- [ ] Repeatable benchmark logging
- [ ] Expanded documentation and community testing

Roadmap items are goals, not promises of completed functionality.

## 📄 License

NovaBoost is licensed under the **MIT License**. See [`LICENSE`](LICENSE) for details.

You are welcome to inspect, modify, and contribute to the project under the terms of that license.

---

### ⚡ NovaBoost — Performance tuning without the paywall.

**Free to use. Open to inspect. Built to improve.**
