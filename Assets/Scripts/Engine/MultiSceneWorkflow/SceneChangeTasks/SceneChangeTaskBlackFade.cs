using System.Collections;
using UnityEngine;

namespace Engine
{
    public class SceneChangeTaskFireFinishedEvent : ISceneChangeTask
    {
        public IEnumerator Execute(SceneChangeContext _context)
        {
            foreach (var observer in Util.FindAllThatImplement<ISceneLoadFinishedObserver>())
                observer.OnSceneLoadFinished();

            yield break;
        }
    }

    public class SceneChangeTaskFireStartedEvent : ISceneChangeTask
    {
        public IEnumerator Execute(SceneChangeContext _context)
        {
            foreach (var observer in Util.FindAllThatImplement<ISceneLoadStartedObserver>())
                observer.OnSceneLoadStarted();

            yield break;
        }
    }

    public class SceneChangeTaskClearAndPrepareSingletons : ISceneChangeTask
    {
        public IEnumerator Execute(SceneChangeContext _context)
        {
            if (!_context.InitialBoot)
            {
                int destroyed = SingletonManager.Instance.DisposeOfSceneSingletons();
                _context.Log($"Destroyed {destroyed} scene singletons.");
            }

            int spawned = SingletonManager.Instance.SpawnEagerSingletons(_context.ToPlayScene);
            _context.Log($"Spawned {spawned} eager singletons.");

            yield break;
        }
    }
    public class SceneChangeTaskKillAllTweens : ISceneChangeTask
    {
        public IEnumerator Execute(SceneChangeContext _context)
        {
            //DOTween.Clear(destroy: true);
            yield break;
        }
    }
}
