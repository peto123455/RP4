using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameAuth : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject main, panel, check;
    [SerializeField] private Text errorMessage;

    void Start()
    {
        IsAlreadyAuthenticated();
    }

    public void AuthButton() => RequestAuth(inputField.text, false); //Unity tlačidlo dokáže zavolať funkciu s max 1 parametrom, takže to takto riešim

    private void RequestAuth(string Key, bool autoLogin = false) => StartCoroutine(RequestAuthEnumerator(Key, autoLogin));

    public IEnumerator RequestAuthEnumerator(string Key, bool autoLogin)
    {

        SwitchToCheck(true);

        using(UnityWebRequest webRequest = UnityWebRequest.Get("http://130.61.92.46/gameAuth.php?gameKey=" + Key)) //Odosielam web request na môj veb, ktorý overí kľúč
        {
            webRequest.timeout = 60;
            yield return webRequest.SendWebRequest();

            //if ((webRequest.isNetworkError || webRequest.isHttpError) && Key != "override")
            if ((webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) && Key != "override")
            {
                errorMessage.text = "Oops, something went wrong !";
            }
            else if(webRequest.downloadHandler.text == "true" || Key == "override") //Kvôli sočke tu pridávam možnosť obísť ochranu, ak by som nemal internet
            {
                if(!autoLogin)PlayerPrefs.SetString("key", inputField.text);
                main.SetActive(true);
                gameObject.SetActive(false);
                gameObject.transform.parent.gameObject.GetComponent<MainMenu>().KeyChecked();
            }
            else errorMessage.text = webRequest.downloadHandler.text; //Authenticate(false, autoLogin, webRequest.downloadHandler.text);

            SwitchToCheck(false);
        }
    }

    public void IsAlreadyAuthenticated()
    {
        if (PlayerPrefs.GetString("key", "empty") == "empty") return;
        RequestAuth(PlayerPrefs.GetString("key", "empty"), true);
    }

    private void SwitchToCheck(bool checking)
    {
        panel.SetActive(!checking);
        check.SetActive(checking);
    }
}
