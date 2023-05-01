using System.Collections;
using System.Collections.Generic;
using Assets.Packages.CloudWalletUnity.Src;
using EosSharp.Core.Api.v1;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

public class WcwDesktopTest : MonoBehaviour
{
    public CloudWalletPlugin _cloudWalletPlugin;
    public string Account { get; private set; }

    private UIDocument Screen;
    private Button loginButton;
    private Button transferButton;
    private Label text;

    public void Start()
    {
        // Instantiate the WaxCloudWalletPlugin
        _cloudWalletPlugin = new GameObject(nameof(CloudWalletPlugin)).AddComponent<CloudWalletPlugin>();

        // Assign Event-Handlers/Callbacks
        _cloudWalletPlugin.OnLoggedIn += (loginEvent) =>
        {
            Account = loginEvent.Account;
            text.text = $"{loginEvent.Account} Logged In";
            Debug.Log($"{loginEvent.Account} Logged In");
        };

        _cloudWalletPlugin.OnError += (errorEvent) =>
        {
            Debug.Log($"Error: {errorEvent.Message}");
        };

        _cloudWalletPlugin.OnTransactionSigned += (signEvent) =>
        {
            text.text = $"Transaction signed: {JsonConvert.SerializeObject(signEvent.Result)}";
            Debug.Log($"Transaction signed: {JsonConvert.SerializeObject(signEvent.Result)}");
        };

        // Inititalize the WebGl binding while passign the RPC-Endpoint of your Choice
        //_cloudWalletPlugin.InitializeWebGl("https://wax.greymass.com");
        _cloudWalletPlugin.InitializeDesktop(1234, "http://127.0.0.1:1234/index.html");
        // NOTE! For other Build Targets you will need to call the related initialize Methods 
        // allowing to provide the necessary parameters needed for Mobile or Desktop-Builds

        Screen = GameObject.FindObjectOfType<UIDocument>();

        loginButton = Screen.rootVisualElement.Q<Button>("login-button");
        loginButton.clickable.clicked += Login;
        transferButton = Screen.rootVisualElement.Q<Button>("transfer-button");
        transferButton.clickable.clicked += Transfer;

        text = Screen.rootVisualElement.Q<Label>("text");
    }

    public void Login()
    {
        _cloudWalletPlugin.Login();
    }

    public void Transfer()
    {
        _cloudWalletPlugin.Sign(new Action[]
        {
            new Action()
            {
                account = "eosio.token",
                name = "transfer",
                authorization = new List<PermissionLevel>(),
                data = new Dictionary<string, object>
                {
                    { "from", Account },
                    { "to", "liquidstudio" },
                    { "quantity", "0.10000000 WAX" },
                    { "memo", "Just a test from Desktop" }
                }
            }
        });
    }
}
