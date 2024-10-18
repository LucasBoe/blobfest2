using System.Collections;
using UnityEngine;

namespace Engine
{
    public interface IDelayedStartObserver
    {
        void DelayedStart();
    }

    public class DelayedStart_BootStep : MonoBehaviour, ISceneChangeTask
    {
        public IEnumerator Execute(SceneChangeContext _context)
        {
            yield return null;

            foreach (var observer in Util.FindAllThatImplement<IDelayedStartObserver>())
                observer.DelayedStart();
        }
    }
}