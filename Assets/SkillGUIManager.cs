using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SkillGUIManager : MonoBehaviour {

	public Image[] UI;
	public static SkillGUIManager Singleton = null;

	public Sprite[] Graphics;
	/*
	{
		Fire,
		Water,
		Rock,
		Air,
		Light,
		Frost,
		Shadow,
		Lightning,
		Life,
		Arcane
	}
	 */

	void Awake(){
		Singleton = this;
		DisableGraphic ();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DisableGraphic(int Index = -1){
		if (Index != -1) {
			UI[Index].enabled = false;
			return;
		}

		for (int i=0; i < UI.Length; i++) {
			if(!UI[i].enabled)
				break;

			UI [i].enabled = false;
		}
	}

	public void UpdateGraphics(List<SpellTypes> spells){
		DisableGraphic ();

		for (int i=0; i < spells.Count; i++) {
			UI[i].enabled = true;

			switch(spells[i]){
				case SpellTypes.FIRE:
				case SpellTypes.FIRE1:
				case SpellTypes.FIRE2:
					UI[i].sprite = Graphics[0];
					break;

				case SpellTypes.WATER:
				case SpellTypes.WATER1:
				case SpellTypes.WATER2:
					UI[i].sprite = Graphics[1];
					break;

				case SpellTypes.ROCK:
				case SpellTypes.ROCK1:
				case SpellTypes.ROCK2:
					UI[i].sprite = Graphics[2];
					break;

				case SpellTypes.AIR:
				case SpellTypes.AIR1:
				case SpellTypes.AIR2:
					UI[i].sprite = Graphics[3];
					break;
			
				case SpellTypes.LIGHT:
				case SpellTypes.LIGHT1:
				case SpellTypes.LIGHT2:
					UI[i].sprite = Graphics[4];
					break;

				case SpellTypes.FROST:
				case SpellTypes.FROST1:
				case SpellTypes.FROST2:
					UI[i].sprite = Graphics[5];
					break;

				case SpellTypes.SHADOW:
				case SpellTypes.SHADOW1:
				case SpellTypes.SHADOW2:
					UI[i].sprite = Graphics[6];
					break;

				case SpellTypes.LIGHTNING:
				case SpellTypes.LIGHTNING1:
				case SpellTypes.LIGHTNING2:
					UI[i].sprite = Graphics[7];
					break;

				case SpellTypes.LIFE:
				case SpellTypes.LIFE1:
				case SpellTypes.LIFE2:
					UI[i].sprite = Graphics[8];
					break;

				case SpellTypes.ARCANE:
				case SpellTypes.ARCANE1:
				case SpellTypes.ARCANE2:
					UI[i].sprite = Graphics[9];
					break;
				default:
					break;

			}
		}

	}
}
