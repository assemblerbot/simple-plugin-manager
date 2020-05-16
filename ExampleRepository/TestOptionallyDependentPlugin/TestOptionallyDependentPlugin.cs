using UnityEngine;

namespace TestPlugin
{
	public class TestOptionallyDependentPluginBehaviour : MonoBehaviour
	{
		void Start()
		{
			Bar();
		}
		
		public static void Bar()
		{
			// defined in TestPlugin/plugin.json
			#if TEST_PLUGIN_INSTALLED
			TestPluginBehaviour.Foo();
			#endif
			
			Debug.Log("Bar");
		}
	}
}