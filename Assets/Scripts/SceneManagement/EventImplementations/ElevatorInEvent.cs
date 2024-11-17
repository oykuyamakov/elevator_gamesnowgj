using Events;

namespace SceneManagement.EventImplementations
{
    public class ElevatorInEvent : Event<ElevatorInEvent>
    {
        public SceneId nexSceneId;
        
        public static ElevatorInEvent Get(SceneId sceneId)
        {
            var evt = GetPooledInternal();
            evt.nexSceneId = sceneId;
            return evt;
        }
    }
}