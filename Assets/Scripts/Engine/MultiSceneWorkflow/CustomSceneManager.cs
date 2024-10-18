using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Engine
{

    public interface ISceneLoadFinishedObserver
    {
        void OnSceneLoadFinished();
    }

    public interface ISceneLoadStartedObserver
    {
        void OnSceneLoadStarted();
    }

    public interface ISceneChangeTask
    {
        public IEnumerator Execute(SceneChangeContext _context);
    }

    public class SceneChangeArgs
    {
        public bool Load;
    }

    public class SceneChangeContext
    {
        public bool ToPlayScene;
        public bool Load;
        public bool InitialBoot;

        private StringBuilder log = new StringBuilder();
        public void Log(string _msg)
        {
            if (log == null)
                log = new StringBuilder();

            log.AppendLine(_msg);
        }

        public void DumpLog()
        {
            if (log == null)
                log = new StringBuilder();

            Debug.Log(log.ToString());
        }
    }

    [SingletonSettings(SingletonLifetime.Persistant, _canBeGenerated: true)]
    public class CustomSceneManager : SingletonBehaviour<CustomSceneManager>
    {
        Coroutine activeChangeSceneRoutine;
        private SceneContextDefinition activeSceneContext;
        private SceneContextDefinition previousSceneContext;

        public bool InPlayScene => activeSceneContext != null ? activeSceneContext.IsPlayingScene : false;

        public bool IsChangingScene => activeChangeSceneRoutine != null;


        public void ChangeScene(SceneContextDefinition _scene, SceneChangeArgs _args)
        {
            if (activeChangeSceneRoutine != null)
            {
                Debug.LogError("Error: Cannot start new scene change, previous scene change is still running.");
                return;
            }

            previousSceneContext = activeSceneContext;
            activeSceneContext = _scene;
            SceneChangeContext context = CreateSceneChangeContext(_scene, _args);
            activeChangeSceneRoutine = StartCoroutine(ChangeSceneRoutine(_scene, context));
        }

        private SceneChangeContext CreateSceneChangeContext(SceneContextDefinition _scene, SceneChangeArgs _args)
        {
            SceneChangeContext context = new SceneChangeContext();
            context.ToPlayScene = _scene.IsPlayingScene;
            context.Load = _args.Load;
            context.InitialBoot = previousSceneContext == null;

            return context;
        }

        private IEnumerator ChangeSceneRoutine(SceneContextDefinition to, SceneChangeContext _context)
        {
            string prevName = previousSceneContext == null ? "none" : previousSceneContext.name;
            Debug.Log($"<b>CustomSceneManager: Started Scene change from {prevName} to {to.name}.</b>");
            _context.Log($"{Time.time.ToMMSS()} | <b>CustomSceneManager: Scene change completed. Expand for Details. </b>");

            float startTime = Time.unscaledTime;
            _context.Log($"startet at {startTime}");

            List<ISceneChangeTask> tasks = GenerateSceneChangeTasks(to);

            //hack to fix timescale 0 after scene change
            Time.timeScale = 1;

            int taskCount = tasks.Count;
            int taskFinishedCount = 0;

            foreach (var task in tasks)
            {
                //FullscreenFade.OnTryChangeLoadingPercentEvent?.Trigger((float)taskFinishedCount / taskCount);

                float startTimestamp = Time.unscaledTime;
                yield return task.Execute(_context);
                float finishTimestamp = Time.unscaledTime;

                _context.Log($"Finished {task.ToString().Split(".").Last()}. Took {finishTimestamp - startTimestamp} seconds.");
                taskFinishedCount++;
            }

            _context.Log($"Finished at {Time.unscaledTime}. Took {Time.unscaledTime - startTime} seconds.");
            _context.DumpLog();
            activeChangeSceneRoutine = null;
        }

        private List<ISceneChangeTask> GenerateSceneChangeTasks(SceneContextDefinition _to)
        {
            List<ISceneChangeTask> tasks = new List<ISceneChangeTask>();

            tasks.Add(new SceneChangeTaskFireStartedEvent());

            tasks.Add(new SceneChangeTaskKillAllTweens());

            AddSceneLoadingTasks(_to, ref tasks);
            AddPrefabTasks(_to, ref tasks);

            tasks.Add(new SceneChangeTaskFireFinishedEvent());

            return tasks;
        }

        private static void AddPrefabTasks(SceneContextDefinition _to, ref List<ISceneChangeTask> tasks)
        {
            Debug.Log("AddPrefabTasks");

            var bootStepsPrefab = _to.GetBootStepsPrefab();

            if (bootStepsPrefab != null)
            {
                var bootStepsObject = GameObject.Instantiate(bootStepsPrefab);
                GameObject.DontDestroyOnLoad(bootStepsObject);
                ISceneChangeTask[] bootSteps = bootStepsObject.GetComponentsInChildren<ISceneChangeTask>();

                Debug.Log($"boot step count {bootSteps.Count()}");

                for (int i = 0; i < bootSteps.Length; i++)
                    tasks.Add(bootSteps[i]);

                tasks.Add(new SceneChangeTaskDestroyGameObject(bootStepsObject));
            }
        }

        private void AddSceneLoadingTasks(SceneContextDefinition _to, ref List<ISceneChangeTask> tasks)
        {
            int[] fromBIA = GetAllActiveScenesBuildIndexArray();
            int[] toBIA = _to.GetScenesForBooting().Select(sr => sr.BuildIndex).Where(i => i >= 0).ToArray();

            if (fromBIA.Length > 0)
            {
                foreach (int unloadable in fromBIA)
                {
                    if (ShouldUnloadScene(unloadable))
                        tasks.Add(new SceneChangeTaskUnloadScene(unloadable));
                }
            }

            //Clear and prep singletons after unload but before load, to ensure eager singletons are ready by the time Awake is called
            tasks.Add(new SceneChangeTaskClearAndPrepareSingletons());

            foreach (int loadable in toBIA)
            {
                if (ShouldLoadScene(fromBIA, loadable))
                    tasks.Add(new SceneChangeTaskLoadScene(loadable));
            }

            if (_to != null && _to.Scenes.Count > 0)
                tasks.Add(new SceneChangeTaskSetSceneActive(toBIA[0]));
        }

        private bool ShouldUnloadScene(int idx)
        {
            return !IsGlobalOrBootScene(idx);
        }

        private static bool IsGlobalOrBootScene(int indexToLoad)
        {
            if (indexToLoad < 0)
                return false;
            var path = SceneManager.GetSceneByBuildIndex(indexToLoad).path;

            if (path == null)
                return false;

            return SceneUtil.IsGlobalOrBootScene(path);
        }

        private bool ShouldLoadScene(int[] loadedId, int indexToLoad)
        {
            //not global or global but not loaded
            return !IsGlobalOrBootScene(indexToLoad) || !loadedId.Contains(indexToLoad);
        }

        private int[] GetAllActiveScenesBuildIndexArray()
        {
            int[] active = new int[SceneManager.sceneCount];

            for (int i = 0; i < active.Length; i++)
                active[i] = SceneManager.GetSceneAt(i).buildIndex;

            return active;
        }
    }
    public class SceneUtil
    {
        public static bool IsGlobalOrBootScene(string path)
        {
            return path.Contains("GLOBAL") || path.Contains("BOOT");
        }
    }
}
