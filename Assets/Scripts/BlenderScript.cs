using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlenderScript : MonoBehaviour
{
	private float _duration = 3, _strength = 3, _randomness = 360;
	private int _vibrato = 25;
	public GameObject _liquid, _lid;
	public GameObject _prefab1, _prefab2, _prefab3;
	bool _isProcess = false, _isFirst = true;
	List<GameObject> _gameObjects = new List<GameObject>();
	List<Color> _colors = new List<Color>();
	Color _color = Vector4.one;
	public Color _targetColor;
	public TextMesh _percentText;
	public GameObject _leftButt, _rightButt, _middleButt, _winPlate;

	private void Awake()
	{
		DOTween.KillAll();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !_isProcess)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.name == "Blender") MixColors();
			}
		}
	}

	public void MixColors()
	{
		if (_gameObjects.Count != 0)
		{
			ShakeBlender();
			_liquid.SetActive(true);
			foreach (GameObject i in _gameObjects)
			{
				_colors.Add(i.GetComponent<MeshRenderer>().material.color);
				Destroy(i);
			}
			_gameObjects.Clear();
			if (_isFirst)
			{
				_color = _colors[0];
				_colors.RemoveAt(0);
				_isFirst = false;
			}
			foreach (Color i in _colors)
			{
				_color += i;
			}
			_color /= (_colors.Count + 1);
			_color.a = 1;
			_colors.Clear();
			_liquid.GetComponent<MeshRenderer>().material.color = _color;
			if (MatchColor() >= 90)
			{
				_isProcess = true;
				ShowWin();
			}
		}
	}

	public void ShowWin()
	{
		_leftButt.SetActive(false);
		_rightButt.SetActive(false);
		if (_middleButt != null) _middleButt.SetActive(false);
		_winPlate.SetActive(true);
	}

	public void LoadNextLevel()
	{
		if (SceneManager.GetActiveScene().buildIndex + 1 < 3)
			SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
		else SceneManager.LoadSceneAsync(0);
	}

	public double MatchColor()
	{
		Vector3 _target = RGBtoLAB(_targetColor);
		Vector3 _current = RGBtoLAB(_color);

		double result = System.Math.Round(System.Math.Sqrt(System.Math.Pow(_current.x - _target.x, 2) + System.Math.Pow(_current.y - _target.y, 2) + System.Math.Pow(_current.z - _target.z, 2)));
		if(result < 100) result = 100 - result;
		else result = 0;

		_percentText.text = result + "%";

		return result;
	}

	public Vector3 RGBtoLAB(Color color)
	{
		double r = (color.r > 0.04045) ? System.Math.Pow((color.r + 0.055) / (
		1 + 0.055), 2.2) : (color.r / 12.92);
		double g = (color.g > 0.04045) ? System.Math.Pow((color.g + 0.055) / (
			1 + 0.055), 2.2) : (color.g / 12.92);
		double b = (color.b > 0.04045) ? System.Math.Pow((color.b + 0.055) / (
			1 + 0.055), 2.2) : (color.b / 12.92);

		Vector3 temp = new Vector3((float)(r * 0.4124 + g * 0.3576 + b * 0.1805), (float)(r * 0.2126 + g * 0.7152 + b * 0.0722), (float)(r * 0.0193 + g * 0.1192 + b * 0.9505));

		Vector3 result = new Vector3();
		result.x = (float)(116.0 * Fxyz(temp.y / 1) - 16);
		result.y = (float)(500.0 * (Fxyz(temp.x / 0.9505f) - Fxyz(temp.y / 1)));
		result.z = (float)(200.0 * (Fxyz(temp.y / 1) - Fxyz(temp.z / 1.089f)));

		return result;
	}

	private static double Fxyz(double t)
	{
		return ((t > 0.008856) ? System.Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));
	}

	public void ShakeBlender()
	{
		transform.DOShakeRotation(_duration, _strength, _vibrato, _randomness, false);
	}

	public void SpawnLeft()
	{
		if (!_isProcess)
		{
			_isProcess = true;
			GameObject _gameObject = Instantiate(_prefab1, this.transform);
			_gameObject.transform.localPosition = new Vector3(.2f, 0, .6f);
			Sequence _mySequence = DOTween.Sequence();
			_mySequence.Append(_lid.transform.DOLocalMove(new Vector3(-.2f, .3f, 0), .5f));
			_mySequence.Append(_gameObject.transform.DOJump(new Vector3(-6.5f, 1.6f, -.35f), .5f, 1, 1.5f));
			_gameObject.GetComponent<Rigidbody>().isKinematic = false;
			_mySequence.Append(transform.DOPunchRotation(new Vector3(0, -1, 0), 1));
			_mySequence.Append(_lid.transform.DOLocalMove(new Vector3(0, .25f, 0), .5f));
			_mySequence.OnComplete(() => _isProcess = false);
			_gameObjects.Add(_gameObject);
		}
	}

	public void SpawnRight()
	{
		if (!_isProcess)
		{
			_isProcess = true;
			GameObject _gameObject = Instantiate(_prefab2, this.transform);
			_gameObjects.Add(_gameObject);
			_gameObject.transform.localPosition = new Vector3(-.2f, 0, .6f);
			Sequence _mySequence = DOTween.Sequence();
			_mySequence.Append(_lid.transform.DOLocalMove(new Vector3(-.2f, .3f, 0), .5f));
			_mySequence.Append(_gameObject.transform.DOJump(new Vector3(-6.5f, 1.6f, -.35f), .5f, 1, 1.5f));
			_gameObject.GetComponent<Rigidbody>().isKinematic = false;
			_mySequence.Append(transform.DOPunchRotation(new Vector3(0, -1, 0), 1));
			_mySequence.Append(_lid.transform.DOLocalMove(new Vector3(0, .25f, 0), .5f));
			_mySequence.OnComplete(() => _isProcess = false);
		}
	}

	public void SpawnMiddle()
	{
		if (!_isProcess)
		{
			_isProcess = true;
			GameObject _gameObject = Instantiate(_prefab3, this.transform);
			_gameObjects.Add(_gameObject);
			_gameObject.transform.localPosition = new Vector3(0, 0, .6f);
			Sequence _mySequence = DOTween.Sequence();
			_mySequence.Append(_lid.transform.DOLocalMove(new Vector3(-.2f, .3f, 0), .5f));
			_mySequence.Append(_gameObject.transform.DOJump(new Vector3(-6.5f, 1.6f, -.35f), .5f, 1, 1.5f));
			_gameObject.GetComponent<Rigidbody>().isKinematic = false;
			_mySequence.Append(transform.DOPunchRotation(new Vector3(0, -1, 0), 1));
			_mySequence.Append(_lid.transform.DOLocalMove(new Vector3(0, .25f, 0), .5f));
			_mySequence.OnComplete(() => _isProcess = false);
		}
	}
}
