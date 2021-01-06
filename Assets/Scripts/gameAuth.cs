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

        using(UnityWebRequest webRequest = UnityWebRequest.Get("http://13.49.138.187/gameAuth.php?gameKey=" + Key)) //Odosielam web request na môj veb, ktorý overí kľúč
        {
            yield return webRequest.SendWebRequest();

            if(webRequest.downloadHandler.text == "true") //Authenticate(true, autoLogin);
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
