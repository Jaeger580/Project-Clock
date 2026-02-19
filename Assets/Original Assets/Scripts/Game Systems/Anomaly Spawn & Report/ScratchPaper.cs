using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class ScratchPaper{ }

public class AnomalyCentralController : MonoBehaviour
{//controls WHEN and WHERE anomalies spawn (needs access to player position prolly)
    [SerializeField, ReadOnly] private List<AnomalyRoomManager> managers = new();
    public void SubscribeToController(AnomalyRoomManager manager)
    {
        managers.Add(manager);
    }

    private void TriggerAnomalySpawn()
    {
        //randomly pick a room the player isn't in
    }
}

public class AnomalyRoomManager : MonoBehaviour
{//controls WHICH anomaly spawns
    [SerializeField] protected AnomalyDataSet anomaliesInRoom = new();

    public void SpawnAnomaly(List<Tag> tagsToMatch, TagOperator.MatchType matchType = TagOperator.MatchType.ANY)
    {
        var validAnomalies = TagOperator.MatchQuery(tagsToMatch, anomaliesInRoom.items, matchType);

        AnomalyData pickedAnomaly = null;
        List<AnomalyData> validPool = new();

        foreach(var anomaly in validAnomalies)
        {//(re)populate tempPool
            validPool.Add(anomaly);
        }

        #region ROUND 1 : Unseen Valid Anomaly
        while (validPool.Count > 0)
        {//while there's stuff in the pool, try to pull a random anomaly
            var index = Random.Range(0, validPool.Count);

            if (!validPool[index].previouslySeen)
            {//if it hasn't been seen before, pick it
                pickedAnomaly = validPool[index];
                break;
            }
            validPool.RemoveAt(index);
        }

        if (pickedAnomaly != null)
        {//if we picked an anomaly, trigger it
            pickedAnomaly.OnAnomalyTriggered?.Invoke();
            return;
        }
        #endregion
        #region ROUND 2 : Seen Valid Anomaly
        foreach (var anomaly in validAnomalies)
        {//(re)populate tempPool
            validPool.Add(anomaly);
        }
        if(validPool.Count > 0)
        {
            pickedAnomaly = validPool[Random.Range(0, validPool.Count)];
        }

        if (pickedAnomaly != null)
        {//if we picked an anomaly, trigger it
            pickedAnomaly.OnAnomalyTriggered?.Invoke();
            return;
        }
        #endregion
        #region ROUND 3 : Unseen Anomaly in Room
        int checkedCount = 0;
        var roomAnomalies = anomaliesInRoom.items;
        while (pickedAnomaly == null && checkedCount < roomAnomalies.Count)
        {//while I haven't picked an unseen anomaly and haven't iterated through the full list
            checkedCount++;
            int index = Random.Range(0, roomAnomalies.Count);
            if (!roomAnomalies[index].previouslySeen)
                pickedAnomaly = roomAnomalies[index];
        }
        if (pickedAnomaly != null)
        {//if we picked an anomaly, trigger it
            pickedAnomaly.OnAnomalyTriggered?.Invoke();
            return;
        }

        #endregion
        #region ROUND 4 : Any Anomaly in Room

        if (roomAnomalies.Count > 0)
            pickedAnomaly = roomAnomalies[Random.Range(0, roomAnomalies.Count)];

        if (pickedAnomaly != null)
        {//if we picked an anomaly, trigger it
            pickedAnomaly.OnAnomalyTriggered?.Invoke();
            return;
        }
        #endregion

        Debug.LogError($"An anomaly was requested but no anomalies were found. Likely an empty list.", this);
    }
}


/***********************************************************************************************************/
/// <summary>
/// Use this property on a ScriptableObject type to allow the editors drawing the field to draw an expandable
/// area that allows for changing the values on the object without having to change editor.
/// url: https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393/
/// youtube (but has issues, should follow above forum post): https://www.youtube.com/watch?v=mCKeSNdO_S0
/// </summary>
public class ExpandableAttribute : PropertyAttribute
{
    public ExpandableAttribute()
    {

    }
}

public class ReadOnlyAttribute : PropertyAttribute
{

}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

/// <summary>
/// Draws the property field for any field marked with ExpandableAttribute.
/// </summary>
[CustomPropertyDrawer(typeof(ExpandableAttribute), true)]
public class ExpandableAttributeDrawer : PropertyDrawer
{
    // Use the following area to change the style of the expandable ScriptableObject drawers;
    #region Style Setup
    private enum BackgroundStyles
    {
        None,
        HelpBox,
        Darken,
        Lighten
    }

    /// <summary>
    /// Whether the default editor Script field should be shown.
    /// </summary>
    private static bool SHOW_SCRIPT_FIELD = false;

    /// <summary>
    /// The spacing on the inside of the background rect.
    /// </summary>
    private static float INNER_SPACING = 6.0f;

    /// <summary>
    /// The spacing on the outside of the background rect.
    /// </summary>
    private static float OUTER_SPACING = 4.0f;

    /// <summary>
    /// The style the background uses.
    /// </summary>
    private static BackgroundStyles BACKGROUND_STYLE = BackgroundStyles.HelpBox;

    /// <summary>
    /// The colour that is used to darken the background.
    /// </summary>
    private static Color DARKEN_COLOUR = new Color(0.0f, 0.0f, 0.0f, 0.2f);

    /// <summary>
    /// The colour that is used to lighten the background.
    /// </summary>
    private static Color LIGHTEN_COLOUR = new Color(1.0f, 1.0f, 1.0f, 0.2f);
    #endregion

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0.0f;

        totalHeight += EditorGUIUtility.singleLineHeight;

        if (property.objectReferenceValue == null)
            return totalHeight;

        if (!property.isExpanded)
            return totalHeight;

        SerializedObject targetObject = new SerializedObject(property.objectReferenceValue);

        if (targetObject == null)
            return totalHeight;

        SerializedProperty field = targetObject.GetIterator();

        field.NextVisible(true);

        if (SHOW_SCRIPT_FIELD)
        {
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        while (field.NextVisible(false))
        {
            totalHeight += EditorGUI.GetPropertyHeight(field, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        totalHeight += INNER_SPACING * 2;
        totalHeight += OUTER_SPACING * 2;

        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect fieldRect = new Rect(position);
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        Rect textRect = new Rect(fieldRect);
        textRect.xMin += 15;

        EditorGUI.PropertyField(textRect, property, label, true);
        if (property.objectReferenceValue == null)
            return;

        property.isExpanded = EditorGUI.Foldout(fieldRect, property.isExpanded, GUIContent.none, true);

        if (!property.isExpanded)
            return;

        SerializedObject targetObject = new SerializedObject(property.objectReferenceValue);

        if (targetObject == null)
            return;


        #region Format Field Rects
        List<Rect> propertyRects = new List<Rect>();
        Rect marchingRect = new Rect(fieldRect);

        Rect bodyRect = new Rect(fieldRect);
        bodyRect.xMin += EditorGUI.indentLevel * 14;
        bodyRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
            + OUTER_SPACING;

        SerializedProperty field = targetObject.GetIterator();
        field.NextVisible(true);

        marchingRect.y += INNER_SPACING + OUTER_SPACING;

        if (SHOW_SCRIPT_FIELD)
        {
            propertyRects.Add(marchingRect);
            marchingRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        while (field.NextVisible(false))
        {
            marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
            marchingRect.height = EditorGUI.GetPropertyHeight(field, true);
            propertyRects.Add(marchingRect);
        }

        marchingRect.y += INNER_SPACING;

        bodyRect.yMax = marchingRect.yMax;
        #endregion

        DrawBackground(bodyRect);

        #region Draw Fields
        EditorGUI.indentLevel++;

        int index = 0;
        field = targetObject.GetIterator();
        field.NextVisible(true);

        if (SHOW_SCRIPT_FIELD)
        {
            //Show the disabled script field
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(propertyRects[index], field, true);
            EditorGUI.EndDisabledGroup();
            index++;
        }

        //Replacement for "editor.OnInspectorGUI ();" so we have more control on how we draw the editor
        while (field.NextVisible(false))
        {
            try
            {
                EditorGUI.PropertyField(propertyRects[index], field, true);
            }
            catch (StackOverflowException)
            {
                field.objectReferenceValue = null;
                Debug.LogError("Detected self-nesting cauisng a StackOverflowException, avoid using the same " +
                    "object iside a nested structure.");
            }

            index++;
        }

        targetObject.ApplyModifiedProperties();

        EditorGUI.indentLevel--;
        #endregion
    }

    /// <summary>
    /// Draws the Background
    /// </summary>
    /// <param name="rect">The Rect where the background is drawn.</param>
    private void DrawBackground(Rect rect)
    {
        switch (BACKGROUND_STYLE)
        {

            case BackgroundStyles.HelpBox:
                EditorGUI.HelpBox(rect, "", MessageType.None);
                break;

            case BackgroundStyles.Darken:
                EditorGUI.DrawRect(rect, DARKEN_COLOUR);
                break;

            case BackgroundStyles.Lighten:
                EditorGUI.DrawRect(rect, LIGHTEN_COLOUR);
                break;
        }
    }
}

#endif