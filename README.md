NConfiguration
===

Library for simple configuration management

[![Build status](https://ci.appveyor.com/api/projects/status/sxmvo3hgntuxivfj?svg=true)](https://ci.appveyor.com/project/ExM/nconfiguration)
[![NuGet Version](http://img.shields.io/nuget/v/NConfiguration.svg?style=flat)](https://www.nuget.org/packages/NConfiguration/) 
[![NuGet Downloads](http://img.shields.io/nuget/dt/NConfiguration.svg?style=flat)](https://www.nuget.org/packages/NConfiguration/)
 
Examples:
---

In your `web.config` or `app.config` make sure you have a custom section:

```xml
<configuration>
	<ExtConfigure>
		<IncludeFile Path="etc\externalConfig.xml" Search="All" Include="All" Required="true"/>
		<IncludeFile Path="etc\machineSpecific.xml" Search="All" Include="All" Required="true"/>
	</ExtConfigure>
</configuration>
```

Find and download all of the files in the parent directory.
Tracking changed included files.

```c# 
public IAppSettings LoadSettings()
{
	var path = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).FilePath;

	var loader = new SettingsLoader();
	loader.XmlFileBySection();
	var result = loader.LoadSettings(new XmlFileSettings(path, "ExtConfigure"));
	
	var fileCheckers = FileChecker.TryCreate(result.Sources).ToArray();
	new FirstChange(fileCheckers).Changed += SettingsChanged;

	return result.Joined.ToAppSettings();
}

private void SettingsChanged(object sender, EventArgs e)
{
	//TODO: reload settings or application
}
```

Watch file configuration:

```xml
	<WatchFile Mode='Auto' Delay='0:0:30' Check='Attr,Hash' />
```

* **Mode** - mode of watch from file
 * `Auto` - try create FileSystemWatcher, otherwise create delayed monitor
 * `System` - create FileSystemWatcher
 * `Time` - create delayed monitor
 * `None` - not watching, default value
* **Delay** - delay between checking of monitored file
* **Check** - what is required to check
 * `None` - only exists file and lenght
 * `Attr` - last write time and other file attributes
 * `Hash` - MD5 hash sum of reader file
 * `All` or `Attr,Hash` - check same as in `Attr` and `Hash`

Deserialization section with the name of all downloaded files.

```c# 
var configs = settings.Get<ConfigClass>("MyConfig");
```
