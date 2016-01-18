using UnityEngine;
using System.Collections;

public class MinableResource : MonoBehaviour {

	public int RocksAvailable = 100;

	public int RocksMinedPerClick = 20;

	// mines this rock formation
	public int Mine()
	{
		if (RocksAvailable > 0)
		{
			RocksAvailable -= RocksMinedPerClick;

			if (RocksAvailable <= 0)
			{
				iTween.ColorTo(gameObject, iTween.Hash("color", new Color(0, 0, 0, 0), "time", 0.5f, "oncomplete", "DestroyMine", "oncompletetarget", gameObject));
			}

			return RocksMinedPerClick;
		}
		return 0;
	}

	public void DestroyMine()
	{
		Destroy(gameObject);
	}
}
