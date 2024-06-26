using System.Collections;
using UnityEngine;

public class CrossPath : MonoBehaviour
{
	[SerializeField] public PathPoint pointPrefab;
	[SerializeField] public PathPoint startPathPoint;
	[SerializeField] private LineRenderer pathConnection;
	[SerializeField] public Vector2 pathPointDistances;
	[SerializeField] public CrossBall crossBall;
	[SerializeField] public float lineRendererTweenSpeed;
	[SerializeField] public BridgeZone bridgeZone;
	[SerializeField] public LineRenderer lineRendererPrefab;
	[SerializeField] public InstantTimer timer;
	private LineRenderer currentLine;

	public PathPoint nextPathPoint { get; private set; }
	public Vector2 ScreenBounds { get; private set; }

	private void Start()
	{
		currentLine = pathConnection;
		ScreenBounds = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
		nextPathPoint = startPathPoint;
	}

	public void GenerateNextPathPoint()
	{
		nextPathPoint.EnableCompletedEffects();
		bool withBridge = Random.Range(0, 1f) > 0.5f;
		timer.SetTimerCountDown(withBridge);
		StartCoroutine(GetLineRendererTween(withBridge));
	}

	public IEnumerator GetLineRendererTween(bool withBridge)
	{
		Vector3 destination;
		destination.x = Random.Range(-ScreenBounds.x + crossBall.CrossBallRendererRadius, ScreenBounds.x - crossBall.CrossBallRendererRadius);
		destination.y = nextPathPoint.transform.position.y + Random.Range(pathPointDistances.x, pathPointDistances.y);
		destination.z = 0f;
		Vector3 initialVector2 = destination - nextPathPoint.transform.position;
		Vector3 initialNormalized = initialVector2.normalized;
		Vector3 currentPointPosition = currentLine.GetPosition(currentLine.positionCount - 1);
		currentLine.positionCount++;

		while (currentPointPosition.y < destination.y)
		{
			currentPointPosition += initialNormalized * lineRendererTweenSpeed * Time.deltaTime;
			currentLine.SetPosition(currentLine.positionCount - 1, currentPointPosition);
			yield return null;
		}

		currentLine.SetPosition(currentLine.positionCount - 1, destination);
		InstantiateNextPoint(destination, withBridge);
	}

	public void InstantiateNextPoint(Vector2 position, bool withBridge)
	{
		var nextPoint = Instantiate(pointPrefab, position, Quaternion.identity, transform);

		if (withBridge)
		{
			Vector2 bridgePosition;
			bridgePosition.x = (nextPoint.transform.position.x + nextPathPoint.transform.position.x) / 2;
			bridgePosition.y = (nextPoint.transform.position.y + nextPathPoint.transform.position.y) / 2;

			var bridge = Instantiate(bridgeZone, bridgePosition, Quaternion.identity, transform);
			bridge.transform.up = nextPoint.transform.position - nextPathPoint.transform.position;
		}


		nextPathPoint = nextPoint;
	}
}
