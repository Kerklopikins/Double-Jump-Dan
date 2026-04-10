using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GiveDamage))]
public class GiveDamageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GiveDamage script = (GiveDamage)target;

        script.controlledExternally = EditorGUILayout.Toggle("Controlled Externally", script.controlledExternally);

        if(!script.controlledExternally)
        {
            script.giveDamageTo = (GiveDamage.GiveDamageTo)EditorGUILayout.EnumPopup("Give Damage To", script.giveDamageTo);
            script.instantKill = EditorGUILayout.Toggle("Instant Kill", script.instantKill);

            if(!script.instantKill)
            {
                script.damageToGive = EditorGUILayout.IntField("Damage To Give", script.damageToGive);

                if(script.giveDamageTo == GiveDamage.GiveDamageTo.Player)
                {
                    //script.playerInputDelay = EditorGUILayout.FloatField("Player Input Delay", script.playerInputDelay);
                    script.givePlayerKnockBack = EditorGUILayout.Toggle("Give Player Knock Back", script.givePlayerKnockBack);
                }

                if(script.givePlayerKnockBack)
                {
                    script.rotationBasedKnockBack = EditorGUILayout.Toggle("Rotation Based KnockBack", script.rotationBasedKnockBack);

                    if(!script.rotationBasedKnockBack)
                    {
                        script.xKnockBack = EditorGUILayout.IntField("X Knock Back", script.xKnockBack);
                        script.yKnockBack = EditorGUILayout.IntField("Y Knock Back", script.yKnockBack);
                        script.xKnockBackOffset = EditorGUILayout.FloatField("X Knock Back Offset", script.xKnockBackOffset);
                        script.offsetAdjustmentMode = EditorGUILayout.Toggle("Offset Adjustment Mode", script.offsetAdjustmentMode);
                    }
                    else
                    {
                        script.knockBack = EditorGUILayout.IntField("Knock Back", script.knockBack);
                        script.rotationOffset = EditorGUILayout.FloatField("Rotation Offset", script.rotationOffset);
                    }
                }
            }
        }
        
        if(GUI.changed)
            EditorUtility.SetDirty(script);
    }
}