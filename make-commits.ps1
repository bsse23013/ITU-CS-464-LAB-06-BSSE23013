# make-commits.ps1
# Makes the Lab 06 commits one step at a time (scripts, then each level, then README).
#
# How to use:
#   1. Close Unity (so it does not recreate files while this runs).
#   2. Open PowerShell in the project folder (the one that has Assets and .git).
#   3. Run:  powershell -ExecutionPolicy Bypass -File .\make-commits.ps1
#   4. Delete this file afterwards. Do not commit it.
#
# Add -Push to push at the end:  .\make-commits.ps1 -Push

param([switch]$Push)

$ErrorActionPreference = "Continue"

if (-not (Test-Path ".git")) { Write-Host "Run this from the project folder (no .git here)."; exit 1 }
if (-not (Test-Path "Assets")) { Write-Host "Run this from the project folder (no Assets here)."; exit 1 }

# Files left over from the old runtime level builder. They are not used any more.
$leftovers = @(
    "Assets/LevelBuilder.cs", "Assets/LevelBuilder.cs.meta",
    "Assets/LevelSpec.cs", "Assets/LevelSpec.cs.meta",
    "Assets/Bootstrap.cs", "Assets/Bootstrap.cs.meta",
    "Assets/MovingHazard.cs", "Assets/MovingHazard.cs.meta",
    "Assets/KillZone.cs", "Assets/KillZone.cs.meta",
    "Assets/Materials/Sweeper.mat", "Assets/Materials/Sweeper.mat.meta",
    "Assets/Scenes/SampleScene.unity", "Assets/Scenes/SampleScene.unity.meta"
)

function Commit-Step {
    param([string]$Message, [string[]]$Paths)
    foreach ($p in $Paths) { git add -A -- $p 2>$null }
    git diff --cached --quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Nothing new for: $Message"
    } else {
        git commit -m $Message | Out-Null
        Write-Host "Committed: $Message"
    }
}

# 0. clear out the old files first
foreach ($f in $leftovers) {
    if (Test-Path $f) { Remove-Item $f -Force }
}

# 1. project setup
Commit-Step "Add Unity project settings and packages" @(
    ".gitignore", "Packages", "ProjectSettings", "Assets/Settings", "Assets/Settings.meta",
    "Assets/InputSystem_Actions.inputactions", "Assets/InputSystem_Actions.inputactions.meta"
)

# 2. player
Commit-Step "Add player controller, health and input" @(
    "Assets/PlayerController.cs*", "Assets/Health.cs*", "Assets/GameInput.cs*"
)

# 3. pickups
Commit-Step "Add collectibles, power-ups and spinner" @(
    "Assets/Collectible.cs*", "Assets/PowerUp.cs*", "Assets/Spinner.cs*"
)

# 4. enemies and hazards
Commit-Step "Add guard patrol, chase and hazards" @(
    "Assets/EnemyPatrol.cs*", "Assets/EnemyGuard.cs*", "Assets/Hazard.cs*", "Assets/Goal.cs*"
)

# 5. game manager
Commit-Step "Add GameManager with lives, timer, win and lose" @("Assets/GameManager.cs*")

# 6. materials
Commit-Step "Add materials" @("Assets/Materials", "Assets/Materials.meta")

# 7. menu
Commit-Step "Add main menu scene" @(
    "Assets/MainMenu.cs*", "Assets/Scenes.meta", "Assets/Scenes/Menu.unity*"
)

# 8. levels, one commit each
Commit-Step "Level 1: First Steps (collect all coins)" @("Assets/Scenes/Level1.unity*")
Commit-Step "Level 2: Beat the Clock (reach the goal in time)" @("Assets/Scenes/Level2.unity*")
Commit-Step "Level 3: Crusher Floor (survive the hazards)" @("Assets/Scenes/Level3.unity*")
Commit-Step "Level 4: Guard Duty (avoid the chasing guards)" @("Assets/Scenes/Level4.unity*")
Commit-Step "Level 5: The Vault (collect keys, then escape)" @("Assets/Scenes/Level5.unity*")

# 9. build settings
Commit-Step "Add all scenes to Build Settings" @("ProjectSettings/EditorBuildSettings.asset")

# 10. removed files
Commit-Step "Remove unused level builder and sample scene" $leftovers

# 11. readme
Commit-Step "Add README" @("README.md")

# 12. anything else (tutorial files, etc.) is reported, not committed
Write-Host ""
Write-Host "Left uncommitted (check this list):"
git status --short

Write-Host ""
git log --oneline -15

if ($Push) {
    $branch = (git rev-parse --abbrev-ref HEAD).Trim()
    git push -u origin $branch
} else {
    Write-Host ""
    Write-Host "Not pushed. When you are happy, run:  git push -u origin <your branch>"
}
