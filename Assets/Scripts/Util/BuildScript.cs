using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
namespace CFC.Utils
{
    public class BuildScript
    {
        private const string baseServerBuildPath = "Builds/Server/";
        private const string baseClientBuildPath = "Builds/Client/";
        
        [MenuItem("")]
        public static void BuildAll()
        {
            
        }
        
        #region Windows

        public static void BuildWindowsServer()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            //buildPlayerOptions.scenes = 
            buildPlayerOptions.locationPathName = baseServerBuildPath + "Windows/BattleAcademy_Server.exe";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;
            
            Console.WriteLine($"Building server {BuildTarget.StandaloneWindows64}...");
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Console.WriteLine($"Built server {BuildTarget.StandaloneWindows64} successfully!");
        }
        
        public static void BuildWindowsClient()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            //buildPlayerOptions.scenes = 
            buildPlayerOptions.locationPathName = baseServerBuildPath + "Windows/BattleAcademy_Client.exe";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;
            
            Console.WriteLine($"Building client {BuildTarget.StandaloneWindows64}...");
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Console.WriteLine($"Built client {BuildTarget.StandaloneWindows64} successfully!");
        }

        #endregion
    }

}*/
