NConfiguration
===

Library for simple configuration management

Source code:
 git://github.com/ExM/NConfiguration.git

 
Examples:
---

In your `web.config` or `app.config` make sure you have a custom section:

```xml
<configuration>
	<configSections>
		<section name="ExtConfigure" type="NConfiguration.PlainXmlSection, NConfiguration"/>
	</configSections>
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
	var loader = new SettingsLoader();
	loader.XmlFileBySection();
	var result = loader.LoadSettings(new XmlSystemSettings("ExtConfigure"));
	result.Changed += SettingsChanged;
	return result;
}

private void SettingsChanged(object sender, EventArgs e)
{
	//TODO: reload settings or application
}
```

Deserialization section with the name of all downloaded files.

```c# 
var configs = settings.Get<ConfigClass>("MyConfig");
```
