using Events;
using UnityEngine;

namespace SceneManagement.EventImplementations
{
    public class SceneChangedEvent : Event<SceneChangedEvent>
    {
        public SceneId SceneId;
        
        public static SceneChangedEvent Get(SceneId sceneId)
        {
            var evt = GetPooledInternal();
            evt.SceneId = sceneId;
            return evt;
        }
    }
}
