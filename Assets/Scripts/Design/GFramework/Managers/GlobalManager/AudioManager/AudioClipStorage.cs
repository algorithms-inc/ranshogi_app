using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    // TODO: PreLoadを用意したい
    public class AudioClipStorage
    {
        private Dictionary<string, AudioClip> _audioClipList = new Dictionary<string, AudioClip>();


        public AudioClip LoadAudioClip(string filePathWithoutExtension, bool willCachedIfNeeded)
        {
            if (HasDataInCache(filePath: filePathWithoutExtension))
            {
                return _audioClipList[filePathWithoutExtension];
            }
            else
            {
                AudioClip resultAudioClip = (AudioClip)Resources.Load("Audio/" + filePathWithoutExtension);

                if (willCachedIfNeeded)
                {
                    _audioClipList.Add(key: filePathWithoutExtension, value: resultAudioClip);
                }

                return resultAudioClip;
            }
        }

        private bool HasDataInCache(string filePath)
        {
            return _audioClipList.ContainsKey(filePath);
        }


        public void PreLoadAudioClip()
        {
            throw new NotImplementedException();
        }


        public void ClearCache()
        {
            _audioClipList.Clear();
        }
    }
}