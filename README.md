> [!Warning]
> This tool disables the anti-cheat system and its errors, which may result in Vanguard Event being triggered in-game due to the absence of an active anti-cheat session.

# NoVgLoL
Small tool to suppress VAN errors in League Client and disables enforcement of Vanguard for both MacOS and Windows. Also disables client telemetry and removes bloatware.

# Usage
On Windows just run the exe. On MacOS, you need to make the file executable, to do this, cd to the directory its located in, then run the following command:
```bash
chmod +x NoVgLoL
```
### Arguments
**--strict** : restores older League Client version without embedded Vanguard module on MacOS in addition to just suppressing errors.

**--norestart** : use this if you dont want to use the vanguard disabler but instead what to bypass the Riot Client prompt to restart your PC in order to play.
