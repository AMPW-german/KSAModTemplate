# KSAModTemplate

Template for a [Kitten Space Agency](https://kittenspaceagency.com) (KSA) **code mod**,
built on [KSAModTools](https://github.com/AMPW-german/KSAModTools) (shared MSBuild
config, KSA assembly resolution and CI actions) and
[StarMap](https://github.com/StarMapLoader/StarMap) (the community code-mod loader).

## Quick start — it is enough to download the script and run it

```sh
curl -L https://raw.githubusercontent.com/AMPW-german/KSAModTemplate/main/new-mod.sh -o new-mod.sh
bash new-mod.sh
```

(If the template's default branch is not `main`, adjust the URL.)

The script will ask for:

- **mod name** — becomes the namespace, assembly name, project folder and mod ID
  (letters, digits, underscore)
- **parent directory** — the repo is cloned into `<parent>/<ModName>`
- **author / company / description** — optional metadata written into the
  `.csproj`, `mod.toml` and the generated `README.md`
- **GitHub repo name and visibility** — the script creates the repo with
  `gh repo create` and pushes it, but only after you confirm

Requirements: `git`, `bash` (Git Bash on Windows is fine), and the
[GitHub CLI](https://cli.github.com/) (`gh auth login`) if you want the script to
create the repository for you.

### What the script does

1. Clones this repository (including the `tooling/` KSAModTools submodule) into
   `<parent>/<ModName>` — or, if run inside an existing unrenamed template clone
   (e.g. one made with GitHub's **Use this template** button), reworks that
   checkout in place.
2. Renames `TemplateMod/` → `<ModName>/`, updates the `.slnx`, `.csproj`,
   workflow and all source files, and fills in author/company/description.
3. Replaces the git history with a single fresh `Initial commit` — the result is
   **not a fork** and shares no history with the template.
4. Sets `core.hooksPath` to `tooling/githooks` and, after your confirmation,
   creates the GitHub repo and pushes `main`.

If you answer "no" at the confirmation, everything up to the push is still done —
you can push manually afterwards.

## What's inside

```
TemplateMod/               <- renamed to <ModName>/ by the script
  TemplateMod.csproj       <- imports tooling/msbuild/KsaMod.{props,targets}
  Mod.cs                   <- [StarMapMod] entry point
  Patches.cs               <- example Harmony prefix patch
TemplateMod.slnx           <- solution (renamed to <ModName>.slnx)
tooling/                   <- KSAModTools submodule
.github/workflows/build.yml<- CI (disabled until secrets are configured, see below)
new-mod.sh                 <- the script described above
```

### The sample code

`Mod.cs` is a `[StarMapMod]` entry class that:

- calls `Harmony.PatchAll()` from `[StarMapAllModsLoaded]` (the recommended place
  for Harmony patching), and
- draws a welcome ImGui window from `[StarMapAfterGui]` using
  `Brutal.ImGuiApi.ImGui` — it appears once per session until closed.

`Patches.cs` shows a `[HarmonyPrefix]` on `KSA.VersionInfo.CheckOnLaunch`, which
runs once per game launch, printing `Hello world from <ModName>!` to the KSA
console. Replace it with your own patches.

## Build locally

Requirements:

- .NET 10 SDK
- KSA game files for assembly references — either a local install
  (`C:\Program Files\Kitten Space Agency` by default; override with the `KSA_DIR`
  environment variable or `KSADir` MSBuild property) or a
  [KSADLLs](https://github.com/AMPW-german/KSADLLs)-style snapshot via
  `KSA_DLL_REPO_DIR` with `UseKsaNuGetPackages=true`
- StarMap installed in your KSA mods folder so `StarMap/StarMap.API.dll` exists
  (override the mods folder with `KSA_MODS_DIR`)

```sh
dotnet build -c Release
```

The build packages the mod into `Release/<ModName>/` and copies it into your
local KSA mods folder (`KSAModsDir`). The generated `mod.toml` comes from
`ModName`, `VersionPrefix`, `Authors` and `Description` in the `.csproj`.

## CI setup

`.github/workflows/build.yml` uses the composite actions from the `tooling/`
submodule (`ci/build-package`, `ci/release`). Its automatic triggers are
**commented out** — it only runs via *Actions → build → Run workflow* until you
enable them. To enable pushes/PR builds and releases:

1. Uncomment the `push`/`pull_request` triggers in `on:`.
2. Create a private **KSA DLL snapshot repo** containing the game DLLs,
   `ksa-versions.csv` and `KsaNuGet.props` — KSAModTools'
   [`scripts/update_ksa_dlls.py`](https://github.com/AMPW-german/KSAModTools)
   generates one from a KSA install.
3. Configure repository secrets/variables:

   | Secret / variable | Purpose |
   | --- | --- |
   | `KSADLLRepo` (secret) | `owner/repo` of the DLL snapshot repo |
   | `KSA_DLL_TOKEN` (secret) | PAT with read access to it — simplest option |
   | `APP_ID` + `APP_PRIVATE_KEY` (secrets) | GitHub App alternative to `KSA_DLL_TOKEN` |
   | `SPACEDOCKUSERNAME` (variable) | your SpaceDock username |
   | `SPACEDOCKAUTH` (secret) | your SpaceDock API token |

4. Set `SPACEDOCK_ID` in `build.yml` to your mod's SpaceDock ID (only needed for
   the release job).

On pushes to `main` that change `VersionPrefix`, the release job publishes a
GitHub release and uploads the package to SpaceDock.

## Using the GitHub template button instead

This repository can also be marked as a **template repository** (Settings →
*Template repository*). "Use this template" then produces a repo with the files
but no shared history — but nothing is renamed yet, so you still run `new-mod.sh`
inside the generated clone to do the renaming and push. The script works the same
either way; downloading and running it directly is the simpler path.
