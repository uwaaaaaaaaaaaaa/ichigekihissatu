using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[Serializable]
public class ExpressionData
{
    public string expressionName;
    public Sprite sprite;
}



[CreateAssetMenu(fileName ="CharacterDatabase", menuName = "Dialog/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> characters;

    //キャラクター名からCharacterDataを取得

    public CharacterData GetCharacter(string characterName)
    {
        var character = characters.Find(c => c.characterName == characterName);
        if(character == null)
        {
            Debug.LogError($"'{character}'がそもそもないお");
        }
        return character;
    }
}
