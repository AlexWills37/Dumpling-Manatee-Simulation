using UnityEngine.SceneManagement;

public class EventCallbacks {
    // Whenever the scene changes, we have to add an entry declaring such.
    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        TelemetryManager.entries.Add(
            new TelemetryEntry("sceneLoaded", scene.name)
        );
    }
}