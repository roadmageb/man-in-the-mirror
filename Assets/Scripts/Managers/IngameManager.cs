using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : SingletonBehaviour<IngameManager>
{
	private int _briefcaseCount = 0;
    public int BriefcaseCount {
		get { return _briefcaseCount; }
		set {
			for (int i = 0; i < value - _briefcaseCount; ++i)
			{
				IngameUIManager.inst.AddBriefcaseUI();
			}
			for (int i = 0; i < _briefcaseCount - value; ++i)
			{
				//RemoveBriefcaseUI();
			}
			_briefcaseCount = value;
		}
	}
}
