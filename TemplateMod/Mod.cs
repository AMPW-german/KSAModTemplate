using Brutal.ImGuiApi;
using HarmonyLib;
using StarMap.API;

namespace TemplateMod;

[StarMapMod]
public class TemplateMod
{
	private bool _showWelcome = true;

	[StarMapAllModsLoaded]
	public void AllModsLoaded()
	{
		new Harmony("TemplateMod").PatchAll();
	}

	[StarMapAfterGui]
	public void AfterGui(double dt)
	{
		if (!_showWelcome)
			return;

		if (ImGui.Begin("Welcome to TemplateMod", ref _showWelcome, ImGuiWindowFlags.NoSavedSettings))
		{
			ImGui.Text("Hello from TemplateMod!");
			ImGui.TextDisabled("This window is drawn from [StarMapAfterGui]. Close it to dismiss.");
			ImGui.End();
		}
	}
}
