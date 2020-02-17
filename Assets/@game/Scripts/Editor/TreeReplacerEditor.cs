using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TreeReplacer))]
public class TreeReplacerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TreeReplacer myScript = (TreeReplacer) target;
        if (GUILayout.Button("ReplaceTrees"))
        {
            myScript.ChangeTree();
        }
        if (GUILayout.Button("Delte Old"))
        {
            myScript.DeleteOldTrees();
        }
    }
}
