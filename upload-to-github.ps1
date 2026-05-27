# upload-to-github.ps1
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# We include the username in the remote URL to force Git Credential Manager to prompt for the correct account
$repoUrl = "https://yaong832@github.com/yaong832/SemiconductorUi.git"

Write-Host "Starting Git initialization and upload..." -ForegroundColor Cyan

# 1. Initialize git if not already initialized
if (-not (Test-Path ".git")) {
    Write-Host "Initializing local Git repository..." -ForegroundColor Yellow
    git init
} else {
    Write-Host "Git repository already initialized." -ForegroundColor Yellow
}

# 2. Configure local Git user settings (using your GitHub username)
Write-Host "Configuring local Git credentials..." -ForegroundColor Yellow
git config user.name "yaong832"
git config user.email "yu16891@gmail.com"

# 3. Add files and commit
Write-Host "Staging files to Git..." -ForegroundColor Yellow
git add .

# Try to commit. If there are no changes, it will fail silently, which is fine.
Write-Host "Creating initial commit..." -ForegroundColor Yellow
git commit -m "Initial commit of Semiconductor UI Control System (EtherCAT / WinForms)" -q

# 4. Set branch to main
Write-Host "Setting main branch..." -ForegroundColor Yellow
git branch -M main

# 5. Configure remote repository URL with explicit username yaong832
Write-Host "Linking to remote repository: $repoUrl..." -ForegroundColor Yellow
$remotes = git remote
if ($remotes -contains "origin") {
    git remote set-url origin $repoUrl
} else {
    git remote add origin $repoUrl
}

# 6. Push to GitHub
Write-Host "Pushing to GitHub (yaong832/SemiconductorUi)..." -ForegroundColor Yellow
Write-Host "NOTE: A login popup may appear. Please log in with your GitHub account: yaong832" -ForegroundColor Green

try {
    git push -u origin main
    Write-Host "Successfully uploaded project to GitHub!" -ForegroundColor Green
} catch {
    Write-Host "An error occurred during push. Please check your network and GitHub authentication." -ForegroundColor Red
    Write-Error $_
}
