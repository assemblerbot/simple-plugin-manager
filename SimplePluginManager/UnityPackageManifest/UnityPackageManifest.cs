using System;
using System.Collections.Generic;

namespace SimplePluginManager
{
	[Serializable]
	public class UnityPackageManifest
	{
		public string                     name;
		public string                     displayName;
		public string                     version;
		public string                     description;
		public List<UnityPackageManifestDependency> dependencies;
	}
}