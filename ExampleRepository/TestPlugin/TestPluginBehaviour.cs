using UnityEngine;

namespace TestPlugin
{
	public class TestPluginBehaviour : MonoBehaviour
	{
		void Start()
		{
			Foo();
		}

		public static void Foo()
		{
			Debug.Log("Foo");
		}
	}
}