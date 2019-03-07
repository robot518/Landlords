using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardControl : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler {
	IMain _delt;
	int _idx;
	const int DY = 30;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void init(IMain delt, int idx){
		_delt = delt;
		_idx = idx;
	}

	public void OnPointerDown(PointerEventData eventData){
		_delt.setIDown (_idx);
		showDown ();
	}

	public void OnPointerEnter(PointerEventData eventData){
		_delt.onEnter (_idx);
	}

	public void OnPointerUp(PointerEventData eventData){
		_delt.onUpShow ();
	}

	public void showDown(){
		var f = 200.0f / 255;
		transform.GetComponent<Image> ().color = new Color (f, f, f, 1);
	}

	public void resetColor(){
		transform.GetComponent<Image>().color = Color.white;
	}

	public void showUp(){
		var px = transform.localPosition.x;
		var py = _delt.getPosY ();
		if (transform.localPosition.y == py)
			transform.localPosition = new Vector3 (px, py + DY, 0);
		else
			transform.localPosition = new Vector3 (px, py, 0);
		resetColor ();
	}
}
