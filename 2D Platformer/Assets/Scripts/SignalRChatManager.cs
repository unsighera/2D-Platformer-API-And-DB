using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class SignalRChatManager : MonoBehaviour
{
    [SerializeField] private InputField messageInput;
    [SerializeField] private Text chatText; 
    [SerializeField] private Button sendButton;

    private HubConnection _connection;
    private SynchronizationContext _unityContext;

    private void Start()
    {
        _unityContext = SynchronizationContext.Current; 

        sendButton.onClick.AddListener(SendMessage);
        messageInput.onSubmit.AddListener(_ => SendMessage());

        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5158/chatHub")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string, string>("ReceiveMessage", (user, msg) =>
        {
            _unityContext.Post(_ =>
            {
                if (chatText == null) return;

                chatText.text += $"{user}: {msg}\n";

                var scrollRect = chatText.GetComponentInParent<ScrollRect>();
                if (scrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
            }, null);
        });

        _connection.StartAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.LogError("Ошибка подключения: " + task.Exception);
            else
                Debug.Log("SignalR подключен!");
        });
    }

    public async void SendMessage()
    {
        if (string.IsNullOrEmpty(messageInput.text)) return;

        try
        {
            await _connection.InvokeAsync("SendMessage",
                $"Player {PlayerPrefs.GetInt("id")}",
                messageInput.text);

            messageInput.text = "";
            messageInput.ActivateInputField();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Ошибка отправки: " + ex.Message);
        }
    }

    private async void OnDestroy()
    {
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}