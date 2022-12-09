using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] Sounds;

    private void Awake(){
        DontDestroyOnLoad(this.gameObject);

        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }
        foreach(Sound s in Sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            if(s.name.Equals("BGM")){
                s.source.loop = true;
            }
        }
        Play("BGM");
    }

    public void Play(string name){
        foreach(Sound s in Sounds){
            if(s.name.Equals("Ambient")){
                s.source.volume = Mathf.Lerp(0,s.volume,0.5f);
                s.source.loop = true;
                s.source.Play();
            }
            else if(s.name.Equals(name)){
                s.source.Play();
            }
        }
    }
}
