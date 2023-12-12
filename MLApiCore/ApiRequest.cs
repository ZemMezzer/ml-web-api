using System.Net.Sockets;

namespace MLApiCore;

public class ApiRequest
{
    public Socket Sender { get; }
    public string RequestData { get; }

    public event Action<string, Socket> OnComplete;

    public ApiRequest(Socket sender, string request, Action<string, Socket> onComplete)
    {
        OnComplete = onComplete;
    }

    internal void OnRequestCompleted(string result)
    {
        OnComplete?.Invoke(result, Sender);
    }
}