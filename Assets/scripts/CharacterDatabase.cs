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

[CreateAssetMenu(fileName = "NewCaracterData", menuName = "Dialog/Charactor Data")]
public class CharacterData : ScriptableObject
{
    //キャラクター識別用
    public string characterName;
    //表情のリスト
    public List<ExpressionData> expressions;

    //表情名からSpriteを取得
    public Sprite GetSprite(string expression)
    {
        var data = expressions.Find(e => e.expressionName == expression);
        if(data != null)
        {
            return data.sprite;
        }
        //表情が割り当てられていなかった時のログ表示
        Debug.LogWarning($"[{characterName}]の'{expression}'がないお");
        return expressions.Count > 0 ? expressions[0].sprite : null;
    }
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
