using System;
using System.Collections;
using System.Collections.Generic;
using Domain;
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
                Debug.Log("Get Qna");
                yield return new WaitForSeconds(5f);
                Debug.Log(QnaManager.Instance.qna.Count);
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
                Debug.Log("Get Chapter");
                foreach (var chapterInfo in PseudoChapter.Instance.chapterInfos)
                {
                    var result = "";
                    foreach (var stageInfo in chapterInfo.stageInfos)
                    {
                        result += $"{chapterInfo.chapterType}-{stageInfo.stageType} : {stageInfo.limitTime}";
                    }
                    Debug.Log(result);
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
    }
}