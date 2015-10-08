﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {
	private Game game;
	public GameObject pathPrefab;

	private GameObject selector;
	private PathRenderer pathRenderer;
	private List<Vector2> path = new List<Vector2>();

	private bool moving = false;
	

	void Awake () {
		game = GameObject.Find("Game").GetComponent<Game>();
		selector = transform.Find("Selector").gameObject;
		CreatePathRenderer();
	}


	private void CreatePathRenderer () {
		GameObject obj = (GameObject)Instantiate(pathPrefab);
		obj.transform.SetParent(game.containers.fx.transform);
		pathRenderer  = obj.GetComponent<PathRenderer>();
	}


	public void SetPath (Vector3 pos) {
		if (moving) { return; }

		if (path != null && path.Count > 0) {
			Vector3 goal = new Vector3(path[path.Count -1].x, 0, path[path.Count -1].y);
			if (pos == goal) {
				FollowPath();
				return;
			}
		}

		path = Grid.astar.SearchPath(
			(int)transform.localPosition.x, 
			(int)transform.localPosition.z,
			(int)pos.x, 
			(int)pos.z
		);

		path.RemoveAt(0);

		pathRenderer.CreatePath(path);
	}


	public void FollowPath () {
		if (path == null) { return; }
		StartCoroutine(FollowPathAnim());
	}


	private IEnumerator FollowPathAnim () {
		moving = true;

		while (path.Count > 0) {
			Vector3 point = new Vector3(path[0].x, 0, path[0].y);
			yield return StartCoroutine(MoveToPos(point, 0.2f));

			path.RemoveAt(0);
			pathRenderer.CreatePath(path);
		}

		path = null;
		moving = false;
	}


	private IEnumerator MoveToPos(Vector3 endPos, float duration) {
		Vector3 startPos = transform.localPosition;
		float startTime = Time.time;

		while(Time.time < startTime + duration) {
			transform.position = Vector3.Lerp(startPos, endPos, (Time.time - startTime) / duration);
			yield return null;
		}

		transform.position = endPos;
	}

}
