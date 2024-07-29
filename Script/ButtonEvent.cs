using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    
    public void SceenLoader(string sceenName)
    {
        SceneManager.LoadScene(sceenName);
    }
   

}
