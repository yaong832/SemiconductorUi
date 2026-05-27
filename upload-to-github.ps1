# upload-to-github.ps1
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$repoUrl = "https://github.com/yaong832/SemiconductorUi.git"

Write-Host "Starting Git initialization and upload..." -ForegroundColor Cyan

# 1. Initialize git if not already initialized
if (-not (Test-Path ".git")) {
    Write-Host "Initializing local Git repository..." -ForegroundColor Yellow
    git init
} else {
    Write-Host "Git repository already initialized." -ForegroundColor Yellow
}

# 2. Configure local Git user settings (using your global settings)
Write-Host "Configuring local Git credentials..." -ForegroundColor Yellow
git config user.name "hyunho0728"
git config user.email "yu16891@gmail.com"

# 3. Add files and commit
Write-Host "Staging files to Git..." -ForegroundColor Yellow
git add .

Write-Host "Creating initial commit..." -ForegroundColor Yellow
git commit -m "Initial commit of Semiconductor UI Control System (EtherCAT / WinForms)"

# 4. Set branch to main
Write-Host "Setting main branch..." -ForegroundColor Yellow
git branch -M main

# 5. Configure remote repository URL
Write-Host "Linking to remote repository: $repoUrl..." -ForegroundColor Yellow
$remotes = git remote
if ($remotes -contains "origin") {
    git remote set-url origin $repoUrl
} else {
    git remote add origin $repoUrl
}

# 6. Push to GitHub
Write-Host "Pushing to GitHub (yaong832/SemiconductorUi)..." -ForegroundColor Yellow
try {
    # We will attempt to push. If Git Credential Manager is set up, it will push automatically.
    git push -u origin main
    Write-Host "Successfully uploaded project to GitHub!" -ForegroundColor Green
} catch {
    Write-Host "An error occurred during push. Please check your network and GitHub authentication." -ForegroundColor Red
    Write-Error $_
}
