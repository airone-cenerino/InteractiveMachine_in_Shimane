using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputFieldValueGetter : MonoBehaviour
{
    //InputFieldを格納するための変数
    InputField inputField;


    private void Start()
    {
        //InputFieldコンポーネントを取得
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
    }


    //入力された名前情報を読み取ってコンソールに出力する関数
    public float GetInputValue()
    {
        float value = -1.0f;

        //InputFieldからテキスト情報を取得する
        try
        {
            value = float.Parse(inputField.text);
        }catch(FormatException e)
        {
        }

        return value;
    }
}
