using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DetectManager : MonoBehaviour
{
    [SerializeField] StatusManager statusManager;

    // Start is called before the first frame update
    void Start()
    {
        // �J�����̖��O�m�F�p�B
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices)
        {
            Debug.Log(device.name);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
