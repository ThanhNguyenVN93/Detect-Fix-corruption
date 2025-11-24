KB Repair Tool

A lightweight, portable utility designed to detect specific Windows 11 update issues (specifically KB5062553) and repair system UI corruption using Microsoft's official PowerShell scripts.

🚀 Overview
Some Windows 11 updates (like KB5062553) may cause UI glitches, such as the Taskbar disappearing, the Start Menu not responding, or system loops. This tool automates the troubleshooting process by checking your system version and re-registering essential Windows Shell components.

✨ Features
System Validation: Automatically checks if the OS is Windows 11 version 24H2 (Build 26100) or newer before running to prevent compatibility issues.

KB Detection: Verifies if the specific update package is installed on the system.

One-Click Repair: Automates the execution of complex PowerShell scripts to fix UI corruption.

Portable: No installation required. Runs directly as a standalone .exe file.

🛠 How It Works
The tool executes the following Microsoft-recommended fix (via PowerShell) to re-register the Windows Client Shell packages:

📋 Prerequisites
Operating System: Windows 11 version 24H2 (Build 26100) or higher.

Runtime: .NET Framework 4.6 (Pre-installed on most Windows 11 systems).

Privileges: Must be run as Administrator.

📥 Installation & Usage
Download the latest release from the Releases page.

Extract the ZIP file.

Right-click on KBRepairTool.exe and select Run as Administrator.

Click "Check System" to verify your OS version and KB status.

Click "Fix / Repair" to apply the patch.

Restart your computer after the process is complete.

⚠️ Disclaimer
This software is provided "as is", without warranty of any kind. While it uses official Microsoft troubleshooting commands, the author is not responsible for any damage or data loss that may occur. Always back up your data before running system repair tools.

📜 License
This project is licensed under the MIT License - see the LICENSE file for details.
