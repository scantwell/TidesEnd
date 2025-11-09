# Multiplayer Play Mode Error Fix

## Error Message
```
Moving F:/Code/Unity/TidesEnd/Library/VP/mppm864f4728/Temp/UnityTempFile-...
to F:/Code/Unity/TidesEnd/Library/VP/mppm864f4728/ProjectSettings/InputManager.asset:
The system cannot find the path specified.
```

## Quick Fix (Try This First)

### Option 1: Clear Virtual Player Cache

1. **Close Unity completely**
2. **Navigate to:** `F:\Code\Unity\TidesEnd\Library\VP\`
3. **Delete the entire `VP` folder**
4. **Restart Unity**
5. **Try Multiplayer Play Mode again**

The `VP` folder contains Virtual Player data and will be regenerated cleanly.

---

### Option 2: Disable and Re-enable Multiplayer Play Mode

1. **Window → Multiplayer Play Mode**
2. **Click "Disable Multiplayer Play Mode"** (top right)
3. **Close the window**
4. **Reopen:** Window → Multiplayer Play Mode
5. **Click "Enable Multiplayer Play Mode"**
6. **Try again**

---

### Option 3: Fix Project Settings Access

The error suggests Unity can't access ProjectSettings folder. Check:

1. **Close Unity**
2. **Right-click** `F:\Code\Unity\TidesEnd` folder
3. **Properties → Security tab**
4. **Ensure your user has "Full Control"**
5. **Click "Advanced" → Enable inheritance if disabled**
6. **Apply to all subfolders**
7. **Restart Unity**

---

## Why This Happens

**Multiplayer Play Mode (MPPM)** creates virtual player instances:
- Each instance gets a copy of project settings
- Virtual players run in separate processes
- They share the same Library but have isolated settings

**The error occurs when:**
- Directory structure not created properly
- Temp files cleaned up mid-process
- File permissions issue
- Unity crash during previous MPPM session

---

## Alternative Testing Methods

If Multiplayer Play Mode keeps giving issues, you can test multiplayer using:

### Method 1: Build and Run

**Most Reliable:**
1. **File → Build Settings**
2. **Build the game** (Builds folder)
3. **Run the built executable** as Player 1
4. **Press Play in Unity Editor** as Player 2

**Pros:** Real networking, no weird errors
**Cons:** Slower iteration (need to rebuild)

---

### Method 2: ParrelSync (Third-Party Tool)

**Install ParrelSync:**
1. **Window → Package Manager**
2. **+ → Add package from git URL**
3. **Enter:** `https://github.com/VeriorPies/ParrelSync.git?path=/ParrelSync`
4. **Wait for install**

**Create Clone:**
1. **ParrelSync → Clones Manager**
2. **Create new clone project**
3. **Open clone in another Unity Editor instance**
4. **Run both Editor instances**

**Pros:** Both in editor, better than MPPM
**Cons:** Uses disk space (duplicate project)

---

### Method 3: Use MPPM with Fewer Virtual Players

If you must use MPPM:

1. **Window → Multiplayer Play Mode**
2. **Set "Virtual Players" to 1** (not 2 or more)
3. **Test with just Main Editor + 1 Virtual Player**
4. **Reduces chance of errors**

---

## Long-Term Solution

### Check Unity Version

This error is more common in certain Unity versions.

**Your version:** Unity 6000.2.8f1

**Fixes:**
- Check Unity Forums for known issues: https://forum.unity.com/
- Update to latest patch if available
- Report bug to Unity if persistent

---

## If Error Persists

### Check These:

1. **Antivirus/Firewall:**
   - Antivirus may block Unity from creating temp files
   - Add Unity Editor to exclusions
   - Add project folder to exclusions

2. **Disk Space:**
   - Multiplayer Play Mode needs temp space
   - Check `F:\` drive has free space
   - Minimum 1GB free recommended

3. **Path Length:**
   - Windows has 260 character path limit
   - Your path: `F:\Code\Unity\TidesEnd\Library\VP\mppm...\ProjectSettings\InputManager.asset`
   - If close to 260 chars, move project to shorter path like `F:\Projects\TidesEnd`

4. **Clean Unity Cache:**
   ```
   Close Unity
   Delete: F:\Code\Unity\TidesEnd\Library\ShaderCache
   Delete: F:\Code\Unity\TidesEnd\Library\APIUpdater
   Delete: F:\Code\Unity\TidesEnd\Temp (if exists)
   Restart Unity
   ```

---

## Recommended Workflow

For development, use **Build + Editor** method:

```
1. Make code changes
2. Test in Editor (solo)
3. When ready to test multiplayer:
   - Build once
   - Run build as Host
   - Connect from Editor as Client
4. Iterate quickly with hot reload in Editor
```

This avoids all MPPM issues and tests real networking conditions.

---

## Nuclear Option: Reimport Project

If nothing works:

1. **Backup:**
   - Copy `Assets` folder
   - Copy `Packages` folder
   - Copy `ProjectSettings` folder

2. **Delete:**
   - `Library` folder entirely
   - `Temp` folder
   - `Logs` folder
   - `obj` folder
   - `.csproj` and `.sln` files

3. **Reopen project in Unity**
   - Unity will reimport everything (takes 5-10 min)
   - Fresh start without corrupted cache

---

## TL;DR

**Quick Fix:**
```
1. Close Unity
2. Delete F:\Code\Unity\TidesEnd\Library\VP
3. Restart Unity
```

**Better Testing:**
```
1. Build game once
2. Run build as Player 1
3. Test in Editor as Player 2
```

**Best Practice:**
- Use Build + Editor for multiplayer testing
- Only use MPPM for quick checks
- ParrelSync if you need multiple Editors

---

## Is This Error Harmful?

**Short answer: No, it's annoying but not harmful.**

The error is just Unity failing to copy a config file to a virtual player. The virtual player will use default input settings instead.

**Impact:**
- Virtual player might not respond to input correctly
- Main editor player works fine
- No data loss or corruption

**When to worry:**
- If error appears when NOT using Multiplayer Play Mode
- If it prevents project from opening
- If it causes crashes

Otherwise, it's just Unity being Unity with MPPM. 🤷

---

**Recommended:** Delete `Library/VP` folder and use Build + Editor for testing.
