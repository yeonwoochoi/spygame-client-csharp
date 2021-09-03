using System;
using Domain.Network.Response;
using Event;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace Http
{
    public static class NetworkManager
    {
        private class NetworkErrorManager {}
        
        public static PseudoResponse response = null;
        public static ErrorResponse errorResponse = null;

        public static event EventHandler<AlertOccurredEventArgs> AlertOccurredEvent;

        public static void HandleResponse(
            UnityWebRequest request,
            out PseudoResponse response,
            out ErrorResponse errorResponse
        )
        {
            if (IsServerError(request))
            {
                response = null;
                errorResponse = null;
                return;
            }

            if (request.responseCode >= 200 && request.responseCode < 300)
            {
                response = PseudoResponse.JsonToResponse(request.downloadHandler.text);
                response.DeserializeAll();
                errorResponse = null;
                return;
            }

            response = null;
            errorResponse = ErrorResponse.JsonToErrorResponse(request.downloadHandler.text);
        }

        public static bool IsServerError(UnityWebRequest request)
        {
            return request.responseCode >= 500 && request.responseCode < 600;
        }

        public static void HandleServerError()
        {
            HandleError(AlertOccurredEventArgs.Builder()
                .Type(AlertType.Notice)
                .Title("Network connection error")
                .Content("There is no response from the server. Please try again later.")
                .OkHandler(() => Application.Quit(0))
                .Build()
            );
        }

        public static void HandleError(AlertOccurredEventArgs e)
        {
            e.title = "Network Error";
            EmitAlertOccurredEvent(e);
            response = null;
        }

        private static void EmitAlertOccurredEvent(AlertOccurredEventArgs e)
        {
            if (AlertOccurredEvent == null) return;
            foreach (var invocation in AlertOccurredEvent.GetInvocationList())
                invocation.DynamicInvoke(new NetworkErrorManager(), e);
        }
    }
}