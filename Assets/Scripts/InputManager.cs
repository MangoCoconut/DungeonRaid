using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	private bool bIsClick = false;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			bIsClick = true;
		}

		if (Input.GetMouseButtonUp(0))
		{
			bIsClick = false;
			GameManager.instance.boardScript.DragOut();
		}

		if (bIsClick)
		{
			Vector3 mousePos = Input.mousePosition;     // 마우스 포지션
			mousePos.z = Camera.main.nearClipPlane;      // 가까운 클리핑 평면 거리 구하기
														 // 마우스 좌표를 월드 좌표로 변환
			Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);

			//마우스 위치에 ray발사
			Ray2D ray = new Ray2D(mouseWorld, Vector2.zero);

			//마우스 위치에 ray발사
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

			if (hit.collider)
			{
				GameManager.instance.boardScript.CheckMatch(hit);
			}
		}
	}

}