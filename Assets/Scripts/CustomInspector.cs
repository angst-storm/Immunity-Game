using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(GameController))]
public class CustomInspector : Editor
{
    private GameController gc;

    private void OnEnable()
    {
        gc = (GameController) target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Белок", EditorStyles.boldLabel);
        gc.proteinCountText = (Text) EditorGUILayout.ObjectField("Текст", gc.proteinCountText, typeof(Text), true);
        gc.startProteinCount = EditorGUILayout.IntField("Кол-во на старте", gc.startProteinCount);
        gc.timeToProteinIncrement = EditorGUILayout.IntField("Промежуток появления", gc.timeToProteinIncrement);
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Температура", EditorStyles.boldLabel);
        gc.temperatureText = (Text) EditorGUILayout.ObjectField("Текст", gc.temperatureText, typeof(Text), true);
        gc.gameOverTemperature = EditorGUILayout.DoubleField("Крит. температура", gc.gameOverTemperature);
        gc.temperatureIncrement = EditorGUILayout.DoubleField("Штраф", gc.temperatureIncrement);
        gc.temperatureDecrement = EditorGUILayout.DoubleField("Знач. падения", gc.temperatureDecrement);
        gc.timeToTemperatureDecrement =
            EditorGUILayout.IntField("Промежуток падения", gc.timeToTemperatureDecrement);
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Угрозы", EditorStyles.boldLabel);
        GUILayout.Label("Префаб угрозы");
        gc.threatPrefab = (GameObject) EditorGUILayout.ObjectField("", gc.threatPrefab, typeof(GameObject), false);
        GUILayout.Space(10);

        GUILayout.Label("Кривая возрастания сложности");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("y = ");
        gc.difficultyK = EditorGUILayout.DoubleField(gc.difficultyK);
        GUILayout.Label(" * x ^ (");
        gc.difficultyA = EditorGUILayout.DoubleField(gc.difficultyA);
        GUILayout.Label(") + ");
        gc.difficultyM = EditorGUILayout.DoubleField(gc.difficultyM);
        EditorGUILayout.EndHorizontal();
        gc.startThreatDifficult = EditorGUILayout.IntField("Начальное x", gc.startThreatDifficult);
        GUILayout.Space(10);

        GUILayout.Label("Кривая падения промежутка появления");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("y = ");
        gc.spawnTimeK = EditorGUILayout.DoubleField(gc.spawnTimeK);
        GUILayout.Label(" * x ^ (");
        gc.spawnTimeA = EditorGUILayout.DoubleField(gc.spawnTimeA);
        GUILayout.Label(") + ");
        gc.spawnTimeM = EditorGUILayout.DoubleField(gc.spawnTimeM);
        EditorGUILayout.EndHorizontal();
        gc.startTimeToThreatSpawn = EditorGUILayout.IntField("Начальное x", gc.startTimeToThreatSpawn);
        GUILayout.Space(10);

        gc.maxThreatsCount = EditorGUILayout.IntField("Макс. кол-во", gc.maxThreatsCount);
        gc.pointsForDestruction = EditorGUILayout.IntField("Очки за победу", gc.pointsForDestruction);
        gc.threatWin =
            (AudioSource) EditorGUILayout.ObjectField("Звук победы", gc.threatWin, typeof(AudioSource), true);
        gc.threatDefeat =
            (AudioSource) EditorGUILayout.ObjectField("Звук поражения", gc.threatDefeat, typeof(AudioSource), true);
        GUILayout.Space(10);

        gc.lymphnode = (GameObject) EditorGUILayout.ObjectField("Лимфоузел", gc.lymphnode, typeof(GameObject), true);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Сюжетный режим");
        gc.plotMode = GUILayout.Toggle(gc.plotMode, "");
        EditorGUILayout.EndHorizontal();
        gc.plotController =
            (PlotController) EditorGUILayout.ObjectField("Контроллер сюжета", gc.plotController, typeof(PlotController),
                true);
        gc.uiManager =
            (UIManagerScript) EditorGUILayout.ObjectField("Переключатель сцен", gc.uiManager, typeof(UIManagerScript),
                true);
    }
}