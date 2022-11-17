using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Data;
using SpeechRecognitionSystem;
using UnityEngine;
using UnityEngine.Android;

namespace DoubTech.SRS
{
    public class SRSAudioBufferAudioRecorder : MonoBehaviour, IAudioProvider
    {
    private List<float> _data = new List<float>();
    
    public float Frequency {
        get {
            return AudioBuffer.Instance.AudioEncoding.samplerate;
        }
    }

    public float[ ] GetData( )
    {
        float[] result;
        lock (_data)
        {
            result = _data.ToArray();
            _data.Clear();
        }
        return result;
    }

    public AudioReadyEvent MicReady = new AudioReadyEvent( );

    private void Awake( ) {
        if ( Application.platform == RuntimePlatform.Android ) {
            if ( !Permission.HasUserAuthorizedPermission( Permission.Microphone ) ) {
                Permission.RequestUserPermission( Permission.Microphone );
            }
        }
    }

    private void OnEnable()
    {
        AudioBuffer.Instance.Events.OnFloatDataReady.AddListener(OnFloatDataReady);
        MicReady.Invoke(this);
    }

    private void OnDisable()
    {
        AudioBuffer.Instance.Events.OnFloatDataReady.RemoveListener(OnFloatDataReady);
    }

    private void OnFloatDataReady(int arg0, float[] data, float maxLevel)
    {
        lock (_data)
        {
            _data.AddRange(data);
            while (_data.Count > 16000) _data.Remove(0);
        }
    }
    }
}