using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeafController : MonoBehaviour {
	public GameObject leafPrefab;
	public float leafFallRateMin;
	public float leafFallRateMax;


	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnLeaf", 1,Random.Range (leafFallRateMin,leafFallRateMax));
		SpawnLeaf();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void SpawnLeaf () {
		GameObject leaf = Instantiate(leafPrefab,  new Vector3 (transform.position.x, 400, 0), transform.rotation) as GameObject;
		leaf.transform.parent = gameObject.transform;
		leaf.transform.position = new Vector3 (Random.Range(30.0f, 640.0f), transform.position.y, 0);
		leaf.transform.rotation = Quaternion.Euler(0, 0, Random.Range (40.0f,130.0f));
		float scale = Random.Range(0.5f,1.2f);
		leaf.transform.localScale  = new Vector3(scale,scale,1);
	}
}
