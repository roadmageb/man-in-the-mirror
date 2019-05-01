using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : SingletonBehaviour<IngameUIManager>
{

	public RectTransform briefcaseGrid;

	public GameObject briefcaseUIPrefab;

	public void AddBriefcaseUI()
	{
		Instantiate(briefcaseUIPrefab, briefcaseGrid);
	}

	public void UpdateBriefcaseUI()
	{
		//TODO : Add briefcase Marker
	}
}
