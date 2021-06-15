using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitScript))]
public class CustomUnitInspector : Editor
{
    private UnitScript us;

    private void OnEnable()
    {
        us = (UnitScript) target;
    }

    public override void OnInspectorGUI()
    {
        us.macrophageSprite = UnitTableCell("Макрофаг", UnitSpecies.Macrophage, us.macrophageSprite);
        us.dendriticCellSprite = UnitTableCell("Дендритная клетка", UnitSpecies.DendriticCell, us.dendriticCellSprite);
        us.neutrophilSprite = UnitTableCell("Нейтрофил", UnitSpecies.Neutrophil, us.neutrophilSprite);
        us.nKCellSprite = UnitTableCell("НК-клетка", UnitSpecies.NkCell, us.nKCellSprite);
        us.tKillerSprite = UnitTableCell("Т-Киллер", UnitSpecies.Killer, us.tKillerSprite);
    }

    private Sprite UnitTableCell(string label, UnitSpecies species, Sprite sprite)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        var newSprite = (Sprite) EditorGUILayout.ObjectField("Спрайт", sprite, typeof(Sprite), false);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Урон");
        UnitScript.UnitsCharacteristics[species].LifeDamage = EditorGUILayout.IntField(UnitScript.UnitsCharacteristics[species].LifeDamage);
        GUILayout.Label("Сбор");
        UnitScript.UnitsCharacteristics[species].AntiBodyDamage = EditorGUILayout.IntField(UnitScript.UnitsCharacteristics[species].AntiBodyDamage);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Скорость");
        UnitScript.UnitsCharacteristics[species].Speed = EditorGUILayout.IntField(UnitScript.UnitsCharacteristics[species].Speed);
        GUILayout.Label("Цена");
        UnitScript.UnitsCharacteristics[species].Cost = EditorGUILayout.IntField(UnitScript.UnitsCharacteristics[species].Cost);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        return newSprite;
    }
}