using MLApiCore;
using MLDbModule;
using MLTextGenerationAPIModule.Data;

namespace MLTextGenerationAPIModule.Requests;

public class TextGenerationRequestsQueue
{
    private readonly TextGenerationApiRequestHandler _requestHandler;
    private readonly Queue<TextGenerationApiRequest> _requestsQueue = new();
    private readonly Queue<TextGenerationApiRequest> _notCompletedQueue = new();

    private bool _isMessageIsComputing;

    public TextGenerationRequestsQueue(string apiUrl)
    {
        _requestHandler = new TextGenerationApiRequestHandler(apiUrl);
    }
    
    public void AddRequestInQueue(TextGenerationApiRequest apiRequest)
    {
        _requestsQueue.Enqueue(apiRequest);
        ExecuteNextRequest();
    }
    
    private void ExecuteNextRequest()
    {
        if (!_isMessageIsComputing && _requestsQueue.Count > 0)
        {
            ComputeRequestInternal(_requestsQueue.Dequeue());
            return;
        }

        if (!_isMessageIsComputing && _notCompletedQueue.Count > 0)
            ComputeRequestInternal(_notCompletedQueue.Dequeue());
    }

    private async void ComputeRequestInternal(TextGenerationApiRequest apiRequest)
    {
        _isMessageIsComputing = true;

        string resultMessage = apiRequest.Input;

        if (!string.IsNullOrEmpty(resultMessage))
        {
            if (!apiRequest.Record.TryGet(DbRecordsDataKeywords.History, out History history))
                history = new History(apiRequest.AiCharacterData.InnerMessage);

            if (!apiRequest.Record.TryGet(DbRecordsDataKeywords.Name, out string username))
                username = apiRequest.Record.Name;
            
            history.AddPromt(resultMessage);

            var result = await _requestHandler.Send(new RequestData(apiRequest.Input, username, apiRequest.AiCharacterData, history));

            if (!string.IsNullOrEmpty(result))
            {
                history.SetModelMessage(result);
                apiRequest.OnRequestCompleted(result);
                OnComputingComplete();
            }
            else
            {
                history.RemoveLast();
                _notCompletedQueue.Enqueue(apiRequest);
                OnComputingComplete();
            }
        }
        else
        {
            apiRequest.OnRequestCompleted(apiRequest.Input);
            OnComputingComplete();
        }
    }

    private void OnComputingComplete()
    {
        _isMessageIsComputing = false;
        ExecuteNextRequest();
    }
}