using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class VersionCheck : MonoBehaviour
{
    [SerializeField] private GameObject version, state;
    private Text versionText, stateText;

    void Awake()
    {
        versionText = version.GetComponent<Text>();
        stateText = state.GetComponent<Text>();

        versionText.text = GlobalValues.versionText;
        StartCoroutine(CheckVersion());
    }

    public IEnumerator CheckVersion()
    {
        using(UnityWebRequest webRequest = UnityWebRequest.Get("http://130.61.92.46/version.php")) //Odosielam web request na môj veb, ktorý overí kľúč
        {
            webRequest.timeout = 10;
            yield return webRequest.SendWebRequest();

            //if (webRequest.isNetworkError || webRequest.isHttpError)
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                state.SetActive(true);
                stateText.text = "Version check error !";
                stateText.color = Color.red;
            }
            else if(GlobalValues.version  < int.Parse(webRequest.downloadHandler.text)) //Authenticate(true, autoLogin);
            {
                state.SetActive(true);
                stateText.text = "There is a new\nupdate available !";    
                stateText.color = Color.yellow;      
            }
            else
            {
                state.SetActive(true);
                stateText.text = "Up to date !";
                stateText.color = Color.green;
            }
        }
    }
}
