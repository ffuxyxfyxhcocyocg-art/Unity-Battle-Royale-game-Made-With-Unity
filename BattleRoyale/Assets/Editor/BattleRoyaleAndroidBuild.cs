using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BattleRoyaleAndroidBuild
{
    // Keep this identifier unchanged after the first public APK.
    private const string PackageId = "com.ffuxyxfyxhcocyocg.battleroyale";

    public static void PerformBuild()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, PackageId);
        PlayerSettings.productName = "Battle Royale";
        PlayerSettings.companyName = "FFUXYXFYXHCocyocg";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;

        string version = Environment.GetEnvironmentVariable("GAME_VERSION");
        if (String.IsNullOrEmpty(version)) version = "0.1." + (Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "1");
        PlayerSettings.bundleVersion = version;

        int versionCode = 1;
        Int32.TryParse(Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER"), out versionCode);
        if (versionCode < 1) versionCode = 1;
        PlayerSettings.Android.bundleVersionCode = versionCode;

        string keystore = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PATH");
        string keystorePass = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASS");
        string aliasName = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_NAME");
        string aliasPass = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASS");
        if (String.IsNullOrEmpty(keystore) || !File.Exists(keystore) ||
            String.IsNullOrEmpty(keystorePass) || String.IsNullOrEmpty(aliasName) ||
            String.IsNullOrEmpty(aliasPass))
            throw new Exception("Missing Android signing secrets. Configure ANDROID_KEYSTORE_BASE64, ANDROID_KEYSTORE_PASS, ANDROID_KEYALIAS_NAME and ANDROID_KEYALIAS_PASS in GitHub Actions secrets.");

        PlayerSettings.Android.keystoreName = keystore;
        PlayerSettings.Android.keystorePass = keystorePass;
        PlayerSettings.Android.keyaliasName = aliasName;
        PlayerSettings.Android.keyaliasPass = aliasPass;

        var scenes = Array.ConvertAll(
            Array.FindAll(EditorBuildSettings.scenes, scene => scene.enabled),
            scene => scene.path
        );
        if (scenes.Length == 0)
            throw new Exception("No enabled scenes found in File > Build Settings.");

        string output = Path.GetFullPath("build/Android/BattleRoyale.apk");
        Directory.CreateDirectory(Path.GetDirectoryName(output));

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = output,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Android build failed: " + report.summary.result + " (" + report.summary.totalErrors + " errors).");

        Debug.Log("APK generated: " + output + " | version " + version + " | versionCode " + versionCode);
    }
}
