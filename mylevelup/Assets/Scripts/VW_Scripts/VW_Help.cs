using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VW_Help : MonoBehaviour
{
    // UniWebView
    public UniWebView webView;

    [SerializeField] RectTransform webviewTransform;
    [SerializeField] GameObject selectFromLeftText;
    [SerializeField] GameObject loadingPanel;

    // button links
    private string gettingStarted = "https://mylevelup.app/help";
    private string words = "https://mylevelup.app/words";
    private string rewards = "https://mylevelup.app/rewards";
    private string settings = "https://mylevelup.app/settings";
    private string stats = "https://mylevelup.app/stats";
    private string contact = "https://mylevelup.app/contact";
    private string playlist = "https://mylevelup.app/playlist";


    void Start()
    {
        Debug.Log("Start Help Scene");
        CreateWebview();
        selectFromLeftText.SetActive(true);
    }

    public void OnGettingStartedButton()
    {
        loadingPanel.SetActive(true);
        selectFromLeftText.SetActive(false);
        webView.Load(gettingStarted);
        webView.Hide(true, UniWebViewTransitionEdge.Right);
        webView.OnPageFinished += (webView, statusCode, url) => {
            webView.Show(true, UniWebViewTransitionEdge.Right);
            loadingPanel.SetActive(false);
        };
    }

    public void OnWordsButton()
    {
        loadingPanel.SetActive(true);
        selectFromLeftText.SetActive(false);
        webView.Load(words);
        webView.Hide(true, UniWebViewTransitionEdge.Right);
        webView.OnPageFinished += (webView, statusCode, url) => {
            webView.Show(true, UniWebViewTransitionEdge.Right);
            loadingPanel.SetActive(false);
        };
    }

    public void OnRewardsButton()
    {
        loadingPanel.SetActive(true);
        selectFromLeftText.SetActive(false);
        webView.Load(rewards);
        webView.Hide(true, UniWebViewTransitionEdge.Right);
        webView.OnPageFinished += (webView, statusCode, url) => {
            webView.Show(true, UniWebViewTransitionEdge.Right);
            loadingPanel.SetActive(false);
        };
    }

    public void OnSettingsButton()
    {
        loadingPanel.SetActive(true);
        selectFromLeftText.SetActive(false);
        webView.Load(settings);
        webView.Hide(true, UniWebViewTransitionEdge.Right);
        webView.OnPageFinished += (webView, statusCode, url) => {
            webView.Show(true, UniWebViewTransitionEdge.Right);
            loadingPanel.SetActive(false);
        };
    }

    public void OnStatsButton()
    {
        loadingPanel.SetActive(true);
        selectFromLeftText.SetActive(false);
        webView.Load(stats);
        webView.Hide(true, UniWebViewTransitionEdge.Right);
        webView.OnPageFinished += (webView, statusCode, url) => {
            webView.Show(true, UniWebViewTransitionEdge.Right);
            loadingPanel.SetActive(false);
        };
    }

    public void OnContactUsButton()
    {
        loadingPanel.SetActive(true);
        selectFromLeftText.SetActive(false);
        webView.Load(contact);
        webView.Hide(true, UniWebViewTransitionEdge.Right);
        webView.OnPageFinished += (webView, statusCode, url) => {
            webView.Show(true, UniWebViewTransitionEdge.Right);
            loadingPanel.SetActive(false);
        };
    }

    public void OnPlaylistButton()
    {
        loadingPanel.SetActive(true);
        selectFromLeftText.SetActive(false);
        webView.Load(playlist);
        webView.Hide(true, UniWebViewTransitionEdge.Right);
        webView.OnPageFinished += (webView, statusCode, url) => {
            webView.Show(true, UniWebViewTransitionEdge.Right);
            loadingPanel.SetActive(false);
        };
    }


    private void CreateWebview()
    {
        // Initialize UniWebView and set options
        var webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();
        UniWebView.SetAllowInlinePlay(true);
        webView.ReferenceRectTransform = webviewTransform;
    }

    public void DestroyWebview()
    {
        // Destroy the webview and release from memory
        Destroy(webView);
        webView = null;        
    }

}
