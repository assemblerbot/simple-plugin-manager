using UnityEngine;

namespace TestPlugin
{
	public class TestDependentPluginBehaviour : MonoBehaviour
	{
		void Start()
		{
			Bar();
		}
		
		public static void Bar()
		{
			TestPluginBehaviour.Foo();
			Debug.Log("Bar");
		}
	}
}