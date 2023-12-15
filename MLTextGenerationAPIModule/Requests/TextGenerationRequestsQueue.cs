using MLApiCore;
using MLApiCore.Data;
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
        
        if (!string.IsNullOrEmpty(apiRequest.Promt))
        {
            var history = apiRequest.History;
            var username = apiRequest.Username;
            var characterData = apiRequest.CharacterData;
            var promt = apiRequest.Promt;

            history.AddPromt(promt);

            var result = await _requestHandler.Send(new RequestData(promt, username, characterData, history));

            if (!string.IsNullOrEmpty(result))
            {
                history.SetModelMessage(result);
                apiRequest.OnRequestCompleted(GenerationStatus.Success, result);
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
            apiRequest.OnRequestCompleted(GenerationStatus.EmptyPromt, string.Empty);
            OnComputingComplete();
        }
    }

    private void OnComputingComplete()
    {
        _isMessageIsComputing = false;
        ExecuteNextRequest();
    }
}