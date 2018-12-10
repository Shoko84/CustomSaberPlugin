﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace CustomSaber
{
    public class Plugin : IPlugin
    {
        public string Name
        {
            get { return "Saber Mod"; }
        }

        public string Version
        {
            get { return "2.5"; }
        }

        private static List<string> _saberPaths;
        private static AssetBundle _currentSaber;
        public static string _currentSaberPath;
        public static Saber RightSaber;
        public static Saber LeftSaber;

        private bool _init;

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            var sabers = RetrieveCustomSabers();
            if (sabers.Count == 0)
            {
                Console.WriteLine("No custom sabers found.");
                return;
            }

            _currentSaberPath = PlayerPrefs.GetString("lastSaber", null);
            if (_currentSaberPath == null || !sabers.Contains(_currentSaberPath))
            {
                _currentSaberPath = sabers[0];
            }
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }
        
        bool doeet = true;
        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if(scene.buildIndex > 0)
                if(doeet) SharedCoroutineStarter.instance.StartCoroutine(SaberTest());
            if (scene.name == "GameCore")
            {
                LoadNewSaber(_currentSaberPath);
                SaberScript.LoadAssets();
            }

            if (scene.name == "Menu")
            {
                if (_currentSaber != null)
                {
                    _currentSaber.Unload(true);
                }
                CustomSaberUI.OnLoad();
            }
        }

        private IEnumerator SaberTest()
        {
            doeet = false;
            Application.LoadLevelAdditiveAsync("GameCore");
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Count() > 1);
            foreach (Saber s in Resources.FindObjectsOfTypeAll<Saber>()) {
                Console.WriteLine($"Saber: {s.name}, GameObj: {s.gameObject.name}, {s.ToString()}");
                if (s.name == "RightSaber") RightSaber = Saber.Instantiate(s);
                else if (s.name == "LeftSaber") LeftSaber = Saber.Instantiate(s);
            }
            if (RightSaber) UnityEngine.Object.DontDestroyOnLoad(RightSaber.gameObject);
            if (LeftSaber) UnityEngine.Object.DontDestroyOnLoad(LeftSaber.gameObject);
            SceneManager.UnloadSceneAsync("GameCore");
        }

        public static List<string> RetrieveCustomSabers()
        {
            _saberPaths = (Directory.GetFiles(Path.Combine(Application.dataPath, "../CustomSabers/"),
                "*.saber", SearchOption.AllDirectories).ToList());
            Console.WriteLine("Found " + _saberPaths.Count + " sabers");
            _saberPaths.Insert(0, "DefaultSabers");
            return _saberPaths;
        }

        public void OnUpdate()
        {
            if (_currentSaber == null) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RetrieveCustomSabers();
                if (_saberPaths.Count == 1) return;
                var oldIndex = _saberPaths.IndexOf(_currentSaberPath);
                if (oldIndex >= _saberPaths.Count - 1)
                {
                    oldIndex = -1;
                }

                var newSaber = _saberPaths[oldIndex + 1];
                LoadNewSaber(newSaber);
                if (SceneManager.GetActiveScene().buildIndex != 4) return;
                SaberScript.LoadAssets();
            }
            else if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftAlt))
            {
                RetrieveCustomSabers();
                if (_saberPaths.Count == 1) return;
                var oldIndex = _saberPaths.IndexOf(_currentSaberPath);
                if (oldIndex <= 0)
                {
                    oldIndex = _saberPaths.Count - 1;
                }

                var newSaber = _saberPaths[oldIndex - 1];
                LoadNewSaber(newSaber);
                if (SceneManager.GetActiveScene().buildIndex != 4) return;
                SaberScript.LoadAssets();
            }
        }

        public static void LoadNewSaber(string path)
        {
            if (_currentSaber != null)
            {
                _currentSaber.Unload(true);
            }

            if (path != "DefaultSabers")
            {
                _currentSaberPath = path;

                _currentSaber =
                    AssetBundle.LoadFromFile(_currentSaberPath);
                Console.WriteLine(_currentSaber.GetAllAssetNames()[0]);
                if (_currentSaber == null)
                {
                    Console.WriteLine("something went wrong getting the asset bundle");
                }
                else
                {
                    Console.WriteLine("Succesfully obtained the asset bundle!");
                    SaberScript.CustomSaber = _currentSaber;
                }
            }

            PlayerPrefs.SetString("lastSaber", _currentSaberPath);
            Console.WriteLine($"Loaded saber {path}");
        }

        public void OnFixedUpdate()
        {

        }

        public void OnLevelWasInitialized(int level)
        {


        }

        public void OnLevelWasLoaded(int level)
        {

        }
    }
}
