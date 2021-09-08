using System;
using System.Collections;
using System.Collections.Generic;
using Domain;
using Domain.Network.Response;
using Event;
using Http;
using Manager;
using UnityEngine;
using UnityEngine.Networking;

namespace Test
{
    public class NetworkTestManager: MonoBehaviour
    {
        private void Start()
        {
            //StartCoroutine(GetQnaData());
            StartCoroutine(GetStageInfo());
        }
        
        private IEnumerator SetStageScore(int score)
        {
            yield return new WaitForSeconds(5f);
            var currentStageInfo = ChapterManager.Instance.GetStageInfo(ChapterType.Chapter1, StageType.Stage1);
            var www = HttpFactory.Build(RequestUrlType.StageScore, score);
            yield return www.SendWebRequest();
            
            NetworkManager.HandleResponse(www, out var response, out var errorResponse);

            if (response == null && errorResponse == null)
            {
                NetworkManager.HandleServerError();
                yield break;
            }

            if (response != null)
            {
                if (currentStageInfo.score < score)
                {
                    ChapterManager.Instance.UpdateStageScore(ChapterType.Chapter1, StageType.Stage1, score);
                    Debug.Log(ChapterManager.Instance.GetStageInfo(ChapterType.Chapter1, StageType.Stage1).score);
                }
                yield break;
            }

            var errorCode = errorResponse.GetErrorCode();
            NetworkManager.HandleError(AlertOccurredEventArgs.Builder()
                .Type(AlertType.Notice)
                .Title("Error while saving stage score")
                .Content(errorCode.message)
                .OkHandler(() => { Application.Quit(0); })
                .Build()
            );
        }

        private IEnumerator GetQnaData()
        {
            var www = HttpFactory.Build(RequestUrlType.Qna);
            yield return www.SendWebRequest();
            
            NetworkManager.HandleResponse(www, out var response, out var errorResponse);

            if (response == null && errorResponse == null)
            {
                NetworkManager.HandleServerError();
                yield break;
            }

            if (response != null)
            {
                foreach (var qna in QnaManager.Instance.qna)
                {
                    Debug.Log($"{qna.difficulty} / {qna.question}");
                }
                yield break;
            }

            var code = errorResponse.GetErrorCode();
            NetworkManager.HandleError(AlertOccurredEventArgs.Builder()
                .Type(AlertType.Notice)
                .Title("Tlqkf")
                .Content(code.message)
                .OkHandler(() => Application.Quit(0))
                .Build()
            );
        }

        private IEnumerator GetStageInfo()
        {
            var www = HttpFactory.Build(RequestUrlType.ChapterInfo);
            yield return www.SendWebRequest();
            
            NetworkManager.HandleResponse(www, out var response, out var errorResponse);
            
            if (response == null && errorResponse == null)
            {
                NetworkManager.HandleServerError();
                yield break;
            }

            if (response != null)
            {
                yield return StartCoroutine(SetStageScore(1));
                yield break;
            }

            var code = errorResponse.GetErrorCode();
            NetworkManager.HandleError(AlertOccurredEventArgs.Builder()
                .Type(AlertType.Notice)
                .Title("Tlqkf")
                .Content(code.message)
                .OkHandler(() => Application.Quit(0))
                .Build()
            );
        }
    }
}