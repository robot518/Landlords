using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Type = PlayCardControl.CardGroupType;

public class AudioMgr : MonoBehaviour {
	static GameObject go;
	static AudioMgr adMgr;
	//音乐文件
	public AudioSource music;
	public AudioSource sound;

	public static AudioMgr getInstance() {
		if (adMgr == null)
			adMgr = go.GetComponent<AudioMgr> ();
		return adMgr;
	}

	void Awake() {
		go = gameObject;
	}

	public void PlayCardSound(Type cardType, int iNum){
		if (iNum == 14 || iNum == 15)
			iNum -= 13;
		string str = "";
		switch (cardType) {
		case Type.cgSINGLE:
			if (iNum < 10) {
				str = "10";
			} else {
				str = "1";
			}
			str += iNum.ToString ();
			break;
		case Type.cgDOUBLE:
			if (iNum < 10) {
				str = "20";
			} else {
				str = "2";
			}
			str += iNum.ToString ();
			break;
		case Type.cgTHREE:
			if (iNum < 10) {
				str = "30";
			} else {
				str = "3";
			}
			str += iNum.ToString ();
			break;
		case Type.cgSINGLE_LINE:
			str = "shunzi";
			break;
		case Type.cgDOUBLE_LINE:
			str = "liandui";
			break;
		case Type.cgTHREE_LINE:
			str = "feiji";
			break;
		case Type.cgTHREE_TAKE_ONE:
			str = "sandaiyi";
			break;
		case Type.cgTHREE_TAKE_TWO:
			str = "sandaiyidui";
			break;
		case Type.cgTHREE_TAKE_ONE_LINE:
			str = "feiji";
			break;
		case Type.cgTHREE_TAKE_TWO_LINE:
			str = "feiji";
			break;
		case Type.cgFOUR_TAKE_ONE:
			str = "sidaier";
			break;
		case Type.cgFOUR_TAKE_TWO:
			str = "sidailiangdui";
			break;
		case Type.cgBOMB_CARD:
			str = "zhadan";
			break;
		case Type.cgKING_CARD:
			str = "wangzha";
			break;
		}
//		print ("str = " + str);
		AudioClip clip = Resources.Load ("Sound/Boy/" + str) as AudioClip;
		sound.PlayOneShot(clip);
	}

	public void PlaySound(string soundName){
		AudioClip clip = Resources.Load ("Sound/Main/" + soundName) as AudioClip;
		sound.PlayOneShot(clip);
	}

	public void PlayMusic(string str){
		AudioClip clip = Resources.Load ("Music/" + str) as AudioClip;
		music.clip = clip;
		music.Play();
	}
}
