using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _inst = null;
	public static T inst
	{
		get
		{
			if (_inst == null)
			{
				if (FindObjectsOfType<T>().Length > 1)
					Debug.LogError("More than one");
				else if (FindObjectOfType<T>() != null)
					_inst = FindObjectOfType<T>();
				else
				{
					GameObject go = new GameObject();
					go.name = typeof(T).Name;
					_inst = go.AddComponent<T>();
				}
			}
			return _inst;
		}
	}

	public void SetStatic()
	{
		if (_inst != null)
			DontDestroyOnLoad(_inst.gameObject);
	}
}
